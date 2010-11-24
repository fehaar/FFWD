using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna.Components;
using PressPlay.U2X.Xna;
#if WINDOWS
using System.Diagnostics;
#endif

namespace PressPlay.U2X.Xna
{
    public static class ContentHelper
    {
        public static GameServiceContainer Services { get; set; }
        public static ContentManager Content;
        public static ContentManager StaticContent;
#if WINDOWS
        public static HashSet<string> MissingAssets = new HashSet<string>();
#endif

        private struct TextureToColor
        {
            public Microsoft.Xna.Framework.Color color;
            public string textureName;
        }

        public static bool IgnoreMissingAssets { get; set; }

        private static Dictionary<string, Texture2D> StaticTextures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Model> StaticModels = new Dictionary<string, Model>();
        private static Dictionary<string, SoundEffect> StaticSounds = new Dictionary<string, SoundEffect>();

        private static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Model> Models = new Dictionary<string, Model>();
        private static Dictionary<TextureToColor, Texture2D> coloredTextures = new Dictionary<TextureToColor, Texture2D>();
        public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Song> Songs = new Dictionary<string, Song>();

        private static bool DeferredLoading = false;
        private static Queue<string> DeferredTextureQueue = new Queue<string>();
        private static Queue<string> DeferredStaticTextureQueue = new Queue<string>();
        private static Queue<string> DeferredModelQueue = new Queue<string>();
        private static Queue<string> DeferredStaticModelQueue = new Queue<string>();
        private static Queue<TextureToColor> DeferredTextureColoringQueue = new Queue<TextureToColor>();
        private static Queue<string> DeferredSoundQueue = new Queue<string>();
        private static Queue<string> DeferredStaticSoundQueue = new Queue<string>();

        public static void StartDeferredLoading()
        {
            DeferredLoading = true;
        }

        public static void EndDeferredLoading()
        {
            DeferredLoading = false;
        }

        public static int DeferredQueueSize()
        {
            return DeferredTextureQueue.Count + DeferredStaticTextureQueue.Count + DeferredSoundQueue.Count
                   + DeferredTextureColoringQueue.Count + DeferredStaticSoundQueue.Count;
        }

        public static void ExecuteDeferredLoadingStep()
        {
            // The order here is important!
            if (DeferredStaticTextureQueue.Count > 0)
            {
                LoadStaticTexture(DeferredStaticTextureQueue.Dequeue());
            }
            else if (DeferredTextureQueue.Count > 0)
            {
                LoadTexture(DeferredTextureQueue.Dequeue());
            }
            else if (DeferredStaticSoundQueue.Count > 0)
            {
                LoadStaticSound(DeferredStaticSoundQueue.Dequeue());
            }
            else if (DeferredSoundQueue.Count > 0)
            {
                LoadSound(DeferredSoundQueue.Dequeue());
            }
            else if (DeferredTextureColoringQueue.Count > 0)
            {
                PreColorTexture(DeferredTextureColoringQueue.Dequeue());
            }
        }

        public static void LoadTexture(string name)
        {
            if (String.IsNullOrEmpty(name))
                return;

            if (DeferredLoading)
            {
                if (!DeferredTextureQueue.Contains(name) && !DeferredStaticTextureQueue.Contains(name))
                {
                    DeferredTextureQueue.Enqueue(name);
                }
                return;
            }

            if (Textures.ContainsKey(name))
            {
                return;
            }
            // Don't load textures that are to be loaded as a static texture.
            if (StaticTextures.ContainsKey(name))
            {
                return;
            }

            string fileName = name.Replace("-", "");
            if (fileName.Contains("."))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            try
            {
                Textures.Add(name, Content.Load<Texture2D>("Textures\\" + fileName));
            }
            catch
            {
                MissingAsset("Texture", name);
                if (!IgnoreMissingAssets)
                {
                    throw;
                }
            }
        }

