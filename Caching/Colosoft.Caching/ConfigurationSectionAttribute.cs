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
	/// Representa um atributo de configuraçãod a seção.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigurationSectionAttribute : ConfigurationAttributeBase
	{
		private string _sectionName;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sectionName"></param>
		public ConfigurationSectionAttribute(string sectionName) : base(false, false)
		{
			_sectionName = sectionName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionName"></param>
		/// <param name="isRequired"></param>
		/// <param name="isCollection"></param>
		public ConfigurationSectionAttribute(string sectionName, bool isRequired, bool isCollection) : base(isRequired, isCollection)
		{
			_sectionName = sectionName;
		}

		/// <summary>
		/// Nome da seção.
		/// </summary>
		public string SectionName
		{
			get
			{
				return _sectionName;
			}
		}
	}
}
