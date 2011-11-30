using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace PressPlay.FFWD.Import
{
    [ContentImporter(".xml", DisplayName = "FFWD - TextAsset Importer" )]
    public class TextAssetImporter : ContentImporter<TextAsset>
    {
        public override TextAsset Import(string filename, ContentImporterContext context)
        {
            string txt = File.ReadAllText(filename, Encoding.UTF8);
            return new TextAsset(txt) { name = Path.GetFileNameWithoutExtension(filename) };
        }
    }
}
