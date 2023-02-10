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
using Newtonsoft.Json;
using System.Text;
using System.Drawing;

namespace neon
{
    public class World
    {
        public const int WorldSize = 256;

        public int CurrentChunkX { get; private set; }
        public int CurrentChunkY { get; private set; }

        public int ScreenX { get; set; } = 0;
        public int ScreenY { get; set; } = 0;

        public const int UnitSize = 25;

        public List<MapObject> Objects { get; set; }
        public LovelyChunk HitMap { get; protected set; }

        private Texture2D pxl;
        private Texture2D sand;
        public MapObject Hero { get; protected set; }
        private bool HitInspect = false;

        public int Biome { get; private set; }
        public string Path { get; private set; }

        public World(ContentManager contentManager, string path)
        {
            CurrentChunkX = -1;
            CurrentChunkY = -1;

            sand = contentManager.Load<Texture2D>("sand");
            pxl = contentManager.Load<Texture2D>("pxl");

            if (path[path.Length-1]!='\\')
                path += "\\";

            Path = path;

            if(File.Exists(Path+"coords"))
            {
                using (StreamReader sr = new StreamReader(Path + "coords"))
                {
                    List<string> lst = sr.ReadToEnd().Split('\n').ToList();

                    CurrentChunkX = int.Parse(lst[0]);
                    CurrentChunkY = int.Parse(lst[1]);
                }
            }

            Objects = new List<MapObject>();
            HitMap = new LovelyChunk(WorldSize);
            LoaderChunk loaderChunk = new LoaderChunk();

            for(int i=0; i<3; i++)
                for(int j=0; j<3; j++)
                {
                    loaderChunk.FillChunk(i, j, this, contentManager);
                }

            if(Hero==null)
            {
                Objects.Add(new Hero(contentManager, WorldSize / 2, WorldSize / 2, this));

                SetHero(Objects[Objects.Count - 1]);
            }
        }

        public void SetHero(MapObject hero)
        {
            if (Hero == null && hero is Hero)
                Hero = hero;
        }

        /// <summary>
        /// Saves desired chunks and deletes them, then generates/loads new
        /// </summary>
        private void SaveDelete(int xmovage, int ymovage, ContentManager contentManager) 
        {
            LoaderChunk loaderChunk = new LoaderChunk();
            HitMap = new LovelyChunk(WorldSize);

            Objects.Remove(Hero);

            if(xmovage!=0)
            {
                for(int i=0; i<3; i++)
                {
                    loaderChunk.SaveDelete(Path, 1-xmovage, i, this);
                }
            }

            if (ymovage != 0)
            {
                for (int i = 0 - Math.Min(0, xmovage); i < 3 - Math.Max(0, xmovage); i++)
                {
                    loaderChunk.SaveDelete(Path, i, 1 - ymovage, this);
                }
            }

            foreach (var currentObject in Objects)
            {
                currentObject.Position = new Vector2(
                    currentObject.Position.X - xmovage * (float)WorldSize / 3,
                    currentObject.Position.Y - ymovage * (float)WorldSize / 3);

                currentObject.HitboxPut = false;
            }

            if (xmovage != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    loaderChunk.FillChunk(1+xmovage, i, this, contentManager);
                }
            }

            if (ymovage != 0)
            {
                for (int i = 0-Math.Min(0, xmovage); i < 3-Math.Max(0, xmovage); i++)
                {
                    loaderChunk.FillChunk(i, 1 + ymovage, this, contentManager);
                }
            }

            Objects.Add(Hero);

            Hero.Position = new Vector2(
                    Hero.Position.X - xmovage * (float)WorldSize / 3,
                    Hero.Position.Y - ymovage * (float)WorldSize / 3);
        }

        public void Save()
        {
            LoaderChunk loaderChunk = new LoaderChunk();

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    loaderChunk.SaveDelete(Path, i, j, this);
                }

