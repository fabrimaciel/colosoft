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

namespace Colosoft.Caching.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigurationRootAttribute : ConfigurationAttributeBase
	{
		private string _rootSectionName;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sectionName"></param>
		public ConfigurationRootAttribute(string sectionName) : base(false, false)
		{
			_rootSectionName = sectionName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rootSectionName"></param>
		/// <param name="isRequired"></param>
		/// <param name="isCollection"></param>
		public ConfigurationRootAttribute(string rootSectionName, bool isRequired, bool isCollection) : base(isRequired, isCollection)
		{
			_rootSectionName = rootSectionName;
		}

		/// <summary>
		/// Nome da seção principal.
		/// </summary>
		public string RootSectionName
		{
			get
			{
				return _rootSectionName;
			}
		}
	}
}
