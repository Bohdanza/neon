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
        public Colt() : base() { }

        public Colt(ContentManager contentManager, Vector2 position, Vector2 movement, World world):
            base(contentManager, position, movement, 5f, null, "colt", world, 
                new List<int> { 15, 15, 15, 15, 15, 300})
        { }

        public override void ShootInDirection(ContentManager contentManager, float Direction, World world, MapObject owner)
        {
            if (TimeTillShot > 0)
                return;

            world.Objects.Add(new RevolverBullet(contentManager,
                new Vector2(Position.X + (float)Math.Cos(Direction) * 5f, Position.Y + (float)Math.Sin(Direction) * 5f),
                new Vector2((float)Math.Cos(Direction)*1.5f, (float)Math.Sin(Direction)*1.5f), world));

            base.ShootInDirection(contentManager, Direction, world, owner);
        }
    }

    public class Spear : Gun
    {
        public Spear() : base() { }
   
        public Spear(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 10f, null, "spear", world,
                new List<int> { 40 })
        { }
        
        public override void ShootInDirection(ContentManager contentManager, float Direction, World world, MapObject owner)
        {
            if (TimeTillShot > 0)
                return;

            world.Objects.Add(new SpearBullet(contentManager,
                new Vector2(Position.X + (float)Math.Cos(Direction) * 20f, Position.Y + (float)Math.Sin(Direction) * 20f),
                new Vector2((float)Math.Cos(Direction)*0.1f, (float)Math.Sin(Direction)*0.1f), world));

            owner.ChangeMovement((float)Math.Cos(Direction) * 10f, (float)Math.Sin(Direction) * 10f);

            base.ShootInDirection(contentManager, Direction, world, owner);
        }
    }

    public class ShotGun:Gun
    {
        public ShotGun(ContentManager contentManager, Vector2 position, Vector2 movement, World world):
            base(contentManager, position, movement, 10f, null, "shotgun", world,
                new List<int> { 150 })
        {

        }

        public override void ShootInDirection(ContentManager contentManager, float Direction, World world, MapObject owner)
        {
            if (TimeTillShot > 0)
                return;

            var rnd = new Random();

            for (int i = 0; i < rnd.Next(5, 7); i++)
            {
                double ang = Direction + rnd.NextDouble() - 0.5;

                world.Objects.Add(new ShotgunBullet(contentManager,
                    new Vector2(Position.X + (float)Math.Cos(Direction) * 5f, Position.Y + (float)Math.Sin(Direction) * 5f),
                    new Vector2((float)Math.Cos(ang) * 3f,
                    (float)Math.Sin(ang) * 3f), world));
            }

            base.ShootInDirection(contentManager, Direction, world, owner);
        }
    }

    public class Arrat : Gun
    {
        public Arrat() : base() { }

        public Arrat(ContentManager contentManager, Vector2 position, Vector2 movement, World world) :
            base(contentManager, position, movement, 5f, null, "arrat", world,
                new List<int> { 55, 55, 100 })
        { }

        public override void ShootInDirection(ContentManager contentManager, float Direction, World world, MapObject owner)
        {
            if (TimeTillShot > 0)
                return;

            world.Objects.Add(new ArratBullet(contentManager,
                new Vector2(Position.X + (float)Math.Cos(Direction) * 7f, Position.Y + (float)Math.Sin(Direction) * 7f),
                new Vector2((float)Math.Cos(Direction) * 2.8f, (float)Math.Sin(Direction) * 2.8f), world));

            base.ShootInDirection(contentManager, Direction, world, owner);
        }
    }
}