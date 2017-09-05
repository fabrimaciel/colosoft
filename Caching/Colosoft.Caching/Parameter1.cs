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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Representa um parametro.
	/// </summary>
	public class Parameter : IGenerator, IComparable
	{
		private string _name;

		/// <summary>
		/// Nome do parametro.
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
		/// <param name="name">Nome do parametro.</param>
		public Parameter(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Compara com a instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is Parameter)
				return string.Compare(((Parameter)obj).Name, Name);
			return -1;
		}

		/// <summary>
		/// Calcula.
		/// </summary>
		/// <returns></returns>
		public object Evaluate()
		{
			return null;
		}

		/// <summary>
		/// Calcula o valor
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public object Evaluate(System.Collections.IDictionary values)
		{
			var list = values[Name] as System.Collections.ArrayList;
			if(list != null)
			{
				return list[0];
			}
			if(!values.Contains(Name))
				throw new ArgumentException(string.Format("Value not found to parameter '{0}'.", Name));
			return values[Name];
		}
	}
}
