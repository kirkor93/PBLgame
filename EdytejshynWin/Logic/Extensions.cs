using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Edytejshyn.Logic
{
    public static class Extensions
    {
        /// <summary>
        /// Inserts treeView node on specified position or at the end when index == -1
        /// </summary>
        /// <param name="nodes">nodes of TreeView</param>
        /// <param name="index">index position to insert to (0-based) or -1 when at the end</param>
        /// <param name="item">node to insert</param>
        public static void AddInsertNode(this TreeNodeCollection nodes, int index, TreeNode item)
        {
            if (index == -1) nodes.Add(item);
            else nodes.Insert(index, item);
        }
    }
}