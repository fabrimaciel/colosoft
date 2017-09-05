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
using System.ServiceModel;
using System.Text;

namespace Colosoft.ServiceProcess
{
	/// <summary>
	/// Implementação do host do agente.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class HttpServiceHost : IDisposable
	{
		private Colosoft.ServiceProcess.IHttpService _service;

		private Colosoft.Logging.ILogger _logger;

		private System.ServiceModel.ServiceHost _serviceHost;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="logger"></param>
		public HttpServiceHost(Colosoft.ServiceProcess.IHttpService service, Colosoft.Logging.ILogger logger)
		{
			_service = service;
			_logger = logger;
			var thread = new System.Threading.Thread(() => OpenHttpChannel());
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~HttpServiceHost()
		{
			Dispose(false);
		}

		/// <summary>
		/// Inicializa a comunicação.
		/// </summary>
		private void InitializeCommunication()
		{
			_serviceHost = null;
		}

		/// <summary>
		/// Abre o objeto de comunicação.
		/// </summary>
		/// <param name="communicationObject"></param>
		/// <returns></returns>
		private bool OpenCommunicationObject(ICommunicationObject communicationObject)
		{
			if(communicationObject == null)
				return false;
			try
			{
				communicationObject.BeginOpen(new AsyncCallback(OpenCommunicationObjectCallback), communicationObject);
				return true;
			}
			catch(Exception exception)
			{
				AbortCommunication(communicationObject, exception);
				return false;
			}
		}

		/// <summary>
		/// Método do callback
		/// </summary>
		/// <param name="result"></param>
		private void OpenCommunicationObjectCallback(IAsyncResult result)
		{
			var asyncState = result.AsyncState as ICommunicationObject;
			try
			{
				asyncState.EndOpen(result);
			}
			catch(CommunicationObjectAbortedException)
			{
			}
			catch(Exception exception)
			{
				AbortCommunication(asyncState, exception);
			}
		}

		/// <summary>
		/// Aborta comunicação.
		/// </summary>
		/// <param name="communicationObject"></param>
		/// <param name="ex"></param>
		private void AbortCommunication(ICommunicationObject communicationObject, Exception ex)
		{
			if(!(ex is System.Threading.ThreadAbortException))
			{
				IMessageFormattable str;
				communicationObject.Abort();
				if(ex is System.TimeoutException)
					str = ResourceMessageFormatter.Create(() => Properties.Resources.HTTPCommunicationFailure, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, false));
				else if(ex is AddressAccessDeniedException)
					str = str = ResourceMessageFormatter.Create(() => Properties.Resources.AddressAccessDeniedException, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, false));
				else
					str = ResourceMessageFormatter.Create(() => Properties.Resources.HTTPCommunicationFailure, Colosoft.Diagnostics.ExceptionFormatter.FormatException(ex, false));
				LogError(str);
			}
		}

		/// <summary>
		/// Espara pelo estado do objeto de comunicação
		/// </summary>
		/// <param name="communicationObject"></param>
		/// <param name="stateToWaitFor">Estado que será aguardado.</param>
		/// <returns></returns>
		private bool WaitForCommunicationObjectState(ICommunicationObject communicationObject, CommunicationState stateToWaitFor)
		{
			while (true)
			{
				CommunicationState state = communicationObject.State;
				if(state == stateToWaitFor)
					return true;
				if(state == CommunicationState.Faulted)
					return false;
				if(state == CommunicationState.Closed)
					return false;
				System.Threading.Thread.Sleep(1000);
			}
		}

		/// <summary>
		/// Cria um binding para comunicação.
		/// </summary>
		/// <param name="securityMode">Modo de segurança.</param>
		/// <param name="requireClientCertificates"></param>
		/// <returns></returns>
		public static System.ServiceModel.Channels.Binding CreateBinding(SecurityMode securityMode, bool requireClientCertificates)
		{
			WSHttpBinding binding = new WSHttpBinding(securityMode);
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
			binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
			binding.MaxReceivedMessageSize = System.Xml.XmlDictionaryReaderQuotas.Max.MaxStringContentLength;
			System.ServiceModel.Channels.Binding binding2 = binding;
			if((securityMode == SecurityMode.Transport) && requireClientCertificates)
			{
				binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
				System.ServiceModel.Channels.BindingElementCollection bindingElementsInTopDownChannelStackOrder = binding.CreateBindingElements();
				var item = new System.ServiceModel.Channels.TransportSecurityBindingElement();
				System.ServiceModel.Security.Tokens.X509SecurityTokenParameters parameters = new System.ServiceModel.Security.Tokens.X509SecurityTokenParameters();
				parameters.InclusionMode = System.ServiceModel.Security.Tokens.SecurityTokenInclusionMode.AlwaysToRecipient;
				item.EndpointSupportingTokenParameters.Endorsing.Add(parameters);
				bindingElementsInTopDownChannelStackOrder.Insert(bindingElementsInTopDownChannelStackOrder.Count - 1, item);
				binding2 = new System.ServiceModel.Channels.CustomBinding(bindingElementsInTopDownChannelStackOrder);
			}
			return binding2;
		}

