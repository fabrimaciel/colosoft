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

namespace Colosoft.Query
{
	/// <summary>
	/// Enumerador da direção do parâmetro.
	/// </summary>
	public enum ParameterDirection
	{
		/// <summary>
		/// Representa um parâmetro de input.
		/// </summary>
		Input = 1,
		/// <summary>
		/// Representa um parâmetro de output.
		/// </summary>
		Output = 2,
		/// <summary>
		/// Representa um parâmetro de output e de input.
		/// </summary>
		InputOutput = 3,
		/// <summary>
		/// Representa o retorno da Stored Procedure.
		/// </summary>
		ReturnValue = 6
	}
	/// <summary>
	/// Representa o parametro que será usado em uma consulta.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetConditionalTermSchema")]
	public sealed class QueryParameter : ISerializable, System.Xml.Serialization.IXmlSerializable, Colosoft.Serialization.ICompactSerializable, ICloneable
	{
		private string _name;

		private object _value;

		private ParameterDirection _direction;

		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Direção do parâmetro.
		/// </summary>
		public ParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		/// <summary>
		/// Construtor vazio passando a direção do parâmetro como padrão Input.
		/// </summary>
		/// <param name="direction">Direção do parâmetro.</param>
		public QueryParameter(ParameterDirection direction = ParameterDirection.Input)
		{
			_direction = direction;
		}

		/// <summary>
		/// Cria uma instância do parâmetro informando seus dados.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		/// <param name="direction">Direção do parâmetro.</param>
		public QueryParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input)
		{
			_name = name;
			_value = value;
			_direction = direction;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private QueryParameter(SerializationInfo info, StreamingContext context)
		{
			_name = info.GetString("Name");
			_direction = (ParameterDirection)info.GetInt32("Direction");
			var isNull = info.GetBoolean("IsNull");
			if(!isNull)
				_value = info.GetValue("Value", Type.GetType(info.GetString("Type")));
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", _name);
			info.AddValue("Direction", (int)_direction);
			info.AddValue("IsNull", _value == null);
			if(_value != null)
			{
				info.AddValue("Type", _value.GetType().FullName);
				info.AddValue("Value", _value);
			}
		}

		/// <summary>
		/// Recupera o esquema Xml da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetConditionalTermSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("QueryParameter", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Name"))
				_name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Direction"))
				_direction = (ParameterDirection)reader.ReadContentAsInt();
			Type type = null;
			if(reader.MoveToAttribute("Type"))
			{
				var typeString = reader.ReadContentAsString();
				if(!string.IsNullOrEmpty(typeString))
					type = Type.GetType(typeString, true);
			}
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				if(type == typeof(byte[]))
				{
					var content = (string)reader.ReadElementContentAs(typeof(string), null);
					_value = Convert.FromBase64String(content);
				}
				else
					_value = reader.ReadElementContentAs(type, null);
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", Name);
			writer.WriteAttributeString("Direction", ((int)_direction).ToString());
			if(_value != null)
			{
				object value = null;
				var valueType = _value.GetType();
				if(valueType.IsEnum)
				{
					valueType = Enum.GetUnderlyingType(valueType);
					value = Convert.ChangeType(_value, valueType);
				}
				else
					value = _value;
				writer.WriteAttributeString("Type", valueType.FullName);
				if(valueType == typeof(byte[]))
					writer.WriteValue(Convert.ToBase64String((byte[])value));
				else
					writer.WriteValue(value);
			}
			else
				writer.WriteAttributeString("Type", "");
		}

