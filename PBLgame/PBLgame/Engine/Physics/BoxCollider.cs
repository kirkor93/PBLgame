using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using PBLgame.Engine.GameObjects;
using Microsoft.Xna.Framework.Graphics;

using PBLgame.Engine.Components;

namespace PBLgame.Engine.Physics
{
    public class BoxCollider
    {
        #region Variables
        private Collision _owner;
        private Vector3 _localPosition;
        private Vector3 _totalPosition;
        private Vector3 _edgesSize;
        private bool _trigger;
        private BoundingBox _box = new BoundingBox();

        //private Vector3 _min;
        //private Vector3 _max;

        private Vector3[] _baseColVerts;

        private Vector3[] _colVerts;

        private Vector3 _previousPosition;
        #endregion

        #region Properties
        public Collision Owner
        {
            get
            {
                return _owner;
            }
            private set { }
        }

        public Vector3 LocalPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
                _totalPosition = _owner.gameObject.transform.Position + _localPosition;
                if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
            }
        }

        public Vector3 TotalPosition
        {
            get
            {
                return _totalPosition;
            }
            private set { }
        }
        public Vector3 PreviousPosition
        {
            get
            {
                return _previousPosition;
            }
            private set { }
        }
        public bool Trigger
        {
            get
            {
                return _trigger;
            }
            set
            {
                _trigger = value;
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
        public Vector3 EdgesSize
        {
            get
            {
                return _edgesSize;
            }
            set
            {
                _edgesSize = value;
            }
        }
        #endregion

        #region Methods
        public BoxCollider(Collision owner)
        {
            _owner = owner;
            _previousPosition = Vector3.Zero;
            _edgesSize = new Vector3(1, 1, 1);
            _localPosition = Vector3.Zero;
            _totalPosition = owner.gameObject.transform.Position + _localPosition;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = false;
            _colVerts = new Vector3[8];
            InitializeVerts();
            _baseColVerts = new Vector3[8];
            _baseColVerts = _colVerts;
            //SetMinMax();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }

        public BoxCollider(Collision owner, Vector3 position, Vector3 size, bool trigger)
        {
            _owner = owner;
            _previousPosition = Vector3.Zero;
            _edgesSize = size;
            _localPosition = position;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = trigger;
            _colVerts = new Vector3[8];
            InitializeVerts();
            _baseColVerts = new Vector3[8];
            _baseColVerts = _colVerts;
            //SetMinMax();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }

        public BoxCollider(Collision owner, Vector3 size, bool trigger)
        {
            _owner = owner;
            _previousPosition = Vector3.Zero;
            _edgesSize = size;
            _localPosition = Vector3.Zero;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = trigger;
            _colVerts = new Vector3[8];
            InitializeVerts();
            _baseColVerts = new Vector3[8];
            _baseColVerts = _colVerts;
            //SetMinMax();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }

        private void InitializeVerts()
        {
            _colVerts[0] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            _colVerts[1] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            _colVerts[2] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            _colVerts[3] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            _colVerts[4] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));
            _colVerts[5] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));
            _colVerts[6] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));
            _colVerts[7] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));

            //_colVerts[0] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[1] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[2] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[3] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[4] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[5] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[6] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
            //_colVerts[7] = new Vector3((_edgesSize.X / 2), (_edgesSize.Y / 2), (_edgesSize.Z / 2));
        }


        public bool Intersect(SphereCollider sphere)
        {
            return _box.Intersects(sphere.Sphere);
        }

        public ContainmentType Contains(SphereCollider sphere)
        {
            return _box.Contains(sphere.Sphere);
        }

        public ContainmentType Contains(BoxCollider box)
        {
            return _box.Contains(box.Box);
        }

        public void UpdatePosition()
        {
            _previousPosition = _totalPosition;
            _totalPosition = _owner.gameObject.transform.Position + _localPosition;
            if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
            InitializeVerts();
            //_baseColVerts = _colVerts;
            //for (int i = 0; i < 8; i++)
            //{
            //    _colVerts[i] = Vector3.Transform(_baseColVerts[i], _owner.gameObject.transform.WorldRotation);
            //}
            //_box = BoundingBox.CreateFromPoints(_colVerts);
        }

        public void Update()
        {

        }


        public void Draw()
        {
            GraphicsDevice graphicsDevice = GlobalInventory.Instance.GraphicsDevice;
            short[] bBoxIndices = 
            {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };

            BasicEffect boxEffect = new BasicEffect(graphicsDevice);
            Vector3[] corners = _colVerts;
            VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

            // Assign the 8 box vertices
            for (int i = 0; i < corners.Length; i++)
            {
                primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
            }

            /* Set your own effect parameters here */

            boxEffect.World = Matrix.Identity;
            boxEffect.View = Camera.MainCamera.ViewMatrix;
            boxEffect.Projection = Camera.MainCamera.ProjectionMatrix;
            boxEffect.TextureEnabled = false;

            // Draw the box with a LineList
            foreach (EffectPass pass in boxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList, primitiveList, 0, 8,
                    bBoxIndices, 0, 12);
            }
        }
        #endregion
    }
}
