/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 04.06.2014
 * Time: 22:47
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ExplorerTreeView
{
	public enum ContextMenuEntry : int
	{
		Copy = 0,
		Cut,
		Paste,
		Seperator,
		OpenInExplorer,
	}
	/// <summary>
	/// An enhanced tree item, that stores an idetifiation name as well as whether the item
	/// represents a directory or file.
	/// </summary>
	public class CustomTreeItem : TreeViewItem
	{
		#region Attributes
		
		/*
		 * Public
		 */
		
		/// <summary>
		/// The name this tree item is identified by.
		/// It is the name of the directory or file this item represents.
		/// </summary>
		public string IdentificationName
		{
			get
			{
				return m_itemComparerObject.sIdentificationName;
			}
			set
			{
				// If no custom display name is set, use the file name
				if (String.IsNullOrWhiteSpace(DisplayName))
					DisplayName = value;
				
				m_itemComparerObject.sIdentificationName = value;
			}
		}
		
		/// <summary>
		/// This name will be displayed inside the tree view.
		/// Used by <see cref="MakeHeader"/>.
		/// </summary>
		public string DisplayName {get; set;}
		
		/// <summary>
		/// Is this a directory or file?
		/// </summary>
		public bool IsDirectory
		{
			get
			{
				return m_itemComparerObject.bIsDirectory;
			}
			set
			{
				m_itemComparerObject.bIsDirectory = value;
			}
		}
		
		
		/// <summary>
		/// The comparer object that decides which item goes before which.
		/// </summary>
		public ItemComparer ComparerObject
		{
			get
			{
				return m_itemComparerObject;
			}
		}
		
		/// <summary>
		/// The full path to the directory or file this item represents
		/// </summary>
		public string FullPathToReference {get; set;}
		
		/// <summary>
		/// Indicates whether the tree item represents a virtual entry (does not exist on a physical drive).
		/// </summary>
		public bool IsVirtual {get; set; }
		
		
		
		
		
		
		
		/*
		 * Private
		 */
		
		/// <summary>
		/// Used for the live sort feature of the tree view item
		/// </summary>
		readonly ItemComparer m_itemComparerObject = new ItemComparer();
		
		
		
		bool m_bShouldUpdate = true;
		
		/// <summary>
		/// The character with with searched items should start with
		/// If with a new request it's the same, then we can use m_nLastSelectedIndex, else we
		/// need to search for the new char again
		/// </summary>
		char m_cLastCharSearchedFor;
		
		/// <summary>
		/// The index of the last selected item starting with m_cLastCharSearchedFor
		/// </summary>
		int m_nLastSelectedIndex;
		
		#endregion
		
		#region CTOR
		public CustomTreeItem()
		{
			IdentificationName = "";
			FullPathToReference = "";
			DisplayName = "";
			Items.IsLiveSorting = true;
			IsDirectory = false;
			IsVirtual = false;
			
			Expanded += HandleExpanded;
			
			Collapsed += delegate {
				if (!IsExpanded)
					m_bShouldUpdate = true;
			};
			
			MouseDoubleClick += delegate(object sender, MouseButtonEventArgs e)
			{
				if (!IsSelected || IsDirectory)
					return;
				var proc = new Process();
				var start = new ProcessStartInfo(FullPathToReference);
				
				proc.StartInfo = start;
				proc.Start();
			};

			MouseRightButtonDown += delegate(object sender, MouseButtonEventArgs e)
			{
				Focus();
				e.Handled = true;
			};
			
		}
		#endregion
		
		
		#region Public methods
		
		/// <summary>
		/// Retrieves a child item fromt the Item collection
		/// </summary>
		/// <param name="sIdentName">The identification name of the child wanted</param>
		/// <returns>If found, the child item, else null</returns>
		public CustomTreeItem GetChildByIdentName(string sIdentName)
		{
			foreach (CustomTreeItem child in Items)
			{
				if (child.IdentificationName == sIdentName)
					return child;
			}
			
			return null;
		}
		
		
		/// <summary>
		/// Deletes the specified child.
		/// </summary>
		/// <param name="sIdentName">The identification name of the child to be deleted</param>
		/// <returns>True if item was found and deleted, else false.</returns>
		public bool RemoveChildByIdentName(string sIdentName)
		{
			int i = 0;
			bool bFound = false;
			
			for (;i < Items.Count; ++i)
			{
				var child = Items[i] as CustomTreeItem;
				
				if (child != null)
				{
					if (child.IdentificationName == sIdentName)
					{
						bFound = true;
						break;
					}
				}
			}
			
			if (bFound)
			{
				Items.RemoveAt(i);
			}
			
			return bFound;
			
		}
		
		
		/// <summary>
		/// Redoes the live sorting sort rules, so added items will be sorted correctly
		/// </summary>
		public void RefreshLiveSorting()
		{
			var view  = CollectionViewSource.GetDefaultView(Items) as CollectionView;
			view.SortDescriptions.Clear();
			view.SortDescriptions.Add(new System.ComponentModel.SortDescription("ComparerObject", System.ComponentModel.ListSortDirection.Ascending));
			Items.Refresh();
		}
		
		public CustomTreeItem GetParentSave()
		{
			if (Parent == null)
				return this;
			else
				return Parent as CustomTreeItem;
		}
		
		/// <summary>
		/// Only works for directories!
		/// Searches the next item of which the display name starts with c, and if found selects it.
		/// </summary>
		/// <param name="c">Character with which the items should start</param>
		public void SelectNextChildStartingWith(char c)
		{
			// We'll only use it for directories
			if (!IsDirectory)
				return;
		
			
			if (c == m_cLastCharSearchedFor)
			{
				m_nLastSelectedIndex = SearchNextChildStartingWith(c, m_nLastSelectedIndex + 1);
			}
			else
			{
				m_nLastSelectedIndex = SearchNextChildStartingWith(c, 0);
				m_cLastCharSearchedFor = c;
			}
			
			// Only select next if found at all
			if (m_nLastSelectedIndex != -1) 
			{
				var itemToBeSeleted = Items[m_nLastSelectedIndex] as TreeViewItem;
				
				if (itemToBeSeleted != null)
					itemToBeSeleted.Focus();
				
			}
			
		}
		
		#endregion
		


		void HandleExpanded(object sender, System.Windows.RoutedEventArgs e)
		{
			
			if (!ShouldUpdateChildren())
				return;
			
			for (int i = Items.Count - 1; i >= 0; --i)
			{
				var item = Items[i] as CustomTreeItem;
				
				if (item != null)
				{
					if (item.IsVirtual == false)
					{
						Items.RemoveAt(i);
					}
				}
			}
			
			
			
			if (IsDirectory)
			{
				
				
				var dirInf = new DirectoryInfo(FullPathToReference);
				
				
				
				foreach (var dir in dirInf.GetDirectories())
				{
					var item = ExplorerTreeViewControl.TreeItemFactory.CreateCustomTreeItemInstance();
					item.Items.Add(CreateNewDummyItem());
					item.IsDirectory = true;
					item.IdentificationName = dir.Name;
					item.FullPathToReference = dir.FullName;
					item.MakeHeader();
					Items.Add(item);
					
				}
				
				foreach (var file  in dirInf.GetFiles())
				{
					var item = ExplorerTreeViewControl.TreeItemFactory.CreateCustomTreeItemInstance();
					item.FullPathToReference = file.FullName;
					item.IdentificationName = file.Name;
					item.IsDirectory = false;
					item.MakeHeader();
					
					Items.Add(item);
				}
				
				//Items.Refresh();
				
				
				
				m_bShouldUpdate = false;
				
				
			}
			
		}
		
		/// <summary>
		/// Creates the header stack panel of this item.
		/// </summary>
		/// <param name="oEntryImage">If not null, this image will be used instead of the files'  one</param>
		public virtual void MakeHeader(System.Windows.Controls.Image oEntryImage = null)
		{
			var stack = new StackPanel();
			stack.Orientation = Orientation.Horizontal;
			
			
			System.Windows.Controls.Image image = null;
			
			
			if (IsDirectory)
			{
				
				image = ExplorerTreeViewControl.TreeItemFactory.CreateFolderIconImage(this);
				
				if (image == null)
				{
					image = new System.Windows.Controls.Image();
					var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(Properties.Resource1.SimpleFolderIcon.Handle,
					                                                                     Int32Rect.Empty,
					                                                                     BitmapSizeOptions.FromEmptyOptions());
					image.Source = src;
				}
			}
			else
			{
				if (!IsVirtual)
				{
					var icon = Icon.ExtractAssociatedIcon(FullPathToReference);
					
					var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
					                                                                     Int32Rect.Empty,
					                                                                     BitmapSizeOptions.FromEmptyOptions());
					
					image = new System.Windows.Controls.Image();
					
					image.Source = src;
					
					
				}
				
				
			}
			
			stack.Children.Add(oEntryImage ?? image);
			
			string sNameToBeDisplayed = DisplayName;
			
			if (String.IsNullOrWhiteSpace(DisplayName))
				sNameToBeDisplayed = IdentificationName;
			
			var lbl = new Label();
			lbl.Content = sNameToBeDisplayed;
			
			stack.Children.Add(lbl);
			
			Header = stack;
		}
		
		public virtual void RefreshContextMenu(ContextMenu menu)
		{
			if (menu == null)
				return;
			
			ContextMenu = menu;
			
			foreach (CustomTreeItem element in Items)
			{
				element.RefreshContextMenu(menu);
			}
		}
		
		public static CustomTreeItem CreateNewDummyItem()
		{
			var item = ExplorerTreeViewControl.TreeItemFactory.CreateCustomTreeItemInstance();
			
			item.Name = "DummyFolderItem";
			item.IdentificationName = "DummyFolderItem";
			
			return item;

		}
		
		/// <summary>
		/// Indicates whether this tree item already has performed an update or needs yet to be filled
		/// </summary>
		/// <returns></returns>
		bool ShouldUpdateChildren()
		{
			// Reverted back to simple but slow solution since we don't update changed files in collapsed items
			
			return m_bShouldUpdate;
		}
		
		/// <summary>
		/// Searches the next child of which the display name starts with c
		/// </summary>
		/// <param name="c">The character to be searched for</param>
		/// <param name="nStartPos">Position at which to start searching</param>
		/// <returns>If found the first occurence after nStartPos, else -1</returns>
		int SearchNextChildStartingWith(char c, int nStartPos)
		{
			int nResult = -1;
			
			for (int i = nStartPos; i < Items.Count; ++i)
			{
				var item = Items.GetItemAt(i) as CustomTreeItem;
				
				if (item != null)
				{
					if (!String.IsNullOrWhiteSpace(item.DisplayName))
					{
						if (c == item.DisplayName[0])
						{
							nResult = i;
							break;
						}
						
					}
				}
			}
			
			// Need to do a new search from start
			if (nResult == -1)
			{
				// Can't use function here because of recursion
				for (int i = 0; i < Items.Count; ++i)
				{
					var item = Items.GetItemAt(i) as CustomTreeItem;
					
					if (item != null)
					{
						if (!String.IsNullOrWhiteSpace(item.DisplayName))
						{
							if (c == item.DisplayName[0])
							{
								nResult = i;
								break;
							}
							
						}
					}
				}
			}
			
			return nResult;
		}

		
		
		
	}
}
