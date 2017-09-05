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
using System.ServiceModel.Channels;
using System.Text;

namespace Colosoft.ServiceModel
{
	public abstract class ChannelFactory : CommunicationObject, IChannelFactory, ICommunicationObject, IDisposable
	{
		private System.ServiceModel.Description.ServiceEndpoint service_endpoint;

		private IChannelFactory factory;

		private List<IClientChannel> opened_channels = new List<IClientChannel>();

		protected ChannelFactory()
		{
		}

		internal IChannelFactory OpenedChannelFactory
		{
			get
			{
				if(factory == null)
				{
					factory = CreateFactory();
					factory.Open();
				}
				return factory;
			}
			private set
			{
				factory = value;
			}
		}

		internal List<IClientChannel> OpenedChannels
		{
			get
			{
				return opened_channels;
			}
		}

		public System.ServiceModel.Description.ServiceEndpoint Endpoint
		{
			get
			{
				return service_endpoint;
			}
		}

		public System.ServiceModel.Description.ClientCredentials Credentials
		{
			get
			{
				return Endpoint.Behaviors.Find<System.ServiceModel.Description.ClientCredentials>();
			}
		}

		internal TimeSpan DefaultCloseTimeout2
		{
			get
			{
				return DefaultCloseTimeout;
			}
		}

		protected override TimeSpan DefaultCloseTimeout
		{
			get
			{
				return Endpoint.Binding.CloseTimeout;
			}
		}

		internal TimeSpan DefaultOpenTimeout2
		{
			get
			{
				return DefaultOpenTimeout;
			}
		}

		protected override TimeSpan DefaultOpenTimeout
		{
			get
			{
				return Endpoint.Binding.OpenTimeout;
			}
		}

		protected virtual void ApplyConfiguration(string endpointConfig)
		{
			if(endpointConfig == null)
				return;
			#if NET_2_1 || XAMMAC_4_5
						try {
				// It should automatically use XmlXapResolver
				var cfg = new SilverlightClientConfigLoader ().Load (XmlReader.Create ("ServiceReferences.ClientConfig"));

				SilverlightClientConfigLoader.ServiceEndpointConfiguration se = null;
				if (endpointConfig == "*")
					se = cfg.GetServiceEndpointConfiguration (Endpoint.Contract.Name);
				if (se == null)
					se = cfg.GetServiceEndpointConfiguration (endpointConfig);

				if (se.Binding != null && Endpoint.Binding == null)
					Endpoint.Binding = se.Binding;
				else // ignore it
					Console.WriteLine ("WARNING: Configured binding not found in configuration {0}", endpointConfig);
				if (se.Address != null && Endpoint.Address == null)
					Endpoint.Address = se.Address;
				else // ignore it
					Console.WriteLine ("WARNING: Configured endpoint address not found in configuration {0}", endpointConfig);
			} catch (Exception) {
				// ignore it.
				Console.WriteLine ("WARNING: failed to load endpoint configuration for {0}", endpointConfig);
			}
#else
			string contractName = Endpoint.Contract.ConfigurationName;
			var client = Colosoft.ServiceModel.Configuration.ConfigUtil.ClientSection;
			System.ServiceModel.Configuration.ChannelEndpointElement endpoint = null;
			foreach (System.ServiceModel.Configuration.ChannelEndpointElement el in client.Endpoints)
			{
				if(el.Contract == contractName && (endpointConfig == el.Name || endpointConfig == "*"))
				{
					if(endpoint != null)
						throw new InvalidOperationException(String.Format("More then one endpoint matching contract {0} was found.", contractName));
					endpoint = el;
				}
			}
			if(endpoint == null)
				throw new InvalidOperationException(String.Format("Client endpoint configuration '{0}' was not found in {1} endpoints.", endpointConfig, client.Endpoints.Count));
			var binding = String.IsNullOrEmpty(endpoint.Binding) ? null : Colosoft.ServiceModel.Configuration.ConfigUtil.CreateBinding(endpoint.Binding, endpoint.BindingConfiguration);
			var contractType = Colosoft.ServiceModel.Configuration.ConfigUtil.GetTypeFromConfigString(endpoint.Contract, Colosoft.ServiceModel.Configuration.NamedConfigCategory.Contract);
			if(contractType == null)
				throw new ArgumentException(String.Format("Contract '{0}' was not found", endpoint.Contract));
			var contract = String.IsNullOrEmpty(endpoint.Contract) ? Endpoint.Contract : System.ServiceModel.Description.ContractDescription.GetContract(contractType);
			if(!String.IsNullOrEmpty(endpoint.Kind))
			{
				var se = Configuration.ConfigUtil.ConfigureStandardEndpoint(contract, endpoint);
				if(se.Binding == null)
					se.Binding = binding;
				if(se.Address == null && se.Binding != null)
					se.Address = new EndpointAddress(endpoint.Address);
				if(se.Binding == null && se.Address != null)
					se.Binding = Configuration.ConfigUtil.GetBindingByProtocolMapping(se.Address.Uri);
				service_endpoint = se;
			}
			else
			{
				if(binding == null && endpoint.Address != null)
					Endpoint.Binding = Configuration.ConfigUtil.GetBindingByProtocolMapping(endpoint.Address);
			}
			if(Endpoint.Binding == null)
				Endpoint.Binding = Configuration.ConfigUtil.CreateBinding(endpoint.Binding, endpoint.BindingConfiguration);
			if(Endpoint.Address == null)
				Endpoint.Address = new EndpointAddress(endpoint.Address);
			if(endpoint.BehaviorConfiguration != "")
				ApplyBehavior(endpoint.BehaviorConfiguration);
			#endif
		}

