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
using System.Collections;
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization
{
	/// <summary>
	/// Representa um vetor virtual.
	/// </summary>
	public class VirtualArray : ICompactSerializable
	{
		private IList _baseArray;

		private long _size;

		/// <summary>
		/// Vetor base.
		/// </summary>
		public IList BaseArray
		{
			get
			{
				return _baseArray;
			}
		}

		/// <summary>
		/// Tamanho do vetor.
		/// </summary>
		public long Size
		{
			get
			{
				return _size;
			}
		}

		/// <summary>
		/// Cria a instancia com os dados do vetor informado.
		/// </summary>
		/// <param name="array"></param>
		public VirtualArray(IList array)
		{
			this._baseArray = array;
			for(int i = 0; i < array.Count; i++)
			{
				byte[] buffer = array[i] as byte[];
				if(buffer != null)
				{
					this._size += buffer.Length;
				}
			}
		}

		/// <summary>
		/// Cria a instancia já definindo o tamanho inicial.
		/// </summary>
		/// <param name="size"></param>
		public VirtualArray(long size)
		{
			this._size = size;
			int num = 0x13c00;
			int num2 = (int)(size / ((long)num));
			num2 += ((size - (num2 * num)) != 0) ? 1 : 0;
			_baseArray = new Array[num2];
			for(int i = 0; i < num2; i++)
			{
				byte[] buffer = null;
				if(size >= 0x13c00)
				{
					buffer = new byte[0x13c00];
					size -= 0x13c00;
				}
				else
					buffer = new byte[size];
				_baseArray[i] = buffer;
			}
		}

		/// <summary>
		/// Copia o buffer de dados.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public int CopyData(byte[] buffer, int offset, int length)
		{
			if((offset + length) > buffer.Length)
				throw new ArgumentException("Length plus offset is greater than buffer size");
			int num = (length >= Size) ? ((int)Size) : length;
			int num2 = num;
			for(int i = 0; num > 0; i++)
			{
				byte[] src = (byte[])_baseArray[i];
				if(src != null)
				{
					int count = Math.Min(src.Length, num);
					Buffer.BlockCopy(src, 0, buffer, offset, count);
					offset += count;
					num -= count;
				}
			}
			return num2;
		}

		/// <summary>
		/// Faz uma cópia dos dados do vetor.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="srcIndex"></param>
		/// <param name="dst"></param>
		/// <param name="dstIndex"></param>
		/// <param name="count"></param>
		public static void CopyData(VirtualArray src, VirtualIndex srcIndex, VirtualArray dst, VirtualIndex dstIndex, int count)
		{
			CopyData(src, srcIndex, dst, dstIndex, count, false);
		}

		/// <summary>
		/// Faz uma cópia dos dados do vetor.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="srcIndex"></param>
		/// <param name="dst"></param>
		/// <param name="dstIndex"></param>
		/// <param name="count"></param>
		/// <param name="allowExpantion"></param>
		public static void CopyData(VirtualArray src, VirtualIndex srcIndex, VirtualArray dst, VirtualIndex dstIndex, int count, bool allowExpantion)
		{
			if(((src != null) && (dst != null)) && ((srcIndex != null) && (dstIndex != null)))
			{
				if(src.Size < srcIndex.IndexValue)
				{
					throw new IndexOutOfRangeException();
				}
				srcIndex = srcIndex.Clone();
				dstIndex = dstIndex.Clone();
				while (count > 0)
				{
					Array array = src._baseArray[srcIndex.YIndex] as Array;
					int num = 0x13c00 - srcIndex.XIndex;
					if(num > count)
					{
						num = count;
					}
					Array array2 = null;
					if(dst._baseArray.Count > dstIndex.YIndex)
					{
						array2 = dst._baseArray[dstIndex.YIndex] as Array;
					}
					int num2 = 0x13c00 - dstIndex.XIndex;
					if(num2 > num)
					{
						num2 = num;
					}
					if(((array2 == null) || (num2 > (array2.Length - dstIndex.XIndex))) && allowExpantion)
					{
						if(array2 == null)
						{
							array2 = new byte[num2];
							dst._baseArray.Add(array2);
						}
						else
						{
							byte[] buffer = new byte[(num2 + array2.Length) - (array2.Length - dstIndex.XIndex)];
							Buffer.BlockCopy(array2, 0, buffer, 0, array2.Length);
							array2 = buffer;
							dst._baseArray[dstIndex.YIndex] = array2;
						}
					}
					Buffer.BlockCopy(array, srcIndex.XIndex, array2, dstIndex.XIndex, num2);
					count -= num2;
					srcIndex.IncrementBy(num2);
					dstIndex.IncrementBy(num2);
				}
			}
		}

		/// <summary>
		/// Recupera o valor na posição do indice informado.
		/// </summary>
		/// <param name="vIndex"></param>
		/// <returns></returns>
		public byte GetValueAt(VirtualIndex vIndex)
		{
			byte[] buffer = _baseArray[vIndex.YIndex] as byte[];
			return buffer[vIndex.XIndex];
		}

		/// <summary>
		/// Define o valor na posição do indice informado.
		/// </summary>
		/// <param name="vIndex"></param>
		/// <param name="value"></param>
		public void SetValueAt(VirtualIndex vIndex, byte value)
		{
			byte[] buffer = _baseArray[vIndex.YIndex] as byte[];
			buffer[vIndex.XIndex] = value;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.Write(_size);
			writer.WriteObject(_baseArray);
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_size = reader.ReadInt64();
			_baseArray = reader.ReadObject() as IList;
		}
	}
}
