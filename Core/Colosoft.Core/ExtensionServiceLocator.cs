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

namespace Colosoft.Extensions
{
	/// <summary>
	/// Localizador dos serviços de extensão.
	/// </summary>
	public class ExtensionServiceLocator
	{
		private Microsoft.Practices.ServiceLocation.IServiceLocator _serviceLocator;

		private static ExtensionServiceLocator _current;

		private static object _objLock = new object();

		/// <summary>
		/// Instancia do localizador de serviço.
		/// </summary>
		public Microsoft.Practices.ServiceLocation.IServiceLocator ServiceLocator
		{
			get
			{
				if(_serviceLocator == null)
					_serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
				return _serviceLocator;
			}
			set
			{
				_serviceLocator = value;
			}
		}

		/// <summary>
		/// Instancia corrente.
		/// </summary>
		public static ExtensionServiceLocator Current
		{
			get
			{
				if(_current == null)
					lock (_objLock)
						if(_current == null)
							_current = new ExtensionServiceLocator();
				return _current;
			}
			set
			{
				_current = value;
			}
		}

		/// <summary>
		/// Recupera a instancia da extensão.
		/// </summary>
		/// <typeparam name="TExtension"></typeparam>
		/// <param name="exportId">Identificador do exporta da extensão.</param>
		/// <returns></returns>
		public TExtension GetInstance<TExtension>(int exportId)
		{
			return ServiceLocator.GetInstance<TExtension>(typeof(TExtension).FullName + "." + exportId);
		}
	}
}
