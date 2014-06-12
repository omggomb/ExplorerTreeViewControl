/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 04.06.2014
 * Time: 22:35
 */
using System;

namespace ExplorerTreeView
{
	/// <summary>
	/// Class used to compare two tree items against each other.
	/// Needed since live sorting is updating according to a atrribute of the items, as which
	/// this class will serve
	/// </summary>
	public class ItemComparer : IComparable
	{
		#region Attributes
		
		/// <summary>
		/// The name that is used to identify a tree item.
		/// It should always be the name of the file or folder that is represented 
		/// by the tree view item.
		/// </summary>
		public string sIdentificationName;
		
		/// <summary>
		/// Is the tree item a directory?
		/// </summary>
		public bool bIsDirectory;
		
		#endregion
		
		#region CTOR
		public ItemComparer()
		{
			sIdentificationName = "";
			bIsDirectory = false;
		}
		#endregion
		
		#region Public methods
		
		/// <summary>
		/// Compares the given object with this one.
		/// Directories go before files. 
		/// If both are of the same type (dir or file) the identification name is compared alphabetically.
		/// </summary>
		/// <param name="compareToThis"></param>
		/// <returns></returns>
		public int CompareTo(object compareToThis)
		{
			var compareCast = compareToThis as ItemComparer;
			
			if (compareCast == null)
				return 1;
			
			if (bIsDirectory == true && compareCast.bIsDirectory == false)
				return -1;
			else if (bIsDirectory == false && compareCast.bIsDirectory == true)
				return 1;
			else
				return sIdentificationName.CompareTo(compareCast.sIdentificationName);		
		}
		
		#endregion
	}
}
