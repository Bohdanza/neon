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


    public class CrawlerBullet : Bullet
    {
        private float Height, Hreduce=0.3f, FallingSpeed;
        public DynamicTexture ShadowTexture=null;

        public CrawlerBullet() : base() { }

        public CrawlerBullet(ContentManager contentManager, Vector2 position, Vector2 movement,
            World world, int subtype, float verticalSpeed) :
            base(contentManager, position, movement, 10000f, 18, @"hitboxes\onebyone",
                "crawlerbullet"+subtype.ToString()+"_", world, 1000)
        {
            Height = 2;
            FallingSpeed = verticalSpeed;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            Height += FallingSpeed;
            FallingSpeed -= Hreduce;

            if (Height <= 0)
            {
                Alive = false;

                var rnd = new Random();
                int bloodCount = rnd.Next(10, 17);
                Color clr = new Color(110 + rnd.Next(0, 40), 0, 0);

                for (int i = 0; i < bloodCount; i++)
                {
                    float ym = (float)(rnd.NextDouble() - 0.5) * 20;

                    world.AddObject(new Blood(contentManager, Position, 
                        new Vector2(ym, rnd.Next(-7, 17)),
                        0, 0, rnd.Next(30, 90), clr, -rnd.Next(0, 20), rnd.Next(0, 3), world));
                }

                HashSet<MapObject> obst = HitboxObstructions(world);

                if (obst.Count > 0)
                {
                    foreach (var co in obst)
                        if (co is Mob&&!(co is Crawler))
                        {
                            ((Mob)co).Damage(Damage, Movement, world, contentManager);
                        }
                }
            }

            if (Hitbox == null)
            {
                if (HitboxPath != null)
                    Hitbox = world.WorldHitboxFabricator.CreateHitbox(HitboxPath);
                else
                    Hitbox = new List<Vector2>();
            }

            if (Texture == null)
                Texture = new DynamicTexture(contentManager, TextureName);

            if (ShadowTexture == null)
                ShadowTexture = new DynamicTexture(contentManager, "shadow0_");

            Lifetime--;

            if (Lifetime <= 0)
                Alive = false;

            Texture.Update(contentManager);

            Position = new Vector2(Position.X + Movement.X, Position.Y + Movement.Y);

            ChangeMovement(-Movement.X, -Movement.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y-(int)Height, color, depth);
            spriteBatch.Draw(ShadowTexture.GetCurrentFrame(), new Vector2(x, y), null, color, 0f, new Vector2(0, 0),
                1f, SpriteEffects.None, depth);
        }

        public override Bullet Copy(ContentManager contentManager, World world)
        {
            return new ShotgunBullet(contentManager, new Vector2(Position.X, Position.Y),
                new Vector2(Movement.X, Movement.Y), world);
        }
    }
}