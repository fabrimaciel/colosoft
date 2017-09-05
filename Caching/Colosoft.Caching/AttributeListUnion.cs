﻿/* 
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
	/// Armazena os dados da união de uma lista de atributos.
	/// </summary>
	[Serializable]
	public class AttributeListUnion : ICloneable
	{
		private List<PortableAttribute> _portableAttributeList;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			AttributeListUnion union = new AttributeListUnion();
			union.PortableAttributes = (PortableAttributes != null) ? ((PortableAttribute[])PortableAttributes.Clone()) : null;
			return union;
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
	}
}
