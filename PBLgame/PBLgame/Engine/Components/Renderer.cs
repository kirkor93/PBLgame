using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.Components
{
    public class Renderer : Component
    {
        #region Variables
        #region Private
        private Mesh _myMesh;
        private MeshMaterial _material;
        private Effect _myEffect;
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
        public Effect MyEffect
        {
            get
            {
                return _myEffect;

            }
            set
            {
                _myEffect = value;
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
                foreach (ModelMeshPart part in modelMesh.MeshParts)
                {

                }
            }
        }


        public override void Update()
        {
        }

        public override void Draw()
        {
            foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
            {
                foreach (ModelMeshPart part in modelMesh.MeshParts)
                {
                    part.Effect = MyEffect;
                    MyEffect.Parameters["world"].SetValue(MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * _gameObject.transform.World);
                    MyEffect.Parameters["view"].SetValue(Camera.MainCamera.ViewMatrix);
                    MyEffect.Parameters["projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
                    MyEffect.Parameters["worldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * _gameObject.transform.World)));
                    MyEffect.Parameters["direction"].SetValue(Camera.MainCamera.Direction);
                    MyEffect.Parameters["diffuseTexture"].SetValue(_material.Diffuse);
                    MyEffect.Parameters["normalIntensity"].SetValue(1);
                    MyEffect.Parameters["normalMap"].SetValue(_material.Normal);
                    MyEffect.Parameters["specularIntensity"].SetValue(1);
                    MyEffect.Parameters["specularTexture"].SetValue(_material.Specular);
                    MyEffect.Parameters["emissiveIntensity"].SetValue(0);
                    MyEffect.Parameters["emissiveTexture"].SetValue(_material.Emissive);
                }
                modelMesh.Draw();
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            base.ReadXml(reader);
            int meshId = Convert.ToInt32(reader.GetAttribute("MeshId"));
            int materialId = Convert.ToInt32(reader.GetAttribute("MaterialId"));
            MyMesh = ResourceManager.Instance.GetMesh(meshId);
            Material = ResourceManager.Instance.GetMaterial(materialId);
            MyEffect = Material.ShaderEffect;
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("MeshId", MyMesh.Id.ToString());
            writer.WriteAttributeString("MaterialId", Material.Id.ToString());
        }

        #endregion
    }
}
