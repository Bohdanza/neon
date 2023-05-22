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
    public abstract class Monster:Mob
    {
        [JsonProperty]
        protected int ShootProbability=0;
        [JsonProperty]
        public Gun GunInHand { get; protected set; } = null;
        [JsonProperty]
        protected int ComfortMin = 30;
        [JsonProperty]
        protected int ComfortMax = 60;

        [JsonIgnore]
        public MapObject Target = null;

        [JsonProperty]
        protected float Speed = 0;

        private float dir = 0f; 

        [JsonConstructor]
        public Monster() :base()
        { }

        public Monster(ContentManager contentManager, Vector2 position, float weight,
            int hp, int maxHp, string hitboxPath, string textureName, float speed, World world):
            base(contentManager, position, new Vector2(0, 0), weight, hp, maxHp, hitboxPath, textureName, world)
        {
            Speed = speed;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            var rnd = new Random();

            //float dir1 = Game1.GetDirection(Movement, new Vector2(0, 0));

            if (Action != "die")
            {
                if (dir < 0)
                    dir += (float)Math.PI;

                if (dir > Math.PI)
                    dir -= (float)Math.PI;

                if (rnd.Next(0, 1000) < 10)
                    dir = (float)(rnd.NextDouble() * Math.PI * 2);

                dir += (float)Math.PI;

                Vector2 vector = Game1.DirectionToVector(dir);

                ChangeMovement(new Vector2(vector.X * Speed, vector.Y * Speed));

                if (GunInHand != null)
                {
                    GunInHand.UpdateInHand(contentManager);

                    GunInHand.Rotation = 
                        (float)(Game1.GetDirection(new Vector2(Position.X, Position.Y-2), 
                        world.Hero.Position)+Math.PI);

                    GunInHand.Position = new Vector2(Position.X, Position.Y - 2);

                    if (rnd.Next(0, 10000)<ShootProbability)
                    {
                        GunInHand.ShootInDirection(contentManager, GunInHand.Rotation, world, this);
                    }
                }

                AutoAction();
            }

            base.Update(contentManager, world);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, color, depth);
        }
    }
}