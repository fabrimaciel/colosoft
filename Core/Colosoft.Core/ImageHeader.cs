/* 
 * Colosoft Framework - generic framework to assist in development on the .NET platform
 * Copyright (C) 2013  <http://www.colosoft.com.br/framework> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Colosoft.Media.Drawing
{
	/// <summary>
	/// Taken from http://stackoverflow.com/questions/111345/getting-image-dimensions-without-reading-the-entire-file/111349
	/// Minor improvements including supporting unsigned 16-bit integers when decoding Jfif and added logic
	/// to load the image using new Bitmap if reading the headers fails
	/// </summary>
	public static class ImageHeader
	{
		private const string errorMessage = "Could not recognise image format.";

		private static Dictionary<byte[], Func<BinaryReader, Size>> _imageFormatDecoders = new Dictionary<byte[], Func<BinaryReader, Size>>() {
			{
				new byte[] {
					0x42,
					0x4D
				},
				DecodeBitmap
			},
			{
				new byte[] {
					0x47,
					0x49,
					0x46,
					0x38,
					0x37,
					0x61
				},
				DecodeGif
			},
			{
				new byte[] {
					0x47,
					0x49,
					0x46,
					0x38,
					0x39,
					0x61
				},
				DecodeGif
			},
			{
				new byte[] {
					0x89,
					0x50,
					0x4E,
					0x47,
					0x0D,
					0x0A,
					0x1A,
					0x0A
				},
				DecodePng
			},
			{
				new byte[] {
					0xff,
					0xd8
				},
				DecodeJfif
			},
		};

		/// <summary>        
		/// Gets the dimensions of an image.        
		/// </summary>        
		/// <param name="path">The path of the image to get the dimensions of.</param>        
		/// <returns>The dimensions of the specified image.</returns>        
		/// <exception cref="ArgumentException">The image was of an unrecognised format.</exception>        
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Size GetDimensions(string path)
		{
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
			{
				try
				{
					return GetDimensions(binaryReader);
				}
				catch(ArgumentException ex)
				{
					string newMessage = string.Format("{0} file: '{1}' ", errorMessage, path);
					throw new ArgumentException(newMessage, "path", ex);
				}
			}
		}

		/// <summary>        
		/// Gets the dimensions of an image.        
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns>The dimensions of the specified image.</returns>        
		/// <exception cref="ArgumentException">The image was of an unrecognised format.</exception>            
		public static Size GetDimensions(BinaryReader binaryReader)
		{
			int maxMagicBytesLength = _imageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;
			byte[] magicBytes = new byte[maxMagicBytesLength];
			for(int i = 0; i < maxMagicBytesLength; i += 1)
			{
				magicBytes[i] = binaryReader.ReadByte();
				foreach (var kvPair in _imageFormatDecoders)
				{
					if(StartsWith(magicBytes, kvPair.Key))
						return kvPair.Value(binaryReader);
				}
			}
			throw new ArgumentException(errorMessage, "binaryReader");
		}

		/// <summary>
		/// Verifica se os bytes começam com o informado.
		/// </summary>
		/// <param name="thisBytes"></param>
		/// <param name="thatBytes"></param>
		/// <returns></returns>
		private static bool StartsWith(byte[] thisBytes, byte[] thatBytes)
		{
			for(int i = 0; i < thatBytes.Length; i += 1)
			{
				if(thisBytes[i] != thatBytes[i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Lê o valor Int16 usndo LittleEndian.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static short ReadLittleEndianInt16(BinaryReader binaryReader)
		{
			byte[] bytes = new byte[sizeof(short)];
			for(int i = 0; i < sizeof(short); i += 1)
				bytes[sizeof(short) - 1 - i] = binaryReader.ReadByte();
			return BitConverter.ToInt16(bytes, 0);
		}

		/// <summary>
		/// Lê o valor UInt16 usndo LittleEndian.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static ushort ReadLittleEndianUInt16(BinaryReader binaryReader)
		{
			byte[] bytes = new byte[sizeof(ushort)];
			for(int i = 0; i < sizeof(ushort); i += 1)
				bytes[sizeof(ushort) - 1 - i] = binaryReader.ReadByte();
			return BitConverter.ToUInt16(bytes, 0);
		}

		/// <summary>
		/// Lê o valor Int32 usndo LittleEndian.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static int ReadLittleEndianInt32(BinaryReader binaryReader)
		{
			byte[] bytes = new byte[sizeof(int)];
			for(int i = 0; i < sizeof(int); i += 1)
			{
				bytes[sizeof(int) - 1 - i] = binaryReader.ReadByte();
			}
			return BitConverter.ToInt32(bytes, 0);
		}

		/// <summary>
		/// Decodifica o tamanho do Bitmap.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static Size DecodeBitmap(BinaryReader binaryReader)
		{
			binaryReader.ReadBytes(16);
			int width = binaryReader.ReadInt32();
			int height = binaryReader.ReadInt32();
			return new Size(width, height);
		}

		/// <summary>
		/// Decodifica o tamanho do Gif.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static Size DecodeGif(BinaryReader binaryReader)
		{
			int width = binaryReader.ReadInt16();
			int height = binaryReader.ReadInt16();
			return new Size(width, height);
		}

		/// <summary>
		/// Decofica o tamanho do Png.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static Size DecodePng(BinaryReader binaryReader)
		{
			binaryReader.ReadBytes(8);
			int width = ReadLittleEndianInt32(binaryReader);
			int height = ReadLittleEndianInt32(binaryReader);
			return new Size(width, height);
		}

		/// <summary>
		/// Decodifica o tamanho do Jpeg.
		/// </summary>
		/// <param name="binaryReader"></param>
		/// <returns></returns>
		private static Size DecodeJfif(BinaryReader binaryReader)
		{
			while (binaryReader.ReadByte() == 0xff)
			{
				byte marker = binaryReader.ReadByte();
				short chunkLength = ReadLittleEndianInt16(binaryReader);
				if(marker == 0xc0)
				{
					binaryReader.ReadByte();
					int height = ReadLittleEndianInt16(binaryReader);
					int width = ReadLittleEndianInt16(binaryReader);
					return new Size(width, height);
				}
				if(chunkLength < 0)
				{
					ushort uchunkLength = (ushort)chunkLength;
					binaryReader.ReadBytes(uchunkLength - 2);
				}
				else
				{
					binaryReader.ReadBytes(chunkLength - 2);
				}
			}
			throw new ArgumentException(errorMessage);
		}
	}
}
