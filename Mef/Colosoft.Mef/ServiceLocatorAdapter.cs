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
using System.ComponentModel.Composition.Hosting;

namespace Colosoft.Mef
{
	/// <summary>
	/// Adaptação do localizador do serviço.
	/// </summary>
	public class ServiceLocatorAdapter : Microsoft.Practices.ServiceLocation.ServiceLocatorImplBase
	{
		private readonly CompositionContainer _compositionContainer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="compositionContainer"></param>
		public ServiceLocatorAdapter(CompositionContainer compositionContainer)
		{
			_compositionContainer = compositionContainer;
		}

		/// <summary>
		/// Recupera todos as instancia do tipo.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			List<object> list = new List<object>();
			IEnumerable<Lazy<object, object>> enumerable = this._compositionContainer.GetExports(serviceType, null, null);
			if(enumerable != null)
			{
				list.AddRange(Enumerable.Select<Lazy<object, object>, object>(enumerable, delegate(Lazy<object, object> export) {
					return export.Value;
				}));
			}
			return list;
		}

		/// <summary>
		/// Recupera a instancia.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		protected override object DoGetInstance(Type serviceType, string key)
		{
			IEnumerable<Lazy<object, object>> source = this._compositionContainer.GetExports(serviceType, null, key);
			if((source == null) || (source.Count<Lazy<object, object>>() <= 0))
			{
				throw new Microsoft.Practices.ServiceLocation.ActivationException(this.FormatActivationExceptionMessage(new System.ComponentModel.Composition.CompositionException("Export not found"), serviceType, key));
			}
			return source.Single<Lazy<object, object>>().Value;
		}
	}
}
