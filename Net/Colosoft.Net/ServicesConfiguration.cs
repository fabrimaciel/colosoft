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

namespace Colosoft.Net
{
	/// <summary>
	/// Armazena os dados de uma entrada da configuração dos serviços.
	/// </summary>
	public class ServicesConfigurationEntry
	{
		private string _servicesContext;

		private ServiceAddress _address;

		/// <summary>
		/// Contexto de serviços associado.
		/// </summary>
		public string ServicesContext
		{
			get
			{
				return _servicesContext;
			}
		}

		/// <summary>
		/// Endereço associado.
		/// </summary>
		public ServiceAddress Address
		{
			get
			{
				return _address;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="servicesContext"></param>
		public ServicesConfigurationEntry(ServiceAddress address, string servicesContext)
		{
			_servicesContext = servicesContext;
			_address = address;
		}
	}
	/// <summary>
	/// Classe responsável pela configuração dos serviços do sistema.
	/// </summary>
	public class ServicesConfiguration : IEnumerable<ServicesConfigurationEntry>
	{
		/// <summary>
		/// Endereços dos serviços gerenciados pelo sistema.
		/// </summary>
		private Dictionary<ServiceAddressKey, ServicesConfigurationEntry> _serviceAddress = new Dictionary<ServiceAddressKey, ServicesConfigurationEntry>(new ServiceAddressKeyEqualityComparer());

		private static object _currentObjLock = new object();

		private static ServicesConfiguration _current;

		private string _servicesContext;

		/// <summary>
		/// Evento acionado quando um novo endereço é adicionado na configuração.
		/// </summary>
		public event ServicesConfigurationActionEventHandler Added;

		/// <summary>
		/// Evento acionado quando um endereço é atualizado.
		/// </summary>
		public event ServicesConfigurationActionEventHandler Updated;

		/// <summary>
		/// Evento acionado quando um endereço é removido.
		/// </summary>
		public event ServicesConfigurationActionEventHandler Removed;

		/// <summary>
		/// Evento acionado quando o contexto dos serviços for alterado.
		/// </summary>
		public event EventHandler ServicesContextChanged;

		/// <summary>
		/// Recupera e define o endereço do serviço pelo nome informado.
		/// </summary>
		/// <param name="addressName"></param>
		/// <returns></returns>
		/// <exception cref="ServiceConfigurationException"></exception>
		public ServiceAddress this[string addressName]
		{
			get
			{
				return Get(addressName, this.ServicesContext);
			}
			set
			{
				var key = new ServiceAddressKey(addressName, this.ServicesContext);
				_serviceAddress.Remove(key);
				if(value != null)
				{
					var entry = new ServicesConfigurationEntry(value, this.ServicesContext);
					_serviceAddress.Add(key, entry);
					OnUpdated(addressName, entry);
				}
			}
		}

		/// <summary>
		/// Recupera o endereço do serviço pelo nome informado.
		/// </summary>
		/// <param name="addressName"></param>
		/// <param name="servicesContext"></param>
		/// <returns></returns>
		public ServiceAddress this[string addressName, string servicesContext]
		{
			get
			{
				return Get(addressName, servicesContext);
			}
			set
			{
				var key = new ServiceAddressKey(addressName, servicesContext);
				_serviceAddress.Remove(key);
				if(value != null)
				{
					var entry = new ServicesConfigurationEntry(value, this.ServicesContext);
					_serviceAddress.Add(key, entry);
					OnUpdated(addressName, entry);
				}
			}
		}

		/// <summary>
		/// Quantidade de endereços 
		/// </summary>
		public int Count
		{
			get
			{
				return _serviceAddress.Count;
			}
		}

		/// <summary>
		/// Nome do contexto de serviços.
		/// </summary>
		public string ServicesContext
		{
			get
			{
				return _servicesContext;
			}
			set
			{
				if(_servicesContext != value)
				{
					var addresses = _serviceAddress.Where(f => f.Key.ServicesContext == _servicesContext).ToArray();
					foreach (var i in addresses)
					{
						if(_servicesContext != null)
							Remove(i.Key.Name, i.Key.ServicesContext);
						OnUpdated(i.Key.Name, i.Value);
					}
					_servicesContext = value;
					if(ServicesContextChanged != null)
						ServicesContextChanged(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Define a atual instancia do gerenciador.
		/// </summary>
		public static ServicesConfiguration Current
		{
			get
			{
				if(_current == null)
					lock (_currentObjLock)
					{
						if(_current == null)
							_current = new ServicesConfiguration();
					}
				return _current;
			}
			set
			{
				lock (_currentObjLock)
					_current = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		private ServicesConfiguration()
		{
		}

		/// <summary>
		/// Recupera a instancia do provedor de endereço dos serviços.
		/// </summary>
		/// <returns></returns>
		private IServiceAddressProvider GetServiceAddressProvider()
		{
			Colosoft.ServiceLocatorValidator.Validate();
			return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IServiceAddressProvider>();
		}

		/// <summary>
		/// Recupera o endereço pelo nome informado.
		/// </summary>
		/// <param name="addressName"></param>
		/// <param name="servicesContext">Contexto de serviços.</param>
		/// <returns></returns>
		private ServiceAddress Get(string addressName, string servicesContext)
		{
			ServicesConfigurationEntry result = null;
			var key = new ServiceAddressKey(addressName, servicesContext);
			lock (_serviceAddress)
				if(_serviceAddress.TryGetValue(key, out result))
					return result.Address;
			var addressProvider = GetServiceAddressProvider();
			if(addressName == addressProvider.ProviderAddressName)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.ImpossibleRecoverAddressProviderAddress, addressName).Format());
			if(addressProvider != null)
			{
				var addresses = addressProvider.GetServiceAddresses(addressName, servicesContext);
				if(addresses != null && addresses.Length > 0)
					Add(addresses[new Random().Next(0, addresses.Length - 1)], servicesContext);
				else
					throw new ServiceConfigurationException(string.Format(Properties.Resources.ServiceConfiguration_AddressNotFoundInAddressProvider, addressName, servicesContext));
				lock (_serviceAddress)
					if(_serviceAddress.TryGetValue(key, out result))
						return result.Address;
			}
			throw new ServiceConfigurationException(string.Format(Properties.Resources.ServiceConfiguration_AddressNotFound, addressName));
		}

		/// <summary>
		/// Método acioando quando um novo endereço for adicionado.
		/// </summary>
		/// <param name="serviceName"></param>
		/// <param name="entry">Dados da entrada adicionada.</param>
		private void OnAdded(string serviceName, ServicesConfigurationEntry entry)
		{
			if(Added != null)
				Added(this, new ServicesConfigurationActionEventArgs(serviceName, entry.Address, entry.ServicesContext));
		}

		/// <summary>
		/// Método acionado quando um endereço for atualizado.
		/// </summary>
		/// <param name="serviceName"></param>
		/// <param name="entry">Entrada atualizada.</param>
		private void OnUpdated(string serviceName, ServicesConfigurationEntry entry)
		{
			if(Updated != null)
				Updated(this, new ServicesConfigurationActionEventArgs(serviceName, entry.Address, entry.ServicesContext));
		}

		/// <summary>
		/// Método acionado quando um endereço for removido.
		/// </summary>
		/// <param name="serviceName"></param>
		/// <param name="entry"></param>
		private void OnRemoved(string serviceName, ServicesConfigurationEntry entry)
		{
			if(Removed != null)
				Removed(this, new ServicesConfigurationActionEventArgs(serviceName, entry.Address, entry.ServicesContext));
		}

		/// <summary>
		/// Adiciona um novo endereço de serviço para a configuração do sistema.
		/// </summary>
		/// <param name="address">Endereço que será adicionado.</param>
		/// <param name="servicesContext">Contexto de serviços associado.</param>
		/// <returns></returns>
		public bool Add(ServiceAddress address, string servicesContext)
		{
			return Add(address, servicesContext, false);
		}

		/// <summary>
		/// Adiciona um novo endereço de serviço para a configuração do sistema.
		/// </summary>
		/// <param name="address">Endereço que será adicionado.</param>
		/// <param name="servicesContext">Contexto de serviços associado.</param>
		/// <param name="ignoreReset">Identifica se é para ignorar o reset do endereço.</param>
		/// <returns></returns>
		public bool Add(ServiceAddress address, string servicesContext, bool ignoreReset)
		{
			address.Require("address").NotNull();
			var key = new ServiceAddressKey(address.Name, servicesContext, ignoreReset);
			ServicesConfigurationEntry entry = null;
			lock (_serviceAddress)
			{
				if(_serviceAddress.ContainsKey(key))
					return false;
				entry = new ServicesConfigurationEntry(address, servicesContext);
				_serviceAddress.Add(key, entry);
			}
			OnAdded(address.Name, entry);
			return true;
		}

		/// <summary>
		/// Adiciona um novo endereço de serviço para a configuração do sistema.
		/// </summary>
		/// <param name="address"></param>
		/// <returns>True caso o endereço tenha sido inserido com sucesso.</returns>
		public bool Add(ServiceAddress address)
		{
			return this.Add(address, this.ServicesContext);
		}

		/// <summary>
		/// Atualiza os dados do endereço de serviço informado.
		/// </summary>
		/// <param name="address">Instacia do endereço que será atualizada.</param>
		/// <param name="servicesContext">Contexto de serviços.</param>
		/// <returns>True caso o endereço tenha sido atualizado.</returns>
		public bool Update(ServiceAddress address, string servicesContext)
		{
			address.Require("address").NotNull();
			var key = new ServiceAddressKey(address.Name, servicesContext);
			ServicesConfigurationEntry entry = null;
			lock (_serviceAddress)
			{
				if(_serviceAddress.ContainsKey(key))
				{
					_serviceAddress.Remove(key);
					entry = new ServicesConfigurationEntry(address, servicesContext);
					_serviceAddress.Add(key, entry);
				}
			}
			if(entry != null)
			{
				OnUpdated(key.Name, entry);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Atualiza os dados do endereço de serviço informado.
		/// </summary>
		/// <param name="address">Instacia do endereço que será atualizada.</param>
		/// <returns>True caso o endereço tenha sido atualizado.</returns>
		public bool Update(ServiceAddress address)
		{
			return Update(address, this.ServicesContext);
		}

		/// <summary>
		/// Remove o endereço da configuração.
		/// </summary>
		/// <param name="addressName">Nome do endereço registrado.</param>
		/// <param name="servicesContext">Contexto de serviços.</param>
		/// <returns>True caso o endereço tenha sido removido com sucesso.</returns>
		public bool Remove(string addressName, string servicesContext)
		{
			var key = new ServiceAddressKey(addressName, servicesContext);
			ServicesConfigurationEntry entry = null;
			var contains = false;
			lock (_serviceAddress)
				if(_serviceAddress.TryGetValue(key, out entry))
					_serviceAddress.Remove(key);
			if(contains)
				OnRemoved(addressName, entry);
			return contains;
		}

		/// <summary>
		/// Remove o endereço da configuração.
		/// </summary>
		/// <param name="addressName">Nome do endereço registrado.</param>
		/// <returns>True caso o endereço tenha sido removido com sucesso.</returns>
		public bool Remove(string addressName)
		{
			return Remove(addressName, this.ServicesContext);
		}

		/// <summary>
		/// Verifica se na configuração existe o endereço com o nome informado.
		/// </summary>
		/// <param name="addressName">Nome do endereço</param>
		/// <returns></returns>
		public bool Contains(string addressName)
		{
			return Contains(addressName, true);
		}

		/// <summary>
		/// Verifica se na configuração existe o endereço com o nome informado.
		/// </summary>
		/// <param name="addressName">Nome do endereço</param>
		/// <param name="servicesContext">Contexto dos serviços.</param>
		/// <param name="useAddressProvider">Identifica se é para verifica pelo AddressProvider</param>
		/// <returns></returns>
		public bool Contains(string addressName, string servicesContext, bool useAddressProvider)
		{
			var key = new ServiceAddressKey(addressName, servicesContext);
			lock (_serviceAddress)
				if(_serviceAddress.ContainsKey(key))
					return true;
			if(!useAddressProvider)
				return false;
			var addressProvider = GetServiceAddressProvider();
			if(addressProvider != null)
			{
				var addresses = addressProvider.GetServiceAddresses(addressName, servicesContext);
				if(addresses != null && addresses.Length > 0)
				{
					Add(addresses[new Random().Next(0, addresses.Length - 1)], servicesContext);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Verifica se na configuração existe o endereço com o nome informado.
		/// </summary>
		/// <param name="addressName">Nome do endereço</param>
		/// <param name="useAddressProvider">Identifica se é para verifica pelo AddressProvider</param>
		/// <returns></returns>
		public bool Contains(string addressName, bool useAddressProvider)
		{
			return Contains(addressName, this.ServicesContext, useAddressProvider);
		}

		/// <summary>
		/// Reseta as configurações.
		/// </summary>
		public void Reset()
		{
			KeyValuePair<ServiceAddressKey, ServicesConfigurationEntry>[] addresses = null;
			lock (_serviceAddress)
				addresses = _serviceAddress.ToArray();
			foreach (var i in addresses)
			{
				if(!i.Key.IgnoreReset)
				{
					Remove(i.Key.Name, i.Key.ServicesContext);
					OnUpdated(i.Key.Name, i.Value);
				}
			}
		}

		/// <summary>
		/// Recupera o enumerador para pecorrer os endereços dos serviços.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ServicesConfigurationEntry> GetEnumerator()
		{
			return _serviceAddress.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _serviceAddress.Values.GetEnumerator();
		}

		/// <summary>
		/// Representa a chave de um endereço.
		/// </summary>
		class ServiceAddressKey
		{
			/// <summary>
			/// Nome do serviço.
			/// </summary>
			public string Name;

			/// <summary>
			/// Contexto de serviços associado.
			/// </summary>
			public string ServicesContext;

			/// <summary>
			/// Identifica se é para ignorar o reset para essa chave.
			/// </summary>
			public bool IgnoreReset
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="servicesContext"></param>
			/// ?<param name="ignoreReset"></param>
			public ServiceAddressKey(string name, string servicesContext, bool ignoreReset = false)
			{
				Name = name;
				ServicesContext = servicesContext;
				IgnoreReset = ignoreReset;
			}
		}

		/// <summary>
		/// Implementação do comparador de chaves.
		/// </summary>
		class ServiceAddressKeyEqualityComparer : IEqualityComparer<ServiceAddressKey>
		{
			/// <summary>
			/// Compara as chaves informadas.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(ServiceAddressKey x, ServiceAddressKey y)
			{
				return (x == null || y == null) || (x != null && y != null && x.Name == y.Name && x.ServicesContext == y.ServicesContext);
			}

			/// <summary>
			/// Recupera o hashcode da chave.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(ServiceAddressKey obj)
			{
				if(obj == null)
					return 0;
				return string.Format("{0}:{1}", obj.Name, obj.ServicesContext).GetHashCode();
			}
		}
	}
}
