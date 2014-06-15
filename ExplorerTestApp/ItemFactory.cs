/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 15.06.2014
 * Time: 11:29
 */
using System;
using ExplorerTreeView;

namespace ExplorerTestApp
{
	/// <summary>
	/// Description of ItemFactory.
	/// </summary>
	public class ItemFactory : ExplorerTreeView.TreeItemFactory
	{
		public ItemFactory()
		{
		}

		#region TreeItemFactory implementation

		public CustomTreeItem CreateCustomTreeItemInstance()
		{
			return new CustomTreeItem();
		}

		public System.Windows.Controls.Image CreateFolderIconImage(CustomTreeItem itemThatIsUsed)
		{
			return null;
		}

		#endregion
	}
}
