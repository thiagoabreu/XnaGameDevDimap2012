#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

#endregion

namespace HideSeek
{
    public class HideSeek : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Personagem jogador;
        Mapa mapa;
        
        public HideSeek ()
            : base()
        {
            graphics = new GraphicsDeviceManager (this);
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds (0.033);

            jogador = new Personagem ();
            //  bloco = new Bloco();

            mapa = new Mapa (new Vector2 (0f, 0f));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize ()
        {
            jogador.Initialize (new Vector2 (32f, 32f));
            mapa.Initialize ();
            base.Initialize ();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent ()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch (GraphicsDevice);
                        
            jogador.LoadContent (Content);
            mapa.LoadContent (Content);
                    }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent ()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update (GameTime gameTime)
        {

            KeyboardState currentState = Keyboard.GetState();

            jogador.Update(Keyboard.GetState(), mapa);

            if (currentState.IsKeyDown(Keys.Escape))
                this.Exit();
                
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin();
            mapa.Draw(spriteBatch);
            jogador.Draw(spriteBatch, gameTime);
            spriteBatch.End();
            base.Draw (gameTime);
        }
    }

    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static HideSeek game;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main ()
        {
            game = new HideSeek ();
            game.Run();
        }
    }
}
