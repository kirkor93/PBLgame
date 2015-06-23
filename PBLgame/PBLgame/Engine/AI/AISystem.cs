using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PBLgame.Engine.AI;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Components;

namespace PBLgame.Engine.AI
{
    public class AISystem
    {
        private static List<GameObject> _aiObjects = new List<GameObject>();

        public static GameObject Player;

        public static void AddAIObject(GameObject obj)
        {
            _aiObjects.Add(obj);
        }

        public static void DeleteAIObject(GameObject obj)
        {
            _aiObjects.Remove(obj);
        }

        public static void SetPlayer(GameObject play)
        {
            Player = play;
        }

        public static void ExecuteAI()
        {
            if(_aiObjects.Count > 0)
            {
                foreach (GameObject go in _aiObjects)
                {
                    if(go.Enabled)
                    {
                        AIComponent comp = go.GetComponent<AIComponent>();
                        if (comp != null && comp.Enabled) comp.MyDTree.ExecuteTree();
                    }
                }
            }

        }
    }
}
