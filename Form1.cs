using System.Formats.Asn1;
using System.Net;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Windows.Foundation.Collections;
using Windows.System.Update;

namespace DDoS_Autofix
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        SshClient ssh;
        ShellStream stream;
        bool platform;
        List<string> entries = new List<string>();
        public Form1()
        {
            InitializeComponent();
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile("https://raw.githubusercontent.com/JeniCzech92/ddos-blocklist/main/blocklist.txt", "blocklist.txt"); //currently hardcoded, might rework later if needed...
                }
                catch (Exception ex)
                {
                    UpdateLog("Download of blocklist failed: " + ex.Message);
                }
                if (File.Exists("blocklist.txt"))
                {
                    entries.AddRange(File.ReadAllLines("blocklist.txt"));
                    UpdateLog("Blocklist file loaded. Added " + entries.Count.ToString() + " entries.\r\nReady! Please connect to the appliance.");
                }
                else
                {
                    UpdateLog("blocklist file not found.");
                    button1.Enabled = false;
                }
            }
        }
        public void UpdateLog(string msg)
        {
            textBox4.Text = msg + "\r\n" + textBox4.Text;
            textBox4.Select(textBox4.TextLength, 0);
        }
        public void FormHandler(int a)
        {
            switch (a)
            {
                case 0:
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    numericUpDown1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
                case 1:
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    numericUpDown1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    break;
                case 2:
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    numericUpDown1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
            }
        }


        private void SelectAllText(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = sender as TextBox;
                tb.SelectAll();
            }
            if (sender is NumericUpDown)
            {
                NumericUpDown n = sender as NumericUpDown;
                n.Select(0, n.Text.Length);
            }

        }
        private async void button1_Click(object sender, EventArgs e)
        {
            bool abort = false;
            if (!Regex.Match(textBox1.Text, @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$").Success && !Regex.Match(textBox1.Text, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$").Success)
            {
                MessageBox.Show("Please check the hostname.", "Hostname not valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                abort = true;
            }
            if (!Regex.Match(textBox2.Text, @"^[a-zA-Z@._-][a-zA-Z0-9@._-]+$").Success)
            {
                MessageBox.Show("Please check the username.", "Username not valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                abort = true;
            }
            if (!abort)
            {
                UpdateLog("Connecting...");
                FormHandler(2);
                ssh = new SshClient(textBox1.Text, Convert.ToInt32(numericUpDown1.Value), textBox2.Text, textBox3.Text);
                textBox3.Text = "**********************";
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
                        textBox3.Text = "";
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
                    textBox3.Text = "";
                    FormHandler(0);
                }
            }
        }
        private async void button4_Click(object sender, EventArgs e)
        {
            FormHandler(2);
            if (platform)
            {
                await stream.Execute("exit",false);
                await stream.Execute("exit",false);
            }
            else
            {
                await stream.Execute("exit");
                await stream.Execute("exit");
            }
            ssh.Disconnect();
            UpdateLog("Disconnected...");
            textBox3.Text = "";
            FormHandler(0);
        }
        private async void button2_Click(object sender, EventArgs e)
        {

            UpdateLog("Applying...");
            progressBar1.Visible = true;
            progressBar1.Maximum = entries.Count*2;
            FormHandler(2);
            if (platform)
            {
                foreach (string entry in entries)
                {
                    if (entry.Contains("/")) await stream.Execute("object address-object address _autofix_" + entry.Replace("/", "_") + " type cidr " + entry);
                    else await stream.Execute("object address-object address _autofix_" + " type host " + entry);
                    progressBar1.Value++;
                }
                await stream.Execute("object address-object group _autofix");
                foreach (string entry in entries)
                {
                    await stream.Execute("address-list _autofix_" + entry.Replace('/', '_'));
                    progressBar1.Value++;
                }
                UpdateLog("Entries updated..."); //TODO: could be more verbose...
                await stream.Execute("/");
                string str = await stream.Execute("show config vrf main secure-policy rule _autofix");
                if (str == "show config vrf main secure-policy rule _autofix\r\ngw running config# ") await stream.uOSAddRule("_autofix");
                await stream.Execute("commit");
            }
            else
            {
                foreach (string entry in entries)
                {
                    await stream.Execute("address-object _autofix_" + entry.Replace('/', '_') + " " + entry);
                    progressBar1.Value++;
                    progressBar1.Value++;
                }
                await stream.Execute("object-group address _autofix");
                foreach (string entry in entries)
                {
                    await stream.Execute("address-object _autofix_" + entry.Replace('/', '_'));
                }
                await stream.Execute("exit");
                UpdateLog("Entries updated..."); //TODO: could be more verbose...
                string str = await stream.Execute("show secure-policy 1");
                if (Regex.IsMatch(str, "name: _autofix"))
                    UpdateLog("Relevant security policy already exists, skipping...");
                else
                {
                    //TODO: could be more dynamic...
                    await stream.Execute("secure-policy insert 1");
                    await stream.Execute("name _autofix");
                    await stream.Execute("from WAN");
                    await stream.Execute("sourceip _autofix");
                    await stream.Execute("action deny");
                    await stream.Execute("exit");
                    UpdateLog("Blocking security policy created on position #1...");
                }
            }
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            FormHandler(1);
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            FormHandler(2);
            string str;
            UpdateLog("Removing...");
            progressBar1.Visible = true;
            if (platform)
            {
                await stream.Execute("vrf main secure-policy");
                await stream.Execute("del rule _autofix");
                await stream.Execute("/");
                await stream.Execute("object address-object");
                await stream.Execute("del group _autofix");
                await stream.Execute("/");
                str = await stream.Execute("show config object address-object address");
                MatchCollection delete = Regex.Matches(str, @"_autofix_\S+");
                await stream.Execute("object address-object");
                progressBar1.Maximum = delete.Count;
                foreach (Match match in delete)
                {
                    await stream.Execute("del address " + match);
                    progressBar1.Value++;
                }
                await stream.Execute("/");
                await stream.Execute("commit");
                UpdateLog("Removed " + delete.Count.ToString() + " entries...");

            }
            else
            {
                str = await stream.Execute("show secure-policy 1");
                if (Regex.IsMatch(str, "name: _autofix"))
                {
                    //TODO: could be more dynamic...
                    await stream.Execute("no secure-policy 1");
                    UpdateLog("Removed relevant security policy...");
                }
                else
                    UpdateLog("No relevant security policy found, skipping...");
                await stream.Execute("no object-group address _autofix");
                str = await stream.Execute("show address-object");
                MatchCollection delete = Regex.Matches(str, @"_autofix_\S+");
                progressBar1.Maximum = delete.Count;
                foreach (Match match in delete)
                {
                    await stream.Execute("no address-object " + match);
                    progressBar1.Value++;
                }
                UpdateLog("Removed " + delete.Count.ToString() + " entries...");
            }
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            FormHandler(1);
        }
    }
    public static class Extensions
    {
        public static async Task<bool> GetPlatform(this ShellStream stream) //Probably could use a better handling...
        {
            var task = Task.Run(() => stream.Expect("> ")); //wait until the initial prompt...
            await task;
            string str = await stream.Execute("show version", false);
            if (Regex.IsMatch(str, "Zyxel Communications Corp")) //probably not the best check...
                return false; //We're on ZLD
            else
                return true; //We're on uOS, or at the very least, we're not on ZLD...
        }
        public static async Task<bool> CheckPermissions(this ShellStream stream, bool platform)
        {
            if (platform)
            {
                string str = await stream.Execute("show config object user-object admin", false);
                if (Regex.IsMatch(str, "ERROR: "))
                    return false;
                else
                    return true;
            }
            else
            {
                string str = await stream.Execute("show running-config", false); //may be risky...
                if (Regex.IsMatch(str, "ERROR: "))
                    return false;
                else
                    return true;
            }
        }
        public static async Task uOSAddRule(this ShellStream stream, string ruleName)
        {
            await stream.Execute("/");
            string str = await stream.Execute("show config vrf main secure-policy rule");
            List<string[]> rules = new List<string[]>();
            var matches = Regex.Matches(str, @"(?s)(rule .*?^\s*\.\.)", RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                string ruleName2 = Regex.Match(match.Value, @"rule\s+(\S+)").Groups[1].Value;
                string user = Regex.Match(match.Value, @"user\s+(\S+)").Groups[1].Value;
                string schedule = Regex.Match(match.Value, @"schedule\s+(\S+)").Groups[1].Value;
                string from = Regex.Match(match.Value, @"from\s+(\S+)").Groups[1].Value;
                string sourceIp = Regex.Match(match.Value, @"source-ip\s+(\S+)").Groups[1].Value;
                string to = Regex.Match(match.Value, @"to\s+(\S+)").Groups[1].Value;
                string destinationIp = Regex.Match(match.Value, @"destination-ip\s+(\S+)").Groups[1].Value;
                string service = Regex.Match(match.Value, @"service\s+(\S+)").Groups[1].Value;
                string action = Regex.Match(match.Value, @"action\s+(\S+)").Groups[1].Value;
                string logging = Regex.Match(match.Value, @"logging\s+(\S+)").Groups[1].Value;
                string contentFilterProfile = Regex.Match(match.Value, @"content-filter-profile\s+(.+)").Groups[1].Value.Trim();
                string sslInspectionProfile = Regex.Match(match.Value, @"ssl-inspection-profile\s+(.+)").Groups[1].Value.Trim();
                string appPatrolProfile = Regex.Match(match.Value, @"app-patrol-profile\s+(.+)").Groups[1].Value.Trim();
                string description = Regex.Match(match.Value, @"description\s+(\S+)").Groups[1].Value;
                string enabled = Regex.Match(match.Value, @"enabled\s+(\S+)").Groups[1].Value;
                string[] vals = {ruleName2, user, schedule, from, sourceIp, to, destinationIp, service, action, logging, contentFilterProfile, sslInspectionProfile, appPatrolProfile, description, enabled};
                rules.Add(vals);
            }
            await stream.Execute("vrf main secure-policy");
            foreach (string[] rule in rules) await stream.Execute("del rule " + rule[0]);
            await stream.Execute("/");
            await stream.Execute("vrf main secure-policy rule "+ruleName);
            await stream.Execute("action deny");
            await stream.Execute("source-ip " + ruleName);
            await stream.Execute("from WAN");
            await stream.Execute("/");
            foreach (string[] rule in rules)
            {
                await stream.Execute("vrf main secure-policy rule " + rule[0]);
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
                await stream.Execute("/");
            }
        }
        public static async Task<string> Execute(this ShellStream stream, string command, bool superuser = true)
        {
            string str=null;
            stream.WriteLine(command);
            if (superuser)
            {
                var task = Task.Run(() => str = stream.Expect("# "));
                await task;
                return str;

            }
            else
            {
                var task = Task.Run(() => str = stream.Expect("> "));
                await task;
                return str;
            }
        }
    }


}
