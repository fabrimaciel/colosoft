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

namespace Colosoft.Runtime
{
	/// <summary>
	/// Implementação padrão do IRuntimeValueStorage.
	/// </summary>
	public class RuntimeValueStorage : IRuntimeValueStorage
	{
		private static IRuntimeValueStorage _default = new RuntimeValueStorage();

		private static IRuntimeValueStorage _instance;

		private System.Collections.Hashtable _values = new System.Collections.Hashtable();

		/// <summary>
		/// Instancia padrão.
		/// </summary>
		public static IRuntimeValueStorage Default
		{
			get
			{
				return _default;
			}
		}

		/// <summary>
		/// Instancia geral.
		/// </summary>
		public static IRuntimeValueStorage Instance
		{
			get
			{
				return _instance ?? _default;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Recupera o valor associado com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetValue(string name)
		{
			lock (_values)
				return _values[name];
		}

		/// <summary>
		/// Define o valor associado com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(string name, object value)
		{
			lock (_values)
				_values[name] = value;
		}

		/// <summary>
		/// Remove o valor associado com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void RemoveValue(string name)
		{
			lock (_values)
				_values.Remove(name);
		}
	}
}
