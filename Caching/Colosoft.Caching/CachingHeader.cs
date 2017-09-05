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

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa os dados do cabeçalho do cache.
	/// </summary>
	public class CachingHeader
	{
		/// <summary>
		/// Identificador da versão do cabeçalho.
		/// </summary>
		private static byte[] VersionId = new byte[] {
			5,
			8,
			2,
			1,
			0
		};

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static CachingHeader()
		{
			VersionId[4] = (byte)(((VersionId[0] | VersionId[1]) | VersionId[2]) | VersionId[3]);
		}

		/// <summary>
		/// Compara a versão.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static bool CompareTo(byte[] v)
		{
			if((v == null) || (v.Length < VersionId.Length))
				return false;
			return ((((VersionId[0] == v[0]) && (VersionId[1] == v[1])) && ((VersionId[2] == v[2]) && (VersionId[3] == v[3]))) && (VersionId[4] == v[4]));
		}

		/// <summary>
		/// Tamanho do cabeçalho.
		/// </summary>
		public static int Length
		{
			get
			{
				return 5;
			}
		}

		/// <summary>
		/// Versão do cabeçalho.
		/// </summary>
		public static byte[] Version
		{
			get
			{
				return VersionId;
			}
		}
	}
}
