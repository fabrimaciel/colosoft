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

namespace Colosoft.Extensions
{
	/// <summary>
	/// Atributo usado para define
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ExportMetadataAttribute : Attribute, Extensions.IExportMetadataAttribute
	{
		/// <summary>
		/// Nome do metadado.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Valor do metadado.
		/// </summary>
		public string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public ExportMetadataAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Recupera os metadados
		/// </summary>
		/// <param name="exportType">Tipo da exportação.</param>
		/// <returns></returns>
		public System.Collections.Hashtable GetMetadata(Type exportType)
		{
			var result = new System.Collections.Hashtable();
			result.Add(Name, Value);
			return result;
		}
	}
}
