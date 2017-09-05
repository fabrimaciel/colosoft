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
using System.Threading.Tasks;

namespace Colosoft
{
	/// <summary>
	/// Implementação base de um agregador de observers.
	/// </summary>
	/// <typeparam name="TObserver"></typeparam>
	public abstract class AggregateObserver<TObserver>
	{
		private Queue<TObserver> _observers = new Queue<TObserver>();

		/// <summary>
		/// Observers agregados.
		/// </summary>
		protected Queue<TObserver> Observers
		{
			get
			{
				return _observers;
			}
		}

		/// <summary>
		/// Adiciona mais um novo observer para o agregador. 
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		public AggregateObserver<TObserver> Add(TObserver observer)
		{
			if(observer != null)
				lock (_observers)
					_observers.Enqueue(observer);
			return this;
		}

		/// <summary>
		/// Remove o observer que já foi anexado.
		/// </summary>
		/// <param name="observer"></param>
		/// <returns></returns>
		public AggregateObserver<TObserver> Remove(TObserver observer)
		{
			if(observer != null)
			{
				lock (_observers)
				{
					var aux = new Queue<TObserver>();
					while (_observers.Count > 0)
					{
						var i = _observers.Dequeue();
						if(!object.ReferenceEquals(i, observer))
							aux.Enqueue(i);
					}
					while (aux.Count > 0)
						_observers.Enqueue(aux.Dequeue());
				}
			}
			return this;
		}

		/// <summary>
		/// Adiciona mais um novo observer para o agregador.
		/// </summary>
		/// <param name="aggregate"></param>
		/// <param name="observer"></param>
		public static AggregateObserver<TObserver>operator +(AggregateObserver<TObserver> aggregate, TObserver observer)
		{
			if(aggregate != null)
				return aggregate.Add(observer);
			return aggregate;
		}

		/// <summary>
		/// Remove o observer que já foi anexado.
		/// </summary>
		/// <param name="aggregate"></param>
		/// <param name="observer"></param>
		public static AggregateObserver<TObserver>operator -(AggregateObserver<TObserver> aggregate, TObserver observer)
		{
			if(aggregate != null)
				return aggregate.Remove(observer);
			return aggregate;
		}
	}
}
