using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

using PhotoshopFile;

namespace PSDImporter
{
	/// <summary>
	/// This class will be instantiated by the XNA Framework Content Pipeline
	/// to import a file from disk into the specified type, TImport.
	/// 
	/// This should be part of a Content Pipeline Extension Library project.
	/// 
	/// TODO: change the ContentImporter attribute to specify the correct file
	/// extension, display name, and default processor for this importer.
	/// </summary>
	[ContentImporter(".psd", DisplayName = "Photoshop .PSD Importer", DefaultProcessor = "TextureProcessor")]
	public class PSDImporter : ContentImporter<Texture2DContent>
	{

		public override Texture2DContent Import(string filename, ContentImporterContext context)
		{
            //PsdFile psdFile = new PsdFile();
            //psdFile.Load(filename);

			Texture2DContent content = new Texture2DContent();
            PixelBitmapContent<Color> bitmap = new PixelBitmapContent<Color>(4, 4);

            //byte[] pixelData = new byte[bitmap.Width * bitmap.Height * 4];
            //List<Color[]> layers = new List<Color[]>();

            ////Debugger.Launch();

            //// Extract all the layers
            //foreach (Layer layer in psdFile.Layers)
            //{
            //    Color[] layerData = new Color[psdFile.Columns * psdFile.Rows];
            //    ExtractLayer(layer, layerData);
            //    layers.Add(layerData);
            //}

            //// Flatten the layers
            //for (int i = layers.Count - 1; i > 0; i--)
            //    FlattenLayers(psdFile.Layers[i], psdFile.Layers[i - 1], layers[i], layers[i - 1]);

            //ColorToByte(layers[0], pixelData);

            //bitmap.SetPixelData(pixelData);

            content.Mipmaps.Add(bitmap);

			return content;
		}

		private void ColorToByte(Color[] colorData, byte[] byteData)
		{
			int byteIndex = 0;
			for (int i = 0; i < colorData.Length; i++)
			{
				byteData[byteIndex] = colorData[i].R;
				byteData[byteIndex + 1] = colorData[i].G;
				byteData[byteIndex + 2] = colorData[i].B;
				byteData[byteIndex + 3] = colorData[i].A;

				byteIndex += 4;
			}
		}

		private void FlattenLayers(Layer layer0, Layer layer1, Color[] layerData0, Color[] layerData1)
		{
			for (int i = 0; i < layerData1.Length; i++)
				layerData1[i] = Blend(layerData0[i], layerData1[i]);
		}

		private void ExtractLayer(Layer layer, Color[] layerData)
		{			
			int pixelIndex = 0;
			for (int y = layer.Rect.Top; y < layer.Rect.Bottom; y++)
			{
				for (int x = layer.Rect.Left; x < layer.Rect.Right; x++)
				{
					Color color = Color.Black;

					foreach (Layer.Channel channel in layer.Channels)
					{
						// For now only take RGBA channels
						if (channel.ID == 0)
							color.R = channel.ImageData[pixelIndex];
						else if (channel.ID == 1)
							color.G = channel.ImageData[pixelIndex];
						else if (channel.ID == 2)
							color.B = channel.ImageData[pixelIndex];
						else if (channel.ID == -1)
							color.A = channel.ImageData[pixelIndex];
					}

					layerData[y * layer.PsdFile.Columns + x] = color;

					pixelIndex++;
				}
			}
		}

		private Color Blend(Color source, Color destination)
		{
			float alpha = source.A / 255.0f;
			float invAlpha = 1.0f - alpha;
			Color result = Color.Transparent;
			result.R = (byte)(source.R * alpha + destination.R * invAlpha);
			result.G = (byte)(source.G * alpha + destination.G * invAlpha);
			result.B = (byte)(source.B * alpha + destination.B * invAlpha);
			result.A = ClampByte((int)source.A + (int)destination.A);

			return result;
		}

		private byte ClampByte(int value)
		{
			if (value > 255)
				return 255;
			else if (value < 0)
				return 0;
			else
				return (byte)value;
		}
	}
}
