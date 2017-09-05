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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa um objeto em estado binario.
	/// </summary>
	[Serializable]
	public class UserBinaryObject : ICompactSerializable, IStreamItem
	{
		private const int LARGE_OBJECT_SIZE = 0x13c00;

		private System.Collections.ArrayList _data;

		private int _index;

		private int _noOfChunks;

		/// <summary>
		/// Vetor com os dados da instancia.
		/// </summary>
		public Array Data
		{
			get
			{
				return _data.ToArray();
			}
		}

		/// <summary>
		/// Lista de dados da instancia.
		/// </summary>
		public List<byte[]> DataList
		{
			get
			{
				var items = new List<byte[]>();
				foreach (byte[] buffer in _data)
					items.Add(buffer);
				return items;
			}
		}

		/// <summary>
		/// Quantidade de itens da instancia.
		/// </summary>
		public int Length
		{
			get
			{
				return this.Size;
			}
			set
			{
			}
		}

		/// <summary>
		/// Tamanho de dados da instancia.
		/// </summary>
		public int Size
		{
			get
			{
				int num = 0;
				for(int i = 0; i < _noOfChunks; i++)
					num += ((byte[])_data[i]).Length;
				return num;
			}
		}

		/// <summary>
		/// Cria uma instancia já definindo o vetor de dados.
		/// </summary>
		/// <param name="data"></param>
		public UserBinaryObject(Array data)
		{
			_data = new System.Collections.ArrayList();
			_noOfChunks = data.Length;
			foreach (byte[] buffer in data)
			{
				_data.Add(buffer);
			}
		}

		/// <summary>
		/// Cria uma instanci já definindo o numero de chunks.
		/// </summary>
		/// <param name="noOfChunks"></param>
		public UserBinaryObject(int noOfChunks)
		{
			_data = new System.Collections.ArrayList();
			_noOfChunks = noOfChunks;
		}

		/// <summary>
		/// Adiciona dados para a instancia.
		/// </summary>
		/// <param name="dataChunk"></param>
		public void AddDataChunk(byte[] dataChunk)
		{
			if(_data != null && _index < _noOfChunks)
			{
				_data.Insert(_index, dataChunk);
				_index++;
			}
		}

		/// <summary>
		/// Cria um objeto binario com os dados informados.
		/// </summary>
		/// <param name="largbyteArray"></param>
		/// <returns></returns>
		public static UserBinaryObject CreateUserBinaryObject(byte[] largbyteArray)
		{
			UserBinaryObject obj2 = null;
			if(largbyteArray != null)
			{
				int noOfChunks = largbyteArray.Length / LARGE_OBJECT_SIZE;
				noOfChunks += ((largbyteArray.Length - (noOfChunks * LARGE_OBJECT_SIZE)) != 0) ? 1 : 0;
				obj2 = new UserBinaryObject(noOfChunks);
				int srcOffset = 0;
				int count = 0;
				for(int i = 1; i <= noOfChunks; i++)
				{
					count = largbyteArray.Length - srcOffset;
					if(count > LARGE_OBJECT_SIZE)
						count = LARGE_OBJECT_SIZE;
					byte[] dst = new byte[count];
					Buffer.BlockCopy(largbyteArray, srcOffset, dst, 0, count);
					srcOffset += count;
					obj2.AddDataChunk(dst);
				}
			}
			return obj2;
		}

		/// <summary>
		/// Cria um objeto binario com os dados informados.
		/// </summary>
		/// <param name="largbyteArray"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static UserBinaryObject CreateUserBinaryObject(byte[] largbyteArray, int startIndex, int count)
		{
			UserBinaryObject obj2 = null;
			if(largbyteArray != null)
			{
				int noOfChunks = count / LARGE_OBJECT_SIZE;
				noOfChunks += ((count - (noOfChunks * LARGE_OBJECT_SIZE)) != 0) ? 1 : 0;
				obj2 = new UserBinaryObject(noOfChunks);
				int num2 = 0;
				int num3 = 0;
				for(int i = 1; i <= noOfChunks; i++)
				{
					num3 = count - num2;
					if(num3 > LARGE_OBJECT_SIZE)
						num3 = LARGE_OBJECT_SIZE;
					byte[] dst = new byte[num3];
					Buffer.BlockCopy(largbyteArray, startIndex, dst, 0, num3);
					num2 += num3;
					startIndex += num3;
					obj2.AddDataChunk(dst);
				}
			}
			return obj2;
		}

		/// <summary>
		/// Recupera todos os bytes da instancia.
		/// </summary>
		/// <returns></returns>
		public byte[] GetFullObject()
		{
			byte[] array = null;
			if(this.Size > 0)
			{
				array = new byte[this.Size];
				int index = 0;
				byte[] buffer2 = null;
				for(int i = 0; i < _data.Count; i++)
				{
					buffer2 = (byte[])_data[i];
					if(buffer2 != null)
					{
						buffer2.CopyTo(array, index);
						index += buffer2.Length;
					}
				}
			}
			return array;
		}

		/// <summary>
		/// Lê os dados da instancia.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public VirtualArray Read(int offset, int length)
		{
			VirtualArray dst = null;
			int num = this.Length;
			if(offset >= num)
				return new VirtualArray(0);
			if((offset + length) > num)
				length -= (offset + length) - num;
			VirtualArray src = new VirtualArray(_data);
			dst = new VirtualArray((long)length);
			VirtualIndex srcIndex = new VirtualIndex(offset);
			VirtualIndex dstIndex = new VirtualIndex();
			VirtualArray.CopyData(src, srcIndex, dst, dstIndex, length);
			return dst;
		}

		/// <summary>
		/// Escreve o buffer no item.
		/// </summary>
		/// <param name="vBuffer">buffer com os dados que serão inseridos.</param>
		/// <param name="srcOffset">Offset da origem dom buffer.</param>
		/// <param name="dstOffset">Offset de destino.</param>
		/// <param name="length">Quantidade de dados que serão escritors</param>
		public void Write(VirtualArray vBuffer, int srcOffset, int dstOffset, int length)
		{
			if(vBuffer != null)
			{
				VirtualArray dst = new VirtualArray(_data);
				VirtualArray.CopyData(vBuffer, new VirtualIndex(srcOffset), dst, new VirtualIndex(dstOffset), length, true);
				_noOfChunks = _data.Count;
			}
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_noOfChunks = reader.ReadInt32();
			_index = reader.ReadInt32();
			if(_noOfChunks > 0)
			{
				_data = new System.Collections.ArrayList(_noOfChunks);
				for(int i = 0; i < _noOfChunks; i++)
					_data.Insert(i, reader.ReadObject() as byte[]);
			}
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.Write(_noOfChunks);
			writer.Write(_index);
			for(int i = 0; i < _noOfChunks; i++)
				writer.WriteObject(_data[i]);
		}
	}
}
