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
    public class CollisionGrid
    {
        private List<MapObject>[,] Empty;
        private Dictionary<int, List<MapObject>[,]> objectGrid;
        private int Width, Height;

        public CollisionGrid(int width, int height)
        {
            Width = width;
            Height = height;
            objectGrid = new Dictionary<int, List<MapObject>[,]>();

            Empty = new List<MapObject>[Width, Height];

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    Empty[i, j] = new List<MapObject>();
        }

        public void AddCollisionLayer(int collisionLevel)
        {
            if (objectGrid.ContainsKey(collisionLevel))
                return;

            List<MapObject>[,] lst = new List<MapObject>[Width, Height];

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    lst[i, j] = new List<MapObject>();

            objectGrid[collisionLevel] = lst;

            return;
        }

        public List<MapObject>[,] GetCollisionLayer(int collisionLevel)
        {
            if (!objectGrid.ContainsKey(collisionLevel))
                return Empty;

            return objectGrid[collisionLevel];
        }
    }
}