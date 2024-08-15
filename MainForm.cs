using System.Text.RegularExpressions;
using Renci.SshNet;

namespace DDoS_Autofix
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private SshClient? ssh;
        private ShellStream stream;
        private bool platform;
        private bool busy, terminationRequest = false;
        private readonly List<string> entries = [];
        public MainForm()
        {
            InitializeComponent();
        }
        private async Task DownloadBlocklist()
        {
            const string url = "https://raw.githubusercontent.com/JeniCzech92/ddos-blocklist/main/blocklist.txt";
            const string filePath = "blocklist.txt";

            using var httpClient = new HttpClient();
            try
            {
                // Download the file content as a string
                string content = await httpClient.GetStringAsync(url);

                // Write the content to a file
                await File.WriteAllTextAsync(filePath, content);

                // Check if the file was created successfully
                if (File.Exists(filePath))
                {
                    entries.AddRange(await File.ReadAllLinesAsync(filePath));
                    UpdateLog($"Ready! Please connect to the appliance.\r\nBlocklist file loaded. Added {entries.Count} entries.");
                }
                else
                {
                    UpdateLog("Blocklist file not found.");
                    btnConnect.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"Download of blocklist failed: {ex.Message}");
                btnConnect.Enabled = false;
            }
        }
        public void UpdateLog(string msg)
        {
            tbLog.Text = msg + "\r\n" + tbLog.Text;
            tbLog.Select(tbLog.TextLength, 0);
        }
        public void FormHandler(int a)
        {
            switch (a)
            {
                case 0:
                    busy = false;
                    tbAddress.Enabled = true;
                    tbAccount.Enabled = true;
                    tbPassword.Enabled = true;
                    nudPort.Enabled = true;
                    btnConnect.Enabled = true;
                    btnApply.Enabled = false;
                    btnRemove.Enabled = false;
                    btnDisconnect.Enabled = false;
                    break;
                case 1:
                    busy = false;
                    tbAddress.Enabled = false;
                    tbAccount.Enabled = false;
                    tbPassword.Enabled = false;
                    nudPort.Enabled = false;
                    btnConnect.Enabled = false;
                    btnApply.Enabled = true;
                    btnRemove.Enabled = true;
                    btnDisconnect.Enabled = true;
                    break;
                case 2:
                    busy = true;
                    tbAddress.Enabled = false;
                    tbAccount.Enabled = false;
                    tbPassword.Enabled = false;
                    nudPort.Enabled = false;
                    btnConnect.Enabled = false;
                    btnApply.Enabled = false;
                    btnRemove.Enabled = false;
                    btnDisconnect.Enabled = false;
                    break;
            }
        }
        private void SelectAllText(object? sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
            else if (sender is NumericUpDown n)
            {
                // Ensure that `n.Text` is not null before accessing `Length`
                var text = n.Text ?? string.Empty;
                n.Select(0, text.Length);
            }
        }
        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            bool abort = false;
            if (!r_FQDNValidation().Match(tbAddress.Text).Success && !r_IPValidation().Match(tbAddress.Text).Success)
            {
                MessageBox.Show("Please check the hostname.", "Hostname not valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                abort = true;
            }
            if (!r_UsernameValidation().Match(tbAccount.Text).Success)
            {
                MessageBox.Show("Please check the username.", "Username not valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                abort = true;
            }
            if (!abort)
            {
                UpdateLog("Connecting...");
                FormHandler(2);
                ssh = new SshClient(tbAddress.Text, Convert.ToInt32(nudPort.Value), tbAccount.Text, tbPassword.Text);
                tbPassword.Text = "**********************";
                Task sshconnect = ssh.ConnectAsync(CancellationToken.None);
                try
                {
                    await sshconnect;
                    stream = ssh.CreateShellStream("xterm", 80, 600, 0, 0, 32 * 1024);
                    platform = await stream.GetPlatform();
                    if (await stream.CheckPermissions(platform))
                    {
                        if (platform)
                        {
                            await stream.Execute("edit running");
                            await stream.Execute("cliconfig pager enabled false");
                        }
                        else await stream.Execute("configure terminal");
                        FormHandler(1);
                        UpdateLog("Connected succesfully...");
                    }
                    else
                    {
                        UpdateLog("Insufficient privileges, disconnecting...");
                        ssh.Disconnect();
                        tbPassword.Text = "";
                        FormHandler(0);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2146232798)
                    {
                        UpdateLog("Insufficient privileges, disconnecting...");
                    }
                    else
                    {
                        UpdateLog("An error occured while connecting to host: " + ex.Message + "\r\n");
                    }
                    ssh.Disconnect();
                    tbPassword.Text = "";
                    FormHandler(0);
                }
            }
        }
        private async void BtnDisconnect_Click(object sender, EventArgs e)
        {
            FormHandler(2);
            if (platform)
            {
                await stream.Execute("exit", false);
                await stream.Execute("exit", false);
            }
            else
            {
                await stream.Execute("exit");
                await stream.Execute("exit");
            }
            ssh?.Disconnect();
            UpdateLog("Disconnected...");
            tbPassword.Text = "";
            FormHandler(0);
        }
        private async void BtnApplyPolicies_Click(object sender, EventArgs e)
        {

            UpdateLog("Applying...");
            progressBar.Visible = true;
            progressBar.Maximum = entries.Count * 2;
            FormHandler(2);
            if (platform)
            {
                foreach (string entry in entries)
                {
                    if (entry.Contains('/')) await stream.Execute("object address-object address _autofix_" + entry.Replace("/", "_") + " type cidr " + entry);
                    else await stream.Execute("object address-object address _autofix_" + " type host " + entry);
                    progressBar.Value++;
                }
                await stream.Execute("object address-object group _autofix");
                foreach (string entry in entries)
                {
                    await stream.Execute("address-list _autofix_" + entry.Replace('/', '_'));
                    progressBar.Value++;
                }
                UpdateLog("Entries updated...");
                await stream.Execute("/");
                string str = await stream.Execute("show config vrf main secure-policy rule _autofix");
                if (str == "show config vrf main secure-policy rule _autofix\r\ngw running config# ")
                {
                    UpdateLog("Adding policy control rule...");
                    progressBar.Style = ProgressBarStyle.Marquee;
                    //await stream.UOSAddRule("_autofix"); not using this now...
                    await stream.Execute("vrf main secure-policy rule _autofix");
                    await stream.Execute("action deny");
                    await stream.Execute("source-ip _autofix");
                    await stream.Execute("from WAN");
                    await stream.Execute("commit");
                    UpdateLog("ATTENTION: For uOS devices, please move the policy control rule to highest priority manually within the WebGUI!");
                    //but the above instead...
                    progressBar.Style = ProgressBarStyle.Continuous;
                }
                else await stream.Execute("commit");
                UpdateLog("Done...");
            }
            else
            {
                foreach (string entry in entries)
                {
                    await stream.Execute("address-object _autofix_" + entry.Replace('/', '_') + " " + entry);
                    progressBar.Value++;
                    progressBar.Value++;
                }
                await stream.Execute("object-group address _autofix");
                foreach (string entry in entries)
                {
                    await stream.Execute("address-object _autofix_" + entry.Replace('/', '_'));
                }
                await stream.Execute("exit");
                UpdateLog("Entries updated...");
                string str = await stream.Execute("show secure-policy");
                if (r_PolicyRuleCheckIfExists().IsMatch(str))
                {
                    UpdateLog("Relevant security policy already exists, skipping...");
                    if (!r_PolicyRuleCheckIfFirst().IsMatch(str)) UpdateLog("NOTICE: The _autofix policy rule does not have the highest priority! If this is not intentional, please adjust your policy control settings!");
                }
                else
                {
                    await stream.Execute("secure-policy insert 1");
                    await stream.Execute("name _autofix");
                    await stream.Execute("from WAN");
                    await stream.Execute("sourceip _autofix");
                    await stream.Execute("action deny");
                    await stream.Execute("exit");
                    UpdateLog("Blocking security policy created on index #1...");
                }
            }
            progressBar.Visible = false;
            progressBar.Value = 0;
            FormHandler(1);
        }
        private async void BtnRemovePolicies_Click(object sender, EventArgs e)
        {
            FormHandler(2);
            string str;
            UpdateLog("Removing...");
            progressBar.Visible = true;
            if (platform)
            {
                await stream.Execute("vrf main secure-policy");
                await stream.Execute("del rule _autofix");
                await stream.Execute("/");
                await stream.Execute("object address-object");
                await stream.Execute("del group _autofix");
                await stream.Execute("/");
                str = await stream.Execute("show config object address-object address");
                MatchCollection delete = r_AddressObjects().Matches(str);
                await stream.Execute("object address-object");
                progressBar.Maximum = delete.Count;
                foreach (Match match in delete)
                {
                    await stream.Execute("del address " + match);
                    progressBar.Value++;
                }
                await stream.Execute("/");
                await stream.Execute("commit");
                UpdateLog("Removed " + delete.Count.ToString() + " entries...");

            }
            else
            {
                str = await stream.Execute("show secure-policy");
                Match m = r_PolicyRuleFindIndex().Match(str);
                if (m.Success)
                {
                    str = r_PolicyGetIndex().Match(m.Value).Value;
                    await stream.Execute("no secure-policy " + str);
                    UpdateLog("Removed relevant security policy on index #" + str + "...");
                }
                else
                    UpdateLog("No relevant security policy found, skipping...");
                await stream.Execute("no object-group address _autofix");
                str = await stream.Execute("show address-object");
                MatchCollection delete = r_AddressObjects().Matches(str);
                progressBar.Maximum = delete.Count;
                foreach (Match match in delete)
                {
                    await stream.Execute("no address-object " + match);
                    progressBar.Value++;
                }
                UpdateLog("Removed " + delete.Count.ToString() + " entries...");
            }
            progressBar.Visible = false;
            progressBar.Value = 0;
            FormHandler(1);
        }
        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!terminationRequest && busy)
            {
                terminationRequest = true;
                e.Cancel = true;
                UpdateLog("Termination request received, will close once the current task is finished...");
                while (busy)
                {
                    await Task.Delay(100);
                }
                this.Close();
            }
            else if (terminationRequest && busy) e.Cancel = true;
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            await DownloadBlocklist();
        }

        [GeneratedRegex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$")]
        private static partial Regex r_FQDNValidation();
        [GeneratedRegex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$")]
        private static partial Regex r_IPValidation();
        [GeneratedRegex(@"^[a-zA-Z@._-][a-zA-Z0-9@._-]+$")]
        private static partial Regex r_UsernameValidation();
        [GeneratedRegex("name: _autofix")]
        private static partial Regex r_PolicyRuleCheckIfExists();
        [GeneratedRegex(@"secure-policy rule:\s*1\s*name:\s*_autofix", RegexOptions.Multiline)]
        private static partial Regex r_PolicyRuleCheckIfFirst();
        [GeneratedRegex(@"secure-policy rule:\s*(\d+)\s*name:\s*_autofix", RegexOptions.Multiline)]
        private static partial Regex r_PolicyRuleFindIndex();
        [GeneratedRegex("(\\d+)")]
        private static partial Regex r_PolicyGetIndex();
        [GeneratedRegex(@"_autofix_\S+")]
        private static partial Regex r_AddressObjects();
    }
    public static partial class Extensions
    {
        public static async Task<bool> GetPlatform(this ShellStream stream)
        {
            var task = Task.Run(() => stream.Expect("> "));
            await task;
            string str = await stream.Execute("show version", false);
            if (r_VersionCheck().IsMatch(str))
                return false;
            else
                return true;
        }
        public static async Task<bool> CheckPermissions(this ShellStream stream, bool platform)
        {
            if (platform)
            {
                string str = await stream.Execute("show config object user-object admin", false);
                if (r_PermissionCheck().IsMatch(str))
                    return false;
                else
                    return true;
            }
            else
            {
                string str = await stream.Execute("show running-config", false); //may be risky...
                if (r_PermissionCheck().IsMatch(str))
                    return false;
                else
                    return true;
            }
        }
        /*public static async Task UOSAddRule(this ShellStream stream, string ruleName)
        {
            await stream.Execute("/");
            string str = await stream.Execute("show config vrf main secure-policy rule");
            List<string[]> rules = [];
            var matches = r_GetPolicyControlRules().Matches(str);

            foreach (Match match in matches)
            {
                string ruleName2 = r_GetRuleName().Match(match.Value).Groups[1].Value;
                string user = r_GetUser().Match(match.Value).Groups[1].Value;
                string schedule = r_GetSchedule().Match(match.Value).Groups[1].Value;
                string from = r_GetZoneSrc().Match(match.Value).Groups[1].Value;
                string sourceIp = r_GetIPSrc().Match(match.Value).Groups[1].Value;
                string to = r_GetZoneDst().Match(match.Value).Groups[1].Value;
                string destinationIp = r_GetIPDst().Match(match.Value).Groups[1].Value;
                string service = r_GetSvc().Match(match.Value).Groups[1].Value;
                string action = r_GetAction().Match(match.Value).Groups[1].Value;
                string logging = r_GetLogging().Match(match.Value).Groups[1].Value;
                string contentFilterProfile = r_GetCFProfile().Match(match.Value).Groups[1].Value.Trim();
                string sslInspectionProfile = r_GetSSLProfile().Match(match.Value).Groups[1].Value.Trim();
                string appPatrolProfile = r_GetAPPProfile().Match(match.Value).Groups[1].Value.Trim();
                string description = r_GetDesc().Match(match.Value).Groups[1].Value;
                string enabled = r_GetEnabled().Match(match.Value).Groups[1].Value;
                string[] vals = [ruleName2, user, schedule, from, sourceIp, to, destinationIp, service, action, logging, contentFilterProfile, sslInspectionProfile, appPatrolProfile, description, enabled];
                rules.Add(vals);
            }
            await stream.Execute("vrf main secure-policy");
            foreach (string[] rule in rules) await stream.Execute("del rule " + rule[0]);
            await stream.Execute("/");
            await stream.Execute("vrf main secure-policy rule "+ruleName);
            await stream.Execute("action deny");
            await stream.Execute("source-ip " + ruleName);
            await stream.Execute("from WAN");
            await stream.Execute("..");
            await stream.Execute("enabled false"); //phew... daredevil...
            await stream.Execute("commit");
            foreach (string[] rule in rules)
            {
                await stream.Execute("rule " + rule[0]);
                await stream.Execute("user " + rule[1]);
                await stream.Execute("schedule " + rule[2]);
                await stream.Execute("from " + rule[3]);
                await stream.Execute("source-ip " + rule[4]);
                await stream.Execute("to " + rule[5]);
                await stream.Execute("destination-ip " + rule[6]);
                await stream.Execute("service " + rule[7]);
                await stream.Execute("action " + rule[8]);
                await stream.Execute("logging " + rule[9]);
                await stream.Execute("content-filter-profile " + rule[10]);
                await stream.Execute("ssl-inspection-profile " + rule[11]);
                await stream.Execute("app-patrol-profile " + rule[12]);
                await stream.Execute("description " + rule[13]);
                await stream.Execute("enabled " + rule[14]);
                await stream.Execute("..");
            }
            await stream.Execute("enabled true");
            await stream.Execute("commit");
            await stream.Execute("/");
        }*/
        public static async Task<string> Execute(this ShellStream stream, string command, bool superuser = true)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(command);
            stream.WriteLine(command);
            string prompt = superuser ? "# " : "> ";
            string? result = await Task.Run(() => stream.Expect(prompt));
            return result ?? string.Empty;
        }

        [GeneratedRegex("Zyxel Communications Corp")]
        private static partial Regex r_VersionCheck();
        [GeneratedRegex("ERROR: ")]
        private static partial Regex r_PermissionCheck();
        /*[GeneratedRegex(@"(?s)(rule .*?^\s*\.\.)", RegexOptions.Multiline)]
        private static partial Regex r_GetPolicyControlRules();
        [GeneratedRegex(@"rule\s+(\S+)")]
        private static partial Regex r_GetRuleName();
        [GeneratedRegex(@"user\s+(\S+)")]
        private static partial Regex r_GetUser();
        [GeneratedRegex(@"schedule\s+(\S+)")]
        private static partial Regex r_GetSchedule();
        [GeneratedRegex(@"from\s+(\S+)")]
        private static partial Regex r_GetZoneSrc();
        [GeneratedRegex(@"source-ip\s+(\S+)")]
        private static partial Regex r_GetIPSrc();
        [GeneratedRegex(@"to\s+(\S+)")]
        private static partial Regex r_GetZoneDst();
        [GeneratedRegex(@"destination-ip\s+(\S+)")]
        private static partial Regex r_GetIPDst();
        [GeneratedRegex(@"service\s+(\S+)")]
        private static partial Regex r_GetSvc();
        [GeneratedRegex(@"action\s+(\S+)")]
        private static partial Regex r_GetAction();
        [GeneratedRegex(@"logging\s+(\S+)")]
        private static partial Regex r_GetLogging();
        [GeneratedRegex(@"content-filter-profile\s+(.+)")]
        private static partial Regex r_GetCFProfile();
        [GeneratedRegex(@"ssl-inspection-profile\s+(.+)")]
        private static partial Regex r_GetSSLProfile();
        [GeneratedRegex(@"app-patrol-profile\s+(.+)")]
        private static partial Regex r_GetAPPProfile();
        [GeneratedRegex(@"description\s+(\S+)")]
        private static partial Regex r_GetDesc();
        [GeneratedRegex(@"enabled\s+(\S+)")]
        private static partial Regex r_GetEnabled();*/
    }
}