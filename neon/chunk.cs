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
using System.Threading;

namespace neon
{
    public class World
    {
        public HitboxFabricator WorldHitboxFabricator;
        public const int WorldSize = 512;
        public int KillCount = 0;
        private SpriteFont mainFont;

        public int CurrentChunkX { get; private set; }
        public int CurrentChunkY { get; private set; }

        public int ScreenX { get; set; } = 0;
        public int ScreenY { get; set; } = 0;

        public const int UnitSize = 9;

        public List<MapObject> Objects { get; set; }
        public LovelyChunk HitMap { get; protected set; }

        private Texture2D pxl;
        private Texture2D sand;
        public MapObject Hero { get; protected set; }
        private bool HitInspect = false, ChunkBorders=false, CurrentlyLoading=false;

        public string Path { get; private set; }

        private int timeSincef7 = 0, timeSincef8 = 0;

        public World(ContentManager contentManager, string path)
        {
            WorldHitboxFabricator = new HitboxFabricator();

            CurrentChunkX = -1;
            CurrentChunkY = -1;

            sand = contentManager.Load<Texture2D>("sand");
            pxl = contentManager.Load<Texture2D>("pxl");
            mainFont = contentManager.Load<SpriteFont>("File");

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

            CurrentChunkX += xmovage;
            CurrentChunkY += ymovage;

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

            if (!CurrentlyLoading&&xmov != 0 || ymov != 0)
            {
                CurrentlyLoading = true;

                int qx = ((int)(Hero.Position.X * UnitSize) + ScreenX - 960);
                int qy = ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540);

                SaveDelete(xmov, ymov, contentManager);
              
                int qn = ScreenX - ((int)(Hero.Position.X * UnitSize) + ScreenX - 960);
                int qm = ScreenY - ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540);

                ScreenX = qn+qx;
                ScreenY = qm+qy;

                CurrentlyLoading = false;
            }

            ScreenX -= (int)((Hero.Position.X * UnitSize) + ScreenX - 960 + Hero.Movement.X * 600) / 12;
            ScreenY -= (int)((Hero.Position.Y * UnitSize) + ScreenY - 540 + Hero.Movement.Y * 337) / 12;

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

            timeSincef7++;

            if (ks.IsKeyDown(Keys.F7)&&timeSincef7>=15)
            {
                timeSincef7 = 0;

                if (HitInspect)
                    HitInspect = false;
                else
                    HitInspect = true;
            }

            timeSincef8++;

            if (ks.IsKeyDown(Keys.F8) && timeSincef8 >= 10)
            {
                timeSincef8 = 0;

                if (ChunkBorders)
                    ChunkBorders = false;
                else
                    ChunkBorders = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int offX = -(Math.Abs(ScreenX) % (sand.Width*Game1.PixelScale));
            int offY = -(Math.Abs(ScreenY) % (sand.Height * Game1.PixelScale));

            for (int i = offX; i < 1920; i += Game1.PixelScale * sand.Width)
                for (int j = offY; j < 1080; j += Game1.PixelScale * sand.Height)
                    spriteBatch.Draw(sand, new Vector2(i, j), null, Color.White,
                        0f, new Vector2(0,0), Game1.PixelScale, SpriteEffects.None, 0f);

            float dpt = 0.1f;
            float dptStep = 0.5f / Objects.Count;

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Draw(spriteBatch, ScreenX + (int)(Objects[i].Position.X * UnitSize), ScreenY + (int)(Objects[i].Position.Y * UnitSize),
                    Color.White, dpt);

                dpt += dptStep;
            }

            if (HitInspect)
            {
                for (int i = 0; i < HitMap.Size * 2; i++)
                    for (int j = 0; j < HitMap.Size * 2; j++)
                    {
                        var vl = HitMap.GetValue(i, j);

                        if (vl.Count > 0)                        {
                            spriteBatch.Draw(pxl, new Vector2(ScreenX + i * UnitSize, ScreenY + j * UnitSize),
                                null, Microsoft.Xna.Framework.Color.Tan, 0f, new Vector2(0, 0),
                                UnitSize, SpriteEffects.None, 0f);
                        }
                    }
            }

            if(ChunkBorders)
            {
                spriteBatch.Draw(pxl,
                    new Vector2(WorldSize * UnitSize / 3 + ScreenX, 0 + ScreenY), null, Color.Blue, 0f, 
                    new Vector2(0, 0), new Vector2(2, WorldSize*UnitSize), SpriteEffects.None, 1f);

                spriteBatch.Draw(pxl,
                    new Vector2(WorldSize * UnitSize / 3*2 + ScreenX, 0 + ScreenY), null, Color.Blue, 0f,
                    new Vector2(0, 0), new Vector2(2, WorldSize * UnitSize), SpriteEffects.None, 1f);

                spriteBatch.Draw(pxl,
                    new Vector2(0 + ScreenX, WorldSize * UnitSize / 3 + ScreenY), null, Color.Blue, 0f,
                    new Vector2(0, 0), new Vector2(WorldSize * UnitSize, 2), SpriteEffects.None, 1f);

                spriteBatch.Draw(pxl,
                    new Vector2(0 + ScreenX, WorldSize * UnitSize / 3*2 + ScreenY), null, Color.Blue, 0f,
                    new Vector2(0, 0), new Vector2(WorldSize * UnitSize, 2), SpriteEffects.None, 1f);
            }

