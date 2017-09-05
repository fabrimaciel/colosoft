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
	/// Representa um atributo de configuração.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigurationAttributeAttribute : ConfigurationAttributeBase
	{
		private string _appendedText;

		private string _attributeName;

		/// <summary>
		/// Texto anexado.
		/// </summary>
		public string AppendedText
		{
			get
			{
				return _appendedText;
			}
		}

		/// <summary>
		/// Nome do atributo.
		/// </summary>
		public string AttributeName
		{
			get
			{
				return _attributeName;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="attribName">Nome do atributo.</param>
		public ConfigurationAttributeAttribute(string attribName) : base(false, false)
		{
			_appendedText = "";
			_attributeName = attribName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="attribName">Nome do atributo.</param>
		/// <param name="appendedText">Texto para ser anexado.</param>
		public ConfigurationAttributeAttribute(string attribName, string appendedText) : this(attribName, false, false, appendedText)
		{
			_attributeName = attribName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="attribName"></param>
		/// <param name="isRequired"></param>
		/// <param name="isCollection"></param>
		/// <param name="appendedText"></param>
		public ConfigurationAttributeAttribute(string attribName, bool isRequired, bool isCollection, string appendedText) : base(isRequired, false)
		{
			_appendedText = "";
			_attributeName = attribName;
			_appendedText = appendedText;
		}
	}
}
