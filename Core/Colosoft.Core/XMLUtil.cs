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

namespace Colosoft.Security.Util
{
	/// <summary>
	/// Classe que contém método que auxilia na utilização do XML.
	/// </summary>
	static class XMLUtil
	{
		/// <summary>
		/// Verifica se o SecurityElement não tem restrição.
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		public static bool IsUnrestricted(System.Security.SecurityElement el)
		{
			string str = el.Attribute("Unrestricted");
			if(str == null)
				return false;
			if(!str.Equals("true") && !str.Equals("TRUE"))
				return str.Equals("True");
			return true;
		}

		/// <summary>
		/// Adiciona um atributo de classe.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="type"></param>
		/// <param name="typename"></param>
		public static void AddClassAttribute(System.Security.SecurityElement element, Type type, string typename)
		{
			if(typename == null)
				typename = type.FullName;
			element.AddAttribute("class", typename + ", " + type.Module.Assembly.FullName.Replace('"', '\''));
		}

		/// <summary>
		/// Verifica se o elemento é um elemento de permissão.
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="el"></param>
		/// <returns></returns>
		public static bool IsPermissionElement(System.Security.IPermission ip, System.Security.SecurityElement el)
		{
			if(!el.Tag.Equals("Permission") && !el.Tag.Equals("IPermission"))
				return false;
			return true;
		}
	}
}
