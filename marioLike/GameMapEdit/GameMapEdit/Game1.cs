using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using GameMapEdit.MenuSystem;

namespace GameMapEdit
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Menu menu;
        GameState gameState = GameState.Menu;

        Texture2D block1Texture, block2Texture, grassTexture;
        List<Block> blocks;
        List<Ruby> rubys;
        List<AnimatedSprite> enemys;
        KeyboardState oldState;

        int currentLevel = 1;
        int maxLevel = 3;

        int scores;

        AnimatedSprite hero;
        Texture2D runTexture;
        Texture2D idleTexture;
        Texture2D jumpTexture;
        Texture2D rubyTexture;
        Texture2D runEnemyTexture;
        Texture2D idleEnemyTexture;

        SpriteFont font;

        SoundEffect music;
        SoundEffect sound;
        SoundEffectInstance musicInst;

        public int Width;
        public int Height;

        int scrollX;
        public int levelLength;

        byte[,] levelMap;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Width = this.graphics.PreferredBackBufferWidth = 400;
            Height = this.graphics.PreferredBackBufferHeight = 400;
        }

        Rectangle rect1 = new Rectangle(100, 300, 50, 50);

        public void Scroll(int dx)
        {
            if (scrollX + dx > 0 && scrollX + dx < levelLength - 400)
                scrollX += dx;
        }

        public Rectangle GetScreenRectangle(Rectangle rect)
        {
            Rectangle r = rect;
            r.Offset(-scrollX, 0);
            return r;
        }
        
        public bool WillFallDown(Rectangle rect)
        {
            Rectangle r = rect;
            r.Offset(0, 5);
            if (!CollidesWithLevel(r))
                return true;
            return false;
        }


        public bool CollidesWithLevel(Rectangle rect)
        {
            /*foreach (Block block in blocks)
            {
                if (block.Rect.Intersects(rect))
                    return true;
            }
            return false;*/
            int minX = rect.Left / 40;
            int maxX = rect.Right / 40;
            int minY = rect.Top / 40;
            int maxY = rect.Bottom / 40;
            for (int j = minY; j <= maxY; j ++)
            {
                for (int i = minX; i <= maxX; i++)
                    if (levelMap[j, i] == 1)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            font = Content.Load<SpriteFont>("gameFont");
            menu = new Menu(font);
            MenuItem startGame = new MenuItem("Start Game");
            MenuItem resume = new MenuItem("Resume");
            MenuItem exit = new MenuItem("Exit");
            resume.Active = false;

            startGame.Click += startGame_Click;
            resume.Click += resume_Click;
            exit.Click += exit_Click;

            menu.Items.Add(startGame);
            menu.Items.Add(resume);
            menu.Items.Add(exit);

            base.Initialize();
        }

        void exit_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        void resume_Click(object sender, EventArgs e)
        {
            gameState = GameState.Game;
        }

        void startGame_Click(object sender, EventArgs e)
        {
            gameState = GameState.Game;
            menu.Items[1].Active = true;

            hero = new AnimatedSprite(rect1, idleTexture, runTexture, jumpTexture, this);
            CreateLevel();
            scores = 0;
        }
        void CreateLevel()
        {
            scrollX = 0;
            blocks = new List<Block>();
            rubys = new List<Ruby>();
            enemys = new List<AnimatedSprite>();

            string[] lines = File.ReadAllLines("content/levels/level" + currentLevel + ".txt");

            levelLength = 40 * lines[0].Length;
            levelMap = new byte[lines.Length, lines[0].Length];

            int x = 0;
            int y = 0;

            int i = 0, j = 0;
            foreach (string line in lines)
            {
                foreach (char c in line)
                {
                    if (c == 'X')
                    {
                        Rectangle rect = new Rectangle(x, y, 40, 40);
                        Block block = new Block(rect, grassTexture, this);
                        blocks.Add(block);
                        levelMap[j, i] = 1;
                    }
                    else if (c == 'Y')
                    {
                        Rectangle rect = new Rectangle(x, y, 40, 40);
                        Block block = new Block(rect, block1Texture, this);
                        blocks.Add(block);
                        levelMap[j, i] = 1;
                    }
                    else if (c == 'Z')
                    {
                        Rectangle rect = new Rectangle(x, y, 40, 40);
                        Block block = new Block(rect, block2Texture, this);
                        blocks.Add(block);
                        levelMap[j, i] = 1;
                    }
                    else if (c == 'R')
                    {
                        Rectangle rubyRect = new Rectangle(x, y, 20, 20);
                        Ruby ruby = new Ruby(rubyRect, rubyTexture, this);
                        rubys.Add(ruby);
                    }
                    else if (c == 'E')
                    {
                        Rectangle enemyRec = new Rectangle(x, y - 25, 60, 60);
                        AnimatedSprite enemy = new AnimatedSprite(enemyRec, idleEnemyTexture, runEnemyTexture, runEnemyTexture, this);
                        enemys.Add(enemy);
                        enemy.Run(false);
                    }
                    x += 40;
                    i++;
                }
                i = 0;
                j++;
                x = 0;
                y += 40;
            }
            hero.rect = rect1;


            if (musicInst != null)
                musicInst.Stop(true);
            musicInst = music.CreateInstance();
            musicInst.IsLooped = true;
            musicInst.Play();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            block1Texture = Content.Load<Texture2D>("textures/block1");
            block2Texture = Content.Load<Texture2D>("textures/block2");
            grassTexture = Content.Load<Texture2D>("textures/grass");
            runTexture = Content.Load<Texture2D>("textures/smurf_run");
            idleTexture = Content.Load<Texture2D>("textures/smurf");
            jumpTexture = Content.Load<Texture2D>("textures/smurf_run");
            rubyTexture = Content.Load<Texture2D>("textures/ruby");
            runEnemyTexture = Content.Load<Texture2D>("textures/enemy/enemyRun");
            idleEnemyTexture = Content.Load<Texture2D>("textures/enemy/enemyText");

            sound = Content.Load<SoundEffect>("sounds/scoreSound");
            music = Content.Load<SoundEffect>("sounds/themeSong");
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.Game)
                UpdateGameLogic(gameTime);
            else
                menu.Update();
            base.Update(gameTime);
        }

        private void UpdateGameLogic(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
                gameState = GameState.Menu;

            if (state.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space) && state.IsKeyDown(Keys.LeftControl))
            {
                currentLevel++;
                if (currentLevel > maxLevel)
                    currentLevel = 1;
                CreateLevel();
            }


            if (state.IsKeyDown(Keys.Left))
                hero.Run(false);
            if (oldState.IsKeyDown(Keys.Left) && state.IsKeyUp(Keys.Left))
                hero.Stop();
            if (state.IsKeyDown(Keys.Right))
                hero.Run(true);
            if (oldState.IsKeyDown(Keys.Right) && state.IsKeyUp(Keys.Right))
                hero.Stop();
            if (state.IsKeyDown(Keys.Up))
                hero.Jump();
            hero.Update(gameTime);

            foreach (Ruby ruby in rubys)
            {
                ruby.Update(gameTime);
            }
            foreach (AnimatedSprite enemy in enemys)
            {
                enemy.UpdateAI(gameTime);
            }

            Rectangle boundingHero = hero.GetBoundingRectangle(hero.rect);
            int i = 0;
            while (i < rubys.Count)
            {
                if (rubys[i].Rect.Intersects(boundingHero))
                {
                    sound.Play();
                    rubys.RemoveAt(i);
                    scores += 10;
                }
                else
                    i++;
            }
            if (rubys.Count < 1)
            {
                currentLevel++;
                if (currentLevel == 4)
                    scores = 9000;
                if (currentLevel > maxLevel)
                    currentLevel = 1;
                CreateLevel();
            }

            foreach (AnimatedSprite enemy in enemys)
            {
                Rectangle en = enemy.GetBoundingRectangle(enemy.rect);
                if (en.Intersects(boundingHero))
                {
                    scores = 0;
                    CreateLevel();
                }
            }

            Rectangle heroScreenRectangle = GetScreenRectangle(hero.rect);
            if (heroScreenRectangle.Left > Width / 2)
                Scroll(3 * gameTime.ElapsedGameTime.Milliseconds / 10);
            if (heroScreenRectangle.Right < Width / 2)
                Scroll(-3 * gameTime.ElapsedGameTime.Milliseconds / 10);

            oldState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState == GameState.Game)
                DrawGame();
            else
                menu.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        private void DrawGame()
        {
            spriteBatch.Begin();
            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
            }
            foreach (Ruby ruby in rubys)
            {
                ruby.Draw(spriteBatch);
            }
            spriteBatch.End();

            hero.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Your scores is: " + scores, Vector2.Zero, Color.White);
            spriteBatch.End();

            foreach (AnimatedSprite enemy in enemys)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
    enum GameState
    {
        Game, 
        Menu
    }
}
