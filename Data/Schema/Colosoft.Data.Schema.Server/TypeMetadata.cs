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

namespace Colosoft.Data.Schema.Server
{
	/// <summary>
	/// Classe que armazena metadados de um tipo
	/// </summary>
	public class TypeMetadata : ITypeMetadata
	{
		private int _typeCode;

		private string _name;

		private string _namespace;

		private string _assembly;

		private TableName _tableInformation;

		private bool _isCache;

		private bool _isVersioned;

		private Dictionary<string, PropertyMetadata> _propertiesDictionaryKeyFullname = new Dictionary<string, PropertyMetadata>();

		private Dictionary<int, PropertyMetadata> _propertiesDictionaryKeyPropertyCode = new Dictionary<int, PropertyMetadata>();

		private List<PropertyMetadata> _keyPropertiesList = new List<PropertyMetadata>();

		/// <summary>
		/// Nome da tabela sql
		/// </summary>
		private string TableSqlName
		{
			get
			{
				if(_tableInformation != null)
					return _tableInformation.Name;
				else
					return null;
			}
			set
			{
				if(_tableInformation == null)
					_tableInformation = new TableName();
				_tableInformation.Name = value;
			}
		}

		/// <summary>
		/// Nome do schema sql
		/// </summary>
		private string TableSchema
		{
			get
			{
				if(_tableInformation != null)
					return _tableInformation.Schema;
				else
					return null;
			}
			set
			{
				if(_tableInformation == null)
					_tableInformation = new TableName();
				_tableInformation.Schema = value;
			}
		}

		/// <summary>
		/// Nome do catálogo sql
		/// </summary>
		private string TableCatalog
		{
			get
			{
				if(_tableInformation != null)
					return _tableInformation.Catalog;
				else
					return null;
			}
			set
			{
				if(_tableInformation == null)
					_tableInformation = new TableName();
				_tableInformation.Catalog = value;
			}
		}

		/// <summary>
		/// Código do tipo.
		/// </summary>
		public int TypeCode
		{
			get
			{
				return _typeCode;
			}
			set
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
			set
			{
				_name = value;
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
			set
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
				return string.IsNullOrEmpty(Namespace) ? Name : Namespace + "." + Name;
			}
			set
			{
				throw new NotImplementedException();
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
			set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Informações da tabela.
		/// </summary>
		public TableName TableName
		{
			get
			{
				return _tableInformation;
			}
			set
			{
				_tableInformation = value;
			}
		}

		/// <summary>
		/// Recupera os dados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns>Retorna os metadados da propriedade</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		IPropertyMetadata ITypeMetadata.this[string propertyName]
		{
			get
			{
				PropertyMetadata property = null;
				if(_propertiesDictionaryKeyFullname.TryGetValue(propertyName, out property))
					return property;
				else
					throw new Exception(ResourceMessageFormatter.Create(() => Server.Properties.Resources.Exception_PropertyNotFound, propertyName, this.FullName).Format());
			}
		}

		/// <summary>
		/// Recupera a quantidade de propriedades da tabela.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		[System.Xml.Serialization.SoapIgnore]
		public int Count
		{
			get
			{
				return _propertiesDictionaryKeyFullname.Count;
			}
		}

		/// <summary>
		/// Define se o tipo pode ser persistido em cache.
		/// </summary>
		public bool IsCache
		{
			get
			{
				return _isCache;
			}
			set
			{
				_isCache = value;
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
			set
			{
				_isVersioned = value;
			}
		}

		/// <summary>
		/// Propriedades do metadado do tipo.
		/// </summary>
		public PropertyMetadata[] Properties
		{
			get
			{
				return _propertiesDictionaryKeyFullname.Values.ToArray();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Recupera o enumerador genérico"/>
		/// </summary>
		/// <returns>Retorna enumerador genérico</returns>
		IEnumerator<IPropertyMetadata> IEnumerable<IPropertyMetadata>.GetEnumerator()
		{
			foreach (var propertyMetadata in _propertiesDictionaryKeyFullname)
				yield return propertyMetadata.Value;
		}

		/// <summary>
		/// Recupera o enumerador do <see cref="System.Collections.IEnumerator"/>
		/// </summary>
		/// <returns>Retorna enumerador genérico do <see cref="System.Collections.IEnumerator"/></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _propertiesDictionaryKeyFullname.GetEnumerator();
		}

		/// <summary>
		/// Carrega os metadados da propriedade nesse tipo
		/// </summary>
		/// <param name="property">Propriedade a ser carregada</param>
		/// <param name="logger"></param>
		internal void AddPropertyMetadata(PropertyMetadata property, Colosoft.Logging.ILogger logger)
		{
			if(property.TypeCode != this.TypeCode)
				throw new Exception(ResourceMessageFormatter.Create(() => global::Colosoft.Data.Schema.Server.Properties.Resources.PropertyAndTypeNotMatched, property.Name, this.Name).Format());
			try
			{
				_propertiesDictionaryKeyFullname.Add(property.Name, property);
			}
			catch(ArgumentException ex)
			{
				logger.Write(ResourceMessageFormatter.Create(() => global::Colosoft.Data.Schema.Server.Properties.Resources.PropertyAlreadyExistsTypeMetadata, property.Name, this.Namespace + this.Name), ex, Logging.Priority.High);
			}
			_propertiesDictionaryKeyPropertyCode.Add(property.PropertyCode, property);
			if(property.ParameterType == PersistenceParameterType.IdentityKey || property.ParameterType == PersistenceParameterType.Key)
				_keyPropertiesList.Add(property);
		}

		/// <summary>
		/// Recupera a propriedade pelo código.
		/// </summary>
		/// <param name="propertyCode">Código da propriedade</param>
		/// <returns>Retorna os metadados da propriedade</returns>
		IPropertyMetadata ITypeMetadata.GetProperty(int propertyCode)
		{
			try
			{
				return _propertiesDictionaryKeyPropertyCode[propertyCode];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(ResourceMessageFormatter.Create(() => Server.Properties.Resources.PropertyCodeNotMapped, propertyCode.ToString()).Format(), ex);
			}
		}

		/// <summary>
		/// Recupera as propriedades chaves.
		/// </summary>
		/// <returns>Retorna lista de propriedades chave</returns>
		public IEnumerable<IPropertyMetadata> GetKeyProperties()
		{
			return _keyPropertiesList;
		}

		/// <summary>
		/// Recupera todas as propriedades que são recuperadas após a consulta.
		/// </summary>
		/// <returns>Propriedades voláteis</returns>
		public IEnumerable<IPropertyMetadata> GetVolatileProperties()
		{
			return _propertiesDictionaryKeyFullname.Values.Where(f => f.IsVolatile);
		}

		/// <summary>
		/// Tenta recupera os metadados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será pesquisada.</param>
		/// <param name="propertyMetadata">Metadados da propriedade encontrada.</param>
		/// <returns></returns>
		public bool TryGet(string propertyName, out IPropertyMetadata propertyMetadata)
		{
			propertyMetadata = _keyPropertiesList.FirstOrDefault(f => f.Name == propertyName);
			return propertyMetadata != null;
		}
	}
}
