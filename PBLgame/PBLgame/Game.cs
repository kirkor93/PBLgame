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
using PBLgame.Engine;
using PBLgame.Engine.Components;
using PBLgame.Engine.GameObjects;
using PBLgame.Engine.Scenes;
using PBLgame.Engine.Singleton;
using PBLgame.GamePlay;

namespace PBLgame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game Instance { get; private set; }

        public GameTime Time { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera mainCamera;

        //For teting-----------------
        public GameObject player;
        public GameObject totalyTmp;
        private Scene _scene;

        //Sounds tetin
        AudioEngine _audioEngine; //Has to be in final version
        WaveBank _waveBank; //Has to be in final version
        SoundBank _soundBank;//Hae to be in final version

        //////////////        
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
            mainCamera = new Camera( new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up,
                MathHelper.PiOver4,(float)Window.ClientBounds.Width,(float)Window.ClientBounds.Height,1,1000);

            InputManager.Instance.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GlobalInventory.Instance.GraphicsDevice = GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            _waveBank = new WaveBank(_audioEngine, @"Content\Audio\WaveBank.xwb");
            _soundBank = new SoundBank(_audioEngine, @"Content\Audio\SoundBank.xsb");

            //For Teting-----------------------

            ResourceManager.Instance.LoadContent();
            ResourceManager.Instance.AssignAudioBank(_soundBank);

            _scene = new Scene();
            _scene.Load(@"Scene 1.xml");
            player = _scene.GameObjects.First();
            player.audioSource.Set3D(mainCamera.audioListener);
            player.audioSource.Play();
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
            Time = gameTime;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            //ForTetting-----------------------
            InputManager.Instance.Update();
            mainCamera.Update(gameTime);

            //-----------------------------


            _scene.Update(gameTime);
            _audioEngine.Update(); //Have to be in final version
            
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
            _scene.Draw(gameTime);

            //---------------------

            base.Draw(gameTime);
        }
    }
}
