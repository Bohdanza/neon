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
        public const int UnitSize = 15;

        public List<MapObject> Objects { get; private set; }
        public LovelyChunk HitMap { get; protected set; }

        public WorldChunk(ContentManager contentManager)
        {
            Objects = new List<MapObject>();
            HitMap = new LovelyChunk(1024);

            Objects.Add(new Hero(contentManager, 2f, 2f));
            Objects.Add(new Spike(contentManager, 15f, 15f));
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
                    Color.White, 0f);
            }
        }
    }
}