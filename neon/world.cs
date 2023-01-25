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
            int px = worldChunk.CurrentChunkX;
            int py = worldChunk.CurrentChunkY;

            worldChunk.Update(contentManager);

            if(worldChunk.Hero.Position.X<3)
            {
                Hero wh = (Hero)worldChunk.Hero;

                wh.Position = new Vector2(worldChunk.HitMap.Size - 3, wh.Position.Y);

                worldChunk.Objects.Remove(wh);
                worldChunk.Save(Path);

                worldChunk = new WorldChunk(contentManager, worldChunk.CurrentChunkX - 1, worldChunk.CurrentChunkY,
                    wh, Path);
            }
            else if (worldChunk.Hero.Position.X > worldChunk.HitMap.Size-2)
            {
                Hero wh = (Hero)worldChunk.Hero;

                wh.Position = new Vector2(4, wh.Position.Y);

                worldChunk.Objects.Remove(wh);
                worldChunk.Save(Path);

                worldChunk = new WorldChunk(contentManager, worldChunk.CurrentChunkX + 1, worldChunk.CurrentChunkY,
                    wh, Path);
            }
            else if (worldChunk.Hero.Position.Y<3)
            {
                Hero wh = (Hero)worldChunk.Hero;

                wh.Position = new Vector2(wh.Position.X, worldChunk.HitMap.Size-3);

                worldChunk.Objects.Remove(wh);
                worldChunk.Save(Path);

                worldChunk = new WorldChunk(contentManager, worldChunk.CurrentChunkX, worldChunk.CurrentChunkY-1,
                    wh, Path);
            }
            else if (worldChunk.Hero.Position.Y > worldChunk.HitMap.Size - 2)
            {
                Hero wh = (Hero)worldChunk.Hero;

                wh.Position = new Vector2(wh.Position.X, 4);

                worldChunk.Objects.Remove(wh);
                worldChunk.Save(Path);

                worldChunk = new WorldChunk(contentManager, worldChunk.CurrentChunkY, worldChunk.CurrentChunkY+1,
                    wh, Path);
            }

            if(px!=worldChunk.CurrentChunkX||py!=worldChunk.CurrentChunkY)
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
