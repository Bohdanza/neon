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

/*
Children distribution 

       |
     2 | 3
   ----|----
     1 | 0
       |
 */

namespace neon
{
    public class LovelyChunk
    {
        public List<MapObject> Value { get; protected set; }
        
        public List<LovelyChunk> children { get; protected set; } = null;
        public int Size { get; private set; }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="size">Better be a 2^n number</param>
        public LovelyChunk(int size)
        {
            Size = size;
            Value = new List<MapObject>();
        }

        protected void TryReducing()
        {
            if (children != null)
            {
                /*children[0].TryReducing();
                children[1].TryReducing();
                children[2].TryReducing();
                children[3].TryReducing();
                */

                if (children[0].children == null && children[1].children == null &&
                    children[2].children == null && children[3].children == null &&
                    children[0].Value.SequenceEqual(children[1].Value) && 
                    children[0].Value.SequenceEqual(children[2].Value) &&
                    children[0].Value.SequenceEqual(children[3].Value))
                {
                    Value = children[0].Value;
                    children = null;
                }
            }
        }

        public void ChangeValue(int x, int y, List<MapObject> value)
        {
            if (children == null && Value == value)
                return;

            if (Size == 1)
            {
                Value = value;
            }
            else
            {
                if (children == null)
                {
                    children = new List<LovelyChunk>();

                    for (int i = 0; i < 4; i++)
                        children.Add(new LovelyChunk(Size / 2));
                }

                if (x >= Size/2 && y >= Size / 2)
                    children[0].ChangeValue(x - Size / 2, y - Size / 2, value);
                
                if (x < Size / 2 && y >= Size / 2)
                    children[1].ChangeValue(x, y - Size / 2, value);

                if (x < Size / 2 && y < Size / 2)
                    children[2].ChangeValue(x, y, value);

                if (x >= Size / 2 && y < Size / 2) 
                    children[3].ChangeValue(x - Size / 2, y, value);

                TryReducing();
            }
        }

        public void RemoveObjectValue(int x, int y, MapObject value)
        {
            if (children == null)
            {
                if(Value.Contains(value))
                    Value.Remove(value);
                
                return;
            }
            else
            {
                if (children == null)
                {
                    children = new List<LovelyChunk>();

                    for (int i = 0; i < 4; i++)
                        children.Add(new LovelyChunk(Size / 2));
                }

                if (x >= Size / 2 && y >= Size / 2)
                    children[0].RemoveObjectValue(x - Size / 2, y - Size / 2, value);

                if (x < Size / 2 && y >= Size / 2)
                    children[1].RemoveObjectValue(x, y - Size / 2, value);

                if (x < Size / 2 && y < Size / 2)
                    children[2].RemoveObjectValue(x, y, value);

                if (x >= Size / 2 && y < Size / 2)
                    children[3].RemoveObjectValue(x - Size / 2, y, value);

                TryReducing();
            }
        }

        public void AddObjectValue(int x, int y, MapObject value)
        {
            if (Size == 1)
            {
                if(!Value.Contains(value))
                    Value.Add(value);
            }
            else
            {
                if (children == null)
                {
                    children = new List<LovelyChunk>();

                    for (int i = 0; i < 4; i++)
                        children.Add(new LovelyChunk(Size / 2));
                }

                if (x >= Size / 2 && y >= Size / 2)
                    children[0].AddObjectValue(x - Size / 2, y - Size / 2, value);

                if (x < Size / 2 && y >= Size / 2)
                    children[1].AddObjectValue(x, y - Size / 2, value);

                if (x < Size / 2 && y < Size / 2)
                    children[2].AddObjectValue(x, y, value);

                if (x >= Size / 2 && y < Size / 2)
                    children[3].AddObjectValue(x - Size / 2, y, value);

                TryReducing();
            }
        }

        public List<MapObject> GetValue(int x, int y)
        {
            if (children == null)
            {
                return new List<MapObject>(Value);
            }
            
            if (x >= Size / 2 && y >= Size / 2)
                return children[0].GetValue(x - Size / 2, y - Size / 2);

            if (x < Size / 2 && y >= Size / 2)
                return children[1].GetValue(x, y - Size / 2);

            if (x < Size / 2 && y < Size / 2)
                return children[2].GetValue(x, y);

            //if (x >= Size/2 && y < Size/2)
            //but we need to return something and i don't want it to be zero
            return children[3].GetValue(x - Size / 2, y);
        }
    }
}