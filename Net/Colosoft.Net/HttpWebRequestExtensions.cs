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

namespace Colosoft.Net.Mono
{
	/// <summary>
	/// Classe com métodos de extensão para manipular o HttpWebRequest.
	/// </summary>
	static class HttpWebRequestExtensions
	{
		private static System.Reflection.FieldInfo _locker;

		private static System.Reflection.FieldInfo _getResponseCalled;

		private static System.Reflection.FieldInfo _asyncWrite;

		private static System.Reflection.FieldInfo _initialMethod;

		private static System.Reflection.FieldInfo _haveRequest;

		private static System.Reflection.FieldInfo _writeStream;

		private static System.Reflection.MethodInfo _getServicePoint;

		private static System.Reflection.FieldInfo _servicePoint;

		private static System.Reflection.FieldInfo _gotRequestStream;

		private static System.Reflection.FieldInfo _requestSent;

		private static System.Reflection.FieldInfo _redirects;

		private static System.Reflection.FieldInfo _abortHandler;

		private static System.Reflection.FieldInfo _connectionGroup;

		private static System.Reflection.ConstructorInfo _webAsyncResult;

		private static System.Reflection.PropertyInfo _aborted;

		/// <summary>
		/// Recupera o objeto de lock.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static object GetLock(this System.Net.HttpWebRequest request)
		{
			return (_locker ?? (_locker = typeof(System.Net.HttpWebRequest).GetField("locker", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Verifica se a resposta já foi chamada.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static bool IsResponseCalled(this System.Net.HttpWebRequest request)
		{
			return (bool)(_getResponseCalled ?? (_getResponseCalled = typeof(System.Net.HttpWebRequest).GetField("getResponseCalled", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Recupera o resultado assincrono da escrita.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static IAsyncResult GetAsyncWrite(this System.Net.HttpWebRequest request)
		{
			return (IAsyncResult)(_asyncWrite ?? (_asyncWrite = typeof(System.Net.HttpWebRequest).GetField("asyncWrite", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Define o resultado assincrono da escrita.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetAsyncWrite(this System.Net.HttpWebRequest request, IAsyncResult value)
		{
			(_asyncWrite ?? (_asyncWrite = typeof(System.Net.HttpWebRequest).GetField("asyncWrite", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Define o nome do método inicial.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetInitialMethod(this System.Net.HttpWebRequest request, object value)
		{
			(_initialMethod ?? (_initialMethod = typeof(System.Net.HttpWebRequest).GetField("initialMethod", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Verifica se já possui requisição.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static bool HaveRequest(this System.Net.HttpWebRequest request)
		{
			return (bool)(_haveRequest ?? (_haveRequest = typeof(System.Net.HttpWebRequest).GetField("haveRequest", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Recupera a stream de escrita.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static System.IO.Stream GetWriteStream(this System.Net.HttpWebRequest request)
		{
			return (System.IO.Stream)(_writeStream ?? (_writeStream = typeof(System.Net.HttpWebRequest).GetField("writeStream", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Recupera o ponto do serviço.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static System.Net.ServicePoint GetServicePoint(this System.Net.HttpWebRequest request)
		{
			return (System.Net.ServicePoint)(_getServicePoint ?? (_getServicePoint = typeof(System.Net.HttpWebRequest).GetMethod("GetServicePoint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).Invoke(request, null);
		}

		/// <summary>
		/// Define o ponto do serviço.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetServicePoint(this System.Net.HttpWebRequest request, System.Net.ServicePoint value)
		{
			(_servicePoint ?? (_servicePoint = typeof(System.Net.HttpWebRequest).GetField("servicePoint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Define que pegou a stream da requisição.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetGotRequestStream(this System.Net.HttpWebRequest request, bool value)
		{
			(_gotRequestStream ?? (_gotRequestStream = typeof(System.Net.HttpWebRequest).GetField("gotRequestStream", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Define se a requisição foi enviada.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetRequestSent(this System.Net.HttpWebRequest request, bool value)
		{
			(_requestSent ?? (_requestSent = typeof(System.Net.HttpWebRequest).GetField("requestSent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Verifica se a requisição foi enviada.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static bool IsRequestSent(this System.Net.HttpWebRequest request)
		{
			return (bool)(_requestSent ?? (_requestSent = typeof(System.Net.HttpWebRequest).GetField("requestSent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Define a quantidade de redirects.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetRedirects(this System.Net.HttpWebRequest request, int value)
		{
			(_redirects ?? (_redirects = typeof(System.Net.HttpWebRequest).GetField("redirects", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Define o AbortHandler.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="value"></param>
		private static void SetAbortHandler(this System.Net.HttpWebRequest request, EventHandler value)
		{
			(_abortHandler ?? (_abortHandler = typeof(System.Net.HttpWebRequest).GetField("abortHandler", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).SetValue(request, value);
		}

		/// <summary>
		/// Recupera o nome do grupo de conexão associado.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static string GetConnectionGroup(this System.Net.HttpWebRequest request)
		{
			return (string)(_connectionGroup ?? (_connectionGroup = typeof(System.Net.HttpWebRequest).GetField("connectionGroup", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request);
		}

		/// <summary>
		/// Cria o resulta assincrono de chamadas Web.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		private static IAsyncResult CreateWebAsyncResult(System.Net.HttpWebRequest request, AsyncCallback callback, object state)
		{
			return (IAsyncResult)(_webAsyncResult ?? (_webAsyncResult = typeof(System.Net.WebClient).Assembly.GetType("System.Net.WebAsyncResult").GetConstructor(new[] {
				typeof(System.Net.HttpWebRequest),
				typeof(AsyncCallback),
				typeof(object)
			}))).Invoke(new[] {
				request,
				callback,
				state
			});
		}

		/// <summary>
		/// Verifica se a requisição foi abortada.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static bool IsAborted(this System.Net.HttpWebRequest request)
		{
			return (bool)(_aborted ?? (_aborted = typeof(System.Net.HttpWebRequest).GetProperty("Aborted", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic))).GetValue(request, null);
		}

		/// <summary>
		/// Inicia o processo assincrono de recuperação da stream de requisição.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IAsyncResult BeginGetRequestStream(System.Net.HttpWebRequest request, AsyncCallback callback, object state)
		{
			try
			{
				if(request.IsAborted())
					throw new System.Net.WebException("The request was canceled.", System.Net.WebExceptionStatus.RequestCanceled);
				bool flag = (((request.Method != "GET") && (request.Method != "CONNECT")) && (request.Method != "HEAD")) && (request.Method != "TRACE");
				if(request.Method == null || !flag)
					throw new System.Net.ProtocolViolationException("Cannot send data when method is: " + request.Method);
				if((request.ContentLength == -1L && !request.SendChunked) && (request.KeepAlive))
					throw new System.Net.ProtocolViolationException("Content-Length not set");
				string transferEncoding = request.TransferEncoding;
				if((!request.SendChunked && (transferEncoding != null)) && (transferEncoding.Trim() != string.Empty))
					throw new System.Net.ProtocolViolationException("SendChunked should be true.");
				object locker = request.GetLock();
				lock (locker)
				{
					if(request.IsResponseCalled())
						throw new InvalidOperationException("The operation cannot be performed once the request has been submitted.");
					if(request.GetAsyncWrite() != null)
						throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
					request.SetAsyncWrite(CreateWebAsyncResult(request, callback, state));
					request.SetInitialMethod(request.Method);
					var writeStream = request.GetWriteStream();
					IAsyncResult asyncWrite = null;
					if(request.HaveRequest() && (writeStream != null))
					{
						asyncWrite = request.GetAsyncWrite();
						asyncWrite.SetCompleted(true, writeStream);
						asyncWrite.DoCallback();
						return request.GetAsyncWrite();
					}
					request.SetGotRequestStream(true);
					asyncWrite = request.GetAsyncWrite();
					if(!request.IsRequestSent())
					{
						request.SetRequestSent(true);
						request.SetRedirects(0);
						var servicePoint = request.GetServicePoint();
						request.SetServicePoint(servicePoint);
						request.SetAbortHandler(servicePoint.SendRequest(request, request.GetConnectionGroup()));
					}
					return asyncWrite;
				}
			}
			catch(Exception ex)
			{
				throw new Exception(ex.ToString(), ex);
			}
		}
	}
}
