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
        private Texture2D sand;
        public MapObject Hero { get; protected set; }
        private bool HitInspect = false;

        public WorldChunk(ContentManager contentManager)
        {
            sand = contentManager.Load<Texture2D>("sand");
            
            var rnd = new Random();

            Objects = new List<MapObject>();
            HitMap = new LovelyChunk(256);

            Objects.Add(new Hero(contentManager, HitMap.Size/2, HitMap.Size/2, this));
            Hero = Objects[0];

            Objects.Add(new Tersol(contentManager, new Vector2(HitMap.Size / 2 + 10, HitMap.Size / 2),
                this));

            for (int i = 2; i < HitMap.Size; i+=4)
                for (int j = 2; j < HitMap.Size; j += 4)
                {
                    if (Game1.GetDistance(HitMap.Size / 2, HitMap.Size / 2, i, j) >= 
                        rnd.Next(HitMap.Size/2 - HitMap.Size/10, HitMap.Size/2-2))
                            Objects.Add(new Spike(contentManager, i + (float)(rnd.NextDouble() * 2f)-1f,
                            j + (float)(rnd.NextDouble() * 2f) - 1f, this));
                }

            pxl = contentManager.Load<Texture2D>("pxl");
        }

        public void AddObject(MapObject mapObject)
        {
            int l = 0, r = Objects.Count-1;

        }

        public void Update(ContentManager contentManager)
        {
            //  ScreenX -= ((int)(Hero.Position.X * UnitSize)+ScreenX-960) / 32;
            //  ScreenY -= ((int)(Hero.Position.Y * UnitSize)+ScreenY-540) / 18;
            
            ScreenX = -(int)(Hero.Position.X * UnitSize - 960);
            ScreenY = -(int)(Hero.Position.Y * UnitSize - 540);

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
                for (int i = 0; i < HitMap.Size; i++)
                    for (int j = 0; j < HitMap.Size; j++)
                    {
                        var vl = HitMap.GetValue(i, j);

                        if (vl.Count > 0)
                        {
                            spriteBatch.Draw(pxl, new Vector2(ScreenX + i * UnitSize, ScreenY+ j * UnitSize),
                                null, Color.Green, 0f, new Vector2(0, 0),
                                UnitSize*2, SpriteEffects.None, 0f);
                        }
                    }
            }
        }

        public Vector2 GetScreenPosition(MapObject mapObject)
        {
            return new Vector2(mapObject.Position.X * UnitSize + ScreenX, mapObject.Position.Y*UnitSize + ScreenY);
        }
    }
}