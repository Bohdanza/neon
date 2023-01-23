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
    public class World
    {
        public WorldChunk worldChunk { get; protected set; }
        public string Path { get; protected set; }

        public World(string path)
        {
            Path = path;
        }

        public void Update(ContentManager contentManager)
        {
            worldChunk.Update(contentManager);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            worldChunk.Draw(spriteBatch);
        }
    }
}
