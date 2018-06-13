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
        private bool isUnlocked = false;

        private delegate void noParamDelegate();
        private noParamDelegate refreshLabelDelegate;

        public TheWitcher2SavelockBypasserForm()
        {
            InitializeComponent();
            RegisterEvents();

            refreshLabelDelegate = new noParamDelegate(refreshLabel);
            key = Registry.CurrentUser.OpenSubKey(@"Software\CD Projekt RED\The Witcher 2", true);
        }

        private void TheWitcher2SavelockBypasserForm_Shown(object sender, EventArgs e)
        {
            queryRegistry();
            refreshLabel();
        }

        private void TheWitcher2SavelockBypasserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Stop listening for events
            regMonitor.Stop();
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
            if (key != null)
            {
                value = key.GetValue("GameData");

                if (value != null)
                {
                    // Get value
                    bytes = (byte[])key.GetValue("GameData");

                    if (bytes.Length > 8)
                        isUnlocked = bytes[8] == 53;
                }
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
                labelLocked.Text = isUnlocked ? "UNLOCKED" : "LOCKED";
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
            catch (Exception)
            {
                MessageBox.Show(@"Error while editing the registry.\nMake sure you have required permissions to edit the registry.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
