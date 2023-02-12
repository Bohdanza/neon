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
    public class Idol:MapObject
    {
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

    public class Spike:MapObject
    {
        public Spike(ContentManager contentManager, float x, float y, World world):base(contentManager, new Vector2(x, y), 
            new Vector2(0f, 0f), 10000000f, 
            null, 
            "pike"+new Random().Next(0, 3).ToString()+"_", 0, world)
        {}
    }

    public class Rock:MapObject
    {
        public Rock(ContentManager contentManager, float x, float y, World world, int subtype):
            base(contentManager, new Vector2(x, y), new Vector2(0f, 0f), 10000000f, 
                @"hitboxes\rock"+subtype.ToString()+".png", "rock"+subtype.ToString()+"_",
                0, world)
        {
            
        }
    }
}