		/// <summary>
		/// Abre o canal http.
		/// </summary>
		/// <returns></returns>
		private bool OpenHttpChannel()
		{
			if(_serviceHost == null)
			{
				try
				{
					if(_service == null)
						throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.HttpServiceHost_HttpServiceCannotBeNull).Format());
					_serviceHost = new System.ServiceModel.ServiceHost(_service);
					_serviceHost.Description.Endpoints.Clear();
					var baseAddress = Colosoft.Net.ServiceHostBaseHelper.GetBaseAddresses(_serviceHost);
					if(baseAddress != null)
						baseAddress.Clear();
					_service.AddServiceEndpoint(_serviceHost);
					if(_serviceHost.Description == null)
						throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.HttpServiceHost_ServiceHostDescriptionCannotBeNull).Format());
					if(_serviceHost.Description.Behaviors == null)
						throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.HttpServiceHost_ServiceHostDescriptionBehaviorCannotBeNull).Format());
					var behavior = (System.ServiceModel.Description.ServiceMetadataBehavior)_serviceHost.Description.Behaviors.Where(f => f is System.ServiceModel.Description.ServiceMetadataBehavior).FirstOrDefault();
					if(behavior != null)
						behavior.HttpGetEnabled = false;
					if(OpenCommunicationObject(_serviceHost) && WaitForCommunicationObjectState(_serviceHost, CommunicationState.Opened))
						return true;
					StopExecution(ResourceMessageFormatter.Create(() => Properties.Resources.CouldNotOpenHttpServiceHost).Format());
				}
				catch(System.Threading.ThreadAbortException)
				{
				}
				catch(Exception exception)
				{
					LogError(Colosoft.Diagnostics.ExceptionFormatter.FormatException(exception, false).GetFormatter());
					StopExecution(ResourceMessageFormatter.Create(() => Properties.Resources.CouldNotOpenHttpServiceHost).Format());
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Fecha o canal http.
		/// </summary>
		private void CloseHttpChannel()
		{
			if(this._serviceHost != null)
			{
				ShutdownCommunicationObject(_serviceHost);
				_serviceHost = null;
			}
		}

		/// <summary>
		/// Para a execução.
		/// </summary>
		/// <param name="message"></param>
		private void StopExecution(string message)
		{
			this.LogError(ResourceMessageFormatter.Create(() => Properties.Resources.HttpServiceHost_ServiceHostStops, message));
		}

		/// <summary>
		/// Registra um log.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		private void LogInternal(IMessageFormattable message, Colosoft.Logging.Category category)
		{
			try
			{
				_logger.Write(message, category, Colosoft.Logging.Priority.None);
			}
			catch(ArgumentException)
			{
			}
			catch(System.ComponentModel.Win32Exception)
			{
			}
			catch(InvalidOperationException)
			{
			}
		}

		/// <summary>
		/// Desliga a comunicação com o objeto informado.
		/// </summary>
		/// <param name="communicationObject"></param>
		private void ShutdownCommunicationObject(ICommunicationObject communicationObject)
		{
			if(communicationObject != null)
			{
				try
				{
					communicationObject.BeginClose(new AsyncCallback(ShutdownCommunicationObjectCallback), communicationObject);
				}
				catch(System.TimeoutException)
				{
					communicationObject.Abort();
				}
				catch(CommunicationException)
				{
					communicationObject.Abort();
				}
				catch(NullReferenceException)
				{
					communicationObject.Abort();
				}
			}
		}

		/// <summary>
		/// Método acionado quando o comunicação for fechada.
		/// </summary>
		/// <param name="result"></param>
		private void ShutdownCommunicationObjectCallback(IAsyncResult result)
		{
			ICommunicationObject asyncState = result.AsyncState as ICommunicationObject;
			try
			{
				asyncState.EndClose(result);
			}
			catch(System.TimeoutException)
			{
				asyncState.Abort();
			}
			catch(CommunicationException)
			{
				asyncState.Abort();
			}
		}

		/// <summary>
		/// Registra o log de uma mensagem de atenção.
		/// </summary>
		/// <param name="message"></param>
		protected void LogWarning(IMessageFormattable message)
		{
			LogInternal(message, Logging.Category.Warn);
		}

		/// <summary>
		/// Registra o log de um erro.
		/// </summary>
		/// <param name="message"></param>
		protected void LogError(IMessageFormattable message)
		{
			LogInternal(message, Logging.Category.Exception);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disponsing"></param>
		protected virtual void Dispose(bool disponsing)
		{
			CloseHttpChannel();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(false);
		}
	}
}
