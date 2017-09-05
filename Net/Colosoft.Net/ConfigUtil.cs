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
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Web.Configuration;

namespace Colosoft.ServiceModel.Configuration
{
	internal static class ConfigUtil
	{
		#if !XAMMAC_4_5
		static object GetSection(string name)
		{
			if(ServiceHostingEnvironment.InAspNet)
				return WebConfigurationManager.GetSection(name);
			else
				return System.Configuration.ConfigurationManager.GetSection(name);
		}

		public static BindingsSection BindingsSection
		{
			get
			{
				return (BindingsSection)GetSection("system.serviceModel/bindings");
			}
		}

		public static ClientSection ClientSection
		{
			get
			{
				return (ClientSection)GetSection("system.serviceModel/client");
			}
		}

		public static ServicesSection ServicesSection
		{
			get
			{
				return (ServicesSection)GetSection("system.serviceModel/services");
			}
		}

		public static BehaviorsSection BehaviorsSection
		{
			get
			{
				return (BehaviorsSection)GetSection("system.serviceModel/behaviors");
			}
		}

		public static DiagnosticSection DiagnosticSection
		{
			get
			{
				return (DiagnosticSection)GetSection("system.serviceModel/diagnostics");
			}
		}

		public static ExtensionsSection ExtensionsSection
		{
			get
			{
				return (ExtensionsSection)GetSection("system.serviceModel/extensions");
			}
		}

		public static ProtocolMappingSection ProtocolMappingSection
		{
			get
			{
				return (ProtocolMappingSection)GetSection("system.serviceModel/protocolMapping");
			}
		}

		public static StandardEndpointsSection StandardEndpointsSection
		{
			get
			{
				return (StandardEndpointsSection)GetSection("system.serviceModel/standardEndpoints");
			}
		}

		static readonly List<Assembly> cached_assemblies = new List<Assembly>();

		static readonly List<NamedConfigType> cached_named_config_types = new List<NamedConfigType>();

		public static Type GetTypeFromConfigString(string name, NamedConfigCategory category)
		{
			Type type = Type.GetType(name);
			if(type != null)
				return type;
			foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
			{
				var cache = cached_named_config_types.FirstOrDefault(c => c.Name == name && c.Category == category);
				if(cache != null)
					return cache.Type;
				if((type = ass.GetType(name)) != null)
					return type;
				if(cached_assemblies.Contains(ass))
					continue;
				if(!ass.IsDynamic)
					cached_assemblies.Add(ass);
				foreach (var t in ass.GetTypes())
				{
					if(cached_named_config_types.Any(ct => ct.Type == t))
						continue;
					NamedConfigType c = null;
					var sca = t.GetCustomAttributes(typeof(ServiceContractAttribute), false).Select(f => (ServiceContractAttribute)f).FirstOrDefault();
					if(sca != null && !String.IsNullOrEmpty(sca.ConfigurationName))
					{
						c = new NamedConfigType() {
							Category = NamedConfigCategory.Contract,
							Name = sca.ConfigurationName,
							Type = t
						};
						cached_named_config_types.Add(c);
					}
					if(c != null && c.Name == name && c.Category == category)
						cache = c;
				}
				if(cache != null)
					return cache.Type;
			}
			return null;
		}

		public static EndpointAddress CreateInstance(this EndpointAddressElementBase el)
		{
			return new EndpointAddress(el.Address, el.Identity.CreateInstance(), el.Headers.Headers);
		}

		public static void CopyFrom(this ChannelEndpointElement to, ChannelEndpointElement from)
		{
			to.Address = from.Address;
			to.BehaviorConfiguration = from.BehaviorConfiguration;
			to.Binding = from.Binding;
			to.BindingConfiguration = from.BindingConfiguration;
			to.Contract = from.Contract;
			if(from.Headers != null)
				to.Headers.Headers = from.Headers.Headers;
			if(from.Identity != null)
				to.Identity.InitializeFrom(from.Identity.CreateInstance());
			to.Name = from.Name;
		}

		public static EndpointAddress CreateEndpointAddress(this ChannelEndpointElement el)
		{
			return new EndpointAddress(el.Address, el.Identity != null ? el.Identity.CreateInstance() : null, el.Headers.Headers);
		}

		public static EndpointAddress CreateEndpointAddress(this ServiceEndpointElement el)
		{
			return new EndpointAddress(el.Address, el.Identity != null ? el.Identity.CreateInstance() : null, el.Headers.Headers);
		}

