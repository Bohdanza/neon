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
    public abstract class Bullet : MapObject
    {
        public int Damage { get; protected set; }
        public int Lifetime { get; protected set; }

        public Bullet(ContentManager contentManager, Vector2 position, Vector2 movement, float weight, 
            int damage, List<Tuple<int, int>> hitbox, string textureName, WorldChunk worldChunk, int lifetime):
            base(contentManager, position, movement, weight, hitbox, textureName, 0, worldChunk)
        {
            Damage = damage;
            Lifetime = lifetime;
        }

        public override void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            Lifetime--;

            if (Lifetime <= 0)
                Alive = false;

            Texture.Update(contentManager);

            Vector2 ppos = new Vector2(Position.X, Position.Y);

            Position = new Vector2(Position.X + Movement.X, Position.Y);

            if (Alive && (int)ppos.X != (int)Position.X)
            {
                HashSet<MapObject> obst = HitboxObstructions(worldChunk);

                if (obst.Count > 0)
                {
                    Alive = false;
                    
                    foreach(var co in obst)
                        if(co is Mob)
                        {
                            ((Mob)co).Damage(Damage);
                        }
                }
            }

            Position = new Vector2(Position.X, Position.Y + Movement.Y);

            if (Alive && (int)ppos.Y != (int)Position.Y)
            {
                HashSet<MapObject> obst = HitboxObstructions(worldChunk);

                if (obst.Count > 0)
                {
                    Alive = false;

                    foreach (var co in obst)
                        if (co is Mob)
                        {
                            ((Mob)co).Damage(Damage);
                        }
                }
            }

            ChangeMovement(-Movement.X, -Movement.Y);
        }

        public virtual Bullet Copy(ContentManager contentManager, WorldChunk worldChunk)
        {
            return null;
        }
    }
}