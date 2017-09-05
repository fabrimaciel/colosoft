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

namespace Colosoft.Data
{
	/// <summary>
	/// Informações das propriedades de metadados
	/// </summary>
	public class PropertyInfo
	{
		/// <summary>
		/// Código da propriedade.
		/// </summary>
		public int PropertyCode
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		///Nome da coluna 
		/// </summary>
		public string ColumnName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo da propriedade
		/// </summary>
		public string PropertyType
		{
			get;
			set;
		}

		/// <summary>
		/// Código de um tipo cuja propriedade é foreign key
		/// </summary>
		public int? ForeignKeyTypeCode
		{
			get;
			set;
		}

		/// <summary>
		/// Direção de persistência que a propriedade pode ser armazenada
		/// </summary>
		public int Direction
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo de coluna de banco de dados que a propriedade representa
		/// </summary>
		public int ParameterType
		{
			get;
			set;
		}

		/// <summary>
		/// Boleano que define se a propriedade é referência alguma tabela
		/// </summary>
		public bool IsForeignMember
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a propriedade representa uma chave estrangeira.
		/// </summary>
		public bool IsForeignKey
		{
			get;
			set;
		}

		/// <summary>
		/// Define se a propriedade pode ser persistida em cache
		/// </summary>
		public bool IsCacheIndexed
		{
			get;
			set;
		}
	}
}
