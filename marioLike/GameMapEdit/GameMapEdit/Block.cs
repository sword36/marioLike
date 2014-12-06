using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMapEdit
{
    public class Block
    {
        public Rectangle Rect { get; set;}

        Texture2D texture;
        Game1 game;
        public Block(Rectangle rect, Texture2D text, Game1 game)
        {
            this.Rect = rect;
            texture = text;
            this.game = game;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle screenRect = game.GetScreenRectangle(Rect);
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }
}
