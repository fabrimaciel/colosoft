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
using System.Text;
using System.IO;

namespace Colosoft.Excel
{
	public partial class Record
	{
		public UInt16 Type;

		public UInt16 Size;

		public byte[] Data;

		public List<Record> ContinuedRecords;

		public Record()
		{
			ContinuedRecords = new List<Record>();
		}

		public Record(Record record)
		{
			Type = record.Type;
			Size = record.Size;
			Data = record.Data;
			ContinuedRecords = record.ContinuedRecords;
		}

		public virtual void Decode()
		{
		}

		public static Record ReadBase(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);
			Record record = new Record();
			record.Type = reader.ReadUInt16();
			record.Size = reader.ReadUInt16();
			record.Data = reader.ReadBytes(record.Size);
			return record;
		}

		public int TotalSize
		{
			get
			{
				int total_size = Size;
				foreach (Record record in ContinuedRecords)
				{
					total_size += record.Size;
				}
				return total_size;
			}
		}

		public byte[] AllData
		{
			get
			{
				if(ContinuedRecords.Count == 0)
					return Data;
				else
				{
					List<byte> data = new List<byte>(TotalSize);
					data.AddRange(Data);
					foreach (Record record in ContinuedRecords)
					{
						data.AddRange(record.AllData);
					}
					return data.ToArray();
				}
			}
		}

		protected int ContinuedIndex = -1;

		public string ReadString(BinaryReader reader, int lengthbits)
		{
			BinaryReader continuedReader;
			return ReadString(reader, lengthbits, out continuedReader);
		}

		public string ReadString(BinaryReader reader, int lengthbits, out BinaryReader continuedReader)
		{
			if(reader.PeekChar() == -1)
			{
				if(ContinuedIndex < ContinuedRecords.Count - 1)
				{
					reader = SwitchToContinuedRecord();
				}
				else
				{
					continuedReader = reader;
					return null;
				}
			}
			int stringlength = lengthbits == 8 ? reader.ReadByte() : reader.ReadUInt16();
			byte option = reader.ReadByte();
			Encoding encoding = Encoding.Default;
			int bytelength = stringlength;
			if((option & 0x01) == 1)
			{
				encoding = Encoding.Unicode;
				bytelength *= 2;
			}
			bool phonetic = (option & 0x04) == 0x04;
			bool richtext = (option & 0x08) == 0x08;
			int runs = 0;
			int size = 0;
			if(richtext)
			{
				runs = reader.ReadUInt16();
			}
			if(phonetic)
			{
				size = reader.ReadInt32();
			}
			byte[] bytes = reader.ReadBytes(bytelength);
			string firstpart = encoding.GetString(bytes);
			StringBuilder text = new StringBuilder();
			text.Append(firstpart);
			continuedReader = reader;
			if(bytes.Length < bytelength)
			{
				continuedReader = SwitchToContinuedRecord();
				text.Append(ReadContinuedString(continuedReader, stringlength - firstpart.Length, out continuedReader));
			}
			ReadBytes(continuedReader, 4 * runs + size);
			return text.ToString();
		}

		private string ReadContinuedString(BinaryReader reader, int stringlength, out BinaryReader continuedReader)
		{
			continuedReader = reader;
			if(reader.PeekChar() == -1)
				return null;
			byte option = reader.ReadByte();
			Encoding encoding = Encoding.ASCII;
			int bytelength = stringlength;
			if((option & 0x01) == 1)
			{
				encoding = Encoding.Unicode;
				bytelength *= 2;
			}
			byte[] bytes = reader.ReadBytes(bytelength);
			string firstpart = encoding.GetString(bytes);
			if(bytes.Length < bytelength)
			{
				continuedReader = SwitchToContinuedRecord();
				StringBuilder text = new StringBuilder();
				text.Append(firstpart);
				text.Append(ReadContinuedString(continuedReader, stringlength - firstpart.Length, out continuedReader));
				return text.ToString();
			}
			else
			{
				return firstpart;
			}
		}

		protected byte[] ReadBytes(BinaryReader reader, int count)
		{
			byte[] bytes = reader.ReadBytes(count);
			int bytesRead = bytes.Length;
			if(bytesRead < count)
			{
				byte[] allbytes = new byte[count];
				byte[] remainedbytes = ReadBytes(SwitchToContinuedRecord(), count - bytesRead);
				bytes.CopyTo(allbytes, 0);
				remainedbytes.CopyTo(allbytes, bytesRead);
				return allbytes;
			}
			return bytes;
		}

		protected BinaryReader SwitchToContinuedRecord()
		{
			ContinuedIndex++;
			MemoryStream stream = new MemoryStream(ContinuedRecords[ContinuedIndex].Data);
			return new BinaryReader(stream);
		}

		public static object DecodeRK(uint value)
		{
			bool muled = (value & 0x01) == 1;
			bool isFloat = (value & 0x02) == 0;
			if(isFloat)
			{
				UInt64 data = ((UInt64)(value & 0xFFFFFFFC)) << 32;
				double num = TreatUInt64AsDouble(data);
				if(muled)
					num /= 100;
				return num;
			}
			else
			{
				Int32 num = (int)(value & 0xFFFFFFFC) >> 2;
				if(muled)
					num /= 100;
				return num;
			}
		}

		public static double TreatUInt64AsDouble(UInt64 data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			return BitConverter.ToDouble(bytes, 0);
		}
	}
}
