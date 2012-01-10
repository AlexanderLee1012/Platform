using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform
{
    class AnimatedSprite
    {
        private Texture2D sticks;
        private List<Rectangle> rectangles;
        private Vector2 sticksOrigin;
        private Vector2 sticksPos;
        private int frameCount;
        private float timePerFrame;
        private float totalElapsed;
        private int currentFrame;
        private int framesPerSec;
        private int numOftimes;
        private bool faceRight;
        bool timerStart;
        float passedTime;

        public void LoadSprite(Texture2D _texture2D, int _posX, int _posY, int _frameX, int _frameY, int _numFrames, int _frameWidth, int _frameHeight, int _framesPerSec)
        {
            sticks = _texture2D;
            rectangles = new List<Rectangle>();

            for (int i = 0; i < _frameWidth; i++)
            {
                rectangles.Add(new Rectangle(_frameX + i * _frameWidth, _frameY, _frameWidth, _frameHeight));
            }

            frameCount = _numFrames;
            framesPerSec = _framesPerSec;
            timePerFrame = (float)1 / framesPerSec;
            currentFrame = 0;
            totalElapsed = 0;
            numOftimes = 0;
            passedTime = 0;
            sticksOrigin = new Vector2(_frameWidth / 2, _frameHeight / 2);
            sticksPos = new Vector2(_posX, _posY);
            timerStart = false;
        }

        public bool Update(GameTime _gameTime, int _posX, int _posY, int _numTimes, bool _faceRight = false)
        {
            float elapsed = (float)_gameTime.ElapsedGameTime.TotalSeconds;
            totalElapsed += elapsed;

            if (totalElapsed > timePerFrame && _numTimes < 0)
            {
                currentFrame++;
                currentFrame = currentFrame % frameCount;
                totalElapsed = 0;
            }

            if (totalElapsed > timePerFrame && numOftimes < _numTimes)
            {
                if (currentFrame == (frameCount - 1))
                {
                    currentFrame = 0;
                    numOftimes++;
                }
                else
                {
                    currentFrame++;
                    totalElapsed = 0;
                }
            }

            if (totalElapsed > timePerFrame && numOftimes == _numTimes)
            {
                currentFrame = frameCount - 1;
                timerStart = true;
                return true;
            }

            if (_posX >= 0 && _posY >= 0)
            {
                sticksPos.X = _posX;
                sticksPos.Y = _posY;
            }
            faceRight = _faceRight;

            return false;
        }

        
        public bool checkTimer(GameTime _gameTime, float totalAmount)
        {
            if (timerStart)
            {
                float elapsed = (float)_gameTime.ElapsedGameTime.TotalSeconds;
                passedTime += elapsed;
                if (passedTime >= totalAmount)
                {
                    timerStart = false;
                    passedTime = 0;
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch _spriteBatch, float scale = 1)
        {
            if(faceRight)
                _spriteBatch.Draw(sticks, sticksPos, rectangles[currentFrame], Color.White, 0, sticksOrigin, scale, SpriteEffects.None, 0.5f);
            else
                _spriteBatch.Draw(sticks, sticksPos, rectangles[currentFrame], Color.White, 0, sticksOrigin, scale, SpriteEffects.FlipHorizontally, 0.5f);
        }

    }
}
