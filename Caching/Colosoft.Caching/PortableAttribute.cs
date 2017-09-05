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
	/// Armazena os dados de um atributo portável.
	/// </summary>
	[Serializable]
	public class PortableAttribute : ICloneable
	{
		private string _name;

		private string _order;

		private string _type;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			PortableAttribute attribute = new PortableAttribute();
			attribute.Name = (this.Name != null) ? ((string)this.Name.Clone()) : null;
			attribute.Type = (this.Type != null) ? ((string)this.Type.Clone()) : null;
			attribute.Order = (this.Order != null) ? ((string)this.Order.Clone()) : null;
			return attribute;
		}

		/// <summary>
		/// Nome do atributo.
		/// </summary>
		[ConfigurationAttribute("name")]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		/// <summary>
		/// Ordem do atributo.
		/// </summary>
		[ConfigurationAttribute("order")]
		public string Order
		{
			get
			{
				return this._order;
			}
			set
			{
				this._order = value;
			}
		}

		/// <summary>
		/// Tipo de atributo.
		/// </summary>
		[ConfigurationAttribute("type")]
		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}
	}
}
