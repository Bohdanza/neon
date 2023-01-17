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
    public class Hero:Mob
    {
        protected Gun GunInHand=null;
        public float Speed { get; private set; } = 0.4f;

        public Hero(ContentManager contentManager, float x, float y, WorldChunk worldChunk) 
            : base(contentManager, new Vector2(x, y), new Vector2(0f, 0f),
            3f, 35,
            new List<Tuple<int, int>> { new Tuple<int, int>(0, 0), new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1), new Tuple<int, int>(-1, -1)},
            "hero", worldChunk)
        {
            GunInHand = new Colt(contentManager, new Vector2(x, y), new Vector2(0f, 0f), worldChunk);
        }

        public override void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.W))
                ChangeMovement(0, -Speed);

            if (ks.IsKeyDown(Keys.S))
                ChangeMovement(0, Speed);

            if (ks.IsKeyDown(Keys.A))
                ChangeMovement(-Speed, 0);

            if (ks.IsKeyDown(Keys.D))
                ChangeMovement(Speed, 0);

            var ms = Mouse.GetState();

            if (GunInHand != null)
            {
                Vector2 screen = worldChunk.GetScreenPosition(this);

                float dir = Game1.GetDirection(screen, new Vector2(ms.X, ms.Y));
                dir += (float)Math.PI;

                GunInHand.UpdateInHand(contentManager);
                GunInHand.Rotation = dir;
                GunInHand.Position=Position;

                if (ms.LeftButton == ButtonState.Pressed)
                { 
                    GunInHand.ShootInDirection(contentManager, dir, worldChunk);
                }
            }

            base.Update(contentManager, worldChunk);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, color, depth);

            if (GunInHand != null)
            {
                Texture2D whatToDraw = Texture.GetCurrentFrame();

                GunInHand.Draw(spriteBatch, x, y, color, depth+0.001f);
            }
        }
    }
}