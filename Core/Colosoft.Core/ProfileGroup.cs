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

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Grupo de propriedades
	/// </summary>
	public class ProfileGroup
	{
		private string _name;

		private IProfile _parent;

		/// <summary>
		/// Recupera e define o valor da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object this[string propertyName]
		{
			get
			{
				return _parent[_name + propertyName];
			}
			set
			{
				_parent[_name + propertyName] = value;
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="parent">Perfil pai do grupo.</param>
		/// <param name="name"></param>
		public void Init(IProfile parent, string name)
		{
			if(_parent == null)
			{
				_parent = parent;
				_name = name + ".";
			}
		}

		/// <summary>
		/// Define o valor da propriedade do grupo.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		public void SetPropertyValue(string propertyName, object propertyValue)
		{
			_parent[_name + propertyName] = propertyValue;
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValue"></param>
		public object GetPropertyValue(string propertyName, object propertyValue)
		{
			return _parent[_name + propertyName];
		}
	}
}
