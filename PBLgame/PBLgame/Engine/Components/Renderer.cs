﻿using System;
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
        public Renderer(GameObject owner) : base(owner)
        {
            _myMesh = null;
            AlphaValue = 1f;
            EmissiveValue = 0f;
        }

        public Renderer(Renderer source, GameObject owner) : base(owner)
        {
            _myMesh   = source._myMesh;
            _material = source._material;
            _myEffect = source._myEffect;
            AlphaValue = source.AlphaValue;
            EmissiveValue = source.EmissiveValue;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            DrawTechnique(gameTime, Technique.Default);
        }

        public override Component Copy(GameObject newOwner)
        {
            return new Renderer(this, newOwner);
        }

        public void DrawTechnique(GameTime gameTime, Technique technique = Technique.Default)
        {
            if (!Enabled) return;
            MyEffect.CurrentTechnique = MyEffect.Techniques[technique.GetString()];
            
            if (technique == Technique.Default || technique == Technique.CustomCamera || technique == Technique.Glow)
            {
                MyEffect.Parameters["diffuseTexture"].SetValue(_material.Diffuse);
                MyEffect.Parameters["normalIntensity"].SetValue(1);
                MyEffect.Parameters["normalMap"].SetValue(_material.Normal);
                MyEffect.Parameters["specularIntensity"].SetValue(1);
                MyEffect.Parameters["specularTexture"].SetValue(_material.Specular);
                MyEffect.Parameters["emissiveIntensity"].SetValue(EmissiveValue);
                MyEffect.Parameters["emissiveTexture"].SetValue(_material.Emissive);
                MyEffect.Parameters["alphaValue"].SetValue(AlphaValue);
            }

            AnimatedMesh animatedMesh = MyMesh as AnimatedMesh;

            if (animatedMesh != null)
            {
                foreach (ModelMesh modelMesh in MyMesh.Model.Meshes)
                {
                    ParameterizeEffectWithMeshWorld(modelMesh); 
                    MyEffect.Parameters["Bones"].SetValue(gameObject.animator.SkeletonMatrix);

                    foreach (ModelMeshPart part in modelMesh.MeshParts)
                    {
                        if (part.Effect.GetType() != typeof(BasicEffect))
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
            AlphaValue    = ReadIfSpecified(reader, "Alpha",    AlphaValue);
            EmissiveValue = ReadIfSpecified(reader, "Emissive", EmissiveValue);
            reader.Read();
        }

        private float ReadIfSpecified(XmlReader reader, string attribute, float defaultVal)
        {
            string s = reader.GetAttribute(attribute);
            return (s == null) ? defaultVal : Convert.ToSingle(s, CultureInfo.InvariantCulture);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("MeshId", MyMesh.Id.ToString());
            writer.WriteAttributeString("MaterialId", Material.Id.ToString());
            if (AlphaValue != 1f)    writer.WriteAttributeString("Alpha",    AlphaValue   .ToString("G", CultureInfo.InvariantCulture));
            if (EmissiveValue != 0f) writer.WriteAttributeString("Emissive", EmissiveValue.ToString("G", CultureInfo.InvariantCulture));
        }

        #endregion

        public enum Technique
        {
            Default = 0, 
            ShadowsPoint,
            ShadowsDirectional,
            CustomCamera,
            Reflection,
            Glow
        }

        /// <summary>
        /// Generates AABB using current mesh & transform.
        /// </summary>
        /// <param name="worldMatrix">World matrix to apply to mesh before generating.</param>
        /// <returns></returns>
        public BoundingBox GenerateAABB(Matrix worldMatrix)
        {
            List<Vector3> vertices = ExtractVertices(gameObject.renderer.MyMesh.Model.Meshes, true, worldMatrix);
            return BoundingBox.CreateFromPoints(vertices);
        }

        /// <summary>
        /// Generates bounding sphere using current mesh.
        /// </summary>
        /// <returns></returns>
        public BoundingSphere GenerateSphere()
        {
            List<Vector3> vertices = ExtractVertices(gameObject.renderer.MyMesh.Model.Meshes);
            return BoundingSphere.CreateFromPoints(vertices);
        }

        private List<Vector3> ExtractVertices(ModelMeshCollection meshes, bool transform = false, Matrix world = default(Matrix))
        {
            List<Vector3> vertices = new List<Vector3>();
            foreach (ModelMesh mesh in meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int vStride = part.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
                    int bufferSize = part.NumVertices * vStride;
                    float[] vertexData = new float[bufferSize];
                    part.VertexBuffer.GetData(vertexData);
                    for (int i = 0; i < bufferSize; i += vStride)
                    {
                        Vector3 vertex = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        if(transform) vertex = Vector3.Transform(vertex, mesh.ParentBone.Transform * world);
                        vertices.Add(vertex);
                    }
                }
            }
            return vertices;
        }


    }
}
