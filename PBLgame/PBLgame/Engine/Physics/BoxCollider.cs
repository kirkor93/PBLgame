using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

using PBLgame.Engine.GameObjects;
using Microsoft.Xna.Framework.Graphics;

using PBLgame.Engine.Components;

namespace PBLgame.Engine.Physics
{
    public class BoxCollider : IXmlSerializable
    {
        #region Variables
        private Collision _owner;
        private Vector3 _localPosition;
        private Vector3 _totalPosition;
        private Vector3 _edgesSize;
        private bool _trigger;
        private BoundingBox _box = new BoundingBox();

        private Vector3 _edgesRealSize;
        private Vector3[] _colVerts;
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
                if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPositionAsVector;
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
                ResizeCollider();
                InitializeVerts();
                _box = BoundingBox.CreateFromPoints(_colVerts);
            }
        }
        #endregion

        #region Methods
        public BoxCollider(Collision owner)
        {
            _owner = owner;
            _edgesSize = new Vector3(1, 1, 1);
            _localPosition = Vector3.Zero;
            _totalPosition = owner.gameObject.transform.Position + _localPosition;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPositionAsVector;
            _trigger = false;
            _colVerts = new Vector3[8];
            ResizeCollider();
            InitializeVerts();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }


        public BoxCollider(Collision owner, Vector3 position, Vector3 size, bool trigger)
        {
            _owner = owner;
            _edgesSize = size;
            _localPosition = position;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPositionAsVector;
            _trigger = trigger;
            _colVerts = new Vector3[8];
            ResizeCollider();
            InitializeVerts();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }

        public BoxCollider(Collision owner, Vector3 size, bool trigger)
        {
            _owner = owner;
            _edgesSize = size;
            _localPosition = Vector3.Zero;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPositionAsVector;
            _trigger = trigger;
            _colVerts = new Vector3[8];
            ResizeCollider();
            InitializeVerts();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }

        public void ResizeCollider()
        {
           _edgesRealSize = EdgesSize *_owner.gameObject.transform.Scale * _owner.gameObject.transform.AncestorsScaleAsVector;       
        }

        private void InitializeVerts()
        {
            //_colVerts[0] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            //_colVerts[1] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            //_colVerts[2] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            //_colVerts[3] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z + (_edgesSize.Z / 2));
            //_colVerts[4] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));
            //_colVerts[5] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y + (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));
            //_colVerts[6] = new Vector3(_totalPosition.X + (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));
            //_colVerts[7] = new Vector3(_totalPosition.X - (_edgesSize.X / 2), _totalPosition.Y - (_edgesSize.Y / 2), _totalPosition.Z - (_edgesSize.Z / 2));


            _colVerts[0] = new Vector3(_totalPosition.X - (_edgesRealSize.X / 2), _totalPosition.Y + (_edgesRealSize.Y / 2), _totalPosition.Z + (_edgesRealSize.Z / 2));
            _colVerts[1] = new Vector3(_totalPosition.X + (_edgesRealSize.X / 2), _totalPosition.Y + (_edgesRealSize.Y / 2), _totalPosition.Z + (_edgesRealSize.Z / 2));
            _colVerts[2] = new Vector3(_totalPosition.X + (_edgesRealSize.X / 2), _totalPosition.Y - (_edgesRealSize.Y / 2), _totalPosition.Z + (_edgesRealSize.Z / 2));
            _colVerts[3] = new Vector3(_totalPosition.X - (_edgesRealSize.X / 2), _totalPosition.Y - (_edgesRealSize.Y / 2), _totalPosition.Z + (_edgesRealSize.Z / 2));
            _colVerts[4] = new Vector3(_totalPosition.X - (_edgesRealSize.X / 2), _totalPosition.Y + (_edgesRealSize.Y / 2), _totalPosition.Z - (_edgesRealSize.Z / 2));
            _colVerts[5] = new Vector3(_totalPosition.X + (_edgesRealSize.X / 2), _totalPosition.Y + (_edgesRealSize.Y / 2), _totalPosition.Z - (_edgesRealSize.Z / 2));
            _colVerts[6] = new Vector3(_totalPosition.X + (_edgesRealSize.X / 2), _totalPosition.Y - (_edgesRealSize.Y / 2), _totalPosition.Z - (_edgesRealSize.Z / 2));
            _colVerts[7] = new Vector3(_totalPosition.X - (_edgesRealSize.X / 2), _totalPosition.Y - (_edgesRealSize.Y / 2), _totalPosition.Z - (_edgesRealSize.Z / 2));
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
            _totalPosition = _owner.gameObject.transform.Position + _localPosition;
            if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPositionAsVector;
            InitializeVerts();
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

        #region Xml Serialization
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            Trigger = Convert.ToBoolean(reader.GetAttribute("IsTrigger"), culture);
            reader.ReadStartElement();
            if (reader.Name == "LocalPosition")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                LocalPosition = tmp;
                reader.Read();
            }
            if (reader.Name == "EdgesSize")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                EdgesSize = tmp;
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            writer.WriteStartElement("BoxCollider");
//            writer.WriteAttributeString("Owner", Owner.gameObject.ID.ToString(culture));
            writer.WriteAttributeString("IsTrigger", Trigger.ToString(culture));
            writer.WriteStartElement("LocalPosition");
            writer.WriteAttributeString("x", LocalPosition.X.ToString("G", culture));
            writer.WriteAttributeString("y", LocalPosition.Y.ToString("G", culture));
            writer.WriteAttributeString("z", LocalPosition.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteStartElement("EdgesSize");
            writer.WriteAttributeString("x", EdgesSize.X.ToString("G", culture));
            writer.WriteAttributeString("y", EdgesSize.Y.ToString("G", culture));
            writer.WriteAttributeString("z", EdgesSize.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        #endregion
        #endregion


    }
}
