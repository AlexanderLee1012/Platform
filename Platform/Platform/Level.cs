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
        private int brickSize = 32;
        private Color[] singleBrick;
        private Color[] beginBrick;
        private Color[] contBrick;
        private Color[] endBrick;
        private Texture2D blank;
        private string[] levelFile;
        private bool[,] collisionCheck;

        public Level(Texture2D _brick, Texture2D _blank, GraphicsDevice graphicsDevice)
        {
            int brick2 = brickSize * brickSize;
            singleBrick = new Color[brick2];
            beginBrick = new Color[brick2];
            contBrick = new Color[brick2];
            endBrick = new Color[brick2];

            _brick.GetData<Color>(0, new Rectangle(0, 0, brickSize, brickSize), singleBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, brickSize, brickSize, brickSize), beginBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 2 * brickSize, brickSize, brickSize), contBrick, 0, brick2);
            _brick.GetData<Color>(0, new Rectangle(0, 3 * brickSize, brickSize, brickSize), endBrick, 0, brick2);

            blank = new Texture2D(graphicsDevice, _blank.Width, _blank.Height);
            Color[] blankColor = new Color[_blank.Height*_blank.Width];
            _blank.GetData<Color>(blankColor);
            blank.SetData<Color>(blankColor) ;
        }

        public void loadCurrentLevel(string FileName)
        {
            levelFile = File.ReadAllLines(FileName);
            collisionCheck = new bool[levelFile[0].Length,levelFile.Length];

            int x, y, lineY, valueX;// 32 by 32
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
            return collisionCheck[hashedX,hashedY];//need to convert 800 by 480 to scale with 25 by 15
        }

        public void draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(blank, new Rectangle(0, 0, GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight), Color.White);
        }
    }
}
