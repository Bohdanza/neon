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
        private List<Tuple<string, List<Tuple<int, int>>>> lst=new List<Tuple<string, List<Tuple<int, int>>>>();

        public HitboxFabricator()
        { }

        public List<Tuple<int, int>> CreateHitbox(string path)
        {
            for(int i=0; i<lst.Count; i++)
            {
                if (lst[i].Item1 == path)
                    return lst[i].Item2;
            }

            List<Tuple<int, int>> htb = new List<Tuple<int, int>>();

            Bitmap bitmap = new Bitmap(path);

            int centerX = 0, centerY = 0;

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color pxl = bitmap.GetPixel(i, j);

                    if (pxl != System.Drawing.Color.FromArgb(255, 255, 255))
                    {
                        if (pxl == System.Drawing.Color.FromArgb(0, 255, 0))
                        {
                            centerX = i;
                            centerY = j;
                        }
                        else
                        {
                            htb.Add(new Tuple<int, int>(i, j));

                            if (pxl == System.Drawing.Color.FromArgb(255, 0, 0))
                            {
                                centerX = i;
                                centerY = j;
                            }
                        }
                    }
                }

            for (int i = 0; i < htb.Count; i++)
                htb[i] = new Tuple<int, int>(htb[i].Item1 - centerX, htb[i].Item2 - centerY);

            lst.Add(new Tuple<string, List<Tuple<int, int>>>(path, htb));

            return htb;
        }
    }
}