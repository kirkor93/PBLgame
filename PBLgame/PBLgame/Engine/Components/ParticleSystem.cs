using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PBLgame.Engine.GameObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLgame.Engine.Components
{
    public class ParticleSystem : Component
    {
        #region Variables
        private MeshMaterial _material;
        private Vector2 _size;
        private Vector3 _dirFrom;
        private Vector3 _dirTo;
        private float _speed;
        private float _duration;
        private float _lifeTimeLimit;
        private bool _triggered;
        private int _max;
        private bool _loop;
        private List<Burst> _bursts;

        //Class private without properties so only for class use 
        private float _timer;
        private float _autoTimer = 1.0f;
        private float _actualTime;
        private int _activated = 0;

        private VertexPositionTexture[] _verts;
        private Vector3[] _directions;
        private bool[] _activationStates;
        private float[] _particleTimes;

        private VertexBuffer _vertexBuffer;

        private Random _random;
        #endregion

        #region Properties
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
        public MeshMaterial Material
        {
            get
            {
                return _material;
            }
            set
            {
                _material = value;
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
        public bool Triggered
        {
            get
            {
                return _triggered;
            }
            set
            {
                _triggered = value;
            }
        }
        public Vector2 Size
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
        #endregion

        #region Methods
        public ParticleSystem(GameObject owner) : base(owner)
        {
            _random = new Random();
            _timer = 0.0f;
            _verts = new VertexPositionTexture[_max * 4];
            _directions = new Vector3[_max];
            _activationStates = new bool[_max];
            _particleTimes = new float[_max];
            _bursts = new List<Burst>();


            InitializeParticles();
        }

        public ParticleSystem(GameObject owner,Vector2 size,int numb)
            : base(owner)
        {
            _bursts = new List<Burst>();
            _size = size;
            _max = numb;
            _random = new Random();
            _timer = 0.0f;
            _verts = new VertexPositionTexture[_max * 4];
            _directions = new Vector3[_max];
            _activationStates = new bool[_max];
            _particleTimes = new float[_max];

            InitializeParticles();
        }

        public override void Update()
        {
            if (Triggered)
            {

                float deltaTime = Game.Instance.Time.ElapsedGameTime.Milliseconds/1000.0f;
                _autoTimer += deltaTime;
                _timer += deltaTime;
                _actualTime += deltaTime;
                if (_actualTime > Duration)
                {
                    _actualTime = _timer % Duration;
                    _activated = 0;
                }

                ParticlesUpdate(_actualTime);

                if (Loop)
                {
                    if (Bursts.Count >= 1)
                    {
                        foreach (Burst burst in Bursts)
                        {
                            if ((burst.When > _actualTime - deltaTime/2) && (burst.When < _actualTime + deltaTime/2))
                            {
                                Emmit(burst.HowMany, _actualTime);
                            }
                        }
                    }
                    else
                    {
                        if (_autoTimer >= 1.0f)
                        {
                            int count = Convert.ToInt16(Max / Duration);
                            Emmit(count, _actualTime);
                            _autoTimer = 0.0f;
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
                            if (_autoTimer >= 1.0f)
                            {
                                int count = Convert.ToInt16(Max / Duration);
                                Emmit(count, _timer);
                                _autoTimer = _autoTimer % 1.0f;
                            }
                        }
                    }
                }   
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
                    }
                    else
                    {
                        for(int j = 0; j < 4; j++)
                        {
                            _verts[i * 4 + j].Position += _directions[i] * Speed;
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
                    ParticleSetActive(i,timer);
                    _activated += 1;
                    counter+=1;
                    if(counter == count)
                    {
                        return;
                    }
                }
            }
        }

        private void ParticleSetActive(int index, float timer)
        {
            _verts[index * 4].Position = new Vector3((Size.X / -20), (Size.Y / 20), 0f);
            _verts[index * 4 + 1].Position = new Vector3((Size.X / 20), (Size.Y / 20), 0f);
            _verts[index * 4 + 2].Position = new Vector3((Size.X / -20), (Size.Y / -20), 0f);
            _verts[index * 4 + 3].Position = new Vector3((Size.X / 20), (Size.Y / -20), 0f);

            for (int i = 0; i < 4; i++ )
            {
                _verts[index * 4 + i].Position += gameObject.transform.Position;
            }
            if (DirectionFrom == DirectionTo)
            {
                _directions[index] = DirectionFrom;
            }
            else
            {
                _directions[index] = new Vector3(_random.Next(Convert.ToInt16(DirectionFrom.X * 10), Convert.ToInt16(DirectionTo.X * 10 + 1)),
                    _random.Next(Convert.ToInt16(DirectionFrom.Y * 10), Convert.ToInt16(DirectionTo.Y * 10 + 1)), _random.Next(Convert.ToInt16(DirectionFrom.Z * 10), Convert.ToInt16(DirectionTo.Z * 10 + 1))) * 0.1f;
            }
            _particleTimes[index] = _timer;
            _activationStates[index] = true;
        }

        private void InitializeParticles()
        {
            for(int i = 0 ; i < Max ; i++)
            {   
                int tmp = i*4;
                _verts[tmp] = new VertexPositionTexture(new Vector3((Size.X / -20), (Size.Y / 20), 0f), Vector2.Zero);
                _verts[tmp + 1] = new VertexPositionTexture(new Vector3((Size.X / 20), (Size.Y / 20), 0f), new Vector2(1, 0)); 
                _verts[tmp + 2] = new VertexPositionTexture(new Vector3((Size.X / -20), (Size.Y / -20), 0f), new Vector2(0, 1));
                _verts[tmp + 3] = new VertexPositionTexture(new Vector3((Size.X / 20), (Size.Y / -20), 0f), new Vector2(1, 1));
            }
            _vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice,
                    typeof(VertexPositionTexture), _verts.Length, BufferUsage.None);
        }

        public override void Draw()
        {
            Game.Instance.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            // Only draw if there are live particles
            for (int i = 0; i < Max; i++ )
            {
                if(_activationStates[i])
                {
                    Game.Instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    _material.ShaderEffect.Parameters["CamPos"].SetValue(Camera.MainCamera.transform.Position);
                    _material.ShaderEffect.Parameters["AllowedRotDir"].SetValue(new Vector3(0, 1, 0));
                    _material.ShaderEffect.Parameters["World"].SetValue(_gameObject.transform.World);
                    _material.ShaderEffect.Parameters["View"].SetValue(Camera.MainCamera.ViewMatrix);
                    _material.ShaderEffect.Parameters["Projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
                    _material.ShaderEffect.Parameters["ParticleTexture"].SetValue(_material.Diffuse);
                    // Draw particles
                    foreach (EffectPass pass in _material.ShaderEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        Game.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                        PrimitiveType.TriangleStrip, _verts, i * 4, 2);
                    }
                    Game.Instance.GraphicsDevice.BlendState = BlendState.Opaque;
                }
            }
                
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
            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
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
