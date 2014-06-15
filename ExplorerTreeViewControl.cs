/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 06/02/2014
 * Time: 12:44
 */
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


using Microsoft.VisualBasic.FileIO;

namespace ExplorerTreeView
{
	/// <summary>
	/// A wpf treeview control with explorer functionalities.
	/// Watches a directory, adds context menu options for files and folders,
	/// Displays files and folders in a tree view
	/// </summary>
	public class ExplorerTreeViewControl : TreeView
	{
		/**
		 * Attributes
		 */
		
		/// <summary>
		/// File system watcher that watches the folder itself.
		/// <see cref="SubdirFileWatcher"/>
		/// </summary>
		public FileSystemWatcher ToplevelFileWatcher {get; private set;}
		
		/// <summary>
		/// File system watcher that watcher all the subdirs inside the dir to be watched
		/// </summary>
		public FileSystemWatcher SubdirFileWatcher {get; private set;}
		
		/// <summary>
		/// Full path to the directory to be watched
		/// </summary>
		public string WatchDir
		{
			get
			{
				return m_sWatchDir;
			}
			set
			{
				if (String.IsNullOrWhiteSpace(value) || !Directory.Exists(value))
				{
					throw new ArgumentException("The provided watch dir string is null, empty or not a directory!");
				}
				else
				{
					m_sWatchDir = value;
					var dirInfo = new DirectoryInfo(value);
					
					SubdirFileWatcher.Path = value;
					
					// FIXME: Possible bug when dir to be watched is a child of a drive (then the dirInfo.Parent will be null?)
					if (dirInfo.Parent != null)
					{
						ToplevelFileWatcher.Path = dirInfo.Parent.FullName;
						ToplevelFileWatcher.Filter = dirInfo.Name + "\\*.*";
					}
				}
			}
		}
		
		
		/// <summary>
		/// Toggle watching of specified directory on or off
		/// </summary>
		public bool IsWatching
		{
			// disable once FunctionNeverReturns
			get
			{
				return IsWatching;
			}
			set
			{
				SubdirFileWatcher.EnableRaisingEvents = value;
				ToplevelFileWatcher.EnableRaisingEvents = value;
			}
		}
		
		/// <summary>
		/// This is used by every tree item
		/// </summary>
		public  ContextMenu GlobalContextMenu {get; set;}
		
		
		/// <summary>
		/// Factory used inside the ExplorerTreeViewControl to create new items
		/// </summary>
		public static TreeItemFactory TreeItemFactory {get; private set; }
		
		string m_sWatchDir;
		
		// used to detect whether a file shoudl be cut or copied (contextmenu)
		bool m_bIsFileCut;
		
		delegate void AddItemDelegate(string sParetTreePath, string sFullPathToEntry, bool bIsDirectory);
		
		delegate void RemoveItemDelegate(string sParentTreePath, string sIdentName);
		
		delegate void RenameItemDelegate(string sTreePathToItem, string sNewName, string sFullPathToRenamed);
		
		/**
		 * CTOR
		 */
		
		/// <summary>
		/// Creates a new instance of the explorer tree view control.
		/// IMPORTANT: This does not do any fill operations. You need to call <see cref="InitializeTree"/> at least once!
		/// </summary>
		/// <param name="sPathToWatch">The path to the top level directory to be watched</param>
		/// <param name="bStartNow">If false,does not start watching the directory</param>
		public ExplorerTreeViewControl(string sPathToWatch, bool bStartNow)
		{
			ToplevelFileWatcher = new FileSystemWatcher();
			SubdirFileWatcher = new FileSystemWatcher();
			
			ToplevelFileWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName |
				NotifyFilters.LastWrite | NotifyFilters.CreationTime;
			// Filter gets set by WatchDir property
			ToplevelFileWatcher.IncludeSubdirectories = true;
			
			SubdirFileWatcher.NotifyFilter = ToplevelFileWatcher.NotifyFilter;
			SubdirFileWatcher.Filter = "*.*";
			SubdirFileWatcher.IncludeSubdirectories = true;
			
			SubdirFileWatcher.Created += OnEntryChanged;
			SubdirFileWatcher.Deleted += OnEntryChanged;
			SubdirFileWatcher.Renamed += OnEntryRenamed;
			
			
			WatchDir = sPathToWatch;
			IsWatching = bStartNow;
			
			SetupGlobalContextMenu();
			SetupContextMenuShortCuts();
			
			KeyDown += HandleKeyDown;
		}

		

		
		
