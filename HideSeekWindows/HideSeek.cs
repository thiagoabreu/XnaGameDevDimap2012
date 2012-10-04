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
		Bloco bloco;


        public HideSeek()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			jogador = new Personagem();
			bloco = new Bloco();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
			jogador.Initialize(new Vector2(5f, 8f));
			bloco.Initialize(true, new Vector2(64f,64f));
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

            // TODO: use this.Content to load your game content here
			jogador.LoadContent(Content,"Seeker");
			bloco.LoadContent(Content,"Wall");
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
        protected override void Update (GameTime gameTime)
		{

			Rectangle jogadorBox = Constantes.BoundingBox (jogador.getPosicao (), jogador.getSprite ());
			Rectangle blocoBox = Constantes.BoundingBox (bloco.getPosicao (), bloco.getSprite ());

			Vector2 novaPosicao = jogador.getPosicao ();

			if (Keyboard.GetState ().IsKeyDown (Keys.Down)) {
				novaPosicao.Y += Constantes.velocidadeSeeker;
				jogadorBox.Y = (int)novaPosicao.Y;

				if (jogadorBox.Intersects (blocoBox)) {
					novaPosicao.Y = blocoBox.Y - jogadorBox.Height;
				}

			} else if (Keyboard.GetState ().IsKeyDown (Keys.Up)) {
				novaPosicao.Y -= Constantes.velocidadeSeeker;
				jogadorBox.Y = (int)novaPosicao.Y;
				
				if (jogadorBox.Intersects (blocoBox)) {
					novaPosicao.Y = blocoBox.Y + jogadorBox.Height;
				}

			} else if (Keyboard.GetState ().IsKeyDown (Keys.Left)) {
				novaPosicao.X -= Constantes.velocidadeSeeker;
				jogadorBox.X = (int)novaPosicao.X;
				
				if (jogadorBox.Intersects (blocoBox)) {
					novaPosicao.X = blocoBox.X + blocoBox.Width;
				}

			} else  if (Keyboard.GetState ().IsKeyDown (Keys.Right)) {
				novaPosicao.X += Constantes.velocidadeSeeker;
				jogadorBox.X = (int)novaPosicao.X;
				
				if (jogadorBox.Intersects (blocoBox)) {
					novaPosicao.X = blocoBox.X - jogadorBox.Width;
				}
			}
				jogador.Update(novaPosicao);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin();
			bloco.Draw(spriteBatch);
			jogador.Draw(spriteBatch);
			spriteBatch.End();

            base.Draw(gameTime);
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
		static void Main()
		{
			game = new HideSeek();
			game.Run();
		}
	}
}
