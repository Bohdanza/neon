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
using System.Text;
using System.Threading;

namespace neon
{
    public abstract class Particle:MapObject
    {
        [JsonProperty]
        protected int OffsetX = 0;
        [JsonProperty]
        protected int OffsetY = 0;
        [JsonProperty]
        public Vector2 OffsetMovement { get; protected set; }
        [JsonProperty]
        protected int LifeTime;

        [JsonConstructor]
        public Particle():base()
        { }

        public Particle(ContentManager contentManager, Vector2 position, Vector2 offsetMovement,
            int offsetX, int offsetY, int lifeTime, string textureName, World world) :
            base(contentManager, position, new Vector2(0, 0), 4f, null, textureName, 2, world)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            LifeTime = lifeTime;
            OffsetMovement = offsetMovement;
        }
        
        public override void Update(ContentManager contentManager, World world)
        {
            LifeTime--;

            if (LifeTime <= 0)
                Alive = false;

            OffsetX += (int)OffsetMovement.X;
            OffsetY += (int)OffsetMovement.Y;

            base.Update(contentManager, world);
        }

        public void ChangeOffsetMovement(float x, float y)
        {
            if (Weight != 0)
            {
                x /= Weight;
                y /= Weight;
            }

            OffsetMovement = new Vector2(OffsetMovement.X + x, OffsetMovement.Y + y);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x+OffsetX, y+OffsetY, color, depth);
        }
    }

    public class Blood:Particle
    {
        [JsonProperty]
        public Color BloodColor { get; protected set; }
        [JsonProperty]
        public int ZeroGround { get; protected set; }

        public Blood():base()
        {}

        public Blood(ContentManager contentManager, Vector2 position, Vector2 offsetMovement, 
            int offsetX, int offsetY, int lifeTime, Color bloodColor, int zeroGround, int textureType, World world):
            base(contentManager, position, offsetMovement, offsetX, offsetY, lifeTime,
                "blooddrop"+textureType.ToString()+"_", world)
        {
            BloodColor = bloodColor;
            ZeroGround = zeroGround;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            ChangeOffsetMovement(-OffsetMovement.X/4, 0);

            if (OffsetY <= ZeroGround)
                ChangeOffsetMovement(0, 5f);
            else
                OffsetMovement = new Vector2(0, 0);

            base.Update(contentManager, world);
        }


        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, BloodColor, depth);
        }
    }

    public class Fire : Particle
    {
        [JsonProperty]
        public Color FireColor { get; protected set; }
        [JsonProperty]
        public int ZeroGround { get; protected set; }

        public Fire() : base()
        { }

        public Fire(ContentManager contentManager, Vector2 position, Vector2 offsetMovement,
            int offsetX, int offsetY, int lifeTime, Color fireColor, int zeroGround, int textureType, World world) :
            base(contentManager, position, offsetMovement, offsetX, offsetY, lifeTime,
                "blooddrop" + textureType.ToString() + "_", world)
        {
            FireColor = fireColor;
            ZeroGround = zeroGround;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            base.Update(contentManager, world);
        }


        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            base.Draw(spriteBatch, x, y, FireColor, depth);
        }
    }
}