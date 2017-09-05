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

namespace Colosoft.Runtime
{
	/// <summary>
	/// Classe com método que auxiliam a identificador da plataforma que está sendo
	/// executada no momento.
	/// </summary>
	public static class PlatformHelper
	{
		private static readonly Lazy<bool> IsRunningOnMonoValue = new Lazy<bool>(() =>  {
			return Type.GetType("Mono.Runtime") != null;
		});

		/// <summary>
		/// Identifica se o sistema está sendo executado sobre o Mono.
		/// </summary>
		public static bool IsRunningOnMono
		{
			get
			{
				return IsRunningOnMonoValue.Value;
			}
		}

		/// <summary>
		/// Verifica se o OS é linux.
		/// </summary>
		public static bool IsLinux
		{
			get
			{
				int p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}
	}
}
