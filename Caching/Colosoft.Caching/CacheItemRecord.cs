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

namespace Colosoft.Caching
{
	/// <summary>
	/// Implementação de um registro de um item de cache.
	/// </summary>
	[Serializable]
	public class CacheItemRecord : Colosoft.Query.Record, Colosoft.Caching.ICacheItemRecord
	{
		/// <summary>
		/// Tipos primitivos usados.
		/// </summary>
		private enum PrimitiveTypes : byte
		{
			Null = 1,
			Byte,
			Int16,
			Int32,
			Int64,
			Single,
			Double,
			Decimal,
			Boolean,
			Char,
			String,
			DateTime,
			DateTimeOffset,
			Bytes
		}

		private Colosoft.Reflection.TypeName _typeName;

		/// <summary>
		/// Nome do tipo representado pelo registro.
		/// </summary>
		public Colosoft.Reflection.TypeName TypeName
		{
			get
			{
				return _typeName;
			}
		}

		/// <summary>
		/// Cria uma instancia com dados já existentes.
		/// </summary>
		/// <param name="typeName">Nome do tipo.</param>
		/// <param name="values">Valores do registro.</param>
		/// <param name="descriptor"></param>
		public CacheItemRecord(Colosoft.Reflection.TypeName typeName, object[] values, Colosoft.Query.Record.RecordDescriptor descriptor) : base(descriptor, values)
		{
			_typeName = typeName;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected CacheItemRecord()
		{
		}

		/// <summary>
		/// Recupera o tipo primitivo do valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private PrimitiveTypes GetPrimitiveType(object value)
		{
			if(value == null)
				return PrimitiveTypes.Null;
			if(value is int)
				return PrimitiveTypes.Int32;
			else if(value is short)
				return PrimitiveTypes.Int16;
			else if(value is long)
				return PrimitiveTypes.Int64;
			else if(value is float)
				return PrimitiveTypes.Single;
			else if(value is double)
				return PrimitiveTypes.Double;
			else if(value is char)
				return PrimitiveTypes.Char;
			else if(value is bool)
				return PrimitiveTypes.Boolean;
			else if(value is string)
				return PrimitiveTypes.String;
			else if(value is DateTime)
				return PrimitiveTypes.DateTime;
			else if(value is DateTimeOffset)
				return PrimitiveTypes.DateTimeOffset;
			else if(value is byte)
				return PrimitiveTypes.Byte;
			else if(value is byte[])
				return PrimitiveTypes.Bytes;
			return PrimitiveTypes.Null;
		}

		/// <summary>
		/// Serializa o valor.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		private void Serialize(Serialization.IO.CompactWriter writer, object value)
		{
			if(value is int)
				writer.Write((int)value);
			else if(value is short)
				writer.Write((short)value);
			else if(value is long)
				writer.Write((long)value);
			else if(value is float)
				writer.Write((float)value);
			else if(value is string)
				writer.Write((string)value);
			else if(value is bool)
				writer.Write((bool)value);
			else if(value is char)
				writer.Write((char)value);
			else if(value is byte)
				writer.Write((byte)value);
			else if(value is byte[])
			{
				var buffer = (byte[])value;
				writer.Write(buffer.Length);
				writer.Write(buffer);
			}
			else if(value is double)
				writer.Write((double)value);
			else if(value is decimal)
				writer.Write((decimal)value);
			else if(value is DateTime)
				writer.Write((DateTime)value);
			else if(value is DateTimeOffset)
			{
				writer.Write(((DateTimeOffset)value).Ticks);
				writer.Write(((DateTimeOffset)value).Offset.Ticks);
			}
		}

		/// <summary>
		/// Deserializa o valor.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private object Deserialize(Serialization.IO.CompactReader reader, PrimitiveTypes type)
		{
			switch(type)
			{
			case PrimitiveTypes.Null:
				return null;
			case PrimitiveTypes.Boolean:
				return reader.ReadBoolean();
			case PrimitiveTypes.Byte:
				return reader.ReadByte();
			case PrimitiveTypes.Bytes:
				var length = reader.ReadInt32();
				return reader.ReadBytes(length);
			case PrimitiveTypes.Char:
				return reader.ReadChar();
			case PrimitiveTypes.DateTime:
				return reader.ReadDateTime();
			case PrimitiveTypes.DateTimeOffset:
				return new DateTimeOffset(reader.ReadInt64(), new TimeSpan(reader.ReadInt64()));
			case PrimitiveTypes.Decimal:
				return reader.ReadDecimal();
			case PrimitiveTypes.Double:
				return reader.ReadDouble();
			case PrimitiveTypes.Int16:
				return reader.ReadInt16();
			case PrimitiveTypes.Int32:
				return reader.ReadInt32();
			case PrimitiveTypes.Int64:
				return reader.ReadInt64();
			case PrimitiveTypes.Single:
				return reader.ReadSingle();
			case PrimitiveTypes.String:
				return reader.ReadString();
			}
			throw new InvalidOperationException("Invalid PrimitiveType");
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(Serialization.IO.CompactReader reader)
		{
			_typeName = new Reflection.TypeName();
			_typeName.Deserialize(reader);
			var length = reader.ReadInt16();
			var values = new object[length];
			for(var i = 0; i < length; i++)
			{
				var type = (PrimitiveTypes)reader.ReadByte();
				values[i] = Deserialize(reader, type);
			}
			SetValues(values);
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(Serialization.IO.CompactWriter writer)
		{
			_typeName.Serialize(writer);
			var values = GetValues();
			writer.Write((short)values.Length);
			foreach (var i in values)
			{
				var type = GetPrimitiveType(i);
				writer.Write((byte)type);
				if(type != PrimitiveTypes.Null)
					Serialize(writer, i);
			}
		}
	}
}
