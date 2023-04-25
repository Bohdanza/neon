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
        private const float GunOffsetY = 1f;

        [JsonProperty]
        public Gun GunInHand { get; protected set; } = null;
        [JsonProperty]
        public float Speed { get; private set; } = 0.65f;
        [JsonProperty]
        public float GunRotationSpeed { get; protected set; } = 0.1f;
        private Texture2D HpDisplay=null;
        private int CurrentHPDraw = 0;

        public Hero() : base() { }
    
        public Hero(ContentManager contentManager, float x, float y, World world) 
            : base(contentManager, new Vector2(x, y), new Vector2(0f, 0f),
            3f, 600, 100,
            @"hitboxes\hero",
            "hero", world)
        {
            GunInHand = new Biowand(contentManager, new Vector2(x, y-GunOffsetY), new Vector2(0,0), world);
            Action = "wa";
        }   

        public override void Update(ContentManager contentManager, World world)
        {
            if (CurrentHPDraw != HP)
                CurrentHPDraw += (HP - CurrentHPDraw) / Math.Abs(HP - CurrentHPDraw) *
                    Math.Min(3, Math.Abs(HP - CurrentHPDraw));

            if(HpDisplay==null)
            {
                HpDisplay = contentManager.Load<Texture2D>("hpfield");
            }

            if (Action != "die")
            {
                var ms = Mouse.GetState();

                float dir = Game1.GetDirection(world.GetScreenPosition(this),
                    new Vector2(ms.X, ms.Y)) + (float)Math.PI;

                Vector2 vct = Game1.DirectionToVector(dir);

                if (Math.Abs(Movement.X) < 0.00001f)
                    SetMovement(vct.X * 0.00001f, Movement.Y);

                if (ms.RightButton==ButtonState.Pressed)
                {
                    ChangeMovement(vct.X * Speed, vct.Y * Speed);
                }

                if (GunInHand != null)
                {
                    GunInHand.Position = new Vector2(Position.X, Position.Y - GunOffsetY);
                    Vector2 screen = world.GetScreenPosition(GunInHand);

                    dir = Game1.GetDirection(new Vector2(ms.X, ms.Y),
                        new Vector2(screen.X, screen.Y));

                    GunInHand.UpdateInHand(contentManager);

                    GunInHand.Rotation = dir;

                    if (GunInHand.Rotation < 0f)
                        GunInHand.Rotation += (float)Math.PI * 2;
                    else
                        GunInHand.Rotation %= (float)Math.PI * 2;

                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        GunInHand.ShootInDirection(contentManager, dir, world, this);
                    }
                }

                if (Math.Abs(Movement.X) > 0.001 || Math.Abs(Movement.Y) > 0.001)
                    Action = "wa";
                else
                    Action = "id";
            }

            base.Update(contentManager, world);
        }
        
        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, color, depth);

            if (GunInHand != null)
            {
                GunInHand.Draw(spriteBatch, x, (int)(y-GunOffsetY*World.UnitSize), color, depth+0.000001f);
            }

            spriteBatch.Draw(HpDisplay, new Vector2(10, 10), null, color, 0f, new Vector2(0, 0),
                new Vector2(CurrentHPDraw, Game1.PixelScale*2), SpriteEffects.None, 1f);
        }
        
        public override void Damage(int damage, Vector2 direction, World world, ContentManager contentManager)
        {
            base.Damage(damage, direction, world, contentManager);

            if (damage>=0 && 
                (Action != "die" || Texture.CurrentTexture != Texture.Textures.Count - 1 || !Texture.BaseName.EndsWith("die_")))
            {
                var rnd = new Random();
                AddBlood(new Color(232 + rnd.Next(-50, 20), 11, 0),
                     Math.Min(50, rnd.Next(damage / 4, (int)(damage / 1.5))),
                    new Vector2(direction.X * 10, direction.Y * 10), world, contentManager, rnd);
            }
        }
    }
}