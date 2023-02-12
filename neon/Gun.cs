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
        public int TimeTillShot { get; protected set; }
        [JsonProperty]
        public List<int> ShootingPauses { get; protected set; }
        [JsonProperty]
        protected int CurrentPause { get; private set; }

        public Gun(ContentManager contentManager, Vector2 position, Vector2 movement, float weight,
            string hitboxPath, string textureName, World world, List<int> shootingPauses) :
            base(contentManager, position, movement, weight, hitboxPath, textureName, 0, world)
        {
            ShootingPauses = shootingPauses;
        }

        public virtual void ShootInDirection(ContentManager contentManager, float Direction, World world, MapObject owner)
        {
            TimeTillShot = ShootingPauses[CurrentPause];

            CurrentPause++;
            CurrentPause %= ShootingPauses.Count;
        }

        public virtual void UpdateInHand(ContentManager contentManager)
        {
            if (Texture == null)
                Texture = new DynamicTexture(contentManager, TextureName);

            Texture.Update(contentManager);

            TimeTillShot--;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();

            if (Rotation >= Math.PI * 1.5 || Rotation < Math.PI / 2)
                spriteBatch.Draw(whatToDraw, new Vector2(x, y - whatToDraw.Height / 2), null, color,
                    Rotation,
                    new Vector2(whatToDraw.Width / 2, whatToDraw.Height / 2), 1f, SpriteEffects.None, depth);
            else
                spriteBatch.Draw(whatToDraw, new Vector2(x, y - whatToDraw.Height / 2), null, color,
                    Rotation,
                    new Vector2(whatToDraw.Width / 2, whatToDraw.Height / 2), 1f, SpriteEffects.FlipVertically, depth);
        }
    }
}