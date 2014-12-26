using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace ExplorerTreeView
{
    public partial class HardLink : Form
    {
        private CustomTreeItem m_selectedItem = null;

       
        public HardLink(CustomTreeItem selectedItem)
        {
            InitializeComponent();

            m_selectedItem = selectedItem;

            if (m_selectedItem == null)
                throw new ArgumentNullException("selectedItem");

            linkNameTextBox.Text = m_selectedItem.IdentificationName;

            Text = Properties.Resources.ButtonCreateHardLink;
            infoLabel.Text = Properties.Resources.MessageHardlinkOrJunction;
            folderLabel.Text = Properties.Resources.ChooseLinkFolder;
            linkNameLabel.Text = Properties.Resources.ChooseLinkName;
            browseButton.Text = Properties.Resources.ButtonBrowse;
            okButton.Text = Properties.Resources.ButtonOK;
            cancelButton.Text = Properties.Resources.ButtonCancel;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            var res = folderBrowserDialog.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                folderPathTextBox.Text = folderBrowserDialog.SelectedPath;
                folderPathTextBox.SelectionStart = folderPathTextBox.Text.Length - 1;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Check if container folder name ends with slash
            // If not, add one
            if (folderPathTextBox.Text[folderPathTextBox.Text.Length - 1] != '\\')
                folderPathTextBox.Text += "\\";

            // Check if the container folder exists, if not do nothing
            if (!FileSystem.DirectoryExists(folderPathTextBox.Text))
            {
                OmgUtils.UserInteraction.UserInteractionUtils.ShowErrorMessageBox(
                    Properties.Resources.FolderNotExisting, 
                    Properties.Resources.CommonError);

                return;
            }

            // Add extension if missing and item is file
            if (!m_selectedItem.IsDirectory)
            {
                if (!linkNameTextBox.Text.Contains('.'))
                    linkNameTextBox.Text += "." +
                        OmgUtils.Path.PathUtils.GetExtension(m_selectedItem.FullPathToReference);
            }
            
            // Construct full link name
            string sFullLinkName = folderPathTextBox.Text + linkNameTextBox.Text;

            // Determine kind of link
            string sLinkFlag = "/J";    // Junction for directories
            if (!m_selectedItem.IsDirectory)
                sLinkFlag = "/H";       // Hard link for files

            // Start cmd shell and execute mklink
            var cmdProc = new Process();
            var startInfo = new ProcessStartInfo("cmd", "/C mklink " + sLinkFlag + " \"" + 
                                                    sFullLinkName + "\" \"" +
                                                    m_selectedItem.FullPathToReference + "\"");
            cmdProc.StartInfo = startInfo;

            OmgUtils.ProcessUt.ProcessUtils.RunProcessWithRedirectedStdErrorStdOut(cmdProc,
                Properties.Resources.ButtonCreateHardLink);

            if (cmdProc.ExitCode == 0)
                Close();
        }

 
    }
}
