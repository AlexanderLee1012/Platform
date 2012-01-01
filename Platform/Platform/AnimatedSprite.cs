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
        private bool faceRight;

        public void LoadSprite(Texture2D _texture2D, int _posX, int _posY, int _frameX, int _frameY, int _numFrames, int _frameWidth, int _frameHeight, int _framesPerSec)
        {
            sticks = _texture2D;
            rectangles = new List<Rectangle>();

            rectangles.Add(new Rectangle(_frameX, _frameY, _frameWidth, _frameHeight));
            rectangles.Add(new Rectangle(_frameX + _frameWidth, _frameY, _frameWidth, _frameHeight));
            rectangles.Add(new Rectangle(_frameX + 2 * _frameWidth, _frameY, _frameWidth, _frameHeight));
            rectangles.Add(new Rectangle(_frameX + 3 * _frameWidth, _frameY, _frameWidth, _frameHeight));
            rectangles.Add(new Rectangle(_frameX + 4 * _frameWidth, _frameY, _frameWidth, _frameHeight));
            rectangles.Add(new Rectangle(_frameX + 5 * _frameWidth, _frameY, _frameWidth, _frameHeight));

            frameCount = _numFrames;
            framesPerSec = _framesPerSec;
            timePerFrame = (float)1 / framesPerSec;
            currentFrame = 0;
            totalElapsed = 0;
            sticksOrigin = new Vector2(_frameWidth / 2, _frameHeight / 2);
            sticksPos = new Vector2(_posX, _posY);

        }

        public void Update(GameTime _gameTime, int _posX, int _posY, bool once = false, bool _faceRight = false)
        {
            float elapsed = (float)_gameTime.ElapsedGameTime.TotalSeconds;
            totalElapsed += elapsed;

            if (totalElapsed > timePerFrame && once)
            {
                if (currentFrame < (frameCount - 1))
                    currentFrame++;
                totalElapsed -= timePerFrame;            
            }

            if (totalElapsed > timePerFrame && !once)
            {
                currentFrame++;
                currentFrame = currentFrame % frameCount;
                totalElapsed -= timePerFrame;
            }

            sticksPos.X = _posX;
            sticksPos.Y = _posY;
            faceRight = _faceRight;
        }

        public void Draw(SpriteBatch _spriteBatch, float scale)
        {
            if(faceRight)
                _spriteBatch.Draw(sticks, sticksPos, rectangles[currentFrame], Color.White, 0, sticksOrigin, scale, SpriteEffects.None, 0.5f);
            else
                _spriteBatch.Draw(sticks, sticksPos, rectangles[currentFrame], Color.White, 0, sticksOrigin, scale, SpriteEffects.FlipHorizontally, 0.5f);
        }

    }
}
