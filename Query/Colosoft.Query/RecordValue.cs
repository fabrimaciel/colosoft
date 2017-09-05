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

namespace Colosoft.Query
{
	/// <summary>
	/// Representa o valor de um campo do resultado.
	/// </summary>
	public class RecordValue
	{
		/// <summary>
		/// Valor do campo.
		/// </summary>
		private object _value;

		/// <summary>
		/// Identifica se o valor da propriedade existe no resultado.
		/// </summary>
		private bool _valueExists;

		/// <summary>
		/// Identifica se o valor da propriedade existe no resultado.
		/// </summary>
		public bool ValueExists
		{
			get
			{
				return _valueExists;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="value">Valor do campo.</param>
		/// <param name="valueExists"></param>
		public RecordValue(object value, bool valueExists)
		{
			_value = value == DBNull.Value ? null : value;
			_valueExists = valueExists;
		}

		/// <summary>
		/// Converte o valor para um Int32.
		/// </summary>
		/// <returns></returns>
		public int ToInt32()
		{
			if(_value is byte[])
				return BitConverter.ToInt32((byte[])_value, 0);
			return Convert.ToInt32(_value);
		}

		/// <summary>
		/// Converte o valor para um Int32Nullable.
		/// </summary>
		/// <returns></returns>
		public int? ToInt32Nullable()
		{
			if(_value == null)
				return null;
			else
				return ToInt32();
		}

		/// <summary>
		/// Converte o valor para um Int32.
		/// </summary>
		/// <returns></returns>
		public uint ToUInt32()
		{
			if(_value is byte[])
				return BitConverter.ToUInt32((byte[])_value, 0);
			return Convert.ToUInt32(_value);
		}

		/// <summary>
		/// Converte o valor para um UInt32Nullable.
		/// </summary>
		/// <returns></returns>
		public uint? ToUInt32Nullable()
		{
			if(_value == null)
				return null;
			else
				return ToUInt32();
		}

		/// <summary>
		/// Converte o valor para um Int16.
		/// </summary>
		/// <returns></returns>
		public short ToInt16()
		{
			if(_value is byte[])
				return BitConverter.ToInt16((byte[])_value, 0);
			return Convert.ToInt16(_value);
		}

		/// <summary>
		/// Converte o valor para um Int16Nullable.
		/// </summary>
		/// <returns></returns>
		public short? ToInt16Nullable()
		{
			if(_value == null)
				return null;
			else
				return ToInt16();
		}

		/// <summary>
		/// Converte o valor para um UInt16.
		/// </summary>
		/// <returns></returns>
		public ushort ToUInt16()
		{
			if(_value is byte[])
				return BitConverter.ToUInt16((byte[])_value, 0);
			return Convert.ToUInt16(_value);
		}

		/// <summary>
		/// Converte o valor para um UInt16Nullable.
		/// </summary>
		/// <returns></returns>
		public ushort? ToUInt16Nullable()
		{
			if(_value == null)
				return null;
			else
				return ToUInt16();
		}

		/// <summary>
		/// Converte o valor para um Int64.
		/// </summary>
		/// <returns></returns>
		public long ToInt64()
		{
			if(_value is byte[])
				return BitConverter.ToInt64((byte[])_value, 0);
			return Convert.ToInt64(_value);
		}

		/// <summary>
		/// Converte o valor para um Int64Nullable.
		/// </summary>
		/// <returns></returns>
		public long? ToInt64Nullable()
		{
			if(_value == null)
				return null;
			else
				return ToInt64();
		}

		/// <summary>
		/// Converte o valor para um UInt64.
		/// </summary>
		/// <returns></returns>
		public ulong ToUInt64()
		{
			if(_value is byte[])
				return BitConverter.ToUInt64((byte[])_value, 0);
			return Convert.ToUInt64(_value);
		}

		/// <summary>
		/// Converte o valor para um UInt64Nullable.
		/// </summary>
		/// <returns></returns>
		public ulong? ToUInt64Nullable()
		{
			if(_value == null)
				return null;
			else
				return ToUInt64();
		}

		/// <summary>
		/// Converte o valor para um Double.
		/// </summary>
		/// <returns></returns>
		public double ToDouble()
		{
			if(_value is byte[])
				return BitConverter.ToDouble((byte[])_value, 0);
			return Convert.ToDouble(_value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para um DoubleNullable.
		/// </summary>
		/// <returns></returns>
		public double? ToDoubleNullable()
		{
			if(_value == null)
				return null;
			else
				return ToDouble();
		}

		/// <summary>
		/// Converte o valor para um Single.
		/// </summary>
		/// <returns></returns>
		public float ToSingle()
		{
			if(_value is byte[])
				return BitConverter.ToSingle((byte[])_value, 0);
			return Convert.ToSingle(_value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para um SingleNullable.
		/// </summary>
		/// <returns></returns>
		public float? ToSingleNullable()
		{
			if(_value == null)
				return null;
			else
				return ToSingle();
		}

		/// <summary>
		/// Converte o valor para um Decimal.
		/// </summary>
		/// <returns></returns>
		public decimal ToDecimal()
		{
			return Convert.ToDecimal(_value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para um DecimalNullable.
		/// </summary>
		/// <returns></returns>
		public decimal? ToDecimalNullable()
		{
			if(_value == null)
				return null;
			else
				return Convert.ToDecimal(_value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para um DateTime.
		/// </summary>
		/// <returns></returns>
		public DateTime ToDateTime()
		{
			if(_value is DateTimeOffset)
				return ((DateTimeOffset)_value).LocalDateTime;
			return Convert.ToDateTime(_value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para um DateTimeNullable.
		/// </summary>
		/// <returns></returns>
		public DateTime? ToDateTimeNullable()
		{
			if(_value == null)
				return null;
			else if(_value is DateTimeOffset)
				return ((DateTimeOffset)_value).LocalDateTime;
			else
				return Convert.ToDateTime(_value);
		}

		/// <summary>
		/// Converte o valor para um DateTimeOffset.
		/// </summary>
		/// <returns></returns>
		public DateTimeOffset ToDateTimeOffset()
		{
			if(!ValueExists || !(_value is DateTimeOffset))
				return DateTimeOffset.MinValue;
			return (DateTimeOffset)_value;
		}

		/// <summary>
		/// Converte o valor para um DateTimeOffsetNullable.
		/// </summary>
		/// <returns></returns>
		public DateTimeOffset? ToDateTimeOffsetNullable()
		{
			if(!ValueExists || !(_value is DateTimeOffset))
				return null;
			return (DateTimeOffset)_value;
		}

		/// <summary>
		/// Converte o valor para um Boolean.
		/// </summary>
		/// <returns></returns>
		public bool ToBoolean()
		{
			if(_value is byte[])
				return BitConverter.ToBoolean((byte[])_value, 0);
			return Convert.ToBoolean(_value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para um BooleanNullable.
		/// </summary>
		/// <returns></returns>
		public bool? ToBooleanNullable()
		{
			if(_value == null)
				return null;
			else
				return ToBoolean();
		}

		/// <summary>
		/// Converte o valor para um Boolean.
		/// </summary>
		/// <returns></returns>
		public byte[] ToBytes()
		{
			return _value as byte[];
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			return _value;
		}

		/// <summary>
		/// Converte o valor para uma string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_value is byte[])
				return BitConverter.ToString((byte[])_value, 0);
			return Convert.ToString(_value);
		}

		/// <summary>
		/// Converte implicitamente para um Int32.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator int(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToInt32();
		}

		/// <summary>
		/// Converte implicitamente para um Int32Nullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator int?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToInt32Nullable();
		}

		/// <summary>
		/// Converte implicitamente para um UInt32.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator uint(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToUInt32();
		}

		/// <summary>
		/// Converte implicitamente para um UInt32Nullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator uint?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToUInt32Nullable();
		}

		/// <summary>
		/// Converte implicitamente para um Int16.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator short(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToInt16();
		}

		/// <summary>
		/// Converte implicitamente para um Int16Nullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator short?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToInt16Nullable();
		}

		/// <summary>
		/// Converte implicitamente para um UInt16.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator ushort(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToUInt16();
		}

		/// <summary>
		/// Converte implicitamente para um UInt16Nullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator ushort?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToUInt16Nullable();
		}

		/// <summary>
		/// Converte implicitamente para um Int64.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator long(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToInt64();
		}

		/// <summary>
		/// Converte implicitamente para um Int64Nullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator long?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToInt64Nullable();
		}

		/// <summary>
		/// Converte implicitamente para um UInt64.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator ulong(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToUInt64();
		}

		/// <summary>
		/// Converte implicitamente para um UInt64Nullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator ulong?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToUInt64Nullable();
		}

		/// <summary>
		/// Converte implicitamente para um Single.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator float(RecordValue value)
		{
			if(value == null)
				return 0.0f;
			return value.ToSingle();
		}

		/// <summary>
		/// Converte implicitamente para um SingleNullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator float?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToSingleNullable();
		}

