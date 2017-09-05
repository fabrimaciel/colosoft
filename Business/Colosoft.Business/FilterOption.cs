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

namespace Colosoft.Business
{
	/// <summary>
	/// Tipos de filtros para os items
	/// </summary>
	public enum FilterOption : int
	{
		/// <summary>
		/// Todos
		/// </summary>
		All = 1,
		/// <summary>
		/// Somente ativos
		/// </summary>
		Activated = 2,
		/// <summary>
		/// Expirados
		/// </summary>
		Expired = 4
	}
	/// <summary>
	/// Filter option helper
	/// </summary>
	public static class FilterOptionHelper
	{
		/// <summary>
		/// Recupera os filtros
		/// </summary>
		/// <returns></returns>
		public static IDictionary<Colosoft.Business.FilterOption, string> GetFilterOptions()
		{
			var dic = new Dictionary<Colosoft.Business.FilterOption, string>();
			dic.Add(Colosoft.Business.FilterOption.All, "Mostrar todos");
			dic.Add(Colosoft.Business.FilterOption.Activated, "Mostrar somente ativos");
			dic.Add(Colosoft.Business.FilterOption.Expired, "Mostrar somente expirados");
			return dic;
		}
	}
}
