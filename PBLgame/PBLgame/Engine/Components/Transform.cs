using System;
using System.Xml;
using Microsoft.Xna.Framework;
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
                _worldRotation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(value.X),
                                                               MathHelper.ToRadians(value.Y), MathHelper.ToRadians(value.Z));
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
                    return _world = gameObject.parent.transform.World * _worldScale * _worldRotation * _worldTranslation;
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

        public void Translate(Vector3 trans)
        {
            _position += trans;
            _worldTranslation *= Matrix.CreateTranslation(trans);  
        }
        public void Translate(float x,float y, float z)
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

        public override void Update()
        {
            
        }

        public override void Draw()
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
            reader.MoveToContent();
            base.ReadXml(reader);
            reader.ReadStartElement();
            if (reader.Name == "Position")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"));
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"));
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"));
                Position = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Rotation")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"));
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"));
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"));
                Rotation = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Scale")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"));
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"));
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"));
                Scale = tmp;
            }
            reader.Read();
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteStartElement("Position");
            writer.WriteAttributeString("x", Position.X.ToString("G"));
            writer.WriteAttributeString("y", Position.Y.ToString("G"));
            writer.WriteAttributeString("z", Position.Z.ToString("G"));
            writer.WriteEndElement();
            writer.WriteStartElement("Rotation");
            writer.WriteAttributeString("x", Rotation.X.ToString("G"));
            writer.WriteAttributeString("y", Rotation.Y.ToString("G"));
            writer.WriteAttributeString("z", Rotation.Z.ToString("G"));
            writer.WriteEndElement();
            writer.WriteStartElement("Scale");
            writer.WriteAttributeString("x", Scale.X.ToString("G"));
            writer.WriteAttributeString("y", Scale.Y.ToString("G"));
            writer.WriteAttributeString("z", Scale.Z.ToString("G"));
            writer.WriteEndElement();
        }

        #endregion


        #endregion
    }
}
