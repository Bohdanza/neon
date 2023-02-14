﻿using Microsoft.VisualBasic;
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
    public abstract class Mob:MapObject
    {
        [JsonProperty]
        public int HP { get; protected set; }
        [JsonProperty]
        public string Action { get; protected set; }
        [JsonIgnore]
        public int Direction { get; private set; } = 0;
        [JsonIgnore]
        private string pact = "";

        [JsonConstructor]
        public Mob() : base()
        { }

        public Mob(ContentManager contentManager, Vector2 position, Vector2 movement, float weight, int hp,
            string hitboxPath, string textureName, World world):
            base(contentManager, position, movement, weight, hitboxPath, textureName+"_id_", 0, world)
        {
            Action = "id";
            HP = hp;

            TextureName = textureName;
        }

        public override void Update(ContentManager contentManager, World world)
        {
            if (Action != pact)
            {
                Texture = new DynamicTexture(contentManager, TextureName + "_" + Action + "_");
            }

            pact = Action;

            if (Action == "die" && Texture.CurrentTexture >= Texture.Textures.Count - 1)
                return;

            if (Movement.X > 0)
                Direction = 1;
            else
                Direction = 0;

            base.Update(contentManager, world);
        }

        protected void AutoAction()
        {
            if (Game1.GetDistance(0, 0, Position.X, Position.Y) >= 0.00001f)
                Action = "wa";
            else
                Action = "id";
        }

        public virtual void Damage(int damage)
        {
            HP -= damage;

            if (HP <= 0)
            {
                Action = "die";
                return;
            }

            if (damage > 0)
                Action = "dam";
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();

            if(Direction==1)
                spriteBatch.Draw(whatToDraw, 
                    new Vector2(x - whatToDraw.Width*Game1.PixelScale / 2, y - whatToDraw.Height*Game1.PixelScale), 
                    null, color, 0f,
                    new Vector2(0, 0), Game1.PixelScale, SpriteEffects.None, depth);
            else
                spriteBatch.Draw(whatToDraw, 
                    new Vector2(x - whatToDraw.Width*Game1.PixelScale / 2, y - whatToDraw.Height * Game1.PixelScale),
                    null, color, 0f,
                    new Vector2(0, 0), Game1.PixelScale, SpriteEffects.FlipHorizontally, depth);
        }
    }
}
