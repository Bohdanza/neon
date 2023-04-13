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
    public class GroundPath : MapObject
    {
        public GroundPath() : base() { }

        public GroundPath(ContentManager contentManager, float x, float y, World world, int subtype)
            : base(contentManager, new Vector2(x, y),
            new Vector2(0f, 0f), 10000000f,
            null,
            "groundpatch_" + subtype.ToString() + "_", 1, world)
        { }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D cf = Texture.GetCurrentFrame();
            base.Draw(spriteBatch, x+cf.Width*Game1.PixelScale/2, y+cf.Height*Game1.PixelScale, color, depth-0.099999f);
        }
    }
}
