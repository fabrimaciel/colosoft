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

namespace Colosoft.Data.Schema.Local
{
	/// <summary>
	/// Implementação basica para <see cref="ITypeMetadata"/>.
	/// </summary>
	public class TypeMetadata : ITypeMetadata, System.Xml.Serialization.IXmlSerializable, ICloneable
	{
		private int _typeCode;

		private string _name;

		private string _namespace;

		private string _assembly;

		private bool _isCache;

		private bool _isVersioned;

		private TableName _tableName;

		private List<IPropertyMetadata> _properties;

		private List<ReferenceTypeMetadata> _baseTypes;

		/// <summary>
		/// Código do tipo.
		/// </summary>
		public int TypeCode
		{
			get
			{
				return _typeCode;
			}
			internal set
			{
				_typeCode = value;
			}
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Espaço de nome onde o tipo está inserido.
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			internal set
			{
				_namespace = value;
			}
		}

		/// <summary>
		/// Nome completo do tipo.
		/// </summary>
		public string FullName
		{
			get
			{
				return string.Format("{0}.{1}", Namespace, Name);
			}
		}

		/// <summary>
		/// Identifica se os metadados do tipo devem ficar em cache.
		/// </summary>
		public bool IsCache
		{
			get
			{
				return _isCache;
			}
		}

