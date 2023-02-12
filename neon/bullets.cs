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
    public class RevolverBullet:Bullet
    {
        public RevolverBullet(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10000f, 35, @"hitboxes\onebyone",
                "peacemaker", world, 1000)
        { }

        public override Bullet Copy(ContentManager contentManager, World world)
        {
            return new RevolverBullet(contentManager, new Vector2(Position.X, Position.Y),
                new Vector2(Movement.X, Movement.Y), world);
        }
    }
    public class SpearBullet : Bullet
    {
        public SpearBullet(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10000f, 20, @"hitboxes\spearhit.png",
                "spearhit", world, 9)
        { }
    }
}