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
    public abstract class Monster:Mob
    {
        public Monster(ContentManager contentManager, Vector2 position, float weight,
            int hp, List<Tuple<int, int>> hitbox, string textureName, WorldChunk worldChunk):
            base(contentManager, position, new Vector2(0, 0), weight, hp, hitbox, textureName, worldChunk)
        { }


    }
}