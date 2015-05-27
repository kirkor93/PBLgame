using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Components;


namespace PBLgame.Engine.Physics
{
    public class PhysicsSystem
    {
        public static List<GameObject> CollisionObjects = new List<GameObject>();

        #region Methods
        public void Update(GameTime gameTime)
        {
            if (CollisionObjects.Count == 0) return;
            List<GameObject> rigidbodies = new List<GameObject>();
            foreach (GameObject go in CollisionObjects)
            {
                if (go.collision.Rigidbody)
                {
                    rigidbodies.Add(go);
                }
            }
            if (rigidbodies.Count == 0) return;
            foreach (GameObject rb in rigidbodies)
            {
                if (!rb.collision.OnTerrain) rb.transform.Translate(0.0f, -0.01f * rb.collision.Mass, 0.0f);
            }
        }

        public static void AddCollisionObject(GameObject obj)
        {
            CollisionObjects.Add(obj);
        }

        public static void DeleteCollisionObject(GameObject obj)
        {
            CollisionObjects.Remove(obj);
        }
        #endregion
    }
}
