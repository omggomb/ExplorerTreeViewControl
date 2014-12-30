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
using System.IO;
using System.Diagnostics;

namespace ExplorerTreeView
{
    public partial class LinkingDialog : Form
    {
        private static CustomTreeItem m_selectedItem = null;

       
        private LinkingDialog()
        {
            InitializeComponent();
        }

        public static void ShowThisDialog(CustomTreeItem selectedItem)
        {
            if (selectedItem == null)
                throw new ArgumentNullException("selectedItem");

            m_selectedItem = selectedItem;

            var instance = new LinkingDialog();

            // Buttons for browsing
            instance.sourceBrowseButton.Text = Properties.Resources.ButtonBrowse;
            instance.destinationBrowseButton.Text = Properties.Resources.ButtonBrowse;

            // OK Cancel buttons
            instance.okButton.Text = Properties.Resources.ButtonOK;
            instance.cancelButton.Text = Properties.Resources.ButtonCancel;

            // Browsing destination is the same for both
            instance.destinationBrowseButton.Click += instance.BrowseDestinationFolder;

            // Creating the link / junction works the same for both
            instance.okButton.Click += instance.CreateHardLink;

            if (selectedItem.IsDirectory)
            {
                
                instance.InitJunctionDialog();
            }
            else
            {
               
                instance.InitHardLinkDialog();
            }

            instance.ShowDialog();
        }

        private void InitHardLinkDialog()
        {
            Text = Properties.Resources.ButtonCreateHardLink;
            destinationTextBox.Text = Properties.Resources.HardLinkDestinationDesc;
            destinationLabe.Text = Properties.Resources.HardLinkDestination;
            sourceFolderPathTextBox.Text = m_selectedItem.FullPathToReference;
            infoLabel.Text = Properties.Resources.MessageHardlinkOrJunction;
            folderLabel.Text = Properties.Resources.HardLinkSource;

            sourceBrowseButton.Click += BrowseSourceFile;
            
        }

        private void CreateHardLink(object sender, EventArgs e)
        {
            // H for hard link, J for junction
            string sLinkFlag = "/H";
            string sSource = sourceFolderPathTextBox.Text;
            string sDestination = "";

            // The path to the destination folder
            string sTrimmedPath = OmgUtils.Path.PathUtils.CheckFolderPath(destinationTextBox.Text);

            try
            {
                if (m_selectedItem.IsDirectory)
                {
                    sLinkFlag = "/J";
                    var dirInfo = new DirectoryInfo(sSource);

                    // Construct the full path                
                    sTrimmedPath += dirInfo.Name + '\\';
                    sDestination = sTrimmedPath;
                }
                else
                {
                    var fileInfo = new FileInfo(sSource);

                    // Same sheme as for junctions
                    sTrimmedPath += fileInfo.Name;
                    sDestination = sTrimmedPath;
                }
            }
            catch (Exception ex)
            {
                OmgUtils.UserInteraction.UserInteractionUtils.ShowErrorMessageBox(ex.Message, Properties.Resources.CommonError);
            }

            RunMKLink(sLinkFlag, sSource, sDestination);
        }

        private void BrowseSourceFile(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = m_selectedItem.FullPathToReference;

            var res = fileDialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                sourceFolderPathTextBox.Text = fileDialog.FileName;
                sourceFolderPathTextBox.SelectionStart = sourceFolderPathTextBox.Text.Length - 1;
            }
        }

        private void BrowseDestinationFolder(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = m_selectedItem.FullPathToReference;

            var res = folderDialog.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                destinationTextBox.Text = folderDialog.SelectedPath;
                destinationTextBox.SelectionStart = destinationTextBox.Text.Length - 1;
            }
            
        }

        private void InitJunctionDialog()
        {
            Text = Properties.Resources.ButtonCreateJunction;
            destinationTextBox.Text = Properties.Resources.JunctionDestinationDesc;
            destinationLabe.Text = Properties.Resources.JunctionDestination;
            sourceFolderPathTextBox.Text = m_selectedItem.FullPathToReference;
            infoLabel.Text = Properties.Resources.MessageNewJunction;
            folderLabel.Text = Properties.Resources.JunctionSource;

            sourceBrowseButton.Click += BrowseSourceFolder;
        }

        private void BrowseSourceFolder(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = m_selectedItem.FullPathToReference;

            var res = dialog.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                sourceFolderPathTextBox.Text = dialog.SelectedPath;
                sourceFolderPathTextBox.SelectionStart = sourceFolderPathTextBox.Text.Length - 1;
            }
        }

      

        private void RunMKLink(string sLinkFlag, string sSource, string sDestination)
        {
            string mkLinkPart = "/C mklink";
            string cmdCommandLine = mkLinkPart + sLinkFlag +
                                    " \"" + sDestination + "\"" +
                                    " \"" + sSource + "\"";


            // Start cmd shell and execute mklink
            var cmdProc = new Process();
            var startInfo = new ProcessStartInfo("cmd", cmdCommandLine);
            cmdProc.StartInfo = startInfo;

            OmgUtils.ProcessUt.ProcessUtils.RunProcessWithRedirectedStdErrorStdOut(cmdProc,
                Properties.Resources.ButtonCreateHardLink);

            if (cmdProc.ExitCode == 0)
                Close();
        }

    }
}
