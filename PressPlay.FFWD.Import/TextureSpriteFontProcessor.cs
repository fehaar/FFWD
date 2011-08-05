using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using System.ComponentModel;
using System.IO;
using System.Text;

namespace PressPlay.FFWD.Import
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "FFWD - Sprite Font Texture")]
    public class TextureSpriteFontProcessor : FontTextureProcessor
    {
        private List<char> Characters = new List<char>();

        public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            string defFile = Path.ChangeExtension(input.Identity.SourceFilename, "txt");
            if (File.Exists(defFile))
            {
                List<CharacterRange> ranges = new List<CharacterRange>();
                string txt = File.ReadAllText(defFile);
                CharacterRange range = null;
                foreach (char item in txt)
                {
                    Characters.Add(item);
                    if (range == null)
                    {
                        range = new CharacterRange(item);
                    }
                    else
                    {
                        if (!range.Extend(item))
                        {
                            ranges.Add(range);
                            range = new CharacterRange(item);
                        }
                    }
                }

                StringBuilder rangeText = new StringBuilder();
                ranges.ForEach(r => rangeText.Append(r + ";"));
                File.WriteAllText(defFile.Insert(defFile.Length - 4, "-ranges"), rangeText.ToString(0, rangeText.Length - 1));
            }
            return base.Process(input, context);
        }

        protected override char GetCharacterForIndex(int index)
        {
            return Characters[index];
        }
    }
}