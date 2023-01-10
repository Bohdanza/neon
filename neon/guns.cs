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
    public class Colt:Gun
    {
        public Colt(ContentManager contentManager, Vector2 position, Vector2 movement, WorldChunk worldChunk):
            base(contentManager, position, movement, 5f, new List<Tuple<int, int>> { }, "colt", worldChunk, 
                new List<int> { 30, 30, 30, 30, 30, 300})
        { }

        public override void ShootInDirection(ContentManager contentManager, float Direction, WorldChunk worldChunk)
        {
            if (TimeTillShot > 0)
                return;

            worldChunk.Objects.Add(new RevolverBullet(contentManager,
                new Vector2(Position.X + (float)Math.Cos(Direction) * 5f, Position.Y + (float)Math.Sin(Direction) * 5f),
                new Vector2((float)Math.Cos(Direction), (float)Math.Sin(Direction)), worldChunk));

            base.ShootInDirection(contentManager, Direction, worldChunk);
        }
    }
}