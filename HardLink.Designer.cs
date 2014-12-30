namespace ExplorerTreeView
{
    partial class LinkingDialog
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
            this.sourceFolderPathTextBox = new System.Windows.Forms.TextBox();
            this.sourceBrowseButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.destinationLabe = new System.Windows.Forms.Label();
            this.destinationTextBox = new System.Windows.Forms.TextBox();
            this.destinationBrowseButton = new System.Windows.Forms.Button();
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
            // sourceFolderPathTextBox
            // 
            this.sourceFolderPathTextBox.Location = new System.Drawing.Point(17, 60);
            this.sourceFolderPathTextBox.Name = "sourceFolderPathTextBox";
            this.sourceFolderPathTextBox.Size = new System.Drawing.Size(184, 20);
            this.sourceFolderPathTextBox.TabIndex = 2;
            // 
            // sourceBrowseButton
            // 
            this.sourceBrowseButton.Location = new System.Drawing.Point(210, 60);
            this.sourceBrowseButton.Name = "sourceBrowseButton";
            this.sourceBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.sourceBrowseButton.TabIndex = 3;
            this.sourceBrowseButton.Text = "button1";
            this.sourceBrowseButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
            // 
            // destinationLabe
            // 
            this.destinationLabe.AutoSize = true;
            this.destinationLabe.Location = new System.Drawing.Point(19, 87);
            this.destinationLabe.Name = "destinationLabe";
            this.destinationLabe.Size = new System.Drawing.Size(35, 13);
            this.destinationLabe.TabIndex = 8;
            this.destinationLabe.Text = "label1";
            // 
            // destinationTextBox
            // 
            this.destinationTextBox.Location = new System.Drawing.Point(19, 104);
            this.destinationTextBox.Name = "destinationTextBox";
            this.destinationTextBox.Size = new System.Drawing.Size(178, 20);
            this.destinationTextBox.TabIndex = 9;
            // 
            // destinationBrowseButton
            // 
            this.destinationBrowseButton.Location = new System.Drawing.Point(210, 104);
            this.destinationBrowseButton.Name = "destinationBrowseButton";
            this.destinationBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.destinationBrowseButton.TabIndex = 10;
            this.destinationBrowseButton.Text = "button1";
            this.destinationBrowseButton.UseVisualStyleBackColor = true;
            // 
            // HardLink
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(300, 261);
            this.Controls.Add(this.destinationBrowseButton);
            this.Controls.Add(this.destinationTextBox);
            this.Controls.Add(this.destinationLabe);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.sourceBrowseButton);
            this.Controls.Add(this.sourceFolderPathTextBox);
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
        private System.Windows.Forms.TextBox sourceFolderPathTextBox;
        private System.Windows.Forms.Button sourceBrowseButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label destinationLabe;
        private System.Windows.Forms.TextBox destinationTextBox;
        private System.Windows.Forms.Button destinationBrowseButton;
    }
}