        private static void MissingAsset(string type, string name)
        {
#if WINDOWS
            string key = type + ": " + name;
            if (!MissingAssets.Contains(key))
            {
                MissingAssets.Add(key);
            }
#else
            return;
#endif
        }

        public static void LoadModel(string name)
        {
            if (String.IsNullOrEmpty(name))
                return;

            if (DeferredLoading)
            {
                if (!DeferredModelQueue.Contains(name) && !DeferredModelQueue.Contains(name))
                {
                    DeferredTextureQueue.Enqueue(name);
                }
                return;
            }

            if (Models.ContainsKey(name))
            {
                return;
            }
            // Don't load textures that are to be loaded as a static texture.
            if (StaticModels.ContainsKey(name))
            {
                return;
            }

            string fileName = name.Replace("-", "");
            if (fileName.Contains("."))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            try
            {
                Models.Add(name, Content.Load<Model>("Models\\" + fileName));
            }
            catch
            {
                MissingAsset("Model", name);
                if (!IgnoreMissingAssets)
                {
                    throw;
                }
            }
        }

        public static SpriteFont LoadFont(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                SpriteFont font = StaticContent.Load<SpriteFont>(Path.GetFileNameWithoutExtension(name));
                return font;
            }
            return null;
        }

        public static void LoadSound(string name)
        {
            //if (AudioController.noSound) return;
            if (String.IsNullOrEmpty(name))
                return;

            if (DeferredLoading)
            {
                if (!DeferredSoundQueue.Contains(name) && !DeferredStaticSoundQueue.Contains(name))
                {
                    DeferredSoundQueue.Enqueue(name);
                }
                return;
            }

            if (Sounds.ContainsKey(name) || StaticSounds.ContainsKey(name))
            {
                return;
            }

            SoundEffect sound = null;
            try
            {
                sound = Content.Load<SoundEffect>("Sounds\\" + Path.GetFileNameWithoutExtension(name));
            }
            catch
            {
                MissingAsset("Sound", name);
            }
            if (sound != null)
            {
                ContentHelper.Sounds.Add(name, sound);
                //ContentHelper.SoundDurations.Add(name, (float)sound.Duration.TotalSeconds);
            }
        }

        public static void LoadSong(string name)
        {
            //if (AudioController.noSound) return;
            if (String.IsNullOrEmpty(name))
                return;

            //if (DeferredLoading)
            //{
            //    if (!DeferredSoundQueue.Contains(name) && !DeferredStaticSoundQueue.Contains(name))
            //    {
            //        DeferredSoundQueue.Enqueue(name);
            //    }
            //    return;
            //}

            if (Songs.ContainsKey(name))
            {
                return;
            }
            //if (StaticSounds.ContainsKey(name))
            //{
            //    return;
            //}

            Song song = null;
            try
            {
                song = Content.Load<Song>("Sounds\\" + Path.GetFileNameWithoutExtension(name));
            }
            catch
            {
                Console.WriteLine("Missing song : " + name);
            }
            if (song != null)
                ContentHelper.Songs.Add(name, song);
        }


        public static void LoadStaticSound(string name)
        {
            //NO LEAK IN STATIC SOUNDS

            //if (AudioController.noSound) return;
            if (String.IsNullOrEmpty(name))
                return;

            if (DeferredLoading)
            {
                if (!DeferredStaticSoundQueue.Contains(name))
                {
                    DeferredStaticSoundQueue.Enqueue(name);
                }
                return;
            }

            if (ContentHelper.StaticSounds.ContainsKey(name))
                return;

            SoundEffect sound = null;
            try
            {
                sound = StaticContent.Load<SoundEffect>("Sounds\\" + Path.GetFileNameWithoutExtension(name));
            }
            catch
            {
                Console.WriteLine("Missing sound: " + name);
            }
            if (sound != null)
            {
                ContentHelper.StaticSounds.Add(name, sound);
                //                ContentHelper.StaticSoundDurations.Add(name, (float)sound.Duration.TotalSeconds);
            }

        }

