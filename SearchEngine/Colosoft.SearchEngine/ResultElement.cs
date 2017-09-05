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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Classe que retorna um elemento para ser trafegado via serviço
	/// </summary>
	public class ResultElement
	{
		private string[] _fields;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="values">Lista de valores</param>
		public ResultElement(IList<object> values)
		{
			_fields = new string[values.Count];
			for(int index = 0; index < values.Count; index++)
			{
				if(values[index] != null)
					_fields[index] = values[index].ToString();
				else
					_fields[index] = null;
			}
		}

		/// <summary>
		/// Campos do elemento convertidos para inteiro
		/// </summary>
		public string[] Fields
		{
			get
			{
				return _fields;
			}
		}

		/// <summary>
		/// Conversão implícita de Element em ResultElement
		/// </summary>
		/// <param name="el">elemento</param>
		/// <returns>ResultElement</returns>
		public static implicit operator ResultElement(Element el)
		{
			return new ResultElement(el.Values);
		}
	}
}
