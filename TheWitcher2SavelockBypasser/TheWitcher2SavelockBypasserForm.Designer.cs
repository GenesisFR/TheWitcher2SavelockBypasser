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
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonUnlock
            // 
            this.buttonUnlock.Location = new System.Drawing.Point(248, 120);
            this.buttonUnlock.Name = "buttonUnlock";
            this.buttonUnlock.Size = new System.Drawing.Size(112, 34);
            this.buttonUnlock.TabIndex = 0;
            this.buttonUnlock.Text = "Unlock save";
            this.buttonUnlock.UseVisualStyleBackColor = true;
            this.buttonUnlock.Click += new System.EventHandler(this.buttonUnlock_Click);
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
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // TheWitcher2SavelockBypasserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 167);
            this.Controls.Add(this.labelLocked);
            this.Controls.Add(this.buttonUnlock);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TheWitcher2SavelockBypasserForm";
            this.Text = "The Witcher 2 Savelock Bypasser v1.0";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TheWitcher2SavelockBypasserForm_FormClosed);
            this.Shown += new System.EventHandler(this.TheWitcher2SavelockBypasserForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonUnlock;
        private System.Windows.Forms.Label labelLocked;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
    }
}

