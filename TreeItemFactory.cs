/*
 * Created by SharpDevelop.
 * User: Ihatenames
 * Date: 12.06.2014
 * Time: 16:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Controls;

namespace ExplorerTreeView
{
	/// <summary>
	/// This interface can be used to hook directly into the tree items
	/// </summary>
	public interface TreeItemFactory
	{
		/// <summary>
		/// Create a new instance of CustomTreeItem
		/// </summary>
		/// <returns></returns>
		CustomTreeItem CreateCustomTreeItemInstance();
		
		/// <summary>
		/// Creates a new image to be used as folder icon inside the tree view
		/// </summary>
		/// <param name="itemThatIsUsed">The item that the folder icon will be added to </param>
		/// <returns></returns>
		Image CreateFolderIconImage(CustomTreeItem itemThatIsUsed);
		
	}
	
}
