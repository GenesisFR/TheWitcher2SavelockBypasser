using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TheWitcher2SavelockBypasser
{
    // TODO: add support for Epilogue
    public partial class TheWitcher2SavelockBypasserForm : Form
    {
        private RegistryChangeMonitor regMonitor; // tracks changes made to the registry key below
        private const string keyName = @"HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2";
        private const string formattedKeyName = @"HKCU\Software\CD Projekt RED\The Witcher 2\GameData";
        private object value;
        private byte[] bytes;
        private bool isInPrologue;
        private bool isInChapters;
        private bool isInEpilogue;
        private bool isUnlocked;

        private delegate void refreshDelegate();
        private refreshDelegate refreshLabelDelegate;

        public TheWitcher2SavelockBypasserForm()
        {
            InitializeComponent();
            StartMonitoringRegistry();

            refreshLabelDelegate = new refreshDelegate(refreshLabel);
            queryRegistry();
        }

        private void TheWitcher2SavelockBypasserForm_Load(object sender, EventArgs e)
        {
            refreshLabel();
        }

        private void TheWitcher2SavelockBypasserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopMonitoringRegistry();
        }

        private void StartMonitoringRegistry()
        {
            regMonitor = new RegistryChangeMonitor(keyName, RegistryChangeMonitor.REG_NOTIFY_CHANGE.LAST_SET);
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
            queryRegistry();
            refreshLabel();
        }

        private void queryRegistry()
        {
            isUnlocked   = false;
            isInPrologue = false;
            isInChapters = false;
            isInEpilogue = false;

            try
            {
                // Get value data
                value = Registry.GetValue(keyName, "GameData", null);

                if (value != null)
                {
                    // Convert data to bytes
                    bytes = (byte[])value;

                    // When playing on insane, the game will set a specific byte to a specific value, based on your progression
                    // The array of bytes will have a specific length, also based on your progression
                    isInPrologue = bytes.Length == 25;
                    isInChapters = bytes.Length == 30;
                    isInEpilogue = bytes.Length == 35;

                    // As soon as you die, the byte is decremented by one and the save is locked
                    // It's also locked if the byte is anything other than what it's supposed to be or if the registry key doesn't exist

                    // Prologue: if the 9th byte is 53, the save is unlocked
                    if (isInPrologue)
                        isUnlocked = bytes[8] == 53;
                    // Chapters: if the 14th byte is 112, the save is unlocked
                    else if (isInChapters)
                        isUnlocked = bytes[13] == 112;
                    // Epilogue: if the ??th byte is ???, the save is unlocked
                    else if (isInEpilogue)
                        isUnlocked = bytes[18] == bytes[18];
                }
            }
            catch
            {
                // Nothing to do here
            }
        }

        private void refreshLabel()
        {
            if (InvokeRequired)
                Invoke(refreshLabelDelegate);
            else
            {
                // Update label
                labelLocked.ForeColor = isUnlocked ? Color.Green : Color.Red;
                labelLocked.Text      = isUnlocked ? "UNLOCKED" : "LOCKED";
            }
        }

        private void buttonUnlock_Click(object sender, EventArgs e)
        {
            try
            {
                // Make sure we're not changing old values
                queryRegistry();

                if (value != null && bytes != null && bytes.Length > 8)
                {
                    // Magic numbers (yer a wizard, Harry)
                    if (isInPrologue)
                        bytes[8] = 53;
                    else if (isInChapters)
                        bytes[13] = 113;
                    else if (isInEpilogue)
                        bytes[18] = bytes[18];

                    // Update registry
                    Registry.SetValue(keyName, "GameData", bytes, RegistryValueKind.None);
                }

                bool isKeyFound = value != null;
                string msg = isKeyFound ? "Save unlocked! Do you want to run the game?" : string.Format("{0}\n\n{1}\n\n{2}",
                "The following registry key doesn't exist:", formattedKeyName, "Do you want to start a new playthrough?");

                // Offer the user to run the game
                if (MessageBox.Show(msg, isKeyFound ? "Success" : "Error", MessageBoxButtons.YesNo,
                    isKeyFound ? MessageBoxIcon.Information : MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start("steam://run/20920");
            }
            catch
            {
                MessageBox.Show(@"Error while editing the registry.\nMake sure you have sufficient privileges.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