		/// <summary>
		/// Converte implicitamente para um Double.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator double(RecordValue value)
		{
			if(value == null)
				return 0.0;
			return value.ToDouble();
		}

		/// <summary>
		/// Converte implicitamente para um DoubleNullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator double?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToDoubleNullable();
		}

		/// <summary>
		/// Converte implicitamente para um Decimal.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator decimal(RecordValue value)
		{
			if(value == null)
				return 0;
			return value.ToDecimal();
		}

		/// <summary>
		/// Converte implicitamente para um DecimalNullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator decimal?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToDecimalNullable();
		}

		/// <summary>
		/// Converte implicitamente para um DateTime.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator DateTime(RecordValue value)
		{
			if(value == null)
				return DateTime.MinValue;
			return value.ToDateTime();
		}

		/// <summary>
		/// Converte implicitamente para um DateTimeNullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator DateTime?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToDateTimeNullable();
		}

		/// <summary>
		/// Converte implicitamente para um Boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator bool(RecordValue value)
		{
			if(value == null)
				return false;
			return value.ToBoolean();
		}

		/// <summary>
		/// Converte implicitamente para um BooleanNullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator bool?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToBooleanNullable();
		}

		/// <summary>
		/// Converte implicitamente para um byte[].
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator byte[](RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToBytes();
		}

		/// <summary>
		/// Converte implicitamente para uma string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator string(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToString();
		}

		/// <summary>
		/// Converte implicitamente para um DateTimeOffset.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator DateTimeOffset(RecordValue value)
		{
			if(value == null)
				return DateTimeOffset.MinValue;
			return value.ToDateTimeOffset();
		}

		/// <summary>
		/// Converte implicitamente para um DateTimeOffsetNullable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator DateTimeOffset?(RecordValue value)
		{
			if(value == null)
				return null;
			return value.ToDateTimeOffsetNullable();
		}
	}
}
