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
	/// Armazena os dados de uma classe portavel.
	/// </summary>
	[Serializable]
	public class PortableClass : ICloneable
	{
		private string _assembly;

		private string _id;

		private string _name;

		private List<PortableAttribute> _portableAttributeList;

		private string _type;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			PortableClass class2 = new PortableClass();
			class2.Name = (Name != null) ? ((string)Name.Clone()) : null;
			class2.ID = (ID != null) ? ((string)ID.Clone()) : null;
			class2.Assembly = (Assembly != null) ? ((string)Assembly.Clone()) : null;
			class2.Type = (Type != null) ? ((string)Type.Clone()) : null;
			class2.PortableAttributes = (PortableAttributes != null) ? ((PortableAttribute[])PortableAttributes.Clone()) : null;
			return class2;
		}

		/// <summary>
		/// Assembly associado.
		/// </summary>
		[ConfigurationAttribute("assembly")]
		public string Assembly
		{
			get
			{
				return _assembly;
			}
			set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Identificador da classe.
		/// </summary>
		[ConfigurationAttribute("handle-id")]
		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
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
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Lista dos atributos.
		/// </summary>
		public List<PortableAttribute> PortableAttributeList
		{
			get
			{
				return _portableAttributeList;
			}
			set
			{
				_portableAttributeList = value;
			}
		}

		/// <summary>
		/// Attributos associados.
		/// </summary>
		[ConfigurationSection("attribute")]
		public PortableAttribute[] PortableAttributes
		{
			get
			{
				if(_portableAttributeList != null)
					return _portableAttributeList.ToArray();
				return null;
			}
			set
			{
				if(_portableAttributeList == null)
					_portableAttributeList = new List<PortableAttribute>();
				_portableAttributeList.Clear();
				if(value != null)
					_portableAttributeList.AddRange(value);
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
				return _type;
			}
			set
			{
				_type = value;
			}
		}
	}
}
