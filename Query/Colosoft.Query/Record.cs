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
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa um registro remoto.
	/// </summary>
	[System.Xml.Serialization.XmlSchemaProvider("GetRecordSchema")]
	[System.Xml.Serialization.XmlRoot(Namespace = Namespaces.Query)]
	[Serializable]
	public partial class Record : IRecord, IDataRecord, IEditableRecord, IXmlSerializable, Colosoft.Serialization.ICompactSerializable
	{
		/// <summary>
		/// Comparador do nome dos campos.
		/// </summary>
		public readonly static StringComparer FieldNameComparer = StringComparer.InvariantCultureIgnoreCase;

		/// <summary>
		/// Descritor do registro.
		/// </summary>
		private RecordDescriptor _descriptor;

		/// <summary>
		/// Valores dos itens do registro.
		/// </summary>
		private object[] _values;

		[NonSerialized]
		private IQueryResult _queryResult;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="descriptor">Descritor do record.</param>
		public Record(RecordDescriptor descriptor)
		{
			descriptor.Require("descriptor").NotNull();
			_descriptor = descriptor;
		}

		/// <summary>
		/// Cria a instancia com o descritor e seus valores associados.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="values"></param>
		protected Record(RecordDescriptor descriptor, object[] values)
		{
			descriptor.Require("descriptor").NotNull();
			_descriptor = descriptor;
			_values = values;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected Record()
		{
		}

		/// <summary>
		/// Instancia do resultado onde o registro está sendo recuperado.
		/// </summary>
		public IQueryResult QueryResult
		{
			get
			{
				return _queryResult;
			}
			set
			{
				_queryResult = value;
			}
		}

		/// <summary>
		/// Descritor do registro.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public RecordDescriptor Descriptor
		{
			get
			{
				return _descriptor;
			}
		}

		/// <summary>
		/// Define os valores do registro.
		/// </summary>
		/// <param name="values"></param>
		public void SetValues(object[] values)
		{
			_values = values;
		}

		/// <summary>
		/// Recupera os valores diretos do registor.
		/// </summary>
		/// <returns></returns>
		protected object[] GetValues()
		{
			return _values;
		}

		/// <summary>
		/// Recupera a posição do campo com base no nome.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private int GetFieldPosition(string name)
		{
			for(int i = 0; i < _descriptor.Count; i++)
				if(FieldNameComparer.Equals(_descriptor[i].Name, name))
					return i;
			throw new RecordFieldNotFoundException(name, _descriptor, _queryResult);
		}

		/// <summary>
		/// Quantidade de campos no resultado.
		/// </summary>
		public int FieldCount
		{
			get
			{
				return _descriptor.Count;
			}
		}

		/// <summary>
		/// Recupera o valor <see cref="Boolean"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public bool GetBoolean(int i)
		{
			return Convert.ToBoolean(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="Boolean"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public bool GetBoolean(string name)
		{
			return Convert.ToBoolean(GetValue(name));
		}

		/// <summary>
		/// Recupera o valor <see cref="Byte"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public byte GetByte(int i)
		{
			return Convert.ToByte(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="Byte"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public byte GetByte(string name)
		{
			return Convert.ToByte(GetValue(name));
		}

		/// <summary>
		/// Recupera os bytes do campo na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera os bytes do campo com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public long GetBytes(string name, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o valor <see cref="char"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public char GetChar(int i)
		{
			return Convert.ToChar(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="char"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public char GetChar(string name)
		{
			return Convert.ToChar(GetValue(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldoffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fieldoffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public long GetChars(string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o nome do tipo de dados do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public string GetDataTypeName(int i)
		{
			return GetFieldType(i).FullName;
		}

		/// <summary>
		/// Recupera o nome do tipo de dados do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public string GetDataTypeName(string name)
		{
			return GetFieldType(name).FullName;
		}

		/// <summary>
		/// Recupera o valor <see cref="DateTime"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public DateTime GetDateTime(int i)
		{
			var value = GetValue(i);
			if(value is DateTimeOffset)
				return ((DateTimeOffset)value).DateTime;
			return Convert.ToDateTime(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o valor <see cref="DateTime"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public DateTime GetDateTime(string name)
		{
			var value = GetValue(name);
			if(value is DateTimeOffset)
				return ((DateTimeOffset)value).DateTime;
			return Convert.ToDateTime(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o valor <see cref="decimal"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public decimal GetDecimal(int i)
		{
			return Convert.ToDecimal(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o valor <see cref="decimal"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public decimal GetDecimal(string name)
		{
			return Convert.ToDecimal(GetValue(name));
		}

		/// <summary>
		/// Recupera o valor <see cref="double"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public double GetDouble(int i)
		{
			return Convert.ToDouble(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o valor <see cref="double"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public double GetDouble(string name)
		{
			return Convert.ToDouble(GetValue(name), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o tipo do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public Type GetFieldType(int i)
		{
			if(i < 0 || i > _values.Length)
				throw new IndexOutOfRangeException();
			return _descriptor[i].Type;
		}

		/// <summary>
		/// Recupera o tipo do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public Type GetFieldType(string name)
		{
			var i = GetFieldPosition(name);
			if(i < 0 || i > _values.Length)
				throw new IndexOutOfRangeException();
			return _descriptor[i].Type;
		}

		/// <summary>
		/// Recupera o valor <see cref="float"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public float GetFloat(int i)
		{
			return Convert.ToSingle(GetValue(i), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o valor <see cref="float"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public float GetFloat(string name)
		{
			return Convert.ToSingle(GetValue(name), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Recupera o valor <see cref="Guid"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public Guid GetGuid(int i)
		{
			return Guid.Parse(GetString(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="Guid"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public Guid GetGuid(string name)
		{
			return Guid.Parse(GetString(name));
		}

		/// <summary>
		/// Recupera o valor <see cref="Int16"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public short GetInt16(int i)
		{
			return Convert.ToInt16(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="Int16"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public short GetInt16(string name)
		{
			return Convert.ToInt16(GetValue(name));
		}

		/// <summary>
		/// Recupera o valor <see cref="Int32"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public int GetInt32(int i)
		{
			return Convert.ToInt32(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="Int32"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public int GetInt32(string name)
		{
			return Convert.ToInt32(GetValue(name));
		}

		/// <summary>
		/// Recupera o valor <see cref="Int64"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo</param>
		/// <returns></returns>
		public long GetInt64(int i)
		{
			return Convert.ToInt64(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="Int64"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public long GetInt64(string name)
		{
			return Convert.ToInt64(GetValue(name));
		}

		/// <summary>
		/// Recupera o nome do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public string GetName(int i)
		{
			if(i < 0 || i > _values.Length)
				throw new IndexOutOfRangeException();
			return _descriptor[i].Name;
		}

		/// <summary>
		/// Recupera a posição ordinal do campos com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		public int GetOrdinal(string name)
		{
			return GetFieldPosition(name);
			;
		}

		/// <summary>
		/// Recupera o valor <see cref="String"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public string GetString(int i)
		{
			return Convert.ToString(GetValue(i));
		}

		/// <summary>
		/// Recupera o valor <see cref="String"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetString(string name)
		{
			return Convert.ToString(GetValue(name));
		}

		/// <summary>
		/// Recupera o valor <see cref="DateTimeOffset"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		public DateTimeOffset GetDateTimeOffset(int i)
		{
			return this[i].ToDateTimeOffset();
		}

		/// <summary>
		/// Recupera o valor <see cref="DateTimeOffset"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DateTimeOffset GetDateTimeOffset(string name)
		{
			return this[name].ToDateTimeOffset();
		}

		/// <summary>
		/// Recupera o valor do campo na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public object GetValue(int i)
		{
			if(i < 0 || i > _values.Length)
				throw new IndexOutOfRangeException();
			return _values[i];
		}

		/// <summary>
		/// Recupera o valor da campo com o nome innformado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetValue(string name)
		{
			var index = GetFieldPosition(name);
			if(index < 0 || index > _values.Length)
				throw new IndexOutOfRangeException();
			return _values[index];
		}

		/// <summary>
		/// Recupera os valores da registro para o vetor informado.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public int GetValues(object[] values)
		{
			values.Require("values").NotNull();
			int num2 = (values.Length < this.FieldCount) ? values.Length : this.FieldCount;
			for(int i = 0; i < num2; i++)
				values[i] = this.GetValue(i);
			return num2;
		}

		/// <summary>
		/// Verifica se o campo na posição informada possui um valor nulo.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public bool IsDBNull(int i)
		{
			return GetValue(i) == null;
		}

		/// <summary>
		/// Verifica se o campo com o onme informado possui um valor nulo.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsDBNull(string name)
		{
			return GetValue(name) == null;
		}

		/// <summary>
		/// Recupera um valor pelo nome da coluna.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065")]
		public RecordValue this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				try
				{
					var obj = GetValue(name);
					return new RecordValue((obj is DBNull ? null : obj), !(obj is DBNull));
				}
				catch(IndexOutOfRangeException ex)
				{
					throw new QueryException(string.Format("No column with the name \"{0}\" was found.", name), ex);
				}
			}
		}

		/// <summary>
		/// Recupera o valor da coluna na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public RecordValue this[int i]
		{
			get
			{
				var obj = GetValue(i);
				return new RecordValue((obj is DBNull ? null : obj), !(obj is DBNull));
			}
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetRecordSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new XmlQualifiedName("Record", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Lê os dados serializados do XML.
		/// </summary>
		/// <param name="reader"></param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			_values = new object[_descriptor.Count];
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if(!string.IsNullOrWhiteSpace(reader.LocalName))
				{
					var position = _descriptor.GetFieldPosition(reader.LocalName);
					if(position >= 0 && !reader.IsEmptyElement)
					{
						var field = _descriptor[position];
						if(field.Type == typeof(byte[]))
						{
							var content = (string)reader.ReadElementContentAs(typeof(string), null);
							_values[position] = Convert.FromBase64String(content);
						}
						else
							_values[position] = reader.ReadElementContentAs(field.Type, null);
					}
					else
						reader.Skip();
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Serializa os dados em XML.
		/// </summary>
		/// <param name="writer"></param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			for(var i = 0; i < _descriptor.Count; i++)
			{
				var field = _descriptor[i];
				var value = _values[i];
				writer.WriteStartElement(field.EncodedName);
				if(value != null)
				{
					if(value is byte[])
						writer.WriteValue(Convert.ToBase64String((byte[])value));
					else
						writer.WriteValue(value);
				}
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public virtual void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_values = new object[_descriptor.Count];
			var buffer = new byte[1];
			for(var i = 0; i < _descriptor.Count; i++)
			{
				var field = _descriptor[i];
				var isNull = reader.Read(buffer, 0, 1) == 1 && buffer[0] == (byte)1;
				if(!isNull)
					try
					{
						_values[i] = reader.ReadObject();
					}
					catch(Exception ex)
					{
						throw new System.Runtime.Serialization.SerializationException(ResourceMessageFormatter.Create(() => Properties.Resources.Record_DeserializeFieldError, field.Name, field.Type.FullName, ex.Message).Format(), ex);
					}
			}
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			for(var i = 0; i < _descriptor.Count; i++)
			{
				var field = _descriptor[i];
				var value = _values[i];
				writer.Write((byte)(value == null ? 1 : 0));
				if(value != null)
					try
					{
						var type = value.GetType();
						writer.WriteObject(value);
					}
					catch(Exception ex)
					{
						throw new System.Runtime.Serialization.SerializationException(ResourceMessageFormatter.Create(() => Properties.Resources.Record_SerializeFieldError, field.Name, field.Type.FullName, ex.Message).Format(), ex);
					}
			}
		}

		/// <summary>
		/// Recupera o valor da coluna na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		object IDataRecord.this[int i]
		{
			get
			{
				return this[i].GetValue();
			}
		}

		/// <summary>
		/// Recupera um valor pelo nome da coluna.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object IDataRecord.this[string name]
		{
			get
			{
				return this[name].GetValue();
			}
		}

		/// <summary>
		/// Atribui o valor para o campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <param name="value">Valor que será atribuído.</param>
		void IEditableRecord.SetValue(string name, object value)
		{
			var index = GetFieldPosition(name);
			if(index < 0 || index > _values.Length)
				throw new IndexOutOfRangeException();
			_values[index] = value;
		}

		/// <summary>
		/// Atribui o valor para o campo com a posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <param name="value">Valor que será atribuído.</param>
		void IEditableRecord.SetValue(int i, object value)
		{
			if(i < 0 || i > _values.Length)
				throw new IndexOutOfRangeException();
			_values[i] = value;
		}
	}
}
