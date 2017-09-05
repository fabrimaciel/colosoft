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
	/// Armazena os dados de configuração de uma classe.
	/// </summary>
	[Serializable]
	public class Class : ICloneable
	{
		private Dictionary<string, Attrib> _attributesTable = new Dictionary<string, Attrib>();

		private string _id;

		private string _name;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Class class2 = new Class();
			class2.ID = (ID != null) ? ((string)ID.Clone()) : null;
			class2.Name = (Name != null) ? ((string)Name.Clone()) : null;
			class2.Attributes = (Attributes != null) ? ((Attrib[])Attributes.Clone()) : null;
			return class2;
		}

		/// <summary>
		/// Atributos da classe.
		/// </summary>
		[ConfigurationSection("attrib")]
		public Attrib[] Attributes
		{
			get
			{
				Attrib[] array = new Attrib[_attributesTable.Count];
				_attributesTable.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				_attributesTable.Clear();
				foreach (Attrib attrib in value)
				{
					_attributesTable.Add(attrib.Name, attrib);
				}
			}
		}

		/// <summary>
		/// Hash dos atributos da classe.
		/// </summary>
		public Dictionary<string, Attrib> AttributesTable
		{
			get
			{
				return _attributesTable;
			}
			set
			{
				_attributesTable = value;
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
	}
}
