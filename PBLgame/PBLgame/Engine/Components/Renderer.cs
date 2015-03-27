using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Renderer : Component
    {
        #region Variables
        #region Private
        private Mesh _myMesh;
        private MeshMaterial _material;

        #endregion
        #endregion  

        #region Properites
        public Mesh MyMesh
        {
            get 
            {
                return _myMesh;
            }
            set
            {
                _myMesh = value;
            }
        }

        public MeshMaterial Material
        {
            get
            {
                return _material;
                
            }
            set
            {
                _material = value;
            }
        }
        #endregion

        #region Methods
        public Renderer(GameObject owner) : base(owner)
        {
            _myMesh = null;
        }

        public void AssignMaterial(MeshMaterial material)
        {
            Material = material;

            foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
            {
                foreach (BasicEffect basicEffect in modelMesh.Effects)
                {
                    basicEffect.TextureEnabled = true;
                    basicEffect.Texture = Material.Diffuse;
                }
            }
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Draw()
        {
            _myMesh.Draw();
        }
        #endregion
    }
}
