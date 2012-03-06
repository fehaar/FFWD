using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace PressPlay.FFWD.Import
{
    [ContentImporter(".exr", DisplayName = "FFWD - Lightmap importer")]
    public class ExrContentImporter : ContentImporter<Texture2D>
    {
        private Dictionary<string, object> attributes = new Dictionary<string, object>();

        public override Texture2D Import(string filename, ContentImporterContext context)
        {
            using (BinaryReader rd = new BinaryReader(new BufferedStream(new FileStream(filename, FileMode.Open))))
            {
                int header = rd.ReadInt32();
                if (header != 20000630)
                {
                    throw new InvalidContentException("The supplied file is not a valid OpenEXR file");
                }
                int version = rd.ReadInt32();
                if ((version & 0x000F) != 2)
                {
                    throw new InvalidContentException("The supplied file is not a valid OpenEXR file. The importer only supports OpenEXR version 2.");
                }

                ReadHeader(rd);

                return ((version & 0x200) == 0) ? ReadScanlines(rd) : ReadTiles(rd);
            }
            return new Texture2D();
        }

        private void ReadHeader(BinaryReader rd)
        {
            while (rd.PeekChar() != 0)
            {
                string name = ReadString(rd);
                string type = ReadString(rd);
                int sz = rd.ReadInt32();
                switch (type)
                {
                    case "chlist":
                        attributes.Add(name, ReadChlist(rd));
                        break;
                }
            }
            rd.ReadByte();
        }

        private string ReadString(BinaryReader rd)
        {
            string s = "";
            char c;
            do
            {
                c = rd.ReadChar();
                if (c != 0)
                {
                    s += c;
                }
            } while (c != 0);
            return s;
        }

        private object ReadChlist(BinaryReader rd)
        {
            throw new NotImplementedException();
        }

        private Texture2D ReadScanlines(BinaryReader rd)
        {
            return new Texture2D();
        }

        private Texture2D ReadTiles(BinaryReader rd)
        {
            throw new NotImplementedException();
        }
    }
}
