using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLgame.Engine.Singleton;

namespace PBLgame.GamePlay
{
    public class VideoHandler : ScreenSystem
    {
        public Rectangle VideoRectangle;
        public Video MyVideo;
        public VideoPlayer MyVideoPlayer;

        public event EventHandler<EventArgs> OnVideoFinished;

        public VideoHandler(Rectangle rect, Video vid, VideoPlayer play)
        {
            VideoRectangle = rect;
            MyVideo = vid;
            MyVideoPlayer = play;
            InputManager.Instance.OnButton += OnButtonClick;
        }

        public VideoHandler()
        {
            VideoRectangle = new Rectangle();
            MyVideo = null;
            MyVideoPlayer = new VideoPlayer();
            InputManager.Instance.OnButton += OnButtonClick;
        }

        public void StartVideo()
        {
            MyVideoPlayer.Play(MyVideo);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(MyVideoPlayer.GetTexture(), VideoRectangle, Color.White); 
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (MyVideoPlayer.State == MediaState.Stopped)
            {
                MyVideoPlayer.Stop();
                InputManager.Instance.OnButton -= OnButtonClick;
                OnVideoFinished(this, new EventArgs());
            }
            MyVideoPlayer.Play(MyVideo);
        }


        private void OnButtonClick(object sender, ButtonArgs buttonArgs)
        {
            if (!buttonArgs.IsDown)
            {
                return;
            }

            if(buttonArgs.ButtonName == Buttons.B)
            {
                MyVideoPlayer.Stop();
                InputManager.Instance.OnButton -= OnButtonClick;
                OnVideoFinished(this, new EventArgs());
            }
        }
    }
}
