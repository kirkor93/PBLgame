using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Scenes
{
    public class CullingManager
    {
        private CullingNode _staticRoot;
        private List<GameObjectBS> _dynamicObjects;
        private List<GameObject> _invisibles;

        public CullingManager(IList<GameObject> gameObjects, int depth)
        {
            List<GameObjectBS> dynamicObjects = new List<GameObjectBS>();
            List<GameObjectAABB> staticObjects = new List<GameObjectAABB>();
            _invisibles = new List<GameObject>();
            // generate AABB for each static mesh
            ClassifyObjectsRecursive(gameObjects.Where(o => o.parent == null), staticObjects, dynamicObjects, _invisibles, false);

            // whole scene into one box
            BoundingBox sceneBox = new BoundingBox();
            sceneBox = staticObjects.Aggregate(sceneBox, (current, o) => BoundingBox.CreateMerged(current, o.Box));
            float size = sceneBox.GetSize().GetMax();
            sceneBox.Max.X = sceneBox.Min.X + size;
            sceneBox.Max.Z = sceneBox.Min.Z + size;
            _staticRoot = new CullingNode(null, sceneBox, staticObjects, depth);
            _dynamicObjects = dynamicObjects;
        }

        private void ClassifyObjectsRecursive(IEnumerable<GameObject> children, List<GameObjectAABB> staticObjects, List<GameObjectBS> dynamicObjects, List<GameObject> invisibleObjects, bool parentIsDynamic)
        {
            foreach (GameObject gameObject in children)
            {
                if (parentIsDynamic)
                {
                    if (gameObject.renderer != null)
                    {
                        BoundingSphere bs = gameObject.renderer.GenerateSphere();
                        dynamicObjects.Add(new GameObjectBS(gameObject, bs));
                    }
                    else
                    {
                        invisibleObjects.Add(gameObject);
                    }
                    ClassifyObjectsRecursive(gameObject.GetChildren(), staticObjects, dynamicObjects, invisibleObjects, true);
                }
                else if (gameObject.renderer != null)
                {
                    Collision collision = gameObject.collision;
                    bool isStatic = (collision == null) || collision.Static;
                    if (isStatic)
                    {
                        BoundingBox aabb = gameObject.renderer.GenerateAABB(gameObject.transform.World);
                        staticObjects.Add(new GameObjectAABB(gameObject, aabb));
                        ClassifyObjectsRecursive(gameObject.GetChildren(), staticObjects, dynamicObjects,
                            invisibleObjects, false);
                    }
                    else
                    {
                        BoundingSphere bs = gameObject.renderer.GenerateSphere();
                        dynamicObjects.Add(new GameObjectBS(gameObject, bs));
                        ClassifyObjectsRecursive(gameObject.GetChildren(), staticObjects, dynamicObjects,
                            invisibleObjects, true);
                    }
                }
                else
                {
                    invisibleObjects.Add(gameObject);
                    ClassifyObjectsRecursive(gameObject.GetChildren(), staticObjects, dynamicObjects, invisibleObjects, false);
                }
            }
        }

        public List<GameObject> GetVisibleGameObjects(Matrix cameraMatrix)
        {
            BoundingFrustum frustum = new BoundingFrustum(cameraMatrix);
            List<GameObject> visibles = _staticRoot.GetVisibleGameObjects(frustum);
            foreach (GameObjectBS obs in _dynamicObjects)
            {
                ContainmentType containment = frustum.Contains(obs.Sphere.Transform(obs.GameObject.transform.World));
                if (containment != ContainmentType.Disjoint)
                {
                    visibles.Add(obs.GameObject);
                }
            }
            return visibles;
        }

        public void Add(GameObject gameObject)
        {
            Renderer renderer = gameObject.renderer;
            if (renderer != null)
            {
                _dynamicObjects.Add(new GameObjectBS(gameObject, renderer.GenerateSphere()));
            }
            else
            {
                _invisibles.Add(gameObject);
            }
        }

        public void Remove(int id)
        {
            _dynamicObjects.RemoveAll(o => o.GameObject.ID == id);
            _invisibles.RemoveAll(o => o.ID == id);
        }

        public IEnumerable<GameObject> GameObjectsForUpdate(Matrix cameraMatrix)
        {
            List<GameObject> updatable = GetVisibleGameObjects(cameraMatrix);
            updatable.AddRange(_invisibles);
            return updatable;
        }
    }


    public struct GameObjectAABB
    {
        public GameObject GameObject;
        public BoundingBox Box;

        public GameObjectAABB(GameObject gameObject, BoundingBox box)
        {
            GameObject = gameObject;
            Box = box;
        }
    }
    
    public struct GameObjectBS
    {
        public GameObject GameObject;
        public BoundingSphere Sphere;

        public GameObjectBS(GameObject gameObject, BoundingSphere sphere)
        {
            GameObject = gameObject;
            Sphere = sphere;
        }
    }


    public class CullingNode
    {
        private readonly CullingNode _parent;
        private readonly CullingNode[] _children;
        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly BoundingBox _box;

        public List<GameObject> GameObjects { get { return _gameObjects; } }
        public BoundingBox Box { get { return _box; } }
        public CullingNode[] Children { get { return _children; } }

        public CullingNode(CullingNode parent, BoundingBox box, List<GameObjectAABB> inputList, int depth)
        {
            _parent = parent;
            _box = box;
            //Console.WriteLine("==== depth: {0}, box: {1}, objs: {2} ====", depth, box.GetSize(), inputList.Count);
            if (inputList.Count <= 1 || depth <= 0 || _box.GetSize().GetMin() <= 1f)
            {
                _children = null;
                _gameObjects.AddRange(inputList.Select(o => o.GameObject));
                //Console.WriteLine("Few gameobjects: {0}", inputList.Count);
            }
            else
            {
                _children = new CullingNode[4];
                BoundingBox[] childBoxes = new BoundingBox[4];
                Vector3 half = (box.Min + box.Max) / 2;
                Vector3 halfUp = new Vector3(half.X, box.Max.Y, half.Z);
                Vector3 trans = (box.Max - box.Min) / 2;
                childBoxes[0] = new BoundingBox(box.Min, halfUp);
                childBoxes[1] = childBoxes[0].Translate(new Vector3(trans.X, 0, 0));
                childBoxes[2] = childBoxes[0].Translate(new Vector3(0, 0, trans.Z));
                childBoxes[3] = childBoxes[0].Translate(new Vector3(trans.X, 0, trans.Z));

                List<GameObjectAABB>[] childList = new List<GameObjectAABB>[4];
                for (int i = 0; i < childList.Length; i++)
                {
                    childList[i] = new List<GameObjectAABB>();
                }

                foreach (GameObjectAABB obj in inputList)
                {
                    for (int i = 0; i < childBoxes.Length; i++)
                    {
                        ContainmentType contains = childBoxes[i].Contains(obj.Box);
                        if (contains == ContainmentType.Contains)
                        {
                            childList[i].Add(obj);
                            //DebugOut(obj, "fully in " + i);
                            break;
                        }
                        else if (contains == ContainmentType.Intersects)
                        {
                            childList[i].Add(obj);
                            //DebugOut(obj, "intersects " + i);
                        }
                    }
                    _gameObjects.Add(obj.GameObject);
                }
                for (int i = 0; i < _children.Length; i++)
                {
                    _children[i] = new CullingNode(this, childBoxes[i], childList[i], depth - 1);
                }
            }
        }

        private void DebugOut(GameObjectAABB obj, string type)
        {
            Console.WriteLine("[{0}] {1} {2}", obj.GameObject.ID, obj.GameObject.Name, type);
        }

        public List<GameObject> GetVisibleGameObjects(BoundingFrustum frustum)
        {
            List<GameObject> visibles = new List<GameObject>();
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Processed = false;
            }

            AddVisibleRecursive(frustum, visibles);
            return visibles;
        }

        private void AddVisibleRecursive(BoundingFrustum frustum, List<GameObject> visibles)
        {
            ContainmentType containment = frustum.Contains(_box);
            switch (containment)
            {
                case ContainmentType.Contains:
                    foreach (GameObject gameObject in _gameObjects)
                    {
                        if (gameObject.Processed) continue;
                        gameObject.Processed = true;
                        visibles.Add(gameObject);
                    }
                    break;

                case ContainmentType.Intersects:
                    if (_children == null)
                    {
                        foreach (GameObject gameObject in _gameObjects)
                        {
                            if (gameObject.Processed) continue;
                            gameObject.Processed = true;
                            visibles.Add(gameObject);
                        }
                    }
                    else
                    {
                        foreach (CullingNode child in _children)
                        {
                            child.AddVisibleRecursive(frustum, visibles);
                        }
                    }
                    break;
            }
        }
    }
}