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
	/// Classe com métodos de extensão para o provedor de consulta.
	/// </summary>
	static class QueryProviderExtensions
	{
		/// <summary>
		/// Verifica se é um provedor do EntityFramework.
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public static bool IsEntityFrameworkProvider(this IQueryProvider provider)
		{
			string fullName = provider.GetType().FullName;
			if((!(fullName == "System.Data.Objects.ELinq.ObjectQueryProvider") && !(fullName == "System.Data.Entity.Core.Objects.ELinq.ObjectQueryProvider")) && !fullName.StartsWith("LinqKit.ExpandableQueryProvider"))
			{
				return fullName.StartsWith("System.Data.Entity.Internal.Linq");
			}
			return true;
		}

		/// <summary>
		/// Verifica se é um provedor de objetos.
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public static bool IsLinqToObjectsProvider(this IQueryProvider provider)
		{
			return provider.GetType().FullName.Contains("EnumerableQuery");
		}
	}
}
