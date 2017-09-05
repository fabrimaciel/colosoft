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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Armazena os campos do canal.
	/// </summary>
	public class ChannelFields : IEnumerable<SchemeField>
	{
		private SchemeField[] _fields;

		private byte _channelId;

		/// <summary>
		/// Identificador do canal associado.
		/// </summary>
		public byte ChannelId
		{
			get
			{
				return _channelId;
			}
		}

		/// <summary>
		/// Quantidade de campos do canal.
		/// </summary>
		public int Count
		{
			get
			{
				return _fields.Length;
			}
		}

		/// <summary>
		/// Recupera o campo no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SchemeField this[int index]
		{
			get
			{
				return _fields[index];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="fields"></param>
		public ChannelFields(byte channelId, SchemeField[] fields)
		{
			_channelId = channelId;
			_fields = fields ?? new SchemeField[0];
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<SchemeField> GetEnumerator()
		{
			foreach (var i in _fields)
				yield return i;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _fields.GetEnumerator();
		}
	}
	/// <summary>
	/// Objeto que armazena canal de elementos
	/// </summary>
	public class Channel
	{
		private ChannelFields _fields;

		private SchemeIndex[] _indexes;

		private Dictionary<string, int> _fieldsPosition = new Dictionary<string, int>();

		private Dictionary<string, int> _indexesPosition = new Dictionary<string, int>();

		/// <summary>
		/// Identificador do canal
		/// </summary>
		public byte ChannelId
		{
			get;
			set;
		}

		/// <summary>
		/// Lista de campos do canal
		/// </summary>
		public ChannelFields Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				_fieldsPosition.Clear();
				var names = value.Select(f => f.Name).ToArray();
				for(int i = 0; i < names.Length; i++)
				{
					if(string.IsNullOrEmpty(names[i]))
						throw new Exception("SchemeField name can not be null");
					try
					{
						_fieldsPosition.Add(names[i], i);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("There are duplicate field \"{0}\" on channel.", names[i]), ex);
					}
				}
				_fields = value;
			}
		}

		/// <summary>
		/// Lista de índices do canal
		/// </summary>
		public SchemeIndex[] Indexes
		{
			get
			{
				return _indexes;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				_indexesPosition.Clear();
				var names = value.Select(f => f.Name).ToArray();
				for(int i = 0; i < names.Length; i++)
				{
					if(string.IsNullOrEmpty(names[i]))
						throw new Exception("SchemeField name can not be null");
					try
					{
						_indexesPosition.Add(names[i], i);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("There are duplicate field \"{0}\" on channel.", names[i]), ex);
					}
				}
				_indexes = value;
			}
		}

		/// <summary>
		/// Cosntrutor padrão de canal
		/// </summary>
		/// <param name="channelId">Id do canal</param>
		/// <param name="fields">Campos do canal.</param>
		/// <param name="indexes">Indices do canal.</param>
		public Channel(byte channelId, SchemeField[] fields, SchemeIndex[] indexes)
		{
			ChannelId = channelId;
			Fields = new ChannelFields(channelId, fields);
			Indexes = indexes;
		}

		/// <summary>
		/// Busca o esquema de um índice no canal pelo seu nome
		/// </summary>
		/// <param name="indexName">Nome do índice</param>
		/// <returns>Esquema do índice</returns>
		public SchemeIndex GetSchemaIndex(string indexName)
		{
			return Indexes.Where(x => x.Name.ToUpper() == indexName.ToUpper()).FirstOrDefault();
		}

		/// <summary>
		/// Recuperea o valor do fulltext do indice.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="indexName"></param>
		/// <returns></returns>
		public string GetIndexFullTextValue(Element element, string indexName)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			int indexPosition = 0;
			try
			{
				indexPosition = _indexesPosition[indexName];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(string.Format("Index with name \"{0}\" not exists", indexName), ex);
			}
			var index = _indexes[indexPosition];
			var value = new StringBuilder();
			foreach (var field in index.FieldSchema)
			{
				var indexValue = GetFieldFullTextValue(element, field.Name);
				if(value.Length > 0)
					value.Append(" ");
				if(!string.IsNullOrEmpty(indexValue))
					value.Append(indexValue);
			}
			if(value.Length > 0)
				return value.ToString();
			return null;
		}

		/// <summary>
		/// Recupera o valor do indice contido no elemento.
		/// </summary>
		/// <param name="element">Elemento de onde o valor será recuperado.</param>
		/// <param name="indexName">Nome do indice que contém o valor.</param>
		/// <returns></returns>
		public string GetIndexValue(Element element, string indexName)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			var index = GetSchemeIndex(indexName);
			var value = new StringBuilder();
			if(index.FieldSchema.Length == 1)
				value.Append(GetFieldValue(element, index.FieldSchema[0]));
			else
			{
				foreach (var field in index.FieldSchema)
				{
					int fieldPosition = 0;
					try
					{
						fieldPosition = _fieldsPosition[field.Name];
					}
					catch(KeyNotFoundException ex)
					{
						throw new Exception(string.Format("Field with name \"{0}\" not exists", field.Name), ex);
					}
					object fieldValue = element.Values[fieldPosition];
					string indexValue = null;
					if(fieldValue is bool)
						indexValue = field.Description ?? field.Name;
					else if(fieldValue != null)
						indexValue = Convert.ToString(fieldValue, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
					if(value.Length > 0)
						value.Append(" ");
					if(!string.IsNullOrEmpty(indexValue))
						value.Append(indexValue);
				}
			}
			if(value.Length > 0)
				return value.ToString();
			var fieldTypes = index.FieldSchema.Select(f => f.Type.Name).Distinct().ToArray();
			if((fieldTypes.Length == 1 && fieldTypes[0] == "String") || fieldTypes.Contains("String"))
				return "<<NULL>>";
			return null;
		}

		/// <summary>
		/// Recupera o valor do objeto do campo.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public object GetFieldObjectValue(Element element, SchemeField field)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			int fieldPosition = 0;
			try
			{
				fieldPosition = _fieldsPosition[field.Name];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(string.Format("Field with name \"{0}\" not exists", field.Name), ex);
			}
			return element.Values[fieldPosition];
		}

		/// <summary>
		/// Recupera o valor do campo do elemento.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public string GetFieldValue(Element element, SchemeField field)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			int fieldPosition = 0;
			try
			{
				fieldPosition = _fieldsPosition[field.Name];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(string.Format("Field with name \"{0}\" not exists", field.Name), ex);
			}
			object value = element.Values[fieldPosition];
			if(value is bool)
				return (bool)value ? "true" : "false";
			else if(value is DateTime)
				return ((DateTime)value).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
			else if(value != null)
				return Convert.ToString(value, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
			return null;
		}

		/// <summary>
		/// Recupera o valor do campo do elemento para o fulltext.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public string GetFieldFullTextValue(Element element, string fieldName)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			int fieldPosition = 0;
			try
			{
				fieldPosition = _fieldsPosition[fieldName];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(string.Format("Field with name \"{0}\" not exists", fieldName), ex);
			}
			object value = element.Values[fieldPosition];
			string result = null;
			var field = _fields[fieldPosition];
			if(field.Type == typeof(bool))
			{
				try
				{
					if(value != null && ((value is bool && (bool)value) || Convert.ToBoolean(value)))
						result = field.Description;
				}
				catch
				{
				}
			}
			else if(value != null)
				result = Convert.ToString(value, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
			return result;
		}

		/// <summary>
		/// Recupera o indice do esquema
		/// </summary>
		/// <param name="indexName"></param>
		/// <returns></returns>
		public SchemeIndex GetSchemeIndex(string indexName)
		{
			int indexPosition = 0;
			try
			{
				indexPosition = _indexesPosition[indexName];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(string.Format("Index with name \"{0}\" not exists", indexName), ex);
			}
			var index = _indexes[indexPosition];
			return index;
		}

		/// <summary>
		/// Indices de string
		/// </summary>
		public SortedDictionary<string, SortedList<int, Element>> StringIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Indices de valor, permitem pesquisa por range
		/// </summary>
		public SortedDictionary<string, SortedDictionary<int, SortedList<int, Element>>> ValueIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se o canal está ou não cancelado
		/// </summary>
		public bool Cancelled
		{
			get;
			set;
		}
	}
}
