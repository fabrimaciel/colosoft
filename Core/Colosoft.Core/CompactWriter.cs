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

namespace Colosoft.Serialization.IO
{
	/// <summary>
	/// Representa um classe de escrita compacta.
	/// </summary>
	public abstract class CompactWriter
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected CompactWriter()
		{
		}

		/// <summary>
		/// Esvreve um <see cref="Boolean"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(bool value);

		/// <summary>
		/// Escreve um <see cref="Byte"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(byte value);

		/// <summary>
		/// Escreve um <see cref="Char"/>.
		/// </summary>
		/// <param name="ch"></param>
		public abstract void Write(char ch);

		/// <summary>
		/// Escreve um <see cref="DateTime"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(DateTime value);

		/// <summary>
		/// Escreve um <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(DateTimeOffset value);

		/// <summary>
		/// Escreve um <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(TimeSpan value);

		/// <summary>
		/// Escreve um <see cref="Decimal"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(decimal value);

		/// <summary>
		/// Escreve um <see cref="Double"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(double value);

		/// <summary>
		/// Escreve um <see cref="Int16"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(short value);

		/// <summary>
		/// Escreve um <see cref="Int32"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(int value);

		/// <summary>
		/// Escreve um <see cref="Int64"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(long value);

		/// <summary>
		/// Escreve um <see cref="Single"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(float value);

		/// <summary>
		/// Escreve o buffer informado.
		/// </summary>
		/// <param name="buffer"></param>
		public abstract void Write(byte[] buffer);

		/// <summary>
		/// Escreve um <see cref="Guid"/>
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(Guid value);

		/// <summary>
		/// Escreve um <see cref="SByte"/>.
		/// </summary>
		/// <param name="value"></param>
		public virtual void Write(sbyte value)
		{
		}

		/// <summary>
		/// Escreve um <see cref="Char"/>.
		/// </summary>
		/// <param name="chars"></param>
		public abstract void Write(char[] chars);

		/// <summary>
		/// Escreve um <see cref="String"/>.
		/// </summary>
		/// <param name="value"></param>
		public abstract void Write(string value);

		/// <summary>
		/// Escreve um <see cref="UInt16"/>.
		/// </summary>
		/// <param name="value"></param>
		public virtual void Write(ushort value)
		{
		}

		/// <summary>
		/// Escreve um <see cref="UInt32"/>.
		/// </summary>
		/// <param name="value"></param>
		public virtual void Write(uint value)
		{
		}

		/// <summary>
		/// Escreve um <see cref="UInt64"/>.
		/// </summary>
		/// <param name="value"></param>
		public virtual void Write(ulong value)
		{
		}

		/// <summary>
		/// Escreve o valor do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public virtual void Write(Type type, object value)
		{
			type.Require("dataType").NotNull();
			if(type.IsGenericType)
			{
				var genericDefinition = type.GetGenericTypeDefinition();
				if(genericDefinition == typeof(Nullable<>))
				{
					type = type.GetGenericArguments()[0];
					Write((byte)(value == null ? 1 : 0));
					if(value == null)
						return;
				}
			}
			switch(type.FullName)
			{
			case "System.String":
				Write((string)value);
				break;
			case "System.Int16":
				Write((short)value);
				break;
			case "System.UInt16":
				Write((ushort)value);
				break;
			case "System.Int32":
				Write((int)value);
				break;
			case "System.UInt32":
				Write((uint)value);
				break;
			case "System.Int64":
				Write((long)value);
				break;
			case "System.UInt64":
				Write((ulong)value);
				break;
			case "System.Single":
				Write((float)value);
				break;
			case "System.Double":
				Write((double)value);
				break;
			case "System.Boolean":
				Write((bool)value);
				break;
			case "System.Char":
				Write((char)value);
				break;
			case "System.Byte":
				Write((byte)value);
				break;
			case "System.DateTime":
				Write((DateTime)value);
				break;
			case "System.DateTimeOffset":
				Write((DateTimeOffset)value);
				break;
			case "System.Decimal":
				Write((decimal)value);
				break;
			case "System.Byte[]":
				Write(((byte[])value).Length);
				Write((byte[])value);
				break;
			default:
				throw new NotSupportedException(string.Format("Not support type '{0}'", type.FullName));
			}
		}

		/// <summary>
		/// Escreve o buffer informado.
		/// </summary>
		/// <param name="buffer">Buffer onde estão os dados que serão escritos.</param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public abstract void Write(byte[] buffer, int index, int count);

		/// <summary>
		/// Escreve o buffer informado.
		/// </summary>
		/// <param name="chars"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		public abstract void Write(char[] chars, int index, int count);

		/// <summary>
		/// Escreve o objeto informado.
		/// </summary>
		/// <param name="graph"></param>
		public abstract void WriteObject(object graph);

		/// <summary>
		/// Escreve o objeto do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="graph"></param>
		public abstract void WriteObjectAs<T>(T graph);

		/// <summary>
		/// Escreve o objeto do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="graph"></param>
		public abstract void WriteObjectAs(Type type, object graph);
	}
}
