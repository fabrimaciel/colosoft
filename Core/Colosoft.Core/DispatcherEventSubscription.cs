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

namespace Colosoft.Domain
{
	/// <summary>
	/// Customnização da classe <see cref="DomainEventSubscription{TPayload}"/> para chamada pelo <see cref="Threading.IDispatcher"/>.
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	public class DispatcherEventSubscription<TPayload> : DomainEventSubscription<TPayload>
	{
		private readonly Threading.IDispatcher _dispatcher;

		///<summary>
		/// Constructor padrão.
		///</summary>
		///<param name="actionReference">Referência para um delegate do tipo <see cref="System.Action{TPayload}"/>.</param>
		///<param name="filterReference">Referência para um delegate do tipo <see cref="Predicate{TPayload}"/>.</param>
		///<param name="allowCallReentrance">Identifica se a instancia irá permitir reentrada de chamada.</param>
		///<param name="dispatcher">Despachante que será usado na instancia.</param>
		///<param name="name">Nome da assinatura.</param>
		public DispatcherEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, bool allowCallReentrance, Threading.IDispatcher dispatcher, string name = null) : base(actionReference, filterReference, allowCallReentrance, name)
		{
			dispatcher.Require("dispatcher").NotNull();
			_dispatcher = dispatcher;
		}

		/// <summary>
		/// Realiza a chama da ação da inscrição.
		/// </summary>
		/// <param name="action">Ação que será executada.</param>
		/// <param name="argument">Argumento que será repassado para a ação.</param>
		public override void InvokeAction(Action<TPayload> action, TPayload argument)
		{
			_dispatcher.Invoke(action, argument);
		}

		/// <summary>
		/// Realiza a chama do filtro da inscrição.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="argument"></param>
		/// <returns></returns>
		public override bool InvokeFilter(Predicate<TPayload> filter, TPayload argument)
		{
			return (bool)_dispatcher.Invoke(filter, argument);
		}
	}
}
