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
        public Tersol(ContentManager contentManager, Vector2 position, WorldChunk worldChunk):
            base(contentManager, position, 5f, 10, 
                new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(0, 0), new Tuple<int, int>(1, 0),  new Tuple<int, int>(2, 0),
                     new Tuple<int, int>(0, 0),
                }, 
                "tersol", worldChunk)
        { }
    }
}