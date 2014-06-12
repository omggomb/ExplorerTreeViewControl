/*
 * Created by SharpDevelop.
 * User: Ihatenames
 * Date: 12.06.2014
 * Time: 16:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

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
		
	}
}
