using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace neon
{
    public class LoaderChunk
    {
        public LoaderChunk()
        { }

        public void FillChunk(int xRelative, int yRelative, World world, ContentManager contentManager)
        {
            if (File.Exists(world.Path + (world.CurrentChunkX + xRelative).ToString() +
                "_" + (world.CurrentChunkY + yRelative).ToString()))
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

                mapObject.Position = new Vector2(mapObject.Position.X + xRelative * (float)World.WorldSize / 3,
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
                world.Path+"chunks\\");

            if (biome == 0)
            {
            }
            else if (biome == 1)
            {
                CherockRing(rnd.Next(3, 6), xOffset + chunkSize / 4, yOffset + chunkSize / 4, world, contentManager);

                int greenmancount = rnd.Next(3, 7);

                for(int i=0; i<greenmancount; i++)
                {
                    double tmpx = xOffset + rnd.NextDouble() * chunkSize;
                    double tmpy = yOffset + rnd.NextDouble() * chunkSize;

                    MapObject man = new Crawler(contentManager, new Vector2((float)tmpx, (float)tmpy), world);

                    if (man.HitboxClear(world))
                        world.AddObject(man);
                }
                
                int pathcount = rnd.Next(16, 25);

                for(int i=0; i<pathcount; i++)
                {
                    double tmpx = xOffset + rnd.Next(0, (int)Math.Floor(chunkSize/Game1.PixelScale))
                        *Game1.PixelScale;
                    double tmpy = yOffset + rnd.Next(0, (int)Math.Floor(chunkSize / Game1.PixelScale))
                        * Game1.PixelScale;

                    MapObject path = new GroundPath(contentManager, (float)tmpx, (float)tmpy, world, rnd.Next(0, 6));

                    world.AddObject(path);
                }
                
                pathcount = rnd.Next(10, 20);

                for (int i = 0; i < pathcount; i++) 
                {
                    double tmpx = xOffset + rnd.Next(0, (int)Math.Floor(chunkSize / Game1.PixelScale))
                        * Game1.PixelScale;
                    double tmpy = yOffset + rnd.Next(0, (int)Math.Floor(chunkSize / Game1.PixelScale))
                        * Game1.PixelScale;

                    MapObject path = new Weed(contentManager, (float)tmpx, (float)tmpy, world, rnd.Next(0, 5));
                     
                    world.AddObject(path);
                }
            }
            else if (biome == 2)
            { 
            }
            else if (biome == 3)
            {
            }
        }

        private void CherockRing(int ringCount, double cx, double cy, World world, ContentManager contentManager)
        {
            var rnd = new Random();
            double rad = 0;

            for (int j = 0; j < ringCount; j++)
            {
                int rockCount = j * 3 + 1;
                double crot = rnd.NextDouble() * (Math.PI * 2 / rockCount);

                for (int k = 0; k < rockCount; k++)
                {
                    int tp = 0;

                    /*if (j == 0)
                        tp = 1;
                    if (j == 1)
                        tp = rnd.Next(0, 2);
                    else if (j < ringCount / 2)
                        tp = 0;*/

                    double nrad = rad + rnd.NextDouble() * 10;

                    MapObject cher = new SpikeLight(contentManager,
                        (float)(cx + Math.Cos(crot) * nrad), (float)(cy + Math.Sin(crot) * nrad),
                        world, tp);

                    if (cher.HitboxClear(world))
                        world.AddObject(cher);

                    crot += rnd.NextDouble() * 0.5 + (Math.PI * 2 / rockCount) - 0.25;
                }

                rad += 2.5;
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
                if (!(world.Objects[i] is Bullet) &&
                    world.Objects[i].Position.X >= xRelative * (float)World.WorldSize / 3 &&
                    world.Objects[i].Position.X < (xRelative + 1) * (float)World.WorldSize / 3 &&
                    world.Objects[i].Position.Y >= yRelative * (float)World.WorldSize / 3 &&
                    world.Objects[i].Position.Y < (yRelative + 1) * (float)World.WorldSize / 3)
                {
                    world.Objects[i].Position = new Vector2(
                        world.Objects[i].Position.X - xRelative * World.WorldSize / 3,
                        world.Objects[i].Position.Y - yRelative * World.WorldSize / 3);

                    str += JsonConvert.SerializeObject(world.Objects[i], jsonSerializerSettings) + "#";

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