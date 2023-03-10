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
using System.Drawing;

namespace neon
{
    public class HitboxFabricator
    {
        private List<Tuple<string, List<Vector2>>> lst=new List<Tuple<string, List<Vector2>>>();

        public HitboxFabricator()
        { }

        public List<Vector2> CreateHitbox(string path)
        {
            for(int i=0; i<lst.Count; i++)
            {
                if (lst[i].Item1 == path)
                    return lst[i].Item2;
            }

            List<Vector2> htb = new List<Vector2>();
            List<string> lst1 = new List<string>();

            using(StreamReader sr=new StreamReader(path))
            {
                lst1 = sr.ReadToEnd().Split('|').ToList();
            }

            for(int i=0; i<lst1.Count; i+=2)
            {
                htb.Add(new Vector2(float.Parse(lst1[i]), float.Parse(lst1[i + 1])));
            }

            return htb;
        }
    }
}