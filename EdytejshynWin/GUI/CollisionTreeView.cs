using System.Windows.Forms;
using Edytejshyn.Model;
using PBLgame.Engine.Physics;

namespace Edytejshyn.GUI
{
    public class CollisionTreeView : TreeView
    {
        public MainForm MainForm;
        private CollisionWrapper _collision;

        public CollisionTreeView()
        {
            if (DesignMode) return;
            ReloadTree();
        }

        public CollisionWrapper Collision
        {
            get { return _collision; }
            set
            {
                _collision = value;
                ReloadTree();
            }
        }

        public void ReloadTree()
        {
            Nodes.Clear();
            if (Collision == null)
            {
                TreeNode empty = new TreeNode("[No colllision component]");
                Nodes.Add(empty);
                return;
            }
            TreeNode mainColliderNode = new CollisionTreeNode("Main collider");
            mainColliderNode.Nodes.Add(new CollisionTreeNode(new SphereColliderWrapper(_collision, _collision.WrappedCollision.MainCollider)));
            Nodes.Add(mainColliderNode);
            TreeNode sphereNode = new CollisionTreeNode("Spheres");
            foreach (SphereCollider collider in _collision.WrappedCollision.SphereColliders)
            {
                sphereNode.Nodes.Add(new CollisionTreeNode(new SphereColliderWrapper(_collision, collider)));
            }
            Nodes.Add(sphereNode);
            
            TreeNode boxNode = new CollisionTreeNode("Boxes");
            foreach (BoxCollider collider in _collision.WrappedCollision.BoxColliders)
            {
                boxNode.Nodes.Add(new CollisionTreeNode(new BoxColliderWrapper(_collision, collider)));
            }
            Nodes.Add(boxNode);


        }

    }
}