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
    public class Tersol:Monster
    {
        public Tersol(ContentManager contentManager, Vector2 position, World world):
            base(contentManager, position, 5f, 10, 
                @"hitboxes\terasol.png",
                "tersol", 0.3f, world)
        { }
    }
}