#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont spriteFont;
        SpriteFont frameFont;
        ChaseCamera camera; 
        Ship ship;
        Model shipModel; 
        Model groundModel; 
        bool cameraSpringEnabled = true;

        Texture2D logoTex;
        Vector2 logopos;
        #endregion

        #region Initialization

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

           
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            

            ship = new Ship(ScreenManager.GraphicsDevice);
            camera = new ChaseCamera(); 
            camera.DesiredPositionOffset=new Vector3(0.0f,2000.0f,3500.0f);
            camera.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f); 
            camera.NearPlaneDistance = 10.0f; 
            camera.FarPlaneDistance = 100000.0f; 
            camera.AspectRatio = (float)ScreenManager.GraphicsDevice.Viewport.Width / ScreenManager.GraphicsDevice.Viewport.Height;
            UpdateCameraChaseTarget();
            camera.Reset();

            spriteFont = content.Load<SpriteFont>("gamefont");
            frameFont = content.Load<SpriteFont>("FrameFont");
            shipModel = content.Load<Model>("Ship"); 
            groundModel = content.Load<Model>("Ground");
            logoTex = content.Load<Texture2D>("Untitled-2");

            logopos = new Vector2(0, 0);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
            ScreenManager.Game.IsFixedTimeStep = false;
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            elapsedTime += gameTime.ElapsedGameTime;


                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }

            if (IsActive)
            {
                ship.Update(gameTime);
                UpdateCameraChaseTarget();

                if (cameraSpringEnabled)
                {
                    camera.Update(gameTime);
                }
                else
                {
                    camera.Reset();
                }
            }

         


            System.Diagnostics.Debug.WriteLine("Time Elapsed");

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                return;
            }
            else
            {
                
            }

            PlayerIndex dummy;
            if (input.IsNewKeyPress(Keys.R, ControllingPlayer, out dummy) || input.IsNewButtonPress(Buttons.RightStick, ControllingPlayer, out dummy)) 
            {
                cameraSpringEnabled = !cameraSpringEnabled;
            }
            if (input.IsNewKeyPress(Keys.R, ControllingPlayer, out dummy) || input.IsNewButtonPress(Buttons.RightStick, ControllingPlayer, out dummy)) 
            {
                ship.Reset();
                camera.Reset();
            }


            

        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue ba                        ckground. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);
            
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;

            DrawModel(shipModel, ship.World);
            DrawModel(groundModel, Matrix.Identity);
            DrawOverlayText();

            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.DrawString(frameFont, fps, new Vector2(33, 33), Color.Black);
            
            //spriteBatch.DrawString(ScreenManager.Font, "Insert Gameplay Here", new Vector2(100, 100), Color.DarkRed);
            //spriteBatch.Draw(logoTex, new Vector2(0, 0), Color.White);


            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 )
            {
                
                ScreenManager.FadeBackBufferToBlack(1 - TransitionAlpha);
            }
        }
        private void UpdateCameraChaseTarget() 
        { 
            camera.ChasePosition = ship.Position; 
            camera.ChaseDirection = ship.Direction;
            camera.Up = ship.Up;
        } 
        private void DrawModel(Model model, Matrix world) 
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms); 
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects) 
                { 
                    effect.EnableDefaultLighting(); 
                    effect.PreferPerPixelLighting = true; 
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                } 
                mesh.Draw();
            } 
        }
        private void DrawOverlayText() 
        {
            ScreenManager.SpriteBatch.Begin(); 
            string text = "Right Trigger or Spacebar = thrust\n" + "Left Thumb Stick or Arrow keys = steer\n" + "A = toggle camera spring (" + (cameraSpringEnabled ? "on" : "off") + ")";
            ScreenManager.SpriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            ScreenManager.SpriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);
            ScreenManager.SpriteBatch.End(); 
        }
        #endregion
    }
}
