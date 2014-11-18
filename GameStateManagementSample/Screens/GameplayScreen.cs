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
using Microsoft.Xna.Framework.Audio;
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
        SpriteFont teleFont;
        Texture2D TGrassBlock;
        Texture2D[] TBall = new Texture2D[4];
        Texture2D TBaseBall;
        Texture2D TBallPop;
        Texture2D TLadyBall;
        Texture2D GameBack;
        Texture2D EnemyNeedle;

        Random random = new Random();

        Vector2[] GroundBlockPos =  new Vector2[30];
        Vector2[] BackgroundPos = new Vector2[3];
        Vector2 Minus1Pos;
        Rectangle[] NeedlePos = new Rectangle[5];

        Vector2 BlockSpawnLocation;
        Rectangle PlayerBallPos;
        Vector2 LadyBallPos;

        string LivesText = "Lives || ";

        int GameSpeed;
        float BallMovementSpeed;
        int jumpCount = 0;
        int jumpCounter = 0;
        float soundCounter = 0;
        int LivesCounter = 3;
        int HitTimeout = 0;
        bool hitCheck = false;
        bool isSound;
        bool gameWin = false;
        float gameCounter;
        float fadeCounter;

        int WidthRatio,HeightRatio;
        float rotationValue = 0.0f;
        string time;
        bool gameReady = false;
        float deltaTime;

        float fpsCounter;
        string fpsString;
        string objectiveString = "SURVIVE LIFE FOR 2 MINUTES";

        int ScreenWidth;
        int ScreenHeight;

        bool isGameOver = false;

        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;

        Cue MusicCue;

        #endregion

        #region Initialization


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

            isSound = ScreenManager.IsSound;
            
            spriteFont = content.Load<SpriteFont>("gamefont");
            teleFont = content.Load<SpriteFont>("TeleSpriteFont");
            TGrassBlock = content.Load<Texture2D>("Grass-Block");
            TBaseBall = content.Load<Texture2D>("Ball");
            TBallPop = content.Load<Texture2D>("BallPop");
            GameBack = content.Load<Texture2D>("GameBackground");
            EnemyNeedle = content.Load<Texture2D>("Needle");
            TLadyBall = content.Load<Texture2D>("Girl_Ball");


            TBall[0] = TBaseBall;
            TBall[1] = TBaseBall;
            TBall[2] = TBaseBall;
            TBall[3] = TBaseBall;

            if (isSound)
            {
                audioEngine = new AudioEngine("Content\\LifeOfBalls.xgs");
                waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
                soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");
                MusicCue = soundBank.GetCue("Music");
                MusicCue.Play();
            }
            

            ScreenWidth = ScreenManager.WindowRect.Right;
            ScreenHeight = ScreenManager.WindowRect.Bottom;

            BackgroundPos[0].X = 0;
            BackgroundPos[0].Y = 0;
            BackgroundPos[1].X = ScreenWidth;
            BackgroundPos[1].Y = 0;
            BackgroundPos[2].X = ScreenWidth*2;
            BackgroundPos[2].Y = 0;

            NeedlePos[0].Y = (ScreenHeight - TGrassBlock.Height) / 5;
            NeedlePos[1].Y = ((ScreenHeight - TGrassBlock.Height) / 5)*2;
            NeedlePos[2].Y = ((ScreenHeight - TGrassBlock.Height) / 5)*3;
            NeedlePos[3].Y = ((ScreenHeight - TGrassBlock.Height) / 5)*4;
            NeedlePos[4].Y = ((ScreenHeight - TGrassBlock.Height) / 5) * 5;

            fadeCounter = 0.0f;

            for (int i = 0; i < NeedlePos.Length; i++) 
            {
                NeedlePos[i].X = ScreenWidth * (random.Next(2, 6));
                NeedlePos[i].Width = EnemyNeedle.Width;
                NeedlePos[i].Height = EnemyNeedle.Height;
            }

                BlockSpawnLocation = new Vector2(ScreenWidth, ScreenHeight);
            for (int i = 0; i < GroundBlockPos.Length; i++) 
            {
                GroundBlockPos[i].X = BlockSpawnLocation.X;
                GroundBlockPos[i].Y = BlockSpawnLocation.Y - 100;
            }
            WidthRatio = (int)/*(Math.Truncate(*/(float)(ScreenWidth / 8.53);//);
            HeightRatio = (int)/*((Math.Truncate(*/(float)(ScreenHeight/4.8);//);
            PlayerBallPos = new Rectangle(WidthRatio * 2, HeightRatio * 2,TBall[0].Width/2,TBall[0].Height/2);
            GameSpeed = 5;
            BallMovementSpeed = 5;

            Minus1Pos = new Vector2(ScreenWidth / 12 + 50, ScreenHeight / 10 + 30);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            

                if (IsActive)
                {
                    if (!gameWin) 
                    {
                        MoveGround();
                    }
                    if (GroundBlockPos[0].X < 0)
                    {
                        gameReady = true;
                    }
                    if (gameReady)
                    {
                        gameCounter += 1 * deltaTime;
                        time = "" + gameCounter;
                        time = String.Format("{0:0.00}", gameCounter);
                        fpsCounter = 1.0f / deltaTime;
                        fpsString = String.Format("{0:0.00}", fpsCounter);

           
                        fadeCounter += 1 * deltaTime;
                        if (isGameOver)
                        {
                            ScreenManager.AddScreen(new GameOverScreen(), PlayerIndex.One);
                            ExitScreen();
                        }
                        
                        
                        PlayerBallPos.Y += (int)BallMovementSpeed;

                        if ((PlayerBallPos.Y + TBall[0].Height / 4) >= GroundBlockPos[0].Y)
                        {
                            BallMovementSpeed = 0;
                            jumpCount = 0;
                            jumpCounter = 0;
                        }
                        else
                        {
                            BallMovementSpeed += 0.2f;
                        }

                        if (isSound)
                        {
                            if (isGameOver)
                            {
                                MusicCue.Stop(AudioStopOptions.Immediate);
                            }

                            if (!gameWin)
                            {
                                if (soundCounter >= 5)
                                {
                                    soundBank.GetCue("Ambience").Play();
                                    soundCounter = 0;
                                }
                            }
                            soundCounter += 1 * deltaTime;
                        }

                        if (gameCounter < 12)
                        {
                            for (int i = 0; i < BackgroundPos.Length; i++)
                            {
                                BackgroundPos[i].X--;
                                if (BackgroundPos[i].X <= -ScreenWidth)
                                {
                                    BackgroundPos[i].X = ScreenWidth;
                                }
                            }
                            rotationValue += 2.0f * deltaTime;
                            for (int i = 0; i < NeedlePos.Length; i++)
                            {
                                NeedlePos[i].X -= 10;
                                if (NeedlePos[i].X <= 0 - EnemyNeedle.Width)
                                {
                                    NeedlePos[i].X = ScreenWidth * random.Next(1, 6);
                                }
                                if (NeedlePos[i].Intersects(PlayerBallPos))
                                {
                                    if (!hitCheck)
                                    {
                                        PlayerHit();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!gameWin) 
                            {
                                soundBank.GetCue("Win").Play();
                                MusicCue.Stop(AudioStopOptions.Immediate);
                                gameWin = true;
                                LadyBallPos = new Vector2(ScreenWidth*2f,ScreenHeight-(TGrassBlock.Height+TLadyBall.Height));
                            }
                            
                            for (int i = 0; i < NeedlePos.Length; i++) 
                            {
                                NeedlePos[i].X -= 50;
                            }
                            if (LadyBallPos.X >= PlayerBallPos.X + TBaseBall.Width)
                            {
                                LadyBallPos.X -= 10;
                                MoveGround();
                                rotationValue += 2.0f * deltaTime;
                                for (int i = 0; i < BackgroundPos.Length; i++)
                                {
                                    BackgroundPos[i].X--;
                                    if (BackgroundPos[i].X <= -ScreenWidth)
                                    {
                                        BackgroundPos[i].X = ScreenWidth;
                                    }
                                }
                            }
                            else 
                            {
                                
                            }
                        }



                        if (!hitCheck)
                        {
                            if (PlayerBallPos.Y <= -10)
                            {
                                PlayerHit();
                                HitTimeout = -30;
                            }
                        }
                        if (hitCheck)
                        {
                            HitTimeout++;
                            Minus1Pos.Y++;
                        }
                        if (HitTimeout >= 100)
                        {
                            hitCheck = false;
                            HitTimeout = 0;
                            Minus1Pos.X = ScreenWidth / 12 + 50;
                            Minus1Pos.Y = ScreenHeight / 10 + 30;
                        }
                        /*if (BallMovementSpeed != 5) 
                        {
                            BallMovementSpeed += 1 *(int)deltaTime;
                        }*/
                        if (isSound)
                        {
                            audioEngine.Update();
                        }
                    }
                }
        }

        void PlayerHit()
        {
            TBall[LivesCounter] = TBallPop;
            LivesCounter--;
            hitCheck = true;
            if (isSound)
            {
                soundBank.GetCue("Hit").Play();
            }
            if (LivesCounter == 0)
            {
                if (isSound)
                {
                    soundBank.GetCue("PopSound").Play();
                }
                isGameOver = true;
            }
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
                MusicCue.Pause();
                soundBank.GetCue("Ambience").Pause();
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                return;
            }
            else if (IsActive) 
            {
                if (MusicCue.IsPaused) 
                {
                    soundBank.GetCue("Ambience").Resume();
                    MusicCue.Resume();
                }
            }


            if ((PlayerBallPos.Y + TBall[0].Height / 2) >= GroundBlockPos[0].Y)
            {
                if (!gameWin)
                {
                    jumpCounter++;
                    if (jumpCount == 0 && keyboardState.IsKeyDown(Keys.W))
                    {
                        BallMovementSpeed -= 10;
                        jumpCount++;
                    }
                }
            }
            else 
            {
                if (!gameWin)
                {
                    jumpCounter++;
                    if (keyboardState.IsKeyDown(Keys.S))
                    {
                        BallMovementSpeed += 2;
                    }
                }
            }
            if (!gameWin)
            {
                if (jumpCount == 1 && keyboardState.IsKeyDown(Keys.W))
                {
                    if (jumpCounter >= 8)
                    {
                        BallMovementSpeed -= 10;
                        jumpCount++;
                    }
                }
            }
            
        }
        public void MoveGround() 
        {
            for (int i = 0; i < GroundBlockPos.Length; i++)
            {
                if (i == 0)
                {
                    GroundBlockPos[i].X -= GameSpeed;
                }
                else if (GroundBlockPos[i].X != BlockSpawnLocation.X)
                {
                    GroundBlockPos[i].X -= GameSpeed;
                }
                else if (GroundBlockPos[i - 1].X < ScreenWidth - 100)
                {
                    GroundBlockPos[i].X -= GameSpeed;
                }
                if (GroundBlockPos[i].X < -100)
                {
                    GroundBlockPos[i].X = BlockSpawnLocation.X;
                }
            }
            
        }
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (gameReady)
                ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                                   Color.CornflowerBlue, 0, 0);
            else 
            {
                ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                                   Color.Black, 0, 0);
            }            
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;

            

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            if (fadeCounter <= 5)
            {
                spriteBatch.DrawString(teleFont, objectiveString, new Vector2(((ScreenWidth / 2) - teleFont.MeasureString(objectiveString).X / 2), ScreenHeight / 4), Color.White);
            }
            if (gameReady)
            {
                for (int i = 0; i < BackgroundPos.Length; i++)
                {
                    spriteBatch.Draw(GameBack, new Rectangle((int)BackgroundPos[i].X, (int)BackgroundPos[i].Y, ScreenWidth, ScreenHeight), Color.White);
                }
                spriteBatch.Draw(TBall[0], new Vector2(PlayerBallPos.X, PlayerBallPos.Y), null,
                                Color.White, rotationValue, new Vector2(TBall[0].Width / 2, TBall[0].Height / 2), 0.5f, SpriteEffects.None, 0);
                for (int i = 0; i < GroundBlockPos.Length; i++)
                {
                    //Added Math.Truncate with its screen ratios to make the textures scale appropriately to the game screen
                    spriteBatch.Draw(TGrassBlock, new Rectangle((int)GroundBlockPos[i].X, (int)GroundBlockPos[i].Y,WidthRatio,HeightRatio) , Color.White);
                }
                for (int i = 0; i < NeedlePos.Length; i++) 
                {
                    spriteBatch.Draw(EnemyNeedle, new Rectangle((int)NeedlePos[i].X, (int)NeedlePos[i].Y, EnemyNeedle.Width, EnemyNeedle.Height), Color.White);
                }
                
                spriteBatch.DrawString(spriteFont, LivesText , new Vector2(ScreenWidth / 12, ScreenHeight / 10), Color.White);
                spriteBatch.Draw(TBall[3], new Vector2((ScreenWidth / 12) + spriteFont.MeasureString(LivesText).X, ScreenHeight / 10), null, Color.White, 0.0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0);
                spriteBatch.Draw(TBall[2], new Vector2((ScreenWidth / 12) + spriteFont.MeasureString(LivesText).X + 30, ScreenHeight / 10), null, Color.White, 0.0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0);
                spriteBatch.Draw(TBall[1], new Vector2((ScreenWidth / 12) + spriteFont.MeasureString(LivesText).X + 60, ScreenHeight / 10), null, Color.White, 0.0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont,"Your Time:"+ time, new Vector2((ScreenWidth / 12) * 9, ScreenHeight / 10), Color.White);
                spriteBatch.DrawString(spriteFont, fpsString, new Vector2(300, 300), Color.White);
                if (hitCheck) 
                {
                    spriteBatch.DrawString(spriteFont, "-1", Minus1Pos, Color.White);
                }
                if (gameWin) 
                {
                    spriteBatch.Draw(TLadyBall,LadyBallPos,Color.White);
                }
            }
            //spriteBatch.DrawString(ScreenManager.Font, "Insert Gameplay Here", new Vector2(100, 100), Color.DarkRed);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 )
            {
                ScreenManager.FadeBackBufferToBlack(1 - TransitionAlpha);
            }
        }


        #endregion
    }
}
