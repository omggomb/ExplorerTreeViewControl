/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 06/02/2014
 * Time: 12:44
 */
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;

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
				return WatchDir;
			}
			set
			{
				if (String.IsNullOrWhiteSpace(value) || !Directory.Exists(value))
				{
					throw new ArgumentException("The provided watch dir string is null, empty or not a directory!");
				}
				else
				{
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
		
		/**
		 * CTOR
		*/
		
		/// <summary>
		/// Creates a new instance of the explorer tree view control.
		/// IMPORTANT: This does not do any fill operations. You need to call <see cref="RefreshTree"/> at least once!
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
			
			
			
			WatchDir = sPathToWatch;
			IsWatching = bStartNow;
		}
		
		/*
		 * Public methods
		*/
		
		/// <summary>
		/// Travels through the WatchDir and its subdirectories and fills the tree view with the files and 
		/// folders found. This does a complete refill, which is likely to take a lot of time!
		/// </summary>
		public void RefreshTree()
		{
			
		}
		
		
	
		
	}
}