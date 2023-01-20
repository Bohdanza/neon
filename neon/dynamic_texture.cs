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
    public class DynamicTexture
    {
        public const int FrameDelay = 1;
        private int TimeSinceLastUpdate = 0;

        [JsonProperty]
        public string BaseName { get; protected set; }

        public List<Texture2D> Textures { get; protected set; }
        public int CurrentTexture { get; protected set; }

        public DynamicTexture(ContentManager contentManager, string name)
        {
            BaseName = name;

            Load(contentManager);
        }

        /// <summary>
        /// Used to load during initialization and to reload (in case of i dunno what)
        /// </summary>
        /// <param name="contentManager">guess what u must pass here</param>
        public void Load(ContentManager contentManager)
        {
            if (contentManager != null)
            {
                Textures = new List<Texture2D>();
                CurrentTexture = 0;

                while (File.Exists(@"Content\" + BaseName + CurrentTexture.ToString() + ".xnb"))
                {
                    Textures.Add(contentManager.Load<Texture2D>(BaseName + CurrentTexture.ToString()));

                    CurrentTexture++;
                }

                CurrentTexture = 0;
            }
            else
            {
                Textures = null;
            }

            TimeSinceLastUpdate = 0;
        }

        /// <summary>
        /// used to move current frame and stuff
        /// </summary>
        public void Update(ContentManager contentManager)
        {
            TimeSinceLastUpdate++;

            if (TimeSinceLastUpdate > FrameDelay)
            {
                TimeSinceLastUpdate = 0;

                if (Textures == null)
                {
                    Load(contentManager);
                }
                else
                {
                    CurrentTexture++;

                    if (CurrentTexture >= Textures.Count)
                        CurrentTexture = 0;
                }
            }
        }

        /// <summary>
        /// Use this to get what u need to draw
        /// </summary>
        /// <returns></returns>
        public Texture2D GetCurrentFrame()
        {
            if (Textures != null && CurrentTexture >= 0 && CurrentTexture < Textures.Count)
                return Textures[CurrentTexture];
            else
                return Game1.NoTexture;
        }
    }
}