		/// <summary>
		/// Deserializa usando o CompactSerializer.
		/// </summary>
		/// <param name="reader">Representa o compact reader.</param>
		public void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_name = reader.ReadString();
			_direction = (ParameterDirection)reader.ReadInt32();
			var option = reader.ReadByte();
			if(option == 1)
				_value = reader.ReadObject();
			else if(option == 2)
			{
				var refParameter = new ReferenceParameter();
				((Colosoft.Serialization.ICompactSerializable)refParameter).Deserialize(reader);
				_value = refParameter;
			}
			else if(option == 3)
			{
				var queryInfo = new QueryInfo();
				((Colosoft.Serialization.ICompactSerializable)queryInfo).Deserialize(reader);
				_value = queryInfo;
			}
		}

		/// <summary>
		/// Serializa usando o CompactSerializer.
		/// </summary>
		/// <param name="writer">Representa o compact writer.</param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_name);
			writer.Write((int)_direction);
			if(_value != null)
			{
				if(_value is Queryable)
					_value = ((Queryable)_value).CreateQueryInfo();
				if(_value is ReferenceParameter)
				{
					writer.Write((byte)2);
					((Colosoft.Serialization.ICompactSerializable)_value).Serialize(writer);
				}
				else if(_value is QueryInfo)
				{
					writer.Write((byte)3);
					((Colosoft.Serialization.ICompactSerializable)_value).Serialize(writer);
				}
				else
				{
					writer.Write((byte)1);
					var valueType = _value.GetType();
					object value;
					if(valueType.IsEnum)
					{
						valueType = Enum.GetUnderlyingType(valueType);
						value = Convert.ChangeType(_value, valueType);
					}
					else
					{
						value = _value;
					}
					writer.WriteObject(value);
				}
			}
			else
				writer.Write((byte)0);
		}

		/// <summary>
		/// Clona um parâmetro.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new QueryParameter(Name, Value, ParameterDirection.Input);
		}

		/// <summary>
		/// Recupera os parametros das informações da consulta.
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <param name="findParametersOfQuery">Identifica se é para pesquisar os parametro de consultas contidas nos parametros.</param>
		/// <returns></returns>
		public static IEnumerable<QueryParameter> GetParameters(QueryInfo queryInfo, bool findParametersOfQuery)
		{
			foreach (var i in queryInfo.Parameters)
				foreach (var j in GetParameters(i, findParametersOfQuery))
					yield return j;
			foreach (var i in GetParameters(queryInfo.WhereClause))
				foreach (var j in GetParameters(i, findParametersOfQuery))
					yield return j;
			foreach (var entity in queryInfo.Entities)
				if(entity.SubQuery != null)
					foreach (var i in GetParameters(entity.SubQuery, findParametersOfQuery))
						foreach (var j in GetParameters(i, findParametersOfQuery))
							yield return j;
			foreach (var join in queryInfo.Joins)
				if(join.Conditional != null)
					foreach (var i in GetParameters(join.Conditional))
						foreach (var j in GetParameters(i, findParametersOfQuery))
							yield return j;
			if(queryInfo.Unions != null)
				foreach (var union in queryInfo.Unions)
					foreach (var i in GetParameters(union.Query, findParametersOfQuery))
						foreach (var j in GetParameters(i, findParametersOfQuery))
							yield return j;
		}

		/// <summary>
		/// Recupera os parametros contidos no parametro
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="findParametersOfQuery">Identifica se é para pesquisar os parametro de consultas contidas nos parametros.</param>
		/// <returns></returns>
		private static IEnumerable<QueryParameter> GetParameters(QueryParameter parameter, bool findParametersOfQuery)
		{
			if(findParametersOfQuery)
			{
				if(parameter.Value is QueryInfo)
				{
					var query = (QueryInfo)parameter.Value;
					foreach (var i in GetParameters(query, findParametersOfQuery))
						yield return i;
				}
				else if(parameter.Value is Queryable)
				{
					var queryable = (Queryable)parameter.Value;
					foreach (var i in GetParameters(queryable, findParametersOfQuery))
						yield return i;
				}
				else
					yield return parameter;
			}
			else
				yield return parameter;
		}

		/// <summary>
		/// Recupera os parametros do container.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		private static IEnumerable<QueryParameter> GetParameters(ConditionalContainer container)
		{
			foreach (var i in container.ParameterContainer)
				yield return i;
			foreach (var conditional in container.Conditionals)
				foreach (var i in GetParameters(conditional))
					yield return i;
		}

		/// <summary>
		/// Recupera os parametros.
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		private static IEnumerable<QueryParameter> GetParameters(ConditionalTerm term)
		{
			if(term is Conditional)
			{
				var conditional = (Conditional)term;
				if(conditional.Left != null)
					foreach (var i in GetParameters(conditional.Left))
						yield return i;
				if(conditional.Right != null)
					foreach (var i in GetParameters(conditional.Right))
						yield return i;
			}
			else if(term is FunctionCall)
			{
				var functionCall = (FunctionCall)term;
				foreach (var parameter in functionCall.Parameters)
					foreach (var i in GetParameters(parameter))
						yield return i;
			}
			else if(term is ConditionalContainer)
			{
				foreach (var i in GetParameters((ConditionalContainer)term))
					yield return i;
			}
		}

		/// <summary>
		/// Recupera os parametros das informações da consulta.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="findParametersOfQuery">Identifica se é para pesquisar os parametro de consultas contidas nos parametros.</param>
		/// <returns></returns>
		public static IEnumerable<QueryParameter> GetParameters(Queryable queryable, bool findParametersOfQuery)
		{
			foreach (var i in queryable.Parameters)
				foreach (var j in GetParameters(i, findParametersOfQuery))
					yield return j;
			foreach (var i in GetParameters(queryable.WhereClause))
				foreach (var j in GetParameters(i, findParametersOfQuery))
					yield return j;
			if(queryable.Entity != null && queryable.Entity.SubQuery != null)
				foreach (var i in GetParameters(queryable.Entity.SubQuery, findParametersOfQuery))
					foreach (var j in GetParameters(i, findParametersOfQuery))
						yield return j;
			foreach (var entity in queryable.JoinEntities)
				if(entity.SubQuery != null)
					foreach (var i in GetParameters(entity.SubQuery, findParametersOfQuery))
						foreach (var j in GetParameters(i, findParametersOfQuery))
							yield return j;
			foreach (var join in queryable.Joins)
				if(join.Conditional != null)
					foreach (var i in GetParameters(join.Conditional))
						foreach (var j in GetParameters(i, findParametersOfQuery))
							yield return j;
			if(queryable.Unions != null)
				foreach (var union in queryable.Unions)
					foreach (var i in GetParameters(union.Query, findParametersOfQuery))
						foreach (var j in GetParameters(i, findParametersOfQuery))
							yield return j;
		}
	}
}
