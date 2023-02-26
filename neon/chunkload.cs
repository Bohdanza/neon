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

            for(int i=(int)xOffset; i<World.WorldSize/3+xOffset; i++)
                for (int j = (int)yOffset; j < World.WorldSize / 3 + yOffset; j++)
                {
                    int biome = world.BiomeReader.GetBiome(
                        world.CurrentChunkX*World.WorldSize/3 + i,
                        world.CurrentChunkY * World.WorldSize / 3 + j,
                        world.Path + "biomes\\");
                    
                    if(biome==1)
                    if(world.BiomeReader.GetBiome(
                        world.CurrentChunkX * World.WorldSize / 3 + i-1,
                        world.CurrentChunkY * World.WorldSize / 3 + j,
                        world.Path + "biomes\\")!=1||
                        world.BiomeReader.GetBiome(
                        world.CurrentChunkX * World.WorldSize / 3 + i+1,
                        world.CurrentChunkY * World.WorldSize / 3 + j,
                        world.Path + "biomes\\")!=1||
                        world.BiomeReader.GetBiome(
                        world.CurrentChunkX * World.WorldSize / 3 + i,
                        world.CurrentChunkY * World.WorldSize / 3 + j-1,
                        world.Path + "biomes\\")!=1||
                        world.BiomeReader.GetBiome(
                        world.CurrentChunkX * World.WorldSize / 3 + i,
                        world.CurrentChunkY * World.WorldSize / 3 + j+1,
                        world.Path + "biomes\\")!=1)
                    {
                        world.Objects.Add(new Boab(contentManager, i-xOffset, j-yOffset, world, rnd.Next(0, 7)));
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