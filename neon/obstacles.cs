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
    public class Spike:MapObject
    {
        public Spike(ContentManager contentManager, float x, float y):base(contentManager, new Vector2(x, y), 
            new Vector2(0f, 0f), 10000000f, 
            new List<Tuple<int, int>> { 
                new Tuple<int, int>(0, 0)}, 
            "pike", 0)
        {}
    }
}