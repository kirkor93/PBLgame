using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;
using PBLgame.Engine.Singleton;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AnimationAux;

namespace PBLgame.Engine.Components
{
    public class Renderer : Component
    {
        #region Variables
        #region Private
        private Mesh _myMesh;
        private MeshMaterial _material;
        private Effect _myEffect;
        private Scene _scene;
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

        public float AlphaValue { get; set; }
        public float EmissiveValue { get; set; }
        #endregion

        #region Methods
        public Renderer(GameObject owner, Scene scene) : base(owner)
        {
            _scene = scene;
            _myMesh = null;
            AlphaValue = 1f;
            EmissiveValue = 0f;
        }

        public Renderer(Renderer source, GameObject owner) : base(owner)
        {
            _myMesh   = source._myMesh;
            _material = source._material;
            _myEffect = source._myEffect;
            _scene    = source._scene;
            AlphaValue = source.AlphaValue;
            EmissiveValue = source.EmissiveValue;
        }

        //public void AssignMaterial(MeshMaterial material)
        //{
        //    Material = material;

        //    foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
        //    {
        //        foreach (ModelMeshPart part in modelMesh.MeshParts)
        //        {

        //        }
        //    }
        //}


        public override void Update(GameTime gameTime)
        {

        }

        

        public override void Draw(GameTime gameTime)
        {

            MyEffect.Parameters["view"].SetValue(Camera.MainCamera.ViewMatrix);
            MyEffect.Parameters["projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
            MyEffect.Parameters["cameraPosition"].SetValue(Camera.MainCamera.transform.Position);
            MyEffect.Parameters["diffuseTexture"].SetValue(_material.Diffuse);
            MyEffect.Parameters["normalIntensity"].SetValue(1);
            MyEffect.Parameters["normalMap"].SetValue(_material.Normal);
            MyEffect.Parameters["specularIntensity"].SetValue(1);
            MyEffect.Parameters["specularTexture"].SetValue(_material.Specular);
            MyEffect.Parameters["emissiveIntensity"].SetValue(EmissiveValue);
            MyEffect.Parameters["emissiveTexture"].SetValue(_material.Emissive);
            MyEffect.Parameters["alphaValue"].SetValue(AlphaValue);

            AnimatedMesh animatedMesh = MyMesh as AnimatedMesh;

            if (animatedMesh != null)
            {
                foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
                {
                    ParameterizeEffectWithMeshWorld(modelMesh); 
                    MyEffect.Parameters["Bones"].SetValue(animatedMesh.SkeletonMatrix);

                    foreach (ModelMeshPart part in modelMesh.MeshParts)
                    {
                        part.Effect = MyEffect;
                    }
                    modelMesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
                {
                    ParameterizeEffectWithMeshWorld(modelMesh);
                    
                    foreach (ModelMeshPart part in modelMesh.MeshParts)
                    {
                        part.Effect = MyEffect;
                    }
                    modelMesh.Draw();
                }
            }
        }


        private void ParameterizeEffectWithMeshWorld(ModelMesh modelMesh)
        {
            Matrix world = modelMesh.ParentBone.Transform * _gameObject.transform.World;
            MyEffect.Parameters["world"].SetValue(world);
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            int meshId = Convert.ToInt32(reader.GetAttribute("MeshId"));
            int materialId = Convert.ToInt32(reader.GetAttribute("MaterialId"));
            MyMesh = ResourceManager.Instance.GetMesh(meshId);
            Material = ResourceManager.Instance.GetMaterial(materialId);
            MyEffect = null;
            if (MyMesh is AnimatedMesh)
            {
                // so much lame way
                MyEffect = ResourceManager.Instance.ShaderEffects.FirstOrDefault(x => x.Name == Material.ShaderEffect.Name + "Skinned");
            }
            if (MyEffect == null) MyEffect = Material.ShaderEffect;
            string alphaStr = reader.GetAttribute("Alpha");
            if (alphaStr != null)
            {
                AlphaValue = Convert.ToSingle(alphaStr, CultureInfo.InvariantCulture);
            }
            string emissiveStr = reader.GetAttribute("Emissive");
            if (emissiveStr != null)
            {
                EmissiveValue = Convert.ToSingle(emissiveStr, CultureInfo.InvariantCulture);
            }
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("MeshId", MyMesh.Id.ToString());
            writer.WriteAttributeString("MaterialId", Material.Id.ToString());
            if (AlphaValue != 1f) writer.WriteAttributeString("Alpha", AlphaValue.ToString("G", CultureInfo.InvariantCulture));
            if (EmissiveValue != 0f) writer.WriteAttributeString("Emissive", AlphaValue.ToString("G", CultureInfo.InvariantCulture));
        }



        #endregion
    }
}
