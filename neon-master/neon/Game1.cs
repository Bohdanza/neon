using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace neon
{
    public class Game1 : Game
    {
        public const int PixelScale = 5;
        public static Texture2D NoTexture;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D cursor;
        private World world;
        private FrameCounter _frameCounter = new FrameCounter();
        private SpriteFont mainFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            IsMouseVisible = false;
            _graphics.ApplyChanges();

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            _graphics.ApplyChanges();

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;

            _graphics.ApplyChanges();

            _graphics.IsFullScreen = false;

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Console.WriteLine();

            world = new World(Content, "worlds\\world1");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cursor = Content.Load<Texture2D>("curs");
            NoTexture = Content.Load<Texture2D>("no_texture");
            mainFont = Content.Load<SpriteFont>("File");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            world.Update(Content);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                world.Save();
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var ms = Mouse.GetState();

            _frameCounter.Update(deltaTime);

            GraphicsDevice.Clear(new Color(0, 0, 0));

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);

            world.Draw(_spriteBatch);

            var fps = _frameCounter.AverageFramesPerSecond.ToString();

            _spriteBatch.DrawString(mainFont, fps, new Vector2(1920 - mainFont.MeasureString(fps).X, 1), Color.White, 0f, 
                new Vector2(0, 0), 1f, SpriteEffects.None, 1f);

            _spriteBatch.Draw(cursor, new Vector2(ms.X - cursor.Width / 2, ms.Y - cursor.Height / 2), null,
                Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1f);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public static float GetDistance(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt((v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y));
        }

        public static float GetDistance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
        public static float GetDistance(int x1, int y1, int x2, int y2)
        {
            return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public static float GetDirection(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Atan2(v1.Y - v2.Y, v1.X - v2.X);
        }

        public static Vector2 DirectionToVector(float direction)
        {
            return new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
        }
    }
}
