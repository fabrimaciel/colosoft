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
	/// Representa a classe que gerencia a publicação e inscrição dos eventos do dominio.
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	public class CompositeDomainEvent<TPayload> : DomainEventBase
	{
		private Threading.IDispatcher _dispatcher;

		/// <summary>
		/// Instancia do despachante associado.
		/// </summary>
		public virtual Threading.IDispatcher Dispatcher
		{
			get
			{
				if(_dispatcher == null)
				{
					try
					{
						_dispatcher = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Threading.IDispatcher>();
					}
					catch
					{
					}
				}
				return _dispatcher;
			}
		}

		/// <summary>
		/// Registra o delegate para um evento que será publicado usando a opção <see cref="DomainEventThreadOption.PublisherThread"/>.
		/// </summary>
		/// <param name="action">Instancia da ação que será registrada.</param>
		/// <returns>Token da inscrição.</returns>
		public SubscriptionToken Subscribe(Action<TPayload> action)
		{
			return Subscribe(action, DomainEventThreadOption.PublisherThread);
		}

		/// <summary>
		/// Registra o delegate para o evento.
		/// </summary>
		/// <param name="action">Instancia da ação que será registrada.</param>
		/// <param name="threadOption">Opção de thread para o acionamento da ação.</param>
		/// <returns>Token da inscrição.</returns>
		public SubscriptionToken Subscribe(Action<TPayload> action, DomainEventThreadOption threadOption)
		{
			return Subscribe(action, threadOption, false);
		}

		/// <summary>
		/// Registra o delegate para um evento que será publicado usando a opção <see cref="DomainEventThreadOption.PublisherThread"/>.
		/// </summary>
		/// <param name="action">Instancia da ação que será registrada.</param>
		/// <param name="keepSubscriberReferenceAlive">Quando <see langword="true"/>, a instancia
		/// mantem uma regerencia da inscrição que não será capturada pelo garbage collected.</param>
		/// <returns>Token da inscrição.</returns>
		public SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive)
		{
			return Subscribe(action, DomainEventThreadOption.PublisherThread, keepSubscriberReferenceAlive);
		}

		/// <summary>
		/// Registra o delegate para o evento.
		/// </summary>
		/// <param name="action">Instancia da ação que será registrada.</param>
		/// <param name="threadOption">Opção de thread para o acionamento da ação.</param>
		/// <param name="keepSubscriberReferenceAlive">Quando <see langword="true"/>, a instancia
		/// mantem uma regerencia da inscrição que não será capturada pelo garbage collected.</param>
		/// <returns>Token da inscrição.</returns>
		public SubscriptionToken Subscribe(Action<TPayload> action, DomainEventThreadOption threadOption, bool keepSubscriberReferenceAlive)
		{
			return Subscribe(action, threadOption, keepSubscriberReferenceAlive, null);
		}

		/// <summary>
		/// Registra o delegate de um evento.
		/// </summary>
		/// <param name="action">Instancia da ação que será registrada.</param>
		/// <param name="threadOption">Opção de thread para o acionamento da ação.</param>
		/// <param name="keepSubscriberReferenceAlive">Quando <see langword="true"/>, a instancia
		/// mantem uma regerencia da inscrição que não será capturada pelo garbage collected.</param>
		/// <param name="filter">Predicado do filtro para acionar o evento registrado.</param>
		/// <param name="allowCallReentrance"><see langword="true"/> identifica que permite rechamada da ação.</param>
		/// <param name="name">Nome da assinatura.</param>
		/// <returns>Token da inscrição.</returns>
		public virtual SubscriptionToken Subscribe(Action<TPayload> action, DomainEventThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter, bool allowCallReentrance = false, string name = null)
		{
			IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
			IDelegateReference filterReference;
			if(filter != null)
				filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);
			else
				filterReference = new DelegateReference(new Predicate<TPayload>(delegate {
					return true;
				}), true);
			DomainEventSubscription<TPayload> subscription;
			switch(threadOption)
			{
			case DomainEventThreadOption.PublisherThread:
				subscription = new DomainEventSubscription<TPayload>(actionReference, filterReference, allowCallReentrance, name);
				break;
			case DomainEventThreadOption.BackgroundThread:
				subscription = new BackgroundEventSubscription<TPayload>(actionReference, filterReference, allowCallReentrance, name);
				break;
			case DomainEventThreadOption.DispatcherThread:
				subscription = new DispatcherEventSubscription<TPayload>(actionReference, filterReference, allowCallReentrance, Dispatcher, name);
				break;
			default:
				subscription = new DomainEventSubscription<TPayload>(actionReference, filterReference, allowCallReentrance, name);
				break;
			}
			return base.InternalSubscribe(subscription);
		}

		/// <summary>
		/// Remove a primeira inscrição que for igual ao <seealso cref="Action{TPayload}"/> que está na lista de inscrições.
		/// </summary>
		/// <param name="subscriber">Instancia da <see cref="Action{TPayload}"/> usada quando o evento foi registrado.</param>
		public virtual void Unsubscribe(Action<TPayload> subscriber)
		{
			lock (Subscriptions)
			{
				IDomainEventSubscription eventSubscription = Subscriptions.Cast<DomainEventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
				if(eventSubscription != null)
					Subscriptions.Remove(eventSubscription);
			}
		}

		/// <summary>
		/// Verifica se existe alguma inscrição igual a instancia informada..
		/// </summary>
		/// <param name="subscriber"></param>
		/// <returns></returns>
		public virtual bool Contains(Action<TPayload> subscriber)
		{
			IDomainEventSubscription eventSubscription;
			lock (Subscriptions)
			{
				eventSubscription = Subscriptions.Cast<DomainEventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
			}
			return eventSubscription != null;
		}

		/// <summary>
		/// Publica o evento com o argumento informado.
		/// </summary>
		/// <param name="payload">Message que será enviada para as inscrições.</param>
		public virtual void Publish(TPayload payload)
		{
			base.InternalPublish(payload);
		}
	}
}
