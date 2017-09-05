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

namespace Colosoft.Business
{
	/// <summary>
	/// Representa a versão do tipo da entidade.
	/// </summary>
	public class BusinessEntityTypeVersion
	{
		private string _typeName;

		private string _typeNamespace;

		private string _typeAssembly;

		private string _description;

		private DateTime _startDate;

		private BusinessEntityTypeVersionProperty[] _properties;

		/// <summary>
		/// Nome do tipo da entidade.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
		}

		/// <summary>
		/// Namespace do tipo
		/// </summary>
		public string TypeNamespace
		{
			get
			{
				return _typeNamespace;
			}
		}

		/// <summary>
		/// Assembly do tipo.
		/// </summary>
		public string TypeAssembly
		{
			get
			{
				return _typeAssembly;
			}
		}

		/// <summary>
		/// Descrição da versão.
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
		}

		/// <summary>
		/// Data de inicio da versão.
		/// </summary>
		public DateTime StartDate
		{
			get
			{
				return _startDate;
			}
		}

		/// <summary>
		/// Propriedades associadas com a entidade.
		/// </summary>
		public BusinessEntityTypeVersionProperty[] Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="typeNamespace"></param>
		/// <param name="typeAssembly"></param>
		/// <param name="description"></param>
		/// <param name="startDate"></param>
		/// <param name="properties"></param>
		public BusinessEntityTypeVersion(string typeName, string typeNamespace, string typeAssembly, string description, DateTime startDate, BusinessEntityTypeVersionProperty[] properties)
		{
			typeName.Require("typeName").NotNull().NotEmpty();
			typeNamespace.Require("typeNamespace").NotNull().NotEmpty();
			typeAssembly.Require("typeAssembly").NotNull().NotEmpty();
			_typeName = typeName;
			_typeNamespace = typeNamespace;
			_typeAssembly = typeAssembly;
			_description = description;
			_startDate = startDate;
			_properties = properties ?? new BusinessEntityTypeVersionProperty[0];
		}
	}
}
