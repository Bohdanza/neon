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
        public Spike(ContentManager contentManager, float x, float y, WorldChunk worldChunk):base(contentManager, new Vector2(x, y), 
            new Vector2(0f, 0f), 10000000f, 
            new List<Tuple<int, int>> { 
                new Tuple<int, int>(0, -1),
                new Tuple<int, int>(-1, -1),
                new Tuple<int, int>(-2, -1),
                new Tuple<int, int>(1, -1),
                new Tuple<int, int>(2, -1),
                new Tuple<int, int>(0, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(1, 0),
                new Tuple<int, int>(0, -2),
                new Tuple<int, int>(-1, -2),
                new Tuple<int, int>(-2, -2),
                new Tuple<int, int>(1, -2),
                new Tuple<int, int>(2, -2),
                new Tuple<int, int>(0, -3), new Tuple<int, int>(-1, -3), new Tuple<int, int>(1, -3)}, 
            "pike"+new Random().Next(0, 3).ToString()+"_", 0, worldChunk)
        {}
    }
}