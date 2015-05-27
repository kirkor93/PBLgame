using System;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using PBLgame.Engine.GameObjects;

using PBLgame.Engine.Physics;

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
                if(gameObject.collision != null && gameObject.collision.Rigidbody)
                {
                    Vector3 prevPos = _position;
                    _position = value;
                    int flag = 0;
                    foreach (GameObject go in Physics.PhysicsSystem.CollisionObjects)
                    {
                        if (gameObject != go && gameObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                        {
                            flag = gameObject.collision.ChceckCollisionDeeper(go);
                        }
                    }
                    if (flag == 0)
                    {
                        _worldTranslation = Matrix.CreateTranslation(value);
                    }
                    else
                    {
                        _position = prevPos;
                    }


                }
                else
                {
                    _worldTranslation = Matrix.CreateTranslation(value);
                    _position = value;
                }
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
                _worldRotation = CreateRotationXYZMatrix(value);
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
                if (gameObject.collision != null)
                {
                    gameObject.collision.ResizeColliders();
                }
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

        public Vector3 WorldPosition
        {
            get
            {
                return World.Translation;
            }
        }

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

        public Vector3 AncestorsPositionAsVector
        {
            get
            {
                // TODO are u sure it works as desired?
                if(gameObject.parent == null)
                {
                    return Vector3.Zero;
                }
                else
                {
                    return gameObject.parent.transform.Position + gameObject.parent.transform.AncestorsPositionAsVector;
                }
            }
        }

        public Vector3 AncestorsScaleAsVector
        {
            get
            {
                if (gameObject.parent == null)
                {
                    return Vector3.One;
                }
                else
                {
                    return gameObject.parent.transform.Scale * gameObject.parent.transform.AncestorsScaleAsVector;
                }
            }
        }

        public Matrix AncestorsWorld
        {
            get
            {
                if (gameObject.parent == null)
                {
                    return Matrix.Identity;
                }
                else
                {
                    return gameObject.parent.transform.World * gameObject.parent.transform.AncestorsWorld;
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
            if (gameObject.collision != null && gameObject.collision.Rigidbody)
            {
                Vector3 prevPos = _position;
                _position += trans;
                int flag = 0;
                foreach (GameObject go in Physics.PhysicsSystem.CollisionObjects)
                {
                    if (gameObject != go && gameObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        flag = gameObject.collision.ChceckCollisionDeeper(go);
                    }
                }
                if (flag == 0)
                {
                    _worldTranslation *= Matrix.CreateTranslation(trans);
                }
                else
                {
                    _position = prevPos;
                }
            }
            else
            {
                _position += trans;
                _worldTranslation *= Matrix.CreateTranslation(trans);  
            }

        }
        public void Translate(float x, float y, float z)
        {
            if (gameObject.collision != null && gameObject.collision.Rigidbody)
            {
                Vector3 prevPos = _position;
                _position += new Vector3(x, y, z);
                int flag = 0;
                foreach (GameObject go in Physics.PhysicsSystem.CollisionObjects)
                {
                    if (gameObject != go && gameObject.collision.MainCollider.Contains(go.collision.MainCollider) != ContainmentType.Disjoint)
                    {
                        flag = gameObject.collision.ChceckCollisionDeeper(go);
                    }
                }
                if (flag == 0)
                {
                    _worldTranslation *= Matrix.CreateTranslation(new Vector3(x, y, z));
                }
                else
                {
                    _position = prevPos;
                }
            }
            else
            {
                _position += new Vector3(x, y, z);
                _worldTranslation *= Matrix.CreateTranslation(new Vector3(x, y, z));
            }

        }

        //public void Rotate(Vector3 rot)
        //{
        //    _rotation += rot;
        //    RotationLimit();
        //    _worldRotation *= CreateRotationXYZMatrix(rot);
        //}

        //public void Rotate(float x, float y, float z)
        //{
        //    Vector3 rot = new Vector3(x, y, z);
        //    Rotate(rot);
        //}

        /// <summary>
        /// Magically transforms gameObject into new parent's world space, 
        /// so that it stays globally unchanged after kidnapping.
        /// </summary>
        /// <param name="newParent"></param>
        public void Reparent(GameObject newParent)
        {
            GameObject oldParent = _gameObject.parent;
            if (oldParent != null)
            {
                Position = Vector3.Transform(Position, oldParent.transform.World);
                Rotation = CalculateRotationVector(_worldRotation * AncestorsRotation);
                Scale    = Vector3.Transform(Scale, AncestorsScale);
            }
            if (newParent != null)
            {
                Position = Vector3.Transform(Position, Matrix.Invert(newParent.transform.World));
                Rotation = CalculateRotationVector(_worldRotation * Matrix.Invert(newParent.transform._worldRotation * newParent.transform.AncestorsRotation));
                Scale    = Vector3.Transform(Scale, Matrix.Invert(newParent.transform._worldScale * newParent.transform.AncestorsScale));
            }
        }

        /// <summary>
        /// Converts rotation matrix into XYZ rotation Vector3.
        /// </summary>
        /// <param name="matrix">source rotation matrix</param>
        /// <returns>rotation vector</returns>
        public static Vector3 CalculateRotationVector(Matrix matrix)
        {

            float psi, theta, fi;
            if (AlmostEqual(matrix.M13, -1.0f))
            {
                fi = 0;
                psi = (float) (fi + Math.Atan2(matrix.M21, matrix.M31));;
                theta = MathHelper.PiOver2;
            }
            else if (AlmostEqual(matrix.M13, 1.0f))
            {
                fi = 0;
                psi = (float) (-fi + Math.Atan2(matrix.M21, matrix.M31));
                theta = -MathHelper.PiOver2;
            }
            else
            {
                float theta1 = (float) -Math.Asin(matrix.M13);
                float theta2 = MathHelper.Pi - theta1;
                float psi1 = (float) Math.Atan2(matrix.M23 / Math.Cos(theta1),  matrix.M33 / Math.Cos(theta1));
                float psi2 = (float) Math.Atan2(matrix.M23 / Math.Cos(theta2),  matrix.M33 / Math.Cos(theta2));

                float fi1  = (float) Math.Atan2(matrix.M12 / Math.Cos(theta1),  matrix.M11 / Math.Cos(theta1));
                float fi2  = (float) Math.Atan2(matrix.M12 / Math.Cos(theta2),  matrix.M11 / Math.Cos(theta2));

                float sum1 = Math.Abs(psi1) + Math.Abs(theta1) + Math.Abs(fi1);
                float sum2 = Math.Abs(psi2) + Math.Abs(theta2) + Math.Abs(fi2);

                if (sum1 < sum2)
                {
                    psi = psi1;
                    theta = theta1;
                    fi = fi1;
                }
                else
                {
                    psi = psi2;
                    theta = theta2;
                    fi = fi2;
                }

            }
            return new Vector3( MathHelper.ToDegrees(psi), 
                                MathHelper.ToDegrees(theta), 
                                MathHelper.ToDegrees(fi)
            );
        }
        
        public static Matrix CreateRotationXYZMatrix(Vector3 rot)
        {
            return Matrix.CreateRotationX(MathHelper.ToRadians(rot.X))
                 * Matrix.CreateRotationY(MathHelper.ToRadians(rot.Y))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians(rot.Z));
        }

        public static  bool AlmostEqual(float a, float b)
        {
            return Math.Abs(a - b) < 0.00001f;
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
