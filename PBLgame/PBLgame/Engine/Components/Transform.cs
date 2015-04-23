using System;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class Transform : Component
    {
        #region Variables
        #region Public
        #endregion
        #region Private
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        private Matrix _worldTranslation;
        private Matrix _worldRotation;
        private Matrix _worldScale;
        private Matrix _world;
        #endregion
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
                _worldTranslation = Matrix.CreateTranslation(value);
                _position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                RotationLimit();
                // TODO consider changing to rotation around axes
                _worldRotation = Matrix.CreateRotationX(MathHelper.ToRadians(value.X))
                               * Matrix.CreateRotationY(MathHelper.ToRadians(value.Y))
                               * Matrix.CreateRotationZ(MathHelper.ToRadians(value.Z));
                //_worldRotation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(value.X),
                //                                               MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.Z));
            }
        }
        public Vector3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _worldScale = Matrix.CreateScale(value);
                _scale = value;
            }
        }
        public Matrix World
        {
            get
            {
                if(gameObject.parent == null)
                {
                    return _world = _worldScale * _worldRotation *_worldTranslation;
                }
                else
                {
                    // Changed multiplication order (Unity-like) - now all children are like a whole group, rotated and scaled around parent.
                    return _world = _worldScale * _worldRotation * _worldTranslation * gameObject.parent.transform.World;
                }

            }
        }

        public Matrix WorldRotation { get { return _worldRotation; } }

        /// <summary>
        /// Rotation matrix multiplied recursively from parents (excluding self) to allow global transformation (when inverted).
        /// </summary>
        public Matrix AncestorsRotation
        {
            get
            {
                if (gameObject.parent == null)
                {
                    return Matrix.Identity;
                }
                else
                {
                    return gameObject.parent.transform._worldRotation * gameObject.parent.transform.AncestorsRotation;
                }
            }
        }

        /// <summary>
        /// Scale matrix multiplied recursively from parents (excluding self) to calculate transformation multipler.
        /// </summary>
        public Matrix AncestorsScale
        {
            get
            {
                if (gameObject.parent == null)
                {
                    return Matrix.Identity;
                }
                else
                {
                    return gameObject.parent.transform._worldScale * gameObject.parent.transform.AncestorsScale;
                }
            }
        }

        #endregion

        #region Methods

        public Transform(GameObject owner) : base(owner)
        {
            _position = Vector3.Zero;
            _rotation = Vector3.Zero;
            _scale = new Vector3(1.0f, 1.0f, 1.0f);

            _worldTranslation = Matrix.Identity;
            _worldRotation = Matrix.Identity;
            _worldScale = Matrix.Identity;
        }

        public Transform(Transform src) : base(src._gameObject)
        {
            CopyData(src);
        }

        public Transform(Transform src, GameObject owner) : base(owner)
        {
            CopyData(src);
        }

        private void CopyData(Transform src)
        {
            _position = src._position;
            _rotation = src._rotation;
            _scale    = src._scale;

            _worldTranslation = src._worldTranslation;
            _worldRotation    = src._worldRotation;
            _worldScale       = src._worldScale;
        }

        public void Translate(Vector3 trans)
        {
            _position += trans;
            _worldTranslation *= Matrix.CreateTranslation(trans);  
        }
        public void Translate(float x, float y, float z)
        {
            _position += new Vector3(x,y,z);
            _worldTranslation *= Matrix.CreateTranslation(new Vector3(x,y,z));
        }
        public void Rotate(Vector3 rot)
        {
            _rotation += rot;
            RotationLimit();
            _worldRotation *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rot.X), 
                                                            MathHelper.ToRadians(rot.Y), MathHelper.ToRadians(rot.Z));
        }
        public void Rotate(float x, float y, float z)
        {
            _rotation += new Vector3(x, y, z);
            RotationLimit();
            _worldRotation *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(x),
                                                                       MathHelper.ToRadians(y), MathHelper.ToRadians(z));
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        private void RotationLimit()
        {
            _rotation.X = _rotation.X % 360.0f;
            _rotation.Y = _rotation.Y % 360.0f;
            _rotation.Z = _rotation.Z % 360.0f;
        }

        #region XML serialization

        public override void ReadXml(XmlReader reader)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.ReadXml(reader);
            reader.ReadStartElement();
            if (reader.Name == "Position")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                Position = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Rotation")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                Rotation = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Scale")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                Scale = tmp;
            }
            reader.Read();
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.WriteXml(writer);
            writer.WriteStartElement("Position");
            writer.WriteAttributeString("x", Position.X.ToString("G", culture));
            writer.WriteAttributeString("y", Position.Y.ToString("G", culture));
            writer.WriteAttributeString("z", Position.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteStartElement("Rotation");
            writer.WriteAttributeString("x", Rotation.X.ToString("G", culture));
            writer.WriteAttributeString("y", Rotation.Y.ToString("G", culture));
            writer.WriteAttributeString("z", Rotation.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteStartElement("Scale");
            writer.WriteAttributeString("x", Scale.X.ToString("G", culture));
            writer.WriteAttributeString("y", Scale.Y.ToString("G", culture));
            writer.WriteAttributeString("z", Scale.Z.ToString("G", culture));
            writer.WriteEndElement();
        }

        #endregion


        #endregion
    }
}
