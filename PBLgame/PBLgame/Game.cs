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
using PBLgame.Engine.GUI;
using PBLgame.Engine.Scenes;
using PBLgame.Engine.Singleton;
using PBLgame.Engine.Physics;
using PBLgame.GamePlay;
using PBLgame.Engine.AI;

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
        private const int ResolutionX = 1280;
        private const int ResolutionY = 720;
        private const bool FullScreenEnabled = false;

//        private const int ResolutionX = 1920;
//        private const int ResolutionY = 1080;
//        private const bool FullScreenEnabled = true;

        //Sounds tetin
        AudioEngine _audioEngine; //Has to be in final version
        WaveBank _waveBank; //Has to be in final version
        SoundBank _soundBank;//Hae to be in final version

        //////////////        
        //------------------------

        public Game()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ResolutionX,
                PreferredBackBufferHeight = ResolutionY,
                IsFullScreen = FullScreenEnabled
            };
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
            mainCamera = new Camera( new Vector3(200, 50, 250), Vector3.Zero, Vector3.Up,
                MathHelper.PiOver4,(float)Window.ClientBounds.Width,(float)Window.ClientBounds.Height,1,1000);

            InputManager.Instance.Initialize();

            base.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            HUD.Instance.Batch = spriteBatch;
            HUD.Instance.CurrentWindowSize = new Vector2(ResolutionX, ResolutionY);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GlobalInventory.Instance.GraphicsDevice = GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.



            _audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            _waveBank = new WaveBank(_audioEngine, @"Content\Audio\WaveBank.xwb");
            _soundBank = new SoundBank(_audioEngine, @"Content\Audio\SoundBank.xsb");

            //For Teting-----------------------

            ResourceManager.Instance.LoadContent();
            ResourceManager.Instance.AssignAudioBank(_soundBank);

            HUD.Instance.Load();

            _scene = new Scene();
            _scene.Load(@"Level_1.xml");

            player = _scene.FindGameObject(8);
            mainCamera.transform.Position = player.transform.Position + new Vector3(0, 100f, 80f);
            mainCamera.SetTarget(player.transform.Position + new Vector3(0,10,0));
            mainCamera.parent = player;

            _scene.FindGameObject(607).AddComponent<EnemyMeleeScript>(new EnemyMeleeScript(_scene.FindGameObject(607)));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
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
            InputManager.Instance.Update(gameTime);
//            mainCamera.Update(gameTime);

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
            //---------------------
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _scene.Draw(gameTime);
            base.Draw(gameTime);
            spriteBatch.Begin();
            HUD.Instance.Draw();
            spriteBatch.End();

        }
    }
}
