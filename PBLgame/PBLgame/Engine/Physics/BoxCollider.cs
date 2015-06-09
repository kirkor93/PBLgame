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
    public class BoxCollider : Collider, IXmlSerializable
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

        private Matrix _worldTranslation;
        private Matrix _world;

        //private Quaternion tmpQ = new Quaternion();
        //private Vector3 tmpV = new Vector3();
        #endregion

        #region Properties
        public Collision Owner
        {
            get
            {
                return _owner;
            }
            //private set { }
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
                _worldTranslation = Matrix.CreateTranslation(_localPosition);
                InitializeVerts();
            }
        }

        public Vector3 TotalPosition
        {
            get
            {
                return _totalPosition;
            }
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
        public BoxCollider(Collision owner) : this(owner, false)
        {
        }

        public BoxCollider(Collision owner, bool trigger) : this(owner, new Vector3(1, 1, 1), trigger)
        {
        }


        public BoxCollider(Collision owner, Vector3 position, Vector3 size, bool trigger)
        {
            _owner = owner;
            _edgesSize = size;
            _localPosition = position;
            _worldTranslation = Matrix.CreateTranslation(_localPosition);
            if (_owner.gameObject.parent != null)
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation * _owner.gameObject.transform.AncestorsRotation * _owner.gameObject.transform.AncestorsTranslation);
                //_world.Decompose(out tmpV, out tmpQ, out _totalPosition);
            }
            else
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation);
                //_world.Decompose(out tmpV, out tmpQ, out _totalPosition);
            }
            _totalPosition = _world.Translation;
            _trigger = trigger;
            _colVerts = new Vector3[8];
            ResizeCollider();
            InitializeVerts();
            _box = BoundingBox.CreateFromPoints(_colVerts);
        }

        public BoxCollider(Collision owner, Vector3 size, bool trigger) : this(owner, Vector3.Zero, size, trigger)
        {
        }

        public void ResizeCollider()
        {
           _edgesRealSize = EdgesSize *_owner.gameObject.transform.Scale * _owner.gameObject.transform.AncestorsScaleAsVector;       
        }

        public void GenerateCollider()
        {
            if (_owner == null || _owner.gameObject.renderer == null || _owner.gameObject.renderer.MyMesh == null || _owner.gameObject.renderer.MyMesh.Model == null) return;
            float minX, maxX, minY, maxY, minZ, maxZ;
            minX = minY = minZ = float.MaxValue;
            maxX = maxY = maxZ = float.MinValue;
            foreach (ModelMesh mMesh in _owner.gameObject.renderer.MyMesh.Model.Meshes)
            {
                foreach (ModelMeshPart part in mMesh.MeshParts)
                {
                    VertexPositionColor[] verts = new VertexPositionColor[part.NumVertices];
                    part.VertexBuffer.GetData<VertexPositionColor>(verts);
                    foreach (VertexPositionColor vert in verts)
                    {
                        if (vert.Position.X > maxX) maxX = vert.Position.X;
                        if (vert.Position.X < minX) minX = vert.Position.X;
                        if (vert.Position.Y > maxY) maxY = vert.Position.Z;
                        if (vert.Position.Y < minY) minY = vert.Position.Z;
                        if (vert.Position.Z > maxZ) maxZ = vert.Position.Y;
                        if (vert.Position.Z < minZ) minZ = vert.Position.Y;
                    }
                }
            }

            //Console.WriteLine("Box");
            //Console.WriteLine("Xmin = " + minX);
            //Console.WriteLine("Xmax = " + maxX);
            //Console.WriteLine("Ymin = " + minY);
            //Console.WriteLine("Ymax = " + maxY);
            //Console.WriteLine("Zmin = " + minZ);
            //Console.WriteLine("Zmax = " + maxZ);
            float midX, midY, midZ;

            midX = (Math.Abs(maxX) + Math.Abs(minX)) / 2f;
            midY = (Math.Abs(maxY) + Math.Abs(minY)) / 2f;
            midZ = (Math.Abs(maxZ) + Math.Abs(minZ)) / 2f;

            LocalPosition = new Vector3(0f, midY, 0f);
            EdgesSize = new Vector3(midX, midY, midZ);

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
            _worldTranslation = Matrix.CreateTranslation(_localPosition);//Matrix.CreateTranslation(_localPosition);
            if (_owner.gameObject.parent != null)
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation * _owner.gameObject.transform.AncestorsRotation * _owner.gameObject.transform.AncestorsTranslation);
                //_world.Decompose(out tmpV, out tmpQ, out _totalPosition);
            }
            else
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation);
                //_world.Decompose(out tmpV, out tmpQ, out _totalPosition);
            }
            _totalPosition = _world.Translation;
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

        public override string ToString()
        {
            return String.Format("{0}Size: {1}", Trigger ? "Trigger, " : "", EdgesSize.ToShortString(" x "));
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
