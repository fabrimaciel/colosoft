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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Armazena os dados do parametro.
	/// </summary>
	[Serializable]
	public class Parameter : ICloneable
	{
		private string _name;

		private string _paramValue;

		/// <summary>
		/// Nome do parametro.
		/// </summary>
		[ConfigurationAttribute("name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		[ConfigurationAttribute("value")]
		public string ParamValue
		{
			get
			{
				return _paramValue;
			}
			set
			{
				_paramValue = value;
			}
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Parameter parameter = new Parameter();
			parameter.Name = (this.Name != null) ? ((string)this.Name.Clone()) : null;
			parameter.ParamValue = (this.ParamValue != null) ? ((string)this.ParamValue.Clone()) : null;
			return parameter;
		}
	}
}
