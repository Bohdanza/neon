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
    public abstract class Monster:Mob
    {
        protected int ComfortMin = 30;
        protected int ComfortMax = 60;

        public Monster(ContentManager contentManager, Vector2 position, float weight,
            int hp, List<Tuple<int, int>> hitbox, string textureName, WorldChunk worldChunk):
            base(contentManager, position, new Vector2(0, 0), weight, hp, hitbox, textureName, worldChunk)
        { }

        public override void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            float dst = Game1.GetDistance(worldChunk.Hero.Position.X, worldChunk.Hero.Position.Y,
                Position.X, Position.Y);

            if(dst<ComfortMin||dst>ComfortMax)
            {
                float dir = Game1.GetDirection(Position, worldChunk.Hero.Position);

                if (dst > ComfortMax)
                    dir += (float)Math.PI;

                ChangeMovement(Game1.DirectionToVector(dir));
            }
            else
            {
                var rnd = new Random();

                float dir = Game1.GetDirection(Movement, new Vector2(0, 0));

                if (dir < 0)
                    dir += (float)Math.PI;

                if (dir > Math.PI)
                    dir -= (float)Math.PI;

                dir += (float)(rnd.NextDouble() * Math.PI * 2 / 5)-(float)(Math.PI/5);

                ChangeMovement(Game1.DirectionToVector(dir));
            }

            base.Update(contentManager, worldChunk);
        }
    }
}