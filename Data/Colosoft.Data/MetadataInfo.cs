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
	/// Informações dos tipos de metadados
	/// </summary>
	public class MetadataInfo
	{
		/// <summary>
		/// Código do tipo.
		/// </summary>
		public int TypeCode
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Espaço de nome onde o tipo está inserido.
		/// </summary>
		public string Namespace
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do assembly onde o tipo está inserido.
		/// </summary>
		public string Assembly
		{
			get;
			set;
		}

		/// <summary>
		/// Catalago onde a tabela está inserida.
		/// </summary>
		public string Catalog
		{
			get;
			set;
		}

		/// <summary>
		/// Esquema onde a tabela está inserida.,
		/// </summary>
		public string Schema
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da tabela.
		/// </summary>
		public string TableName
		{
			get;
			set;
		}

		/// <summary>
		/// Define se o tipo pode ser persistido em cache
		/// </summary>
		public bool IsCache
		{
			get;
			set;
		}

		/// <summary>
		/// Informações das propriedades
		/// </summary>
		public PropertyInfo[] PropertyInfos
		{
			get;
			set;
		}
	}
}
