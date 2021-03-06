﻿using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.Components
{
    public class Mesh
    {
        #region Variables
        #region Private

        protected Model _model;
        protected string _path;
        protected int _id;

        protected Matrix[] _boneTransforms;

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

        public virtual Matrix[] BonesTransorms
        {
            get
            {
                return _boneTransforms;
            }
            set
            {
                _boneTransforms = value;
            }
        }

        #endregion

        #region Methods

        public Mesh()
        {
            Id = -1;
            Path = "";
            Model = null;
        }

        public Mesh(int id, string path, Model model)
        {
            Id = id;
            Path = path;
            AttatchModel(model);
        }

        

        public void AttatchModel(Model model)
        {
            Model = model;
            _boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(_boneTransforms);
        }

        public void Draw()
        {

        }

        public override string ToString()
        {
            const string prefix = @"Models\";
            return _path.StartsWith(prefix) ? _path.Substring(prefix.Length) : _path;
        }

        #endregion
    }
}
