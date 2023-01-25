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

        public World(ContentManager contentManager, string path)
        {
            Path = path;

            if (Path[Path.Length - 1] != '\\')
                Path += "\\";

            if (!Directory.Exists(Path))
            {
                var dr = Directory.CreateDirectory(Path);
            }

            if (!File.Exists(Path + "current"))
            {
                var fl=File.Create(Path + "current");

                fl.Close();

                using (StreamWriter sw = new StreamWriter(Path + "current"))
                {
                    sw.Write("0\n0\n");
                }
            }

            int chunkX = 0, chunkY = 0;

            using (StreamReader sr = new StreamReader(Path + "current"))
            {
                List<string> lst = sr.ReadToEnd().Split('\n').ToList();

                chunkX = Int32.Parse(lst[0]);
                chunkY = Int32.Parse(lst[1]);
            }

            worldChunk = new WorldChunk(contentManager, chunkX, chunkY, null, Path);
        }

        public void Update(ContentManager contentManager)
        {
            worldChunk.Update(contentManager);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            worldChunk.Draw(spriteBatch);
        }

        public void Save()
        {
            worldChunk.Save(Path);
        }
    }
}
