using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameMapEdit.MenuSystem
{
    public class Menu
    {
        public List<MenuItem> Items { get; set; }
        SpriteFont font;
        int currentItem;
        KeyboardState oldState;
        public Menu(SpriteFont font)
        {
            Items = new List<MenuItem>();
            this.font = font;
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter))
            {
                Items[currentItem].OnClick();
            }
            int delta = 0;

            if (state.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
                delta = -1;
            if (state.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
                delta = 1;

            currentItem += delta;
            bool ok = false;
            while (!ok)
            {
                if (currentItem < 0)
                    currentItem = Items.Count - 1;
                else if (currentItem >= Items.Count)
                    currentItem = 0;
                else if (Items[currentItem].Active == false)
                    currentItem += delta;
                else ok = true;
            }

            oldState = state;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int y = 100;
            spriteBatch.Begin();
            foreach (MenuItem item in Items)
            {
                Color color = Color.White;
                if (item.Active == false)
                    color = Color.Gray;
                else if (item == Items[currentItem])
                    color = Color.Green;
                spriteBatch.DrawString(font, item.Name, new Vector2(150, y), color);
                y += 50;
            }
            spriteBatch.End();
        }
    }
}
