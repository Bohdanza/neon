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
    public abstract class Bullet : MapObject
    {
        [JsonProperty]
        public int Damage { get; protected set; }
        [JsonProperty]
        public int Lifetime { get; protected set; }

        public Bullet() : base() { }

        public Bullet(ContentManager contentManager, Vector2 position, Vector2 movement, float weight, 
            int damage, string hitboxPath, string textureName, World world, int lifetime):
            base(contentManager, position, movement, weight, hitboxPath, textureName, 0, world)
        {
            Damage = damage;
            Lifetime = lifetime;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            if (Hitbox == null)
            {
                if (HitboxPath != null)
                    Hitbox = world.WorldHitboxFabricator.CreateHitbox(HitboxPath);
                else
                    Hitbox = new List<Vector2>();
            }

            if (Texture == null)
                Texture = new DynamicTexture(contentManager, TextureName);

            Lifetime--;

            if (Lifetime <= 0)
                Alive = false;

            Texture.Update(contentManager);

            Vector2 ppos = new Vector2(Position.X, Position.Y);

            Position = new Vector2(Position.X + Movement.X, Position.Y);

            if (Alive && (int)ppos.X != (int)Position.X)
            {
                HashSet<MapObject> obst = HitboxObstructions(world);

                if (obst.Count > 0)
                {
                    Alive = false;
                    
                    foreach(var co in obst)
                        if(co is Mob)
                        {
                            ((Mob)co).Damage(Damage, Movement, world, contentManager);
                        }
                }
            }

            Position = new Vector2(Position.X, Position.Y + Movement.Y);

            if (Alive && (int)ppos.Y != (int)Position.Y)
            {
                HashSet<MapObject> obst = HitboxObstructions(world);

                if (obst.Count > 0)
                {
                    Alive = false;

                    foreach (var co in obst)
                        if (co is Mob)
                        {
                            ((Mob)co).Damage(Damage, Movement, world, contentManager);
                        }
                }
            }

            ChangeMovement(-Movement.X, -Movement.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();

            float Rotation = Game1.GetDirection(Movement, new Vector2(0, 0));

            if (Rotation >= Math.PI * 1.5 || Rotation < Math.PI / 2)
                spriteBatch.Draw(whatToDraw, new Vector2(x, y - whatToDraw.Height / 2 * Game1.PixelScale), null, color,
                    Rotation,
                    new Vector2(whatToDraw.Width / 2, whatToDraw.Height / 2), Game1.PixelScale, SpriteEffects.None, depth);
            else
                spriteBatch.Draw(whatToDraw, new Vector2(x, y - whatToDraw.Height / 2*Game1.PixelScale), null, color,
                    Rotation,
                    new Vector2(whatToDraw.Width / 2, whatToDraw.Height / 2), Game1.PixelScale, SpriteEffects.FlipVertically, depth);
        }

        public virtual Bullet Copy(ContentManager contentManager, World world)
        {
            return null;
        }
    }
}