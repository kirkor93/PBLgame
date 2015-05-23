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
        public void Update(float gameTime)
        {

        }

        public void Update(float gameTime, List<GameObject> gameObjects)
        {

        }

        public static void AddCollisionObject(GameObject obj)
        {
            CollisionObjects.Add(obj);
        }

        public static void DeleteCollisionObject(GameObject obj)
        {
            CollisionObjects.Remove(obj);
        }

        //public void Update(List<GameObject> gameObjects)
        //{
        //    if (gameObjects.Count == 0) return;
        //    List<GameObject> rigidbodies = new List<GameObject>();
        //    List<GameObject> toDelete = new List<GameObject>();
        //    foreach(GameObject go in gameObjects)
        //    {
        //        if (go.collision.Rigidbody)
        //        {
        //            rigidbodies.Add(go);
        //            toDelete.Add(go);
        //        }
        //    }
        //    //foreach(GameObject go in toDelete)
        //    //{
        //    //    gameObjects.Remove(go);
        //    //}
        //    CollisionObjects = gameObjects;
        //    if (rigidbodies.Count == 0) return;
        //    foreach(GameObject rb in rigidbodies)
        //    {
        //        if(!rb.collision.OnTerrain)rb.transform.Translate(0.0f, -0.5f, 0.0f);
        //    }
        //}
        #endregion
    }
}
