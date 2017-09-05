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
	/// Implementação da resultado de uma operação assincrona.
	/// </summary>
	public class AsyncResult : IAsyncResult, IDisposable
	{
		private Exception _exception;

		private bool _isCompleted;

		private object _stateObject;

		private AsyncCallback _userCallback;

		private System.Threading.ManualResetEvent _waitHandle;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="userCallback"></param>
		/// <param name="stateObject"></param>
		public AsyncResult(AsyncCallback userCallback, object stateObject)
		{
			_userCallback = userCallback;
			_stateObject = stateObject;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~AsyncResult()
		{
			Dispose(false);
		}

		/// <summary>
		/// Valida o IAsyncResult que é compatível com o AsyncResult e dispara
		/// uma exception caso exista.
		/// </summary>
		/// <param name="ar"></param>
		public static void Validate(IAsyncResult ar)
		{
			var asyncResult = ar as AsyncResult;
			if(asyncResult != null && asyncResult.Exception != null)
				throw asyncResult.Exception;
		}

		/// <summary>
		/// Registra que a operação foi executado com sucesso.
		/// </summary>
		/// <param name="asyncException"></param>
		/// <param name="completedSynchronously"></param>
		public virtual void Completed(Exception asyncException, bool completedSynchronously)
		{
			_exception = asyncException;
			lock (this)
			{
				_isCompleted = completedSynchronously;
				if(_waitHandle != null)
					_waitHandle.Set();
			}
			if(_userCallback != null)
				_userCallback(this);
		}

		/// <summary>
		/// Estado.
		/// </summary>
		public object AsyncState
		{
			get
			{
				return _stateObject;
			}
		}

		/// <summary>
		/// Manipulador de espera.
		/// </summary>
		public System.Threading.WaitHandle AsyncWaitHandle
		{
			get
			{
				if(_waitHandle == null)
				{
					lock (this)
						if(_waitHandle == null)
							_waitHandle = new System.Threading.ManualResetEvent(_isCompleted);
				}
				return _waitHandle;
			}
		}

		/// <summary>
		/// Identifica se será completado de forma sincronizada.
		/// </summary>
		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se foi executado
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				return _isCompleted;
			}
		}

		/// <summary>
		/// Instancia do erro ocorrido.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		/// <summary>
		/// Libera instancia.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
				lock (this)
					if(_waitHandle != null)
						((IDisposable)_waitHandle).Dispose();
		}
	}
	/// <summary>
	/// Implementação genérica e padrão para <see cref="IAsyncResult"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AsyncResult<T> : IAsyncResult, IDisposable
	{
		private readonly AsyncCallback _callback;

		private readonly System.Threading.ManualResetEvent _waitHandle;

		private object _asyncState;

		private readonly object _syncRoot;

		private Exception _exception;

		private bool _completed;

		private bool _completedSynchronously;

		private T _result;

		/// <summary>
		/// Estado do resultado.
		/// </summary>
		public object AsyncState
		{
			get
			{
				return _asyncState;
			}
		}

		/// <summary>
		/// Handle associado.
		/// </summary>
		public System.Threading.WaitHandle AsyncWaitHandle
		{
			get
			{
				return _waitHandle;
			}
		}

		/// <summary>
		/// Identifica se foi sincronizado completamente.
		/// </summary>
		public bool CompletedSynchronously
		{
			get
			{
				lock (_syncRoot)
					return _completedSynchronously;
			}
		}

		/// <summary>
		/// Identifica se foi completado.
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				lock (_syncRoot)
					return _completed;
			}
		}

		/// <summary>
		/// Exception ocorrida.
		/// </summary>
		public Exception Exception
		{
			get
			{
				lock (_syncRoot)
					return _exception;
			}
		}

		/// <summary>
		/// Instancia do resultado.
		/// </summary>
		public T Result
		{
			get
			{
				lock (_syncRoot)
					return _result;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		public AsyncResult(AsyncCallback cb, object state) : this(cb, state, false)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		/// <param name="completed"></param>
		public AsyncResult(AsyncCallback cb, object state, bool completed)
		{
			_callback = cb;
			_asyncState = state;
			_completed = completed;
			_completedSynchronously = completed;
			_waitHandle = new System.Threading.ManualResetEvent(false);
			_syncRoot = new object();
		}

		/// <summary>
		/// Valida o IAsyncResult que é compatível com o AsyncResult e dispara
		/// uma exception caso exista.
		/// </summary>
		/// <param name="ar"></param>
		public static T Validate(IAsyncResult ar)
		{
			var asyncResult = ar as AsyncResult<T>;
			if(asyncResult != null && asyncResult.Exception != null)
				throw asyncResult.Exception;
			if(asyncResult == null)
				throw new ArgumentException(string.Format("Argument 'ar' isn't {0}", typeof(AsyncResult<T>).Name));
			return asyncResult.Result;
		}

		/// <summary>
		/// Registra que a operação foi executado com sucesso.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="completedSynchronously"></param>
		public virtual void Complete(T result, bool completedSynchronously)
		{
			lock (_syncRoot)
			{
				_completed = true;
				_completedSynchronously = completedSynchronously;
				_result = result;
			}
			this.SignalCompletion();
		}

		/// <summary>
		/// Notifica um erro ocorrido.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="completedSynchronously"></param>
		public virtual void HandleException(Exception e, bool completedSynchronously)
		{
			lock (_syncRoot)
			{
				_completed = true;
				_completedSynchronously = completedSynchronously;
				_exception = e;
			}
			this.SignalCompletion();
		}

		/// <summary>
		/// Sinaliza a finalização da execução.
		/// </summary>
		private void SignalCompletion()
		{
			_waitHandle.Set();
			System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(this.InvokeCallback));
		}

		/// <summary>
		/// Chama o método de retorno.
		/// </summary>
		/// <param name="state"></param>
		private void InvokeCallback(object state)
		{
			if(_callback != null)
				_callback(this);
		}

		/// <summary>
		/// Libera instancia.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
				lock (_syncRoot)
					if(_waitHandle != null)
						((IDisposable)_waitHandle).Dispose();
		}
	}
}
