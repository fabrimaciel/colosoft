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
	/// Representa o predicado usado para definir se uma consulta aninha
	/// deve ou não ser executada com base em parametros da consulta pai.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public sealed class QueryExecutePredicate : Colosoft.Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable, System.Runtime.Serialization.ISerializable, ICloneable
	{
		private string _expression;

		private RequiredField[] _requiredFields;

		private QueryParameter[] _parameters;

		[NonSerialized]
		private Delegate _predicateCompiled;

		/// <summary>
		/// Expressão do predicado.
		/// </summary>
		public string Expression
		{
			get
			{
				return _expression;
			}
			set
			{
				if(_expression != value)
				{
					_expression = value;
					_predicateCompiled = null;
				}
			}
		}

		/// <summary>
		/// Relação dos campos requeridos para o processamento do predicado.
		/// </summary>
		public RequiredField[] RequiredFields
		{
			get
			{
				return _requiredFields;
			}
			set
			{
				_requiredFields = value;
			}
		}

		/// <summary>
		/// Parametros do predicado.
		/// </summary>
		public QueryParameter[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				if(_parameters != value)
				{
					_parameters = value;
					_predicateCompiled = null;
				}
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public QueryExecutePredicate()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="parameters"></param>
		/// <param name="requiredFields"></param>
		public QueryExecutePredicate(string expression, QueryParameter[] parameters, RequiredField[] requiredFields)
		{
			_expression = expression;
			_parameters = parameters;
			_requiredFields = requiredFields;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private QueryExecutePredicate(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			Expression = info.GetString("Expression");
			var parametersCount = info.GetInt32("ParamCount");
			_parameters = new QueryParameter[parametersCount];
			for(var i = 0; i < parametersCount; i++)
				_parameters[i] = (QueryParameter)info.GetValue("P" + i, typeof(QueryParameter));
			var requiredFieldsCount = info.GetInt32("ReqCount");
			_requiredFields = new RequiredField[requiredFieldsCount];
			for(var i = 0; i < requiredFieldsCount; i++)
				_requiredFields[i] = (RequiredField)info.GetValue("R" + i, typeof(RequiredField));
		}

		/// <summary>
		/// Cria um novo predicado de execução.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static QueryExecutePredicate Create(string expression, params QueryParameter[] parameters)
		{
			expression.Require("expression").NotNull().NotEmpty();
			var lambdaExpression = Dynamic.DynamicExpression.ParseLambda<IRecord, bool>(expression, parameters);
			var requiredFields = new List<RequiredField>();
			foreach (var methodExpression in NavigateRecordMethods(lambdaExpression))
			{
				var firstArgument = methodExpression.Arguments.FirstOrDefault();
				if(firstArgument != null)
				{
					if(firstArgument is System.Linq.Expressions.ConstantExpression)
					{
						var argumentValue = ((System.Linq.Expressions.ConstantExpression)firstArgument).Value;
						if(argumentValue is string)
							requiredFields.Add(new RequiredField {
								Name = (string)argumentValue,
								Index = -1
							});
						else if(argumentValue is int)
							requiredFields.Add(new RequiredField {
								Index = (int)argumentValue
							});
					}
				}
			}
			return new QueryExecutePredicate(expression, parameters, requiredFields.ToArray());
		}

		/// <summary>
		/// Verifica se o predicado é valido com os dados
		/// do registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public bool Verify(IRecord record)
		{
			if(_predicateCompiled == null)
			{
				_predicateCompiled = Dynamic.DynamicExpression.ParseLambda<IRecord, bool>(Expression, Parameters).Compile();
			}
			try
			{
				return (bool)_predicateCompiled.DynamicInvoke(record);
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// Navega pelos método do Record que estão na expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		private static IEnumerable<System.Linq.Expressions.MethodCallExpression> NavigateRecordMethods(System.Linq.Expressions.Expression expression)
		{
			if(expression is System.Linq.Expressions.MethodCallExpression)
			{
				var methodCallExpression = (System.Linq.Expressions.MethodCallExpression)expression;
				foreach (var argument in methodCallExpression.Arguments)
					foreach (var i in NavigateRecordMethods(argument))
						yield return i;
				if(methodCallExpression.Method.DeclaringType == typeof(IRecord))
					yield return methodCallExpression;
			}
			else if(expression is System.Linq.Expressions.LambdaExpression)
			{
				var lambdaExpression = (System.Linq.Expressions.LambdaExpression)expression;
				foreach (var i in NavigateRecordMethods(lambdaExpression.Body))
					yield return i;
			}
			else if(expression is System.Linq.Expressions.BinaryExpression)
			{
				var binaryExpression = (System.Linq.Expressions.BinaryExpression)expression;
				foreach (var i in NavigateRecordMethods(binaryExpression.Left))
					yield return i;
				foreach (var i in NavigateRecordMethods(binaryExpression.Right))
					yield return i;
			}
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Expression", Expression);
			info.AddValue("ParamCount", Parameters == null ? 0 : Parameters.Length);
			for(var i = 0; Parameters != null && i < Parameters.Length; i++)
				info.AddValue("P" + i, Parameters[i]);
			info.AddValue("ReqCount", RequiredFields == null ? 0 : RequiredFields.Length);
			for(var i = 0; RequiredFields != null && i < RequiredFields.Length; i++)
				info.AddValue("R" + i, RequiredFields[i]);
		}

		void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			this.Expression = reader.ReadString();
			var count = reader.ReadInt32();
			var parameters = new List<QueryParameter>(count);
			for(var i = 0; i < count; i++)
			{
				var parameter = new QueryParameter();
				((Colosoft.Serialization.ICompactSerializable)parameter).Deserialize(reader);
				parameters.Add(parameter);
			}
			_parameters = parameters.ToArray();
			count = reader.ReadInt32();
			var requiredFields = new List<RequiredField>(count);
			for(var i = 0; i < count; i++)
			{
				var field = new RequiredField();
				((Colosoft.Serialization.ICompactSerializable)field).Deserialize(reader);
				requiredFields.Add(field);
			}
			_requiredFields = requiredFields.ToArray();
		}

		void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(Expression);
			writer.Write(Parameters == null ? 0 : Parameters.Length);
			for(var i = 0; Parameters != null && i < Parameters.Length; i++)
				((Colosoft.Serialization.ICompactSerializable)Parameters[i]).Serialize(writer);
			writer.Write(RequiredFields == null ? 0 : RequiredFields.Length);
			for(var i = 0; RequiredFields != null && i < RequiredFields.Length; i++)
				((Colosoft.Serialization.ICompactSerializable)RequiredFields[i]).Serialize(writer);
		}

		/// <summary>
		/// Recupera o esquema que representa o tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("QueryExecutePredicate", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			Expression = reader.ReadElementString("Expression", Namespaces.Query);
			var parameters = new List<QueryParameter>();
			reader.ReadStartElement("Parameters", Namespaces.Query);
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.LocalName == "QueryParameter")
				{
					var parameter = new QueryParameter();
					((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
					parameters.Add(parameter);
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
			var requiredFields = new List<RequiredField>();
			reader.ReadStartElement("RequiredFields", Namespaces.Query);
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.LocalName == "Field")
				{
					var field = new RequiredField();
					((System.Xml.Serialization.IXmlSerializable)field).ReadXml(reader);
					requiredFields.Add(field);
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
			this.Parameters = parameters.ToArray();
			this.RequiredFields = requiredFields.ToArray();
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString("Expression", Namespaces.Query, Expression);
			writer.WriteStartElement("Parameters", Namespaces.Query);
			if(_parameters != null)
				foreach (System.Xml.Serialization.IXmlSerializable i in _parameters)
				{
					writer.WriteStartElement("QueryParameter", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
			writer.WriteStartElement("RequiredFields", Namespaces.Query);
			if(_requiredFields != null)
				foreach (System.Xml.Serialization.IXmlSerializable i in _requiredFields)
				{
					writer.WriteStartElement("Field", Namespaces.Query);
					i.WriteXml(writer);
					writer.WriteEndElement();
				}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Armazena os dados de um campo requerido.
		/// </summary>
		[Serializable]
		[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
		public sealed class RequiredField : Colosoft.Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable, System.Runtime.Serialization.ISerializable, ICloneable
		{
			private string _name;

			private int _index;

			/// <summary>
			/// Nome do campo.
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
			/// Indice do campo.
			/// </summary>
			public int Index
			{
				get
				{
					return _index;
				}
				set
				{
					_index = value;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			public RequiredField()
			{
			}

			/// <summary>
			/// Construtor usado na deserialização dos dados.
			/// </summary>
			/// <param name="info"></param>
			/// <param name="context"></param>
			private RequiredField(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				Name = info.GetString("Name");
				Index = info.GetInt32("Index");
			}

			/// <summary>
			/// Recupera os dados da instancia.
			/// </summary>
			/// <param name="info"></param>
			/// <param name="context"></param>
			void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				info.AddValue("Name", Name);
				info.AddValue("Index", Index);
			}

			void Colosoft.Serialization.ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
			{
				_name = reader.ReadString();
				_index = reader.ReadInt32();
			}

			void Colosoft.Serialization.ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
			{
				writer.Write(Name);
				writer.Write(Index);
			}

			/// <summary>
			/// Recupera o esquema que representa o tipo.
			/// </summary>
			/// <param name="xs"></param>
			/// <returns></returns>
			public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
			{
				xs.ResolveQuerySchema();
				return new System.Xml.XmlQualifiedName("RequiredField", Namespaces.Query);
			}

			System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
			{
				throw new NotImplementedException();
			}

			void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
			{
				reader.ReadStartElement();
				Name = reader.ReadElementString("Name", Namespaces.Query);
				Index = int.Parse(reader.ReadElementString("Index", Namespaces.Query));
				reader.ReadEndElement();
			}

			void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
			{
				writer.WriteElementString("Name", Namespaces.Query, Name);
				writer.WriteElementString("Index", Namespaces.Query, Index.ToString());
			}

			/// <summary>
			/// Clona a instancia.
			/// </summary>
			/// <returns></returns>
			public object Clone()
			{
				return new RequiredField {
					_index = this._index,
					_name = this._name
				};
			}
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new QueryExecutePredicate {
				_expression = this._expression,
				_parameters = this._parameters.Select(f => (QueryParameter)f.Clone()).ToArray(),
				_requiredFields = this._requiredFields.Select(f => (RequiredField)f.Clone()).ToArray()
			};
		}
	}
}
