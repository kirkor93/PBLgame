using System.Xml;
using Microsoft.Xna.Framework;
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
            System.Random rand = new System.Random();
            Vector3[] lightsP = new Vector3[3];
            Vector4[] lightsC = new Vector4[3];
            float[] lightsA = new float[3];
            float[] lightsF = new float[3];

            lightsP[0] = new Vector3(-5, -5, -5);
            lightsC[0] = new Vector4(1, 0, 0, 1);
            lightsA[0] = 10.0f;
            lightsF[0] = 2f;

            lightsP[1] = new Vector3(3, 3, 3);
            lightsC[1] = new Vector4(0, 1, 0, 1);
            lightsA[1] = 10.0f;
            lightsF[1] = 2f;

            lightsP[2] = new Vector3(-2, 2, -2);
            lightsC[2] = new Vector4(0, 0, 1, 1);
            lightsA[2] = 10.0f;
            lightsF[2] = 2f;

            int[] pointLights = new int[]{1,1,1};
            int[] directionalLights = new int[]{0,0,0};

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
                    //!!!!!! lightsCount have to be less or equal 30
                    MyEffect.Parameters["lightsCount"].SetValue(1);
                    MyEffect.Parameters["lightsPositions"].SetValue(lightsP);
                    MyEffect.Parameters["lightsColors"].SetValue(lightsC);
                    MyEffect.Parameters["lightsAttenuations"].SetValue(lightsA);
                    MyEffect.Parameters["lightsFalloffs"].SetValue(lightsF);
                    MyEffect.Parameters["lightsPoint"].SetValue(pointLights);
                    MyEffect.Parameters["lightsDirectional"].SetValue(directionalLights);




                }
                modelMesh.Draw();
            }
        }

        private void ParameterizeEffect()
        {

        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
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
