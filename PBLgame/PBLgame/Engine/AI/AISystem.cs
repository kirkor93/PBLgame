using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using PBLgame.Engine.AI;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.AI
{
    public class AISystem
    {
        private static List<GameObject> _aiObjects;

        public static void AddAIObject(GameObject obj)
        {
            _aiObjects.Add(obj);
        }

        public static void DeleteAIObject(GameObject obj)
        {
            _aiObjects.Remove(obj);
        }

        public void ExecuteAI()
        {
            foreach(GameObject go in _aiObjects)
            {
                AIComponent comp = go.GetComponent<AIComponent>();
                if (comp != null) comp.MyDTree.ExecuteTree();
            }
        }
    }
}
