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

namespace Colosoft.ServiceModel
{
	public class ChannelFactory<TChannel> : ChannelFactory, System.ServiceModel.Channels.IChannelFactory<TChannel>
	{
		public ChannelFactory()
		{
		}

		protected ChannelFactory(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(!type.IsInterface)
				throw new InvalidOperationException("The type argument to the generic ChannelFactory constructor must be an interface type.");
			InitializeEndpoint(CreateDescription());
		}

		public ChannelFactory(string endpointConfigurationName)
		{
			if(endpointConfigurationName == null)
				throw new ArgumentNullException("endpointConfigurationName");
			InitializeEndpoint(endpointConfigurationName, null);
		}

		public ChannelFactory(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
		{
			if(endpointConfigurationName == null)
				throw new ArgumentNullException("endpointConfigurationName");
			InitializeEndpoint(endpointConfigurationName, remoteAddress);
		}

		public ChannelFactory(System.ServiceModel.Description.ServiceEndpoint endpoint)
		{
			if(endpoint == null)
				throw new ArgumentNullException("serviceEndpoint");
			InitializeEndpoint(endpoint);
		}

		public ChannelFactory(System.ServiceModel.Channels.Binding binding, string remoteAddress) : this(binding, new System.ServiceModel.EndpointAddress(remoteAddress))
		{
		}

		public ChannelFactory(System.ServiceModel.Channels.Binding binding) : this(binding, (System.ServiceModel.EndpointAddress)null)
		{
		}

		public ChannelFactory(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : this(typeof(TChannel))
		{
			if(binding == null)
				throw new ArgumentNullException();
			Endpoint.Binding = binding;
			Endpoint.Address = remoteAddress;
		}

		internal object OwnerClientBase
		{
			get;
			set;
		}

		public TChannel CreateChannel()
		{
			EnsureOpened();
			return CreateChannel(Endpoint.Address);
		}

		public TChannel CreateChannel(System.ServiceModel.EndpointAddress address)
		{
			return CreateChannel(address, null);
		}

		static TChannel CreateChannelCore(ChannelFactory<TChannel> cf, Func<ChannelFactory<TChannel>, TChannel> f)
		{
			var ch = f(cf);
			((System.ServiceModel.ICommunicationObject)(object)ch).Closed += delegate {
				if(cf.State == System.ServiceModel.CommunicationState.Opened)
					cf.Close();
			};
			return ch;
		}

		public static TChannel CreateChannel(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address)
		{
			return CreateChannelCore(new ChannelFactory<TChannel>(binding, address), f => f.CreateChannel());
		}

		public static TChannel CreateChannel(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address, Uri via)
		{
			return CreateChannelCore(new ChannelFactory<TChannel>(binding), f => f.CreateChannel(address, via));
		}

		public virtual TChannel CreateChannel(System.ServiceModel.EndpointAddress address, Uri via)
		{
			#if FULL_AOT_RUNTIME
						throw new InvalidOperationException ("MonoTouch does not support dynamic proxy code generation. Override this method or its caller to return specific client proxy instance");
#else
			var existing = Endpoint.Address;
			try
			{
				Endpoint.Address = address;
				EnsureOpened();
				#if DISABLE_REAL_PROXY
							Type type = ClientProxyGenerator.CreateProxyType (typeof (TChannel), Endpoint.Contract, false);
			// in .NET and SL2, it seems that the proxy is RealProxy.
			// But since there is no remoting in SL2 (and we have
			// no special magic), we have to use different approach
			// that should work either.
			var proxy = (IClientChannel) Activator.CreateInstance (type, new object [] {Endpoint, this, address ?? Endpoint.Address, via});
#else
				var proxy = (System.ServiceModel.IClientChannel)new ClientRealProxy(typeof(TChannel), new ClientRuntimeChannel(Endpoint, this, address ?? Endpoint.Address, via), false).GetTransparentProxy();
				#endif
				proxy.Opened += delegate {
					OpenedChannels.Add(proxy);
				};
				proxy.Closing += delegate {
					OpenedChannels.Remove(proxy);
				};
				return (TChannel)proxy;
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				if(ex.InnerException != null)
					throw ex.InnerException;
				else
					throw;
			}
			finally
			{
				Endpoint.Address = existing;
			}
			#endif
		}

		protected static TChannel CreateChannel(string endpointConfigurationName)
		{
			return CreateChannelCore(new ChannelFactory<TChannel>(endpointConfigurationName), f => f.CreateChannel());
		}

		protected override System.ServiceModel.Description.ServiceEndpoint CreateDescription()
		{
			var cd = System.ServiceModel.Description.ContractDescription.GetContract(typeof(TChannel));
			var ep = new System.ServiceModel.Description.ServiceEndpoint(cd);
			ep.Behaviors.Add(new System.ServiceModel.Description.ClientCredentials());
			return ep;
		}
	}
}
