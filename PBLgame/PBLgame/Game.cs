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

        Camera mainCamera;

        //For teting-----------------
        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;

        BasicEffect effect;

        Matrix worldT = Matrix.Identity;
        Matrix worldR = Matrix.Identity;


        private Mesh mesh;

        public GameObject player;
        
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


        public void TriangleTranslate(Object o, MoveArgs e)
        {
            e.AxisValue *= 0.01f;
            worldT *= Matrix.CreateTranslation(e.AxisValue.X, e.AxisValue.Y, 0.0f);
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

            //Camera Movement
            //InputManager.Instance.OnMove += mainCamera.EventMove;

            ResourceManager.Instance.LoadContent();
            mesh = ResourceManager.Instance.GetMesh(@"Models\Helmet");

            player = new GameObject();
            player.AddComponent<GamePlay.PlayerScript>(new GamePlay.PlayerScript(player));
            player.AddComponent<Renderer>(new Renderer(player));
            player.renderer.MyMesh = mesh;
            player.renderer.MyMesh.AssignRenderer(player.renderer);
            player.GetComponent<GamePlay.PlayerScript>().Initialize();


            //InputManager.Instance.OnMove += TriangleTranslate;

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

            //For Teting-----------------------
            verts = new VertexPositionColor[3];
            verts[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
            verts[1] = new VertexPositionColor(new Vector3(1, -1, 0), Color.Red);
            verts[2] = new VertexPositionColor(new Vector3(-1, -1, 0), Color.Green);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            effect = new BasicEffect(GraphicsDevice);

            GameObject obj = new GameObject();
            obj.transform.Translate(new Vector3(2.0f, 2.0f, 1.0f));
            Transform t = obj.GetComponent<Transform>();
            t.Translate(new Vector3(0.0f, 10.0f, 0.0f));
            //------------------------

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

            // Rotation
            worldR *= Matrix.CreateFromYawPitchRoll(
            MathHelper.PiOver4 / 60,
            0,
            0);
            //Translate
            KeyboardState keyboardState = Keyboard.GetState( );
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                worldT *= Matrix.CreateTranslation(-0.01f, 0.0f, 0.0f);
                //mainCamera.Transform.Translate(-0.01f, 0.0f, 0.0f);
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                worldT *= Matrix.CreateTranslation(0.01f, 0, 0);
                //mainCamera.Transform.Translate(0.01f, 0.0f, 0.0f);
            }
            //-----------------------------


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
            //mesh.Draw();
            player.GetComponent<Renderer>().Draw();
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            effect.World = worldR * worldT;
            effect.View = mainCamera.ViewMatrix;
            effect.Projection = mainCamera.ProjectionMatrix;
            effect.VertexColorEnabled = true;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, verts, 0, 1);
            }
            //---------------------

            base.Draw(gameTime);
        }
    }
}
