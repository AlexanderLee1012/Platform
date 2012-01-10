using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform
{
    class Level
    {
        Texture2D brick;
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

        public Level(Texture2D _brick, Texture2D _blank, GraphicsDevice graphicsDevice, SpriteBatch _spriteBatch)
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
            isDoorOpen = false;
            levelFile = File.ReadAllLines(FileName);
            collisionCheck = new int[levelFile[0].Length,levelFile.Length];
            destroyTheseBricks = new bool[levelFile[0].Length, levelFile.Length];

            destructBrickArr = new AnimatedSprite[levelFile[0].Length, levelFile.Length];

            int x, y, lineY, valueX;// 20 by 20
            x = 0;
            y = 0;
            lineY = 0;
            valueX = 0;

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

        public bool checkCollision(int x, int y)
        {   //need to convert 800 by 480 to scale with 40 by 24
            int hashedX = x / brickSize;
            int hashedY = y / brickSize;
            if (collisionCheck[hashedX, hashedY] == 1)
                return true;
            else if (collisionCheck[hashedX, hashedY] == 2)
            {
                destroyCurrBlock(x, y);
                return true;
            }
            else if (collisionCheck[hashedX, hashedY] == 3)
            {
                setBlank(x, y);
                currentCollectables++;
                return false;
            }
            else
                return false;
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

        public bool animateDestructBrick(GameTime _gt, int playerX, int playerY)
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
                                destroyTheseBricks[x,y] = false;
                                currLvlText.SetData<Color>(0, new Rectangle(x* brickSize, y* brickSize, brickSize, brickSize), destructBrick, 0, contBrick.Length);
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
            return playerDead;
        }

        public void draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(currLvlText, new Rectangle(0, 0, GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight), Color.White);
        }
    }
}
