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
	/// Armazena os dados de um classe compacta.
	/// </summary>
	[Serializable]
	public class CompactClass : ICloneable
	{
		private string _assembly;

		private string _id;

		private string _name;

		private string _type;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			CompactClass class2 = new CompactClass();
			class2.Name = (this.Name != null) ? ((string)this.Name.Clone()) : null;
			class2.ID = (this.ID != null) ? ((string)this.ID.Clone()) : null;
			class2.Type = (this.Type != null) ? ((string)this.Type.Clone()) : null;
			return class2;
		}

		/// <summary>
		/// Assembly da classe.
		/// </summary>
		[ConfigurationAttribute("assembly")]
		public string Assembly
		{
			get
			{
				return this._assembly;
			}
			set
			{
				this._assembly = value;
			}
		}

		/// <summary>
		/// Identificador da classe.
		/// </summary>
		[ConfigurationAttribute("id")]
		public string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		/// <summary>
		/// Nome da classe.
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
		/// Idenitifica se a classe é portável.
		/// </summary>
		public bool Portable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Tipo da classe.
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
