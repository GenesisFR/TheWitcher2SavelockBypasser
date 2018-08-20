namespace TheWitcher2SavelockBypasser
{
    partial class TheWitcher2SavelockBypasserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TheWitcher2SavelockBypasserForm));
            this.buttonUnlock = new System.Windows.Forms.Button();
            this.labelLocked = new System.Windows.Forms.Label();
            this.buttonBackup = new System.Windows.Forms.Button();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonUnlock
            // 
            this.buttonUnlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonUnlock.Location = new System.Drawing.Point(248, 121);
            this.buttonUnlock.Name = "buttonUnlock";
            this.buttonUnlock.Size = new System.Drawing.Size(112, 34);
            this.buttonUnlock.TabIndex = 4;
            this.buttonUnlock.Text = "Unlock save";
            this.buttonUnlock.UseVisualStyleBackColor = true;
            this.buttonUnlock.Click += new System.EventHandler(this.ButtonUnlock_Click);
            // 
            // labelLocked
            // 
            this.labelLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLocked.ForeColor = System.Drawing.Color.Red;
            this.labelLocked.Location = new System.Drawing.Point(12, 9);
            this.labelLocked.Name = "labelLocked";
            this.labelLocked.Size = new System.Drawing.Size(585, 108);
            this.labelLocked.TabIndex = 1;
            this.labelLocked.Text = "LOCKED";
            this.labelLocked.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonBackup
            // 
            this.buttonBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonBackup.Location = new System.Drawing.Point(130, 120);
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(112, 34);
            this.buttonBackup.TabIndex = 3;
            this.buttonBackup.Text = "Backup";
            this.buttonBackup.UseVisualStyleBackColor = true;
            this.buttonBackup.Click += new System.EventHandler(this.ButtonBackup_Click);
            // 
            // buttonRestore
            // 
            this.buttonRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonRestore.Location = new System.Drawing.Point(366, 120);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(112, 34);
            this.buttonRestore.TabIndex = 5;
            this.buttonRestore.Text = "Restore";
            this.buttonRestore.UseVisualStyleBackColor = true;
            this.buttonRestore.Click += new System.EventHandler(this.ButtonRestore_Click);
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.Filter = "backup.reg";
            this.fileSystemWatcher.NotifyFilter = System.IO.NotifyFilters.FileName;
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.FileSystemWatcher_Changed);
            this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.FileSystemWatcher_Changed);
            this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.FileSystemWatcher_Changed);
            // 
            // buttonReset
            // 
            this.buttonReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonReset.Location = new System.Drawing.Point(12, 120);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(112, 34);
            this.buttonReset.TabIndex = 2;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.buttonPlay.Location = new System.Drawing.Point(485, 120);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(112, 34);
            this.buttonPlay.TabIndex = 6;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.ButtonPlay_Click);
            // 
            // TheWitcher2SavelockBypasserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 167);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonRestore);
            this.Controls.Add(this.buttonBackup);
            this.Controls.Add(this.labelLocked);
            this.Controls.Add(this.buttonUnlock);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TheWitcher2SavelockBypasserForm";
            this.Text = "The Witcher 2 Savelock Bypasser v1.0";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheWitcher2SavelockBypasserForm_FormClosed);
            this.Load += new System.EventHandler(this.TheWitcher2SavelockBypasserForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonUnlock;
        private System.Windows.Forms.Label labelLocked;
        private System.Windows.Forms.Button buttonBackup;
        private System.Windows.Forms.Button buttonRestore;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonPlay;
    }
}

