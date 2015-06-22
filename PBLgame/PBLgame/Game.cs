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
        private Scene _loadedScene;
        private Scene _activeScene;
        private ScreenSystem _activeScreenSystem;
        private HUD _hud;
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
        private string _windowTitle;
        private int _frames;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

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
            _frames = 0;
            _windowTitle = this.Window.Title;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GlobalInventory.Instance.GraphicsDevice = GraphicsDevice;

            _audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            _waveBank = new WaveBank(_audioEngine, @"Content\Audio\WaveBank.xwb");
            _soundBank = new SoundBank(_audioEngine, @"Content\Audio\SoundBank.xsb");

            //For Teting-----------------------

            ResourceManager.Instance.LoadContent();
            ResourceManager.Instance.AssignAudioBank(_soundBank);

            _hud = new HUD();
            _hud.Load();
            _hud.CurrentWindowSize = new Vector2(ResolutionX, ResolutionY);
            Intro intro = new Intro();
            intro.OnIntroFinished += OnIntroFinished;
            _activeScreenSystem = intro;
            intro.Load();
            intro.CurrentWindowSize = new Vector2(ResolutionX, ResolutionY);
            intro.Start();
            _activeScene = new DummyScene();
            _loadedScene = new Scene();
            _loadedScene.Load(@"Level_1.xml");
            //_scene.Load(@"AnimScene.xml");

            //sooooo hardCode
            //LOOK AT THIS BEAUTY!!!
            int playerMeshId = ResourceManager.Instance.GetMesh(@"Models\Champions\Ace").Id;
            int smartDroidMeshId = ResourceManager.Instance.GetMesh(@"Models\Enemies\Smart_Droid").Id;
            int mechaRangerMeshId = ResourceManager.Instance.GetMesh(@"Models\Enemies\Mecha_Ranger").Id;
            int nJChuckMeshId = ResourceManager.Instance.GetMesh(@"Models\Enemies\NJ").Id;
            
            foreach (GameObject gameObject in _loadedScene.GameObjects.Where(gameObject => gameObject.renderer != null && gameObject.renderer.MyMesh != null))
            {
                if(gameObject.renderer.MyMesh.Id == playerMeshId)
                {
                    gameObject.AddComponent( new AttachSlot(gameObject, gameObject.GetChild("Sword"), "miecz123") );
                }
                else if (gameObject.renderer.MyMesh.Id == smartDroidMeshId)
                {
                    gameObject.AddComponent(new AttachSlot(gameObject, gameObject.GetChild("Katana"), "katana"));
                }
                else if (gameObject.renderer.MyMesh.Id == mechaRangerMeshId)
                {
                    gameObject.AddComponent(new AttachSlot(gameObject, gameObject.GetChild("Crossbow"), "kuszaMR"));
                }
                else if (gameObject.renderer.MyMesh.Id == nJChuckMeshId)
                {
                    gameObject.AddComponent(new AttachSlot(gameObject, gameObject.GetChild("Chainsword"), "spalinowy"));
                }
            }

            player = _loadedScene.FindGameObject(8);

            mainCamera.transform.Position = player.transform.Position + new Vector3(0, 100f, 80f);
            mainCamera.SetTarget(player.transform.Position + new Vector3(0,10,0));
            mainCamera.parent = player;

            //Shieet xD so sweeet cause GO cant find another GO xD totaly fucked up hard coding  
            player.GetComponent<PlayerScript>().ShieldParticle = _loadedScene.FindGameObject(968).GetComponent<ParticleSystem>();
            _loadedScene.FindGameObject(968).GetComponent<ParticleSystem>().Static = false;

            player.GetComponent<PlayerScript>().BananaAttack = _loadedScene.FindGameObject(969).GetComponent<BananaScript>();
            _loadedScene.FindGameObject(969).GetComponent<BananaScript>().Player = player;
  
            //_loadedScene.Save(@"Level_1.xml");
            //OnIntroFinished(null, null);
        }

        private void OnIntroFinished(object sender, EventArgs eventArgs)
        {
            _activeScene = _loadedScene;
            _activeScreenSystem = _hud;
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
            
            InputManager.Instance.Update(gameTime);

            _activeScene.Update(gameTime);
            _activeScreenSystem.Update(gameTime);
            

            _audioEngine.Update(); //Have to be in final version

            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                Window.Title = string.Format("{0}: {1} fps", _windowTitle, _frames);
                _frames = 0;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _frames++;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //For Teting----------------
            //---------------------
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _activeScene.Draw(gameTime);

            base.Draw(gameTime);
            spriteBatch.Begin();

            _activeScreenSystem.Draw(spriteBatch);

            spriteBatch.End();

        }
    }
}
