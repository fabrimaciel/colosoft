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

namespace Colosoft.Validation
{
	/// <summary>
	/// Classe que representa os dados de um validador nativo do sistema.
	/// </summary>
	/// <typeparam name="T">Tipo do argumento que será validado.</typeparam>
	public class NativeValidator<T>
	{
		/// <summary>
		/// Inicializa uma nova instancia da classe <see cref="NativeValidator{T}"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="name"></param>
		public NativeValidator(T value, string name)
		{
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// Recupera e define o nome do argumento que será validado.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera e define o valor do argumento que será validado.
		/// </summary>
		public T Value
		{
			get;
			set;
		}
	}
}
