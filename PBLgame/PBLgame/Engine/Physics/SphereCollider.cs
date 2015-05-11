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
    public class SphereCollider : IXmlSerializable
    {
        #region Variables
        private Collision _owner;
        private Vector3 _localPosition;
        private Vector3 _totalPosition;
        private float _radius;
        private bool _trigger;
        private BoundingSphere _sphere = new BoundingSphere();

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
                _sphere.Center = _totalPosition;
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
                _sphere.Radius = _radius;
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
            private set{ }
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
        public SphereCollider(Collision owner)
        {
            _owner = owner;
            _localPosition = Vector3.Zero;
            _totalPosition = owner.gameObject.transform.Position + _localPosition;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _previousPosition = Vector3.Zero;
            _radius = 0.0f;
            _trigger = false;
            _sphere = new BoundingSphere(_totalPosition,_radius);
        }

        public SphereCollider(Collision owner, Vector3 position, float radius, bool trigger)
        {
            _owner = owner;
            _previousPosition = Vector3.Zero;
            _radius = radius;
            _localPosition = position;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = trigger;
            _sphere = new BoundingSphere(TotalPosition, Radius);
        }

        public SphereCollider(Collision owner, float radius, bool trigger)
        {
            _owner = owner;
            _previousPosition = Vector3.Zero;
            _radius = radius;
            _localPosition = Vector3.Zero;
            _totalPosition = _localPosition + owner.gameObject.transform.Position;
            if (owner.gameObject.parent != null) _totalPosition += owner.gameObject.transform.AncestorsPosition;
            _trigger = trigger;
            _sphere = new BoundingSphere(TotalPosition, Radius);
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
            _previousPosition = _totalPosition;
            _totalPosition = _owner.gameObject.transform.Position + _localPosition;
            if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
            _sphere.Center = _totalPosition;
        }

        public void Update()
        {
        //    _previousPosition = _totalPosition;
        //    _totalPosition = _owner.gameObject.transform.Position + _localPosition;
        //    if (_owner.gameObject.parent != null) _totalPosition += _owner.gameObject.transform.AncestorsPosition;
        //    _sphere.Center = _totalPosition;
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
