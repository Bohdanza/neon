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
    public class Idol:MapObject
    {
        public Idol() : base() { }

        public Idol(ContentManager contentManager, float x, float y, World world) : 
            base(contentManager, new Vector2(x, y),
            new Vector2(0f, 0f), 10000000f,
            @"hitboxes\idol.png",
            "idol_0_", 0, world)
        { }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();
                
            spriteBatch.Draw(whatToDraw, new Vector2(x - whatToDraw.Width / 2, y - whatToDraw.Height+27), null, color, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, depth);
        }
    }

    public class Spike : MapObject
    {
        public Spike():base(){}

        public Spike(ContentManager contentManager, float x, float y, World world, int subtype)
            :base(contentManager, new Vector2(x, y), 
            new Vector2(0f, 0f), 10000000f, 
            null,
            "pike"+subtype.ToString()+"_", 0, world)
        {}
    }

    public class Rock:MapObject
    {
        public Rock() : base() { }

        public Rock(ContentManager contentManager, float x, float y, World world, int subtype):
            base(contentManager, new Vector2(x, y), new Vector2(0f, 0f), 10000000f, 
                @"hitboxes\rock"+subtype.ToString(), "rock"+subtype.ToString()+"_",
                0, world)
        {
            
        }
    }

    public class Cheerock : MapObject
    {
        public Cheerock() : base() { }

        public Cheerock(ContentManager contentManager, float x, float y, World world, int subtype) :
            base(contentManager, new Vector2(x, y), new Vector2(0f, 0f), 10000000f,
                @"hitboxes\cheerock" + subtype.ToString(), "cheerock" + subtype.ToString() + "_",
                0, world)
        {

        }
    }

    public class Boab : MapObject
    {
        public Boab() : base() { }

        public Boab(ContentManager contentManager, float x, float y, World world, int subtype) :
            base(contentManager, new Vector2(x, y), new Vector2(0f, 0f), 10000000f,
                @"hitboxes\boab" + subtype.ToString(), "boab" + subtype.ToString() + "_",
                0, world)
        {

        }
    }

    public class Weed : MapObject
    {
        public Weed() : base() { }

        public Weed(ContentManager contentManager, float x, float y, World world, int subtype) :
            base(contentManager, new Vector2(x, y), new Vector2(0f, 0f), 10000000f,
                null, "weed" + subtype.ToString() + "_",
                1, world)
        {

        }
    }
}