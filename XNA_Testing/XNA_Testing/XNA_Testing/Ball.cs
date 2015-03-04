using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_Testing
{
    class Ball
    {
        Vector2 motion;
        Vector2 position;
        float ballSpeed = 4;

        Texture2D texture;
        Rectangle screenBounds;

        public Ball(Texture2D texture, Rectangle screenBounds)
        {
            this.texture = texture;
            this.screenBounds = screenBounds;
        }

        public void Update()
        {
            position += motion * ballSpeed;
            CheckWallCollision();
        }

        private void CheckWallCollision()
        {
            if(position.X < 0)
            {
                position.X = 0;
                motion.X *= -1;
            }
            if(position.X + texture.Width > screenBounds.Width)
            {
                position.X = screenBounds.Width - texture.Width;
                motion.X *= -1;
            }
            if(position.Y < 0)
            {
                position.Y = 0;
                motion.Y *= -1;
            }
        }

        public void SetInStartPosition(Rectangle paddleLocation)
        {
            motion = new Vector2(1, -1);
            position.Y = paddleLocation.Y - texture.Width;
            position.X = paddleLocation.X + (paddleLocation.Width - texture.Width) / 2;
        }

        public bool OffBottom()
        {
            if (position.Y > screenBounds.Height) return true;
            else return false;
        }

        public void PaddleCollision(Rectangle paddleLocation)
        {
            Rectangle ballLocation = new Rectangle(
                (int)position.X,
                (int)position.Y,
                texture.Width,
                texture.Height);

            if(paddleLocation.Intersects(ballLocation))
            {
                position.Y = paddleLocation.Y - texture.Height;
                motion.Y *= -1;
            }
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(texture,position,Color.White);
        }
        
    }
}
