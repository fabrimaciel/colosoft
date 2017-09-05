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
	class CachingUtils
	{
		/// <summary>
		/// Diretório de deploy dos assemblies.
		/// </summary>
		public static readonly string DeployedAssemblyDir;

		/// <summary>
		/// Data de inicio do cache.
		/// </summary>
		private static DateTime START_DT;

		private static string _installDir = "";

		/// <summary>
		/// Diretório de instalação.
		/// </summary>
		public static string InstallDir
		{
			get
			{
				return _installDir;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		static CachingUtils()
		{
			DeployedAssemblyDir = @"deploy\";
			START_DT = new DateTime(0x7d4, 12, 0x1f, 0, 0, 0, 0, DateTimeKind.Utc);
		}

		/// <summary>
		/// Recupera a diferença em segundos.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static int DiffSeconds(DateTime dt)
		{
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
			TimeSpan span = (TimeSpan)(dt - START_DT);
			return (int)span.TotalSeconds;
		}

		/// <summary>
		/// Recupera a diference em milisegundos.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static int DiffMilliseconds(DateTime dt)
		{
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
			TimeSpan span = (TimeSpan)(dt - START_DT);
			return span.Milliseconds;
		}

		/// <summary>
		/// Recupera a data absoluta.
		/// </summary>
		/// <param name="absoluteTime"></param>
		/// <returns></returns>
		public static DateTime GetDateTime(int absoluteTime)
		{
			DateTime time = new DateTime(START_DT.Ticks, DateTimeKind.Utc);
			return time.AddSeconds((double)absoluteTime);
		}

		/// <summary>
		/// Recupera a diference em ticks.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static long DiffTicks(DateTime dt)
		{
			dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
			TimeSpan span = (TimeSpan)(dt - START_DT);
			return span.Ticks;
		}
	}
}
