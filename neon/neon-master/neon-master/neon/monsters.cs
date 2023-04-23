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
    public class Tersol:Monster
    {
        public Tersol() : base() { }

        public Tersol(ContentManager contentManager, Vector2 position, World world):
            base(contentManager, position, 5f, 10, 
                @"hitboxes\terasol.png",
                "tersol", 0.3f, world)
        {
            GunInHand = new Colt(contentManager, position, new Vector2(0, 0), world);
        }
    }

    public class ScaryLilGreenman : Monster
    {
        public ScaryLilGreenman() : base() { }

        public ScaryLilGreenman(ContentManager contentManager, Vector2 position, World world) :
            base(contentManager, position, 4f, 10,
                @"hitboxes\greenman",
                "lilgreenman", 0.3f, world)
        {
            GunInHand = new Arrat(contentManager, position, new Vector2(0, 0), world);
        }
    }

    public class MossCrab : Monster
    {
        public MossCrab() : base() { }

        public MossCrab(ContentManager contentManager, Vector2 position, World world) :
            base(contentManager, position, 4f, 300,
                @"hitboxes\mosscrab",
                "mosscrab", 0.1f, world)
        {
            ShootProbability = 15;
            GunInHand = new Colt(contentManager, position, new Vector2(0, 0), world);
        }

        public override void Update(ContentManager contentManager, World world)
        {
            base.Update(contentManager, world);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, color, depth);
        }

        public override void Damage(int damage, Vector2 direction, World world, ContentManager contentManager)
        {
            base.Damage(damage, direction, world, contentManager);

            if (Action != "die"||Texture.CurrentTexture!=Texture.Textures.Count-1||!Texture.BaseName.EndsWith("die_"))
            {
                var rnd = new Random();

                AddBlood(new Color(232 + rnd.Next(-50, 20), 11, 0),
                    Math.Min(100, rnd.Next(damage / 4, (int)(damage/1.5))), 
                    new Vector2(direction.X * 7, direction.Y*7),
                    world, contentManager, rnd);

                int HealCount = rnd.Next(0, 4);

                for(int i=0; i<HealCount; i++)
                {
                    world.AddObject(new Health(contentManager, Position, rnd.Next(1, 20), world));
                }
            }
        }
    }
}