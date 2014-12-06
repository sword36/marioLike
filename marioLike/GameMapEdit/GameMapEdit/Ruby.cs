using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMapEdit
{
    public class Ruby
    {
        public Rectangle Rect { get; set;}
        int dy;
        Texture2D texture;
        Game1 game;
        public Ruby(Rectangle rect, Texture2D text, Game1 game)
        {
            this.Rect = rect;
            texture = text;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            float t = (float)(gameTime.TotalGameTime.TotalMilliseconds / 150 + Rect.X);
            dy = (int)(Math.Sin(t) * 3);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle r = Rect;
            r.Offset(10, dy + 10);
            Rectangle screenRect = game.GetScreenRectangle(r);
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }
}