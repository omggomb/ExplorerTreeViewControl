/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 05.06.2014
 * Time: 11:52
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ExplorerTreeView
{
	/// <summary>
	/// Class used to serach directories and fill finding into a FileSystemEntry struct.
	/// </summary>
	public class DirectorySearcher
	{
		
		
		/// <summary>
		/// Called when searching has finished
		/// </summary>
		public delegate void FinishedSearchingDelegate();
		FinishedSearchingDelegate m_callbackDelegate;
		
		/// <summary>
		/// The top level entry of the found entries
		/// </summary>
		public FileSystemEntry FoundEntry {get; private set;}
		
		
		/// <summary>
		/// Directory to be searched
		/// </summary>
		string m_sDirToSearch;
		
		public DirectorySearcher()
		{
		}
		
		public void BeginSearch(string sTopLevelDir, FinishedSearchingDelegate callback)
		{
			m_callbackDelegate = callback;
			m_sDirToSearch = sTopLevelDir;
			
			if (!Directory.Exists(m_sDirToSearch))
				throw new ArgumentException(string.Format("Specified path to search: {0} does not exist!", sTopLevelDir));
			
			var thread = new Thread(new ThreadStart(DoSearch));
			thread.Start();
		}
		
		void DoSearch()
		{
			var topDir = new DirectoryInfo(m_sDirToSearch);
			
			var entry = new FileSystemEntry(topDir.FullName, topDir.Name, true, null);
			
			
			SearchSubDir(topDir, ref entry);
			
			FoundEntry = entry;
			
			m_callbackDelegate();
		}
		
		void SearchSubDir(DirectoryInfo dir, ref FileSystemEntry fillThis)
		{
			Thread.Sleep(1);
			
			foreach (var dirInf in dir.GetDirectories())
			{
				var dirEntry = new FileSystemEntry(dirInf.FullName, dirInf.Name, true, /*Properties.
				                                    
				                                    */ null);
				
				
				SearchSubDir(dirInf, ref dirEntry);
				
				fillThis.SubEntries.Add(dirEntry);
			}
			
			foreach (var fileInf in dir.GetFiles()) 
			{
				
				var icon = Icon.ExtractAssociatedIcon(fileInf.FullName);
				
				var fileEntry = new FileSystemEntry(fileInf.FullName,
											fileInf.Name,
											false,
											System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, 
												Int32Rect.Empty,
                                            	BitmapSizeOptions.FromEmptyOptions())
											);
				
				
				fillThis.SubEntries.Add(fileEntry);
			}
		}
	}
}
