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
using System.Collections;

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Comparador para o resultado das consultas.
	/// </summary>
	internal class QueryResultComparer : IComparer
	{
		private bool _ascending;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="ascending">Ordenação da comparação.</param>
		public QueryResultComparer(bool ascending)
		{
			_ascending = ascending;
		}

		/// <summary>
		/// Compara as instancias.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			int num2 = (int)x;
			int num3 = (int)y;
			if(_ascending)
				return num2 > num3 ? 1 : -1;
			return num2 < num3 ? 1 : -1;
		}
	}
}
