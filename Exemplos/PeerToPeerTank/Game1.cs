#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

#endregion

namespace PeerToPeerTank
{
	    /// <summary>
    /// Sample showing how to implement a simple multiplayer
    /// network session, using a peer-to-peer network topology.
    /// </summary>
    public class PeerToPeerGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        const int screenWidth = 1067;
        const int screenHeight = 600;

        const int maxGamers = 16;
        const int maxLocalGamers = 4;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        KeyboardState currentKeyboardState;
        GamePadState currentGamePadState;

        NetworkSession networkSession;

        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();
        
        string errorMessage;

        #endregion

        #region Initialization


        public PeerToPeerGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

            Content.RootDirectory = "Content";

            Components.Add(new GamerServicesComponent(this));
        }


        /// <summary>
        /// Load your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            font = Content.Load<SpriteFont>("Font");
        }


        #endregion

        #region Update


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            if (networkSession == null)
            {
                // If we are not in a network session, update the
                // menu screen that will let us create or join one.
                UpdateMenuScreen();
            }
            else
            {
                // If we are in a network session, update it.
                UpdateNetworkSession();
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// Menu screen provides options to create or join network sessions.
        /// </summary>
        void UpdateMenuScreen()
        {
            if (IsActive)
            {
                if (Gamer.SignedInGamers.Count == 0)
                {
                    // If there are no profiles signed in, we cannot proceed.
                    // Show the Guide so the user can sign in.
                    Guide.ShowSignIn(maxLocalGamers, false);
                }
                else if (IsPressed(Keys.A, Buttons.A))
                {
                    // Create a new session?
                    CreateSession();
                }
                else if (IsPressed(Keys.B, Buttons.B))
                {
                    // Join an existing session?
                    JoinSession();
                }
            }
        }


        /// <summary>
        /// Starts hosting a new network session.
        /// </summary>
        void CreateSession()
        {
            DrawMessage("Creating session...");

            try
            {
                networkSession = NetworkSession.Create(NetworkSessionType.SystemLink,
                                                       maxLocalGamers, maxGamers);

                HookSessionEvents();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }


        /// <summary>
        /// Joins an existing network session.
        /// </summary>
        void JoinSession()
        {
            DrawMessage("Joining session...");

            try
            {
                // Search for sessions.
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(NetworkSessionType.SystemLink,
                                                maxLocalGamers, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        errorMessage = "No network sessions found.";
                        return;
                    }

                    // Join the first session we found.
                    networkSession = NetworkSession.Join(availableSessions[0]);

                    HookSessionEvents();
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }


        /// <summary>
        /// After creating or joining a network session, we must subscribe to
        /// some events so we will be notified when the session changes state.
        /// </summary>
        void HookSessionEvents()
        {
            networkSession.GamerJoined += GamerJoinedEventHandler;
            networkSession.SessionEnded += SessionEndedEventHandler;
        }


        /// <summary>
        /// This event handler will be called whenever a new gamer joins the session.
        /// We use it to allocate a Tank object, and associate it with the new gamer.
        /// </summary>
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            int gamerIndex = networkSession.AllGamers.IndexOf(e.Gamer);

            e.Gamer.Tag = new Tank(gamerIndex, Content, screenWidth, screenHeight);
        }


        /// <summary>
        /// Event handler notifies us when the network session has ended.
        /// </summary>
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            errorMessage = e.EndReason.ToString();

            networkSession.Dispose();
            networkSession = null;
        }


        /// <summary>
        /// Updates the state of the network session, moving the tanks
        /// around and synchronizing their state over the network.
        /// </summary>
        void UpdateNetworkSession()
        {
            // Update our locally controlled tanks, and send their
            // latest position data to everyone in the session.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                UpdateLocalGamer(gamer);
            }

            // Pump the underlying session object.
            networkSession.Update();

            // Make sure the session has not ended.
            if (networkSession == null)
                return;

            // Read any packets telling us the positions of remotely controlled tanks.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                ReadIncomingPackets(gamer);
            }
        }


        /// <summary>
        /// Helper for updating a locally controlled gamer.
        /// </summary>
        void UpdateLocalGamer(LocalNetworkGamer gamer)
        {
            // Look up what tank is associated with this local player.
            Tank localTank = gamer.Tag as Tank;

            // Update the tank.
            ReadTankInputs(localTank, gamer.SignedInGamer.PlayerIndex);

            localTank.Update();

            // Write the tank state into a network packet.
            packetWriter.Write(localTank.Position);
            packetWriter.Write(localTank.TankRotation);
            packetWriter.Write(localTank.TurretRotation);

            // Send the data to everyone in the session.
            gamer.SendData(packetWriter, SendDataOptions.InOrder);
        }


        /// <summary>
        /// Helper for reading incoming network packets.
        /// </summary>
        void ReadIncomingPackets(LocalNetworkGamer gamer)
        {
            // Keep reading as long as incoming packets are available.
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;

                // Read a single packet from the network.
                gamer.ReceiveData(packetReader, out sender);

                // Discard packets sent by local gamers: we already know their state!
                if (sender.IsLocal)
                    continue;

                // Look up the tank associated with whoever sent this packet.
                Tank remoteTank = sender.Tag as Tank;

                // Read the state of this tank from the network packet.
                remoteTank.Position = packetReader.ReadVector2();
                remoteTank.TankRotation = packetReader.ReadSingle();
                remoteTank.TurretRotation = packetReader.ReadSingle();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (networkSession == null)
            {
                // If we are not in a network session, draw the
                // menu screen that will let us create or join one.
                DrawMenuScreen();
            }
            else
            {
                // If we are in a network session, draw it.
                DrawNetworkSession();
            }

            base.Draw(gameTime);
        }


        /// <summary>
        /// Draws the startup screen used to create and join network sessions.
        /// </summary>
        void DrawMenuScreen()
        {
            string message = string.Empty;

            if (!string.IsNullOrEmpty(errorMessage))
                message += "Error:\n" + errorMessage.Replace(". ", ".\n") + "\n\n";

            message += "A = create session\n" +
                       "B = join session";

            spriteBatch.Begin();

            spriteBatch.DrawString(font, message, new Vector2(161, 161), Color.Black);
            spriteBatch.DrawString(font, message, new Vector2(160, 160), Color.White);
            
            spriteBatch.End();
        }


        /// <summary>
        /// Draws the state of an active network session.
        /// </summary>
        void DrawNetworkSession()
        {
            spriteBatch.Begin();

            // For each person in the session...
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Look up the tank object belonging to this network gamer.
                Tank tank = gamer.Tag as Tank;

                // Draw the tank.
                tank.Draw(spriteBatch);

                // Draw a gamertag label.
                string label = gamer.Gamertag;
                Color labelColor = Color.Black;
                Vector2 labelOffset = new Vector2(100, 150);

                if (gamer.IsHost)
                    label += " (host)";

                // Flash the gamertag to yellow when the player is talking.
                if (gamer.IsTalking)
                    labelColor = Color.Yellow;

                spriteBatch.DrawString(font, label, tank.Position, labelColor, 0,
                                       labelOffset, 0.6f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }


        /// <summary>
        /// Helper draws notification messages before calling blocking network methods.
        /// </summary>
        void DrawMessage(string message)
        {
            if (!BeginDraw())
                return;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, message, new Vector2(161, 161), Color.Black);
            spriteBatch.DrawString(font, message, new Vector2(160, 160), Color.White);

            spriteBatch.End();

            EndDraw();
        }

        
        #endregion

        #region Handle Input


        /// <summary>
        /// Handles input.
        /// </summary>
        private void HandleInput()
        {
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (IsActive && IsPressed(Keys.Escape, Buttons.Back))
            {
                Exit();
            }
        }


        /// <summary>
        /// Checks if the specified button is pressed on either keyboard or gamepad.
        /// </summary>
        bool IsPressed(Keys key, Buttons button)
        {
            return (currentKeyboardState.IsKeyDown(key) ||
                    currentGamePadState.IsButtonDown(button));
        }


        /// <summary>
        /// Reads input data from keyboard and gamepad, and stores
        /// it into the specified tank object.
        /// </summary>
        void ReadTankInputs(Tank tank, PlayerIndex playerIndex)
        {
            // Read the gamepad.
            GamePadState gamePad = GamePad.GetState(playerIndex);

            Vector2 tankInput = gamePad.ThumbSticks.Left;
            Vector2 turretInput = gamePad.ThumbSticks.Right;

            // Read the keyboard.
            KeyboardState keyboard = Keyboard.GetState(playerIndex);

            if (keyboard.IsKeyDown(Keys.Left))
                tankInput.X = -1;
            else if (keyboard.IsKeyDown(Keys.Right))
                tankInput.X = 1;

            if (keyboard.IsKeyDown(Keys.Up))
                tankInput.Y = 1;
            else if (keyboard.IsKeyDown(Keys.Down))
                tankInput.Y = -1;

            if (keyboard.IsKeyDown(Keys.A))
                turretInput.X = -1;
            else if (keyboard.IsKeyDown(Keys.D))
                turretInput.X = 1;

            if (keyboard.IsKeyDown(Keys.W))
                turretInput.Y = 1;
            else if (keyboard.IsKeyDown(Keys.S))
                turretInput.Y = -1;

            // Normalize the input vectors.
            if (tankInput.Length() > 1)
                tankInput.Normalize();

            if (turretInput.Length() > 1)
                turretInput.Normalize();

            // Store these input values into the tank object.
            tank.TankInput = tankInput;
            tank.TurretInput = turretInput;
        }


        #endregion
    }

}

