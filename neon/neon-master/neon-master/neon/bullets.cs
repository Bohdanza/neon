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
        public RevolverBullet() : base() { }

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
        public SpearBullet() : base() { }

        public SpearBullet(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10000f, 20, @"hitboxes\spearhit.png",
                "spearhit", world, 9)
        { }
    }

    public class ShotgunBullet : Bullet
    {
        public ShotgunBullet() : base() { }

        public ShotgunBullet(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10000f, 18, @"hitboxes\onebyone",
                "shotbul", world, 1000)
        { }

        public override Bullet Copy(ContentManager contentManager, World world)
        {
            return new ShotgunBullet(contentManager, new Vector2(Position.X, Position.Y),
                new Vector2(Movement.X, Movement.Y), world);
        }
    }

    public class ArratBullet : Bullet
    {
        public ArratBullet() : base() { }

        public ArratBullet(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10000f, 75, @"hitboxes\arratbul",
                "arratbul", world, 1000)
        { }

        public override Bullet Copy(ContentManager contentManager, World world)
        {
            return new ArratBullet(contentManager, new Vector2(Position.X, Position.Y),
                new Vector2(Movement.X, Movement.Y), world);
        }
    }

    public class Biospike:Bullet
    {
        public Biospike() : base() { }

        public Biospike(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10000f, 300, @"hitboxes\onebyone",
                "biospike", world, 500)
        { }

        public override void Update(ContentManager contentManager, World world)
        {
            var rnd = new Random();
            int fireCount = rnd.Next(6, 11);

            float spd = Game1.GetDistance(new Vector2(0, 0), Movement);
            float dir = Game1.GetDirection(Movement, new Vector2(0, 0));
            dir %=(float)Math.PI * 2;

            spd += 0.1f;

            SetMovement(Game1.DirectionToVector(dir) * spd);

            for (int i = 0; i < fireCount; i++)
            {
                double dir1 = dir +rnd.NextDouble()-0.5;
                Vector2 vv = Game1.DirectionToVector((float)dir1);
                vv = new Vector2(vv.X * (7 + rnd.Next(0, 5)), vv.Y * (7 + rnd.Next(0, 5)));

                world.AddObject(new Fire(contentManager, new Vector2(Position.X, Position.Y),
                    vv,
                    0, 0,
                    5 + rnd.Next(0, 15),
                    new Color(158+rnd.Next(-10, 20), 255, 250 + rnd.Next(-10, 20), 225), -10000, 0, world));
            }

            base.Update(contentManager, world);
        }

        public override Bullet Copy(ContentManager contentManager, World world)
        {
            return new Biospike(contentManager, new Vector2(0,0), new Vector2(0,0), world);
        }
    }
}