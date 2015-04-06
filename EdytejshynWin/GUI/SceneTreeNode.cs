using System.ComponentModel;
using System.Windows.Forms;
using Edytejshyn.Logic;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI
{
    public class SceneTreeNode : TreeNode
    {
        public GameObjectWrapper GameObject { get; private set; }
        
        public SceneTreeNode(GameObjectWrapper gameObject, MainForm mainForm) : base(gameObject.Name)
        {
            GameObject = gameObject;
            GameObject.ChangedEvent += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "Name")
                {
                    Text = GameObject.Name;
                }
            };

            GameObject[] children = gameObject.GetChildren();
            if (children.Length == 0) return;
            foreach (GameObject child in children)
            {
                Nodes.Add(new SceneTreeNode(new GameObjectWrapper(child, mainForm), mainForm));
            }
        }

    }
}