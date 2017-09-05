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
	/// Implementação do armazenamento de valores em sessões http.
	/// </summary>
	public class HttpSessionValueStorage : Colosoft.Runtime.IRuntimeValueStorage
	{
		private Colosoft.Runtime.RuntimeValueStorage _innerStorage = new Runtime.RuntimeValueStorage();

		/// <summary>
		/// Recupera o valor com base no nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetValue(string name)
		{
			if(HttpContext.Current == null || HttpContext.Current.Session == null)
				return _innerStorage.GetValue(name);
			return HttpContext.Current.Session[name];
		}

		/// <summary>
		/// Remove o valor associado com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void RemoveValue(string name)
		{
			if(HttpContext.Current == null || HttpContext.Current.Session == null)
			{
				_innerStorage.RemoveValue(name);
				return;
			}
			HttpContext.Current.Session.Remove(name);
		}

		/// <summary>
		/// Associa o valor com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(string name, object value)
		{
			if(HttpContext.Current == null || HttpContext.Current.Session == null)
			{
				_innerStorage.SetValue(name, value);
				return;
			}
			HttpContext.Current.Session[name] = value;
		}
	}
}
