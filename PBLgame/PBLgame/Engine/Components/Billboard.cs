using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public abstract class BillboardBase : Component
    {
        protected MeshMaterial _material;
        protected Vector2 _size;
        protected float _height;

        public virtual MeshMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public abstract bool Triggered { get; set; }
        public abstract float Height { get; set; }
        public abstract Vector2 Size { get; set; }

        protected BillboardBase(Component source) : base(source) { }
        protected BillboardBase(GameObject owner) : base(owner) { }
    }

    public class Billboard : BillboardBase
    {
        private VertexPositionTexture[] _verts;
        private VertexBuffer _vertexBuffer;
        private Effect _effect;
        private float _alpha = 1f;

        public override bool Triggered
        {
            get { return false; }
            set { }
        }

        public override float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                Reinitialize();
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
                Reinitialize();
            }
        }

        public override MeshMaterial Material
        {
            set
            {
                _material = value;
                _effect = _material.ShaderEffect;
            }
        }

        public float Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        public Billboard(GameObject owner, Vector2 size, float height) : base(owner)
        {
            _size = size;
            _height = height;


            _verts = new VertexPositionTexture[4];
            _verts[0].TextureCoordinate = new Vector2(0, 0);
            _verts[1].TextureCoordinate = new Vector2(1, 0);
            _verts[2].TextureCoordinate = new Vector2(0, 1);
            _verts[3].TextureCoordinate = new Vector2(1, 1);

            _vertexBuffer = new VertexBuffer(GlobalInventory.Instance.GraphicsDevice,
                    typeof(VertexPositionTexture), _verts.Length, BufferUsage.None);

            Reinitialize();
        }

        public Billboard(BillboardBase src, GameObject newOwner) : base(newOwner)
        {
        }

        public override Component Copy(GameObject newOwner)
        {
            return new Billboard(this, newOwner);
        }

        public void Reinitialize()
        {
            _verts[0].Position = new Vector3(-_size.X,  _size.Y + _height, 0);
            _verts[1].Position = new Vector3( _size.X,  _size.Y + _height, 0);
            _verts[2].Position = new Vector3(-_size.X, -_size.Y + _height, 0);
            _verts[3].Position = new Vector3( _size.X, -_size.Y + _height, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;
            Reinitialize();

            Vector3 translation = gameObject.transform.World.Translation;

            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Position += translation;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Enabled) return;

            GraphicsDevice graphicsDevice = GlobalInventory.Instance.GraphicsDevice;
            graphicsDevice.SetVertexBuffer(_vertexBuffer);

            graphicsDevice.BlendState = BlendState.AlphaBlend;

            DepthStencilState oldDepthStencil = graphicsDevice.DepthStencilState;
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            _effect.Parameters["CamPos"].SetValue(Camera.MainCamera.transform.Position);
            _effect.Parameters["AllowedRotDir"].SetValue(new Vector3(0, 1, 0));
            _effect.Parameters["Alpha"].SetValue(_alpha);
            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["View"].SetValue(Camera.MainCamera.ViewMatrix);
            _effect.Parameters["Projection"].SetValue(Camera.MainCamera.ProjectionMatrix);
            _effect.Parameters["ParticleTexture"].SetValue(_material.Diffuse);
            foreach (EffectPass pass in _material.ShaderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, _verts, 0, 2);
            }

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = oldDepthStencil;
        }
    }
}