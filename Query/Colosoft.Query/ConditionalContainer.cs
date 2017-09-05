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
using Colosoft.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Enumeration dos operadores lógicos usados no comando sql.
	/// </summary>
	public enum LogicalOperator
	{
		/// <summary>
		/// E.
		/// </summary>
		And,
		/// <summary>
		/// Ou.
		/// </summary>
		Or
	}
	/// <summary>
	/// Container de uma estrutura condicional.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetConditionalContainerSchema")]
	public class ConditionalContainer : ConditionalTerm, IQueryParameterContainerExt, System.Xml.Serialization.IXmlSerializable, ICompactSerializable, Collections.ISearchParameterDescriptionContainer
	{
		/// <summary>
		/// Condições do container.
		/// </summary>
		private List<ConditionalTerm> _conditionals = new List<ConditionalTerm>();

		/// <summary>
		/// Operadores lógicos usados.
		/// </summary>
		private List<LogicalOperator> _logicalOperators = new List<LogicalOperator>();

		private QueryParameterCollection _parameters = new QueryParameterCollection();

		private IQueryParameterContainer _parameterContainer;

		[NonSerialized]
		private Collections.ISearchParameterDescriptionContainer _searchParameterDescriptionContainer;

		[NonSerialized]
		private Collections.SearchParameterDescriptionCollection _searchParameterDescriptions;

		/// <summary>
		/// Container onde são registrados os parametros.
		/// </summary>
		public IQueryParameterContainer ParameterContainer
		{
			get
			{
				if(_parameterContainer == null)
					return this;
				return _parameterContainer;
			}
			set
			{
				if(_parameterContainer != null && value != null && _parameterContainer != value)
					foreach (var i in _parameterContainer)
					{
						var found = false;
						foreach (var j in value)
							if(j.Name == i.Name)
							{
								found = true;
								continue;
							}
						if(!found)
							value.Add(i);
					}
				_parameterContainer = value;
			}
		}

		/// <summary>
		/// Recupera os operadores lógicos do container.
		/// </summary>
		public IEnumerable<LogicalOperator> LogicalOperators
		{
			get
			{
				return _logicalOperators;
			}
		}

		/// <summary>
		/// Condicionais do container.
		/// </summary>
		public IEnumerable<ConditionalTerm> Conditionals
		{
			get
			{
				return _conditionals;
			}
		}

		/// <summary>
		/// Quantidade de itens no container.
		/// </summary>
		public int ConditionalsCount
		{
			get
			{
				return _conditionals.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ConditionalContainer()
		{
		}

		/// <summary>
		/// Cria a instancia com referência para o container
		/// das descrições dos parametros de pesquisa.
		/// </summary>
		/// <param name="searchParameterDescriptionContainer"></param>
		public ConditionalContainer(Collections.ISearchParameterDescriptionContainer searchParameterDescriptionContainer)
		{
			_searchParameterDescriptionContainer = searchParameterDescriptionContainer;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="conditional">Condicionais do container.</param>
		public ConditionalContainer(ConditionalTerm conditional)
		{
			if(conditional != null)
			{
				_conditionals.Add(conditional);
				if(conditional is ConditionalContainer)
				{
					var cc = (ConditionalContainer)conditional;
					if(cc.ParameterContainer != this.ParameterContainer)
						cc.ParameterContainer = this.ParameterContainer;
				}
			}
		}

		/// <summary>
		/// Construtor que recebe vários <see cref="ConditionalTerm"/>.
		/// </summary>
		/// <param name="conditionals">Condicionais do container.</param>
		public ConditionalContainer(IEnumerable<ConditionalTerm> conditionals)
		{
			if(conditionals != null)
			{
				foreach (var conditional in conditionals)
				{
					_conditionals.Add(conditional);
					var cc = (ConditionalContainer)conditional;
					if(cc.ParameterContainer != this.ParameterContainer)
						cc.ParameterContainer = this.ParameterContainer;
				}
			}
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ConditionalContainer(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			var conditionalsCount = info.GetInt32("CC");
			var parametersCount = info.GetInt32("PC");
			var logicalOperatorsCount = info.GetInt32("LC");
			for(var i = 0; i < conditionalsCount; i++)
				_conditionals.Add((ConditionalTerm)info.GetValue("c" + i, Type.GetType(info.GetString("ct" + i), true)));
			for(var i = 0; i < parametersCount; i++)
				_parameters.Add((QueryParameter)info.GetValue("p" + i, Type.GetType(info.GetString("pt" + i), true)));
			for(var i = 0; i < logicalOperatorsCount; i++)
				_logicalOperators.Add((LogicalOperator)info.GetInt32("l" + i));
		}

		/// <summary>
		/// Configura o container das descrições de parametros de pesquisa.
		/// </summary>
		/// <param name="searchParameterDescriptionContainer"></param>
		public virtual void ConfigureSearchParameterDescriptionContainer(Collections.ISearchParameterDescriptionContainer searchParameterDescriptionContainer)
		{
			_searchParameterDescriptionContainer = searchParameterDescriptionContainer;
		}

		/// <summary>
		/// Adiciona a descrição da condicinal aplicada a consulta.
		/// </summary>
		/// <param name="description">Descrição que será adicionada.</param>
		/// <param name="parameterName">Nome do parametro associado a condicionais.</param>
		/// <returns></returns>
		public virtual ConditionalContainer AddDescription(IMessageFormattable description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, description);
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicinal aplicada a consulta.
		/// </summary>
		/// <param name="description">Descrição que será adicionada.</param>
		/// <param name="parameterName">Nome do parametro associado a condicionais.</param>
		/// <returns></returns>
		public virtual ConditionalContainer AddDescription(string description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, description.GetFormatter());
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public virtual ConditionalContainer AddDescription(Lazy<IMessageFormattable> description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, description);
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public virtual ConditionalContainer AddDescription(Lazy<string> description, string parameterName = null)
		{
			SearchParameterDescriptions.Add(parameterName, new Lazy<IMessageFormattable>(() => description.Value != null ? description.Value.GetFormatter() : null));
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public virtual ConditionalContainer AddDescription(Func<string> description, string parameterName = null)
		{
			description.Require("description").NotNull();
			SearchParameterDescriptions.Add(parameterName, new Lazy<IMessageFormattable>(() => description().GetFormatter()));
			return this;
		}

		/// <summary>
		/// Adiciona a descrição da condicional aplicada a consulta.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public virtual ConditionalContainer AddDescription(Func<IMessageFormattable> description, string parameterName = null)
		{
			description.Require("description").NotNull();
			SearchParameterDescriptions.Add(parameterName, new Lazy<IMessageFormattable>(() => description()));
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual ConditionalContainer And(ConditionalTerm conditional)
		{
			conditional.Require("conditional").NotNull();
			_conditionals.Add(conditional);
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.And);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Or(ConditionalTerm conditional)
		{
			conditional.Require("conditional").NotNull();
			_conditionals.Add(conditional);
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.Or);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer And(string expression)
		{
			expression.Require("expression").NotNull();
			_conditionals.Add(Conditional.ParseTerm(expression));
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.And);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Or(string expression)
		{
			expression.Require("expression").NotNull();
			_conditionals.Add(Conditional.ParseTerm(expression));
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.Or);
			return this;
		}

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Start(ConditionalTerm conditional)
		{
			conditional.Require("conditional").NotNull();
			_conditionals.Clear();
			_logicalOperators.Clear();
			_conditionals.Add(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Start(string expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			_conditionals.Clear();
			_logicalOperators.Clear();
			_conditionals.Add(Conditional.ParseTerm(expression));
			return this;
		}

		/// <summary>
		/// Executa o parse da expressão para um container.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static ConditionalContainer Parse(string expression, params QueryParameter[] parameters)
		{
			return ConditionalParser.Parse(expression).Add(parameters);
		}

		/// <summary>
		/// Executa o parse da expressão para um ConditionalTerm.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ConditionalTerm ParseTerm(string expression)
		{
			return ConditionalParser.ParseTerm(expression);
		}

		/// <summary>
		/// Recupera a string que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_conditionals.Count > 0)
			{
				var sb = new StringBuilder("(");
				for(int i = 0, j = -1; i < _conditionals.Count; i++, j++)
				{
					if(_logicalOperators.Count > j && j >= 0)
						sb.Append(_logicalOperators[j] == LogicalOperator.Or ? " || " : " && ");
					sb.Append(_conditionals[i].ToString());
				}
				sb.Append(")");
				return sb.ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// Clona um container condicional.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			var container = new ConditionalContainer();
			foreach (var term in this.Conditionals)
				container._conditionals.Add((ConditionalTerm)term.Clone());
			foreach (var param in this.ParameterContainer)
				container.Add((QueryParameter)param.Clone());
			container._logicalOperators = new List<LogicalOperator>(this.LogicalOperators);
			return container;
		}

		/// <summary>
		/// Adiciana um parametro para container.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ConditionalContainer Add(string name, object value)
		{
			ParameterContainer.Add(new QueryParameter(name, value));
			return this;
		}

		/// <summary>
		/// Adiciona um parametro para o container.
		/// </summary>
		/// <param name="parameter">Parâmetros a ser adicionado.</param>
		public ConditionalContainer Add(QueryParameter parameter)
		{
			ParameterContainer.Add(parameter);
			return this;
		}

		/// <summary>
		/// Adiciona um parametro para o container.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public ConditionalContainer Add(params QueryParameter[] parameters)
		{
			parameters.Require("parameters").NotNull();
			foreach (var i in parameters)
				ParameterContainer.Add(i);
			return this;
		}

		/// <summary>
		/// Adiciona vários parâmetros para o container.
		/// </summary>
		/// <param name="parameters">Parâmetros a serem adicionados.</param>
		public ConditionalContainer Add(IEnumerable<QueryParameter> parameters)
		{
			parameters.Require("parameters").NotNull();
			foreach (var i in parameters)
				ParameterContainer.Add(i);
			return this;
		}

		/// <summary>
		/// Recupera os dados
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("LC", _logicalOperators.Count);
			for(var i = 0; i < _logicalOperators.Count; i++)
				info.AddValue("l" + i, (int)_logicalOperators[i]);
			info.AddValue("CC", _conditionals.Count);
			for(var i = 0; i < _conditionals.Count; i++)
			{
				info.AddValue("ct" + i, _conditionals[i].GetType().FullName);
				info.AddValue("c" + i, _conditionals[i]);
			}
			info.AddValue("PC", ParameterContainer.Count);
			var index = 0;
			foreach (var i in ParameterContainer)
			{
				info.AddValue("pt" + index, i.GetType().FullName);
				info.AddValue("p" + (index++), i);
			}
		}

		/// <summary>
		/// Faz a desserialização do objeto.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			string position = reader.ReadString();
			while (position.Equals("ConditionalTerm"))
			{
				_conditionals.Add(GetConditionalTerm(reader));
				position = reader.ReadString();
			}
			while (position.Equals("LogicalOperator"))
			{
				_logicalOperators.Add((LogicalOperator)Enum.Parse(typeof(LogicalOperator), reader.ReadString()));
				position = reader.ReadString();
			}
			while (position.Equals("Parameter"))
			{
				var parameter = new QueryParameter();
				((ICompactSerializable)parameter).Deserialize(reader);
				Add(parameter);
				position = reader.ReadString();
			}
		}

		/// <summary>
		/// Faz a serialização compacta
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			foreach (ConditionalTerm i in _conditionals)
			{
				writer.Write("ConditionalTerm");
				writer.Write(i.GetType().Name);
				((ICompactSerializable)i).Serialize(writer);
			}
			foreach (var i in _logicalOperators)
			{
				writer.Write("LogicalOperator");
				writer.Write(i.ToString());
			}
			foreach (ICompactSerializable i in this.ParameterContainer)
			{
				writer.Write("Parameter");
				i.Serialize(writer);
			}
			writer.Write("End");
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
		IEnumerator<QueryParameter> IEnumerable<QueryParameter>.GetEnumerator()
		{
			if(_parameterContainer != null)
				return _parameterContainer.GetEnumerator();
			else
				return _parameters.GetEnumerator();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			if(_parameterContainer != null)
				return _parameterContainer.GetEnumerator();
			else
				return _parameters.GetEnumerator();
		}

		/// <summary>
		/// Remove todos os parametros.
		/// </summary>
		void IQueryParameterContainer.RemoveAllParameters()
		{
			if(_parameters != null)
				_parameters.RemoveAllParameters();
			if(_conditionals != null)
				foreach (var conditional in _conditionals)
					if(conditional is IQueryParameterContainer)
						((IQueryParameterContainer)conditional).RemoveAllParameters();
		}

		void IQueryParameterContainer.Add(QueryParameter parameter)
		{
			if(_parameters == null)
				_parameters = new QueryParameterCollection();
			_parameters.Add(parameter);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
		int IQueryParameterContainer.Count
		{
			get
			{
				return _parameters.Count;
			}
		}

		/// <summary>
		/// Remove o parametros informado.
		/// </summary>
		/// <param name="parameter"></param>
		bool IQueryParameterContainerExt.Remove(QueryParameter parameter)
		{
			return _parameters.Remove(parameter);
		}

		/// <summary>
		/// Recupera o esquema do registro.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetConditionalContainerSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("ConditionalContainer", Namespaces.Query);
		}

		/// <summary>
		/// Nome XML que irá representa a o tipo na serialização.
		/// </summary>
		public override System.Xml.XmlQualifiedName QualifiedName
		{
			get
			{
				return new System.Xml.XmlQualifiedName("ConditionalContainer", Namespaces.Query);
			}
		}

		/// <summary>
		/// Lê os dados serializados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			if(!reader.IsEmptyElement && reader.LocalName == "Conditionals")
			{
				reader.ReadStartElement("Conditionals", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					_conditionals.Add(GetConditionalTerm(reader));
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "LogicalOperators")
			{
				reader.ReadStartElement("LogicalOperators", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
					_logicalOperators.Add((LogicalOperator)Enum.Parse(typeof(LogicalOperator), reader.ReadElementString()));
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			if(!reader.IsEmptyElement && reader.LocalName == "Parameters")
			{
				reader.ReadStartElement("Parameters", Namespaces.Query);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var parameter = new QueryParameter();
					((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
					Add(parameter);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
			reader.ReadEndElement();
		}

		/// <summary>
		/// Salva os dados da instancia no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("xmlns", "i", null, Namespaces.SchemaInstance);
			writer.WriteStartElement("Conditionals", Namespaces.Query);
			foreach (ConditionalTerm i in _conditionals)
			{
				writer.WriteStartElement(i.QualifiedName.Name, i.QualifiedName.Namespace);
				var qname = i.QualifiedName;
				if(qname.Name != "ConditionalTerm")
				{
					var prefix = writer.LookupPrefix(qname.Namespace);
					if(string.IsNullOrEmpty(prefix))
						writer.WriteAttributeString("xmlns", qname.Namespace);
				}
				((System.Xml.Serialization.IXmlSerializable)i).WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("LogicalOperators", Namespaces.Query);
			foreach (var i in _logicalOperators)
				writer.WriteElementString("LogicalOperator", Namespaces.Query, i.ToString());
			writer.WriteEndElement();
			writer.WriteStartElement("Parameters", Namespaces.Query);
			foreach (System.Xml.Serialization.IXmlSerializable i in this.ParameterContainer)
			{
				writer.WriteStartElement("QueryParameter", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Descrições dos parametros de pesquisa.
		/// </summary>
		public Collections.SearchParameterDescriptionCollection SearchParameterDescriptions
		{
			get
			{
				if(_searchParameterDescriptionContainer != null)
					return _searchParameterDescriptionContainer.SearchParameterDescriptions;
				else if(_searchParameterDescriptions == null)
					_searchParameterDescriptions = new Collections.SearchParameterDescriptionCollection();
				return _searchParameterDescriptions;
			}
		}
	}
}
