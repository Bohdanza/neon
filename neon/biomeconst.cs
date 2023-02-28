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
    public class BiomeReader
    {
        public const int InterpolationScale = 3;

        /*0 - wall
         *1 - shroomgroves
         *2 - stranger land
         *3 - fire forest
         */
        public static List<Tuple<Rectangle, int>> BiomeSeparation = new List<Tuple<Rectangle, int>>
        {
            new Tuple<Rectangle, int>(new Rectangle(0, 0, 333, 1000), 0),
            new Tuple<Rectangle, int>(new Rectangle(333, 0, 667, 333), 1),
            new Tuple<Rectangle, int>(new Rectangle(333, 333, 333, 666), 2),
            new Tuple<Rectangle, int>(new Rectangle(666, 333, 334, 667), 3)
        };

        public BiomeReader()
        { }

        public int GetBiome(int chunkX, int chunkY, string path)
        {
            if (path[path.Length - 1] != '\\')
                path += "\\";

            Tuple<int, int> cond = GetConditions(chunkX, chunkY, path);

            for (int i = 0; i < BiomeSeparation.Count; i++)
                if (BiomeSeparation[i].Item1.Contains(cond.Item1, cond.Item2))
                    return BiomeSeparation[i].Item2;

            return -1; 
        }

        private Tuple<int, int> GetConditions(int x, int y, string path)
        {
            double xq = (double)x / InterpolationScale;
            double yq = (double)y / InterpolationScale;

            int x1 = (int)Math.Floor((double)(x / InterpolationScale));
            int y1 = (int)Math.Floor((double)(y / InterpolationScale));

            int x2 = x1 + 1;
            int y2 = y1 + 1;

            //bilinear interpolation

            double ls1, ls12;
            double ls2, ls22;
            double ls3, ls32;
            double ls4, ls42;

            Tuple<int, int> lst1 = GetStats(x1, y1, path);
            Tuple<int, int> lst2 = GetStats(x2, y1, path);
            Tuple<int, int> lst3 = GetStats(x1, y2, path);
            Tuple<int, int> lst4 = GetStats(x2, y2, path);

            ls1 = lst1.Item1 * (x2 - xq);
            ls2 = lst2.Item1 * (xq - x1);
            ls3 = lst3.Item1 * (x2 - xq);
            ls4 = lst4.Item1 * (xq - x1);
            
            ls12 = lst1.Item2 * (x2 - xq);
            ls22 = lst2.Item2 * (xq - x1);
            ls32 = lst3.Item2 * (x2 - xq);
            ls42 = lst4.Item2 * (xq - x1);

            return new Tuple<int, int>((int)((ls1 + ls2) * (y2 - yq) + (ls3 + ls4) * (yq - y1)),
                (int)((ls12 + ls22) * (y2 - yq) + (ls32 + ls42) * (yq - y1)));
        }

        //first temperature, then humidity
        private Tuple<int, int> GetStats(int x, int y, string path)
        {
            if (File.Exists(path + x.ToString() + "_" + y.ToString()))
            {
                List<string> cr = new List<string>();

                using(StreamReader sr=new StreamReader(path + x.ToString() + "_" + y.ToString()))
                {
                    cr = sr.ReadToEnd().Split('\n').ToList();
                }

                return new Tuple<int, int>(Int32.Parse(cr[0]), Int32.Parse(cr[1]));
            }

            var rnd = new Random();

            int humidity = rnd.Next(0, 1000);
            int temperature = rnd.Next(0, 1000);

            string toWrite = temperature.ToString() + "\n" + humidity.ToString();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.Create(path + x.ToString() + "_" + y.ToString()).Close();

            using(StreamWriter sw=new StreamWriter(path+x.ToString()+"_"+y.ToString()))
            {
                sw.WriteLine(toWrite);
            }

            return new Tuple<int, int>(temperature, humidity);
        }
    }
}