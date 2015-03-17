using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.GameObject
{
    public class Mesh
    {
        #region Variables
        #region Private

        private Model _model;
        private string _name;
        private int _id;

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

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion

        #region Methods
        #region Private



        #endregion
        #region Protected



        #endregion
        #region Public

        public Mesh()
        {
            Model = ResourceManager.Instance.GetModel();
        }

        public Mesh(string name)
        {
            this.Name = name;
            Model = ResourceManager.Instance.GetModel(name);
        }

        public Mesh(int id)
        {
            this.Id = id;
            Model = ResourceManager.Instance.GetModel(id);
        }

        #endregion
        #endregion
    }
}
