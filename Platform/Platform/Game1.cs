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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Platform
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        float scale = 0.39f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboard;
        Vector2 PlayerVector;
        Vector2 PlayerOrigin;

        //Environment Variables---------------------------------------------
        Texture2D brick;
        Texture2D blankLevel;
        int currentLevel;
        int latestLevel;
        private Level lvl;
        //------------------------------------------------------------------


        //Player variables--------------------------------------------------
        Texture2D stand;
        Texture2D duckTexture;
        Texture2D jumpUp;
        Texture2D jumpDown;
        float jumpAm;
        bool move;
        bool faceRight;
        bool duck;
        bool jumpPeak;
        int jump;
        int pHeight;
        int pWidth;
        //------------------------------------------------------------------


        private AnimatedSprite spriteWalk;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            spriteWalk = new AnimatedSprite();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            currentLevel = 1;
            latestLevel = 0;
            jumpPeak = false;
            jumpAm = 0;
            PlayerVector = new Vector2(100, 110);
            faceRight = true;
            jump = 0;
            PlayerOrigin = new Vector2(25, 50);
            pHeight = (int)(scale * 100);
            pWidth = (int)(scale * 50);
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

            spriteWalk.LoadSprite(Content.Load<Texture2D>("Sticks"), 
                (int)PlayerVector.X, (int)PlayerVector.Y,
                0, 0, 6, 50, 100, 15);
            duckTexture = Content.Load<Texture2D>("StickDuck");
            stand = Content.Load<Texture2D>("Stick");
            jumpUp = Content.Load<Texture2D>("StickJumpUp");
            jumpDown = Content.Load<Texture2D>("StickJumpDown");
            
            brick = Content.Load<Texture2D>("brick");
            blankLevel = Content.Load <Texture2D>("blankLevel");

            lvl = new Level(brick, blankLevel, GraphicsDevice);
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

        private void loadLevel(int currentLevel)
        {
            if (latestLevel == currentLevel)
                return;
            else
            {
                lvl.loadCurrentLevel("C:\\Users\\Alexander\\Platform\\Platform\\PlatformContent\\lvl" + currentLevel + ".txt");
                latestLevel = currentLevel;
            }
        }

        //for direction expects to specify whether checking the top, bottom, right or left of the player
        private bool collideSide(int x, int y, int height, int width, string direction, Level lvl) //assuming that the x and y given are in the middle of the side
        {
            if (direction == "top" || direction == "bottom")
            {
                if (direction == "top")
                    y = y - (height / 2);
                else
                    y = y + (height / 2);
                for (int i = (x - (width / 2)); i < (x + (width / 2)); i++)
                {
                    if (lvl.checkCollision(i, y) == true)
                        return true;
                }
                return false;
            }
            else
            {
                if (direction == "right")
                    x = x + (width / 2);
                else
                    x = x - (width / 2);
                for (int i = (y - (height / 2)); i < (y + (height / 2)); i++)
                {
                    if (lvl.checkCollision(x, i) == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            float check = PlayerVector.Y;
            loadLevel(currentLevel);
            duck = false;
            move = false;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float distancePerSecond = 160;
            float delta = distancePerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Down))
            {
                duck = true;
            }

            //jumping logic
            if (!jumpPeak && keyboard.IsKeyDown(Keys.Up)  && jump != 2
                && !(jump == 0 && !collideSide((int)PlayerVector.X, (int)(PlayerVector.Y + delta), pHeight, pWidth, "bottom", lvl)) //used to check if the player tries to jump mid-air from a previous platform
                && jumpPeak == false && !collideSide((int)PlayerVector.X, (int)(PlayerVector.Y - delta), pHeight, pWidth, "top", lvl))
            {
                jump = 1;
                jumpAm += delta;
                PlayerVector.Y -= delta;
                if (jumpAm > 30 * delta)
                    jumpPeak = true;
            }
            else if (jumpAm > 0 && !collideSide((int)PlayerVector.X, (int)(PlayerVector.Y + delta), pHeight, pWidth, "bottom", lvl))
            {
                jump = 2;
                PlayerVector.Y += delta;
                jumpAm -= delta;
                if (jumpAm == 0)
                    jumpPeak = false;
            }
            else
                jump = 0;

            // TODO
            if (!collideSide((int)PlayerVector.X, (int)(PlayerVector.Y + delta), pHeight, pWidth, "bottom", lvl) && jump == 0)
            {
                jump = 2;
                PlayerVector.Y += delta;
            }


            if (!duck && keyboard.IsKeyDown(Keys.Right) && !collideSide((int)(PlayerVector.X + delta), (int)PlayerVector.Y, pHeight, pWidth, "right", lvl))
            {
                faceRight = true;
                move = true;
                PlayerVector.X += delta;
            }

            if (!duck && keyboard.IsKeyDown(Keys.Left) && !collideSide((int)(PlayerVector.X - delta), (int)PlayerVector.Y, pHeight, pWidth, "left", lvl))
            {
                faceRight = false;
                move = true;
                PlayerVector.X -= delta;
            }

            if (!duck && keyboard.IsKeyDown(Keys.Right) && keyboard.IsKeyDown(Keys.Left))
            {
                move = false;
            }

            if (check == PlayerVector.Y)//needed to fix cases when peaked jump is interrupted by environment
            {
                jumpPeak = false;
                jumpAm = 0;
            }

            spriteWalk.Update(gameTime, (int)PlayerVector.X, (int)PlayerVector.Y, false, faceRight);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            lvl.draw(spriteBatch);

            if (jump == 1 && faceRight)
                spriteBatch.Draw(jumpUp, PlayerVector, new Rectangle(0, 0, 50, 100), Color.White, 0,
                    PlayerOrigin, scale, SpriteEffects.None, 0.5f);
            else if (jump == 1 && !faceRight)
                spriteBatch.Draw(jumpUp, PlayerVector, new Rectangle(0, 0, 50, 100), Color.White, 0,
                    PlayerOrigin, scale, SpriteEffects.FlipHorizontally, 0.5f);
            else if (jump == 2 && faceRight)
                spriteBatch.Draw(jumpDown, PlayerVector, new Rectangle(0, 0, 50, 100), Color.White, 0,
                    PlayerOrigin, scale, SpriteEffects.None, 0.5f);
            else if (jump == 2 && !faceRight)
                spriteBatch.Draw(jumpDown, PlayerVector, new Rectangle(0, 0, 50, 100), Color.White, 0,
                    PlayerOrigin, scale, SpriteEffects.FlipHorizontally, 0.5f);
            else if (move)
                spriteWalk.Draw(spriteBatch, scale);
            else if (duck)
                spriteBatch.Draw(duckTexture, PlayerVector, new Rectangle(0, 0, 50, 100), Color.White, 0,
                    PlayerOrigin, scale, SpriteEffects.None, 0.5f);                
            else
                spriteBatch.Draw(stand, PlayerVector, new Rectangle(0, 0, 50, 100), Color.White, 0,
                    PlayerOrigin, scale, SpriteEffects.None, 0.5f);

            //lvl.openExit(spriteBatch, gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
