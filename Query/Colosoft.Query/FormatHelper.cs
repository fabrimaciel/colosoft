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

namespace Colosoft.Query
{
	internal static class FormatHelper
	{
		/// <summary>
		/// Formata os nomes das colunas em sequencia.
		/// </summary>
		/// <param name="members"></param>
		/// <returns></returns>
		internal static string FormatColumnNamesInSequence(IEnumerable<System.Reflection.MemberInfo> members)
		{
			if(!members.Any())
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			bool isFirst = true;
			foreach (var m in members)
			{
				if(!isFirst)
					sb.Append(", ");
				else
					isFirst = false;
				sb.Append(m.Name);
			}
			return sb.ToString();
		}
	}
}
