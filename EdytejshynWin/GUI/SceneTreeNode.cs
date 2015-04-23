using System.ComponentModel;
using System.Windows.Forms;
using Edytejshyn.Model;

namespace Edytejshyn.GUI
{
    public class SceneTreeNode : TreeNode
    {
        public GameObjectWrapper WrappedGameObject { get; private set; }
        
        /// <summary>
        /// Create scene treeView node. This is done recursively through all descendants.
        /// </summary>
        /// <param name="gameObject">GameObjectWrapper with which create node</param>
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

            foreach (GameObjectWrapper child in gameObject.Children)
            {
                Nodes.Add(new SceneTreeNode(child));
            }
        }
    }
}