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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Classe com os métodos de extensão para auxiliar a utilização da interface <see cref="IDataCacheManager"/>.
	/// </summary>
	public static class DataCacheManagerExtensions
	{
		/// <summary>
		/// Registra todas as classes contidas no namespace do assembly informado.
		/// </summary>
		/// <param name="dataCacheManager"></param>
		/// <param name="assembly"></param>
		/// <param name="nameSpace"></param>
		/// <returns></returns>
		public static IDataCacheManager RegisterNamespace(this IDataCacheManager dataCacheManager, System.Reflection.Assembly assembly, string nameSpace)
		{
			dataCacheManager.Require("dataCacheManager").NotNull();
			assembly.Require("assembly").NotNull();
			nameSpace.Require("nameSpace").NotNull().NotEmpty();
			foreach (var i in assembly.GetTypes())
				if(i.Namespace.IndexOf(nameSpace) == 0)
					dataCacheManager.Register(i);
			return dataCacheManager;
		}
	}
}
