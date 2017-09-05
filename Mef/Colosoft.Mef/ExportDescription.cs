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

namespace Colosoft.Mef
{
	/// <summary>
	/// Define uma descrição abstrata de um Export.
	/// </summary>
	public class ExportDescription
	{
		/// <summary>
		/// Nome do contrato.
		/// </summary>
		public string ContractName
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do contrato.
		/// </summary>
		public Colosoft.Reflection.TypeName ContractTypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do contrato associado.
		/// </summary>
		public Type ContractType
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do membro.
		/// </summary>
		public string MemberName
		{
			get;
			set;
		}

		/// <summary>
		/// Metadados associados com o export.
		/// </summary>
		/// <value>An <see cref="IDictionary{TKey,TValue}"/> object.</value>
		public IDictionary<string, object> Metadata
		{
			get;
			set;
		}
	}
}
