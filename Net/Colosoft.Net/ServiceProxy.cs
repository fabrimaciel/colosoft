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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Security.Cryptography;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação base de um proxy para serviços.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ServiceProxy<T> : IDisposable
	{
		private Binding _binding;

		private T _channel;

		private ChannelFactory<T> _channelFactory;

		private Uri _uri;

		private string _url;

		/// <summary>
		/// Instancia do canal.
		/// </summary>
		protected T Channel
		{
			get
			{
				if(_channel == null)
					_channel = ChannelFactory.CreateChannel();
				return _channel;
			}
		}

		/// <summary>
		/// Instancia da factory do canal.
		/// </summary>
		protected ChannelFactory<T> ChannelFactory
		{
			get
			{
				if(_channelFactory == null)
				{
					_channelFactory = new ChannelFactory<T>(this._binding, new EndpointAddress(this._url));
					if(ServiceProxy<T>.DetermineSecurityMode(this._uri) == SecurityMode.Transport)
					{
						_channelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
						_channelFactory.Endpoint.Behaviors.Add(new Colosoft.Net.SecurityTokenBehavior());
					}
				}
				return _channelFactory;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="url">Url do serviço.</param>
		public ServiceProxy(string url) : this(url, TimeSpan.FromMinutes(5.0))
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="url">Url do serviço.</param>
		/// <param name="sendTimeout">Timeout de envio.</param>
		public ServiceProxy(string url, TimeSpan sendTimeout) : this(url, sendTimeout, CommunicationHelpers.CreateBinding(ServiceProxy<T>.DetermineSecurityMode(url), false))
		{
		}

		/// <summary>
		/// Cria uma instancia do proxy.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="sendTimeout"></param>
		/// <param name="binding"></param>
		public ServiceProxy(string url, TimeSpan sendTimeout, Binding binding)
		{
			binding.SendTimeout = sendTimeout;
			_url = url;
			_uri = new Uri(url);
			_binding = binding;
		}

		/// <summary>
		/// Cria uma nova instancia com os parametros informados.
		/// </summary>
		/// <param name="binding"></param>
		/// <param name="endpointAddress"></param>
		public ServiceProxy(Binding binding, EndpointAddress endpointAddress)
		{
			binding.Require("binding").NotNull();
			endpointAddress.Require("endpointAddress").NotNull();
			_binding = binding;
			_uri = endpointAddress.Uri;
			_url = endpointAddress.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Inicia uma chamada assincrona.
		/// </summary>
		/// <param name="begin">Callback de inicio.</param>
		/// <param name="end">Callback de fim.</param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		protected IAsyncResult BeginAsyncInvoke(Func<T, AsyncCallback, object, IAsyncResult> begin, Action<T, IAsyncResult> end, AsyncCallback callback, object state)
		{
			var operation = new AsyncOperation();
			operation.Begin = begin;
			operation.End = end;
			operation.CertificateIndex = -1;
			operation.InnerCallback = callback;
			operation.InnerState = state;
			return this.BeginOperation(operation);
		}

		/// <summary>
		/// Inicia uma chamada assincrona.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		protected IAsyncResult BeginAsyncInvoke<TResult>(Func<T, AsyncCallback, object, IAsyncResult> begin, Func<T, IAsyncResult, TResult> end, AsyncCallback callback, object state)
		{
			var operation = new AsyncOperationWithResult<TResult>();
			operation.Begin = begin;
			operation.End = end;
			operation.CertificateIndex = -1;
			operation.InnerCallback = callback;
			operation.InnerState = state;
			return this.BeginOperation(operation);
		}

		/// <summary>
		/// Executa a ação.
		/// </summary>
		/// <param name="action"></param>
		protected void Do(Action<T> action)
		{
			this.Do<bool>(f =>  {
				action(f);
				return true;
			});
		}

		/// <summary>
		/// Executa a ação.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="action"></param>
		/// <returns></returns>
		protected TResult Do<TResult>(Func<T, TResult> action)
		{
			return action.Invoke(this.ChannelFactory.CreateChannel());
		}

		/// <summary>
		/// Método de finalização da chamada assincrona.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		protected TResult EndAsyncInvoke<TResult>(IAsyncResult result)
		{
			this.EndAsyncInvoke(result);
			var result2 = result as AsyncOperationWithResult<TResult>;
			if(result2 == null)
			{
				return default(TResult);
			}
			return result2.Result;
		}

		/// <summary>
		/// Método de finalização da chamada assincrona.
		/// </summary>
		/// <param name="result"></param>
		protected void EndAsyncInvoke(IAsyncResult result)
		{
			var base2 = result as AsyncOperationBase;
			if(base2 != null)
			{
				base2.AsyncWaitHandle.WaitOne();
				((IDisposable)base2.AsyncWaitHandle).Dispose();
				if(base2.InnerException != null)
				{
					throw base2.InnerException;
				}
			}
		}

		/// <summary>
		/// Inicia a operação assincrona.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		private IAsyncResult BeginOperation(AsyncOperationBase operation)
		{
			operation.Channel = ChannelFactory.CreateChannel();
			operation.InnerResult = operation.Begin.Invoke(operation.Channel, new AsyncCallback(this.EndRequest), operation);
			return operation;
		}

		/// <summary>
		/// Determina o modo de segurança.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private static SecurityMode DetermineSecurityMode(string url)
		{
			return ServiceProxy<T>.DetermineSecurityMode(new Uri(url));
		}

		/// <summary>
		/// Determina o modo de segurança.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		private static SecurityMode DetermineSecurityMode(Uri uri)
		{
			if(!uri.Scheme.Equals(Uri.UriSchemeHttps))
			{
				return SecurityMode.None;
			}
			return SecurityMode.Transport;
		}

		/// <summary>
		/// Método de finalização da requisição assincrona.
		/// </summary>
		/// <param name="result"></param>
		private void EndRequest(IAsyncResult result)
		{
			var asyncState = result.AsyncState as AsyncOperationBase;
			if(asyncState != null)
			{
				try
				{
					asyncState.EndOperation(result);
					asyncState.Complete();
				}
				catch(MessageSecurityException exception)
				{
					if(!ServiceProxy<T>.IsAuthenticationFailure(exception.InnerException as FaultException))
					{
						asyncState.InnerException = exception;
						asyncState.Complete();
					}
				}
				catch(CryptographicException)
				{
				}
				catch(Exception exception2)
				{
					asyncState.InnerException = exception2;
					asyncState.Complete();
				}
				if(!asyncState.IsCompleted)
				{
					if(!asyncState.UsingTransportSecurity)
						asyncState.Complete();
					else
					{
						if(asyncState.UsingCachedCertificate)
						{
							asyncState.UsingCachedCertificate = false;
						}
						asyncState.CertificateIndex++;
						this.BeginOperation(asyncState);
					}
				}
			}
		}

		/// <summary>
		/// Verifica se houve falha a autenticação.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		private static bool IsAuthenticationFailure(FaultException exception)
		{
			return (((((exception != null) && (exception.Code != null)) && (exception.Code.IsSenderFault && (exception.Code.SubCode != null))) && exception.Code.SubCode.Name.Equals("FailedAuthentication")) && exception.Code.SubCode.Namespace.Equals("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"));
		}

		/// <summary>
		/// tenta executar.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="action"></param>
		/// <param name="channel"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool TryDo<TResult>(Func<T, TResult> action, T channel, out TResult result)
		{
			try
			{
				result = action.Invoke(channel);
				return true;
			}
			catch(MessageSecurityException exception)
			{
				if(!ServiceProxy<T>.IsAuthenticationFailure(exception.InnerException as FaultException))
					throw;
			}
			catch(CryptographicException)
			{
			}
			result = default(TResult);
			return false;
		}

		/// <summary>
		/// Libera a versão.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			CommunicationHelpers.Shutdown(this._channelFactory);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Representa uma operação assincrona.
		/// </summary>
		private class AsyncOperation : ServiceProxy<T>.AsyncOperationBase
		{
			public Action<T, IAsyncResult> End
			{
				get;
				set;
			}

			/// <summary>
			/// Finaliza a operação.
			/// </summary>
			/// <param name="result"></param>
			public override void EndOperation(IAsyncResult result)
			{
				this.End.Invoke(base.Channel, result);
			}
		}

		/// <summary>
		/// Implementação base de uma operação assincrona.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
		private abstract class AsyncOperationBase : IAsyncResult
		{
			private bool _isCompleted;

			private ManualResetEvent _waitHandle;

			protected AsyncOperationBase()
			{
				_waitHandle = new ManualResetEvent(false);
			}

			/// <summary>
			/// Notifica que a operação foi finalizada.
			/// </summary>
			public void Complete()
			{
				_waitHandle.Set();
				_isCompleted = true;
				if(InnerCallback != null)
					InnerCallback(this);
			}

			/// <summary>
			/// Método acionado quand a operação e finalizada.
			/// </summary>
			/// <param name="result"></param>
			public abstract void EndOperation(IAsyncResult result);

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return _waitHandle;
				}
			}

			public Func<T, AsyncCallback, object, IAsyncResult> Begin
			{
				get;
				set;
			}

			public int CertificateIndex
			{
				get;
				set;
			}

			public T Channel
			{
				get;
				set;
			}

			public AsyncCallback InnerCallback
			{
				get;
				set;
			}

			public Exception InnerException
			{
				get;
				set;
			}

			public IAsyncResult InnerResult
			{
				get;
				set;
			}

			public object InnerState
			{
				get;
				set;
			}

			public bool IsCompleted
			{
				get
				{
					return _isCompleted;
				}
			}

			object IAsyncResult.AsyncState
			{
				get
				{
					return this.InnerState;
				}
			}

			bool IAsyncResult.CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool UsingCachedCertificate
			{
				get;
				set;
			}

			public bool UsingTransportSecurity
			{
				get;
				set;
			}
		}

		/// <summary>
		/// Representa uma operaçõa assincrona com resultado.
		/// </summary>
		/// <typeparam name="TResult">Tipo do resultado.</typeparam>
		private class AsyncOperationWithResult<TResult> : ServiceProxy<T>.AsyncOperationBase
		{
			/// <summary>
			/// Callback.
			/// </summary>
			public Func<T, IAsyncResult, TResult> End
			{
				get;
				set;
			}

			/// <summary>
			/// Resultado.
			/// </summary>
			public TResult Result
			{
				get;
				set;
			}

			public override void EndOperation(IAsyncResult result)
			{
				this.Result = this.End.Invoke(base.Channel, result);
			}
		}
	}
}
