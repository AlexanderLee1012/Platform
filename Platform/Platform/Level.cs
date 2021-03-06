﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform
{
    class Projectile
    {
        Texture2D proj;
        bool right;
        float x;
        float originalX;
        float y;
        float originalY;

        public Projectile(float _x, float _y, bool faceRight, Texture2D projectile)
        {
            x = _x;
            y = _y;
            originalX = x;
            originalY = y;
            right = faceRight;
            proj = projectile;
        }

        public float bulletX() { return x; }
        public float bulletY() { return y; }

        public void bulletCollision()
        {
            x = originalX;
            y = originalY;
        }

        public void updateProjectile()
        {
            if (right)
                x += 3.33f;
            else
                x -= 3.33f;
        }

        public void drawProjectile(SpriteBatch spriteBatch)
        {
            if (right)
                spriteBatch.Draw(proj, new Vector2(x,y), Color.White); 
            else
                spriteBatch.Draw(proj, new Vector2(x, y), null, Color.White, 0, new Vector2(1f, 3/2f), 1f, SpriteEffects.FlipHorizontally, 0); 
        }
    }

    class Level
    {
        Texture2D brick;
        Texture2D badGuys;
        SpriteBatch spriteBatch;
        private int brickSize = 20;
        private Color[] singleBrick;
        private Color[] beginBrick;
        private Color[] contBrick;
        private Color[] endBrick;
        private Color[] topDoor;
        private Color[] botDoor;
        private Color[] destructBrick;
        private Color[] collectable;
        private Color[] blankLevel;
        private Texture2D projectile;
        private Texture2D currLvlText;
        private string[] levelFile;
        private int[,] collisionCheck;
        private bool[,] destroyTheseBricks;
        private bool isDoorOpen;
        private int totalCollectables = 0;
        private int currentCollectables = 0;

        int doorX, doorY;
        AnimatedSprite openDoor;
        AnimatedSprite[,] destructBrickArr;
        AnimatedSprite[,] badShooters;
        Projectile[] projectileArr;

        public Level(Texture2D _brick, Texture2D _badGuys, Texture2D _projectile, Texture2D _blank, GraphicsDevice graphicsDevice, SpriteBatch _spriteBatch)
        {
            spriteBatch = _spriteBatch;

            int brick2 = brickSize * brickSize;

            singleBrick = new Color[brick2];
            beginBrick = new Color[brick2];
            contBrick = new Color[brick2];
            endBrick = new Color[brick2];
            topDoor = new Color[brick2];
            botDoor = new Color[brick2];
            destructBrick = new Color[brick2];
            collectable = new Color[brick2];

            openDoor = new AnimatedSprite();

            projectile = _projectile;
            badGuys = _badGuys;
            brick = _brick;
            _brick.GetData<Color>(0, new Rectangle(0, 0, brickSize, brickSize), topDoor, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, brickSize, brickSize, brickSize), botDoor, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 2 * brickSize, brickSize, brickSize), singleBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 3 * brickSize, brickSize, brickSize), beginBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 4 * brickSize, brickSize, brickSize), contBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 5 * brickSize, brickSize, brickSize), endBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 6 * brickSize, brickSize, brickSize), destructBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 7 * brickSize, brickSize, brickSize), collectable, 0, brick2);

            currLvlText = new Texture2D(graphicsDevice, _blank.Width, _blank.Height);
            blankLevel = new Color[_blank.Height*_blank.Width];
            _blank.GetData<Color>(blankLevel);
            currLvlText.SetData<Color>(blankLevel);
        }

        public Vector2 loadCurrentLevel(string FileName)
        {
            currLvlText.SetData<Color>(blankLevel);

            Vector2 PlayerVector = new Vector2();
            projectileArr = new Projectile[50];//arbitrary value
            isDoorOpen = false;
            levelFile = File.ReadAllLines(FileName);
            collisionCheck = new int[levelFile[0].Length,levelFile.Length];
            destroyTheseBricks = new bool[levelFile[0].Length, levelFile.Length];

            destructBrickArr = new AnimatedSprite[levelFile[0].Length, levelFile.Length];
            badShooters = new AnimatedSprite[levelFile[0].Length, levelFile.Length];

            int x, y, lineY, valueX, currProjectile;// 20 by 20
            x = 0;
            y = 0;
            lineY = 0;
            valueX = 0;
            currProjectile = 0;

            foreach (string line in levelFile)
            {
                foreach (char value in line)
                {
                    if (value == '-')
                    {
                        collisionCheck[valueX, lineY] = 1;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), contBrick, 0, contBrick.Length);
                    }
                    if (value == '[')
                    {
                        collisionCheck[valueX, lineY] = 1;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), beginBrick, 0, contBrick.Length);
                    }
                    if (value == ']')
                    {
                        collisionCheck[valueX, lineY] = 1;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), endBrick, 0, contBrick.Length);
                    }
                    if (value == '_') 
                    {
                        collisionCheck[valueX, lineY] = 1;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), singleBrick, 0, contBrick.Length);
                    }
                    if (value == 'T')
                    {
                        doorX = x;
                        doorY = y;
                        collisionCheck[valueX, lineY] = 1;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), topDoor, 0, contBrick.Length);
                    }
                    if (value == '|')
                    {
                        collisionCheck[valueX, lineY] = 1;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), botDoor, 0, contBrick.Length);
                        openDoor.LoadSprite(brick, x + (brickSize / 2), y, 0, 0, 6, brickSize, brickSize * 2, 10);
                    }
                    if (value == 'X')
                    {
                        collisionCheck[valueX, lineY] = 2;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), destructBrick, 0, contBrick.Length);
                        destructBrickArr[valueX, lineY] = new AnimatedSprite();
                        destructBrickArr[valueX, lineY].LoadSprite(brick, x + (brickSize / 2), y + (brickSize / 2), 0, 6 * brickSize, 3, brickSize, brickSize, 15);
                    }
                    if (value == '>')//shooter facing right
                    {
                        collisionCheck[valueX, lineY] = 4;
                        collisionCheck[valueX, lineY - 1] = 4;
                        badShooters[valueX, lineY] = new AnimatedSprite();
                        badShooters[valueX, lineY].LoadSprite(badGuys, x + brickSize/2, y, 0, 0, 7, 50, 100, 9, true);
                        projectileArr[currProjectile] = new Projectile(x, y, true, projectile);
                        currProjectile++;
                    }
                    if (value == '<')
                    {
                        collisionCheck[valueX, lineY] = 4;
                        collisionCheck[valueX, lineY - 1] = 4;
                        badShooters[valueX, lineY] = new AnimatedSprite();
                        badShooters[valueX, lineY].LoadSprite(badGuys, x + brickSize / 2, y, 0, 0, 7, 50, 100, 9, false);
                        projectileArr[currProjectile] = new Projectile(x, y, false, projectile);
                        currProjectile++;
                    }
                    if (value == 'C')
                    {
                        collisionCheck[valueX, lineY] = 3;
                        totalCollectables++;
                        currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), collectable, 0, contBrick.Length);
                    }
                    if (value == 'O')
                    {
                        PlayerVector.X = (float)x;
                        PlayerVector.Y = (float)y;
                    }


                    x += brickSize;
                    valueX++;
                }
                x = 0;
                y += brickSize;
                valueX = 0;
                lineY++;
            }
            return PlayerVector;
        }

        public bool checkCollision(int x, int y, bool bullet = false)
        {   //need to convert 800 by 480 to scale with 40 by 24
            int hashedX = x / brickSize;
            int hashedY = y / brickSize;
            if (collisionCheck[hashedX, hashedY] == 1)
                return true;
            else if (collisionCheck[hashedX, hashedY] == 2)
            {
                if (bullet == false)
                    destroyCurrBlock(x, y);
                return true;
            }
            else if (collisionCheck[hashedX, hashedY] == 3)
            {
                if (bullet == false)
                {
                    setBlank(x, y);
                    currentCollectables++;
                }
                return false;
            }
            else if (collisionCheck[hashedX, hashedY] == 4)
            {
                //TODO
                return true;
            }
            else
                return false;
        }

        public int bulletCollisionPlayer(int playerX, int playerY, int pHeight, int pWidth, bool duck, Projectile bullet)
        {
            int projX = (int)bullet.bulletX();
            int projY = (int)bullet.bulletY();
            if (checkCollision(projX, projY, true))
            {
                bullet.bulletCollision();
                return 0;
            }
            else if (projX > playerX - pWidth/2 && projX < playerX + pWidth/2)
            {
                if (duck)
                    if (projY < playerY + pHeight / 2 && projY > playerY)
                        return 1;
                    else
                        return 0;
                else
                    if (projY < playerY + pHeight / 2 && projY > playerY - pHeight / 2)
                        return 1;
                    else
                        return 0;                    
            }
            else
                return 0;
        }

        public bool checkWin(int playerX, int playerY)
        {
            Rectangle door = new Rectangle(doorX, doorY, brickSize, brickSize * 2);
            if (door.Contains(playerX, playerY) && isDoorOpen)
                return true;
            else
                return false;
        }

        void setBlank(int x, int y)
        {
            Color[] gray = new Color[brickSize * brickSize];
            for (int i = 0; i < brickSize * brickSize; i++)
                gray[i] = Color.Gray;

            x = x - x % brickSize;
            y = y - y % brickSize;

            collisionCheck[x / brickSize, y / brickSize] = 0;
            currLvlText.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), gray, 0, gray.Length);
        }

        private void destroyCurrBlock(int x, int y)
        {
            destroyTheseBricks[x / brickSize, y / brickSize] = true;
        }

        private void openExit(GameTime _gameTime)
        {
            isDoorOpen = true;
            if (openDoor.Update(_gameTime, -1, -1, 1))
            {
                setBlank(doorX, doorY);
                setBlank(doorX, doorY + brickSize);
            }
            openDoor.Draw(spriteBatch);
        }

        public bool animateLvlElements(GameTime _gt, int playerX, int playerY, int pHeight, int pWidth, bool duck)
        {
            bool playerDead = false;
            for (int y = 0; y < destroyTheseBricks.GetLength(1); y++)
                for (int x = 0; x < destroyTheseBricks.GetLength(0); x++)
                {
                    if (destroyTheseBricks[x, y] == true)
                    {
                        if (destructBrickArr[x, y].Update(_gt, -1, -1, 5))
                        {
                            setBlank(x * brickSize, y * brickSize);
                            if (destructBrickArr[x, y].checkTimer(_gt, 5))
                            {
                                collisionCheck[x, y] = 2;
                                destroyTheseBricks[x, y] = false;
                                currLvlText.SetData<Color>(0, new Rectangle(x * brickSize, y * brickSize, brickSize, brickSize), destructBrick, 0, contBrick.Length);
                                destructBrickArr[x, y].LoadSprite(brick, x * brickSize + (brickSize / 2), y * brickSize + (brickSize / 2), 0, 6 * brickSize, 3, brickSize, brickSize, 15);
                                if (playerX - (x * brickSize) < brickSize && playerX - (x * brickSize) >= 0 && playerY - (y * brickSize) < brickSize && playerY - (y * brickSize) >= 0)
                                    playerDead = true;

                            }
                        }
                        else
                            destructBrickArr[x, y].Draw(spriteBatch);
                    }
                }

            if (currentCollectables == totalCollectables)
                openExit(_gt);

            for (int y = 0; y < badShooters.GetLength(1); y++)
                for (int x = 0; x < badShooters.GetLength(0); x++)
                {
                    if (badShooters[x, y] != null)
                    {
                        badShooters[x, y].Update(_gt, -1, -1, -1);
                        badShooters[x, y].Draw(spriteBatch, .4f);
                    }
                }

            int playerIsDead = 0;
            for (int i = 0; i < 50; i++)
                if (projectileArr[i] != null)
                {
                    playerIsDead += bulletCollisionPlayer(playerX, playerY, pHeight, pWidth, duck, projectileArr[i]);
                    projectileArr[i].updateProjectile();
                    projectileArr[i].drawProjectile(spriteBatch);
                }
            if (playerIsDead > 0)
                playerDead = true;
          
            return playerDead;
        }
        
        public void draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(currLvlText, new Rectangle(0, 0, GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight), Color.White);
        }
    }
}