		public ExplorerTreeViewControl () : this("C:\\", false)
		{
		}
		
		#region Public methods
		
		/// <summary>
		/// Does a primer search and displays the contents of the WatchDir. Not recursive.
		/// </summary>
		public void InitializeTree(TreeItemFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException();
			
			TreeItemFactory = factory;
			SearchDirectory();
		}
		
		/// <summary>
		/// Travels through the tree view and retrieves the first item that matches the path given
		/// </summary>
		/// <param name="sPath">Path of the wanted tree view item, separated by backslashes ('\\')</param>
		/// <param name="bAssumeRoot">If true, first element of the path is assumed to be a root item, meaning it will be searched directly
		/// inside the TreeView's Item property, else the first item in there is taken and the path will be traveled inside that first item.</param>
		/// <returns>The item if found, else returns null</returns>
		public CustomTreeItem GetTreeItemByPath(string sPath, bool bAssumeRoot = false)
		{
			var splitPath = sPath.Split('\\');
			
			if (splitPath.Length == 0)
				return null;
			
			CustomTreeItem rootItem = null;
			
			if (bAssumeRoot)
			{
				foreach (CustomTreeItem item in Items)
				{
					if (item.IdentificationName == splitPath[0])
					{
						rootItem = item;
						break;
					}
				}
			}
			else
			{
				if (Items.Count > 0)
					rootItem = Items[0] as CustomTreeItem;
			}
			
			// If the assumed root doesn't exist the path is invalid
			if (rootItem == null)
				return null;
			
			// Not neccessary but better for reading
			CustomTreeItem currentItem = rootItem;
			
			for (int i = bAssumeRoot ? 1 : 0; i < splitPath.Length; ++i)
			{
				currentItem = currentItem.GetChildByIdentName(splitPath[i]);
				
				
				// If along the way an item can't be found it doesn't exist and therefore the path is invalid
				if (currentItem == null)
					return null;
			}
			
			// At this point currentItem is the item we looked for
			return currentItem;
		}
		
		
		
		
		/// <summary>
		/// Assigns common key combinations to the tree view (CTRL - C etc)
		/// </summary>
		public void SetupContextMenuShortCuts()
		{
			KeyDown += delegate(object sender, System.Windows.Input.KeyEventArgs e) 
			{
				if (e.Key == System.Windows.Input.Key.Delete)
				{
					OnContextDeleteClicked(null, null);
				}
				else if (e.Key == System.Windows.Input.Key.F2)
				{
					OnContextRenameClicked(null, null);
				}
				else if ((e.Key == System.Windows.Input.Key.C) && 
				         ((System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)) ||
				          (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl))))
				{
					OnContextCopyClicked(null, null);
				}
				else if ((e.Key == System.Windows.Input.Key.X) && 
				         ((System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)) ||
				          (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl))))
				{
					OnContextCutClicked(null, null);
				}
				else if ((e.Key == System.Windows.Input.Key.V) && 
				         ((System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl)) ||
				          (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl))))
				{
					OnContextPasteClicked(null, null);
				}
			};
		}
		
		#endregion
		
		#region Private methods
		
		
		
		
		void SearchDirectory()
		{
			Items.Clear();
			var topLevel = TreeItemFactory.CreateCustomTreeItemInstance();
			topLevel.IsDirectory = true;
			
			var dirInf = new DirectoryInfo(WatchDir);
			
			topLevel.FullPathToReference = WatchDir;
			topLevel.IdentificationName = dirInf.Name;
			topLevel.MakeHeader();
			topLevel.Items.Add(CustomTreeItem.CreateNewDummyItem());
			
			topLevel.RefreshContextMenu(GlobalContextMenu);
			
			Items.Add(topLevel);
			
			topLevel.IsExpanded = true;
		}

		void HandleKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			// Only process if no system key (ctrl etc is pressed) since the are meant to trigger shortcuts
			if (e.SystemKey == Key.None)
			{
				var item = SelectedItem as CustomTreeItem;
				
				if (item != null)
				{
					var parent  = item.GetParentSave();
					
					var converter = new KeyConverter();
					
					char c = (char) converter.ConvertFrom(e.Key);
					
					parent.SelectNextChildStartingWith(c);
				}
				
			}
		}
		
		
		#region Filesystemwatcher handling
		void OnEntryChanged(object sender, FileSystemEventArgs e)
		{
			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Created:
					{
						bool bIsDirectory = Directory.Exists(e.FullPath);

						if (bIsDirectory)
						{
							var dirInfo = new DirectoryInfo(e.FullPath);

							string sRelParenPath = TrimFilePath(dirInfo.Parent.FullName);

							Dispatcher.BeginInvoke(new AddItemDelegate(AddToItem), sRelParenPath, e.FullPath, bIsDirectory);
						}
						else
						{
							var fileInf = new FileInfo(e.FullPath);

							string sRelParentPath = TrimFilePath(fileInf.Directory.FullName);
							Parent.Dispatcher.BeginInvoke(new AddItemDelegate(AddToItem), sRelParentPath, e.FullPath, bIsDirectory);
						}

						
					}break;
				case WatcherChangeTypes.Deleted:
					{
						bool bIsDirectory = Directory.Exists(e.FullPath);

						if (bIsDirectory)
						{
							var dirInfo = new DirectoryInfo(e.FullPath);

							string sRelParenPath = TrimFilePath(dirInfo.Parent.FullName);

							Dispatcher.BeginInvoke(new RemoveItemDelegate(RemoveItem), sRelParenPath, dirInfo.Name);
						}
						else
						{
							var fileInf = new FileInfo(e.FullPath);

							string sRelParentPath = TrimFilePath(fileInf.Directory.FullName);
							Parent.Dispatcher.BeginInvoke(new RemoveItemDelegate(RemoveItem), sRelParentPath, fileInf.Name);
						}

					}break;
				default:
					throw new Exception("Invalid value for WatcherChangeTypes");
			}
		}
		
		void OnEntryRenamed(object sender, RenamedEventArgs e)
		{
			string sTrimmedPath = TrimFilePath(e.OldFullPath);
			
			Dispatcher.BeginInvoke(DispatcherPriority.Normal,
			                       new RenameItemDelegate(RenameItem),
			                       sTrimmedPath,
			                       e.Name,
			                       e.FullPath);
			
		}
		
		/// <summary>
		/// Internally used by the file system watcher entities
		/// </summary>
		/// <param name="sParentTreePath"></param>
		/// <param name="sFullPathToEntry"></param>
		/// <param name="bIsDirectory"></param>
		void AddToItem(string sParentTreePath, string sFullPathToEntry, bool bIsDirectory)
		{
			var parentItem = GetTreeItemByPath(sParentTreePath, true);
			
			// If the item is null, then the entry doesn't exist meaning it is not shown and does not need to be updated
			if (parentItem != null && parentItem.IsExpanded == true)
			{
				var itemToAdd = TreeItemFactory.CreateCustomTreeItemInstance();
				
				// We can safely assume that this is a valid path since only the file system watcher call this method
				var splitPath = sFullPathToEntry.Split('\\');
				
				itemToAdd.IdentificationName = splitPath[splitPath.Length - 1];
				itemToAdd.FullPathToReference = sFullPathToEntry;
				itemToAdd.IsDirectory = bIsDirectory;
				if (bIsDirectory)
					itemToAdd.Items.Add(CustomTreeItem.CreateNewDummyItem());
				itemToAdd.MakeHeader();
				
				
				parentItem.Items.Add(itemToAdd);
				parentItem.RefreshLiveSorting();
				parentItem.Items.Refresh();
			}
		}
		
		/// <summary>
		/// Internally used by the file system watcher entities
		/// </summary>
		/// <param name="sPathToParent"></param>
		/// <param name="sIdentName"></param>
		void RemoveItem(string sPathToParent, string sIdentName)
		{
			var parentItem = GetTreeItemByPath(sPathToParent, true);
			
			if (parentItem != null && parentItem.IsExpanded)
			{
				parentItem.RemoveChildByIdentName(sIdentName);
				parentItem.RefreshLiveSorting();
				parentItem.Items.Refresh();
			}
		}
		
		void RenameItem(string sTreePathToItem, string sNewName, string sFullPathToRenamed)
		{
			var item = GetTreeItemByPath(sTreePathToItem, true);
			
			if (item != null)
			{
				var parent = item.Parent as CustomTreeItem;
				
				if (parent != null)
				{
					item.IdentificationName = sNewName;
					item.FullPathToReference = sFullPathToRenamed;
					item.MakeHeader();
					
					parent.RefreshLiveSorting();
					parent.Items.Refresh();
				}
			}
		}
		
		#endregion
		/// <summary>
		/// Trims a path so it can act as path inside the tree view. Only works for paths inside the WatchDir.
		/// Includes the watched dir itself into the trimmed path
		/// </summary>
		/// <param name="sPathToTrim"></param>
		/// <returns>The trimmed path or, if failed the complete path</returns>
		string TrimFilePath(string sPathToTrim)
		{
			string sTrimmedPath = WatchDir.TrimEnd('\\');
			
			string sPathToParent = sTrimmedPath.Substring(0, sTrimmedPath.LastIndexOf('\\'));
			int pos = sPathToTrim.IndexOf(sPathToParent);

			return pos == 0 ? sPathToTrim.Substring(sPathToParent.Length, sPathToTrim.Length - sPathToParent.Length).Trim('\\') : sPathToTrim;
		}
		
		#region tree item context menu
		public  void SetupGlobalContextMenu()
		{
			if (GlobalContextMenu == null)
				GlobalContextMenu = new ContextMenu();
			
			var contextMenuItem = new MenuItem();
			var sep = new Separator();
			
			// Copy
			contextMenuItem.Header = Properties.Resources.ButtonCopy;
			contextMenuItem.Click += OnContextCopyClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			// Cut
			contextMenuItem = new MenuItem();
			contextMenuItem.Header = Properties.Resources.ButtonCut;
			contextMenuItem.Click += OnContextCutClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			// Paste
			contextMenuItem = new MenuItem();
			contextMenuItem.Header = Properties.Resources.ButtonPaste;
			contextMenuItem.Click += OnContextPasteClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			// Rename
			contextMenuItem = new MenuItem();
			contextMenuItem.Header = Properties.Resources.ButtonRename;
			contextMenuItem.Click += OnContextRenameClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			// Delete
			contextMenuItem = new MenuItem();
			contextMenuItem.Header = Properties.Resources.ButtonDelete;
			contextMenuItem.Click += OnContextDeleteClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			GlobalContextMenu.Items.Add(sep);
			
			// Create new dir
			contextMenuItem = new MenuItem();
			contextMenuItem.Header = Properties.Resources.ButtonNewDirectory;
			contextMenuItem.Click += OnContextAddDirClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			// Open in explorer
			contextMenuItem = new MenuItem();
			contextMenuItem.Header = Properties.Resources.ButtonOpenInExplorer;
			contextMenuItem.Click += OnContextOpenInExplorerClicked;
			GlobalContextMenu.Items.Add(contextMenuItem);
			
			
			
		}
		
		void  OnContextCopyClicked(object sender, RoutedEventArgs e)
		{
			var selectedItem = SelectedItem as CustomTreeItem;
			
			if (selectedItem != null)
			{
				StringCollection coll = new StringCollection();
				
				coll.Add(selectedItem.FullPathToReference);
				Clipboard.SetFileDropList(coll);
			}
		}
		
		void  OnContextCutClicked(object sender, RoutedEventArgs e)
		{
			OnContextCopyClicked(null, null);
			m_bIsFileCut = true;
		}
		
		void  OnContextPasteClicked(object sender, RoutedEventArgs e)
		{
			if (!Clipboard.ContainsFileDropList())
				return;
			
			var targetItem = SelectedItem as CustomTreeItem;
			
			if (!targetItem.IsDirectory)
			{
				targetItem = targetItem.GetParentSave() as CustomTreeItem;
			}
			
			//string fileSource = Clipboard.GetFileDropList()[0];
			
			foreach (string fileSource in Clipboard.GetFileDropList())
			{
				FileInfo fileInfo = new FileInfo(fileSource);
				
				try
				{
					if (fileInfo.Exists)
					{
						
						string fileTarget = targetItem.FullPathToReference + "\\" + fileInfo.Name;
						
						
						//File.Copy(fileSource, fileTarget, true);
						
						bool bOverride = false;
						
						// This is a case that does not seem to be handled by FileIO.CopyFile
						if (fileInfo.FullName == fileTarget)
						{
							var res = System.Windows.Forms.MessageBox.Show(Properties.Resources.AskOverrideOrRename,
							                                               Properties.Resources.CommonNotice,
							                                               System.Windows.Forms.MessageBoxButtons.YesNoCancel,
							                                               System.Windows.Forms.MessageBoxIcon.Exclamation);
							
							if (res == System.Windows.Forms.DialogResult.Yes)
							{
								int dotPos = fileTarget.LastIndexOf('.');
								
								fileTarget = fileTarget.Insert(dotPos, Properties.Resources.ButtonCopy);
							}
							else if (res == System.Windows.Forms.DialogResult.No)
							{
								bOverride = true;
							}
							else
								return;
						}
						
						
						if (m_bIsFileCut)
						{
							FileSystem.MoveFile(fileSource, fileTarget, UIOption.AllDialogs, UICancelOption.DoNothing);
						}
						else
						{
							OmgUtils.ProcessUt.ProcessUtils.CopyFile(fileSource, fileTarget, bOverride);
						}
						
					}
					else
					{
						// It's a direcory?
						DirectoryInfo dirInf = new DirectoryInfo(fileSource);
						if (dirInf.Exists)
						{
							string targetDir = targetItem.FullPathToReference;
							
							bool bOverride = false;
							string sCustomName = dirInf.Name;
							targetDir += "\\" + sCustomName;
							
							if (dirInf.FullName == targetDir)
							{
								var res = System.Windows.Forms.MessageBox.Show(Properties.Resources.AskOverrideOrRename,
								                                               Properties.Resources.CommonNotice,
								                                               System.Windows.Forms.MessageBoxButtons.YesNoCancel,
								                                               System.Windows.Forms.MessageBoxIcon.Exclamation);
								
								if (res == System.Windows.Forms.DialogResult.Yes)
								{
									sCustomName = dirInf.Name + " - " + Properties.Resources.ButtonCopy;
								}
								else if (res == System.Windows.Forms.DialogResult.No)
								{
									bOverride = true;
								}
								else
									return;
							}
							
							targetDir = targetItem.FullPathToReference + "\\" + sCustomName;
							
							
							
							if (!Directory.Exists(targetDir))
								Directory.CreateDirectory(targetDir);
							
							if (m_bIsFileCut)
							{
								OmgUtils.ProcessUt.ProcessUtils.MoveDirectory(dirInf.FullName, targetDir, bOverride);
							}
							else
							{
								OmgUtils.ProcessUt.ProcessUtils.CopyDirectory(dirInf.FullName, targetDir, false);
							}
						}
					}
					
					m_bIsFileCut = false;
				}
				catch (Exception ex)
				{
					
					OmgUtils.UserInteraction.UserInteractionUtils.ShowErrorMessageBox(ex.Message, Properties.Resources.CommonError);
					m_bIsFileCut = false;
				}
			}
		}
		
		void  OnContextDeleteClicked(object sender, RoutedEventArgs e)
		{
			var item = SelectedItem as CustomTreeItem;
			
			if (item != null && !item.IsVirtual)
			{
				try
				{
					if (item.IsDirectory)

						FileSystem.DeleteDirectory(item.FullPathToReference, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
					else
						FileSystem.DeleteFile(item.FullPathToReference, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
					
				}
				catch (Exception ex)
				{
					OmgUtils.UserInteraction.UserInteractionUtils.ShowErrorMessageBox(ex.Message, Properties.Resources.CommonError);
				}
			}
		}
		
		void  OnContextOpenInExplorerClicked(object sender, RoutedEventArgs e)
		{
			if (SelectedItem != null)
			{
				var item = SelectedItem as CustomTreeItem;
				
				if (item == null)
					return;
				
				if (!item.IsDirectory)
					item = item.GetParentSave();

				Process proc = new Process();
				proc.StartInfo = new ProcessStartInfo("explorer", item.FullPathToReference);
				proc.Start();
			}
		}
		
		void  OnContextRenameClicked(object sender, RoutedEventArgs e)
		{
			if (SelectedItem != null)
			{
				var item = SelectedItem as CustomTreeItem;
				
				if (item != null)
				{
					OmgUtils.UserInteraction.UserInteractionUtils.AskUserToEnterString(Properties.Resources.CommonNotice,
					                                                                   Properties.Resources.RenameMessage, 
					                                                                   new OmgUtils.UserInteraction.UserInteractionUtils.UserFinishedEnteringStringDelegate(RenameItemFromUserEntry),
																						Properties.Resources.CommonOK,
																						Properties.Resources.CommonCancel,		
					                                                                   item.IdentificationName);
				}
			}
		}
		
		void RenameItemFromUserEntry(TextBox boxWithNewName, MessageBoxResult res)
		{
			if (SelectedItem != null && res == MessageBoxResult.OK)
			{
				if (!String.IsNullOrWhiteSpace(boxWithNewName.Text))
				{
					if (boxWithNewName.Text.IndexOf('.') != -1)
					{
						var resu = MessageBox.Show(Properties.Resources.AskOverrideExtension, Properties.Resources.CommonNotice,
						                          MessageBoxButton.OKCancel, MessageBoxImage.Question);
						
						if (resu == MessageBoxResult.Cancel)
							return ;
						else if (resu == MessageBoxResult.No)
						{
							boxWithNewName.Text = boxWithNewName.Text.Substring(0, boxWithNewName.Text.IndexOf('.'));
						}
					}
					
					var item = SelectedItem as CustomTreeItem;
					
					try
					{
						if (item != null)
						{
							if (item.IsDirectory)
								FileSystem.RenameDirectory(item.FullPathToReference, boxWithNewName.Text);
							else
							{
								if (boxWithNewName.Text.IndexOf('.') == -1)
								{
									var fileInf = new FileInfo(item.FullPathToReference);
									boxWithNewName.Text += fileInf.Extension;
								}
								FileSystem.RenameFile(item.FullPathToReference, boxWithNewName.Text);
							}
						}
					}
					catch (Exception e)
					{
						
						OmgUtils.UserInteraction.UserInteractionUtils.ShowErrorMessageBox(e.Message, Properties.Resources.CommonError);
					}
				}
			}
			return;
		}
		
		void  OnContextAddDirClicked(object sender, RoutedEventArgs e)
		{
			OmgUtils.UserInteraction.UserInteractionUtils.AskUserToEnterString(Properties.Resources.CommonNotice,
			                                                                   Properties.Resources.DirectoryMessage,
			                                                                   new OmgUtils.UserInteraction.UserInteractionUtils.UserFinishedEnteringStringDelegate(AddNewDirFromUserEntry),
			                                                                  Properties.Resources.CommonOK,
			                                                                 Properties.Resources.CommonCancel);
		}
		
		void AddNewDirFromUserEntry(TextBox boxWithName, MessageBoxResult res)
		{
			if (res == MessageBoxResult.Cancel)
				return;
			if (SelectedItem != null && !String.IsNullOrWhiteSpace(boxWithName.Text))
			{
				var item = SelectedItem as CustomTreeItem;
				
				if (item != null)
				{
					if (!item.IsDirectory)
					{
						item = item.GetParentSave();
					}
					
					string newDir = item.FullPathToReference + "\\" + boxWithName.Text;
					
					try
					{
						FileSystem.CreateDirectory(newDir);
					} 
					catch (Exception e)
					{
						OmgUtils.UserInteraction.UserInteractionUtils.ShowErrorMessageBox(e.Message, Properties.Resources.CommonError);
					}
				}
			}
			
			return ;
		}
		
		#endregion
		#endregion
		
	}
}