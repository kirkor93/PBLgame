using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Mesh : IXmlSerializable
    {
        #region Variables
        #region Private

        private Model _model;
        private string _path;
        private int _id;

        private Matrix[] _boneTransforms;

        #endregion
        #region Protected



        #endregion
        #region Public



        #endregion
        #endregion

        #region Properties

        public Model Model
        {
            get { return _model; }
            private set { _model = value; }
        }

        public string Path
        {
            get { return _path; }
            private set { _path = value; }
        }

        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        #endregion

        #region Methods

        public Mesh()
        {
            this.Id = -1;
            this.Path = "";
            this.Model = null;
        }

        public Mesh(int id, string path)
        {
            this.Id = id;
            this.Path = path;
            this.Model = null;
        }

        public Mesh(int id, string path, Model model)
        {
            this.Id = id;
            this.Path = path;
            AttatchModel(model);
        }

        public void AttatchModel(Model model)
        {
            this.Model = model;
            _boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(_boneTransforms);
        }

        public void Draw()
        {
            foreach (ModelMesh modelMesh in this.Model.Meshes)
            {
                foreach (BasicEffect effect in modelMesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = _boneTransforms[modelMesh.ParentBone.Index];
                    effect.View = Camera.MainCamera.ViewMatrix;
                    effect.Projection = Camera.MainCamera.ProjectionMatrix;
                }

                modelMesh.Draw();
            }
        }

        #region Serialization

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.Id = Convert.ToInt32(reader.GetAttribute("ID"));
            this.Path = reader.GetAttribute("Path");
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

    }
}
