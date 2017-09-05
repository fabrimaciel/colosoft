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
	/// Classe responsável por gerenciar os serviços do sistema.
	/// </summary>
	public class ServiceClientsManager : IDisposable
	{
		private object _objLock = new object();

		/// <summary>
		/// Dicionário onde serão armazenados os clientes dos serviço.
		/// </summary>
		private Dictionary<string, IServiceClientsLoader> _clientLoaders = new Dictionary<string, IServiceClientsLoader>();

		private int _cacheDurationInSecondsDefault = 180;

		private static object _currentObjLock = new object();

		private static ServiceClientsManager _current;

		/// <summary>
		/// Tempo de duração do cache padrão.
		/// </summary>
		/// <remarks>Padrão: 180 segundos</remarks>
		public int CacheDurationInSecondsDefault
		{
			get
			{
				return _cacheDurationInSecondsDefault;
			}
			set
			{
				_cacheDurationInSecondsDefault = value;
			}
		}

		/// <summary>
		/// Define a atual instancia do gerenciador.
		/// </summary>
		public static ServiceClientsManager Current
		{
			get
			{
				if(_current == null)
					lock (_currentObjLock)
					{
						if(_current == null)
							_current = new ServiceClientsManager();
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
		/// Registrar um cliente para o gerenciador.
		/// </summary>
		/// <param name="key">Chave que identifica o serviço unicamente.</param>
		/// <param name="cacheDurationInSeconds">Duração em segundos que uma instancia do serviço será mantida no cache.</param>
		/// <param name="clientLoader">Loader responsável por recupera a instancia do cliente.</param>
		/// <returns>True caso o serviço tenha sido registrado ou false caso o serviço já exista.</returns>
		public bool Register(string key, int cacheDurationInSeconds, Func<System.ServiceModel.ICommunicationObject> clientLoader)
		{
			lock (_objLock)
			{
				if(!_clientLoaders.ContainsKey(key))
				{
					_clientLoaders.Add(key, new WCFServiceClientsLoader(clientLoader, cacheDurationInSeconds));
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Registrar um cliente para o gerenciador.
		/// </summary>
		/// <param name="key">Chave que identifica o serviço unicamente.</param>
		/// <param name="clientLoader">Loader responsável por recupera a instancia do cliente.</param>
		/// <returns>True caso o serviço tenha sido registrado ou false caso o serviço já exista.</returns>
		public bool Register(string key, Func<System.ServiceModel.ICommunicationObject> clientLoader)
		{
			return Register(key, _cacheDurationInSecondsDefault, clientLoader);
		}

		/// <summary>
		/// Remove o cliente registrado com a chave informada.
		/// </summary>
		/// <param name="key">Chave do cliente registrado.</param>
		/// <returns></returns>
		public bool Remove(string key)
		{
			lock (_objLock)
			{
				IServiceClientsLoader client = null;
				if(_clientLoaders.TryGetValue(key, out client))
					client.Dispose();
				return _clientLoaders.Remove(key);
			}
		}

		/// <summary>
		/// Remove todas os clientes.
		/// </summary>
		public void RemoveAll()
		{
			lock (_objLock)
			{
				foreach (var i in _clientLoaders.Select(f => f.Value))
					i.Dispose();
				_clientLoaders.Clear();
			}
		}

		/// <summary>
		/// Recupera a instancia do cliente do serviço.
		/// </summary>
		/// <param name="key">Chave do cliente registrado.</param>
		/// <returns></returns>
		public object Get(string key)
		{
			lock (_clientLoaders)
			{
				IServiceClientsLoader loader = null;
				if(_clientLoaders.TryGetValue(key, out loader))
					return loader.Instance;
				return null;
			}
		}

		/// <summary>
		/// Recupera a instacia do cliente do serviço.
		/// </summary>
		/// <typeparam name="T">Tipo do cliente do serviço.</typeparam>
		/// <param name="key">Chave que identifica o serviço.</param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			var instance = Get(key);
			return (T)instance;
		}

		/// <summary>
		/// Reseta a instancia do serviço.
		/// </summary>
		/// <param name="key"></param>
		public void Reset(string key)
		{
			lock (_clientLoaders)
			{
				IServiceClientsLoader loader = null;
				if(_clientLoaders.TryGetValue(key, out loader))
					loader.Reset();
			}
		}

		/// <summary>
		/// Reseta todos os clientes da instancia.
		/// </summary>
		public void ResetAll()
		{
			var loaders = _clientLoaders.ToArray();
			foreach (var i in loaders)
				if(i.Value != null)
					i.Value.Reset();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			lock (_objLock)
			{
				foreach (var i in _clientLoaders.Select(f => f.Value))
					i.Dispose();
				_clientLoaders.Clear();
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
