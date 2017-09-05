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
	/// Implementação básico da inscrição de eventos.
	/// </summary>
	/// <typeparam name="TPayload">Tipo generico usado pelos tipos <see cref="System.Action{TPayload}"/> e <see cref="Predicate{TPayload}"/>.</typeparam>
	public class DomainEventSubscription<TPayload> : IDomainEventSubscription
	{
		/// <summary>
		/// Nome da assinatura.
		/// </summary>
		private string _name;

		/// <summary>
		/// Referência para o delegate da ação.
		/// </summary>
		private readonly IDelegateReference _actionReference;

		/// <summary>
		/// Referência para o delegate do filtro.
		/// </summary>
		private readonly IDelegateReference _filterReference;

		/// <summary>
		/// Identifica se permite reentrada de chamada.
		/// </summary>
		private readonly bool _allowCallReentrance;

		/// <summary>
		/// Dicionário com as estratégias com base na thread.
		/// </summary>
		private static readonly Dictionary<int, Dictionary<string, DomainEventExecutionStrategy>> _threadStrategies = new Dictionary<int, Dictionary<string, DomainEventExecutionStrategy>>();

		/// <summary>
		/// Recupera o delegate da ação.
		/// </summary>
		public Action<TPayload> Action
		{
			get
			{
				return (Action<TPayload>)_actionReference.Target;
			}
		}

		/// <summary>
		/// Recupera o delegate do filtro.
		/// </summary>
		public Predicate<TPayload> Filter
		{
			get
			{
				return (Predicate<TPayload>)_filterReference.Target;
			}
		}

		/// <summary>
		/// Token da inscrição.
		/// </summary>
		public SubscriptionToken SubscriptionToken
		{
			get;
			set;
		}

		///<summary>
		/// Constructor padrão.
		///</summary>
		///<param name="actionReference">Referência para um delegate do tipo <see cref="System.Action{TPayload}"/>.</param>
		///<param name="filterReference">Referência para um delegate do tipo <see cref="Predicate{TPayload}"/>.</param>
		///<param name="allowCallReentrance">Identifica se a instancia irá permitir reentrada de chamada.</param>
		///<param name="name">Nome da assinatura.</param>
		public DomainEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, bool allowCallReentrance, string name = null)
		{
			actionReference.Require("actionReference").NotNull();
			_name = name;
			if(!(actionReference.Target is Action<TPayload>))
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_InvalidDelegateRerefenceType, typeof(Action<TPayload>).FullName).Format(), "actionReference");
			filterReference.Require("filterReference").NotNull();
			if(!(filterReference.Target is Predicate<TPayload>))
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_InvalidDelegateRerefenceType, typeof(Predicate<TPayload>).FullName).Format(), "filterReference");
			_actionReference = actionReference;
			_filterReference = filterReference;
			_allowCallReentrance = allowCallReentrance;
		}

		/// <summary>
		/// Recupera a ação da execução.
		/// </summary>
		/// <returns></returns>
		protected virtual Action<object[]> GetExecuteAction()
		{
			Action<TPayload> action = this.Action;
			if(action != null)
				return arguments =>  {
					TPayload argument = default(TPayload);
					if(arguments != null && arguments.Length > 0 && arguments[0] != null)
						argument = (TPayload)arguments[0];
					InvokeAction(action, argument);
				};
			return null;
		}

		/// <summary>
		/// Recupera o predicado do filtro.
		/// </summary>
		/// <returns></returns>
		protected virtual Predicate<object[]> GetFilterPredicate()
		{
			var filter = this.Filter;
			if(filter != null)
				return arguments =>  {
					TPayload argument = default(TPayload);
					if(arguments != null && arguments.Length > 0 && arguments[0] != null)
						argument = (TPayload)arguments[0];
					return InvokeFilter(filter, argument);
				};
			return null;
		}

		/// <summary>
		/// Recupera a estratégia de execução para publicar o evento.
		/// </summary>
		/// <returns>Instancia da estratégia.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public virtual IDomainEventExecutionStrategy GetExecutionStrategy()
		{
			if(!_allowCallReentrance)
			{
				var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
				DomainEventExecutionStrategy result = null;
				lock (_threadStrategies)
				{
					Dictionary<string, DomainEventExecutionStrategy> items = null;
					if(_threadStrategies.TryGetValue(threadId, out items))
						items.TryGetValue(_name ?? typeof(TPayload).FullName, out result);
					else
					{
						items = new Dictionary<string, DomainEventExecutionStrategy>();
						_threadStrategies.Add(threadId, items);
					}
					if(result == null)
					{
						result = new DomainEventExecutionStrategy(_name, typeof(TPayload), GetExecuteAction(), GetFilterPredicate());
						result.Disposed += new EventHandler(StrategyDisposed);
						items.Add(_name ?? result.DomainEventType.FullName, result);
					}
				}
				return result;
			}
			else
				return new DomainEventExecutionStrategy(_name, typeof(TPayload), GetExecuteAction(), GetFilterPredicate());
		}

		/// <summary>
		/// Método acionado quando algum estratégia da inscrição 
		/// ter sua instancia liberada.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StrategyDisposed(object sender, EventArgs e)
		{
			var strategy = (DomainEventExecutionStrategy)sender;
			strategy.Disposed -= StrategyDisposed;
			var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			lock (_threadStrategies)
			{
				Dictionary<string, DomainEventExecutionStrategy> items = null;
				if(_threadStrategies.TryGetValue(threadId, out items))
				{
					if(items.Remove(_name ?? strategy.DomainEventType.FullName) && _threadStrategies.Count == 0)
						_threadStrategies.Remove(threadId);
				}
			}
		}

		/// <summary>
		/// Realiza a chama da ação da inscrição.
		/// </summary>
		/// <param name="action">Ação que será executada.</param>
		/// <param name="argument">Argumento que será repassado para a ação.</param>
		public virtual void InvokeAction(Action<TPayload> action, TPayload argument)
		{
			action.Require("action").NotNull();
			action(argument);
		}

		/// <summary>
		/// Realiza a chama do filtro da inscrição.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="argument"></param>
		/// <returns></returns>
		public virtual bool InvokeFilter(Predicate<TPayload> filter, TPayload argument)
		{
			filter.Require("filter").NotNull();
			return filter(argument);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(!string.IsNullOrEmpty(_name))
				return string.Format("[Name : {0}]", _name);
			return base.ToString();
		}
	}
}
