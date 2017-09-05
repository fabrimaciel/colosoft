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
	/// Representa um tipo de entidade do sistema.
	/// </summary>
	public class BusinessEntityType
	{
		private string _fullName;

		private BusinessEntityTypeVersion[] _versions;

		/// <summary>
		/// Nome da entidade
		/// </summary>
		public string FullName
		{
			get
			{
				return _fullName;
			}
		}

		/// <summary>
		/// Versões do tipo.
		/// </summary>
		public BusinessEntityTypeVersion[] Versions
		{
			get
			{
				return _versions;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="versions"></param>
		public BusinessEntityType(string fullName, BusinessEntityTypeVersion[] versions)
		{
			fullName.Require("fullName").NotNull().NotEmpty();
			_fullName = fullName;
			_versions = versions ?? new BusinessEntityTypeVersion[0];
		}
	}
}
