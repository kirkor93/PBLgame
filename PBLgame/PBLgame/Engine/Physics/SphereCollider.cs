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
    public class SphereCollider : Collider, IXmlSerializable
    {
        #region Variables
        private Collision _owner;
        private Vector3 _localPosition;
        private Vector3 _totalPosition;
        private float _radius;
        private bool _trigger;
        private BoundingSphere _sphere = new BoundingSphere();

        private float _realRadius;
        private Matrix _worldTranslation;
        private Matrix _world;
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
                UpdatePosition();

            }
        }
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                ResizeCollider();
                _sphere = new BoundingSphere(_totalPosition, _realRadius);

            }
        }
        public Vector3 TotalPosition
        {
            get
            {
                _worldTranslation = Matrix.CreateTranslation(_localPosition);
                if (_owner.gameObject.parent != null)
                {
                    _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation * _owner.gameObject.transform.AncestorsRotation * _owner.gameObject.transform.AncestorsTranslation);
                }
                else
                {
                    _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation);
                }
                _totalPosition = _world.Translation;
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
        public BoundingSphere Sphere
        {
            get
            {
                return _sphere;
            }
            set
            {
                _sphere = value;
            }
        }
        #endregion

        #region Methods
        public SphereCollider(Collision owner) : this(owner, false)
        {
        }

        public SphereCollider(Collision owner, bool trigger) : this(owner, 0, trigger)
        {
            // srsly code duplication is like a ticking bomb
        }

        public SphereCollider(Collision owner, Vector3 position, float radius, bool trigger)
        {
            _owner = owner;
            _radius = radius;
            _localPosition = position; 
            _worldTranslation = Matrix.CreateTranslation(_localPosition);
            if (_owner.gameObject.parent != null)
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation * _owner.gameObject.transform.AncestorsRotation * _owner.gameObject.transform.AncestorsTranslation);
            }
            else
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation);
            }
            _totalPosition = _world.Translation;
            _trigger = trigger;
            ResizeCollider();
            _sphere = new BoundingSphere(TotalPosition, _realRadius);
        }

        public SphereCollider(Collision owner, float radius, bool trigger) : this(owner, Vector3.Zero, radius, trigger)
        {
            // i'm not joking
        }

        public SphereCollider(SphereCollider src, Collision owner) : this(owner, src._localPosition, src._radius, src._trigger)
        {
            // should work with init list only
        }

        public void GenerateCollider()
        {
            if (_owner == null || _owner.gameObject.renderer == null || _owner.gameObject.renderer.MyMesh == null || _owner.gameObject.renderer.MyMesh.Model == null) return;
            float minX, maxX, minY, maxY, minZ, maxZ;
            minX = minY = minZ = float.MaxValue;
            maxX = maxY = maxZ = float.MinValue;
            foreach(ModelMesh mMesh in _owner.gameObject.renderer.MyMesh.Model.Meshes)
            {
                foreach(ModelMeshPart part in mMesh.MeshParts )
                {
                    VertexPositionColor[] verts = new VertexPositionColor[part.NumVertices];
                    part.VertexBuffer.GetData<VertexPositionColor>(verts);
                    foreach (VertexPositionColor vert in verts)
                    {
                        if(vert.Position.X > maxX) maxX = vert.Position.X;
                        if(vert.Position.X < minX) minX = vert.Position.X;
                        if(vert.Position.Y > maxY) maxY = vert.Position.Z;
                        if(vert.Position.Y < minY) minY = vert.Position.Z;
                        if(vert.Position.Z > maxZ) maxZ = vert.Position.Y;
                        if(vert.Position.Z < minZ) minZ = vert.Position.Y;
                    }
                }
            }

            //Console.WriteLine("Sphere");
            //Console.WriteLine("Xmin = " + minX);
            //Console.WriteLine("Xmax = " + maxX);
            //Console.WriteLine("Ymin = " + minY);
            //Console.WriteLine("Ymax = " + maxY);
            //Console.WriteLine("Zmin = " + minZ);
            //Console.WriteLine("Zmax = " + maxZ);

            float midX, midY, midZ;
            //midX = (maxX + minX) / 2f;
            //midY = (maxY + minY) / 2f;
            //midZ = (maxZ + minZ) / 2f;


            midX = (Math.Abs(maxX) + Math.Abs(minX)) / 2f;
            midY = (Math.Abs(maxY) + Math.Abs(minY)) / 2f;
            midZ = (Math.Abs(maxZ) + Math.Abs(minZ)) / 2f;
            
            LocalPosition = new Vector3(0f, midY, 0f);

            if (midX > midY && midX > midZ) Radius = midX;
            else if (midY > midX && midY > midZ) Radius = midY;
            else if (midZ > midX && midZ > midY) Radius = midZ;
        }

        public bool Intersect(SphereCollider sphere)
        {
            return _sphere.Intersects(sphere.Sphere);
        }

        public ContainmentType Contains(SphereCollider sphere)
        {
            return _sphere.Contains(sphere.Sphere);
        }

        public ContainmentType Contains(BoxCollider box)
        {
            return _sphere.Contains(box.Box);
        }

        public void UpdatePosition()
        {
            //_totalPosition = LocalPosition + _owner.gameObject.transform.Position;
            //if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPositionAsVector;
            _worldTranslation = Matrix.CreateTranslation(_localPosition);
            if (_owner.gameObject.parent != null)
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation * _owner.gameObject.transform.AncestorsRotation * _owner.gameObject.transform.AncestorsTranslation);
            }
            else
            {
                _world = (_worldTranslation * _owner.gameObject.transform.WorldRotation * _owner.gameObject.transform.WorldTranslation);
            }
            _totalPosition = _world.Translation;
            _sphere.Center = _totalPosition;
        }

        public void ResizeCollider()
        {
            Vector3 tmpVec = _owner.gameObject.transform.Scale * _owner.gameObject.transform.AncestorsScaleAsVector;
            float tmpFloat = tmpVec.X;
            if (tmpVec.Y > tmpFloat) tmpFloat = tmpVec.Y;
            if (tmpVec.Z > tmpFloat) tmpFloat = tmpVec.Z;

            _realRadius = _radius * tmpFloat;
        }

        public void Update()
        {

        }

        
        public void Draw()
        {
            List<Vector3> verticesToDraw = new List<Vector3>();
            List<short> indices = new List<short>();
            Vector3 right = Vector3.Right * this.Sphere.Radius;
            Vector3 verticalCirclePoint;
            Vector3 horizontalCirclePoint;
            Vector3 rightUp = Vector3.Right + Vector3.Up;
            Vector3 rightDown = Vector3.Right + Vector3.Down;
            rightUp.Normalize();
            rightDown.Normalize();
            rightDown *= this.Sphere.Radius;
            rightUp *= this.Sphere.Radius;
            Vector3 pointToAdd;
            for (short i = 0; i < 360; ++i)
            {
                if (i - 1 > 0)
                {
                    indices.Add((short)(i - 1));
                }
                verticalCirclePoint = Vector3.Transform(right, Matrix.CreateRotationZ(MathHelper.ToRadians(i)));
                indices.Add(i);
                verticalCirclePoint.X += _totalPosition.X;
                verticalCirclePoint.Y += _totalPosition.Y;
                verticalCirclePoint.Z += _totalPosition.Z;
                verticesToDraw.Add(verticalCirclePoint);
            }

            for (short i = 0; i < 360; ++i)
            {
                if (i - 1 > 0)
                {
                    indices.Add((short)(360 + i - 1));
                }
                horizontalCirclePoint = Vector3.Transform(right, Matrix.CreateRotationY(MathHelper.ToRadians(i)));
                indices.Add((short)(360 + i));
                horizontalCirclePoint.X += _totalPosition.X;
                horizontalCirclePoint.Y += _totalPosition.Y;
                horizontalCirclePoint.Z += _totalPosition.Z;
                verticesToDraw.Add(horizontalCirclePoint);
            }

            for (short i = 0; i < 360; ++i)
            {
                if (i - 1 > 0)
                {
                    indices.Add((short)(720 + i - 1));
                }
                pointToAdd = Vector3.Transform(rightUp, Matrix.CreateRotationZ(MathHelper.ToRadians(i)) * Matrix.CreateRotationY(MathHelper.ToRadians(45.0f)));
                indices.Add((short)(720 + i));
                pointToAdd.X += _totalPosition.X;
                pointToAdd.Y += _totalPosition.Y;
                pointToAdd.Z += _totalPosition.Z;
                verticesToDraw.Add(pointToAdd);
            }

            for (short i = 0; i < 360; ++i)
            {
                if (i - 1 > 0)
                {
                    indices.Add((short)(1080 + i - 1));
                }
                pointToAdd = Vector3.Transform(rightDown, Matrix.CreateRotationZ(MathHelper.ToRadians(i)) * Matrix.CreateRotationY(MathHelper.ToRadians(-45.0f)));
                indices.Add((short)(1080 + i));
                pointToAdd.X += _totalPosition.X;
                pointToAdd.Y += _totalPosition.Y;
                pointToAdd.Z += _totalPosition.Z;
                verticesToDraw.Add(pointToAdd);
            }

            Vector3[] vertices = verticesToDraw.ToArray();
            VertexPositionColor[] primitiveList = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; ++i)
            {
                primitiveList[i] = new VertexPositionColor(vertices[i], Color.White);
            }
            GraphicsDevice gd = GlobalInventory.Instance.GraphicsDevice;

            BasicEffect lineEffect = new BasicEffect(gd);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;

            VertexBuffer buffer = new VertexBuffer(gd, typeof(VertexPositionColor), primitiveList.Length, BufferUsage.None);
            buffer.SetData(primitiveList);
            short[] indicesArray = indices.ToArray();
            IndexBuffer ib = new IndexBuffer(gd, IndexElementSize.SixteenBits, indicesArray.Length, BufferUsage.WriteOnly);
            ib.SetData(indicesArray);
            gd.SetVertexBuffer(buffer);
            gd.Indices = ib;

            lineEffect.World = Matrix.Identity;
            lineEffect.View = Camera.MainCamera.ViewMatrix;
            lineEffect.Projection = Camera.MainCamera.ProjectionMatrix;
            foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.LineList, primitiveList, 0, 1440, indicesArray, 0, 4 * 359);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}R = {1}", Trigger ? "Trigger, " : "", Radius);
        }

        #region Xml serialization
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            Radius = Convert.ToInt32(reader.GetAttribute("Radius"), culture);
            Trigger = Convert.ToBoolean(reader.GetAttribute("IsTrigger"), culture);
            reader.ReadStartElement();
            if (reader.Name == "LocalPosition")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                LocalPosition = tmp;
            }
            reader.Read();

        }

        public void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            writer.WriteStartElement("SphereCollider");
            writer.WriteAttributeString("Radius", Radius.ToString("G", culture));
            writer.WriteAttributeString("IsTrigger", Trigger.ToString(culture));
            writer.WriteStartElement("LocalPosition");
            writer.WriteAttributeString("x", LocalPosition.X.ToString("G", culture));
            writer.WriteAttributeString("y", LocalPosition.Y.ToString("G", culture));
            writer.WriteAttributeString("z", LocalPosition.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        #endregion
        #endregion
    }
}
