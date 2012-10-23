#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

#endregion

namespace HideSeek
{
    public class HideSeek : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        NetworkSession rede;
        Personagem jogador;
        Mapa mapa;
        KeyboardState currentState;
        SpriteFont fonte;

        PacketWriter caixaSaida = new PacketWriter();
        PacketReader caixaEntrada = new PacketReader();
        
        public HideSeek ()
            : base()
        {
            graphics = new GraphicsDeviceManager (this);
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds (0.033);

            new Bloco();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize ()
        {
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

            fonte = Content.Load<SpriteFont>("Font");
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
            if (rede == null)
                UpdateMenu();
            else 
                UpdateSessao();
                
            base.Update(gameTime);
        }

        protected void UpdateMenu ()
        {
            currentState = Keyboard.GetState();
            if (currentState.IsKeyDown (Keys.N))
                    CriaSessao ();
                else if (currentState.IsKeyDown (Keys.J))
                    JoinSessao ();
        }

        protected void CriaSessao ()
        {
            try {
                rede = NetworkSession.Create(NetworkSessionType.SystemLink, 2, 2);
                mapa = new Mapa(new Vector2(0f,0f));
                mapa.Initialize();
                mapa.LoadContent(Content);
                Console.WriteLine("criou");
            } catch (Exception ex) {
                // Faca nada
            }

            ManipulaEventos();
        }

        protected void JoinSessao ()
        {
            try {
                using (AvailableNetworkSessionCollection disponiveis = NetworkSession.Find(NetworkSessionType.SystemLink, 2,null)) {
                    if (disponiveis.Count != 0) {
                        rede = NetworkSession.Join (disponiveis [0]);
                        mapa = new Mapa(new Vector2(0f,0f));
                        mapa.Initialize();
                        mapa.LoadContent(Content);
                        ManipulaEventos ();
                    } else {
                        return;
                    }
                }
            } catch (Exception ex) {
                // Faca nada
            }
        }

        protected void ManipulaEventos ()
        {
            rede.GamerJoined += delegate(object sender, GamerJoinedEventArgs e) {
                Personagem player = new Personagem();
                player.Initialize(new Vector2(32f,32f));
                player.LoadContent(Content);
                if (mapa != null)
                    player.Mapa = mapa;
                e.Gamer.Tag = player;
            };

            rede.SessionEnded += delegate(object sender, NetworkSessionEndedEventArgs e) {
                rede.Dispose();
                rede = null;
            };
        }

        void UpdateSessao ()
        {
            currentState = Keyboard.GetState ();

            foreach (var gamer in rede.LocalGamers) {
                Personagem player = gamer.Tag as Personagem;
                player.Update (Keyboard.GetState (gamer.SignedInGamer.PlayerIndex));

                caixaSaida.Write (player.Mapa.Descricao);
                caixaSaida.Write (player.Posicao);
                caixaSaida.Write (player.PosicaoAlvo);
                gamer.SendData (caixaSaida, SendDataOptions.InOrder);
            }

            rede.Update ();

            if (rede == null)
                return;

            foreach (LocalNetworkGamer gamer in rede.LocalGamers) {
                while (gamer.IsDataAvailable) {
                    NetworkGamer remetente;

                    gamer.ReceiveData (caixaEntrada, out remetente);

                    if (remetente.IsLocal)
                        continue; // Nao quero local

                    Personagem player = remetente.Tag as Personagem;

                        mapa = new Mapa (new Vector2 (0f, 0f));
                        mapa.Initialize (caixaEntrada.ReadString ());
                        mapa.LoadContent (Content);
                        player.Mapa = mapa;
                    player.Posicao = caixaEntrada.ReadVector2();
                    player.PosicaoAlvo = caixaEntrada.ReadVector2 ();
                }
            }

            if (Apertou (Keys.Escape))
                this.Exit ();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear (Color.Black);

            if (rede == null) {

                string mensagem = "N para nova sessao\nJ para juntar uma existente";

                spriteBatch.Begin ();
                spriteBatch.DrawString (fonte, mensagem, new Vector2 (160f, 160f), Color.White);
                spriteBatch.DrawString (fonte, mensagem, new Vector2 (161f, 161f), Color.Gray);
                spriteBatch.End ();
            } else {
                spriteBatch.Begin ();
                if (mapa != null)
                    mapa.Draw (spriteBatch);

                foreach (NetworkGamer gamer in rede.AllGamers) {
                    Personagem player = gamer.Tag as Personagem;

                    player.Draw (spriteBatch, gameTime);
                }

                spriteBatch.End ();
            }


            base.Draw (gameTime);
        }

        bool Apertou (Keys key)
        {
            return currentState.IsKeyDown(key);
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
