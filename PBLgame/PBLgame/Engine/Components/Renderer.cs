using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;
using PBLgame.Engine.Singleton;
using System.Collections.Generic;
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
        #endregion

        #region Methods
        public Renderer(GameObject owner, Scene scene) : base(owner)
        {
            _scene = scene;
            _myMesh = null;
        }

        public Renderer(Renderer source, GameObject owner) : base(owner)
        {
            _myMesh   = source._myMesh;
            _material = source._material;
            _myEffect = source._myEffect;
            _scene    = source._scene;
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
            ParameterizeEffectWithLights();

            MyEffect.Parameters["view"].SetValue(Camera.MainCamera.ViewMatrix);
            MyEffect.Parameters["projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
            MyEffect.Parameters["direction"].SetValue(Camera.MainCamera.Direction);
            MyEffect.Parameters["diffuseTexture"].SetValue(_material.Diffuse);
            MyEffect.Parameters["normalIntensity"].SetValue(1);
            MyEffect.Parameters["normalMap"].SetValue(_material.Normal);
            MyEffect.Parameters["specularIntensity"].SetValue(1);
            MyEffect.Parameters["specularTexture"].SetValue(_material.Specular);
            MyEffect.Parameters["emissiveIntensity"].SetValue(0);
            MyEffect.Parameters["emissiveTexture"].SetValue(_material.Emissive);

            AnimatedMesh animatedMesh = MyMesh as AnimatedMesh;

            if (animatedMesh != null)
            {
                foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
                {
                    MyEffect.Parameters["world"].SetValue(MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * _gameObject.transform.World);
                    MyEffect.Parameters["worldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * _gameObject.transform.World)));
                    MyEffect.Parameters["Bones"].SetValue(animatedMesh.Skeleton);

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
                    //UpdateLightsPositions(); // TODO why is it here?
                    MyEffect.Parameters["world"].SetValue(MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * _gameObject.transform.World);
                    MyEffect.Parameters["worldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(MyMesh.BonesTransorms[modelMesh.ParentBone.Index] * _gameObject.transform.World)));

                    foreach (ModelMeshPart part in modelMesh.MeshParts)
                    {
                        part.Effect = MyEffect;
                    }
                    modelMesh.Draw();
                }
            }
        }
        
        private void UpdateLightsPositions()
        {
            
            List<Light> lights = _scene.SceneLights;
            Vector3[] pos_dir = new Vector3[30];

            for (int i = 0; i < lights.Count; ++i)
            {
                if (lights[i].Type == LightType.Directional)
                {
                    MyDirectionalLight dLight = lights[i] as MyDirectionalLight;
                    pos_dir[i] = dLight.Direction;
                }
                else
                {
                    PointLight pLight = lights[i] as PointLight;
                    pos_dir[i] = pLight.Position;
                }
            }
            MyEffect.Parameters["lightsPositions"].SetValue(pos_dir);

        }

        private void ParameterizeEffectWithLights()
        {
            List<Light> lights = _scene.SceneLights;
            Vector3[] pos_dir = new Vector3[30];
            Vector4[] colors = new Vector4[30];
            float[] att_int = new float[30];
            float[] falloff = new float[30];
            int[] points = new int[30];
            int[] dirs = new int[30];

            for (int i = 0; i < lights.Count; ++i )
            {
                if(lights[i].Type == LightType.Directional)
                {
                    MyDirectionalLight dLight = lights[i] as MyDirectionalLight;
                    pos_dir[i] = dLight.Direction;
                    colors[i] = dLight.Color;
                    att_int[i] = dLight.Intensity;
                    dirs[i] = 1;
                    points[i] = 0;
                }
                else
                {
                    PointLight pLight = lights[i] as PointLight;
                    pos_dir[i] = pLight.Position;
                    colors[i] = pLight.Color;
                    att_int[i] = pLight.Attenuation;
                    falloff[i] = pLight.FallOff;
                    points[i] = 1;
                    dirs[i] = 0;
                }
            }
                //!!!!!! lightsCount have to be less or equal 30
            MyEffect.Parameters["lightsCount"].SetValue(lights.Count);
            MyEffect.Parameters["lightsPositions"].SetValue(pos_dir);
            MyEffect.Parameters["lightsColors"].SetValue(colors);
            MyEffect.Parameters["lightsAttenuations"].SetValue(att_int);
            MyEffect.Parameters["lightsFalloffs"].SetValue(falloff);
            MyEffect.Parameters["lightsPoint"].SetValue(points);
            MyEffect.Parameters["lightsDirectional"].SetValue(dirs);
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
