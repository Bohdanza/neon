using Microsoft.Xna.Framework;
using System;

namespace neon
{
    public class CollisionDetector
    {
        public CollisionDetector() { }

        public bool ObjectsCollide(MapObject mapObject1, MapObject mapObject2)
        {
            if (mapObject1.Hitbox == null || mapObject2.Hitbox == null ||
                mapObject1.Hitbox.Count < 3 || mapObject2.Hitbox.Count < 3)
                return false;

            for (int i = 0; i < mapObject1.Hitbox.Count; i++)
            {
                Vector2 v1 = mapObject1.Hitbox[i];
                Vector2 v2 = mapObject1.Hitbox[(i + 1) % mapObject1.Hitbox.Count];

                Vector2 minmax1 = MinMaxPos(Game1.GetDirection(v1, v2), mapObject1);
                Vector2 minmax2 = MinMaxPos(Game1.GetDirection(v1, v2), mapObject2);

                if (!((minmax1.X>minmax2.X&&minmax1.X<minmax2.Y)|| 
                    (minmax2.X > minmax1.X && minmax2.X < minmax1.Y)))
                    return false;
            }

            for (int i = 0; i < mapObject2.Hitbox.Count; i++)
            {
                Vector2 v1 = mapObject2.Hitbox[i];
                Vector2 v2 = mapObject2.Hitbox[(i + 1) % mapObject2.Hitbox.Count];

                Vector2 minmax1 = MinMaxPos(Game1.GetDirection(v1, v2), mapObject1);
                Vector2 minmax2 = MinMaxPos(Game1.GetDirection(v1, v2), mapObject2);
                
                if(!((minmax1.X > minmax2.X && minmax1.X < minmax2.Y) ||
                    (minmax2.X > minmax1.X && minmax2.X < minmax1.Y)))
                    return false;
            }

            return true;
        }

        private Vector2 MinMaxPos(float direction, MapObject mapObject)
        {
            float maxdist = 0, mindist = 100000f;

            for (int i = 0; i < mapObject.Hitbox.Count; i++)
            {
                float dist = (float)Math.Cos(direction -
                    Game1.GetDirection(new Vector2(0, 0), mapObject.Hitbox[i])) *
                    Game1.GetDistance(new Vector2(0, 0), mapObject.Hitbox[i]);

                maxdist = Math.Max(maxdist, dist);
                mindist = Math.Min(mindist, dist);
            }

            return new Vector2(mindist, maxdist);
        }
    }
}