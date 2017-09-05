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
	/// Classe responsável por gerencia o processo de lock.
	/// </summary>
	public static class LockProcessManager
	{
		private static bool _lockProcessLoaded;

		private static ILockProcess _lockProcess;

		private static bool _registerLockProcessLoaded;

		private static IRegisterLockProcess _registerLockProcess;

		/// <summary>
		/// Recupera a instancia geral do processo de lock.
		/// </summary>
		public static ILockProcess LockProcess
		{
			get
			{
				if(!_lockProcessLoaded)
				{
					ServiceLocatorValidator.Validate();
					try
					{
						_lockProcess = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ILockProcess>("ApplicationLockProcess");
					}
					catch(Exception)
					{
					}
					_lockProcessLoaded = true;
				}
				return _lockProcess;
			}
		}

		/// <summary>
		/// Instancia do registrador de processos de lock.
		/// </summary>
		public static IRegisterLockProcess RegisterLockProcess
		{
			get
			{
				if(!_registerLockProcessLoaded)
				{
					ServiceLocatorValidator.Validate();
					try
					{
						_registerLockProcess = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IRegisterLockProcess>();
					}
					catch(Exception)
					{
					}
					_registerLockProcessLoaded = true;
				}
				return _registerLockProcess;
			}
		}
	}
}