		#if !NET_2_1 && !XAMMAC_4_5
		private static object CreateBehavior(System.ServiceModel.Configuration.BehaviorExtensionElement el)
		{
			return typeof(System.ServiceModel.Configuration.BehaviorExtensionElement).GetMethod("CreateBehavior", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(el, null);
		}

		private void ApplyBehavior(string behaviorConfig)
		{
			var behaviorsSection = Colosoft.ServiceModel.Configuration.ConfigUtil.BehaviorsSection;
			var behaviorElement = behaviorsSection.EndpointBehaviors[behaviorConfig];
			int i = 0;
			foreach (System.ServiceModel.Configuration.BehaviorExtensionElement el in behaviorElement)
			{
				var behavior = (System.ServiceModel.Description.IEndpointBehavior)CreateBehavior(el);
				Endpoint.Behaviors.Remove(behavior.GetType());
				Endpoint.Behaviors.Add(behavior);
			}
		}

		#endif
		protected virtual IChannelFactory CreateFactory()
		{
			bool isOneWay = true;
			foreach (var od in Endpoint.Contract.Operations)
				if(!od.IsOneWay)
				{
					isOneWay = false;
					break;
				}
			BindingParameterCollection pl = CreateBindingParameters();
			switch(Endpoint.Contract.SessionMode)
			{
			case SessionMode.Required:
				if(Endpoint.Binding.CanBuildChannelFactory<IDuplexSessionChannel>(pl))
					return Endpoint.Binding.BuildChannelFactory<IDuplexSessionChannel>(pl);
				break;
			case SessionMode.Allowed:
				if(Endpoint.Binding.CanBuildChannelFactory<IDuplexChannel>(pl))
					return Endpoint.Binding.BuildChannelFactory<IDuplexChannel>(pl);
				if(Endpoint.Binding.CanBuildChannelFactory<IDuplexSessionChannel>(pl))
					return Endpoint.Binding.BuildChannelFactory<IDuplexSessionChannel>(pl);
				break;
			default:
				if(Endpoint.Binding.CanBuildChannelFactory<IDuplexChannel>(pl))
					return Endpoint.Binding.BuildChannelFactory<IDuplexChannel>(pl);
				break;
			}
			if(Endpoint.Contract.CallbackContractType != null)
				throw new InvalidOperationException("The binding does not support duplex channel types that the contract requies for CallbackContractType.");
			if(isOneWay)
			{
				switch(Endpoint.Contract.SessionMode)
				{
				case SessionMode.Required:
					if(Endpoint.Binding.CanBuildChannelFactory<IOutputSessionChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IOutputSessionChannel>(pl);
					if(Endpoint.Binding.CanBuildChannelFactory<IDuplexSessionChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IDuplexSessionChannel>(pl);
					break;
				case SessionMode.Allowed:
					if(Endpoint.Binding.CanBuildChannelFactory<IOutputChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IOutputChannel>(pl);
					if(Endpoint.Binding.CanBuildChannelFactory<IDuplexChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IDuplexChannel>(pl);
					goto case SessionMode.Required;
				default:
					if(Endpoint.Binding.CanBuildChannelFactory<IOutputChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IOutputChannel>(pl);
					if(Endpoint.Binding.CanBuildChannelFactory<IDuplexChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IDuplexChannel>(pl);
					break;
				}
			}
			{
				switch(Endpoint.Contract.SessionMode)
				{
				case SessionMode.Required:
					if(Endpoint.Binding.CanBuildChannelFactory<IRequestSessionChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IRequestSessionChannel>(pl);
					break;
				case SessionMode.Allowed:
					if(Endpoint.Binding.CanBuildChannelFactory<IRequestChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IRequestChannel>(pl);
					if(Endpoint.Binding.CanBuildChannelFactory<IRequestSessionChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IRequestSessionChannel>(pl);
					break;
				default:
					if(Endpoint.Binding.CanBuildChannelFactory<IRequestChannel>(pl))
						return Endpoint.Binding.BuildChannelFactory<IRequestChannel>(pl);
					break;
				}
			}
			throw new InvalidOperationException(String.Format("The binding does not support any of the channel types that the contract '{0}' allows.", Endpoint.Contract.Name));
		}

		BindingParameterCollection CreateBindingParameters()
		{
			BindingParameterCollection pl = new BindingParameterCollection();
			var cd = Endpoint.Contract;
			#if !NET_2_1
			#endif
			foreach (System.ServiceModel.Description.IEndpointBehavior behavior in Endpoint.Behaviors)
				behavior.AddBindingParameters(Endpoint, pl);
			return pl;
		}

		protected abstract System.ServiceModel.Description.ServiceEndpoint CreateDescription();

		void IDisposable.Dispose()
		{
			Close();
		}

		public T GetProperty<T>() where T : class
		{
			if(OpenedChannelFactory != null)
				return OpenedChannelFactory.GetProperty<T>();
			return null;
		}

		protected void EnsureOpened()
		{
			if(Endpoint == null)
				throw new InvalidOperationException("A service endpoint must be configured for this channel factory");
			if(Endpoint.Contract == null)
				throw new InvalidOperationException("A service Contract must be configured for this channel factory");
			if(Endpoint.Binding == null)
				throw new InvalidOperationException("A Binding must be configured for this channel factory");
			if(State != CommunicationState.Opened)
				Open();
		}

		protected void InitializeEndpoint(string endpointConfigurationName, EndpointAddress remoteAddress)
		{
			InitializeEndpoint(CreateDescription());
			if(remoteAddress != null)
				service_endpoint.Address = remoteAddress;
			ApplyConfiguration(endpointConfigurationName);
		}

		protected void InitializeEndpoint(Binding binding, EndpointAddress remoteAddress)
		{
			InitializeEndpoint(CreateDescription());
			if(binding != null)
				service_endpoint.Binding = binding;
			if(remoteAddress != null)
				service_endpoint.Address = remoteAddress;
		}

		protected void InitializeEndpoint(System.ServiceModel.Description.ServiceEndpoint endpoint)
		{
			if(endpoint == null)
				throw new ArgumentNullException("endpoint");
			service_endpoint = endpoint;
		}

		protected override void OnAbort()
		{
			if(OpenedChannelFactory != null)
				OpenedChannelFactory.Abort();
		}

		Action<TimeSpan> close_delegate;

		Action<TimeSpan> open_delegate;

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(close_delegate == null)
				close_delegate = new Action<TimeSpan>(OnClose);
			return close_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			if(open_delegate == null)
				open_delegate = new Action<TimeSpan>(OnClose);
			return open_delegate.BeginInvoke(timeout, callback, state);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			if(close_delegate == null)
				throw new InvalidOperationException("Async close operation has not started");
			close_delegate.EndInvoke(result);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			if(open_delegate == null)
				throw new InvalidOperationException("Async close operation has not started");
			open_delegate.EndInvoke(result);
		}

		protected override void OnClose(TimeSpan timeout)
		{
			DateTime start = DateTime.Now;
			foreach (var ch in opened_channels.ToArray())
				ch.Close(timeout - (DateTime.Now - start));
			if(OpenedChannelFactory != null)
				OpenedChannelFactory.Close(timeout - (DateTime.Now - start));
		}

		protected override void OnOpen(TimeSpan timeout)
		{
		}

		protected override void OnOpening()
		{
			base.OnOpening();
			OpenedChannelFactory = CreateFactory();
		}

		protected override void OnOpened()
		{
			base.OnOpened();
			OpenedChannelFactory.Open();
		}
	}
}
