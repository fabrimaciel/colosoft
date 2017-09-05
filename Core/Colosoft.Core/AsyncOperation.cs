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

namespace Colosoft.Threading
{
	/// <summary>
	/// Representa uma operação assincrona.
	/// </summary>
	public class AsyncOperation
	{
		private object _endLock;

		private Exception _exception;

		private int _maxCount;

		private int _pending;

		private int _waiting;

		/// <summary>
		/// Erro ocorrido.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
			set
			{
				if(_exception == null)
				{
					lock (this)
						if(_exception == null)
							_exception = value;
				}
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AsyncOperation()
		{
		}

		/// <summary>
		/// Cria uma nova instancia.
		/// </summary>
		/// <param name="maxCount">Quantidade máxima de operações suportadas.</param>
		public AsyncOperation(int maxCount)
		{
			_maxCount = maxCount;
			_endLock = new object();
		}

		/// <summary>
		/// Inicia um operação.
		/// </summary>
		public virtual void Begin()
		{
			if(_endLock != null)
			{
				lock (_endLock)
				{
					while (_pending >= _maxCount)
					{
						_waiting++;
						System.Threading.Monitor.Wait(_endLock);
						_waiting--;
					}
					_pending++;
					return;
				}
			}
			_pending++;
		}

		/// <summary>
		/// Finaliza uma operação.
		/// </summary>
		public virtual void End()
		{
			int num = 0;
			if(_endLock != null)
			{
				lock (_endLock)
				{
					num = --_pending;
					if(_waiting > 0)
						System.Threading.Monitor.Pulse(_endLock);
				}
			}
			else if(_pending > 0)
				num = --_pending;
			if(num == 0)
			{
				lock (this)
					if(_pending == 0)
						System.Threading.Monitor.Pulse(this);
			}
		}

		/// <summary>
		/// Espera finalizar.
		/// </summary>
		public void WaitForCompletion()
		{
			this.WaitForCompletion(-1);
		}

		/// <summary>
		/// Espera finalizar.
		/// </summary>
		/// <param name="millisecondsTimeout">Tempo de espera.</param>
		/// <returns></returns>
		public bool WaitForCompletion(int millisecondsTimeout)
		{
			lock (this)
			{
				if(_pending > 0)
					return System.Threading.Monitor.Wait(this, millisecondsTimeout);
			}
			return true;
		}
	}
}
