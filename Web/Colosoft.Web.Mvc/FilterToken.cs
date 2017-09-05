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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Possíveis tipos de token do filtro.
	/// </summary>
	public enum FilterTokenType
	{
		/// <summary>
		/// Propriedade.
		/// </summary>
		Property,
		/// <summary>
		/// Operador de comparação.
		/// </summary>
		ComparisonOperator,
		/// <summary>
		/// Ou.
		/// </summary>
		Or,
		/// <summary>
		/// E.
		/// </summary>
		And,
		/// <summary>
		/// Negação.
		/// </summary>
		Not,
		/// <summary>
		/// Função.
		/// </summary>
		Function,
		/// <summary>
		/// Número.
		/// </summary>
		Number,
		/// <summary>
		/// Texto.
		/// </summary>
		String,
		/// <summary>
		/// Boolean.
		/// </summary>
		Boolean,
		/// <summary>
		/// DateTime.
		/// </summary>
		DateTime,
		/// <summary>
		/// Parenteses da esquerda.
		/// </summary>
		LeftParenthesis,
		/// <summary>
		/// Parenteses da direita.
		/// </summary>
		RightParenthesis,
		/// <summary>
		/// Virgula.
		/// </summary>
		Comma
	}
	/// <summary>
	/// Token do filtro.
	/// </summary>
	public class FilterToken
	{
		/// <summary>
		/// Tipo de token.
		/// </summary>
		public FilterTokenType TokenType
		{
			get;
			set;
		}

		/// <summary>
		/// Valor.
		/// </summary>
		public string Value
		{
			get;
			set;
		}
	}
}
