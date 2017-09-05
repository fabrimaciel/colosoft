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
	/// Armazena os dados de um tipo.
	/// </summary>
	[Serializable]
	public class Type : ICloneable
	{
		private AttributeListUnion _attrbiuteList;

		private string _id;

		private string _name;

		private bool _portable;

		private List<PortableClass> _portableClassList;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Type type = new Type();
			type.ID = (ID != null) ? ((string)ID.Clone()) : null;
			type.Name = (Name != null) ? ((string)Name.Clone()) : null;
			type.Portable = Portable;
			type.PortableClasses = (PortableClasses != null) ? ((PortableClass[])PortableClasses.Clone()) : null;
			type.AttributeList = (AttributeList != null) ? ((AttributeListUnion)AttributeList.Clone()) : null;
			return type;
		}

		/// <summary>
		/// Lista dos atributos associados.
		/// </summary>
		[ConfigurationSection("attribute-list")]
		public AttributeListUnion AttributeList
		{
			get
			{
				return _attrbiuteList;
			}
			set
			{
				_attrbiuteList = value;
			}
		}

		/// <summary>
		/// Identificador do tipo.
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
		/// Nome do tipo.
		/// </summary>
		[ConfigurationAttribute("handle")]
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
		/// Identifica se o tipo é portavel.
		/// </summary>
		[ConfigurationAttribute("portable")]
		public bool Portable
		{
			get
			{
				return _portable;
			}
			set
			{
				_portable = value;
			}
		}

		/// <summary>
		/// Classes portáveis associadas.
		/// </summary>
		[ConfigurationSection("class")]
		public PortableClass[] PortableClasses
		{
			get
			{
				if(_portableClassList != null)
					return _portableClassList.ToArray();
				return null;
			}
			set
			{
				if(_portableClassList == null)
					_portableClassList = new List<PortableClass>();
				_portableClassList.Clear();
				if(value != null)
					_portableClassList.AddRange(value);
			}
		}

		/// <summary>
		/// Lista das classes portáveis associadas.
		/// </summary>
		public List<PortableClass> PortableClassList
		{
			get
			{
				return _portableClassList;
			}
			set
			{
				_portableClassList = value;
			}
		}
	}
}
