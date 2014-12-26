namespace ExplorerTreeView
{
    partial class HardLink
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
            this.infoLabel = new System.Windows.Forms.Label();
            this.folderLabel = new System.Windows.Forms.Label();
            this.folderPathTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.linkNameLabel = new System.Windows.Forms.Label();
            this.linkNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(13, 13);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(54, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Properties";
            // 
            // folderLabel
            // 
            this.folderLabel.AutoSize = true;
            this.folderLabel.Location = new System.Drawing.Point(16, 38);
            this.folderLabel.Name = "folderLabel";
            this.folderLabel.Size = new System.Drawing.Size(35, 13);
            this.folderLabel.TabIndex = 1;
            this.folderLabel.Text = "label1";
            // 
            // folderPathTextBox
            // 
            this.folderPathTextBox.Location = new System.Drawing.Point(16, 59);
            this.folderPathTextBox.Name = "folderPathTextBox";
            this.folderPathTextBox.Size = new System.Drawing.Size(184, 20);
            this.folderPathTextBox.TabIndex = 2;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(207, 57);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 3;
            this.browseButton.Text = "button1";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // linkNameLabel
            // 
            this.linkNameLabel.AutoSize = true;
            this.linkNameLabel.Location = new System.Drawing.Point(15, 86);
            this.linkNameLabel.Name = "linkNameLabel";
            this.linkNameLabel.Size = new System.Drawing.Size(35, 13);
            this.linkNameLabel.TabIndex = 4;
            this.linkNameLabel.Text = "label1";
            // 
            // linkNameTextBox
            // 
            this.linkNameTextBox.Location = new System.Drawing.Point(16, 107);
            this.linkNameTextBox.Name = "linkNameTextBox";
            this.linkNameTextBox.Size = new System.Drawing.Size(184, 20);
            this.linkNameTextBox.TabIndex = 5;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(210, 226);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "button1";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(129, 226);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "button2";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // HardLink
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(300, 261);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.linkNameTextBox);
            this.Controls.Add(this.linkNameLabel);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.folderPathTextBox);
            this.Controls.Add(this.folderLabel);
            this.Controls.Add(this.infoLabel);
            this.Name = "HardLink";
            this.Text = "HardLink";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label folderLabel;
        private System.Windows.Forms.TextBox folderPathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label linkNameLabel;
        private System.Windows.Forms.TextBox linkNameTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}