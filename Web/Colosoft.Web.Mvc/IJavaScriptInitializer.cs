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

namespace Colosoft.Web.Mvc.Infrastructure
{
	/// <summary>
	/// Assinatura de um inicializador javascript.
	/// </summary>
	public interface IJavaScriptInitializer
	{
		/// <summary>
		/// Cria uma instancia do serializador.
		/// </summary>
		/// <returns></returns>
		IJavaScriptSerializer CreateSerializer();

		/// <summary>
		/// Inicializa.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		string Initialize(string id, string name, IDictionary<string, object> options);

		/// <summary>
		/// Inicializa para.
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="name"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		string InitializeFor(string selector, string name, IDictionary<string, object> options);

		/// <summary>
		/// Serializa os valores informados.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		string Serialize(IDictionary<string, object> value);
	}
}
