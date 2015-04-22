using System.Windows.Forms;
using Edytejshyn.Model;

namespace Edytejshyn.GUI
{
    public class SceneTreeView : TreeView
    {

        public MainForm MainForm;

        public SceneTreeView()
        {
        }

        public void ReloadTree()
        {
            Nodes.Clear();
            if (MainForm.Logic.WrappedScene == null) return;
            foreach (GameObjectWrapper wrapper in MainForm.Logic.WrappedScene.RootGameObjects)
            {
                Nodes.Add(new SceneTreeNode(wrapper));
            }

        }

    }
}