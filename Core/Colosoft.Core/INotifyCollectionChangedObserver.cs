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

namespace Colosoft.Collections
{
	/// <summary>
	/// Assinatura da classe que trabalha como um observer 
	/// de uma coleção com suporte para notificações.
	/// </summary>
	public interface INotifyCollectionChangedObserver
	{
		/// <summary>
		/// Método acionado qaundo a coleção sofre alguam alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e);
	}
	/// <summary>
	/// Implementação do agragador de observers.
	/// </summary>
	public class AggregateNotifyCollectionChangedObserver : INotifyCollectionChangedObserver, IDisposable
	{
		private Queue<Tuple<NotifyCollectionChangedObserverLiveScope, INotifyCollectionChangedObserver>> _observers = new Queue<Tuple<NotifyCollectionChangedObserverLiveScope, INotifyCollectionChangedObserver>>();

		/// <summary>
		/// Destrutor.
		/// </summary>
		~AggregateNotifyCollectionChangedObserver()
		{
			Dispose(true);
		}

		/// <summary>
		/// Adiciona mais um novo observer para o agregador. 
		/// </summary>
		/// <param name="observer"></param>
		/// <param name="liveScope"></param>
		/// <returns></returns>
		public AggregateNotifyCollectionChangedObserver Add(INotifyCollectionChangedObserver observer, NotifyCollectionChangedObserverLiveScope liveScope)
		{
			if(observer != null)
				lock (_observers)
					_observers.Enqueue(new Tuple<NotifyCollectionChangedObserverLiveScope, INotifyCollectionChangedObserver>(liveScope, observer));
			return this;
		}

		/// <summary>
		/// Remove o observer que já foi anexado.
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		public AggregateNotifyCollectionChangedObserver Remove(INotifyCollectionChangedObserver observer)
		{
			if(observer != null)
			{
				lock (_observers)
				{
					var aux = new Queue<Tuple<NotifyCollectionChangedObserverLiveScope, INotifyCollectionChangedObserver>>();
					while (_observers.Count > 0)
					{
						var i = _observers.Dequeue();
						if(!object.ReferenceEquals(i.Item2, observer))
							aux.Enqueue(i);
					}
					while (aux.Count > 0)
						_observers.Enqueue(aux.Dequeue());
				}
			}
			return this;
		}

		/// <summary>
		/// Método acionado quando a coleção sofre alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			lock (_observers)
				foreach (var i in _observers)
					i.Item2.OnCollectionChanged(sender, e);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			while (_observers.Count > 0)
			{
				var observer = _observers.Dequeue();
				if(observer != null && observer.Item1 == NotifyCollectionChangedObserverLiveScope.Instance && observer.Item2 is IDisposable)
					((IDisposable)observer.Item2).Dispose();
			}
		}

		/// <summary>
		/// Libera a instancia
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
