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
	static class SecurityUtility
	{
		/// <summary>
		/// Verifica se o parametro é valido.
		/// </summary>
		/// <param name="param">Valor do parametro.</param>
		/// <param name="checkForNull">Verificar se o parametro é nulo.</param>
		/// <param name="checkIfEmpty">Verificar se o parametro é vazio.</param>
		/// <param name="checkForCommas">Verifica por virgulas no parametro.</param>
		/// <param name="maxSize">Tamanho máximo do parametro.</param>
		/// <param name="paramName">Nome do parametro.</param>
		internal static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
		{
			if(param == null)
			{
				if(checkForNull)
					throw new ArgumentNullException(paramName);
			}
			else
			{
				param = param.Trim();
				if(checkIfEmpty && (param.Length < 1))
					throw new ArgumentException("Parameter can not be empty", paramName);
				if((maxSize > 0) && (param.Length > maxSize))
					throw new ArgumentException(string.Format("Parameter '{0}' too long. MaxSize: {1}", paramName, maxSize.ToString(System.Globalization.CultureInfo.InvariantCulture)), paramName);
				if(checkForCommas && param.Contains(","))
					throw new ArgumentException("Parameter cannot contain comma", paramName);
			}
		}

		/// <summary>
		/// Verifica se o parametro é valido.
		/// </summary>
		/// <param name="param"></param>
		/// <param name="checkForNull"></param>
		/// <param name="checkIfEmpty"></param>
		/// <param name="checkForCommas"></param>
		/// <param name="maxSize"></param>
		/// <param name="paramName"></param>
		internal static void CheckArrayParameter(ref string[] param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
		{
			if(param == null)
			{
				throw new ArgumentNullException(paramName);
			}
			if(param.Length < 1)
			{
				throw new ArgumentException("Parameter array empty", paramName);
			}
			var hashtable = new System.Collections.Hashtable(param.Length);
			for(int i = param.Length - 1; i >= 0; i--)
			{
				CheckParameter(ref param[i], checkForNull, checkIfEmpty, checkForCommas, maxSize, paramName + "[ " + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ]");
				if(hashtable.Contains(param[i]))
					throw new ArgumentException("Parameter duplicate array element", paramName);
				hashtable.Add(param[i], param[i]);
			}
		}
	}
}
