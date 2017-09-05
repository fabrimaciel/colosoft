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
	/// Implementação do escritor binário compacto.
	/// </summary>
	public class CompactBinaryWriter : CompactWriter, IDisposable
	{
		private SerializationContext _context;

		private BinaryWriter _writer;

		/// <summary>
		/// Escritor base.
		/// </summary>
		internal BinaryWriter BaseWriter
		{
			get
			{
				return _writer;
			}
		}

		/// <summary>
		/// Nome do contexto do cache.
		/// </summary>
		public string CacheContext
		{
			get
			{
				return _context.CacheContext;
			}
		}

		/// <summary>
		/// Contexto de serialização.
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
		/// <param name="output">Stream base para a instancia.</param>
		public CompactBinaryWriter(Stream output) : this(output, new UTF8Encoding(true))
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="output">Stream base para a instancia.</param>
		/// <param name="encoding"><see cref="Encoding"/> que será usado na serialização.</param>
		public CompactBinaryWriter(Stream output, Encoding encoding)
		{
			_context = new SerializationContext();
			_writer = new BinaryWriter(output, encoding);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(bool value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(byte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ch"></param>
		public override void Write(char ch)
		{
			_writer.Write(ch);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(DateTime value)
		{
			_writer.Write(value.Ticks);
		}

		/// <summary>
		/// Escreve um <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <param name="value"></param>
		public override void Write(DateTimeOffset value)
		{
			Write(value.DateTime);
			Write(value.Offset);
		}

		/// <summary>
		/// Escreve um <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="value"></param>
		public override void Write(TimeSpan value)
		{
			_writer.Write(value.Ticks);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(decimal value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(short value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(int value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(long value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		public override void Write(byte[] buffer)
		{
			if(buffer != null)
			{
				_writer.Write(buffer);
			}
			else
			{
				this.WriteObject(null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(float value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(double value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(Guid value)
		{
			_writer.Write(value.ToByteArray());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(sbyte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(uint value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="chars"></param>
		public override void Write(char[] chars)
		{
			if(chars != null)
			{
				_writer.Write(chars);
			}
			else
			{
				this.WriteObject(null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(string value)
		{
			var encoding = System.Text.Encoding.UTF8;
			var count = value == null ? -1 : encoding.GetByteCount(value);
			_writer.Write(count);
			if(value != null)
				_writer.Write(encoding.GetBytes(value), 0, count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(ushort value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public override void Write(ulong value)
		{
			_writer.Write(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int index, int count)
		{
			if(buffer != null)
			{
				_writer.Write(buffer, index, count);
			}
			else
			{
				this.WriteObject(null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="chars"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public override void Write(char[] chars, int index, int count)
		{
			if(chars != null)
			{
				_writer.Write(chars, index, count);
			}
			else
			{
				this.WriteObject(null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="graph"></param>
		public override void WriteObject(object graph)
		{
			ISerializationSurrogate surrogateForObject = TypeSurrogateSelector.GetSurrogateForObject(graph, _context.CacheContext);
			_writer.Write(surrogateForObject.TypeHandle);
			try
			{
				surrogateForObject.Write(this, graph);
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
		/// <param name="graph"></param>
		public override void WriteObjectAs<T>(T graph)
		{
			if(graph == null)
				throw new ArgumentNullException("graph");
			TypeSurrogateSelector.GetSurrogateForType(typeof(T), _context.CacheContext).Write(this, graph);
		}

		/// <summary>
		/// Escreve o objeto do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="graph"></param>
		public override void WriteObjectAs(Type type, object graph)
		{
			type.Require("type").NotNull();
			if(graph == null)
				throw new ArgumentNullException("graph");
			TypeSurrogateSelector.GetSurrogateForType(type, _context.CacheContext).Write(this, graph);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose()
		{
			if(_writer != null)
				_writer.Close();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="closeStream">True para fecha a stream quando a instancia for liberada.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose(bool closeStream)
		{
			if(closeStream)
				_writer.Close();
			_writer = null;
		}
	}
}
