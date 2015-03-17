using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObject;

namespace PBLgame.Engine.Singleton
{

    public class ResourceManager : Singleton<ResourceManager>
    {
        #region Variables
        #region Private

        private const string MESHES_LIST_PATH = @"meshes.xml";

        private IList<Mesh> _meshes;
        private IList<Texture2D> _textures;

        #endregion
        #region Protected



        #endregion
        #region Public



        #endregion
        #endregion

        #region Properties

        #endregion

        #region Methods
        #region Private



        #endregion
        #region Protected



        #endregion
        #region Public
        #region Constructors

        public ResourceManager()
        {
            _meshes = new List<Mesh>();
            _textures = new List<Texture2D>();
        }

        #endregion

        public void LoadMeshes()
        {

            Model model = Game.GameInstance.Content.Load<Model>(@"Models\Helmet");
            _meshes.Add(new Mesh(0, "", model));

        }

        public void LoadTextures(string path)
        {
            throw new NotImplementedException();
        }

        public Mesh GetModel()
        {
            throw new NotImplementedException();
        }

        public Mesh GetModel(string name)
        {
            throw new NotImplementedException();
        }

        public Mesh GetModel(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
        #endregion


    }
}