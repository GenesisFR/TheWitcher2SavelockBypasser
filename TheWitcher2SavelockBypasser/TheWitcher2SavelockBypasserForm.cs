using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TheWitcher2SavelockBypasser
{
    public partial class TheWitcher2SavelockBypasserForm : Form
    {
        private RegistryChangeMonitor regMonitor; // tracks changes made to the registry key below
        private const string KEY_NAME = @"HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2";
        private const string VALUE_NAME = @"GameData";
        private const string FORMATTED_KEY_NAME = @"HKCU\Software\CD Projekt RED\The Witcher 2\GameData";
        private object value;
        private byte[] bytes;
        private bool hasOnePlaythrough;
        private bool hasTwoPlaythroughs;
        private bool hasThreePlaythroughs;
        private bool hasFourPlaythroughs;
        private bool hasFiveOrMorePlaythroughs;
        private bool isUnlocked;

        private delegate void refreshDelegate();
        private refreshDelegate refreshUIDelegate;

        public TheWitcher2SavelockBypasserForm()
        {
            InitializeComponent();
            StartMonitoringRegistry();

            refreshUIDelegate = new refreshDelegate(RefreshUI);
            QueryRegistry();
        }

        private void TheWitcher2SavelockBypasserForm_Load(object sender, EventArgs e)
        {
            RefreshUI();
            fileSystemWatcher.Path = Directory.GetCurrentDirectory();
            buttonRestore.Enabled = File.Exists("backup.reg");

            if (value == null)
            {
                string msg = string.Format("{0}\n\n{1}\n\n{2}", "The following registry entry doesn't exist:", FORMATTED_KEY_NAME,
                    "Make sure you started a new insane playthrough.");

                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TheWitcher2SavelockBypasserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopMonitoringRegistry();
        }

        private void StartMonitoringRegistry()
        {
            regMonitor = new RegistryChangeMonitor(KEY_NAME, RegistryChangeMonitor.REG_NOTIFY_CHANGE.LAST_SET);
            regMonitor.Changed += OnRegistryChanged;

            // Start listening for events
            regMonitor.Start();
        }

        private void StopMonitoringRegistry()
        {
            // Stop listening for events
            regMonitor.Dispose();
        }

        private void OnRegistryChanged(object sender, RegistryChangeEventArgs e)
        {
            QueryRegistry();
            RefreshUI();
        }

        private void QueryRegistry()
        {
            isUnlocked = false;
            hasOnePlaythrough = false;
            hasTwoPlaythroughs = false;
            hasThreePlaythroughs = false;
            hasFourPlaythroughs = false;
            hasFiveOrMorePlaythroughs = false;

            try
            {
                // Get value data
                value = Registry.GetValue(KEY_NAME, VALUE_NAME, null);

                if (value != null)
                {
                    // When playing on insane, the game will read/write a specific entry in registry, which value is an array of bytes, so
                    // we need to convert it.
                    bytes = (byte[])value;

                    // This array of bytes has a specific length, based on the number of insane playthroughs started. It starts at 25
                    // bytes and is incremented by 5 for each new insane playthrough.
                    hasOnePlaythrough         = bytes.Length >= 25;
                    hasTwoPlaythroughs        = bytes.Length >= 30;
                    hasThreePlaythroughs      = bytes.Length >= 35;
                    hasFourPlaythroughs       = bytes.Length >= 40;
                    hasFiveOrMorePlaythroughs = bytes.Length >= 45;

                    // When you die, a specific byte (based on how many insane playthroughs you've started) is shifted by one, effectively
                    // locking the save for the current playthrough. It's also locked if the byte is anything other than what it's supposed
                    // to be or if the registry entry doesn't exist.
                    // This byte is different depending on the number of playthroughs, that's why I'm only supporting up to 5 playthroughs.

                    // If the 9th byte is 53 (35 in hexa), the save is unlocked
                    if (hasOnePlaythrough)
                        isUnlocked = bytes[8] == 53;
                    // If the 14th byte is 112 (70 in hexa), the save is unlocked
                    if (hasTwoPlaythroughs)
                        isUnlocked = bytes[13] == 112;
                    // If the 19th byte is 119 (77 in hexa), the save is unlocked
                    if (hasThreePlaythroughs)
                        isUnlocked = bytes[18] == 119;
                    // If the 24th byte is 4, the save is unlocked
                    if (hasFourPlaythroughs)
                        isUnlocked = bytes[23] == 4;
                    // If the 29th byte is 17 (11 in hexa), the save is unlocked
                    if (hasFiveOrMorePlaythroughs)
                        isUnlocked = bytes[28] == 17;
                }
            }
            catch
            {
                // Nothing to do here
            }
        }

        private void RefreshUI()
        {
            if (InvokeRequired)
                Invoke(refreshUIDelegate);
            else
            {
                // Update label
                labelLocked.ForeColor = isUnlocked ? Color.Green : Color.Red;
                labelLocked.Text = isUnlocked ? "UNLOCKED" : "LOCKED";

                buttonReset.Enabled = value != null;
                buttonBackup.Enabled = value != null;
                buttonUnlock.Enabled = value != null;
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Your saved game information in registry will be deleted. Without it, you won't be" +
                " able to load any save made on insane difficulty. Would you like to back it up first?", "Warning",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
                buttonBackup.PerformClick();

            if (result != DialogResult.Cancel)
            {
                if (LaunchProcess("reg.exe", string.Format("delete \"{0}\" /v \"{1}\" /f", KEY_NAME, VALUE_NAME)))
                    MessageBox.Show("Reset successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButtonBackup_Click(object sender, EventArgs e)
        {
            try
            {
                // Make a temporary REG file
                if (value != null && LaunchProcess("reg.exe", string.Format("export \"{0}\" \"{1}\" /y", KEY_NAME, "backup_temp.reg")) &&
                    File.Exists("backup_temp.reg"))
                {
                    // Remove unnecessary lines
                    List<string> lines = new List<string>(File.ReadLines("backup_temp.reg"));
                    lines.RemoveAll(item => item.Contains("Language") || item.Contains("Speech") || item.Contains("InstallStatus") ||
                        item.Contains(@"[HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2\Downloads]"));

                    // Create the final REG file
                    File.WriteAllLines("backup.reg", lines);
                    File.Delete("backup_temp.reg");

                    MessageBox.Show("Backup successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                // Nothing to do here
            }
        }

        private void ButtonUnlock_Click(object sender, EventArgs e)
        {
            try
            {
                // Make sure we're not changing old values
                QueryRegistry();

                if (value != null && bytes != null && bytes.Length > 8)
                {
                    // Magic numbers (yer a wizard, Harry)
                    if (hasOnePlaythrough)
                        bytes[8] = 53;
                    if (hasTwoPlaythroughs)
                        bytes[13] = 112;
                    if (hasThreePlaythroughs)
                        bytes[18] = 119;
                    if (hasFourPlaythroughs)
                        bytes[23] = 4;
                    if (hasFiveOrMorePlaythroughs)
                        bytes[28] = 17;

                    // Update registry
                    Registry.SetValue(KEY_NAME, VALUE_NAME, bytes, RegistryValueKind.None);
                }

                if (value != null)
                    MessageBox.Show("Save unlocked successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(@"Error while editing the registry.\nMake sure you have sufficient privileges.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    private void ButtonRestore_Click(object sender, EventArgs e)
    {
        if (LaunchProcess("reg.exe", "import backup.reg"))
            MessageBox.Show("Restoration successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ButtonPlay_Click(object sender, EventArgs e)
    {
        Process.Start("steam://run/20920");
    }

    private bool LaunchProcess(string program, string arguments)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = program;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.Arguments = arguments;
                    proc.Start();
                    proc.WaitForExit();

                    string stdout = proc.StandardOutput.ReadToEnd();
                    string stderr = proc.StandardError.ReadToEnd();

                    if (stderr.StartsWith("ERROR"))
                    {
                        MessageBox.Show(stderr, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            buttonRestore.Enabled = File.Exists("backup.reg");
        }
    }
}