            SaveCoords();
        }

        private void SaveCoords()
        {
            using (StreamWriter sw = new StreamWriter(Path + "coords"))
            {
                sw.WriteLine(CurrentChunkX.ToString() + "\n" + CurrentChunkY.ToString());
            }
        }

        public void Update(ContentManager contentManager)
        {
            int xmov = 0, ymov = 0;

            if (Hero.Position.X < (float)WorldSize / 3)
                xmov = -1;

            if (Hero.Position.X >= (float)WorldSize / 3 * 2)
                xmov = 1;

            if (Hero.Position.Y < (float)WorldSize / 3)
                ymov = -1;

            if (Hero.Position.Y >= (float)WorldSize / 3 * 2)
                ymov = 1;

            if (xmov != 0 || ymov != 0)
            {
                int qx = ((int)(Hero.Position.X * UnitSize) + ScreenX - 960);
                int qy = ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540);

                SaveDelete(xmov, ymov, contentManager);
                CurrentChunkX += xmov;
                CurrentChunkY += ymov;

                int qn = ScreenX - ((int)(Hero.Position.X * UnitSize) + ScreenX - 960);
                int qm = ScreenY - ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540);

                ScreenX = qn+qx;
                ScreenY = qm+qy;
            }

            ScreenX -= ((int)(Hero.Position.X * UnitSize) + ScreenX - 960) / 16;
            ScreenY -= ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540) / 16;

            int l = 1;

            for (int i = 0; i < Objects.Count; i += l)
            {
                l = 1;

                Objects[i].Update(contentManager, this);

                if (!Objects[i].Alive)
                {
                    Objects.RemoveAt(i);
                    l = 0;
                }
            }

            Objects.Sort((a, b) => a.ComparePositionTo(b));

            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.F7))
            {
                if (HitInspect)
                    HitInspect = false;
                else
                    HitInspect = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int offX = -(Math.Abs(ScreenX) % sand.Width);
            int offY = -(Math.Abs(ScreenY) % sand.Height);

            for (int i = offX; i < 1920; i += sand.Width)
                for (int j = offY; j < 1080; j += sand.Height)
                    spriteBatch.Draw(sand, new Vector2(i, j), Microsoft.Xna.Framework.Color.White);

            float dpt = 0.1f;
            float dptStep = 0.5f / Objects.Count;

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Draw(spriteBatch, ScreenX + (int)(Objects[i].Position.X * UnitSize), ScreenY + (int)(Objects[i].Position.Y * UnitSize),
                    Microsoft.Xna.Framework.Color.White, dpt);

                dpt += dptStep;
            }

            if (HitInspect)
            {
                for (int i = 0; i < HitMap.Size * 2; i++)
                    for (int j = 0; j < HitMap.Size * 2; j++)
                    {
                        var vl = HitMap.GetValue(i, j);

                        if (vl.Count > 0)
                        {
                            spriteBatch.Draw(pxl, new Vector2(ScreenX + i * UnitSize, ScreenY + j * UnitSize),
                                null, Microsoft.Xna.Framework.Color.Tan, 0f, new Vector2(0, 0),
                                UnitSize, SpriteEffects.None, 0f);
                        }
                    }
            }
        }

        public Vector2 GetScreenPosition(MapObject mapObject)
        {
            return new Vector2(mapObject.Position.X * UnitSize + ScreenX, mapObject.Position.Y * UnitSize + ScreenY);
        }
      
        public bool LineClear(int x1, int y1, int x2, int y2, int obstacleLevel)
        {
            float xc = x1, yc = y1;
            float stepx = x2 - x1, stepy = y2 - y1;
        
            if(stepx>stepy)
            {
                stepy = (float)(1 / stepx * stepy);
                stepx = 1;
            }

            if (stepy > stepx)
            {
                stepx = (float)(1 / stepy * stepx);
                stepy = 1;
            }

            while((int)xc!=x2||(int)yc!=y2)
            {
                var q = HitMap.GetValue((int)xc, (int)yc);

                foreach(MapObject mapObject in q)
                {
                    if (mapObject.CollsionLevel == obstacleLevel)
                        return false;
                }

                xc += stepx;
                yc += stepy;
            }

            return true;
        }
    }

    public class LoaderChunk
    {
        public LoaderChunk()
        { }

        public void FillChunk(int xRelative, int yRelative, World world, ContentManager contentManager)
        {
            if(File.Exists(world.Path+(world.CurrentChunkX+xRelative).ToString()+
                "_"+(world.CurrentChunkY+yRelative).ToString()))
            {
                Load(contentManager, world.Path + (world.CurrentChunkX + xRelative).ToString() +
                "_" + (world.CurrentChunkY + yRelative).ToString(), world, xRelative, yRelative);
            }
            else
            {
                Generate(contentManager, xRelative, yRelative, world);
            }
        }

        private void Load(ContentManager contentManager, string path, World world, int xRelative, int yRelative)
        {
            List<string> data = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd().Split('#').ToList();
            }

            //Biome = Int32.Parse(data[0]);

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.Objects;

            for (int i = 0; i < data.Count - 1; i++)
            {
                MapObject mapObject = JsonConvert.DeserializeObject<MapObject>(data[i], jss);

                mapObject.Position = new Vector2(mapObject.Position.X+ xRelative* (float)World.WorldSize/3, 
                    mapObject.Position.Y + yRelative * (float)World.WorldSize / 3);

                world.Objects.Add(mapObject);

                world.SetHero(mapObject);
            }
        }

        private void Generate(ContentManager contentManager, int xRelative, int yRelative, World world)
        {
            var rnd = new Random();

            float chunkSize = World.WorldSize / 3;
            float xOffset = chunkSize * xRelative;
            float yOffset = chunkSize * yRelative;

            //Biome = rnd.Next(0, 1);

            world.Objects.Add(new Tersol(contentManager, new Vector2(xOffset + chunkSize / 2 + 10, 
                yOffset + chunkSize / 2),
                world));

            int rockCount = rnd.Next(1, 4);

            for (int i = 0; i < rockCount; i++)
                world.Objects.Add(new Rock(contentManager,
                    xOffset + (float)rnd.NextDouble() * chunkSize,
                    yOffset + (float)rnd.NextDouble() * chunkSize, world, rnd.Next(0, 4))); 
        }

        public void SaveDelete(string path, int xRelative, int yRelative, World world)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (StreamWriter sw = new StreamWriter(path + (xRelative + world.CurrentChunkX).ToString()
                + "_" + (yRelative + world.CurrentChunkY).ToString()))
            {
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                for (int i = 0; i < world.Objects.Count; i++)
                {
                    if (world.Objects[i].Position.X >= xRelative * (float)World.WorldSize / 3 &&
                        world.Objects[i].Position.X < (xRelative + 1) * (float)World.WorldSize / 3 &&
                        world.Objects[i].Position.Y >= yRelative * (float)World.WorldSize / 3 &&
                        world.Objects[i].Position.Y < (yRelative + 1) * (float)World.WorldSize / 3)
                    {
                        world.Objects[i].Position = new Vector2(
                            world.Objects[i].Position.X - xRelative * World.WorldSize / 3,
                            world.Objects[i].Position.Y - yRelative * World.WorldSize / 3);

                        sw.WriteLine(JsonConvert.SerializeObject(world.Objects[i], jsonSerializerSettings) + "#");

                        world.Objects.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}