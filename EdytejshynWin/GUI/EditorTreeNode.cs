using System.Windows.Forms;
using Edytejshyn.Logic;

namespace Edytejshyn.GUI
{
    public class EditorTreeNode : TreeNode
    {
        public object Data { get; private set; }

        public EditorTreeNode(string text, TreeNode[] children) : base(text, children)
        {
        }

        public EditorTreeNode(string text) : base(text)
        {
        }

        public EditorTreeNode(string text, object data) : base(text)
        {
            Data = data;
        }


    }
}