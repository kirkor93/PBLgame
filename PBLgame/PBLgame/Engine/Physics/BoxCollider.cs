using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLgame.Engine.Physics
{
    public class BoxCollider
    {
        #region Variables
        private Vector3 _position;
        private BoundingBox _box = new BoundingBox();
        private float _size;

        private VertexPositionColor[] _verts;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public BoundingBox Box
        {
            get
            {
                return _box;
            }
            set
            {
                _box = value;
            }
        }
        public float Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
        #endregion

        #region Methods
        public void InitializeSphere(Vector3 pos)
        {

        }

        public void RealPosition(Vector3 pos)
        {
            _box.
        }

        public void Update()
        {

        }


        public void Draw()
        {

        }
        #endregion
    }
}
