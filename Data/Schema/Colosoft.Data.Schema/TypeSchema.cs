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
	/// Implementação do esquema de tipos com suporte a dados em memória local.
	/// </summary>
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public class TypeSchema : ITypeSchema, System.Xml.Serialization.IXmlSerializable, ICloneable
	{
		private object _objLock = new object();

		private Func<string, ITypeMetadata> _typeMetadataLoaderFromFullName;

		private Func<int, ITypeMetadata> _typeMetadataLoaderFromTypeCode;

		private Func<int, ITypeMetadata> _typeMetadataLoaderFromPropertyCode;

		private Func<IEnumerable<ITypeMetadata>> _typeMetadataLoader;

		private Dictionary<string, ITypeMetadata> _typeMetadatasFromFullName = new Dictionary<string, ITypeMetadata>();

		private Dictionary<int, ITypeMetadata> _typeMetadatasFromTypeCode = new Dictionary<int, ITypeMetadata>();

		private Dictionary<int, IPropertyMetadata> _propertyMetadatas = new Dictionary<int, IPropertyMetadata>();

		/// <summary>
		/// Identifica se já carregou todos.
		/// </summary>
		private bool _loadedAll = false;

		private string _defaultNamespace;

		private string _defaultAssembly;

		private string _defaultSchema;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TypeSchema()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeMetadataLoaderFromFullName">Delegate usado para carregar os metadados do tipo pelo FullName.</param>
		/// <param name="typeMetadataLoaderFromTypeCode">>Delegate usado para carregar os metadados do tipo pelo TypeCode.</param>
		/// <param name="typeMetadataLoaderFromPropertyCode">>Delegate usado para carregar os metadados do tipo pelo PropertyCode.</param>
		/// <param name="typeMetadataLoader">Delegate usado para carregar todos os metadados.</param>
		public TypeSchema(Func<string, ITypeMetadata> typeMetadataLoaderFromFullName, Func<int, ITypeMetadata> typeMetadataLoaderFromTypeCode, Func<int, ITypeMetadata> typeMetadataLoaderFromPropertyCode, Func<IEnumerable<ITypeMetadata>> typeMetadataLoader)
		{
			Initialize(typeMetadataLoaderFromFullName, typeMetadataLoaderFromTypeCode, typeMetadataLoaderFromPropertyCode, typeMetadataLoader);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="typeMetadataLoaderFromFullName">Delegate usado para carregar os metadados do tipo pelo FullName.</param>
		/// <param name="typeMetadataLoaderFromTypeCode">>Delegate usado para carregar os metadados do tipo pelo TypeCode.</param>
		/// <param name="typeMetadataLoaderFromPropertyCode">>Delegate usado para carregar os metadados do tipo pelo PropertyCode.</param>
		/// <param name="typeMetadataLoader">Delegate usado para carregar todos os metadados.</param>
		protected void Initialize(Func<string, ITypeMetadata> typeMetadataLoaderFromFullName, Func<int, ITypeMetadata> typeMetadataLoaderFromTypeCode, Func<int, ITypeMetadata> typeMetadataLoaderFromPropertyCode, Func<IEnumerable<ITypeMetadata>> typeMetadataLoader)
		{
			_typeMetadataLoaderFromFullName = typeMetadataLoaderFromFullName;
			_typeMetadataLoaderFromPropertyCode = typeMetadataLoaderFromPropertyCode;
			_typeMetadataLoaderFromTypeCode = typeMetadataLoaderFromTypeCode;
			_typeMetadataLoader = typeMetadataLoader;
		}

		/// <summary>
		/// Verifica se no esquema já existem algum metadado associado ao informado.
		/// </summary>
		/// <param name="typeMetadata"></param>
		/// <returns></returns>
		private bool InternalExists(ITypeMetadata typeMetadata)
		{
			typeMetadata.Require("typeMetadata").NotNull();
			if(_typeMetadatasFromFullName.ContainsKey(typeMetadata.FullName))
				return false;
			if(typeMetadata.TypeCode == 0 && (typeMetadata is TypeMetadata || typeMetadata is AggregateTypeMetadata))
			{
				var code = (_typeMetadatasFromTypeCode.Count > 0 ? _typeMetadatasFromTypeCode.Keys.Where(f => f > 500000).Max() : 500000) + 1;
				if(typeMetadata is TypeMetadata)
					((TypeMetadata)typeMetadata).TypeCode = code;
				else
					((AggregateTypeMetadata)typeMetadata).TypeCode = code;
			}
			return _typeMetadatasFromTypeCode.ContainsKey(typeMetadata.TypeCode);
		}

		/// <summary>
		/// Adiciona os metadados de um tipo para o esquema.
		/// </summary>
		/// <param name="typeMetadata"></param>
		/// <returns>True se os dados foram adicionados com sucesso.</returns>
		private bool InternalAddTypeMetadata(ITypeMetadata typeMetadata)
		{
			if(InternalExists(typeMetadata))
				return false;
			_typeMetadatasFromFullName.Add(typeMetadata.FullName, typeMetadata);
			_typeMetadatasFromTypeCode.Add(typeMetadata.TypeCode, typeMetadata);
			foreach (var prop in typeMetadata)
			{
				if(prop.PropertyCode == 0 && prop is PropertyMetadata)
					((PropertyMetadata)prop).PropertyCode = (_propertyMetadatas.Count > 0 ? _propertyMetadatas.Keys.Where(f => f > 500000).Max() : 500000) + 1;
				if(!_propertyMetadatas.ContainsKey(prop.PropertyCode))
					_propertyMetadatas.Add(prop.PropertyCode, prop);
			}
			return true;
		}

		/// <summary>
		/// Verifica se no esquema já existem algum metadado associado ao informado.
		/// </summary>
		/// <param name="typeMetadata"></param>
		/// <returns></returns>
		public bool Exists(ITypeMetadata typeMetadata)
		{
			lock (_objLock)
				return InternalExists(typeMetadata);
		}

		/// <summary>
		/// Adiciona os metadados de um tipo para o esquema.
		/// </summary>
		/// <param name="typeMetadata"></param>
		/// <returns>True se os dados foram adicionados com sucesso.</returns>
		public bool AddTypeMetadata(ITypeMetadata typeMetadata)
		{
			lock (_objLock)
				return InternalAddTypeMetadata(typeMetadata);
		}

		/// <summary>
		/// Recupera os metadados do tipo com o nome informado.
		/// </summary>
		/// <param name="fullName">Nome completo do tipo.</param>
		/// <returns>Instancia dos metadados do tipo.</returns>
		public ITypeMetadata GetTypeMetadata(string fullName)
		{
			ITypeMetadata result = null;
			lock (_objLock)
			{
				if(!_typeMetadatasFromFullName.TryGetValue(fullName, out result) && _typeMetadataLoaderFromFullName != null)
				{
					result = _typeMetadataLoaderFromFullName(fullName);
					if(result != null)
						InternalAddTypeMetadata(result);
				}
			}
			return result;
		}

		/// <summary>
		/// Recupera os metadados do tipo com base no código informado.
		/// </summary>
		/// <param name="typeCode">Código do tipo.</param>
		/// <returns></returns>
		public ITypeMetadata GetTypeMetadata(int typeCode)
		{
			ITypeMetadata result = null;
			lock (_objLock)
			{
				if(!_typeMetadatasFromTypeCode.TryGetValue(typeCode, out result) && _typeMetadataLoaderFromTypeCode != null)
				{
					result = _typeMetadataLoaderFromTypeCode(typeCode);
					if(result != null)
						InternalAddTypeMetadata(result);
				}
			}
			return result;
		}

		/// <summary>
		/// Recupera os metadados de uma propriedade pelo código informado.
		/// </summary>
		/// <param name="propertyCode">Código da propriedade.</param>
		/// <returns>Instancia dos metadados da propriedade.</returns>
		public IPropertyMetadata GetPropertyMetadata(int propertyCode)
		{
			IPropertyMetadata result = null;
			lock (_objLock)
			{
				if(!_propertyMetadatas.TryGetValue(propertyCode, out result) && _typeMetadataLoaderFromPropertyCode != null)
				{
					var typeMetadata = _typeMetadataLoaderFromPropertyCode(propertyCode);
					if(typeMetadata != null)
					{
						InternalAddTypeMetadata(typeMetadata);
						result = typeMetadata.GetProperty(propertyCode);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Recupera os metadados dos tipos associados com o assembly informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly onde os tipo estão inseridos.</param>
		/// <returns></returns>
		public IEnumerable<ITypeMetadata> GetTypeMetadatas(string assemblyName)
		{
			return _typeMetadataLoader();
		}

		/// <summary>
		/// Recupera os metadados de todos o tipos registrados
		/// </summary>
		/// <returns>Intâncias dos metadados dos tipos</returns>
		public IEnumerable<ITypeMetadata> GetTypeMetadatas()
		{
			if(!_loadedAll)
			{
				lock (_objLock)
				{
					if(_typeMetadataLoader != null)
						foreach (var i in _typeMetadataLoader())
							InternalAddTypeMetadata(i);
					_loadedAll = true;
				}
			}
			return _typeMetadatasFromTypeCode.Values;
		}

		/// <summary>
		/// Realiza um Fix nas chaves estrangeiras.
		/// </summary>
		public void FixForeignKeys()
		{
			foreach (PropertyMetadata property in _propertyMetadatas.Values)
			{
				if(property.ForeignKey != null)
				{
					var type = GetTypeMetadata(string.Format("{0}.{1}", property.ForeignKey.Namespace, property.ForeignKey.TypeName));
					if(type != null)
					{
						var fkProperty = type.FirstOrDefault(f => f.Name == property.ForeignKey.Property);
						if(fkProperty != null)
						{
							property.IsForeignKey = true;
							property.ForeignKeyTypeCode = fkProperty.PropertyCode;
						}
					}
				}
			}
		}

		/// <summary>
		/// Carrega os dados do esquema contidos no arquivo.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static TypeSchema Load(string fileName)
		{
			using (var inputStream = System.IO.File.OpenRead(fileName))
				return Load(inputStream);
		}

		/// <summary>
		/// Carrega os dados do esquema contidos na stream de dados.
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static TypeSchema Load(System.IO.Stream inputStream)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TypeSchema), Namespaces.Schema);
			return (TypeSchema)serializer.Deserialize(inputStream);
		}

		/// <summary>
		/// Recupera o esquema de serialização.
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveSchema();
			return new System.Xml.XmlQualifiedName("TypeSchema", Namespaces.Schema);
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			_defaultNamespace = reader.GetAttribute("namespace");
			_defaultAssembly = reader.GetAttribute("assembly");
			_defaultSchema = reader.GetAttribute("schema");
			reader.ReadStartElement();
			var typesMetadata = new List<ITypeMetadata>();
			if(!reader.IsEmptyElement)
			{
				while (reader.LocalName == "TypeMetadata" && reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var typeMetadata = new TypeMetadata();
					if(string.IsNullOrEmpty(typeMetadata.Namespace) && !string.IsNullOrEmpty(_defaultNamespace))
						typeMetadata.Namespace = _defaultNamespace;
					if(string.IsNullOrEmpty(typeMetadata.Assembly) && !string.IsNullOrEmpty(_defaultAssembly))
						typeMetadata.Assembly = _defaultAssembly;
					((System.Xml.Serialization.IXmlSerializable)typeMetadata).ReadXml(reader);
					if(string.IsNullOrEmpty(typeMetadata.TableName.Schema) && !string.IsNullOrEmpty(_defaultSchema))
						typeMetadata.TableName.Schema = _defaultSchema;
					typesMetadata.Add(typeMetadata);
				}
				reader.Skip();
			}
			else
				reader.ReadEndElement();
			foreach (var i in typesMetadata)
			{
				var result = i;
				var typeMetadata = result as TypeMetadata;
				if(typeMetadata != null && typeMetadata.BaseTypes.Any())
				{
					var baseTypes = typeMetadata.BaseTypes.Select(f => typesMetadata.FirstOrDefault(x => x.FullName == f.FullName)).Where(f => f != null).ToArray();
					if(baseTypes.Any())
						result = new AggregateTypeMetadata(new[] {
							result
						}.Concat(baseTypes));
				}
				this.AddTypeMetadata(result);
			}
			FixForeignKeys();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (var i in _typeMetadatasFromFullName.Values)
			{
				if(i is System.Xml.Serialization.IXmlSerializable)
				{
					writer.WriteStartElement("TypeMetadata");
					((System.Xml.Serialization.IXmlSerializable)i).WriteXml(writer);
					writer.WriteEndElement();
				}
			}
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var instance = new TypeSchema();
			foreach (var i in _typeMetadatasFromFullName.Values)
				instance.AddTypeMetadata(i);
			return instance;
		}

		/// <summary>
		/// Recarrega o esquema.
		/// </summary>
		public void Reload()
		{
		}
	}
}
