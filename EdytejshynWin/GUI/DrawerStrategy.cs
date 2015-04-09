using Edytejshyn.Model;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI
{
    public class RealisticDrawerStrategy : IDrawerStrategy
    {
        public void Draw(GameObjectWrapper gameObjectWrapper)
        {
            gameObjectWrapper.Content.renderer.Draw();
        }
    }

    public class BasicDrawerStrategy : IDrawerStrategy
    {
        private readonly GraphicsDevice _graphicsDevice;

        public BasicDrawerStrategy(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Draw(GameObjectWrapper gameObjectWrapper)
        {
            BasicEffect basic = new BasicEffect(_graphicsDevice);
            basic.EnableDefaultLighting();
            basic.View = Camera.MainCamera.ViewMatrix;
            basic.Projection = Camera.MainCamera.ProjectionMatrix;
            foreach (ModelMesh modelMesh in gameObjectWrapper.Content.renderer.MyMesh.Model.Meshes)
            {
                foreach (ModelMeshPart part in modelMesh.MeshParts)
                {
                    part.Effect = basic;
                    basic.World = gameObjectWrapper.Content.renderer.MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * gameObjectWrapper.Content.transform.World;
                }
                modelMesh.Draw();
            }

            //foreach (ModelMesh modelMesh in gameObjectWrapper.Content.renderer.MyMesh.Model.Meshes)
            //{
            //    foreach (Effect effect in modelMesh.Effects)
            //    {
            //        BasicEffect basic = (BasicEffect) effect;
            //        basic.EnableDefaultLighting();
            //        basic.World = gameObjectWrapper.Content.renderer.MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * gameObjectWrapper.Content.transform.World;
            //        basic.View = Camera.MainCamera.ViewMatrix;
            //        basic.Projection = Camera.MainCamera.ProjectionMatrix;
            //    }
            //    gameObjectWrapper.Content.renderer.MyMesh.Draw();
            //}

        }
    }
}