            spriteBatch.DrawString(mainFont, KillCount.ToString(),
                new Vector2(3, 3), Color.Black, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, 0.999f);

            spriteBatch.DrawString(mainFont, KillCount.ToString(), 
                new Vector2(1, 1), Color.White, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, 1f);
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
            
            int biome = new BiomeReader().GetBiome(world.CurrentChunkX+xRelative, world.CurrentChunkY+yRelative,
                world.Path+"biomes\\");

            if (biome == 0)
            {
                int rockCount = rnd.Next(4, 10);

                for (int i = 0; i < rockCount; i++)
                    world.Objects.Add(new Rock(contentManager,
                        xOffset + (float)rnd.NextDouble() * chunkSize,
                        yOffset + (float)rnd.NextDouble() * chunkSize, world, 2));
            }
            else if(biome==1)
            {
                int BoabCount = rnd.Next(3, 10);

                for(int i=0; i<BoabCount; i++)
                {
                    int vl = rnd.Next(0, 4);

                    MapObject boab = new Boab(contentManager, 
                        xOffset + (float)rnd.NextDouble() * chunkSize,
                        yOffset + (float)rnd.NextDouble() * chunkSize, world, vl);

                    if (boab.HitboxClear(world))
                        world.Objects.Add(boab);
                }

                if (rnd.Next(0, 3)==0)
                {
                    MapObject bigboab = new Boab(contentManager,
                        xOffset + (float)rnd.NextDouble() * chunkSize,
                        yOffset + (float)rnd.NextDouble() * chunkSize, world, 6);

                    if (bigboab.HitboxClear(world))
                        world.Objects.Add(bigboab);
                }

                int thorns = rnd.Next(2, 6);

                for(int i=0; i<thorns; i++)
                {
                    int q = rnd.Next(5, 10);
                    float centX = xOffset + (float)rnd.NextDouble() * chunkSize;
                    float centY = yOffset + (float)rnd.NextDouble() * chunkSize;
                    double xrad = 0;

                    for (int j=0; j<q; j++)
                    {
                        xrad += rnd.NextDouble()*5 + 1;
                        double rot = rnd.Next(0, 24) * Math.PI / 12;

                        MapObject grASS = new ThornGrass(contentManager,
                            centX + (float)(Math.Cos(rot) * xrad),
                            centY + (float)(Math.Sin(rot) * xrad),
                            world, rnd.Next(0, 2));

                        if (world.HitMap.GetValue((int)grASS.Position.X, (int)grASS.Position.Y).Count<1)
                            world.Objects.Add(grASS);
                    }
                }
            }
            else if (biome == 2)
            {
                int rockCount = rnd.Next(1, 16);

                for (int i = 0; i < rockCount; i++)
                    world.Objects.Add(new Rock(contentManager,
                        xOffset + (float)rnd.NextDouble() * chunkSize,
                        yOffset + (float)rnd.NextDouble() * chunkSize, world, 1));
            }
            else if (biome == 3)
            {
                int pikeCount = rnd.Next(5, 13);

                for (int i = 0; i < pikeCount; i++)
                    world.Objects.Add(new Spike(contentManager,
                        xOffset + (float)rnd.NextDouble() * chunkSize,
                        yOffset + (float)rnd.NextDouble() * chunkSize, world, 1));
            }
        }

        public void SaveDelete(string path, int xRelative, int yRelative, World world)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string str = "";
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            for (int i = 0; i < world.Objects.Count; i++)
            {
                if (!(world.Objects[i] is Bullet)&&
                    world.Objects[i].Position.X >= xRelative * (float)World.WorldSize / 3 &&
                    world.Objects[i].Position.X < (xRelative + 1) * (float)World.WorldSize / 3 &&
                    world.Objects[i].Position.Y >= yRelative * (float)World.WorldSize / 3 &&
                    world.Objects[i].Position.Y < (yRelative + 1) * (float)World.WorldSize / 3)
                {
                    world.Objects[i].Position = new Vector2(
                        world.Objects[i].Position.X - xRelative * World.WorldSize / 3,
                        world.Objects[i].Position.Y - yRelative * World.WorldSize / 3);

                    str+=JsonConvert.SerializeObject(world.Objects[i], jsonSerializerSettings) + "#";

                    world.Objects.RemoveAt(i);
                    i--;
                }
            }

            using (StreamWriter sw = new StreamWriter(path + (xRelative + world.CurrentChunkX).ToString()
                + "_" + (yRelative + world.CurrentChunkY).ToString()))
            {
                sw.WriteLine(str);
            }
        }
    }
}