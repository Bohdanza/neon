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
        public static Texture2D NoTexture;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D texture;
        private WorldChunk worldChunk;
        private FrameCounter _frameCounter = new FrameCounter();
        private SpriteFont mainFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
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

            worldChunk = new WorldChunk(Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("pxl");
            NoTexture = Content.Load<Texture2D>("no_texture");
            mainFont = Content.Load<SpriteFont>("File");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            worldChunk.Update(Content);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            GraphicsDevice.Clear(new Color(5, 5, 6));

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            worldChunk.Draw(_spriteBatch);

            var fps = _frameCounter.AverageFramesPerSecond.ToString();

            _spriteBatch.DrawString(mainFont, fps, new Vector2(1920 - mainFont.MeasureString(fps).X, 1), Color.White);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
