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
	/// Dispatcher padrão.
	/// </summary>
	public class DefaultDispatcher : IDispatcher
	{
		/// <summary>
		/// Verifica se a thread de chamada é a thread associada com o dispatcher.
		/// </summary>
		/// <returns></returns>
		public virtual bool CheckAccess()
		{
			return (System.Threading.Thread.CurrentThread == Thread || System.Threading.Thread.CurrentThread.ManagedThreadId == 1);
		}

		/// <summary>
		/// Verifica se a thraed de chamada é a thread associada com o dispatcher.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Caso a thread de chamada não seja a mesma que a Thread vinculada.</exception>
		public void VerifyAccess()
		{
			if(!CheckAccess())
				throw new InvalidOperationException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona com os argumentos especificados na linha que o Dispatcher foi criado.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(Delegate method, params object[] args)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(Action method)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona na prioridade especificada no segmento Dispatcher está associado.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona com os argumentos especificados, na prioridade especificada, no segmento que o Dispatcher foi criado.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="priority"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority, params object[] args)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona na prioridade especificada e com o argumento especificado no segmento do Dispatcher está associado.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="method"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa o delegate de forma assíncrona na prioridade especificada e com a matriz especificada de argumentos na discussão do Dispatcher está associado.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="method"></param>
		/// <param name="arg"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public virtual IDispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa um delegate de forma assincrona.
		/// </summary>
		/// <param name="method">Delegate do método que será executado.</param>
		/// <param name="args">Parametros do método que será executado.</param>
		/// <returns>
		/// O valor de retorno do delegate que está sendo 
		/// chamado ou nulo se o delegate não tem valor de retorno.
		/// </returns>
		public virtual object Invoke(Delegate method, params object[] args)
		{
			if(method == null)
				throw new ArgumentNullException("method");
			return method.DynamicInvoke(args);
		}

		/// <summary>
		/// Executa um delegate de forma assincrona.
		/// </summary>
		/// <param name="method">Delegate do método que será executado.</param>
		/// <param name="priority"></param>
		/// <param name="args">Parametros do método que será executado.</param>
		/// <returns>
		/// O valor de retorno do delegate que está sendo 
		/// chamado ou nulo se o delegate não tem valor de retorno.
		/// </returns>
		public virtual object Invoke(Delegate method, DispatcherPriority priority, object[] args)
		{
			return Invoke(method, args);
		}

		/// <summary>
		/// Executa a sincronização com a thread do despachante.
		/// </summary>
		/// <param name="func">Func que será acionada.</param>
		/// <returns></returns>
		public T Invoke<T>(Func<T> func)
		{
			if(func == null)
				throw new ArgumentNullException("func");
			return func();
		}

		/// <summary>
		/// Executa um delegate de forma assincrona.
		/// </summary>
		/// <param name="action">Delegate do método que será executado.</param>
		public virtual void Invoke(Action action)
		{
			if(action == null)
				throw new ArgumentNullException("action");
			action();
		}

		/// <summary>
		/// Processa os eventos pendentes no sistema.
		/// </summary>
		public void DoEvents()
		{
		}

		/// <summary>
		/// Thread relacionada com o dispatcher.
		/// </summary>
		public System.Threading.Thread Thread
		{
			get
			{
				return System.Threading.Thread.CurrentThread;
			}
		}
	}
}
