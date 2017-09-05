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

namespace Colosoft.Reports
{
	/// <summary>
	/// Implementação da definição de um subrelatório.
	/// </summary>
	public class SubreportDefinition : ISubreportDefinition
	{
		private string _name;

		private Func<System.IO.Stream> _definitionGetter;

		/// <summary>
		/// Nome do subrelatório.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="definitionGetter"></param>
		public SubreportDefinition(string name, Func<System.IO.Stream> definitionGetter)
		{
			_name = name;
			_definitionGetter = definitionGetter;
		}

		/// <summary>
		/// Recupera a definição do subreport.
		/// </summary>
		/// <returns></returns>
		public System.IO.Stream GetDefinition()
		{
			return _definitionGetter();
		}
	}
}
