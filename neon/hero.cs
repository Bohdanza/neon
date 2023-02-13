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
using Newtonsoft.Json;

namespace neon
{
    public class Hero:Mob
    {
        [JsonProperty]
        public Gun GunInHand { get; protected set; } = null;
        [JsonProperty]
        public float Speed { get; private set; } = 0.4f;
        [JsonProperty]
        public float GunRotationSpeed { get; protected set; } = 0.1f;

        public Hero() : base() { }

        public Hero(ContentManager contentManager, float x, float y, World world) 
            : base(contentManager, new Vector2(x, y), new Vector2(0f, 0f),
            3f, 35,
            @"hitboxes\hero.png",
            "hero", world)
        {
           // GunInHand = new Spear(contentManager, new Vector2(x, y-2), Movement, world);
            Action = "wa";
        }

        public override void Update(ContentManager contentManager, World world)
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.W))
            {
               // world.ScreenY--;
                ChangeMovement(0, -Speed);
            }

            if (ks.IsKeyDown(Keys.S))
            {
               // world.ScreenY++;
                ChangeMovement(0, Speed);
            }

            if (ks.IsKeyDown(Keys.A))
            {
                //world.ScreenX--;
                ChangeMovement(-Speed, 0);
            }

            if (ks.IsKeyDown(Keys.D))
            {
                //world.ScreenX++;
                ChangeMovement(Speed, 0);
            }

            var ms = Mouse.GetState();

            if (GunInHand != null)
            {
                Vector2 screen = world.GetScreenPosition(this);

                float dir = Game1.GetDirection(new Vector2(ms.X, ms.Y), 
                    new Vector2(screen.X, screen.Y+Position.Y-GunInHand.Position.Y));
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

                GunInHand.Position=new Vector2(Position.X, Position.Y-5);

                if (ms.LeftButton == ButtonState.Pressed)
                { 
                    GunInHand.ShootInDirection(contentManager, dir, world, this);
                }
            }

            if (Math.Abs(Movement.X) > 0.001 || Math.Abs(Movement.Y) > 0.001)
                Action = "wa";
            else
                Action = "id";

            base.Update(contentManager, world);
        }
        
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, color, depth);

            if (GunInHand != null)
            {
                GunInHand.Draw(spriteBatch, x, y-5*World.UnitSize, color, depth+0.000001f);
            }
        }
    }
}