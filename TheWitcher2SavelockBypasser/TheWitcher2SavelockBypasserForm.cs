﻿using Microsoft.Win32;
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
        private const string FORMATTED_KEY_NAME = @"HKCU\Software\CD Projekt RED\The Witcher 2\GameData";
        private const string KEY_NAME = @"HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2";
        private const string REG_EXE = "reg.exe";
        private const string REG_FILE_NAME = "backup.reg";
        private const string REG_LAST_KEY_NAME = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";
        private const string REG_TEMP_FILE_NAME = "backup_temp.reg";
        private const string VALUE_NAME = @"GameData";
        private object value;
        private byte[] bytes;
        private bool hasOnePlaythrough;
        private bool hasTwoPlaythroughs;
        private bool hasThreePlaythroughs;
        private bool hasFourPlaythroughs;
        private bool hasFiveOrMorePlaythroughs;
        private bool isUnlocked;
        private bool showWarning = false;

        private delegate void refreshDelegate();
        private refreshDelegate refreshUIDelegate;

        public TheWitcher2SavelockBypasserForm()
        {
            InitializeComponent();
            StartMonitoringRegistry();

            refreshUIDelegate = new refreshDelegate(RefreshUI);
        }

        private void TheWitcher2SavelockBypasserForm_Shown(object sender, EventArgs e)
        {
            textBoxRegistryKey.Text = KEY_NAME;
            fileSystemWatcher.Path = Directory.GetCurrentDirectory();
            buttonRestore.Enabled = File.Exists(REG_FILE_NAME);

            OnRegistryChanged();

            if (value == null)
            {
                string msg = string.Format("{0}\n\n{1}\n\n{2}", "The following registry entry doesn't exist:", FORMATTED_KEY_NAME,
                    "Make sure you started a new playthrough.");
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TheWitcher2SavelockBypasserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Clean up resources
            StopMonitoringRegistry();
        }

        private void StartMonitoringRegistry()
        {
            if (regMonitor == null)
            {
                regMonitor = new RegistryChangeMonitor(KEY_NAME, RegistryChangeMonitor.REG_NOTIFY_CHANGE.LAST_SET);
                regMonitor.Changed += OnRegistryChanged;
            }

            // Start listening for events
            regMonitor.Start();
        }

        private void StopMonitoringRegistry()
        {
            // Stop listening for events
            regMonitor?.Dispose();
        }

        private void OnRegistryChanged(object sender = null, RegistryChangeEventArgs e = null)
        {
            QueryRegistry();
            RefreshUI();

            bool hasMoreThanFivePlaythroughs = bytes?.Length > 45;

            if (!showWarning && hasMoreThanFivePlaythroughs)
            {
                showWarning = true;
                MessageBox.Show("We detected you started more than 5 playthroughs. This tool can only unlock your first 5 " +
                    "playthroughs. Therefore, the locked status shown may be incorrect. Check the readme for more details.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void QueryRegistry()
        {
            hasOnePlaythrough = hasTwoPlaythroughs = hasThreePlaythroughs = hasFourPlaythroughs = hasFiveOrMorePlaythroughs = false;
            isUnlocked = false;

            try
            {
                // Get value data
                value = Registry.GetValue(KEY_NAME, VALUE_NAME, null);

                if (value != null)
                {
                    // When starting a new playthrough, the game will read/write a specific entry in registry, which value is an array of
                    // bytes, so we need to convert it.
                    bytes = (byte[])value;

                    // This array of bytes has a specific length, based on the number of playthroughs started. It starts at 25 bytes and
                    // is incremented by 5 for each new playthrough.
                    int bytesLength = bytes.Length;
                    hasOnePlaythrough = bytesLength >= 25;
                    hasTwoPlaythroughs = bytesLength >= 30;
                    hasThreePlaythroughs = bytesLength >= 35;
                    hasFourPlaythroughs = bytesLength >= 40;
                    hasFiveOrMorePlaythroughs = bytesLength >= 45;

                    // When you die while playing on insane, a specific byte is shifted by one, effectively locking saves from the current
                    // playthrough. They're also locked if the byte has any other value or the registry entry doesn't exist.
                    // The byte is different depending on the number of playthroughs, that's why I'm only supporting up to 5 playthroughs.

                    // If the 9th byte is 53 (35 in hexa), saves for the first playthrough are unlocked
                    if (hasOnePlaythrough)
                        isUnlocked = bytes[8] == 53;
                    // If the 14th byte is 112 (70 in hexa), saves for the second playthrough are unlocked
                    if (hasTwoPlaythroughs)
                        isUnlocked = bytes[13] == 112;
                    // If the 19th byte is 119 (77 in hexa), saves for the third playthrough are unlocked
                    if (hasThreePlaythroughs)
                        isUnlocked = bytes[18] == 119;
                    // If the 24th byte is 4, saves for the fourth playthrough are unlocked
                    if (hasFourPlaythroughs)
                        isUnlocked = bytes[23] == 4;
                    // If the 29th byte is 17 (11 in hexa), saves for the fifth playthrough are unlocked
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

                // Update buttons status
                buttonOpenInRegedit.Enabled = value != null;
                buttonReset.Enabled = value != null;
                buttonBackup.Enabled = value != null;
                buttonUnlock.Enabled = value != null;
            }
        }

        private void ButtonOpenInRegedit_Click(object sender, EventArgs e)
        {
            try
            {
                Registry.SetValue(REG_LAST_KEY_NAME, "LastKey", KEY_NAME);
                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Your saved game information in registry will be deleted. Without it, you won't be" +
                " able to load any save made on insane difficulty. Would you like to back it up first?", "Warning",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result != DialogResult.Cancel)
            {
                if (result == DialogResult.Yes)
                    buttonBackup.PerformClick();

                if (LaunchProcess(REG_EXE, string.Format("delete \"{0}\" /v \"{1}\" /f", KEY_NAME, VALUE_NAME)))
                {
                    showWarning = false;
                    Array.Resize(ref bytes, 0);
                    MessageBox.Show("Reset successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ButtonBackup_Click(object sender, EventArgs e)
        {
            try
            {
                // Make a temporary REG file
                bool isTempBackupSuccessful = LaunchProcess(REG_EXE, string.Format("export \"{0}\" \"{1}\" /y", KEY_NAME,
                    REG_TEMP_FILE_NAME));

                if (value != null && isTempBackupSuccessful && File.Exists(REG_TEMP_FILE_NAME))
                {
                    // Remove unnecessary lines
                    List<string> lines = new List<string>(File.ReadLines(REG_TEMP_FILE_NAME));
                    lines.RemoveAll(item => item.Contains("Language") || item.Contains("Speech") || item.Contains("InstallStatus") ||
                        item.Contains(@"[HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2\Downloads]"));

                    // Create the final REG file
                    File.WriteAllLines(REG_FILE_NAME, lines);
                    File.Delete(REG_TEMP_FILE_NAME);

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

                if (value != null)
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
                    MessageBox.Show("Save unlocked successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show(@"Error while editing the registry.\nMake sure you have sufficient privileges.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    private void ButtonRestore_Click(object sender, EventArgs e)
    {
        if (LaunchProcess(REG_EXE, $"import {REG_FILE_NAME}"))
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

                    // Return code is 0 for success or 1 for failure
                    if (proc.ExitCode == 1)
                    {
                        MessageBox.Show(proc.StandardError.ReadToEnd(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            buttonRestore.Enabled = File.Exists(REG_FILE_NAME);
        }
    }
}
