using System.ComponentModel;
using System.Windows.Forms;
using Edytejshyn.Model;

namespace Edytejshyn.GUI
{
    public class SceneTreeNode : TreeNode
    {
        public GameObjectWrapper WrappedGameObject { get; private set; }
        
        public SceneTreeNode(GameObjectWrapper gameObject) : base(gameObject.Name)
        {
            WrappedGameObject = gameObject;
            gameObject.TreeViewNode = this;
            WrappedGameObject.ChangedEvent += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "Name")
                {
                    Text = WrappedGameObject.Name;
                }
            };

            GameObjectWrapper[] children = gameObject.GetWrappedChildren();
            if (children.Length == 0) return;
            foreach (GameObjectWrapper child in children)
            {
                Nodes.Add(new SceneTreeNode(child));
            }
        }
    }
}