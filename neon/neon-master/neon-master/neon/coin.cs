﻿using Microsoft.VisualBasic;
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
    public class Coin:MapObject
    {
        public const float CollectionDistance = 30f;
        public const float PickUpDistance = 5f;

        public Coin() : base() { }

        public Coin(ContentManager contentManager, Vector2 position, int value, World world):
            base(contentManager, position, new Vector2(0, 0), 1f, null,
                "coin"+(value/3).ToString()+"_", 0, world)
        { }

        public override void Update(ContentManager contentManager, World world)
        {
            float dst = Game1.GetDistance(Position, world.Hero.Position);
         
            if (dst < CollectionDistance)
            {
                if (dst < PickUpDistance)
                    Alive = false;
                else
                {
                    float dir = Game1.GetDirection(world.Hero.Position, Position);

                    ChangeMovement((float)Math.Cos(dir) * ((Hero)world.Hero).Speed * 5f,
                        (float)Math.Sin(dir) * ((Hero)world.Hero).Speed*5f);
                }
            }

            base.Update(contentManager, world);
        }

        public virtual void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();

            spriteBatch.Draw(whatToDraw, new Vector2(x - whatToDraw.Width / 2, y - whatToDraw.Height), null, color, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, depth+0.5f);
        }
    }

    public class Health : MapObject
    {
        public const float CollectionDistance = 35f;
        public const float PickUpDistance = 2f;
        public int Value { get; protected set; }

        public Health() : base() { }

        public Health(ContentManager contentManager, Vector2 position, int value, World world) :
            base(contentManager, position, new Vector2(0, 0), 3f, null,
                "heal" + (value/10).ToString() + "_", 0, world)
        {
            Value = value;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            float dst = Game1.GetDistance(Position, world.Hero.Position);

            if (dst < CollectionDistance)
            {
                if (dst < PickUpDistance)
                {
                    Alive = false;
                    ((Mob)world.Hero).Damage(-Value, new Vector2(0, 0), world, contentManager);
                }
                else
                {
                    float dir = Game1.GetDirection(world.Hero.Position, Position);

                    ChangeMovement((float)Math.Cos(dir) * ((Hero)world.Hero).Speed * 2f,
                        (float)Math.Sin(dir) * ((Hero)world.Hero).Speed * 2f);
                }
            }

            base.Update(contentManager, world);
        }
    }
}