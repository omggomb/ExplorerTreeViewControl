using ExplorerTreeView;

/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 15.06.2014
 * Time: 11:29
 */

using System;
using System.Windows;
using System.Windows.Media.Imaging;

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
            // Using TreeViewItem supplied by the control.
            // If you want to change the context menu, use ExplorerTreeView.GloablContextMenu
            return new CustomTreeItem();
        }

        public System.Windows.Controls.Image CreateFolderIconImage(CustomTreeItem itemThatIsUsed)
        {
            var image = new System.Windows.Controls.Image();

            var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(Properties.Resource1.SimpleFolderIcon.Handle,
                                                                             Int32Rect.Empty,
                                                                               BitmapSizeOptions.FromEmptyOptions());

            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.Source = src;
            return image;
        }

        #endregion TreeItemFactory implementation
    }
}