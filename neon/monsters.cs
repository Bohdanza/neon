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
        public Tersol() : base() { }

        public Tersol(ContentManager contentManager, Vector2 position, World world):
            base(contentManager, position, 5f, 10, 
                @"hitboxes\terasol.png",
                "tersol", 0.3f, world)
        {
            GunInHand = new Colt(contentManager, position, new Vector2(0, 0), world);
        }
    }

    public class ScaryLilGreenman : Monster
    {
        public ScaryLilGreenman() : base() { }

        public ScaryLilGreenman(ContentManager contentManager, Vector2 position, World world) :
            base(contentManager, position, 4f, 10,
                @"hitboxes\greenman",
                "lilgreenman", 0.3f, world)
        {
            GunInHand = new Arrat(contentManager, position, new Vector2(0, 0), world);
        }
    }

    public class MossCrab : Monster
    {
        public MossCrab() : base() { }

        public MossCrab(ContentManager contentManager, Vector2 position, World world) :
            base(contentManager, position, 4f, 300,
                @"hitboxes\mosscrab",
                "mosscrab", 0.1f, world)
        {
            GunInHand = null;
        }
    }
}