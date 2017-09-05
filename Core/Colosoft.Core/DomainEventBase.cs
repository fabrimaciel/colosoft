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
	/// Implementação básica de um evento de domínio.
	/// </summary>
	public abstract class DomainEventBase
	{
		private readonly List<IDomainEventSubscription> _subscriptions = new List<IDomainEventSubscription>();

		/// <summary>
		/// Recupera a coleção das inscrições feitas para o evento.
		/// </summary>
		protected virtual ICollection<IDomainEventSubscription> Subscriptions
		{
			get
			{
				return _subscriptions;
			}
		}

		/// <summary>
		/// Recupera as estratégia de execução.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<IDomainEventExecutionStrategy> PruneAndReturnStrategies()
		{
			var returnList = new List<IDomainEventExecutionStrategy>();
			lock (Subscriptions)
			{
				for(var i = Subscriptions.Count - 1; i >= 0; i--)
				{
					var listItem = _subscriptions[i].GetExecutionStrategy();
					if(listItem == null)
						_subscriptions.RemoveAt(i);
					else
						returnList.Add(listItem);
				}
			}
			return returnList;
		}

		/// <summary>
		/// Inscreve um evento para o domínio.
		/// </summary>
		/// <param name="subscription">Instancia da inscrição.</param>
		/// <returns>Token gerado para a inscrição feita.</returns>
		protected virtual SubscriptionToken InternalSubscribe(IDomainEventSubscription subscription)
		{
			subscription.Require("subscription").NotNull();
			subscription.SubscriptionToken = new SubscriptionToken();
			lock (Subscriptions)
				Subscriptions.Add(subscription);
			return subscription.SubscriptionToken;
		}

		/// <summary>
		/// Remove a inscrição associada com o token informado.
		/// </summary>
		/// <param name="token">Instancia do token da inscrição.</param>
		public virtual void Unsubscribe(SubscriptionToken token)
		{
			lock (Subscriptions)
			{
				var subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
				if(subscription != null)
					Subscriptions.Remove(subscription);
			}
		}

		/// <summary>
		/// Verifica se o token já está registrado na instancia.
		/// </summary>
		/// <param name="token">Token da inscrição que será verificado.</param>
		/// <returns><see langword="true"/> caso a instancia tenha o token informado.</returns>
		public virtual bool Contains(SubscriptionToken token)
		{
			lock (Subscriptions)
			{
				var subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);
				return subscription != null;
			}
		}

		/// <summary>
		/// Chama todas as estratégias de execução das inscrições.
		/// </summary>
		/// <param name="arguments">Argumentos que serão passados para os listeners</param>
		protected virtual void InternalPublish(params object[] arguments)
		{
			var executionStrategies = PruneAndReturnStrategies();
			foreach (var executionStrategy in executionStrategies)
			{
				if(executionStrategy.CanExecute(arguments))
					executionStrategy.Execute(arguments);
				executionStrategy.Dispose();
			}
		}
	}
}
