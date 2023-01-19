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
        public float GunRotationSpeed { get; protected set; } = 0.1f;

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

                float dir = Game1.GetDirection(new Vector2(ms.X, ms.Y), screen);
                dir += (float)Math.PI * 2;
                dir %= (float)(Math.PI * 2);

                GunInHand.UpdateInHand(contentManager);

                if (GunInHand.Rotation < 0f)
                    GunInHand.Rotation += (float)Math.PI*2;
                else
                    GunInHand.Rotation %= (float)Math.PI*2;

                if (dir != GunInHand.Rotation)
                {
                    if ((dir > GunInHand.Rotation
                        && dir - GunInHand.Rotation < GunInHand.Rotation + Math.PI * 2 - dir) ||
                        (dir < GunInHand.Rotation
                        && dir + Math.PI * 2 - GunInHand.Rotation < GunInHand.Rotation - dir))
                    {
                        if (Math.Abs(dir - GunInHand.Rotation) > GunRotationSpeed)
                            GunInHand.Rotation += GunRotationSpeed;
                        else
                            GunInHand.Rotation = dir;
                    }
                    else
                    {
                        if (Math.Abs(dir - GunInHand.Rotation) > GunRotationSpeed)
                            GunInHand.Rotation -= GunRotationSpeed;
                        else
                            GunInHand.Rotation = dir;
                    }
                }

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

                GunInHand.Draw(spriteBatch, x, y, color, depth+0.000001f);
            }
        }
    }
}