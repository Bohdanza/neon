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
        protected int ComfortMin = 30;
        [JsonProperty]
        protected int ComfortMax = 60;

        [JsonIgnore]
        public MapObject Target = null;
        protected float Speed = 1;

        private float dir = 0f; 

        public Monster(ContentManager contentManager, Vector2 position, float weight,
            int hp, List<Tuple<int, int>> hitbox, string textureName, float speed, WorldChunk worldChunk):
            base(contentManager, position, new Vector2(0, 0), weight, hp, hitbox, textureName, worldChunk)
        {
            Speed = speed;
        }

        public override void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            var rnd = new Random();
                    
            //float dir1 = Game1.GetDirection(Movement, new Vector2(0, 0));

            if (dir < 0)
                dir += (float)Math.PI;

            if (dir > Math.PI)
                dir -= (float)Math.PI;

            if(rnd.Next(0, 1000)<10)
                dir = (float)(rnd.NextDouble() * Math.PI*2);

            dir += (float)Math.PI;     

            Vector2 vector = Game1.DirectionToVector(dir);

            ChangeMovement(new Vector2(vector.X*Speed, vector.Y*Speed));

            base.Update(contentManager, worldChunk);
        }
    }
}