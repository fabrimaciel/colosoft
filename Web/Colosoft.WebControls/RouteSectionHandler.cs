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
using System.Text;
using Colosoft.Configuration.Handlers;
using System.Configuration;
using System.Xml;

namespace Colosoft.WebControls.Route
{
	public class RouteSectionHandler : BaseSectionHandler, IConfigurationSectionHandler
	{
		/// <summary>
		/// Construtor.
		/// </summary>
		public RouteSectionHandler() : base(null)
		{
		}

		/// <summary>
		/// Método chamado pelo .NET passando as informações sobre a sessão
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			root = section;
			return this;
		}
	}
}
