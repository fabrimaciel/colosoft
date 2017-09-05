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
using Colosoft.Serialization.Surrogates;

namespace Colosoft.Serialization.IO
{
	/// <summary>
	/// Implementação do leitor compacto para estruturas binárias.
	/// </summary>
	public class CompactBinaryReader : CompactReader, IDisposable
	{
		private SerializationContext _context;

		private BinaryReader _reader;

		/// <summary>
		/// Leitor base.
		/// </summary>
		internal BinaryReader BaseReader
		{
			get
			{
				return _reader;
			}
		}

		/// <summary>
		/// Nome do contexto associado.
		/// </summary>
		public string CacheContext
		{
			get
			{
				return _context.CacheContext;
			}
		}

		/// <summary>
		/// Contexto ser serialização.
		/// </summary>
		internal SerializationContext Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="input">Instancia do stream que será usado com base.</param>
		public CompactBinaryReader(Stream input) : this(input, new UTF8Encoding(true))
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input">Instancia do stream que será usado com base.</param>
		/// <param name="encoding">Enconding que será usado na gravação.</param>
		public CompactBinaryReader(Stream input, Encoding encoding)
		{
			_context = new SerializationContext();
			_reader = new BinaryReader(input, encoding);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int index, int count)
		{
			return _reader.Read(buffer, index, count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(char[] buffer, int index, int count)
		{
			return _reader.Read(buffer, index, count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool ReadBoolean()
		{
			return _reader.ReadBoolean();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override byte ReadByte()
		{
			return _reader.ReadByte();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public override byte[] ReadBytes(int count)
		{
			return _reader.ReadBytes(count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override char ReadChar()
		{
			return _reader.ReadChar();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public override char[] ReadChars(int count)
		{
			return _reader.ReadChars(count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override DateTime ReadDateTime()
		{
			return new DateTime(_reader.ReadInt64());
		}

		/// <summary>
		/// Lê um <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <returns></returns>
		public override DateTimeOffset ReadDateTimeOffset()
		{
			var dateTime = ReadDateTime();
			var offset = ReadTimeSpan();
			return new DateTimeOffset(dateTime, offset);
		}

		/// <summary>
		/// Lê um <see cref="TimeSpan"/>.
		/// </summary>
		/// <returns></returns>
		public override TimeSpan ReadTimeSpan()
		{
			return new TimeSpan(ReadInt64());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override decimal ReadDecimal()
		{
			return _reader.ReadDecimal();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override double ReadDouble()
		{
			return _reader.ReadDouble();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Guid ReadGuid()
		{
			return new Guid(_reader.ReadBytes(0x10));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override short ReadInt16()
		{
			return _reader.ReadInt16();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int ReadInt32()
		{
			return _reader.ReadInt32();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override long ReadInt64()
		{
			return _reader.ReadInt64();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override object ReadObject()
		{
			short handle = _reader.ReadInt16();
			ISerializationSurrogate surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, _context.CacheContext);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, _reader.ReadInt16(), _context.CacheContext);
			object obj2 = null;
			try
			{
				obj2 = surrogateForTypeHandle.Read(this);
			}
			catch(CompactSerializationException)
			{
				throw;
			}
			catch(Exception exception)
			{
				throw new CompactSerializationException(exception.Message);
			}
			return obj2;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public override T ReadObjectAs<T>()
		{
			return (T)ReadObjectAs(typeof(T));
		}

		/// <summary>
		/// Lê um objeto do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será deserializado.</param>
		/// <returns></returns>
		public override object ReadObjectAs(Type type)
		{
			if(typeof(ICompactSerializable).IsAssignableFrom(type))
			{
				ICompactSerializable graph = Activator.CreateInstance(type) as ICompactSerializable;
				graph.Deserialize(this);
				return graph;
			}
			return TypeSurrogateSelector.GetSurrogateForType(type, _context.CacheContext).Read(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override sbyte ReadSByte()
		{
			return _reader.ReadSByte();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override float ReadSingle()
		{
			return _reader.ReadSingle();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ReadString()
		{
			var count = _reader.ReadInt32();
			if(count > 0)
			{
				var buffer = new byte[count];
				while (count > 0)
				{
					var read = _reader.Read(buffer, 0, count);
					count -= read;
				}
				return System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
			}
			else
				return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override ushort ReadUInt16()
		{
			return _reader.ReadUInt16();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override uint ReadUInt32()
		{
			return _reader.ReadUInt32();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override ulong ReadUInt64()
		{
			return _reader.ReadUInt64();
		}

		/// <summary>
		/// Verifica se o valor lido é um substituto identificando salto.
		/// </summary>
		/// <param name="readObjectValue"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public object IfSkip(object readObjectValue, object defaultValue)
		{
			if(readObjectValue is SkipSerializationSurrogate)
				return defaultValue;
			return readObjectValue;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipBoolean()
		{
			long num;
			Stream baseStream = _reader.BaseStream;
			baseStream.Position = (num = baseStream.Position) + 1;
			_reader.BaseStream.Position = num;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipByte()
		{
			long num;
			Stream baseStream = _reader.BaseStream;
			baseStream.Position = (num = baseStream.Position) + 1;
			_reader.BaseStream.Position = num;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		public override void SkipBytes(int count)
		{
			_reader.BaseStream.Position += count;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipChar()
		{
			long num;
			Stream baseStream = _reader.BaseStream;
			baseStream.Position = (num = baseStream.Position) + 1;
			_reader.BaseStream.Position = num;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		public override void SkipChars(int count)
		{
			_reader.BaseStream.Position += count;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipDateTime()
		{
			this.SkipInt64();
		}

		/// <summary>
		/// Salta um <see cref="DateTimeOffset"/>.
		/// </summary>
		public override void SkipDateTimeOffset()
		{
			this.SkipDateTime();
			this.SkipTimeSpan();
		}

		/// <summary>
		/// Salta um <see cref="TimeSpan"/>.
		/// </summary>
		public override void SkipTimeSpan()
		{
			this.SkipInt64();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipDecimal()
		{
			_reader.BaseStream.Position += 0x10;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipDouble()
		{
			_reader.BaseStream.Position += 8;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipGuid()
		{
			_reader.BaseStream.Position += 0x10;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipInt16()
		{
			_reader.BaseStream.Position += 2;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipInt32()
		{
			_reader.BaseStream.Position += 4;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipInt64()
		{
			_reader.BaseStream.Position += 8;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipObject()
		{
			short handle = _reader.ReadInt16();
			var surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForTypeHandle(handle, _context.CacheContext);
			if(surrogateForTypeHandle == null)
				surrogateForTypeHandle = TypeSurrogateSelector.GetSurrogateForSubTypeHandle(handle, _reader.ReadInt16(), _context.CacheContext);
			try
			{
				surrogateForTypeHandle.Skip(this);
			}
			catch(CompactSerializationException)
			{
				throw;
			}
			catch(Exception exception)
			{
				throw new CompactSerializationException(exception.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public override void SkipObjectAs<T>()
		{
			TypeSurrogateSelector.GetSurrogateForType(typeof(T), _context.CacheContext).Skip(this);
		}

		/// <summary>
		/// Salta um objeto do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será usado para saltar.</param>
		public override void SkipObjectAs(Type type)
		{
			TypeSurrogateSelector.GetSurrogateForType(type, _context.CacheContext).Skip(this);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipSByte()
		{
			Stream baseStream = _reader.BaseStream;
			baseStream.Position++;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipSingle()
		{
			_reader.BaseStream.Position += 4;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipString()
		{
			_reader.ReadString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipUInt16()
		{
			_reader.BaseStream.Position += 2;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipUInt32()
		{
			_reader.BaseStream.Position += 4;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void SkipUInt64()
		{
			_reader.BaseStream.Position += 8;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose()
		{
			if(_reader != null)
				_reader.Close();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="closeStream">True para libera fechando a stream associada.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose(bool closeStream)
		{
			if(closeStream)
				_reader.Close();
			_reader = null;
		}
	}
}
