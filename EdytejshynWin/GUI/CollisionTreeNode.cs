using System.ComponentModel;
using System.Windows.Forms;
using Edytejshyn.Model;
using PBLgame.Engine.Physics;

namespace Edytejshyn.GUI
{
    public class CollisionTreeNode : TreeNode
    {
        public ColliderWrapper WrappedCollider { get; private set; }

        public CollisionTreeNode(ColliderWrapper collider) : base(collider.ToString())
        {
            WrappedCollider = collider;
        }

        public CollisionTreeNode(string text) : base(text)
        {
        }
    }
}