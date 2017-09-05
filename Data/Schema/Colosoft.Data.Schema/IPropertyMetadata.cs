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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Assinatura das classes que armazena os metadados
	/// da propriedade de um tipo.
	/// </summary>
	public interface IPropertyMetadata
	{
		/// <summary>
		/// Código da propriedade.
		/// </summary>
		int PropertyCode
		{
			get;
		}

		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Nome da coluna associada.
		/// </summary>
		string ColumnName
		{
			get;
		}

		/// <summary>
		/// Nome do tipo associado com a propriedade.
		/// </summary>
		string PropertyType
		{
			get;
		}

		/// <summary>
		/// Código de um tipo cuja propriedade é foreign key.
		/// </summary>
		int? ForeignKeyTypeCode
		{
			get;
		}

		/// <summary>
		/// Direção de persistencia da propriedade.
		/// </summary>
		DirectionParameter Direction
		{
			get;
		}

		/// <summary>
		/// Tipo de parametro que a propriedade representa.
		/// </summary>
		PersistenceParameterType ParameterType
		{
			get;
		}

		/// <summary>
		/// Boleano que define se a propriedade é referência alguma tabela.
		/// </summary>
		bool IsForeignKey
		{
			get;
		}

		/// <summary>
		/// Define se a propriedade pode ser persistida em cache.
		/// </summary>
		bool IsCacheIndexed
		{
			get;
		}

		/// <summary>
		/// Define se o campo deve ser sempre recuperado do banco de dados
		/// </summary>
		bool IsVolatile
		{
			get;
		}
	}
}