		#endif
		public static EndpointIdentity CreateInstance(this IdentityElement el)
		{
			if(el.Certificate != null)
				return new X509CertificateEndpointIdentity(el.Certificate.CreateInstance());
			else if(el.CertificateReference != null)
				return new X509CertificateEndpointIdentity(el.CertificateReference.CreateInstance());
			else if(el.Dns != null)
				return new DnsEndpointIdentity(el.Dns.Value);
			else if(el.Rsa != null)
				return new RsaEndpointIdentity(el.Rsa.Value);
			else if(el.ServicePrincipalName != null)
				return new SpnEndpointIdentity(el.ServicePrincipalName.Value);
			else if(el.UserPrincipalName != null)
				return new UpnEndpointIdentity(el.UserPrincipalName.Value);
			else
				return null;
		}

		public static X509Certificate2 CreateCertificateFrom(StoreLocation storeLocation, StoreName storeName, X509FindType findType, Object findValue)
		{
			var store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
			try
			{
				foreach (var c in store.Certificates.Find(findType, findValue, false))
					return c;
				throw new InvalidOperationException(String.Format("Specified X509 certificate with find type {0} and find value {1} was not found in X509 store {2} location {3}", findType, findValue, storeName, storeLocation));
			}
			finally
			{
				store.Close();
			}
		}

		public static X509Certificate2 CreateInstance(this CertificateElement el)
		{
			return new X509Certificate2(Convert.FromBase64String(el.EncodedValue));
		}

		public static X509Certificate2 CreateInstance(this CertificateReferenceElement el)
		{
			return CreateCertificateFrom(el.StoreLocation, el.StoreName, el.X509FindType, el.FindValue);
		}

		public static X509Certificate2 CreateInstance(this X509ClientCertificateCredentialsElement el)
		{
			return CreateCertificateFrom(el.StoreLocation, el.StoreName, el.X509FindType, el.FindValue);
		}

		public static X509Certificate2 CreateInstance(this X509ScopedServiceCertificateElement el)
		{
			return CreateCertificateFrom(el.StoreLocation, el.StoreName, el.X509FindType, el.FindValue);
		}

		public static X509Certificate2 CreateInstance(this X509DefaultServiceCertificateElement el)
		{
			return CreateCertificateFrom(el.StoreLocation, el.StoreName, el.X509FindType, el.FindValue);
		}

		private static Binding GetDefault(BindingCollectionElement session)
		{
			return (Binding)typeof(BindingCollectionElement).GetMethod("GetDefault", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(session, null);
		}

		public static Binding CreateBinding(string binding, string bindingConfiguration)
		{
			BindingCollectionElement section = ConfigUtil.BindingsSection[binding];
			if(section == null)
				throw new ArgumentException(String.Format("binding section for {0} was not found.", binding));
			Binding b = GetDefault(section);
			foreach (IBindingConfigurationElement el in section.ConfiguredBindings)
				if(el.Name == bindingConfiguration)
					el.ApplyConfiguration(b);
			return b;
		}

		public static Binding GetBindingByProtocolMapping(Uri address)
		{
			ProtocolMappingElement el = ConfigUtil.ProtocolMappingSection.ProtocolMappingCollection[address.Scheme];
			if(el == null)
				return null;
			return ConfigUtil.CreateBinding(el.Binding, el.BindingConfiguration);
		}

		private static StandardEndpointElement GetDefaultStandardEndpointElement(EndpointCollectionElement section)
		{
			return (StandardEndpointElement)typeof(EndpointCollectionElement).GetMethod("GetDefaultStandardEndpointElement", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(section, null);
		}

		private static ServiceEndpoint CreateServiceEndpoint(StandardEndpointElement e, ContractDescription cd)
		{
			return (ServiceEndpoint)typeof(ContractDescription).GetMethod("CreateServiceEndpoint", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(e, new object[] {
				cd
			});
		}

		public static ServiceEndpoint ConfigureStandardEndpoint(ContractDescription cd, ChannelEndpointElement element)
		{
			string kind = element.Kind;
			string endpointConfiguration = element.EndpointConfiguration;
			EndpointCollectionElement section = ConfigUtil.StandardEndpointsSection[kind];
			if(section == null)
				throw new ArgumentException(String.Format("standard endpoint section for '{0}' was not found.", kind));
			StandardEndpointElement e = GetDefaultStandardEndpointElement(section);
			ServiceEndpoint inst = CreateServiceEndpoint(e, cd);
			foreach (StandardEndpointElement el in section.ConfiguredEndpoints)
			{
				if(el.Name == endpointConfiguration)
				{
					el.InitializeAndValidate(element);
					el.ApplyConfiguration(inst, element);
					break;
				}
			}
			return inst;
		}
	}
	enum NamedConfigCategory
	{
		None,
		Contract
	}
	class NamedConfigType
	{
		public NamedConfigCategory Category
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public Type Type
		{
			get;
			set;
		}
	}
}
