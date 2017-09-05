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
using System.Reflection;
using System.Collections;
using Colosoft.Caching.Exceptions;
using System.IO;
using System.Threading;
using Colosoft.Logging;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Gerenciador dos provedores de leitura.
	/// </summary>
	internal class ReadThruProviderManager : IDisposable
	{
		private string _cacheName;

		private CacheRuntimeContext _context;

		private IReadThruProvider _provider;

		/// <summary>
		/// Identificador do cache.
		/// </summary>
		public string CacheId
		{
			get
			{
				return _cacheName;
			}
		}

		/// <summary>
		/// Instancia do <see cref="ILogger"/>.
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return _context.Logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ReadThruProviderManager()
		{
		}

		/// <summary>
		/// Cria a instancia com os parametros iniciais.
		/// </summary>
		/// <param name="cacheName">Nome do cache.</param>
		/// <param name="properties">Propriedades de configuração.</param>
		/// <param name="context">Contexto de execução.</param>
		public ReadThruProviderManager(string cacheName, IDictionary properties, CacheRuntimeContext context)
		{
			_cacheName = cacheName;
			_context = context;
			this.Initialize(properties);
		}

		/// <summary>
		/// Recupera o caminho do assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		private string GetReadThruAssemblyPath(string assembly)
		{
			string str = @"\";
			string[] strArray = assembly.Split(new char[] {
				',',
				'='
			});
			return (str + strArray[0]);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties">Propriedades de configuração.</param>
		private void Initialize(IDictionary properties)
		{
			properties.Require("properties").NotNull();
			Assembly assembly = null;
			try
			{
				if(!properties.Contains("assembly-name"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_MissingAssemblyNameForReadThru).Format());
				if(!properties.Contains("class-name"))
					throw new ConfigurationException(ResourceMessageFormatter.Create(() => Properties.Resources.ConfigurationException_MissingClassNameForReadThru).Format());
				string assemblyName = Convert.ToString(properties["assembly-name"]);
				string typeName = Convert.ToString(properties["class-name"]);
				string extension = ".dll";
				if(properties.Contains("full-name"))
					extension = Path.GetExtension(Convert.ToString(properties["full-name"]));
				var parameters = (properties["parameters"] as IDictionary) ?? new Hashtable();
				if(extension.EndsWith(".dll") || extension.EndsWith(".exe"))
				{
					try
					{
						string assemblyFile = CachingUtils.DeployedAssemblyDir + _cacheName + this.GetReadThruAssemblyPath(assemblyName) + extension;
						try
						{
							assembly = Assembly.LoadFrom(assemblyFile);
						}
						catch(Exception exception)
						{
							throw new Exception(string.Format("Could not load assembly \"" + assemblyFile + "\". {0}", exception.Message));
						}
						if(assembly != null)
							_provider = (IReadThruProvider)assembly.CreateInstance(typeName);
						if(_provider == null)
							throw new Exception("Unable to instantiate " + typeName);
						_provider.Start(parameters);
						return;
					}
					catch(InvalidCastException)
					{
						throw new ConfigurationException("The class specified in read-thru does not implement IDatasourceReader");
					}
					catch(Exception exception2)
					{
						throw new ConfigurationException(exception2.Message, exception2);
					}
				}
			}
			catch(ConfigurationException)
			{
				throw;
			}
			catch(Exception exception3)
			{
				throw new ConfigurationException(exception3.GetFormatter().Format(), exception3);
			}
		}

		/// <summary>
		/// Carrega os itens com base nas chaves.
		/// </summary>
		/// <param name="keys">Chaves do itens que serão carregados.</param>
		/// <returns>Dicionário dos itens encontrados.</returns>
		public Dictionary<string, ProviderCacheItem> ReadThru(string[] keys)
		{
			Dictionary<string, ProviderCacheItem> dictionary = null;
			try
			{
				dictionary = _provider.LoadFromSource(keys);
			}
			catch(Exception exception)
			{
				throw new OperationFailedException(ResourceMessageFormatter.Create(() => Properties.Resources.OperationFailedException_IReadThruProviderLoadFromSourceFailed).Format(), exception);
			}
			return dictionary;
		}

		/// <summary>
		/// Lê um item com base na chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="item"></param>
		public void ReadThru(string key, out ProviderCacheItem item)
		{
			item = null;
			try
			{
				_provider.LoadFromSource(key, out item);
			}
			catch(Exception exception)
			{
				throw new OperationFailedException(ResourceMessageFormatter.Create(() => Properties.Resources.OperationFailedException_IReadThruProviderLoadFromSourceFailed).Format(), exception);
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		void IDisposable.Dispose()
		{
			if(_provider != null)
			{
				IReadThruProvider provider;
				Monitor.Enter(provider = _provider);
				try
				{
					_provider.Stop();
				}
				catch(Exception exception)
				{
					this.Logger.Error(("ReadThruProviderMgr: User code threw " + exception.GetType().Name).GetFormatter());
				}
				finally
				{
					Monitor.Exit(provider);
				}
				_provider = null;
			}
		}
	}
}
