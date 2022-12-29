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
    public class Hero:MapObject
    {
        public Hero(ContentManager contentManager, float x, float y) : base(contentManager, new Vector2(x, y), new Vector2(0f, 0f),
            3f, new List<Tuple<int, int>> { new Tuple<int, int>(0, 0), new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1), new Tuple<int, int>(-1, -1)},
            "hero", 0)
        {

        }

        public override void Update(ContentManager contentManager, WorldChunk worldChunk)
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.W))
                ChangeMovement(0, -0.2f);

            if (ks.IsKeyDown(Keys.S))
                ChangeMovement(0, 0.2f);

            if (ks.IsKeyDown(Keys.A))
                ChangeMovement(-0.2f, 0);

            if (ks.IsKeyDown(Keys.D))
                ChangeMovement(0.2f, 0);

            base.Update(contentManager, worldChunk);
        }
    }
}