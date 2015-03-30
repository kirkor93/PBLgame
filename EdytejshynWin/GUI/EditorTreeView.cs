using System.Drawing;
using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn.GUI
{
    public class EditorTreeView : TreeView
    {

        public EditorTreeView()
        {
        }

        #region Commands
        public class RemoveNodeCommand : ICommand
        {
            private readonly TreeView _treeView;
            private readonly TreeNode _node;
            private readonly TreeNode _parent;
            private readonly int _index;
            private readonly TreeNodeCollection _nodes;
            private readonly string _path;

            public RemoveNodeCommand(TreeNode node)
            {
                _node = node;
                _treeView = node.TreeView;
                _parent = _node.Parent;
                _path = _node.FullPath;
                _index = _node.Index;
                _nodes = (_parent == null) ? _treeView.Nodes : _parent.Nodes;
            }

            public string Description
            {
                get { return "Remove node"; }
            }

            public string Message
            {
                get { return string.Format("Remove node: {0}", _path); }
            }

            public void Do()
            {
                _nodes.RemoveAt(_index);
                _treeView.SelectedNode = null;
            }

            public void Undo()
            {
                _nodes.Insert(_index, _node);
                _node.EnsureVisible();
                _treeView.SelectedNode = _node;
            }
        }
        #endregion

        public ICommand GetRemoveSelectedNodeCommand()
        {
            return new RemoveNodeCommand(this.SelectedNode);
        }
    }
}