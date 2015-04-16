using Edytejshyn.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;

namespace Edytejshyn.GUI
{
    public class RealisticDrawerStrategy : IDrawerStrategy
    {
        public void Draw(GameObjectWrapper gameObjectWrapper, GameTime gameTime)
        {
            gameObjectWrapper.Nut.renderer.Draw(gameTime);
        }
    }

    public class BasicDrawerStrategy : IDrawerStrategy
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BasicEffect _basic;

        public BasicDrawerStrategy(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _basic = new BasicEffect(_graphicsDevice);
        }

        public void Draw(GameObjectWrapper gameObjectWrapper, GameTime gameTime)
        {
            _basic.View = Camera.MainCamera.ViewMatrix;
            _basic.Projection = Camera.MainCamera.ProjectionMatrix;
            _basic.Texture = gameObjectWrapper.Nut.renderer.Material.Diffuse;
            _basic.TextureEnabled = true;
            //_basic.EnableDefaultLighting();

            foreach (ModelMesh modelMesh in gameObjectWrapper.Nut.renderer.MyMesh.Model.Meshes)
            {
                foreach (ModelMeshPart part in modelMesh.MeshParts)
                {
                    part.Effect = _basic;
                    _basic.World = gameObjectWrapper.Nut.renderer.MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * gameObjectWrapper.Nut.transform.World;
                }
                modelMesh.Draw();
            }
        }
    }
}