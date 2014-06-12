/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 05.06.2014
 * Time: 12:02
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ExplorerTreeView
{
	/// <summary>
	/// Description of FileSystemEntry.
	/// </summary>
	public class FileSystemEntry
	{
		public string sFullPath;
		public string sIdentName;
		public bool bIsDirectory;
		public BitmapSource oDisplayIcon;
		
		public static BitmapSource FolderIconBitmapSource {get; set;}
		
		public List<FileSystemEntry> SubEntries {get; private set;}
		
		public FileSystemEntry() : this("", "", false, null)
		{
		}
		
		public FileSystemEntry(string sFullPath, string sIdentName, bool bIsDirectory, BitmapSource displayIcon)
		{
			this.sFullPath = sFullPath;
			this.sIdentName = sIdentName;
			this.bIsDirectory = bIsDirectory;
			this.oDisplayIcon = displayIcon;
			
			
			
			if (FolderIconBitmapSource == null)
				FolderIconBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(Properties.Resource1.SimpleFolderIcon.Handle,  Int32Rect.Empty,
                                                                                            BitmapSizeOptions.FromEmptyOptions());
			
			this.SubEntries = new List<FileSystemEntry>();
		}
		
		
		
	}
}
