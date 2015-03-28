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

        private Model _model;
        private string _path;
        private int _id;
        private MeshMaterial _meshMaterial;

        private Renderer _myRenderer;

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

        public MeshMaterial Material
        {
            get { return _meshMaterial; }
            set { _meshMaterial = value; }
        }
        public Matrix[] BonesTransorms
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

        public void AssignRenderer(Renderer renderer)
        {
            _myRenderer = renderer;
        }

        #endregion

    }
}
