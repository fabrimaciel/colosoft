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

namespace Colosoft
{
	/// <summary>
	/// Recupera dados do servidor
	/// </summary>
	public static class ServerData
	{
		private static bool _uniqueInstanceLoaded;

		private static IServerData _uniqueInstance;

		static IServerData ServerDataInstance
		{
			get
			{
				if(!_uniqueInstanceLoaded)
				{
					_uniqueInstance = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IServerData>();
					_uniqueInstanceLoaded = true;
				}
				return _uniqueInstance;
			}
		}

		/// <summary>
		/// Recupera data e hora do servidor.
		/// </summary>
		/// <returns></returns>
		public static DateTime GetDateTime()
		{
			var instance = ServerDataInstance;
			if(instance != null)
				return instance.GetDateTime();
			return DateTime.Now;
		}

		/// <summary>
		/// Recupera data, hora e offset
		/// </summary>
		/// <returns></returns>
		public static DateTimeOffset GetDateTimeOffSet()
		{
			var instance = ServerDataInstance;
			if(instance != null)
				return instance.GateDateTimeOffSet();
			return DateTimeOffset.Now;
		}

		/// <summary>
		/// Recupera data
		/// </summary>
		/// <returns></returns>
		public static DateTime GetDate()
		{
			return GetDateTime().Date;
		}
	}
}
