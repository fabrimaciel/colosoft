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

namespace Colosoft.Lock
{
	/// <summary>
	/// Classe para o tratamento do lock.
	/// </summary>
	public class Lock
	{
		private static ILockProcess _lockProcess;

		private static object _lockObject = new object();

		/// <summary>
		/// Instância da implementação padrão de ILockProcess
		/// </summary>
		public static ILockProcess Instance
		{
			get
			{
				if(_lockProcess == null)
				{
					lock (_lockObject)
					{
						if(_lockProcess == null)
						{
							_lockProcess = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ILockProcess>();
						}
					}
				}
				return _lockProcess;
			}
		}
	}
}
