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
    public abstract class Mob:MapObject
    {
        [JsonProperty]
        public int HP { get; protected set; }
        [JsonProperty]
        public string Action { get; protected set; }
        [JsonIgnore]
        public int Direction { get; private set; } = 0;
        [JsonIgnore]
        private int pdir = 1;
        [JsonIgnore]
        private string pact = "";

        public Mob(ContentManager contentManager, Vector2 position, Vector2 movement, float weight, int hp,
            List<Tuple<int, int>> hitbox, string textureName, WorldChunk worldChunk):
            base(contentManager, position, movement, weight, hitbox, textureName+"_id_0_", 0, worldChunk)
        {
            Action = "id";
            HP = hp;

            TextureName = textureName;
        }

        public override void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            if (Action != pact || pdir != Direction)
            {
                Texture = new DynamicTexture(contentManager, TextureName + "_" + Action + "_" + Direction.ToString() + "_");
            }

            pdir = Direction;
            pact = Action;

            if (Action == "die" && Texture.CurrentTexture >= Texture.Textures.Count - 1)
                return;

            if (Movement.X > 0)
                Direction = 1;
            else
                Direction = 0;

            base.Update(contentManager, worldChunk);
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
    }
}
