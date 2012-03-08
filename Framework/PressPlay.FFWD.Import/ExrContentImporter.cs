using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace PressPlay.FFWD.Import
{
    /// <summary>
    /// NOTE - this file is incomplete as I got stuck on doing the PIZ decompression.
    /// </summary>
    [ContentImporter(".exr", DisplayName = "FFWD - Lightmap importer")]
    public class ExrContentImporter : ContentImporter<Texture2D>
    {
        private Dictionary<string, object> attributes = new Dictionary<string, object>();

        private class Channel
        {
            public string name;
            public enum PixelType { UINT, HALF, FLOAT };
            public PixelType pixelType;
            public byte pLinear;
            public int xSampling;
            public int ySampling;
        }
        private enum Compression { NO_COMPRESSION, RLE_COMPRESSION, ZIPS_COMPRESSION, ZIP_COMPRESSION, PIZ_COMPRESSION, PXR24_COMPRESSION, B44_COMPRESSION, B44A_COMPRESSION }
        private enum LineOrder { INCREASING, DECREASING, RANDOM_Y }

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
                    case "compression":
                        attributes.Add(name, (Compression)rd.ReadByte());
                        break;
                    case "box2i":
                        attributes.Add(name, ReadBox2i(rd));
                        break;
                    case "box2f":
                        attributes.Add(name, ReadBox2f(rd));
                        break;
                    case "lineOrder":
                        attributes.Add(name, (LineOrder)rd.ReadByte());
                        break;
                    case "float":
                        attributes.Add(name, rd.ReadSingle());
                        break;
                    case "v2f":
                        attributes.Add(name, Readv2f(rd));
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

        private List<Channel> ReadChlist(BinaryReader rd)
        {
            List<Channel> channels = new List<Channel>();
            while (rd.PeekChar() != 0)
            {
                Channel ch = new Channel();
                ch.name = ReadString(rd);
                ch.pixelType = (Channel.PixelType)rd.ReadInt32();
                ch.pLinear = rd.ReadByte();
                // Read reserved values
                rd.ReadByte();
                rd.ReadByte();
                rd.ReadByte();
                ch.xSampling = rd.ReadInt32();
                ch.ySampling = rd.ReadInt32();
                channels.Add(ch);
            }
            rd.ReadByte();
            return channels;
        }

        private Rect ReadBox2i(BinaryReader rd)
        {
            Rect r = new Rect();
            r.xMin = rd.ReadInt32();
            r.yMin = rd.ReadInt32();
            r.xMax = rd.ReadInt32();
            r.yMin = rd.ReadInt32();
            return r;
        }

        private Rect ReadBox2f(BinaryReader rd)
        {
            Rect r = new Rect();
            r.xMin = rd.ReadSingle();
            r.yMin = rd.ReadSingle();
            r.xMax = rd.ReadSingle();
            r.yMin = rd.ReadSingle();
            return r;
        }

        private Vector2 Readv2f(BinaryReader rd)
        {
            Vector2 v = new Vector2();
            v.x = rd.ReadSingle();
            v.y = rd.ReadSingle();
            return v;
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
