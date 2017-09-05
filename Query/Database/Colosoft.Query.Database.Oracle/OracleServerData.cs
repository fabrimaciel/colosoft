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

namespace Colosoft.Query.Database.Oracle
{
	/// <summary>
	/// Implementação do ServerData para Oracle.
	/// </summary>
	public class OracleServerData : IServerData
	{
		private TimeSpan _offset = TimeSpan.Zero;

		private DateTime _lastUpdate;

		private bool _isValid = false;

		/// <summary>
		/// Verifica se a data é válida.
		/// </summary>
		private bool IsValid
		{
			get
			{
				if(!_isValid || _lastUpdate.AddMinutes(3) < DateTime.Now)
					return false;
				return true;
			}
		}

		/// <summary>
		/// Recupera a data atual.
		/// </summary>
		public DateTimeOffset Current
		{
			get
			{
				if(!IsValid)
				{
					try
					{
						using (var session = new GDA.GDASession())
						{
							var date = new GDA.DataAccess().ExecuteScalar(session, "SELECT SYSDATE FROM dual");
							if(date is DateTime)
							{
								var date2 = (DateTime)date;
								var offset = date2 - DateTime.Now;
								_offset = offset;
								_lastUpdate = DateTime.Now;
							}
						}
						_isValid = true;
					}
					catch
					{
					}
				}
				return DateTime.Now.Add(_offset);
			}
		}

		/// <summary>
		/// Recupera a data atual.
		/// </summary>
		/// <returns></returns>
		public DateTimeOffset GateDateTimeOffSet()
		{
			return Current;
		}

		/// <summary>
		/// Recupera a data atual.
		/// </summary>
		/// <returns></returns>
		public DateTime GetDateTime()
		{
			return Current.DateTime;
		}
	}
}
