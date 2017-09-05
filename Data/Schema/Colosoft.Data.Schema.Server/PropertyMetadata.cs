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
	/// Assinatura das classes que armazena os metadados
	/// da propriedade de um tipo.
	/// </summary>
	[Serializable]
	public class PropertyMetadata : IPropertyMetadata
	{
		private int _propertyCode;

		private string _name;

		private string _columnName;

		private string _propertyType;

		private DirectionParameter _direction;

		private PersistenceParameterType _parameterType;

		private int? _foreignKeyTypeCode;

		private bool _isForeignKey;

		private bool _isCacheIndexed;

		private bool _isVolatile;

		/// <summary>
		/// Código do tipo em a propriedade está inserida
		/// </summary>
		internal int TypeCode
		{
			get;
			set;
		}

		/// <summary>
		/// Código da propriedade.
		/// </summary>
		public int PropertyCode
		{
			get
			{
				return _propertyCode;
			}
			set
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
			set
			{
				_name = value;
			}
		}

		/// <summary>
		///Nome da coluna 
		/// </summary>
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				_columnName = value;
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
		/// Código de um tipo cuja propriedade é foreign key
		/// </summary>
		public int? ForeignKeyTypeCode
		{
			get
			{
				return _foreignKeyTypeCode;
			}
			set
			{
				_foreignKeyTypeCode = value;
			}
		}

		/// <summary>
		/// Direção de persistência que a propriedade pode ser armazenada
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public DirectionParameter Direction
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
		/// Tipo de coluna de banco de dados que a propriedade representa
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public PersistenceParameterType ParameterType
		{
			get
			{
				return _parameterType;
			}
			set
			{
				_parameterType = value;
			}
		}

		/// <summary>
		/// Boleano que define se a propriedade é referência alguma tabela
		/// </summary>
		public bool IsForeignKey
		{
			get
			{
				return _isForeignKey;
			}
			set
			{
				_isForeignKey = value;
			}
		}

		/// <summary>
		/// Define se a propriedade pode ser persistida em cache
		/// </summary>
		public bool IsCacheIndexed
		{
			get
			{
				return _isCacheIndexed;
			}
			set
			{
				_isCacheIndexed = value;
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
			set
			{
				_isVolatile = value;
			}
		}
	}
}
