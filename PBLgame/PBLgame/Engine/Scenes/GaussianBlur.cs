using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLgame.Engine.Scenes
{
    public class GaussianBlur
    {
        private readonly Effect _effect;
        /// <summary>
        /// Radius of blur. Change it equally in GaussianBlur.fx shader.
        /// </summary>
        private const int RADIUS = 10;
        private float _amount;
        private float _sigma;
        private float[] _kernel;
        private Vector2[] _offsetsHoriz;
        private Vector2[] _offsetsVert;

        public GaussianBlur(Effect effect, float amount, int width, int height)
        {
            _effect = effect;
            _amount = amount;

            _kernel = new float[RADIUS * 2 + 1];
            _sigma = RADIUS / amount;

            float twoSigmaSquare = 2.0f * _sigma * _sigma;
            float sigmaRoot = (float) Math.Sqrt(twoSigmaSquare * Math.PI);
            float total = 0.0f;

            //float[] weights = { 0.1f, 0.2f, 0.55f, 0.8f, 0.9f, 0.95f, 0.98f };
            //for (int i = 0; i < RADIUS; i++)
            //{
            //    _kernel[i] = weights[i];
            //    _kernel[_kernel.Length - i - 1] = weights[i];
            //    total += _kernel[i] + _kernel[_kernel.Length - i - 1];
            //}
            //_kernel[RADIUS] = 1.0f;

            for (int i = 0; i < _kernel.Length; i++)
            {
                int x = i - RADIUS;
                float distance = x * x;
                _kernel[i] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += _kernel[i];
            }
            total /= 1.1f;
            for (int i = 0; i < _kernel.Length; i++)
            {
                _kernel[i] /= total;
            }


             float xOffset = 1.0f / width;
             float yOffset = 1.0f / height;
             _offsetsHoriz = new Vector2[RADIUS * 2 + 1];
             _offsetsVert = new Vector2[RADIUS * 2 + 1];

             for (int i = 0; i < _kernel.Length; ++i)
             {
                 int w = i - RADIUS;
                 _offsetsHoriz[i] = new Vector2(w * xOffset, 0.0f);
                 _offsetsVert[i]  = new Vector2(0.0f, w * yOffset);
             }
        }

        public void Perform(Texture2D source, GraphicsDevice graphics, SpriteBatch spriteBatch, RenderTarget2D tmpTarget, RenderTarget2D output)
        {
            Rectangle tmpRect = new Rectangle(0, 0, tmpTarget.Width, tmpTarget.Height);
            Rectangle outRect = new Rectangle(0, 0, output.Width, output.Height);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, tmpTarget.Width, tmpTarget.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            _effect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            // Horizontal blur
            graphics.SetRenderTarget(tmpTarget);

            _effect.CurrentTechnique = _effect.Techniques["GaussianBlur"];
            _effect.Parameters["weights"].SetValue(_kernel);
            _effect.Parameters["colorMapTexture"].SetValue(source);
            _effect.Parameters["offsets"].SetValue(_offsetsHoriz);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            spriteBatch.Draw(source, tmpRect, Color.Black);
            spriteBatch.End();

            // Vertical blur
            graphics.SetRenderTarget(output);

            projection = Matrix.CreateOrthographicOffCenter(0, output.Width, output.Height, 0, 0, 1);
            _effect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            
            _effect.Parameters["colorMapTexture"].SetValue(tmpTarget);
            _effect.Parameters["offsets"].SetValue(_offsetsVert);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _effect);
            spriteBatch.Draw(tmpTarget, outRect, Color.Black);
            spriteBatch.End();

            graphics.SetRenderTarget(null);
        }
    }
}