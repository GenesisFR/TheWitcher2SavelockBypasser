using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TheWitcher2SavelockBypasser
{
    public partial class TheWitcher2SavelockBypasserForm : Form
    {
        private RegistryChangeMonitor regMonitor;
        private RegistryKey key;
        private object value;
        private byte[] bytes;
        private bool isUnlocked;

        private delegate void refreshDelegate();
        private refreshDelegate refreshLabelDelegate;

        public TheWitcher2SavelockBypasserForm()
        {
            InitializeComponent();
            RegisterEvents();

            refreshLabelDelegate = new refreshDelegate(refreshLabel);
            queryRegistry();
        }

        private void TheWitcher2SavelockBypasserForm_Load(object sender, EventArgs e)
        {
            refreshLabel();
        }

        private void TheWitcher2SavelockBypasserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            key.Dispose();

            // Stop listening for events
            regMonitor.Dispose();
        }

        private void RegisterEvents()
        {
            regMonitor = new RegistryChangeMonitor(@"HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2",
                RegistryChangeMonitor.REG_NOTIFY_CHANGE.LAST_SET);
            regMonitor.Changed += OnRegistryChanged;
            regMonitor.Start();
        }

        private void OnRegistryChanged(object sender, RegistryChangeEventArgs e)
        {
            queryRegistry();
            refreshLabel();
        }

        private void queryRegistry()
        {
            isUnlocked = false;

            try
            {
                // Get key
                key = Registry.CurrentUser.OpenSubKey(@"Software\CD Projekt RED\The Witcher 2", true);

                if (key != null)
                {
                    // Get value data
                    value = key.GetValue("GameData");

                    if (value != null)
                    {
                        // Convert data to bytes
                        bytes = (byte[])value;

                        // When starting a new insane playthrough, the game will set the 9th byte to 53, which means the save is unlocked
                        // As soon as you die, this byte will change to 52 and the save will be locked
                        // It's also locked if the 9th byte is anything other than 53 or if the GameData registry key doesn't exist
                        if (bytes.Length > 8)
                            isUnlocked = bytes[8] == 53;
                    }
                }
            }
            catch
            {

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

                if (bytes.Length > 8)
                {
                    // Magic number (yer a wizard, Harry)
                    bytes[8] = 53;

                    // Update registry
                    key.SetValue("GameData", bytes, RegistryValueKind.None);
                }

                bool isKeyFound = key != null && value != null;
                string msg = isKeyFound ? "Save unlocked! Do you want to run the game?" :
                        @"The following registry key doesn't exist:\n
                        HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2\GameData\n
                        doesn't exist. Do you want to run the game and start a new playthrough?";

                // Offer the user to run the game
                if (MessageBox.Show(msg, isKeyFound ? "Success" : "Error", MessageBoxButtons.YesNo,
                    isKeyFound ? MessageBoxIcon.Information : MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start("steam://run/20920");
            }
            catch
            {
                MessageBox.Show(@"Error while editing the registry.\nMake sure you have required permissions to edit the registry.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
