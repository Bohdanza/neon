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

            int rockCount = rnd.Next(3, 7);

            if (biome == 0)
            {
                int grassCount = rnd.Next(25, 50);

                for (int i = 0; i < grassCount; i++)
                {
                    double x = rnd.Next(0, (int)(chunkSize)) + xOffset;
                    double y = rnd.Next(0, (int)(chunkSize)) + yOffset;

                    x += rnd.NextDouble();
                    y += rnd.NextDouble();

                    MapObject grass = new ThornGrass(contentManager, (float)x, (float)y, world, rnd.Next(0, 4));
                }
            }
            else if (biome == 1)
            {
                for (int i = 0; i < rockCount; i++)
                {
                    double x = rnd.Next(0, (int)(chunkSize / 7.6)) * 7.6 + xOffset;
                    double y = rnd.Next(0, (int)(chunkSize / 7.6)) * 7.6 + yOffset;

                    x += rnd.NextDouble() * 2.5 - 1.25;
                    y += rnd.NextDouble() * 3;

                    MapObject rock = new Rock(contentManager, (float)x, (float)y, world, rnd.Next(0, 4));

                    if (rock.HitboxClear(world))
                    {
                        world.AddObject(rock);
                    }
                }

                int GroveCount = rnd.Next(1, 3);

                for(int i=0; i<GroveCount; i++)
                {
                    int cx = rnd.Next((int)xOffset, (int)(xOffset + chunkSize));
                    int cy = rnd.Next((int)yOffset, (int)(yOffset + chunkSize));
                    int shroomCount = rnd.Next(10, 30);
                    double rad = 0;

                    for (int j = 0; j < shroomCount; j++)
                    {
                        rad += rnd.NextDouble() * 3+3;
                        double rot = rnd.NextDouble() * Math.PI * 2;
                        int type = rnd.Next(0, 7);

                        MapObject shroom = new Boab(contentManager, (float)(Math.Cos(rot) * rad + cx), 
                            (float)(Math.Sin(rot) * rad + cy),
                            world, type);

                        if (shroom.HitboxClear(world))
                            world.AddObject(shroom);
                    }
                }

                /*if (Game1.GetDistance(new Vector2(xRelative + world.CurrentChunkX, 
                    yRelative + world.CurrentChunkY),
                    new Vector2(0, 0)) > Math.Sqrt(2))
                {
                    int grassCount = rnd.Next(4, 8);

                    for (int i = 0; i < grassCount; i++)
                    {
                        MapObject greenMan = new ScaryLilGreenman(contentManager,
                            new Vector2((float)rnd.NextDouble() * chunkSize + xOffset,
                            (float)rnd.NextDouble() * chunkSize + yOffset), world);

                        if (greenMan.HitboxClear(world))
                            world.Objects.Add(greenMan);
                    }
                }*/
            }
            else if (biome == 2)
            {

               /* if (Game1.GetDistance(new Vector2(xRelative + world.CurrentChunkX, yRelative + world.CurrentChunkY),
                    new Vector2(0, 0)) > 2)
                {
                    int grassCount = rnd.Next(3, 6);    

                    for (int i = 0; i < grassCount; i++)
                    {   
                        MapObject greenMan = new ScaryLilGreenman(contentManager,
                            new Vector2((float)rnd.NextDouble() * chunkSize + xOffset,
                            (float)rnd.NextDouble() * chunkSize + yOffset), world);

                        if (greenMan.HitboxClear(world))
                            world.Objects.Add(greenMan);
                    }
                }*/ 

            }
            else if (biome == 3)
            {
                int pikeCount = rnd.Next(5, 13);

                for (int i = 0; i < pikeCount; i++)
                    world.Objects.Add(new Spike(contentManager,
                        xOffset + (float)rnd.NextDouble() * chunkSize,
                        yOffset + (float)rnd.NextDouble() * chunkSize, world, rnd.Next(0, 3)));
                
                int grassCount = rnd.Next(15, 30);

                for (int i = 0; i < grassCount; i++)
                {
                    double x = rnd.Next(0, (int)(chunkSize)) + xOffset;
                    double y = rnd.Next(0, (int)(chunkSize)) + yOffset;

                    x += rnd.NextDouble();
                    y += rnd.NextDouble();

                    MapObject grass = new ThornGrass(contentManager, (float)x, (float)y, world, rnd.Next(0, 4));
                }

                grassCount = rnd.Next(3, 6);

                for(int i=0; i<grassCount; i++)
                {
                    MapObject greenMan = new ScaryLilGreenman(contentManager,
                        new Vector2((float)rnd.NextDouble() * chunkSize + xOffset,
                        (float)rnd.NextDouble() * chunkSize + yOffset), world);

                    if (greenMan.HitboxClear(world))
                        world.Objects.Add(greenMan);
                }
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