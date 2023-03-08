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

namespace neon
{
    public abstract class MapObject
    {
        [JsonProperty("tname")]
        protected string TextureName = "";
        [JsonProperty("al")]
        public bool Alive { get; protected set; } = true;

        //objects with different collision levels would ignore each other
        /*CollisionLevel vocab
         * 0-ground
         * 1-sky
         * 2-weapons, items*/
        [JsonProperty("coll")]
        public int CollsionLevel { get; private set; }
        [JsonIgnore]
        public List<Vector2> Hitbox { get; protected set; } = null;
        [JsonProperty("pth")]
        protected string HitboxPath=null;

        [JsonProperty("mix")]
        public float HitboxMinX { get; protected set; }
        [JsonProperty("max")]
        public float HitboxMaxX { get; protected set; }
        [JsonProperty("miy")]
        public float HitboxMinY { get; protected set; }
        [JsonProperty("may")]
        public float HitboxMaxY { get; protected set; }

        [JsonProperty("wgh")]
        public float Weight { get; protected set; }

        [JsonProperty("pos")]
        public Vector2 Position { get; set; }
        
        [JsonProperty("mov")]
        public Vector2 Movement { get; protected set; }

        [JsonIgnore]
        public DynamicTexture Texture { get; protected set; } = null;
        [JsonIgnore]
        public bool HitboxPut = false;

        [JsonConstructor]
        public MapObject()
        {}

        public MapObject(ContentManager contentManager,
            Vector2 position, Vector2 movement, float weight, string hitboxPath,
            string textureName, int collisionLevel, World world)
        {
            CollsionLevel = collisionLevel;

            Position = position;
            Movement = movement;

            HitboxPath = hitboxPath;

            if (HitboxPath != null)
                Hitbox = world.WorldHitboxFabricator.CreateHitbox(HitboxPath);
            else
                Hitbox = new List<Vector2>();

            HitboxMaxX = -100000;
            HitboxMaxY = -100000;
            HitboxMinY = 100000;
            HitboxMinX = 100000;

            for (int i=0; i<Hitbox.Count; i++)
            {
                HitboxMaxX = Math.Max(HitboxMaxX, Hitbox[i].X);
                HitboxMaxY = Math.Max(HitboxMaxY, Hitbox[i].Y);
                HitboxMinX = Math.Min(HitboxMinX, Hitbox[i].X);
                HitboxMinY = Math.Min(HitboxMinY, Hitbox[i].Y);
            }

            Weight = weight;

            TextureName = textureName;
        }

        public virtual void Update(ContentManager contentManager, World world)
        {
            if (Hitbox == null)
            {
                if (HitboxPath != null)
                    Hitbox = world.WorldHitboxFabricator.CreateHitbox(HitboxPath);
                else
                    Hitbox = new List<Vector2>();
            }

            if (Texture == null)
                Texture = new DynamicTexture(contentManager, TextureName);

            Texture.Update(contentManager);

            Vector2 ppos = new Vector2(Position.X, Position.Y);

            //Position = new Vector2((float)Math.Round(Position.X, 7), (float)Math.Round(Position.Y, 7));
            //Movement = new Vector2((float)Math.Round(Movement.X, 7), (float)Math.Round(Movement.Y, 7));

            Position = new Vector2(Position.X + Movement.X, Position.Y);

            if (ppos.X != Position.X && !HitboxClear(world))
            {
                Position = new Vector2(Position.X - Movement.X, Position.Y);
                Movement = new Vector2(0, Movement.Y);
            }

            Position = new Vector2(Position.X, Position.Y + Movement.Y);

            if (ppos.Y != Position.Y && !HitboxClear(world))
            {
                Position = new Vector2(Position.X, Position.Y - Movement.Y);
                Movement = new Vector2(Movement.X, 0);
            }

            if (ppos.X != Position.X || ppos.Y != Position.Y)
            {
                Vector2 p2 = Position;
                Position = ppos;
                world.DeleteFromGrid(this);
                Position = p2;
                world.AddToGrid(this);
            }

            ChangeMovement(-Movement.X, -Movement.Y);
        }

        public virtual void Draw(SpriteBatch spriteBatch, int x, int y, Color color, float depth)
        {
            Texture2D whatToDraw = Texture.GetCurrentFrame();

            spriteBatch.Draw(whatToDraw, 
                new Vector2(x - whatToDraw.Width * Game1.PixelScale / 2, y - whatToDraw.Height * Game1.PixelScale),
                null, color, 0f,
                new Vector2(0, 0), Game1.PixelScale, SpriteEffects.None, depth);
        }

        public virtual void DrawHitbox(SpriteBatch spriteBatch, int x, int y, Color color, float depth, World world)
        {
            for (int i = 0; i < Hitbox.Count; i++)
            {
                Vector2 v1 = Hitbox[i];
                Vector2 v2 = Hitbox[(i+1)%Hitbox.Count];

                float rot = Game1.GetDirection(v1, v2)+(float)Math.PI;
                float scale = Game1.GetDistance(v1, v2)*World.UnitSize;

                spriteBatch.Draw(world.pxl, new Vector2(x + Hitbox[i].X * World.UnitSize, y + Hitbox[i].Y * World.UnitSize),
                    null, color, rot, new Vector2(0, 0), new Vector2(scale, 2), SpriteEffects.None, depth);
            }
        }

        //Fellow adventurer, from here on lies the realm of hitboxes
        //Once upon a time, it was a great kingdom of big yet elegant solutions 
        //The kingdom was prosperous, but from the beginning it had one great foe. Chunk transition.
        //So it fell.
        //It fell for the new kingdom to arise on it's ruins.
        //The kingdom you see here.
        public bool HitboxClear(World world)
        {
            var collisionChecker = new CollisionDetector();

            HashSet<MapObject> mo = new HashSet<MapObject>();

            for (float i = Math.Max(0, Position.X + HitboxMinX);
                i < Math.Min(World.WorldSize, Position.X + HitboxMaxX+ World.GridUnitSize); 
                i += World.GridUnitSize)
                for (float j = Math.Max(0, Position.Y + HitboxMinY);
                    j < Math.Min(World.WorldSize, Position.Y + HitboxMaxY+ World.GridUnitSize);
                    j += World.GridUnitSize)
                {
                    int tmpx = (int)Math.Floor(i / World.GridUnitSize), 
                        tmpy = (int)Math.Floor(j / World.GridUnitSize);

                    for (int k=0; k< world.objectGrid[tmpx, tmpy].Count; k++)
                        mo.Add(world.objectGrid[tmpx, tmpy][k]);
                }

            foreach (var curentObject in mo)
                if (curentObject.CollsionLevel == CollsionLevel && curentObject != this &&
                    collisionChecker.ObjectsCollide(this, curentObject))
                    return false;

            return true;
        }

        protected HashSet<MapObject> HitboxObstructions(World world)
        {
            HashSet<MapObject> ans = new HashSet<MapObject>();

            var collisionChecker = new CollisionDetector();

            foreach (var curentObject in world.Objects)
                if (curentObject.CollsionLevel == CollsionLevel &&
                    collisionChecker.ObjectsCollide(this, curentObject))
                    ans.Add(curentObject);

            return ans;
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

        public int ComparePositionTo(MapObject mapObject)
        {
            if (Position.Y == mapObject.Position.Y)
                return Position.X.CompareTo(mapObject.Position.X);

            return Position.Y.CompareTo(mapObject.Position.Y);
        }
    }
}