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
    public class RevolverBullet:Bullet
    {
        public RevolverBullet(ContentManager contentManager, Vector2 position, Vector2 movement, WorldChunk worldChunk) :
            base(contentManager, position, movement, 1000f, 35, new List<Tuple<int, int>> { new Tuple<int, int>(0, 0) },
                "peacemaker", worldChunk, 1000)
        { }        
    }
}