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
        private const string keyName = @"HKEY_CURRENT_USER\Software\CD Projekt RED\The Witcher 2";
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
            // Stop listening for events
            regMonitor.Dispose();
        }

        private void RegisterEvents()
        {
            regMonitor = new RegistryChangeMonitor(keyName, RegistryChangeMonitor.REG_NOTIFY_CHANGE.LAST_SET);
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
                // Get value data
                value = Registry.GetValue(keyName, "GameData", null);

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

                if (bytes.Length > 8)
                {
                    // Magic number (yer a wizard, Harry)
                    bytes[8] = 53;

                    // Update registry
                    Registry.SetValue(keyName, "GameData", bytes, RegistryValueKind.None);
                }

                bool isKeyFound = value != null;
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
                MessageBox.Show(@"Error while editing the registry.\nMake sure you have sufficient privileges.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
