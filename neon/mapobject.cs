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
    public abstract class MapObject
    {
        //objects with different collision levels would ignore each other
        public int CollsionLevel { get; private set; }
        protected List<Tuple<int, int>> Hitbox;

        public float Weight { get; protected set; }
        
        public Vector2 Position { get; protected set; }
        public Vector2 Movement { get; protected set; }
        
        public DynamicTexture Texture { get; private set; }

        public MapObject(ContentManager contentManager, 
            Vector2 position, Vector2 movement, float weight, List<Tuple<int, int>> hitbox, 
            string textureName, int collisionLevel)
        {
            CollsionLevel = collisionLevel;
            
            Position = position;
            Movement = movement;

            Hitbox = hitbox;
            Weight = weight;

            Texture = new DynamicTexture(contentManager, textureName);
        }

        public virtual void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            Texture.Update(contentManager);

            EraseHitbox(worldChunk);

            Position = new Vector2(Position.X + Movement.X, Position.Y);
        
            if(!HitboxClear(worldChunk))
            {
                Position = new Vector2(Position.X - Movement.X, Position.Y);
            }

            Position = new Vector2(Position.X, Position.Y+Movement.Y);

            if (!HitboxClear(worldChunk))
            {
                Position = new Vector2(Position.X, Position.Y-Movement.Y);
            }

            PutHitbox(worldChunk);

            ChangeMovement(-Movement.X, -Movement.Y);
        }

        public virtual void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();

            spriteBatch.Draw(whatToDraw, new Vector2(x - whatToDraw.Width / 2, y - whatToDraw.Height), null, color, 0f, 
                new Vector2(0, 0), 1f, SpriteEffects.None, depth);
        }

        //Fellow adventurer, from here on lies the realm of hitboxes
        //In which nothing should be changed
        protected bool HitboxClear(WorldChunk worldChunk)
        {
            LovelyChunk chunk = worldChunk.HitMap;
            int x1 = (int)Position.X, y1 = (int)Position.Y;

            foreach (var currentHitPoint in Hitbox)
            {
                int x2 = x1 + currentHitPoint.Item1;
                int y2 = y1 + currentHitPoint.Item2;

                if (x2 < 0 || y2 < 0 || x2 >= chunk.Size || y2 >= chunk.Size)
                    return false;

                List<MapObject> mpo = chunk.GetValue(x2, y2);

                for (int i = 0; i < mpo.Count; i++)
                    if (mpo[i] != this && mpo[i].CollsionLevel == CollsionLevel)
                        return false;
            }

            return true;
        }
        
        protected void EraseHitbox(WorldChunk worldChunk)
        {
            LovelyChunk chunk = worldChunk.HitMap;
            int x1 = (int)Position.X, y1 = (int)Position.Y;

            foreach (var currentHitPoint in Hitbox)
            {
                int x2 = x1 + currentHitPoint.Item1;
                int y2 = y1 + currentHitPoint.Item2;

                if (x2 >= 0 && y2 >= 0 && x2 < chunk.Size && y2 < chunk.Size)
                {
                    chunk.RemoveObjectValue(x2, y2, this);
                }
            }
        }

        protected void PutHitbox(WorldChunk worldChunk)
        {
            LovelyChunk chunk = worldChunk.HitMap;
            int x1 = (int)Position.X, y1 = (int)Position.Y;

            foreach (var currentHitPoint in Hitbox)
            {
                int x2 = x1 + currentHitPoint.Item1;
                int y2 = y1 + currentHitPoint.Item2;

                if (x2 >= 0 && y2 >= 0 && x2 < chunk.Size && y2 < chunk.Size)
                {
                    chunk.AddObjectValue(x2, y2, this);
                }
            }
        }

        //And here we have movement
        public void SetMovement(float x, float y)
        {
            Movement = new Vector2(x, y);
        }

        /// <summary>
        /// Adds x to Movement.X and y to Movement.Y counting the weight
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ChangeMovement(float x, float y)
        {
            if (Weight != 0)
            {
                x /= Weight;
                y /= Weight;
            }

            Movement = new Vector2(Movement.X + x, Movement.Y + y);
        }

        /// <summary>
        /// Adds add.X to Movement.X and add.Y to Movement.Y counting the weight
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ChangeMovement(Vector2 add)
        {
            float x = add.X;
            float y = add.Y;

            if (Weight != 0)
            {
                x /= Weight;
                y /= Weight;
            }

            Movement = new Vector2(Movement.X + x, Movement.Y + y);
        }
    }
}