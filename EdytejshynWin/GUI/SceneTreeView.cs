using System.Windows.Forms;
using Edytejshyn.Logic;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;

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
            if (MainForm.Logic.CurrentScene == null) return;
            foreach (GameObject obj in MainForm.Logic.CurrentScene.GameObjects)
            {
                if (obj.parent != null) continue;
                Nodes.Add(new SceneTreeNode(new GameObjectWrapper(obj, MainForm), MainForm));
            }

        }

    }
}