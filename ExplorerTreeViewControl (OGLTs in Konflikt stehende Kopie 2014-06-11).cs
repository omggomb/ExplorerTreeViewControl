/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 06/02/2014
 * Time: 12:44
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


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
		
	
		
			string m_sWatchDir;
			
			delegate void AddItemDelegate(string sParetTreePath, string sFullPathToEntry, bool bIsDirectory);
		
		delegate void RemoveItemDelegate(string sParentTreePath, string sIdentName);
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
			// Filter gets set by WatchDir propety
			ToplevelFileWatcher.IncludeSubdirectories = true;
			
			SubdirFileWatcher.NotifyFilter = ToplevelFileWatcher.NotifyFilter;
			SubdirFileWatcher.Filter = "*.*";
			SubdirFileWatcher.IncludeSubdirectories = true;
			
			SubdirFileWatcher.Created += OnEntryChanged;
			SubdirFileWatcher.Deleted += OnEntryChanged;
			SubdirFileWatcher.Renamed += OnEntryRenamed;
			
			
			WatchDir = sPathToWatch;
			IsWatching = bStartNow;
			
		
		}

	

	
		
		public ExplorerTreeViewControl () : this("C:\\", false)
		{
		}
		
		#region Public methods
		
		/// <summary>
		/// Does a primer search and displays the contents of the WatchDir. Not recursive.
		/// </summary>
		public void InitializeTree()
		{
			SearchDirectory();
		}
		
		/// <summary>
		/// Adds an element to the treeview that does not exist on a physical drive.
		/// Does not perform item creation if the desired parent does not exist.
		/// </summary>
		/// <param name="sRelativePath">Path to the desired parent item</param>
		/// <param name="bAssumeRoot"><see cref="GetTreeItemByPath"/></param>
		/// <param name="itemToBeAdded">The item to be added to the parent item</param>
		/// <returns>True if successfully added, else false</returns>
		public bool AddVirtualElement(string sRelativePath, bool bAssumeRoot, CustomTreeItem itemToBeAdded)
		{
			var parentItem = GetTreeItemByPath(sRelativePath, bAssumeRoot);
			
			if (parentItem != null)
			{
				itemToBeAdded.IsVirtual = true;
				parentItem.Items.Add(itemToBeAdded);
				return true;
			}
			else
				return false;
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
		
		#endregion
		
	#region Private methods
		
		
		
		
		void SearchDirectory()
		{	
			var topLevel = new CustomTreeItem();
			topLevel.IsDirectory = true;
			
			var dirInf = new DirectoryInfo(WatchDir);
			
			topLevel.FullPathToReference = WatchDir;
			topLevel.IdentificationName = dirInf.Name;
			topLevel.Header = topLevel.IdentificationName;
			topLevel.Items.Add(CustomTreeItem.s_dummyItemForFolders);
			
			Items.Add(topLevel);
			
			topLevel.IsExpanded = true;	
		}
		
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
			
		}
		
		void AddToItem(string sParentTreePath, string sFullPathToEntry, bool bIsDirectory)
		{
			var parentItem = GetTreeItemByPath(sParentTreePath);
			
			// If the item is null, then the entry doesn't exist meaning it is not shown and does not need to be updated
			if (parentItem != null && parentItem.IsExpanded == true)
			{
				var itemToAdd = new CustomTreeItem();
				
				// We can safely assume that this is a valid path since only the file system watcher call this method
				var splitPath = sFullPathToEntry.Split('\\');
				
				itemToAdd.IdentificationName = splitPath[splitPath.Length - 1];
				itemToAdd.FullPathToReference = sFullPathToEntry;
				itemToAdd.IsDirectory = bIsDirectory;
				if (bIsDirectory)
					itemToAdd.Items.Add(CustomTreeItem.s_dummyItemForFolders);
				itemToAdd.MakeHeader();
				
				
				parentItem.Items.Add(itemToAdd);
				parentItem.RefreshLiveSorting();
				parentItem.Items.Refresh();
			}
		}
		
		
		void RemoveItem(string sPathToParent, string sIdentName)
		{
			var parentItem = GetTreeItemByPath(sPathToParent);
			
			if (parentItem != null && parentItem.IsExpanded)
			{
				parentItem.RemoveChildByIdentName(sIdentName);
				parentItem.RefreshLiveSorting();
				parentItem.Items.Refresh();
			}
		}
		
		string TrimFilePath(string sPathToTrim)
		{
			int pos = sPathToTrim.IndexOf(WatchDir);

            if (pos == 0)
            {
                return sPathToTrim.Substring(WatchDir.Length, sPathToTrim.Length - WatchDir.Length);
            }
            else
                return sPathToTrim;
		}
	
			
		#endregion
		
	}
}