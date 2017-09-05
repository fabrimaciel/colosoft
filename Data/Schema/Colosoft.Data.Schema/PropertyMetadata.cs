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
	/// Implementação básica dos metadados de uma propriedade.
	/// </summary>
	public class PropertyMetadata : IPropertyMetadata, System.Xml.Serialization.IXmlSerializable, ICloneable
	{
		private int _propertyCode;

		private string _name;

		private string _columnName;

		private string _propertyType;

		private int? _foreignKeyTypeCode;

		private bool _isCacheIndexed;

		private DirectionParameter _direction;

		private PersistenceParameterType _parameterType;

		bool _isForeignKey;

		bool _isVolatile;

		private ForeignKeyInfo _foreignKey;

		/// <summary>
		/// Código da propriedade.
		/// </summary>
		public int PropertyCode
		{
			get
			{
				return _propertyCode;
			}
			internal set
			{
				_propertyCode = value;
			}
		}

		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Nome da coluna associada.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
		}

		/// <summary>
		/// Nome do tipo associado com a propriedade.
		/// </summary>
		public string PropertyType
		{
			get
			{
				return _propertyType;
			}
			set
			{
				_propertyType = value;
			}
		}

		/// <summary>
		/// Código de um tipo cuja propriedade é foreign key.
		/// </summary>
		public int? ForeignKeyTypeCode
		{
			get
			{
				return _foreignKeyTypeCode;
			}
			internal set
			{
				_foreignKeyTypeCode = value;
			}
		}

		/// <summary>
		/// Identifica se está no cache.
		/// </summary>
		public bool IsCacheIndexed
		{
			get
			{
				return _isCacheIndexed;
			}
		}

		/// <summary>
		/// Direção de persistencia da propriedade.
		/// </summary>
		public DirectionParameter Direction
		{
			get
			{
				return _direction;
			}
		}

		/// <summary>
		/// Tipo de parametro que a propriedade representa.
		/// </summary>
		public PersistenceParameterType ParameterType
		{
			get
			{
				return _parameterType;
			}
		}

		/// <summary>
		/// Boleano que define se a propriedade é referência alguma tabela.
		/// </summary>
		public bool IsForeignKey
		{
			get
			{
				return _isForeignKey;
			}
			internal set
			{
				_isForeignKey = value;
			}
		}

		/// <summary>
		/// Define se o campo deve ser sempre recuperado do banco de dados
		/// </summary>
		public bool IsVolatile
		{
			get
			{
				return _isVolatile;
			}
		}

		/// <summary>
		/// Informações da chave estrangeira associada.
		/// </summary>
		public ForeignKeyInfo ForeignKey
		{
			get
			{
				return _foreignKey;
			}
			set
			{
				_foreignKey = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="propertyCode">Código da propriedade.</param>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="columnName">Nome da coluna associada.</param>
		/// <param name="propertyType">Nome do tipo associado com a propriedade.</param>
		/// <param name="foreignKeyTypeCode">Código de um tipo cuja propriedade é foreign key.</param>
		/// <param name="isCacheIndexed">Identifica se está no cache.</param>
		/// <param name="direction">Direção de persistencia da propriedade.</param>
		/// <param name="isVolatile">Define se o campo deve ser sempre recuperado do banco de dados</param>
		/// <param name="parameterType">Tipo de parametro que a propriedade representa.</param>
		/// <param name="isForeignMember">Boleano que define se a propriedade é referência alguma tabela.</param>
		public PropertyMetadata(int propertyCode, string name, string columnName, string propertyType, int? foreignKeyTypeCode = null, bool isCacheIndexed = true, DirectionParameter direction = DirectionParameter.InputOutput, bool isVolatile = false, PersistenceParameterType parameterType = PersistenceParameterType.Field, bool isForeignMember = false)
		{
			_propertyCode = propertyCode;
			_name = name;
			_columnName = columnName;
			_propertyType = propertyType;
			_foreignKeyTypeCode = foreignKeyTypeCode;
			_isCacheIndexed = isCacheIndexed;
			_direction = direction;
			_parameterType = parameterType;
			_isForeignKey = isForeignMember;
			_isVolatile = isVolatile;
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public PropertyMetadata()
		{
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			int.TryParse(reader.GetAttribute("code"), out _propertyCode);
			_name = reader.GetAttribute("name");
			_columnName = string.IsNullOrEmpty(reader.GetAttribute("columnName")) ? _name : reader.GetAttribute("columnName");
			_propertyType = reader.GetAttribute("type");
			if(!Enum.TryParse<DirectionParameter>(reader.GetAttribute("direction"), out _direction))
				_direction = DirectionParameter.InputOutput;
			if(!Enum.TryParse<PersistenceParameterType>(reader.GetAttribute("parameterType"), out _parameterType))
				_parameterType = PersistenceParameterType.Field;
			bool.TryParse(reader.GetAttribute("isVolatile"), out _isVolatile);
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				if(reader.LocalName == "ForeignKey" && reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var fkInfo = new ForeignKeyInfo();
					((System.Xml.Serialization.IXmlSerializable)fkInfo).ReadXml(reader);
					_foreignKey = fkInfo;
					_isForeignKey = true;
					reader.Skip();
				}
				else
					reader.ReadEndElement();
			}
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("code", PropertyCode.ToString());
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("columnName", ColumnName);
			writer.WriteAttributeString("type", PropertyType);
			writer.WriteAttributeString("direction", Direction.ToString());
			writer.WriteAttributeString("parameterType", ParameterType.ToString());
			writer.WriteAttributeString("isVolatile", IsVolatile.ToString().ToLower());
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Code: {0}, Name: {1}", PropertyCode, Name);
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new PropertyMetadata(_propertyCode, _name, _columnName, _propertyType, _foreignKeyTypeCode, _isCacheIndexed, _direction, _isVolatile, _parameterType, _isForeignKey);
		}
	}
}
