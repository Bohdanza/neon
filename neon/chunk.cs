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
        public const int UnitSize = 12;

        public List<MapObject> Objects { get; private set; }
        public LovelyChunk HitMap { get; protected set; }

        private Texture2D pxl;

        public WorldChunk(ContentManager contentManager)
        {
            Objects = new List<MapObject>();
            HitMap = new LovelyChunk(256);

            Objects.Add(new Hero(contentManager, 2f, 2f, this));

            var rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                Objects.Add(new Spike(contentManager, rnd.Next(0, 100)+0.5f, rnd.Next(0, 100)+0.5f, this));
            }

            pxl = contentManager.Load<Texture2D>("pxl");
        }

        public void AddObject(MapObject mapObject)
        {
          
        }

        public void Update(ContentManager contentManager)
        {
            for(int i=0; i<Objects.Count; i++)
            {
                Objects[i].Update(contentManager, this);      
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i=0; i<Objects.Count; i++)
            {
                Objects[i].Draw(spriteBatch, (int)(Objects[i].Position.X * UnitSize), (int)(Objects[i].Position.Y * UnitSize),
                    Color.White, 1f);
            }

            for(int i=0; i<200; i++)
                for (int j = 0; j < 200; j++)
                {
                    var vl = HitMap.GetValue(i, j);

                    if(vl.Count>0)
                    {
                        spriteBatch.Draw(pxl, new Vector2(i * UnitSize, j * UnitSize), null, Color.Green, 0f, new Vector2(0, 0),
                            UnitSize, SpriteEffects.None, 0f);
                    }
                }
        }
    }
}