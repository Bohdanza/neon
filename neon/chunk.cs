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
    public class WorldChunk
    {
        public int CurrentChunkX { get; private set; }
        public int CurrentChunkY { get; private set; }

        public int ScreenX { get; protected set; } = 0;
        public int ScreenY { get; protected set; } = 0;

        public const int UnitSize = 8;

        public List<MapObject> Objects { get; private set; }
        public LovelyChunk HitMap { get; protected set; }

        private Texture2D pxl;
        private MapObject hero;

        public WorldChunk(ContentManager contentManager)
        {
            Objects = new List<MapObject>();
            HitMap = new LovelyChunk(256);

            Objects.Add(new Hero(contentManager, 2f, 2f, this));
            hero = Objects[0];

            var rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                Objects.Add(new Spike(contentManager, rnd.Next(0, 254)+0.5f, rnd.Next(0, 254)+0.5f, this));
            }

            pxl = contentManager.Load<Texture2D>("pxl");
        }

        public void AddObject(MapObject mapObject)
        {
            int l = 0, r = Objects.Count-1;

        }

        public void Update(ContentManager contentManager)
        {
            ScreenX -= ((int)(hero.Position.X * UnitSize)+ScreenX-960) / 32;
            ScreenY -= ((int)(hero.Position.Y * UnitSize)+ScreenY-540) / 18;

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

            Objects.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float dpt = 0f;
            float dptStep = 0.5f / Objects.Count;

            for (int i=0; i<Objects.Count; i++)
            {
                Objects[i].Draw(spriteBatch, ScreenX+(int)(Objects[i].Position.X * UnitSize), ScreenY+(int)(Objects[i].Position.Y * UnitSize),
                    Color.White, dpt);

                dpt += dptStep;
            }

            /*for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                {
                    var vl = HitMap.GetValue(i, j);

                    if(vl.Count>0)
                    {
                        spriteBatch.Draw(pxl, new Vector2(i * UnitSize, j * UnitSize), null, Color.Green, 0f, new Vector2(0, 0),
                            UnitSize, SpriteEffects.None, 0f);
                    }
                }*/
        }

        public Vector2 GetScreenPosition(MapObject mapObject)
        {
            return new Vector2(mapObject.Position.X * UnitSize + ScreenX, mapObject.Position.Y*UnitSize + ScreenY);
        }
    }
}