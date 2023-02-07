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

namespace neon
{
    public class WorldChunk
    {
        public int CurrentChunkX { get; private set; }
        public int CurrentChunkY { get; private set; }

        public int ScreenX { get; set; } = 0;
        public int ScreenY { get; set; } = 0;

        public const int UnitSize = 25;

        public List<MapObject> Objects { get; private set; }
        public LovelyChunk HitMap { get; protected set; }

        private Texture2D pxl;
        private Texture2D sand;
        public MapObject Hero { get; protected set; }
        private bool HitInspect = false;

        public int Biome { get; private set; }
        public int RoomType { get; private set; }

        public WorldChunk(ContentManager contentManager, int currentChunkX, int currentChunkY, 
            Hero hero, string path)
        {
            CurrentChunkX = currentChunkX;
            CurrentChunkY = currentChunkY;

            sand = contentManager.Load<Texture2D>("sand");
            pxl = contentManager.Load<Texture2D>("pxl");

            Objects = new List<MapObject>();

            Objects = new List<MapObject>();

            if(File.Exists(path+currentChunkX.ToString()+"_"+currentChunkY.ToString()))
            {
              //  try
              //  {
                    Load(contentManager, path + currentChunkX.ToString() + "_" + currentChunkY.ToString());
              //  }
              //  catch { }
            }
            else
                Generate(contentManager, hero);

            if(Hero==null)
            {
                Hero = hero;
                Objects.Add(Hero);
            }
        }

        public void Save(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (StreamWriter sw = new StreamWriter(path + CurrentChunkX.ToString() + "_" + CurrentChunkY.ToString()))
            {
                sw.WriteLine(Biome.ToString() + "#" + RoomType.ToString() + "#" + HitMap.Size.ToString() + "#");

                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                for (int i = 0; i < Objects.Count; i++)
                {
                    StringBuilder wstr=new StringBuilder(JsonConvert.SerializeObject(Objects[i], jsonSerializerSettings) + "#");
                    int bg = 0;
                    string check_for = "System.Private.CoreLib";

                    for (int j = 0; j < wstr.Length; j++)
                    {
                        if (j-bg<check_for.Length && wstr[j] != check_for[j - bg])
                            bg = j;
                        else if(j-bg==check_for.Length)
                        {
                            wstr[bg] = '~';
                            wstr.Remove(bg + 1, j - bg-1);
                            j = bg;
                        }
                    }

                    sw.WriteLine(wstr.ToString());
                }
            }
        }

        public void Load(ContentManager contentManager, string path)
        {
            List<string> data = new List<string>();

            using(StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd().Split('#').ToList();
            }

            Biome = Int32.Parse(data[0]);
            RoomType = Int32.Parse(data[1]);

            HitMap = new LovelyChunk(Int32.Parse(data[2]));

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.Objects;

            for (int i=3; i<data.Count-1; i++)
            {
                StringBuilder stb = new StringBuilder(data[i]);
                string check_for = "System.Private.CoreLib";

                for (int j=0; j<stb.Length; j++)
                {
                    if(stb[j]=='~')
                    {
                        stb.Remove(j, 1);
                        stb.Insert(j, check_for);
                    }
                }

                MapObject mapObject = (MapObject)JsonConvert.DeserializeObject<MapObject>(stb.ToString(), jss);

                Objects.Add(mapObject);

                if (Objects[i-3] is Hero)
                    this.Hero = Objects[i-3];
            }
        }

        public void Generate(ContentManager contentManager, Hero hero)
        {
            var rnd = new Random();

            Biome = rnd.Next(0, 1);
            RoomType = rnd.Next(0, 1);

            int sz = 0;

            if (RoomType == 0)
                sz = 256;
            else
                sz = 128;

            HitMap = new LovelyChunk(sz);

            if (hero == null)
                Objects.Add(new Hero(contentManager, HitMap.Size / 2, HitMap.Size / 2, this));
            else
                Objects.Add(hero);

            Hero = Objects[0];

            if (CurrentChunkX == 0 && CurrentChunkY == 0)
                Objects.Add(new Idol(contentManager, HitMap.Size / 2, 60f, this));

            Objects.Add(new Tersol(contentManager, new Vector2(HitMap.Size / 2 + 10, HitMap.Size / 2),
                this));

            for (int i = 2; i < HitMap.Size; i += 4)
                for (int j = 2; j < HitMap.Size; j += 4)
                {
                    if (Game1.GetDistance(HitMap.Size / 2, HitMap.Size / 2, i, j) >=
                        rnd.Next(HitMap.Size / 2 - 10, HitMap.Size / 2 - 2)
                        && i/4!=HitMap.Size/8 && j / 4 != HitMap.Size / 8)
                        Objects.Add(new Spike(contentManager, i + (float)(rnd.NextDouble() * 2f) - 1f,
                        j + (float)(rnd.NextDouble() * 2f) - 1f, this));
                }

            for(int i=0; i<100; i++)
            {
                Objects.Add(new Coin(contentManager, new Vector2((float)rnd.NextDouble() * HitMap.Size,
                    (float)rnd.NextDouble() * HitMap.Size), rnd.Next(0, 13), this));
            }
        }

        public void AddObject(MapObject mapObject)
        {
            int l = 0, r = Objects.Count-1;

        }

        public void Update(ContentManager contentManager)
        {
            ScreenX -= ((int)(Hero.Position.X * UnitSize)+ScreenX-960) / 16;
            ScreenY -= ((int)(Hero.Position.Y * UnitSize)+ScreenY-540) / 16;
            
            //ScreenX = -(int)(Hero.Position.X * UnitSize - 960);
            //ScreenY = -(int)(Hero.Position.Y * UnitSize - 540);

            ScreenX = Math.Min(0, ScreenX);
            ScreenY = Math.Min(0, ScreenY);

            ScreenX = Math.Max(-HitMap.Size * UnitSize+1920, ScreenX);
            ScreenY = Math.Max(-HitMap.Size * UnitSize+1080, ScreenY);

            int l = 1;

            for(int i=0; i<Objects.Count; i+=l)
            {
                l = 1;

                Objects[i].Update(contentManager, this);      
                
                if(!Objects[i].Alive)
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
                    spriteBatch.Draw(sand, new Vector2(i, j), Color.White);

            float dpt = 0.1f;
            float dptStep = 0.5f / Objects.Count;

            for (int i=0; i<Objects.Count; i++)
            {
                Objects[i].Draw(spriteBatch, ScreenX+(int)(Objects[i].Position.X * UnitSize), ScreenY+(int)(Objects[i].Position.Y * UnitSize),
                    Color.White, dpt);

                dpt += dptStep;
            }

            if (HitInspect)
            {
                for (int i = 0; i < HitMap.Size*2; i++)
                    for (int j = 0; j < HitMap.Size*2; j++)
                    {
                        var vl = HitMap.GetValue(i, j);

                        if (vl.Count > 0)
                        {
                            spriteBatch.Draw(pxl, new Vector2(ScreenX + i * UnitSize, ScreenY+ j * UnitSize),
                                null, Color.Green, 0f, new Vector2(0, 0),
                                UnitSize, SpriteEffects.None, 0f);
                        }
                    }
            }
        }

        public Vector2 GetScreenPosition(MapObject mapObject)
        {
            return new Vector2(mapObject.Position.X * UnitSize + ScreenX, mapObject.Position.Y*UnitSize + ScreenY);
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
}