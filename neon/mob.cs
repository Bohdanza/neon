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

namespace neon
{
    public abstract class Mob:MapObject
    {
        public int HP { get; protected set; }
        public string Action { get; private set; }
        public int Direction { get; private set; } = 0;

        private int pdir = 1;
        private string pact = "";
        private string TextureName = "";
        

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
            if (Movement.X > 0)
                Direction = 1;
            else
                Direction = 0;

            base.Update(contentManager, worldChunk);
            
            if(Action!=pact||pdir!=Direction)
            {
                Texture = new DynamicTexture(contentManager, TextureName + "_" + Action + "_" + Direction.ToString() + "_");
            }

            pdir = Direction;
            pact = Action;
        }
    }
}
