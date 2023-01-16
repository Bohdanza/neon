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
    public class AI
    {
        public int InputSize { get; protected set; }
        public int OutputSize { get; protected set; }

        private List<List<int>> Chain { get; set; }
        private List<float> Weights { get; set; }

        public AI(int inputSize, int outputSize)
        {
            InputSize = inputSize;
            OutputSize = outputSize;

            Chain = new List<List<int>>();
            Weights = new List<float>();

            Weights.Add(1f); Weights.Add(1f); Weights.Add(1f);
        }

        public List<float> GetRes(List<float> input)
        {
            List<float> np = new List<float>(input);
            np.AddRange(Weights);

            Weights = np;

            List<float> ans = new List<float>();

            for(int i=0; i<OutputSize; i++)
                ans.Add(GetResultRelative(OutputSize + i));

            return ans;
        }

        private float GetResultRelative(int vert)
        {
            if (vert < InputSize)
                return Weights[vert];

            List<int> ancestors = Chain[vert - OutputSize];
            float ans = Weights[vert];

            for (int i = 0; i < ancestors.Count; i++)
                ans *= GetResultRelative(ancestors[i]);

            return ans;
        }


    }
}