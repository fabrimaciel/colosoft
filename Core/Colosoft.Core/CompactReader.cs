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
	/// Representa um letira de dados compacto.
	/// </summary>
	public abstract class CompactReader
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected CompactReader()
		{
		}

		/// <summary>
		/// Realiza a leitura dos dados que estão no buffer informado.
		/// </summary>
		/// <param name="buffer">Buffer de onde os dados serão lidos.</param>
		/// <param name="index">Indice inicial de onde os dados serão lidos.</param>
		/// <param name="count">Quantidade de bytes que serão lidos.</param>
		/// <returns>Quantidade de bytes lidos.</returns>
		public abstract int Read(byte[] buffer, int index, int count);

		/// <summary>
		/// Realiza a leitura dos dados que estão no buffer informado.
		/// </summary>
		/// <param name="buffer">Buffer de onde os dados serão lidos.</param>
		/// <param name="index">Indice inicial de onde os dados serão lidos.</param>
		/// <param name="count">Quantidade de chars que serão lidos.</param>
		/// <returns>Quantidade de chars lidos.</returns>
		public abstract int Read(char[] buffer, int index, int count);

		/// <summary>
		/// Lê um valor <see cref="Boolean"/>.
		/// </summary>
		/// <returns></returns>
		public abstract bool ReadBoolean();

		/// <summary>
		/// Lê um <see cref="Byte"/>.
		/// </summary>
		/// <returns></returns>
		public abstract byte ReadByte();

		/// <summary>
		/// Lê a quantidade de bytes informada.
		/// </summary>
		/// <param name="count">Quantidade de bytes que serão lidos.</param>
		/// <returns></returns>
		public abstract byte[] ReadBytes(int count);

		/// <summary>
		/// Lê um <see cref="Char"/>.
		/// </summary>
		/// <returns></returns>
		public abstract char ReadChar();

		/// <summary>
		/// Lê a quantidade de <see cref="Char"/> informada.
		/// </summary>
		/// <param name="count">Quantidade de <see cref="Char"/> que serão lidas.</param>
		/// <returns></returns>
		public abstract char[] ReadChars(int count);

		/// <summary>
		/// Lê um <see cref="DateTime"/>.
		/// </summary>
		/// <returns></returns>
		public abstract DateTime ReadDateTime();

		/// <summary>
		/// Lê um <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <returns></returns>
		public abstract DateTimeOffset ReadDateTimeOffset();

		/// <summary>
		/// Lê um <see cref="TimeSpan"/>.
		/// </summary>
		/// <returns></returns>
		public abstract TimeSpan ReadTimeSpan();

		/// <summary>
		/// Lê um <see cref="Decimal"/>.
		/// </summary>
		/// <returns></returns>
		public abstract decimal ReadDecimal();

		/// <summary>
		/// Lê um <see cref="Double"/>.
		/// </summary>
		/// <returns></returns>
		public abstract double ReadDouble();

		/// <summary>
		/// Lê um <see cref="Guid"/>.
		/// </summary>
		/// <returns></returns>
		public abstract Guid ReadGuid();

		/// <summary>
		/// Lê um <see cref="Int16"/>.
		/// </summary>
		/// <returns></returns>
		public abstract short ReadInt16();

		/// <summary>
		/// Lê um <see cref="Int32"/>.
		/// </summary>
		/// <returns></returns>
		public abstract int ReadInt32();

		/// <summary>
		/// Lê um <see cref="Int64"/>.
		/// </summary>
		/// <returns></returns>
		public abstract long ReadInt64();

		/// <summary>
		/// Lê um <see cref="Object"/>
		/// </summary>
		/// <returns></returns>
		public abstract object ReadObject();

		/// <summary>
		/// Lê um objeto do tipo informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será deserializado.</typeparam>
		/// <returns></returns>
		public abstract T ReadObjectAs<T>();

		/// <summary>
		/// Lê um objeto do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será deserializado.</param>
		/// <returns></returns>
		public abstract object ReadObjectAs(Type type);

		/// <summary>
		/// Lê um <see cref="SByte"/>.
		/// </summary>
		/// <returns></returns>
		public virtual sbyte ReadSByte()
		{
			return 0;
		}

		/// <summary>
		/// Lê um <see cref="Single"/>.
		/// </summary>
		/// <returns></returns>
		public abstract float ReadSingle();

		/// <summary>
		/// Lê um <see cref="String"/>.
		/// </summary>
		/// <returns></returns>
		public abstract string ReadString();

		/// <summary>
		/// Lê um <see cref="UInt16"/>.
		/// </summary>
		/// <returns></returns>
		public virtual ushort ReadUInt16()
		{
			return 0;
		}

		/// <summary>
		/// Lê um <see cref="UInt32"/>.
		/// </summary>
		/// <returns></returns>
		public virtual uint ReadUInt32()
		{
			return 0;
		}

		/// <summary>
		/// Lê um <see cref="UInt64"/>.
		/// </summary>
		/// <returns></returns>
		public virtual ulong ReadUInt64()
		{
			return 0;
		}

		/// <summary>
		/// Lê os dados do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual object Read(Type type)
		{
			type.Require("dataType").NotNull();
			if(type.IsGenericType)
			{
				var genericDefinition = type.GetGenericTypeDefinition();
				if(genericDefinition == typeof(Nullable<>))
				{
					type = type.GetGenericArguments()[0];
					if(ReadByte() == 1)
						return null;
				}
			}
			switch(type.FullName)
			{
			case "System.String":
				return ReadString();
			case "System.Int16":
				return ReadInt16();
			case "System.UInt16":
				return ReadUInt16();
			case "System.Int32":
				return ReadInt32();
			case "System.UInt32":
				return ReadUInt32();
			case "System.Int64":
				return ReadInt64();
			case "System.UInt64":
				return ReadUInt64();
			case "System.Single":
				return ReadSingle();
			case "System.Double":
				return ReadDouble();
			case "System.Boolean":
				return ReadBoolean();
			case "System.Char":
				return ReadChar();
			case "System.Byte":
				return ReadByte();
			case "System.DateTime":
				return ReadDateTime();
			case "System.DateTimeOffset":
				return ReadDateTimeOffset();
			case "System.Decimal":
				return ReadDecimal();
			case "System.Byte[]":
				var bytesCount = ReadInt32();
				return ReadBytes(bytesCount);
			default:
				throw new NotSupportedException(string.Format("Not support type '{0}'", type.FullName));
			}
		}

		/// <summary>
		/// Lê os dados do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual void Skip(Type type)
		{
			type.Require("dataType").NotNull();
			if(type.IsGenericType)
			{
				var genericDefinition = type.GetGenericTypeDefinition();
				if(genericDefinition == typeof(Nullable<>))
				{
					type = type.GetGenericArguments()[0];
					if(ReadByte() == 1)
						return;
				}
			}
			switch(type.FullName)
			{
			case "System.String":
				SkipString();
				break;
			case "System.Int16":
				SkipInt16();
				break;
			case "System.UInt16":
				SkipUInt16();
				break;
			case "System.Int32":
				SkipInt32();
				break;
			case "System.UInt32":
				SkipUInt32();
				break;
			case "System.Int64":
				SkipInt64();
				break;
			case "System.UInt64":
				SkipUInt64();
				break;
			case "System.Single":
				SkipSingle();
				break;
			case "System.Double":
				SkipDouble();
				break;
			case "System.Boolean":
				SkipBoolean();
				break;
			case "System.Char":
				SkipChar();
				break;
			case "System.Byte":
				SkipByte();
				break;
			case "System.DateTime":
				SkipDateTime();
				break;
			case "System.DateTimeOffset":
				SkipDateTimeOffset();
				break;
			case "System.Decimal":
				SkipDecimal();
				break;
			case "System.Byte[]":
				var bytesCount = ReadInt32();
				SkipBytes(bytesCount);
				break;
			default:
				throw new NotSupportedException(string.Format("Not support type '{0}'", type.FullName));
			}
		}

		/// <summary>
		/// Salta um <see cref="Boolean"/>.
		/// </summary>
		public abstract void SkipBoolean();

		/// <summary>
		/// Salta um <see cref="Byte"/>.
		/// </summary>
		public abstract void SkipByte();

		/// <summary>
		/// Salta a quantidade de bytes informada.
		/// </summary>
		/// <param name="count">Quantidade de bytes que serão saltados.</param>
		public abstract void SkipBytes(int count);

		/// <summary>
		/// Salta um <see cref="Char"/>.
		/// </summary>
		public abstract void SkipChar();

		/// <summary>
		/// Salta a quantidade de <see cref="Char"/> informada.
		/// </summary>
		/// <param name="count"></param>
		public abstract void SkipChars(int count);

		/// <summary>
		/// Salta um <see cref="DateTime"/>.
		/// </summary>
		public abstract void SkipDateTime();

		/// <summary>
		/// Salta um <see cref="DateTimeOffset"/>.
		/// </summary>
		public abstract void SkipDateTimeOffset();

		/// <summary>
		/// Salta um <see cref="TimeSpan"/>.
		/// </summary>
		public abstract void SkipTimeSpan();

		/// <summary>
		/// Salta um <see cref="Decimal"/>.
		/// </summary>
		public abstract void SkipDecimal();

		/// <summary>
		/// Salta um <see cref="Double"/>.
		/// </summary>
		public abstract void SkipDouble();

		/// <summary>
		/// Salta um <see cref="Guid"/>.
		/// </summary>
		public abstract void SkipGuid();

		/// <summary>
		/// Salta um <see cref="Int16"/>.
		/// </summary>
		public abstract void SkipInt16();

		/// <summary>
		/// Salta um <see cref="Int32"/>.
		/// </summary>
		public abstract void SkipInt32();

		/// <summary>
		/// Salta um <see cref="Int64"/>.
		/// </summary>
		public abstract void SkipInt64();

		/// <summary>
		/// Salta um <see cref="Object"/>.
		/// </summary>
		public abstract void SkipObject();

		/// <summary>
		/// Salta um objeto do tipo informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado para saltar.</typeparam>
		public abstract void SkipObjectAs<T>();

		/// <summary>
		/// Salta um objeto do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será usado para saltar.</param>
		public abstract void SkipObjectAs(Type type);

		/// <summary>
		/// Salva um <see cref="Byte"/>.
		/// </summary>
		public virtual void SkipSByte()
		{
		}

		/// <summary>
		/// Salta um <see cref="Single"/>.
		/// </summary>
		public abstract void SkipSingle();

		/// <summary>
		/// Salta um <see cref="String"/>
		/// </summary>
		public abstract void SkipString();

		/// <summary>
		/// Salta um <see cref="UInt16"/>.
		/// </summary>
		public virtual void SkipUInt16()
		{
		}

		/// <summary>
		/// Salta um <see cref="UInt32"/>
		/// </summary>
		public virtual void SkipUInt32()
		{
		}

		/// <summary>
		/// Salta um <see cref="UInt64"/>.
		/// </summary>
		public virtual void SkipUInt64()
		{
		}
	}
}
