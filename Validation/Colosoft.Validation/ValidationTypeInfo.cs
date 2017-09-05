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

namespace Colosoft.Validation
{
	/// <summary>
	/// Armazena as informações de validação de um tipo.
	/// </summary>
	public class ValidationTypeInfo
	{
		private string _typeName;

		private string _typeNamespace;

		private string _typeAssembly;

		private PropertyValidationInfo[] _properties;

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
		}

		/// <summary>
		/// Namespace do tipo.
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
		/// Propriedades do tipo.
		/// </summary>
		public PropertyValidationInfo[] Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeNamespace"></param>
		/// <param name="typeName"></param>
		/// <param name="typeAssembly"></param>
		/// <param name="properties"></param>
		public ValidationTypeInfo(string typeNamespace, string typeName, string typeAssembly, PropertyValidationInfo[] properties)
		{
			_typeNamespace = typeNamespace;
			_typeName = typeName;
			_typeAssembly = typeAssembly;
			_properties = properties ?? new PropertyValidationInfo[0];
		}
	}
}
