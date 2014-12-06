using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMapEdit
{
    public class AnimatedSprite
    {
        Texture2D idleTexture;
        Texture2D runTexture;
        Texture2D jumpTexture;

        public Rectangle rect;

        int frameWidth;
        int frameHeight;
        int currentFrame;
        int timeElapsed;
        int timeForFrame = 100;

        bool isRunning = false;
        bool isRunningRight;
        bool isJumping;

        float ySpeed;
        float maxYSpeed = 8;
        float g = 0.2f;

        Game1 game;

        public int FrameCount
        {
            get
            {
                return runTexture.Width / frameWidth;
            }
        }

        public void Run(bool right)
        {
            if (!isRunning)
            {
                isRunning = true;
                currentFrame = 0;
                timeElapsed = 0;
            }
            isRunningRight = right;
        }

        public void Jump()
        {
            if (!isJumping && ySpeed == 0)
            {
                ySpeed = maxYSpeed;
                isJumping = true;
                currentFrame = 0;
                timeElapsed = 0;
            }
        }
        public void Stop()
        {
            isRunning = false;
            currentFrame = 0;
            timeElapsed = 0;
        }
        public AnimatedSprite(Rectangle rect, Texture2D idle, Texture2D run, Texture2D jump, Game1 game)
        {
            jumpTexture = jump;
            this.rect = rect;
            idleTexture = idle;
            runTexture = run;
            this.game = game;

            frameWidth = frameHeight = run.Height;
        }

        public void ApplyGravity(GameTime gameTime)
        {
            ySpeed = ySpeed - g * gameTime.ElapsedGameTime.Milliseconds / 10;
            float dy = ySpeed * gameTime.ElapsedGameTime.Milliseconds / 10;

            Rectangle nextPosition = rect;
            nextPosition.Offset(0, -(int)Math.Floor(dy));
            Rectangle boundingRectangle = GetBoundingRectangle(nextPosition);
            if (nextPosition.Top > 0 && nextPosition.Bottom < game.Height
                && !game.CollidesWithLevel(boundingRectangle))
                rect = nextPosition;

            bool collidesOnFallDown = game.CollidesWithLevel(boundingRectangle) && ySpeed < 0;
            bool collidesOnJumping = game.CollidesWithLevel(boundingRectangle) && ySpeed > 0;
            if (nextPosition.Bottom > game.Height || collidesOnFallDown)
            {
                isJumping = false;
                ySpeed = 0;
            }
            if (nextPosition.Top < 0 || collidesOnJumping)
            {
                ySpeed = 0;
            }

        }
        public void UpdateAI(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (timeElapsed > timeForFrame)
            {
                timeElapsed = 0;
                currentFrame = (++currentFrame) % FrameCount;
            }

            if (isRunning)
            {

                int dx = 3 * gameTime.ElapsedGameTime.Milliseconds / 10;
                if (!isRunningRight)
                    dx = -dx;
                Rectangle nextPosition = rect;
                nextPosition.Offset(dx, 0);

                Rectangle boundingRectangle = GetBoundingRectangle(nextPosition);

                if (boundingRectangle.Left <= 0 || boundingRectangle.Right >= game.levelLength)
                    isRunningRight = !isRunningRight;
                else if (game.CollidesWithLevel(boundingRectangle))
                    isRunningRight = !isRunningRight;
                else if (game.WillFallDown(boundingRectangle))
                    isRunningRight = !isRunningRight;
                else rect = nextPosition;
            }

            ApplyGravity(gameTime);
        }
        public void Update(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (timeElapsed > timeForFrame)
            {
                timeElapsed = 0;
                currentFrame = (++currentFrame) % FrameCount;
            }

            if (isRunning)
            {

                int dx = 3 * gameTime.ElapsedGameTime.Milliseconds / 10;
                if (!isRunningRight)
                    dx = -dx;
                Rectangle nextPosition = rect;
                nextPosition.Offset(dx, 0);

                Rectangle boundingRectangle = GetBoundingRectangle(nextPosition);
                Rectangle screenBoundingRect = game.GetScreenRectangle(boundingRectangle);
                if (screenBoundingRect.Left > 0 && screenBoundingRect.Right < game.Width
                    && !game.CollidesWithLevel(boundingRectangle))
                    rect = nextPosition;
            }

            ApplyGravity(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            Rectangle source = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            SpriteEffects effect = SpriteEffects.None;
            if (!isRunningRight)
                effect = SpriteEffects.FlipHorizontally;

            Rectangle screenRect = game.GetScreenRectangle(rect);

            if (isJumping)
            {
                spriteBatch.Draw(jumpTexture, screenRect, source, Color.White, 0, Vector2.Zero, effect, 0);
            } else 
            if (isRunning)
            {
                spriteBatch.Draw(runTexture, screenRect, source, Color.White, 0, Vector2.Zero, effect, 0);
            }
            else
            {
                spriteBatch.Draw(idleTexture, screenRect, null, Color.White, 0, Vector2.Zero, effect, 0);

            }
            spriteBatch.End();
        }

        public Rectangle GetBoundingRectangle(Rectangle rectangle)
        {
            int width = (int)(rectangle.Width * 0.6f);
            int dx = (int)(rectangle.Width * 0.2f);
            return new Rectangle(rectangle.X + dx, rectangle.Y, width, rectangle.Height);
        }
    }
}
