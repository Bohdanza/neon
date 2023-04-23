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
    public abstract class Gun : MapObject
    {
        public float Rotation { get; set; }

        [JsonProperty]
        public int PowerType { get; private set; }
        [JsonProperty]
        public int TimeTillShot { get; protected set; }
        [JsonProperty]
        public List<int> ShootingPauses { get; protected set; }
        [JsonProperty]
        protected int CurrentPause { get; private set; }

        private string Action = "", pact="";

        public Gun():base() { Texture = null; }

        public Gun(ContentManager contentManager, Vector2 position, Vector2 movement, float weight,
            string hitboxPath, string textureName, World world, List<int> shootingPauses, int powerType) :
            base(contentManager, position, movement, weight, hitboxPath, textureName, 0, world)
        {
            PowerType = powerType;
            ShootingPauses = shootingPauses;
        }

        public virtual void ShootInDirection(ContentManager contentManager, float Direction, World world, Mob owner)
        {
            Action = "_shot_";
            TimeTillShot = ShootingPauses[CurrentPause];

            CurrentPause++;
            CurrentPause %= ShootingPauses.Count;
        }

        public virtual void UpdateInHand(ContentManager contentManager)
        {
            if (Texture == null||pact!=Action)
                Texture = new DynamicTexture(contentManager, TextureName+Action);

            Texture.Update(contentManager);

            pact = Action;

            if (Action == "_shot_" && Texture.CurrentTexture == Texture.Textures.Count - 1)
                Action = "";

            TimeTillShot--;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            if (Texture != null)
            {
                Texture2D whatToDraw = Texture.GetCurrentFrame();

                if (Rotation >= Math.PI * 1.5 || Rotation < Math.PI / 2)
                    spriteBatch.Draw(whatToDraw, new Vector2(x, y),
                        null, color, Rotation,
                        new Vector2(whatToDraw.Width / 2, whatToDraw.Height / 2),
                        Game1.PixelScale, SpriteEffects.None, depth);
                else
                    spriteBatch.Draw(whatToDraw, new Vector2(x, y),
                        null, color, Rotation,
                        new Vector2(whatToDraw.Width / 2, whatToDraw.Height / 2), Game1.PixelScale, SpriteEffects.FlipVertically, depth);
            }
        }
    }
}