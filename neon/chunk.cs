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
        public const int WorldSize = 360;
        public const float MinimalCollisionDistance = 0.0001f;
        public int KillCount = 0;
        private SpriteFont mainFont;

        public int CurrentChunkX { get; private set; }
        public int CurrentChunkY { get; private set; }

        public int ScreenX { get; set; } = 0;
        public int ScreenY { get; set; } = 0;

        public const int UnitSize = 16;
        public const int GridUnitSize = 6;

        public List<MapObject> Objects { get; private set; }
        public List<MapObject>[,] objectGrid { get; private set; }

        public Texture2D pxl { get; private set; }
        private Texture2D sand, dark;
        public MapObject Hero { get; protected set; }
        private bool HitInspect = false, ChunkBorders = false, CurrentlyLoading = false;

        public string Path { get; private set; }

        private int timeSincef7 = 0, timeSincef8 = 0;

        public World(ContentManager contentManager, string path)
        {
            WorldHitboxFabricator = new HitboxFabricator();
            objectGrid = new List<MapObject>[WorldSize / GridUnitSize+1, WorldSize / GridUnitSize+1];

            for (int i = 0; i <= WorldSize / GridUnitSize; i++)
                for (int j = 0; j <= WorldSize / GridUnitSize; j++)
                    objectGrid[i, j] = new List<MapObject>();

                    CurrentChunkX = -1;
            CurrentChunkY = -1;

            sand = contentManager.Load<Texture2D>("sand");
            pxl = contentManager.Load<Texture2D>("pxl");
            dark = contentManager.Load<Texture2D>("vnt");
            mainFont = contentManager.Load<SpriteFont>("File");

            if (path[path.Length - 1] != '\\')
                path += "\\";

            Path = path;

            if (File.Exists(Path + "coords"))
            {
                using (StreamReader sr = new StreamReader(Path + "coords"))
                {
                    List<string> lst = sr.ReadToEnd().Split('\n').ToList();

                    CurrentChunkX = int.Parse(lst[0]);
                    CurrentChunkY = int.Parse(lst[1]);
                }
            }

            Objects = new List<MapObject>();
            LoaderChunk loaderChunk = new LoaderChunk();

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    loaderChunk.FillChunk(i, j, this, contentManager);
                }

            foreach (var co in Objects)
                AddToGrid(co);

            if (Hero == null)
            {
                Objects.Add(new Hero(contentManager, WorldSize / 2, WorldSize / 2, this));

                SetHero(Objects[Objects.Count - 1]);
            }
        }

        public void AddObject(MapObject mapObject)
        {
            Objects.Add(mapObject);

            AddToGrid(mapObject);
        }

        public void DeleteFromGrid(MapObject mapObject)
        {
            float bgi = Math.Max(0, mapObject.Position.X + mapObject.HitboxMinX - GridUnitSize);
            float eni = Math.Min(World.WorldSize, mapObject.Position.X + mapObject.HitboxMaxX + GridUnitSize);
            float bgj = Math.Max(0, mapObject.Position.Y + mapObject.HitboxMinY - GridUnitSize);
            float enj = Math.Min(World.WorldSize, mapObject.Position.Y + mapObject.HitboxMaxY + GridUnitSize);

            for (float i = bgi; i < eni; i += GridUnitSize)
                for (float j = bgj; j < enj; j += GridUnitSize)
                    if (objectGrid[(int)Math.Floor((float)i / GridUnitSize),
                        (int)Math.Floor((float)j / GridUnitSize)].Contains(mapObject))
                        objectGrid[(int)Math.Floor((float)i / GridUnitSize),
                            (int)Math.Floor((float)j / GridUnitSize)].Remove(mapObject);
        }

        public void AddToGrid(MapObject mapObject)
        {
            float bgi = Math.Max(0, mapObject.Position.X + mapObject.HitboxMinX - GridUnitSize);
            float eni = Math.Min(World.WorldSize, mapObject.Position.X + mapObject.HitboxMaxX + GridUnitSize);
            float bgj = Math.Max(0, mapObject.Position.Y + mapObject.HitboxMinY - GridUnitSize);
            float enj = Math.Min(World.WorldSize, mapObject.Position.Y + mapObject.HitboxMaxY + GridUnitSize);

            for (float i = bgi; i < eni; i += GridUnitSize)
                for (float j = bgj; j < enj; j += GridUnitSize)
                    if (!objectGrid[(int)Math.Floor((float)i / GridUnitSize),
                        (int)Math.Floor((float)j / GridUnitSize)].Contains(mapObject))
                        objectGrid[(int)Math.Floor((float)i / GridUnitSize),
                            (int)Math.Floor((float)j / GridUnitSize)].Add(mapObject);
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

            Objects.Remove(Hero);

            if (xmovage != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    loaderChunk.SaveDelete(Path, 1 - xmovage, i, this);
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
                DeleteFromGrid(currentObject);

                currentObject.Position = new Vector2(
                    currentObject.Position.X - xmovage * (float)WorldSize / 3,
                    currentObject.Position.Y - ymovage * (float)WorldSize / 3);

                AddToGrid(currentObject);
            }

            CurrentChunkX += xmovage;
            CurrentChunkY += ymovage;

            if (xmovage != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    loaderChunk.FillChunk(1 + xmovage, i, this, contentManager);
                }
            }

            if (ymovage != 0)
            {
                for (int i = 0 - Math.Min(0, xmovage); i < 3 - Math.Max(0, xmovage); i++)
                {
                    loaderChunk.FillChunk(i, 1 + ymovage, this, contentManager);
                }
            }

            Hero.Position = new Vector2(
                    Hero.Position.X - xmovage * (float)WorldSize / 3,
                    Hero.Position.Y - ymovage * (float)WorldSize / 3);

            AddObject(Hero);
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

            if (!CurrentlyLoading && xmov != 0 || ymov != 0)
            {
                CurrentlyLoading = true;

                int qx = ((int)(Hero.Position.X * UnitSize) + ScreenX - 960);
                int qy = ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540);

                SaveDelete(xmov, ymov, contentManager);

                int qn = ScreenX - ((int)(Hero.Position.X * UnitSize) + ScreenX - 960);
                int qm = ScreenY - ((int)(Hero.Position.Y * UnitSize) + ScreenY - 540);

                ScreenX = qn + qx;
                ScreenY = qm + qy;

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

            if (ks.IsKeyDown(Keys.F7) && timeSincef7 >= 15)
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
            int offX = -(Math.Abs(ScreenX) % (sand.Width * Game1.PixelScale));
            int offY = -(Math.Abs(ScreenY) % (sand.Height * Game1.PixelScale));

            for (int i = offX; i < 1920; i += Game1.PixelScale * sand.Width)
                 for (int j = offY; j < 1080; j += Game1.PixelScale * sand.Height)
                     spriteBatch.Draw(sand, new Vector2(i, j), null, Color.White,
                         0f, new Vector2(0,0), Game1.PixelScale, SpriteEffects.None, 0f);
            

            int darkX = (int)(Hero.Position.X * UnitSize + ScreenX)-dark.Width/2;
            int darkY = (int)(Hero.Position.Y * UnitSize + ScreenY)-dark.Height/2;
             
            spriteBatch.Draw(dark, new Vector2(darkX, darkY), null, Color.White, 0f, 
                new Vector2(0,0), 1f, SpriteEffects.None, 0.95f);

            spriteBatch.Draw(dark, new Vector2(darkX, 0), new Rectangle(0, 0, dark.Width, 1), 
                Color.White, 0f, new Vector2(0, 0),
                new Vector2(1, darkY), SpriteEffects.None, 0.95f);

            spriteBatch.Draw(dark, new Vector2(darkX, darkY+dark.Height),
                new Rectangle(0, dark.Height-1, dark.Width, 1),
                Color.White, 0f, new Vector2(0, 0),
                new Vector2(1, 1080-darkY-dark.Height), SpriteEffects.None, 0.95f);

            spriteBatch.Draw(dark, new Vector2(0, darkY), new Rectangle(0, 0, 1, dark.Height),
                Color.White, 0f, new Vector2(0, 0),
                new Vector2(darkX, 1), SpriteEffects.None, 0.95f);

            spriteBatch.Draw(dark, new Vector2(darkX+dark.Width, darkY), 
                new Rectangle(dark.Width-1, 0, 1, dark.Height),
                Color.White, 0f, new Vector2(0, 0),
                new Vector2(1920-darkX-dark.Width, 1), SpriteEffects.None, 0.95f);


            spriteBatch.Draw(dark, new Vector2(0, 0),
                new Rectangle(0, 0, 1, 1),
                Color.White, 0f, new Vector2(0, 0),
                new Vector2(darkX, darkY),
                SpriteEffects.None, 0.95f);

            spriteBatch.Draw(dark, new Vector2(darkX + dark.Width, 0),
                new Rectangle(dark.Width - 1, 0, 1, 1),
                Color.White, 0f, new Vector2(0, 0),
                new Vector2(1920 - darkX - dark.Width, darkY),
                SpriteEffects.None, 0.95f);
            
            float dpt = 0.1f;
            float dptStep = 0.5f / Objects.Count;

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Draw(spriteBatch, ScreenX + (int)(Objects[i].Position.X * UnitSize), ScreenY + (int)(Objects[i].Position.Y * UnitSize),
                    new Color(255, 255, 255, 255), dpt);

                if (HitInspect)
                    Objects[i].DrawHitbox(spriteBatch,
                        ScreenX + (int)(Objects[i].Position.X * UnitSize), ScreenY + (int)(Objects[i].Position.Y * UnitSize),
                        Color.Lime, 1f, this);

                dpt += dptStep;
            }

            if (ChunkBorders)
            {
                for(int i=0; i<=WorldSize/GridUnitSize; i++)
                {
                    Color clr = Color.Yellow;

                    if (i == (int)WorldSize / 3 / GridUnitSize|| i == (int)WorldSize / 3*2 / GridUnitSize)
                        clr = Color.Blue;

                    spriteBatch.Draw(pxl,
                        new Vector2(0, i * UnitSize * GridUnitSize + ScreenY), null, clr, 0f,
                        new Vector2(0, 0), new Vector2(WorldSize * UnitSize, 2), SpriteEffects.None, 1f);

                    spriteBatch.Draw(pxl,
                        new Vector2(i*UnitSize*GridUnitSize + ScreenX, 0), null, clr, 0f,
                        new Vector2(0, 0), new Vector2(2, WorldSize * UnitSize), SpriteEffects.None, 1f);
                }
            }

            spriteBatch.DrawString(mainFont, KillCount.ToString(),
                new Vector2(3, 3), Color.Black, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, 0.999f);

            spriteBatch.DrawString(mainFont, KillCount.ToString(),
                new Vector2(1, 1), Color.White, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, 1f);

            spriteBatch.DrawString(mainFont, Objects.Count.ToString(),
                new Vector2(1920 - mainFont.MeasureString(Objects.Count.ToString()).X, 30), Color.White, 0f,
                new Vector2(0, 0), 1f, SpriteEffects.None, 1f);
        }

        public Vector2 GetScreenPosition(MapObject mapObject)
        {
            return new Vector2(mapObject.Position.X * UnitSize + ScreenX, mapObject.Position.Y * UnitSize + ScreenY);
        }
    }
}