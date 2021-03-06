﻿#region Using Statements
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
    public enum EstadoDeJogo
    {
        Splash,
        Menu,
        Lobby,
        Loading,
        Mapa
    }

    public class HideSeek : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        NetworkSession rede;
        //Personagem jogador;
        Mapa mapa;
        KeyboardState currentState;
        SpriteFont fonte;
        PacketWriter caixaSaida;
        PacketReader caixaEntrada;
        EstadoDeJogo estado_atual = EstadoDeJogo.Splash;
        SplashScreen splashScr;
        Matrix spriteScale;

        public HideSeek ()
            : base()
        {
            graphics = new GraphicsDeviceManager (this);
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";

            caixaSaida = new PacketWriter ();
            caixaEntrada = new PacketReader ();

            splashScr = new SplashScreen ();

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds (0.33f);

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

            fonte = Content.Load<SpriteFont> ("Font");
            splashScr.LoadContent (Content);
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
            switch (estado_atual) {
            case EstadoDeJogo.Splash:
                splashScr.Update (gameTime, ref estado_atual);
                break;
            case EstadoDeJogo.Menu:
                UpdateMenu ();
                break;
            case EstadoDeJogo.Mapa:
                UpdateSessao ();
                break;
            case EstadoDeJogo.Lobby:
                CarregaMapa ();
                break;
            case EstadoDeJogo.Loading:
                InicializaJogadores ();
                break;
            default:
                //Nao deveria chegar aqui, ainda.
                break;
            }

            spriteScale = Matrix.CreateScale ((float)GraphicsDevice.Viewport.Width / 800f, (float)GraphicsDevice.Viewport.Height / 640f, 1f);

            base.Update (gameTime);
        }

        protected void UpdateMenu ()
        {
            currentState = Keyboard.GetState ();
            if (currentState.IsKeyDown (Keys.N)) {
                CriaSessao ();
            } else if (currentState.IsKeyDown (Keys.J)) {
                JoinSessao ();
            }
        }

        protected void CriaSessao ()
        {
            estado_atual = EstadoDeJogo.Loading;
            try {
                rede = NetworkSession.Create (NetworkSessionType.SystemLink, 2, 2);
                estado_atual = EstadoDeJogo.Lobby;
                ManipulaEventos ();
            } catch (Exception ex) {
                Console.WriteLine ("Erro: " + ex.Message);
            }
        }

        protected void JoinSessao ()
        {
            estado_atual = EstadoDeJogo.Loading;
            try {
                using (AvailableNetworkSessionCollection disponiveis = NetworkSession.Find(NetworkSessionType.SystemLink, 2,null)) {
                    if (disponiveis.Count != 0) {
                        rede = NetworkSession.Join (disponiveis [0]);
                        mapa = new Mapa(new Vector2(0f,0f));
                        mapa.Initialize();
                        mapa.LoadContent(Content);
                        estado_atual = EstadoDeJogo.Mapa;
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
                Personagem player = new Personagem (e.Gamer.IsHost);
                player.LoadContent (Content);
                e.Gamer.Tag = player;
            };

            rede.SessionEnded += delegate(object sender, NetworkSessionEndedEventArgs e) {
                rede.Dispose ();
                rede = null;
            };
        }

        void CarregaMapa ()
        {
            foreach (var gamer in rede.LocalGamers) {
                if (gamer.IsHost && null == mapa) {
                    mapa = new Mapa (new Vector2 (0f, 0f));
                    mapa.Initialize ();
                }
                if (null != mapa) {
                    caixaSaida.Write (mapa.Descricao);
                    Console.WriteLine ("Enviei mapa");
                } else
                    caixaSaida.Write ("");
                gamer.SendData (caixaSaida, SendDataOptions.Reliable);
            }

            rede.Update ();
            if (null == rede)
                return;

            String descricaoRemota = "";
            bool ok = true;

            foreach (LocalNetworkGamer gamer in rede.LocalGamers)
                while (gamer.IsDataAvailable) {
                    NetworkGamer remetente;
                    gamer.ReceiveData (caixaEntrada, out remetente);
                    if (remetente.IsLocal)
                        continue;
                    string descricaoAux = caixaEntrada.ReadString ();
                    if (!String.IsNullOrEmpty (descricaoAux))
                        descricaoRemota = descricaoAux;
                    ok = ok && !String.IsNullOrEmpty (descricaoAux);
                }

            if (!String.IsNullOrEmpty (descricaoRemota) && null == mapa) {
                mapa = new Mapa (new Vector2 (0.0f, 0.0f));
                mapa.Initialize (descricaoRemota);
                mapa.LoadContent (Content);
            }

            if (ok && rede.RemoteGamers.Count > 0 && null != mapa)
                estado_atual = EstadoDeJogo.Loading;
        }

        void InicializaJogadores ()
        {
            mapa.LoadContent (Content);

            foreach (LocalNetworkGamer gamer in rede.LocalGamers) {
                Personagem player = gamer.Tag as Personagem;
                player.Initialize (new Vector2 (32.0f, 32.0f));
                player.Mapa = mapa;
            }

            rede.Update ();
            if (null == rede)
                return;


            estado_atual = EstadoDeJogo.Mapa;
        }

        void UpdateSessao ()
        {

            foreach (var gamer in rede.LocalGamers) {
                Personagem player = gamer.Tag as Personagem;
                player.Update (Keyboard.GetState ());
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
                    player.Posicao = caixaEntrada.ReadVector2 ();
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
            spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null, spriteScale);

            switch (estado_atual) {
            case EstadoDeJogo.Splash:
                splashScr.Draw (spriteBatch);
                break;
            case EstadoDeJogo.Menu:
                string mensagem = "N para nova sessao\nJ para juntar uma existente";

                spriteBatch.DrawString (fonte, mensagem, new Vector2 (160f, 160f), Color.Gray);
                spriteBatch.DrawString (fonte, mensagem, new Vector2 (161f, 161f), Color.White);
                break;
            case EstadoDeJogo.Mapa:

                if (mapa != null)
                    mapa.Draw (spriteBatch);

                foreach (NetworkGamer gamer in rede.AllGamers) {
                    Personagem player = gamer.Tag as Personagem;

                    player.Draw (spriteBatch, gameTime);
                }

                break;
            case EstadoDeJogo.Lobby:
                spriteBatch.DrawString (fonte, "Esperando jogadores", new Vector2 (100f, 100f), Color.Gray);
                spriteBatch.DrawString (fonte, "Esperando jogadores", new Vector2 (101f, 101f), Color.White);
                break;
            case EstadoDeJogo.Loading:
                spriteBatch.DrawString (fonte, "Carregando", new Vector2 (100f, 100f), Color.Gray);
                spriteBatch.DrawString (fonte, "Carregando", new Vector2 (101f, 101f), Color.White);
                break;
            default:
                estado_atual = EstadoDeJogo.Menu;
                Console.WriteLine ("Houve um erro bizarro com o EstadodeJogo");
                break;
            }

            spriteBatch.End ();

            base.Draw (gameTime);
        }

        bool Apertou (Keys key)
        {
            return Keyboard.GetState ().IsKeyDown (key);
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
            game.Run ();
        }
    }
}
