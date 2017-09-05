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
	/// Represenat o esquema de vinculação de um descritor de registros.
	/// </summary>
	class TypeBindRecordDescriptorSchema : IEnumerable<TypeBindRecordDescriptorSchema.Property>, IEquatable<Record.RecordDescriptor>, IDisposable
	{
		private List<Property> _properties;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeBindStrategy">Instancia da estratégia de vinculação associada.</param>
		/// <param name="descriptor"></param>
		public TypeBindRecordDescriptorSchema(TypeBindStrategy typeBindStrategy, Record.RecordDescriptor descriptor)
		{
			_properties = new List<Property>();
			var typeProperties = typeBindStrategy.Type.GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
			var stringComparer = StringComparer.InvariantCultureIgnoreCase;
			foreach (var field in descriptor)
			{
				var prop = typeProperties.Where(f => f.CanWrite && stringComparer.Equals(f.Name, field.Name)).FirstOrDefault();
				if(prop != null)
				{
					if(_properties.Exists(f => stringComparer.Equals(f.FieldName, field.Name)))
						throw new TypeBindStrategyException(ResourceMessageFormatter.Create(() => Properties.Resources.TypeBindStrategy_DuplicateFieldName, field.Name, typeBindStrategy.Type.FullName, string.Join(", ", descriptor.Select(f => f.Name).ToArray())).Format());
					var converter = System.ComponentModel.TypeDescriptor.GetConverter(prop.PropertyType);
					_properties.Add(new Property(field.Name, prop, converter));
				}
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~TypeBindRecordDescriptorSchema()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o enumerador das propriedades.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TypeBindRecordDescriptorSchema.Property> GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das propriedades.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_properties.Clear();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Compara a instancia com um descritor.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Record.RecordDescriptor other)
		{
			if(other.Count == _properties.Count)
			{
				var comparer = StringComparer.InvariantCultureIgnoreCase;
				foreach (var field in other)
					if(!_properties.Exists(f => comparer.Equals(f.FieldName, field.Name)))
						return false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Representa uma propriedade do esquema.
		/// </summary>
		public class Property
		{
			private string _fieldName;

			private System.Reflection.PropertyInfo _info;

			private System.ComponentModel.TypeConverter _converter;

			/// <summary>
			/// Nome do campo do registro associado com a propriedade.
			/// </summary>
			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			/// <summary>
			/// Nome da propriedade.
			/// </summary>
			public string Name
			{
				get
				{
					return _info.Name;
				}
			}

			/// <summary>
			/// Tipo da propriedade.
			/// </summary>
			public Type PropertyType
			{
				get
				{
					return _info.PropertyType;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="fieldName">Nome do campo associado.</param>
			/// <param name="info"></param>
			/// <param name="converter"></param>
			public Property(string fieldName, System.Reflection.PropertyInfo info, System.ComponentModel.TypeConverter converter)
			{
				_fieldName = fieldName;
				_info = info;
				_converter = converter;
			}

			/// <summary>
			/// Converte o valor para o tipo informado.
			/// </summary>
			/// <param name="value"></param>
			/// <param name="toType"></param>
			/// <returns></returns>
			private static object ConvertValue(object value, Type toType)
			{
				if(toType.IsNullable() && value != null)
				{
					var value2 = ConvertValue(value, Nullable.GetUnderlyingType(toType));
					try
					{
						return Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(Nullable.GetUnderlyingType(toType)), value2);
					}
					catch(System.Reflection.TargetInvocationException ex)
					{
						throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.TypeBindStrategy_ConvertValueError2, value.GetType().FullName, toType.FullName).Format(), ex.InnerException);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.TypeBindStrategy_ConvertValueError2, value.GetType().FullName, toType.FullName).Format(), ex);
					}
				}
				else if(toType.IsEnum)
				{
					var sourceType = value.GetType();
					var underlyngType = Enum.GetUnderlyingType(toType);
					var enumValue = value;
					if(value is string)
					{
						enumValue = Enum.Parse(toType, (string)value, true);
					}
					else if(toType == typeof(long))
						enumValue = (int)(long)value;
					else if(sourceType == typeof(uint))
					{
						if(underlyngType == typeof(int))
							enumValue = (int)(uint)value;
						else if(underlyngType == typeof(short))
							enumValue = (short)(uint)value;
					}
					else if(sourceType == typeof(ushort))
					{
						if(underlyngType == typeof(int))
							enumValue = (int)(ushort)value;
						else if(underlyngType == typeof(short))
							enumValue = (short)(ushort)value;
					}
					return Enum.ToObject(toType, enumValue);
				}
				else if(value is long && toType == typeof(int))
					return (int)(long)value;
				else if(value is decimal)
				{
					if(toType == typeof(long))
						return (long)(decimal)value;
					if(toType == typeof(double))
						return (double)(decimal)value;
					if(toType == typeof(int))
						return (int)(decimal)value;
					if(toType == typeof(short))
						return (short)(decimal)value;
					if(toType == typeof(bool))
						return (bool)(((decimal)value) != 0);
					if(toType == typeof(float))
						return (float)(decimal)value;
				}
				else if(toType == typeof(bool))
				{
					if(value is int)
						return ((int)value) != 0;
					else if(value is short)
						return ((short)value) != 0;
					else if(value is long)
						return ((long)value) != 0;
					else if(value is byte)
						return ((byte)value) != 0;
					else if(value is sbyte)
						return ((sbyte)value) != 0;
					else if(value is ulong)
						return ((ulong)value) != 0;
					else if(value is uint)
						return ((ulong)value) != 0;
					else if(value is ushort)
						return ((ulong)value) != 0;
				}
				else if(value is uint)
				{
					if(toType == typeof(int))
						return (int)(uint)value;
					else if(toType == typeof(short))
						return (short)(uint)value;
					else if(toType == typeof(long))
						return (long)(uint)value;
				}
				else if(value is ushort)
				{
					if(toType == typeof(int))
						return (int)(ushort)value;
					else if(toType == typeof(short))
						return (short)(ushort)value;
					else if(toType == typeof(long))
						return (long)(ushort)value;
				}
				else if(value is ulong)
				{
					if(toType == typeof(int))
						return (int)(ulong)value;
					else if(toType == typeof(short))
						return (short)(ulong)value;
					else if(toType == typeof(long))
						return (long)(ulong)value;
					else if(toType == typeof(bool))
						return Convert.ToBoolean((ulong)value);
				}
				else if(value is long)
				{
					if(toType == typeof(ulong))
						return (ulong)(long)value;
					else if(toType == typeof(ushort))
						return (ushort)(long)value;
					else if(toType == typeof(uint))
						return (uint)(long)value;
					else if(toType == typeof(int))
						return (int)(long)value;
					else if(toType == typeof(short))
						return (short)(long)value;
				}
				else if(value is sbyte)
				{
					if(toType == typeof(bool))
						return Convert.ToBoolean((sbyte)value);
					else if(toType == typeof(int))
						return (int)(sbyte)value;
					else if(toType == typeof(short))
						return (short)(sbyte)value;
					else if(toType == typeof(long))
						return (long)(sbyte)value;
					else if(toType == typeof(uint))
						return (uint)(sbyte)value;
					else if(toType == typeof(ushort))
						return (ushort)(sbyte)value;
					else if(toType == typeof(ulong))
						return (ulong)(sbyte)value;
				}
				else if(value is int)
				{
					if(toType == typeof(uint))
						return (uint)(int)value;
					else if(toType == typeof(ushort))
						return (ushort)(int)value;
					else if(toType == typeof(ulong))
						return (ulong)(int)value;
				}
				else if(value is double)
				{
					if(toType == typeof(decimal))
						return (decimal)(double)value;
					else if(toType == typeof(long))
						return (long)(double)value;
					else if(toType == typeof(int))
						return (int)(double)value;
					else if(toType == typeof(float))
						return (float)(double)value;
				}
				else if(value is float)
				{
					if(toType == typeof(decimal))
						return (decimal)(float)value;
				}
				else if(value is decimal)
				{
					if(toType == typeof(float))
						return (float)(decimal)value;
					if(toType == typeof(double))
						return (double)(decimal)value;
					if(toType == typeof(int))
						return (int)(decimal)value;
					if(toType == typeof(long))
						return (long)(decimal)value;
					if(toType == typeof(short))
						return (short)(decimal)value;
				}
				else if(value is DateTimeOffset && toType == typeof(DateTime))
					value = ((DateTimeOffset)value).DateTime;
				else if(value is DateTime && toType == typeof(DateTimeOffset))
					value = new DateTimeOffset((DateTime)value);
				return value;
			}

			/// <summary>
			/// Converte o valor da propriedade.
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			private object ConvertValue(object value)
			{
				Type valueType = value == null ? null : value.GetType();
				Type nullableUnderlynigType = null;
				if(value != null && valueType != PropertyType)
				{
					if((PropertyType == typeof(int) && valueType == typeof(long)) || (PropertyType == typeof(uint) && valueType == typeof(int)) || (PropertyType == typeof(ushort) && valueType == typeof(int)) || (PropertyType == typeof(long) && valueType == typeof(double)) || (PropertyType == typeof(int) && valueType == typeof(double)) || (PropertyType == typeof(bool) && (valueType == typeof(int) || valueType == typeof(short) || valueType == typeof(long) || valueType == typeof(decimal) || valueType == typeof(byte) || valueType == typeof(sbyte))))
					{
						value = ConvertValue(value, PropertyType);
					}
					else if(PropertyType.IsNullable() && (nullableUnderlynigType = Nullable.GetUnderlyingType(PropertyType)) != valueType)
					{
						value = ConvertValue(value, PropertyType);
					}
					else if(PropertyType.IsEnum)
					{
						if(value is decimal)
							value = (int)(decimal)value;
						var underlyingType = Enum.GetUnderlyingType(PropertyType);
						if(underlyingType == typeof(byte))
						{
							if(value is char)
								value = (byte)(char)value;
							else if(value is string)
								value = (byte)((string)value).FirstOrDefault();
						}
						value = Enum.ToObject(PropertyType, value);
					}
					else if((valueType == typeof(decimal) && (PropertyType == typeof(int) || PropertyType == typeof(short) || PropertyType == typeof(long))) || (valueType == typeof(double) && (PropertyType == typeof(decimal))) || (valueType == typeof(float) && (PropertyType == typeof(decimal))))
					{
						value = ConvertValue(value, PropertyType);
					}
					else if(valueType == typeof(uint) && PropertyType == typeof(int))
					{
						value = (int)(uint)value;
					}
					else if(valueType == typeof(ushort) && PropertyType == typeof(short))
					{
						value = (short)(ushort)value;
					}
					else if(_converter.CanConvertFrom(valueType))
					{
						value = _converter.ConvertFrom(value);
					}
					else if(value.GetType().IsArray && typeof(byte).IsAssignableFrom(valueType.GetElementType()))
					{
						var byteArray = (byte[])value;
						switch(PropertyType.FullName)
						{
						case "System.Int16":
							value = BitConverter.ToInt16(byteArray, 0);
							break;
						case "System.Int32":
							value = BitConverter.ToInt32(byteArray, 0);
							break;
						case "System.Int64":
							value = BitConverter.ToInt64(byteArray, 0);
							break;
						case "System.UInt16":
							value = BitConverter.ToUInt16(byteArray, 0);
							break;
						case "System.UInt32":
							value = BitConverter.ToUInt32(byteArray, 0);
							break;
						case "System.UInt64":
							value = BitConverter.ToUInt64(byteArray, 0);
							break;
						case "System.Single":
							value = BitConverter.ToSingle(byteArray, 0);
							break;
						case "System.Double":
							value = BitConverter.ToDouble(byteArray, 0);
							break;
						case "System.String":
							value = BitConverter.ToString(byteArray, 0);
							break;
						case "System.Char":
							value = BitConverter.ToChar(byteArray, 0);
							break;
						case "System.Boolean":
							value = BitConverter.ToBoolean(byteArray, 0);
							break;
						}
					}
					else if(PropertyType == typeof(double))
						value = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
					else if(PropertyType == typeof(float))
						value = Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture);
				}
				return value;
			}

			/// <summary>
			/// Recupera o valor do registro representado pela propriedade.
			/// </summary>
			/// <param name="record"></param>
			/// <returns></returns>
			public object GetRecordValue(IRecord record)
			{
				var value = record[FieldName].GetValue();
				return ConvertValue(value);
			}

			/// <summary>
			/// Define o valor para a propriedade.
			/// </summary>
			/// <param name="instance">Instancia na qual a propriedade será defina.</param>
			/// <param name="value">Valor que será definido.</param>
			public void SetValue(object instance, object value)
			{
				if(value != null)
				{
					var valueType = value.GetType();
					if(valueType != PropertyType)
						value = ConvertValue(value, PropertyType);
				}
				_info.SetValue(instance, value, null);
			}

			/// <summary>
			/// Recupera o valor da propriedade da instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <returns></returns>
			public object GetValue(object instance)
			{
				var value = _info.GetValue(instance, null);
				return ConvertValue(value);
			}
		}
	}
}
