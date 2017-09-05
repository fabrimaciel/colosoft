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

namespace Colosoft.Web.Mvc.Extensions
{
	/// <summary>
	/// Classe com métodos de extensão para o token do filtro.
	/// </summary>
	public static class FilterTokenExtensions
	{
		private static readonly IDictionary<FilterOperator, string> _operatorToToken;

		private static readonly IDictionary<string, FilterOperator> _tokenToOperator;

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static FilterTokenExtensions()
		{
			Dictionary<string, FilterOperator> dictionary = new Dictionary<string, FilterOperator>();
			dictionary.Add("eq", FilterOperator.IsEqualTo);
			dictionary.Add("neq", FilterOperator.IsNotEqualTo);
			dictionary.Add("lt", FilterOperator.IsLessThan);
			dictionary.Add("lte", FilterOperator.IsLessThanOrEqualTo);
			dictionary.Add("gt", FilterOperator.IsGreaterThan);
			dictionary.Add("gte", FilterOperator.IsGreaterThanOrEqualTo);
			dictionary.Add("startswith", FilterOperator.StartsWith);
			dictionary.Add("contains", FilterOperator.Contains);
			dictionary.Add("notsubstringof", FilterOperator.DoesNotContain);
			dictionary.Add("endswith", FilterOperator.EndsWith);
			dictionary.Add("doesnotcontain", FilterOperator.DoesNotContain);
			_tokenToOperator = dictionary;
			Dictionary<FilterOperator, string> dictionary2 = new Dictionary<FilterOperator, string>();
			dictionary2.Add(FilterOperator.IsEqualTo, "eq");
			dictionary2.Add(FilterOperator.IsNotEqualTo, "neq");
			dictionary2.Add(FilterOperator.IsLessThan, "lt");
			dictionary2.Add(FilterOperator.IsLessThanOrEqualTo, "lte");
			dictionary2.Add(FilterOperator.IsGreaterThan, "gt");
			dictionary2.Add(FilterOperator.IsGreaterThanOrEqualTo, "gte");
			dictionary2.Add(FilterOperator.StartsWith, "startswith");
			dictionary2.Add(FilterOperator.Contains, "contains");
			dictionary2.Add(FilterOperator.DoesNotContain, "notsubstringof");
			dictionary2.Add(FilterOperator.EndsWith, "endswith");
			_operatorToToken = dictionary2;
		}

		/// <summary>
		/// Converte o token par aum operador.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static FilterOperator ToFilterOperator(this Infrastructure.Implementation.FilterToken token)
		{
			return _tokenToOperator[token.Value];
		}

		/// <summary>
		/// Converte o operador para um token.
		/// </summary>
		/// <param name="filterOperator"></param>
		/// <returns></returns>
		public static string ToToken(this FilterOperator filterOperator)
		{
			return _operatorToToken[filterOperator];
		}
	}
}
