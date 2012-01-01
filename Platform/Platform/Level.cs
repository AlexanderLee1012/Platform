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
        private int brickSize = 20;
        private Color[] singleBrick;
        private Color[] beginBrick;
        private Color[] contBrick;
        private Color[] endBrick;
        private Color[] topDoor;
        private Color[] botDoor;
        private Texture2D blank;
        private string[] levelFile;
        private bool[,] collisionCheck;

        int doorX, doorY;
        AnimatedSprite openDoor;

        public Level(Texture2D _brick, Texture2D _blank, GraphicsDevice graphicsDevice)
        {
            int brick2 = brickSize * brickSize;
            singleBrick = new Color[brick2];
            beginBrick = new Color[brick2];
            contBrick = new Color[brick2];
            endBrick = new Color[brick2];
            topDoor = new Color[brick2];
            botDoor = new Color[brick2];

            openDoor = new AnimatedSprite();

            brick = _brick;
            _brick.GetData<Color>(0, new Rectangle(0, 0, brickSize, brickSize), topDoor, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, brickSize, brickSize, brickSize), botDoor, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 2 * brickSize, brickSize, brickSize), singleBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 3 * brickSize, brickSize, brickSize), beginBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 4 * brickSize, brickSize, brickSize), contBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 5 * brickSize, brickSize, brickSize), endBrick, 0, brick2);

            blank = new Texture2D(graphicsDevice, _blank.Width, _blank.Height);
            Color[] blankColor = new Color[_blank.Height*_blank.Width];
            _blank.GetData<Color>(blankColor);
            blank.SetData<Color>(blankColor) ;
        }

        public void loadCurrentLevel(string FileName)
        {
            levelFile = File.ReadAllLines(FileName);
            collisionCheck = new bool[levelFile[0].Length,levelFile.Length];

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
                        collisionCheck[valueX, lineY] = true;
                        blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), contBrick, 0, contBrick.Length);
                    }
                    if (value == '[')
                    {
                        collisionCheck[valueX, lineY] = true;
                        blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), beginBrick, 0, contBrick.Length);
                    }
                    if (value == ']')
                    {
                        collisionCheck[valueX, lineY] = true;
                        blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), endBrick, 0, contBrick.Length);
                    }
                    if (value == '_') 
                    {
                        collisionCheck[valueX, lineY] = true;
                        blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), singleBrick, 0, contBrick.Length);
                    }
                    if (value == 'T')
                    {
                        doorX = x;
                        doorY = y;
                        collisionCheck[valueX, lineY] = true;
                        blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), topDoor, 0, contBrick.Length);
                    }
                    if (value == '|')
                    {
                        collisionCheck[valueX, lineY] = true;
                        blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), botDoor, 0, contBrick.Length);
                        openDoor.LoadSprite(brick, 0, 0, doorX + 16, doorY + 32, 6, brickSize, brickSize * 2, 10);
                    }


                    x += brickSize;
                    valueX++;
                }
                x = 0;
                y += brickSize;
                valueX = 0;
                lineY++;
            }
        }

        public bool checkCollision(int x, int y)
        {
            int hashedX = x / brickSize;
            int hashedY = y / brickSize;
            return collisionCheck[hashedX,hashedY];//need to convert 800 by 480 to scale with 40 by 24
        }

        void setBlank(int x, int y)
        {
            Color[] gray = new Color[brickSize * brickSize];
                for (int i = 0; i < brickSize * brickSize; i++)
                    gray[i] = Color.Gray;

            collisionCheck[x / brickSize, y / brickSize] = false;
            blank.SetData<Color>(0, new Rectangle(x, y, brickSize, brickSize), gray, 0, gray.Length);
        }

        public void openExit(SpriteBatch _spriteBatch, GameTime _gametime)
        {
            setBlank(doorX, doorY);
            setBlank(doorX, doorY + brickSize);
            openDoor.Update(_gametime, doorX + 16, doorY + 32, true);
            openDoor.Draw(_spriteBatch, 1);
        }

        public void draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(blank, new Rectangle(0, 0, GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight), Color.White);
        }
    }
}