		/// <summary>
		/// Nome do assembly onde o tipo está inserido.
		/// </summary>
		public string Assembly
		{
			get
			{
				return _assembly;
			}
			internal set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Nome da tabela associada.
		/// </summary>
		public TableName TableName
		{
			get
			{
				return _tableName;
			}
		}

		/// <summary>
		/// Recupera a propriedade do tipo pelo nome informado.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IPropertyMetadata this[string propertyName]
		{
			get
			{
				return _properties.Where(f => f.Name == propertyName).FirstOrDefault();
			}
		}

		/// <summary>
		/// Recupera a quantidade de propriedades no tipo.
		/// </summary>
		public int Count
		{
			get
			{
				return _properties != null ? _properties.Count : 0;
			}
		}

		/// <summary>
		/// Define se a coluna é versionada ou não.
		/// </summary>
		public bool IsVersioned
		{
			get
			{
				return _isVersioned;
			}
		}

		/// <summary>
		/// Tipos base associados.
		/// </summary>
		public List<ReferenceTypeMetadata> BaseTypes
		{
			get
			{
				return _baseTypes;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TypeMetadata()
		{
			_properties = new List<IPropertyMetadata>();
			_baseTypes = new List<ReferenceTypeMetadata>();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <param name="name"></param>
		/// <param name="nameSpace"></param>
		/// <param name="assembly"></param>
		/// <param name="isCache"></param>
		/// <param name="isVersioned">Identififa se é um tipo versionado.</param>
		/// <param name="tableName"></param>
		/// <param name="properties"></param>
		/// <param name="baseTypes"></param>
		public TypeMetadata(int typeCode, string name, string nameSpace, string assembly, bool isCache, bool isVersioned, TableName tableName, IPropertyMetadata[] properties, ReferenceTypeMetadata[] baseTypes = null)
		{
			_typeCode = typeCode;
			_name = name;
			_namespace = nameSpace;
			_assembly = assembly;
			_isCache = isCache;
			_isVersioned = isVersioned;
			_tableName = tableName;
			_properties = new List<IPropertyMetadata>(properties ?? new IPropertyMetadata[0]);
			_baseTypes = new List<ReferenceTypeMetadata>(baseTypes ?? new ReferenceTypeMetadata[0]);
		}

		/// <summary>
		/// Recupera a propriedade pelo código.
		/// </summary>
		/// <param name="propertyCode"></param>
		/// <returns></returns>
		public IPropertyMetadata GetProperty(int propertyCode)
		{
			return _properties.Where(f => f.PropertyCode == propertyCode).FirstOrDefault();
		}

		/// <summary>
		/// Recupera as propriedades chaves do tipo.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IPropertyMetadata> GetKeyProperties()
		{
			return _properties.Where(f => f.ParameterType == PersistenceParameterType.Key || f.ParameterType == PersistenceParameterType.IdentityKey);
		}

		/// <summary>
		/// Recupera todas as propriedades que são recuperadas após a consulta.
		/// </summary>
		/// <returns>Propriedades voláteis</returns>
		public IEnumerable<IPropertyMetadata> GetVolatileProperties()
		{
			return _properties.Where(f => f.IsVolatile);
		}

		/// <summary>
		/// Tenta recupera os metadados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será pesquisada.</param>
		/// <param name="propertyMetadata">Metadados da propriedade encontrada.</param>
		/// <returns></returns>
		public bool TryGet(string propertyName, out IPropertyMetadata propertyMetadata)
		{
			propertyMetadata = _properties.FirstOrDefault(f => f.Name == propertyName);
			return propertyMetadata != null;
		}

		/// <summary>
		/// Recupera o enumerador das propriedades.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IPropertyMetadata> GetEnumerator()
		{
			foreach (var p in _properties)
				yield return p;
		}

		/// <summary>
		/// Recupera o enumerador das propriedades.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			int.TryParse(reader.GetAttribute("code"), out _typeCode);
			_name = reader.GetAttribute("name");
			_namespace = string.IsNullOrEmpty(reader.GetAttribute("namespace")) ? _namespace : reader.GetAttribute("namespace");
			_assembly = string.IsNullOrEmpty(reader.GetAttribute("assembly")) ? _assembly : reader.GetAttribute("assembly");
			_tableName = new TableName(reader.GetAttribute("catalog"), reader.GetAttribute("schema"), string.IsNullOrEmpty(reader.GetAttribute("tableName")) ? _name : reader.GetAttribute("tableName"));
			bool.TryParse(reader.GetAttribute("isCache"), out _isCache);
			bool.TryParse(reader.GetAttribute("isVersioned"), out _isVersioned);
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.LocalName == "Properties")
				{
					if(reader.IsEmptyElement)
						reader.Skip();
					else
					{
						reader.ReadStartElement();
						while (reader.LocalName == "Property" && reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							var propertyMetadata = new PropertyMetadata();
							((System.Xml.Serialization.IXmlSerializable)propertyMetadata).ReadXml(reader);
							if(propertyMetadata.ForeignKey != null)
							{
								if(string.IsNullOrEmpty(propertyMetadata.ForeignKey.Namespace))
									propertyMetadata.ForeignKey.Namespace = Namespace;
								if(string.IsNullOrEmpty(propertyMetadata.ForeignKey.Assembly))
									propertyMetadata.ForeignKey.Assembly = Assembly;
							}
							_properties.Add(propertyMetadata);
							reader.Skip();
						}
						reader.ReadEndElement();
					}
				}
				else if(reader.LocalName == "BaseTypes")
				{
					if(reader.IsEmptyElement)
						reader.Skip();
					else
					{
						reader.ReadStartElement();
						while (reader.LocalName == "ReferenceTypeMetadata" && reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							var reference = new ReferenceTypeMetadata();
							((System.Xml.Serialization.IXmlSerializable)reference).ReadXml(reader);
							if(string.IsNullOrEmpty(reference.Namespace))
								reference.Namespace = Namespace;
							if(string.IsNullOrEmpty(reference.Assembly))
								reference.Assembly = Assembly;
							_baseTypes.Add(reference);
							reader.Skip();
						}
						reader.ReadEndElement();
					}
				}
				else
					break;
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Serializa os dados como Xml.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("code", TypeCode.ToString());
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("namespace", Namespace);
			writer.WriteAttributeString("assembly", Assembly);
			writer.WriteAttributeString("catalog", TableName.Catalog);
			writer.WriteAttributeString("schema", TableName.Schema);
			writer.WriteAttributeString("tableName", TableName.Name);
			writer.WriteAttributeString("isCache", IsCache.ToString().ToLower());
			writer.WriteAttributeString("isVersioned", IsCache.ToString().ToLower());
			writer.WriteStartElement("Properties");
			foreach (var property in this)
			{
				if(property is System.Xml.Serialization.IXmlSerializable)
				{
					writer.WriteStartElement("Property");
					((System.Xml.Serialization.IXmlSerializable)property).WriteXml(writer);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
			writer.WriteStartElement("BaseTypes");
			foreach (var baseType in BaseTypes)
			{
				if(baseType is System.Xml.Serialization.IXmlSerializable)
				{
					writer.WriteStartElement("ReferenceTypeMetadata");
					((System.Xml.Serialization.IXmlSerializable)baseType).WriteXml(writer);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new TypeMetadata(_typeCode, _name, _namespace, _assembly, _isCache, _isVersioned, (TableName)_tableName.Clone(), _properties.Select(f => (IPropertyMetadata)((ICloneable)f).Clone()).ToArray(), _baseTypes.Select(f => (ReferenceTypeMetadata)((ICloneable)f).Clone()).ToArray());
		}
	}
}
