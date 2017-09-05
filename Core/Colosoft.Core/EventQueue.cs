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
using Colosoft.DataStructures;
using System.Collections;
using System.Threading;

namespace Colosoft.Threading
{
	/// <summary>
	/// Implementação de uma fila de eventos.
	/// </summary>
	internal class EventQueue : BinaryPriorityQueue
	{
		/// <summary>
		/// Verifica se a fila está vazia.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				lock (this)
					return (base.Count == 0);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EventQueue() : base(new EventQueueComparer())
		{
		}

		/// <summary>
		/// Limpa a instancia.
		/// </summary>
		public override void Clear()
		{
			lock (this)
			{
				base.Clear();
				Monitor.Pulse(this);
			}
		}

		/// <summary>
		/// Recupera o evento que está na frente da fila.
		/// </summary>
		/// <returns></returns>
		new public QueuedEvent Peek()
		{
			lock (this)
				return (base.Peek() as QueuedEvent);
		}

		/// <summary>
		/// Recupera e remove o primeiro elemento da fila.
		/// </summary>
		/// <returns></returns>
		new public QueuedEvent Pop()
		{
			lock (this)
				return (base.Pop() as QueuedEvent);
		}

		/// <summary>
		/// Adiciona o elemento na fila.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override int Push(object value)
		{
			lock (this)
			{
				int num = base.Push(value);
				Monitor.Pulse(this);
				return num;
			}
		}

		/// <summary>
		/// Implementação de um comparador para a fila de eventos.
		/// </summary>
		internal class EventQueueComparer : IComparer
		{
			/// <summary>
			/// Compara a instancias.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(object x, object y)
			{
				QueuedEvent event2 = x as QueuedEvent;
				QueuedEvent event3 = y as QueuedEvent;
				return event2.SchedTime.CompareTo(event3.SchedTime);
			}
		}
	}
}
