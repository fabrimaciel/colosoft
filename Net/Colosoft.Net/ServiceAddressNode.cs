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
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Colosoft.Net
{
	/// <summary>
	/// Representa um nós dos dados do endereço do serviço.
	/// </summary>
	[Serializable]
	[XmlSchemaProvider("MySchema")]
	public sealed class ServiceAddressNode : IEnumerable<ServiceAddressNode>, System.Xml.Serialization.IXmlSerializable, ISerializable
	{
		private ServiceAddressParameterCollection _parameters;

		private List<ServiceAddressNode> _children;

		/// <summary>
		/// Nome do nó.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Acessa o nó filho no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ServiceAddressNode this[int index]
		{
			get
			{
				return _children[index];
			}
			set
			{
				_children[index] = value;
			}
		}

		/// <summary>
		/// Acessa o nó filho pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ServiceAddressNode this[string name]
		{
			get
			{
				return _children.Where(f => f.Name == name).FirstOrDefault();
			}
		}

		/// <summary>
		/// Parametros associados com o nó.
		/// </summary>
		public ServiceAddressParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServiceAddressNode()
		{
			_parameters = new ServiceAddressParameterCollection();
			_children = new List<ServiceAddressNode>();
		}

		/// <summary>
		/// Cria uma nova instancia do nó definindo os parametros iniciais.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameters"></param>
		/// <param name="children"></param>
		public ServiceAddressNode(string name, ServiceAddressParameterCollection parameters, params ServiceAddressNode[] children)
		{
			Name = name;
			if(parameters != null)
				_parameters = parameters;
			else
				_parameters = new ServiceAddressParameterCollection();
			_children = new List<ServiceAddressNode>();
			if(children != null)
				foreach (var i in children)
					if(i != null)
						Add(i);
		}

		/// <summary>
		/// Construtor usado na deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ServiceAddressNode(SerializationInfo info, StreamingContext context)
		{
			_children = new List<ServiceAddressNode>();
			_parameters = new ServiceAddressParameterCollection();
			Name = info.GetString("Name");
			_parameters = (ServiceAddressParameterCollection)info.GetValue("Parameters", typeof(ServiceAddressParameterCollection));
			var count = info.GetInt32("Count");
			for(var i = 0; i < count; i++)
				_children.Add((ServiceAddressNode)info.GetValue(i.ToString(), typeof(ServiceAddressNode)));
		}

		/// <summary>
		/// Adiciona um nó filho.
		/// </summary>
		/// <param name="node"></param>
		public void Add(ServiceAddressNode node)
		{
			_children.Add(node);
		}

		/// <summary>
		/// Remove um nó filho.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool Remove(ServiceAddressNode node)
		{
			return _children.Remove(node);
		}

		/// <summary>
		/// Verifica se existe no nó algum filho com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			return _children.Exists(f => f.Name == name);
		}

		/// <summary>
		/// Recupera o primeiro nó com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ServiceAddressNode GetNode(string name)
		{
			return _children.Find(f => f.Name == name);
		}

		/// <summary>
		/// Cria o nó a partir de um elemento xml.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static ServiceAddressNode CreateFromXmlElement(System.Xml.XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			var parameters = ServiceAddressParameterCollection.CreateFromXmlAttributes(element.Attributes);
			var children = new List<ServiceAddressNode>();
			if(element.HasChildNodes)
				foreach (var i in element.ChildNodes)
				{
					var xmlElement = i as System.Xml.XmlElement;
					if(xmlElement != null)
						children.Add(CreateFromXmlElement(xmlElement));
				}
			return new ServiceAddressNode(element.LocalName, parameters, children.ToArray());
		}

		/// <summary>
		/// Quantidade de itens no nó.
		/// </summary>
		public int Count
		{
			get
			{
				return _children.Count;
			}
		}

		/// <summary>
		/// Identifica se a coleção do nó é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return ((ICollection)_children).IsSynchronized;
			}
		}

		/// <summary>
		/// Elemento de sincronização do nó.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return ((ICollection)_children).SyncRoot;
			}
		}

		/// <summary>
		/// Recupera o enumerado para percorre os itens do nó.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		IEnumerator<ServiceAddressNode> IEnumerable<ServiceAddressNode>.GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		/// <summary>
		/// Método usado para recupera o esquema da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static XmlSchemaType MySchema(XmlSchemaSet xs)
		{
			var complexType = new XmlSchemaComplexType();
			complexType.Attributes.Add(new XmlSchemaAttribute {
				Name = "name",
				SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"),
				Use = XmlSchemaUse.Required
			});
			var sequence = new XmlSchemaSequence();
			var parametersElement = new XmlSchemaElement();
			parametersElement.Name = "Parameters";
			var complexTypeParameters = new XmlSchemaComplexType();
			var sequenceParameters = new XmlSchemaSequence();
			var paramElement = new XmlSchemaElement();
			paramElement.Name = "Param";
			paramElement.SchemaType = ServiceAddressParameter.MySchema(null);
			sequenceParameters.Items.Add(paramElement);
			complexTypeParameters.Particle = sequenceParameters;
			parametersElement.SchemaType = complexTypeParameters;
			sequence.Items.Add(parametersElement);
			var elementChildren = new XmlSchemaElement();
			elementChildren.Name = "Children";
			var compleTypeChildren = new XmlSchemaComplexType();
			var sequenceChildren = new XmlSchemaSequence();
			sequenceChildren.Items.Add(new XmlSchemaAny {
				MinOccurs = 0,
				MaxOccurs = decimal.MaxValue
			});
			compleTypeChildren.Particle = sequenceChildren;
			elementChildren.SchemaType = compleTypeChildren;
			sequence.Items.Add(elementChildren);
			complexType.Particle = sequence;
			return complexType;
		}

		/// <summary>
		/// Não usar;
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Recupera os dados contidos no xml informado.
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("name"))
				Name = reader.ReadContentAsString();
			reader.MoveToElement();
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.Name == "Parameters")
				{
					if(!reader.IsEmptyElement)
						((System.Xml.Serialization.IXmlSerializable)Parameters).ReadXml(reader);
					else
						reader.Skip();
				}
				else if(reader.Name == "Children")
				{
					if(!reader.IsEmptyElement)
						ReadChildrenXml(reader);
					else
						reader.Skip();
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Lê os filhos do nó.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadChildrenXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.Name == "Node")
				{
					var node = new ServiceAddressNode();
					((System.Xml.Serialization.IXmlSerializable)node).ReadXml(reader);
					this.Add(node);
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Escreve os dados do nó usando o escritor XML.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteStartElement("Parameters");
			((System.Xml.Serialization.IXmlSerializable)Parameters).WriteXml(writer);
			writer.WriteEndElement();
			WriteChildrenXml(writer);
		}

		private void WriteChildrenXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Children");
			foreach (var i in _children)
			{
				writer.WriteStartElement("Node");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", Name);
			info.AddValue("Parameters", Parameters);
			info.AddValue("Count", _children.Count);
			for(var i = 0; i < _children.Count; i++)
				info.AddValue(i.ToString(), _children[i]);
		}
	}
}
