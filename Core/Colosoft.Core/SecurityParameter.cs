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

namespace Colosoft.Security
{
	/// <summary>
	/// Assinatura do provedor de parametros de segurança.
	/// </summary>
	public interface ISecurityParameterProvider
	{
		/// <summary>
		/// Recupera os parametros associados.
		/// </summary>
		/// <returns></returns>
		IEnumerable<SecurityParameter> GetParameters();
	}
	/// <summary>
	/// Armazena os dados de um parametro usado na autenticação.
	/// </summary>
	public class SecurityParameter
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		public string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public SecurityParameter()
		{
		}

		/// <summary>
		/// Cria uma nova instancia já com os valores do parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public SecurityParameter(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
