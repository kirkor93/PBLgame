using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;
using PBLgame.Engine.Singleton;

namespace PBLgame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game Instance { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera mainCamera;

        //For teting-----------------
        public GameObject player;

        //Sounds tetin
        AudioEngine _audioEngine; //Has to be in final version
        WaveBank _waveBank; //Has to be in final version
        SoundBank _soundBank;//Hae to be in final version

        //////////////

        public Effect phEffect;
        
        //------------------------

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mainCamera = new Camera( new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up,
                MathHelper.PiOver4,(float)Window.ClientBounds.Width,(float)Window.ClientBounds.Height,1,100);

            InputManager.Instance.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            _waveBank = new WaveBank(_audioEngine, @"Content\Audio\WaveBank.xwb");
            _soundBank = new SoundBank(_audioEngine, @"Content\Audio\SoundBank.xsb");

            //For Teting-----------------------

            ResourceManager.Instance.LoadContent();
            ResourceManager.Instance.AssignAudioBank(_soundBank);

            Scene scene = new Scene();
            scene.Load("Scene 1.xml");
            player = scene.GameObjects.First();
            player.audioSource.Set3D(mainCamera.audioListener);
            player.audioSource.Play();
            scene.Save("Scene 1.xml");

            
            ResourceManager.Instance.SaveContent();
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            //ForTetting-----------------------
            InputManager.Instance.Update();
            mainCamera.Update(gameTime);

            //-----------------------------


            player.Update();
            player.audioSource.Set3D(mainCamera.audioListener);

            _audioEngine.Update(); //Have to be in final version

            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //For Teting----------------

            player.Draw();

            //---------------------

            base.Draw(gameTime);
        }
    }
}
