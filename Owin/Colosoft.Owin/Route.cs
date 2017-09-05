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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Representa uma rota.
	/// </summary>
	class Route
	{
		/// <summary>
		/// Tipo do controlador.
		/// </summary>
		public Type Controller
		{
			get;
			private set;
		}

		/// <summary>
		/// Nome da área da rota.
		/// </summary>
		public string Area
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da rota.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="controller"></param>
		public Route(string area, string name, Type controller)
		{
			Area = area;
			Name = name;
			Controller = controller;
		}
	}
}
