using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Xml;
using PBLgame.Engine.GameObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.Singleton;

namespace PBLgame.Engine.Components
{
    public class ParticleSystem : BillboardBase
    {
        #region Variables
        private Vector3 _dirFrom;
        private Vector3 _dirTo;
        private float _speed;
        private float _duration;
        private float _lifeTimeLimit;
        private bool _triggered;
        private int _max;
        private bool _loop;
        private List<Burst> _bursts;

        private bool _static;

        //Class private without properties so only for class use 
        private float _timer;
        private float _autoTimer = 1.0f;
        private float _actualTime;
        private int _activated = 0;
        private Vector3 _previousFramePosition;
        private Vector3 _currentFramePosition;

        private VertexPositionTexture[] _verts;
        private Vector3[] _directions;
        private bool[] _activationStates;
        private float[] _particleTimes;

        private VertexBuffer _vertexBuffer;

        private Random _random;
        #endregion

        #region Properties
        public override float Height
        {
            get { return _height; }
            set 
            {
                _height = value;
                InitializeParticles();
            }
        }
        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }
        public float Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }
        public override bool Triggered
        {
            get
            {
                return _triggered;
            }
            set
            {
                _triggered = value;
                if(_triggered)
                {
                    _timer = 0.0f;
                    _autoTimer = 0.0f;
                    _timer = 0.0f;
                    _actualTime = 0.0f;
                    _activated = 0;
                    for(int i = 0; i<_activationStates.Count(); ++i)
                    {
                        _activationStates[i] = false;
                    }
                    InitializeParticles();
                }
            }
        }
        public override Vector2 Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                InitializeParticles();
            }
        }
        public Vector3 DirectionFrom
        {
            get
            {
                return _dirFrom;
            }
            set
            {
                _dirFrom = value;
            }
        }
        public Vector3 DirectionTo
        {
            get
            {
                return _dirTo;
            }
            set
            {
                _dirTo = value;
            }
        }
        public float LifeTimeLimit
        {
            get
            {
                return _lifeTimeLimit;
            }
            set
            {
                _lifeTimeLimit = value;
            }
        }
        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                _verts = new VertexPositionTexture[_max * 4];
                _directions = new Vector3[_max];
                _activationStates = new bool[_max];
                _particleTimes = new float[_max];
                InitializeParticles();
            }
        }
        public bool Loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
            }
        }
        public List<Burst> Bursts
        {
            get
            {
                return _bursts;
            }
            set
            {
                _bursts = value;
            }
        }
        public bool Static
        {
            get { return _static; }
            set { _static = value; }
        }
        #endregion

        #region Methods
        public ParticleSystem(GameObject owner) : base(owner)
        {
            Construct();
        }

        public ParticleSystem(GameObject owner,Vector2 size,int numb)
            : base(owner)
        {
            _size = size;
            _max = numb;

            Construct();

            InitializeParticles();
        }

        private void Construct()
        {
            _random = new Random();
            _timer = 0.0f;
            _verts = new VertexPositionTexture[_max * 4];
            _directions = new Vector3[_max];
            _activationStates = new bool[_max];
            _particleTimes = new float[_max];
            _bursts = new List<Burst>();
            _static = true;
            _previousFramePosition = new Vector3();
            _currentFramePosition = new Vector3();
        }

        public ParticleSystem(ParticleSystem src, GameObject owner) : base(owner)
        {
            // TODO check if copying properly
            _random = new Random();
            _material         = src._material;
            _size             = src._size;
            _dirFrom          = src._dirFrom;
            _dirTo            = src._dirTo;
            _speed            = src._speed;
            _duration         = src._duration;
            _lifeTimeLimit    = src._lifeTimeLimit;
            _triggered        = src._triggered;
            _max              = src._max;
            _loop             = src._loop;
            _bursts           = new List<Burst>(src._bursts);
            _timer            = src._timer;
            _autoTimer        = src._autoTimer;
            _actualTime       = src._actualTime;
            _activated        = src._activated;
            Max               = src.Max;
            _previousFramePosition     = src._previousFramePosition;
            Static            = src.Static;
                
            //InitializeParticles(); // done in Max

        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled && Triggered)
            {
                _currentFramePosition = gameObject.transform.World.Translation;
                float deltaTime = gameTime.ElapsedGameTime.Milliseconds/1000.0f;
                ParticlesUpdate(_actualTime);
                if (_actualTime > Duration)
                {
                    _actualTime = _actualTime % Duration;
                    _activated = 0;
                    //for (int i = 0; i < _activationStates.Count(); ++i)
                    //{
                    //    _activationStates[i] = false;
                    //}
                }
                if (Loop)
                {
                    if (Bursts.Count >= 1)
                    {
                        foreach (Burst burst in Bursts)
                        {
                            if(burst.When == 0.0f)
                            {
                                if ((_actualTime < deltaTime) && (burst.When < (_actualTime + deltaTime)))
                                {
                                    Emmit(burst.HowMany, _actualTime);
                                }
                            }
                            else
                            {
                                if ((burst.When > (_actualTime - (deltaTime / 2.0f))) && (burst.When <= (_actualTime + (deltaTime / 2.0f))))
                                {
                                    Emmit(burst.HowMany, _actualTime);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_autoTimer >= ((Duration / Max) * deltaTime))
                        {
                            int count = Convert.ToInt16(((Max / Duration) * deltaTime) + 1);
                            Emmit(count, _actualTime);
                            _autoTimer = _autoTimer % ((Max / Duration) * deltaTime);
                        }
                    }
                }
                else
                {
                    if (_timer < Duration)
                    {
                        if (Bursts.Count >= 1)
                        {
                            foreach (Burst burst in Bursts)
                            {
                                if ((burst.When > _timer - deltaTime / 2) && (burst.When < _timer + deltaTime / 2))
                                {
                                    Emmit(burst.HowMany, _timer);
                                }
                            }
                        }
                        else
                        {
                            if (_autoTimer >= ((Duration / Max) * deltaTime))
                            {
                                int count = Convert.ToInt16(((Max / Duration) * deltaTime) + 1);
                                Emmit(count, _timer);
                                _autoTimer = _autoTimer % ((Max / Duration) * deltaTime);
                            }
                        }
                    }
                    else
                    {
                        Triggered = false;
                    }
                }

                _autoTimer += deltaTime;
                _timer += deltaTime;
                _actualTime += deltaTime;

                _previousFramePosition = gameObject.transform.World.Translation;
            }
        }

        private void ParticlesUpdate(float time)
        {
            for(int i = 0; i < Max ; i++)
            {
                if(_activationStates[i])
                {
                    if(_particleTimes[i] + LifeTimeLimit < _timer)
                    {
                        _activationStates[i] = false;
                        _verts[i * 4].Position = new Vector3((-Size.X), Size.Y + _height, 0);
                        _verts[i * 4 + 1].Position = new Vector3(Size.X, Size.Y + _height, 0);
                        _verts[i * 4 + 2].Position = new Vector3((-Size.X), (-Size.Y + _height), 0);
                        _verts[i * 4 + 3].Position = new Vector3(Size.X, (-Size.Y + _height), 0);

                    }
                    else
                    {
                        for(int j = 0; j < 4; j++)
                        {
                            if (_static) _verts[i * 4 + j].Position += _directions[i] * Speed;
                            else
                            {
                                _verts[i * 4 + j].Position += (_currentFramePosition - _previousFramePosition) + _directions[i] * Speed;
                            }
                        }
                    }
                }
            }
        }

        private void Emmit(int count,float timer)
        {
            int counter = 0;
            for(int i = _activated; i < Max ; i++)
            {
                if(!_activationStates[i])
                {
                    if(counter == count)
                    {
                        return;
                    }
                    ParticleSetActive(i,timer);
                    _activated += 1;
                    counter+=1;
                }
            }
        }

        private void ParticleSetActive(int index, float timer)
        {
            for (int i = 0; i < 4; i++ )
            {
                _verts[index * 4 + i].Position += _currentFramePosition;
            }
            if (DirectionFrom == DirectionTo)
            {
                _directions[index] = DirectionFrom;
            }
            else
            {
                _directions[index] = new Vector3(_random.Next(Convert.ToInt16(DirectionFrom.X * 100), Convert.ToInt16(DirectionTo.X * 100 + 1)),
                    _random.Next(Convert.ToInt16(DirectionFrom.Y * 100), Convert.ToInt16(DirectionTo.Y * 100 + 1)), _random.Next(Convert.ToInt16(DirectionFrom.Z * 100), Convert.ToInt16(DirectionTo.Z * 100 + 1))) * 0.01f;
            }
            _particleTimes[index] = _timer;
            _activationStates[index] = true;
        }


        private void InitializeParticles()
        {
            Vector3 pos = gameObject.transform.Position + gameObject.transform.AncestorsTranslation.Translation;
            for(int i = 0 ; i < Max ; i++)
            {   
                int tmp = i*4;
                _verts[tmp] = new VertexPositionTexture(new Vector3((-Size.X), Size.Y + _height, 0), Vector2.Zero);
                _verts[tmp + 1] = new VertexPositionTexture(new Vector3(Size.X, Size.Y + _height, 0), new Vector2(1, 0));
                _verts[tmp + 2] = new VertexPositionTexture(new Vector3((-Size.X), (-Size.Y) + _height, 0), new Vector2(0, 1));
                _verts[tmp + 3] = new VertexPositionTexture(new Vector3(Size.X, (-Size.Y) + _height, 0), new Vector2(1, 1));
            }
            _vertexBuffer = new VertexBuffer(GlobalInventory.Instance.GraphicsDevice,
                    typeof(VertexPositionTexture), _verts.Length, BufferUsage.None);
        }

        public override void Draw(GameTime gameTime)
        {
            if(Enabled && Triggered)
            {
                GraphicsDevice graphicsDevice = GlobalInventory.Instance.GraphicsDevice;
                graphicsDevice.SetVertexBuffer(_vertexBuffer);
                // Only draw if there are live particles
                graphicsDevice.BlendState = BlendState.AlphaBlend;
                graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                for (int i = 0; i < Max; i++ )
                {
                    if(_activationStates[i])
                    {
                        _material.ShaderEffect.Parameters["CamPos"].SetValue(Camera.MainCamera.transform.Position);
                        _material.ShaderEffect.Parameters["AllowedRotDir"].SetValue(new Vector3(0, 1, 0));
                        _material.ShaderEffect.Parameters["Alpha"].SetValue(0.5f);
                        _material.ShaderEffect.Parameters["World"].SetValue(Matrix.Identity);
                        _material.ShaderEffect.Parameters["View"].SetValue(Camera.MainCamera.ViewMatrix);
                        _material.ShaderEffect.Parameters["Projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
                        _material.ShaderEffect.Parameters["ParticleTexture"].SetValue(_material.Diffuse);
                        // Draw particles
                        foreach (EffectPass pass in _material.ShaderEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                                PrimitiveType.TriangleStrip, _verts, i * 4, 2);
                        }
                    }
                }
                graphicsDevice.BlendState = BlendState.NonPremultiplied;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }                
        }

        public override Component Copy(GameObject newOwner)
        {
            return new ParticleSystem(this, newOwner);
        }

        public void AddBurst(Burst burst)
        {
            _bursts.Add(burst);
        }
        public void DeleteBurst(Burst burst)
        {
            _bursts.Remove(burst);
        }
        #region XML Serialization

        public override void ReadXml(XmlReader reader)
        {
            Bursts = new List<Burst>();
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.ReadXml(reader);

            int matId = Convert.ToInt32(reader.GetAttribute("MaterialId"), culture);
            Material = ResourceManager.Instance.GetMaterial(matId);
            Duration = Convert.ToSingle(reader.GetAttribute("Duration"), culture);
            LifeTimeLimit = Convert.ToSingle(reader.GetAttribute("LifeTimeLimit"), culture);
            Loop = Convert.ToBoolean(reader.GetAttribute("Loop"), culture);
            Max = Convert.ToInt32(reader.GetAttribute("Max"), culture);
            Speed = Convert.ToSingle(reader.GetAttribute("Speed"), culture);
            Triggered = Convert.ToBoolean(reader.GetAttribute("Triggered"), culture);
            Height = Convert.ToSingle(reader.GetAttribute("Height"), culture);

            reader.ReadStartElement();
            if (reader.Name == "DirectionFrom")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                DirectionFrom = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "DirectionTo")
            {
                Vector3 tmp = Vector3.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                tmp.Z = Convert.ToSingle(reader.GetAttribute("z"), culture);
                DirectionTo = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Size")
            {
                Vector2 tmp = Vector2.Zero;
                tmp.X = Convert.ToSingle(reader.GetAttribute("x"), culture);
                tmp.Y = Convert.ToSingle(reader.GetAttribute("y"), culture);
                Size = tmp;
            }
            reader.ReadStartElement();
            if (reader.Name == "Bursts")
            {

                reader.ReadStartElement();
                while (reader.Name == "Burst")
                {
                    float when = Convert.ToSingle(reader.GetAttribute("When"), culture);
                    int howMany = Convert.ToInt32(reader.GetAttribute("HowMany"), culture);
                    Bursts.Add(new Burst(when, howMany));
                    reader.Read();
                }
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            base.WriteXml(writer);

            writer.WriteAttributeString("MaterialId", Material.Id.ToString("G", culture));
            writer.WriteAttributeString("Duration", Duration.ToString("G", culture));
            writer.WriteAttributeString("LifeTimeLimit", LifeTimeLimit.ToString("G", culture));
            writer.WriteAttributeString("Loop", Loop.ToString(culture));
            writer.WriteAttributeString("Max", Max.ToString("G", culture));
            writer.WriteAttributeString("Speed", Speed.ToString("G", culture));
            writer.WriteAttributeString("Triggered", Triggered.ToString(culture));
            writer.WriteAttributeString("Height", Height.ToString("G", culture));
            writer.WriteStartElement("DirectionFrom");
            writer.WriteAttributeString("x", DirectionFrom.X.ToString("G", culture));
            writer.WriteAttributeString("y", DirectionFrom.Y.ToString("G", culture));
            writer.WriteAttributeString("z", DirectionFrom.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteStartElement("DirectionTo");
            writer.WriteAttributeString("x", DirectionTo.X.ToString("G", culture));
            writer.WriteAttributeString("y", DirectionTo.Y.ToString("G", culture));
            writer.WriteAttributeString("z", DirectionTo.Z.ToString("G", culture));
            writer.WriteEndElement();
            writer.WriteStartElement("Size");
            writer.WriteAttributeString("x", Size.X.ToString("G", culture));
            writer.WriteAttributeString("y", Size.Y.ToString("G", culture));
            writer.WriteEndElement();
            if(_bursts.Count > 0)
            {
                writer.WriteStartElement("Bursts");
                foreach (Burst burst in Bursts)
                {
                    writer.WriteStartElement("Burst");
                    writer.WriteAttributeString("When", burst.When.ToString("G", culture));
                    writer.WriteAttributeString("HowMany", burst.HowMany.ToString("G", culture));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        #endregion
        #endregion
    }

    public struct Burst
    {
        public float When;
        public int HowMany;

        public Burst(float when, int howMany)
        {
            When = when;
            HowMany = howMany;
        }
    }

}
