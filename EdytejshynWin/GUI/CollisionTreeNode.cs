using System.ComponentModel;
using System.Windows.Forms;
using Edytejshyn.Model;
using PBLgame.Engine.Physics;

namespace Edytejshyn.GUI
{
    public class CollisionTreeNode : TreeNode
    {
        public ColliderWrapper WrappedCollider { get; private set; }

        public bool IsMainCollider
        {
            get
            {
                CollisionTreeView treeView = TreeView as CollisionTreeView;
                if (treeView == null || WrappedCollider == null) return false;
                SphereColliderWrapper sphere = WrappedCollider as SphereColliderWrapper;
                if (sphere == null) return false;
                return (sphere.Collider == treeView.Collision.WrappedCollision.MainCollider);
            }
        }

        public CollisionTreeNode(ColliderWrapper collider) : base(collider.ToString())
        {
            WrappedCollider = collider;
        }

        public CollisionTreeNode(string text) : base(text)
        {
        }
    }
}