        public static Scene LoadScene(string name, int level)
        {
            Scene theScene = Content.Load<Scene>("Scenes\\" + name);
            PreLoadContent(level);
            return theScene;
        }

        public static void LoadStaticTexture(string name)
        {
            //NO LEAK IN STATIC TEXTURES
            if (String.IsNullOrEmpty(name))
                return;

            if (DeferredLoading)
            {
                if (!DeferredStaticTextureQueue.Contains(name))
                {
                    DeferredStaticTextureQueue.Enqueue(name);
                }
                return;
            }

            if (ContentHelper.StaticTextures.ContainsKey(name))
            {
                return;
            }


            string fileName = name.Replace("-", "");

            if (fileName.Contains("."))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }
            Texture2D tex = StaticContent.Load<Texture2D>("Textures\\" + fileName);
            StaticTextures.Add(name, tex);

        }

        public static Texture2D GetTexture(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            Texture2D texture;
            Textures.TryGetValue(name, out texture);
            if (texture == null)
            {
                StaticTextures.TryGetValue(name, out texture);
            }
            return texture;
        }

        public static Model GetModel(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            Model model;
            Models.TryGetValue(name, out model);
            if (model == null)
            {
                StaticModels.TryGetValue(name, out model);
            }
            return model;
        }

        public static SoundEffect GetSound(string name)
        {
            //if (AudioController.noSound) return null;
            if (String.IsNullOrEmpty(name))
                return null;

            SoundEffect sound;
            Sounds.TryGetValue(name, out sound);
            if (sound == null)
            {
                StaticSounds.TryGetValue(name, out sound);
            }
            return sound;
        }

        //public static float GetSoundDuration(string name)
        //{
        //    if (AudioController.noSound) return -1;
        //    if (String.IsNullOrEmpty(name))
        //        return -1;

        //    float duration = -1;
        //    SoundDurations.TryGetValue(name, out duration);
        //    if (duration == -1)
        //    {
        //        StaticSoundDurations.TryGetValue(name, out duration);
        //    }
        //    return duration;
        //}

        public static Song GetSong(string name)
        {
            //if (AudioController.noSound) return null;
            if (String.IsNullOrEmpty(name))
                return null;

            Song song;
            Songs.TryGetValue(name, out song);
            return song;
        }

        private static void PreColorTexture(TextureToColor textureToColor)
        {
            GetColoredTexture(textureToColor.color, textureToColor.textureName, true);
        }

        /// <summary>
        /// Here we make sure the static textures are loaded and we pre-color textures for the given level.
        /// </summary>
        /// <param name="level"></param>
        private static void PreLoadContent(int level)
        {
            // Make sure we have loaded the static textures.
            foreach (string tex in StaticTextureNames)
            {
                LoadStaticTexture(tex);
            }
            foreach (string sound in StaticSoundNames)
            {
                LoadStaticSound(sound);
            }

            // Pre color textures (for particle systems mostly)
            Dictionary<string, Color[]> texsToLoad;

            texturesToPreColor.TryGetValue(level, out texsToLoad);
            if (texsToLoad == null) return;
            foreach (var item in texsToLoad.Keys)
            {
                LoadTexture(item);
            }
            foreach (var item in texsToLoad)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    TextureToColor key = new TextureToColor() { color = item.Value[i], textureName = item.Key };
                    if (DeferredLoading)
                    {
                        if (!DeferredTextureColoringQueue.Contains(key))
                        {
                            DeferredTextureColoringQueue.Enqueue(key);
                        }
                    }
                    else
                        PreColorTexture(key);
                }
            }

        }

        internal static Texture2D GetColoredTexture(Color aColor, string name)
        {
            return GetColoredTexture(aColor, name, false);
        }

        internal static Texture2D GetColoredTexture(Color aColor, string name, bool preColoring)
        {
            Texture2D colorMultipliedTexture;
            TextureToColor key = new TextureToColor() { color = aColor, textureName = name };

            coloredTextures.TryGetValue(key, out colorMultipliedTexture);

            if (colorMultipliedTexture != null)
                return colorMultipliedTexture;

            //if (!preColoring && Time.timeSinceLevelLoad > 10)
            //    Console.WriteLine("Texture " + key.textureName + " with color: " + "new Color(" + key.color.R + ","+ key.color.G + "," + key.color.B + "," + key.color.A + ")");

            Texture2D originalTex = GetTexture(name);
            colorMultipliedTexture = new Texture2D(originalTex.GraphicsDevice, originalTex.Width, originalTex.Height);
            Color[] textureData = new Color[originalTex.Width * originalTex.Height];
            originalTex.GetData(textureData, 0, textureData.Length);
            for (int i = 0; i < textureData.Length; i++)
            {
                textureData[i].R = (byte)((int)(textureData[i].R) * (int)(aColor.R) / 255);
                textureData[i].G = (byte)((int)(textureData[i].G) * (int)(aColor.G) / 255);
                textureData[i].B = (byte)((int)(textureData[i].B) * (int)(aColor.B) / 255);
                textureData[i].A = (byte)((int)(textureData[i].A) * (int)(aColor.A) / 255);
            }
            colorMultipliedTexture.SetData(textureData);
            coloredTextures.Add(key, colorMultipliedTexture);
            return colorMultipliedTexture;
        }

        public static void CleanUp()
        {
            foreach (var tex in Textures)
            {
                tex.Value.Dispose();
            }
            Textures = new Dictionary<string, Texture2D>();
            coloredTextures = new Dictionary<TextureToColor, Texture2D>();
            Sounds = new Dictionary<string, SoundEffect>();
            //            SoundDurations = new Dictionary<string, float>();

            DeferredTextureQueue = new Queue<string>();
            DeferredSoundQueue = new Queue<string>();
            Content.Unload();
            Content.Dispose();
            Content = new ContentManager(Services, "Content");
        }

        #region Static Textures
        private static string[] StaticTextureNames = new string[]
        { "whitePixel",
          "debugCircle",
            "AtlasTiles_suburbia.png",
            "AtlasPulley_pirate.png",
            "AtlasTiles_pirate1.png",
            "AtlasTiles_pirate1_sketch.png",
            "AtlasTiles_pirate2.png",
            "AtlasTiles_pirate2_sketch.png",
            "AtlasTiles_pirate3.png",
            "AtlasTiles_pirate3_sketch.png",
            "AtlasTiles_sketch_suburbia.png",
            "AtlasTiles_suburbia.png",
            "Atlas_deco_pirate.png",
            "Atlas_deco_pirate_sketch.png",
            "AtlasCommon.png",
            "AtlasCommon_sketch.png",
            "AtlasGUI.png",
            "markerTexture.png",
            "max_spawn_flat.png",
            "maxSpriteSheet01.png",
            "maxSpriteSheet02.png",
            "maxSpriteSheet03.png",
            "maxSpriteSheet04.png",
            "monster_PauseGfx.png",
            "Monster_SpriteSheet.png",
            "Monster_SpriteSheet_2.png",
            "Monster_SpriteSheet_3.png",
            "monsterStealingInkSheet.png",
            "Hints\\hint_ink",
            "Hints\\hint_draw",
            "Hints\\hint_erase",
            "Hints\\hint_deleteAll",
            "Hints\\hint_pullPush",
            "Hints\\hint_combine",
            "Hints\\hint_seesaw",
            "Hints\\hint_erasePart",
            "Hints\\hint_killGobos",
            "Hints\\hint_pause",
            "Hints\\hint_letGo",
            "Hints\\hint_weightOfDrawings",
            "Hints\\hint_zoom",
            "Hints\\hint_hooks"
        };
        private static string[] StaticSoundNames = new string[]
        { "freezeMode",
          "checkpoint",
          "maxClimb",
          "maxExtremeFall",
          "maxExtremeJump",
          "maxGrab",
          "maxJump01",
          "maxQuickClimb",
          "maxSlide",
          "checkpoint"};
        #endregion
        #region Texture precolor structure
        private static Dictionary<int, Dictionary<string, Color[]>> texturesToPreColor = new Dictionary<int, Dictionary<string, Color[]>>()
        {
            {1, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10), 
                                               new Color(46, 4, 99, 255),
                                               new Color(0, 0, 0, 255), 
                                               new Color(0, 0, 12, 255), 
                                               new Color(227, 225, 216, 255), 
                                               new Color(255, 255, 255, 10),
                                               new Color(254, 251, 248, 255),
                                               new Color(183, 179, 198, 255),
                                               new Color(25, 10, 1, 255),
                                               new Color(255, 229, 185, 255),
                                               new Color(46, 4, 99, 255)}},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(255, 255, 255, 10),
                                               new Color(52, 52, 52, 52),
                                               new Color(95, 95, 95, 95),
                                               new Color(137, 137, 137, 137),
                                               new Color(255, 255, 255, 180),
                                               new Color(198, 198, 198, 198),
                                               new Color(217, 217, 217, 217),
                                               new Color(236, 236, 236, 236),
                                               new Color(255, 255, 255, 255)}},
                    {"WaterParticleTex.png", new Color[] 
                                             { new Color(255, 255, 255, 10),
                                               new Color(52, 52, 52, 52),
                                               new Color(95, 95, 95, 95),
                                               new Color(137, 137, 137, 137),
                                               new Color(255, 255, 255, 180),
                                               new Color(198, 198, 198, 198),
                                               new Color(217, 217, 217, 217),
                                               new Color(236, 236, 236, 236),
                                               new Color(255, 255, 255, 255)}},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255), 
                                               new Color(255, 255, 255, 10),
                                               new Color(25, 10, 1, 255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {2, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10), 
                                               new Color(18, 0, 27, 255),
                                               new Color(200, 14, 221, 255),
                                               new Color(0, 0, 0, 255), 
                                               new Color(0, 0, 12, 255), 
                                               new Color(227, 225, 216, 255), 
                                               new Color(255, 255, 255, 10),
                                               new Color(254, 251, 248, 255),
                                               new Color(183, 179, 198, 255),
                                               new Color(25, 10, 1, 255),
                                               new Color(48,12,129,255),
                                               new Color(211,238,247,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(46, 4, 99, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(255, 255, 255, 10),
                                               new Color(52, 52, 52, 52),
                                               new Color(95, 95, 95, 95),
                                               new Color(249,241,255,255),
                                               new Color(250,244,255,255),
                                               new Color(252,248,255,255),
                                               new Color(253,251,255,255),
                                               new Color(137, 137, 137, 137),
                                               new Color(255, 255, 255, 180),
                                               new Color(198, 198, 198, 198),
                                               new Color(217, 217, 217, 217),
                                               new Color(236, 236, 236, 236),
                                               new Color(255, 255, 255, 255)}},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255), 
                                               new Color(255, 255, 255, 10),
                                               new Color(25, 10, 1, 255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {3, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18, 0, 27, 255)
                                              ,new Color(200, 14, 221, 255)
                                              ,new Color(183, 179, 198, 255)
                                              ,new Color(0, 0, 0, 255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(249,241,255,255)
                                              ,new Color(250,244,255,255)
                                              ,new Color(252,248,255,255)
                                              ,new Color(253,251,255,255)
                                              ,new Color(255, 255, 255, 255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {4, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18, 0, 27, 255)
                                              ,new Color(200, 14, 221, 255)
                                              ,new Color(183, 179, 198, 255)
                                              ,new Color(0, 0, 0, 255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(249,241,255,255)
                                              ,new Color(250,244,255,255)
                                              ,new Color(252,248,255,255)
                                              ,new Color(253,251,255,255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(255, 255, 255, 255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {5, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18, 0, 27, 255)
                                              ,new Color(200, 14, 221, 255)
                                              ,new Color(183, 179, 198, 255)
                                              ,new Color(0, 0, 0, 255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(249,241,255,255)
                                              ,new Color(250,244,255,255)
                                              ,new Color(252,248,255,255)
                                              ,new Color(253,251,255,255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(255, 255, 255, 255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {6, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                             }},
                    {"fallingRockPiece.png", new Color[]
                                           { new Color(25,25,25,255) 
                                           }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {7, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"fallingRockPiece.png", new Color[]
                                           { new Color(25,25,25,255) 
                                           }},
                     {"Default-Particle.png", new Color[]
                                           { new Color(0,0,0,255) 
                                              ,new Color(0,0,0,236)
                                              ,new Color(0,0,0,218)
                                              ,new Color(0,0,0,199)
                                              ,new Color(0,0,0,181)
                                              ,new Color(0,0,0,138)
                                              ,new Color(0,0,0,95)
                                              ,new Color(0,0,0,52)
                                              ,new Color(0,0,0,10)
                                           }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {8, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"WaterParticleTex.png", new Color[] 
                                             { new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"fallingRockPiece.png", new Color[]
                                           { new Color(25,25,25,255) 
                                           }},
                     {"Default-Particle.png", new Color[]
                                           { new Color(0,0,0,255) 
                                              ,new Color(0,0,0,236)
                                              ,new Color(0,0,0,218)
                                              ,new Color(0,0,0,199)
                                              ,new Color(0,0,0,181)
                                              ,new Color(0,0,0,138)
                                              ,new Color(0,0,0,95)
                                              ,new Color(0,0,0,52)
                                              ,new Color(0,0,0,10)
                                           }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {9, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"WaterParticleTex.png", new Color[] 
                                             { new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"fallingRockPiece.png", new Color[]
                                           { new Color(25,25,25,255) 
                                           }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {10, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"WaterParticleTex.png", new Color[] 
                                             { new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"fallingRockPiece.png", new Color[]
                                           { new Color(25,25,25,255) 
                                           }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {11, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {12, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {13, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {14, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            },
            {15, 
                new Dictionary<string, Color[]>()
                {
                    {"WhiteCircleHardAlpha.png", new Color[]
                                             { new Color(255, 255, 255, 10)
                                              ,new Color(18,0,27,255)
                                              ,new Color(200,14,221,255)
                                              ,new Color(183,179,198,255)
                                              ,new Color(227,225,216,255)
                                              ,new Color(46,4,99,255)
                                              ,new Color(0,0,0,255)
                                              ,new Color(0,0,12,255)
                                              ,new Color(254, 251, 248, 255)
                                             }},
                     {"Default-Particle.png", new Color[]
                                           { new Color(0,0,0,255) 
                                              ,new Color(0,0,0,236)
                                              ,new Color(0,0,0,218)
                                              ,new Color(0,0,0,199)
                                              ,new Color(0,0,0,181)
                                              ,new Color(0,0,0,138)
                                              ,new Color(0,0,0,95)
                                              ,new Color(0,0,0,52)
                                              ,new Color(0,0,0,10)
                                           }},
                    {"WhiteCircleSoftAlpha.png", new Color[] 
                                             { new Color(0, 0, 0, 255)
                                              ,new Color(255,255,255,10)
                                              ,new Color(52, 52, 52, 52)
                                              ,new Color(95, 95, 95, 95)
                                              ,new Color(137, 137, 137, 137)
                                              ,new Color(255, 255, 255, 180)
                                              ,new Color(198, 198, 198, 198)
                                              ,new Color(217, 217, 217, 217)
                                              ,new Color(236, 236, 236, 236)
                                              ,new Color(255,255,255,255)
                                             }},
                    {"AtlasCommon.png", new Color[]
                                             { new Color(7, 5, 0, 255),
                                               new Color(255, 255, 255, 10),
                                               new Color(25,10,1,255),
                                               new Color(255, 229, 185, 255),
                                               new Color(255, 136, 64, 255) }}
                }
            }

        };
        #endregion

    }
}
