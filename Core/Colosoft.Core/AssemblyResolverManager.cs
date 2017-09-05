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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Implementação do assembly resolver.
	/// </summary>
	public class AssemblyResolverManager : IEnumerable<IAssemblyResolver>, IDisposable
	{
		private AppDomain _appDomain;

		/// <summary>
		/// Relação dos assemblies carregados.
		/// </summary>
		private Dictionary<string, System.Reflection.Assembly> _assemblies = new Dictionary<string, System.Reflection.Assembly>(StringComparer.InvariantCultureIgnoreCase);

		private List<IAssemblyResolver> _resolvers = new List<IAssemblyResolver>();

		private object _objLock = new object();

		/// <summary>
		/// Armazena a relação dos assemblies carregados
		/// </summary>
		private List<string> _loadedAssemblies;

		/// <summary>
		/// Quantidade de itens no gerenciador.
		/// </summary>
		public int Count
		{
			get
			{
				return _resolvers.Count;
			}
		}

		/// <summary>
		/// Recupera o item pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IAssemblyResolver this[int index]
		{
			get
			{
				return _resolvers[index];
			}
		}

		/// <summary>
		/// Instancia do domínio da aplicação associado.
		/// </summary>
		public AppDomain AppDomain
		{
			get
			{
				return _appDomain;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="appDomain">Domínio da aplicação que será utilizado.</param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, ControlAppDomain = true)]
		public AssemblyResolverManager(AppDomain appDomain)
		{
			appDomain.Require("appDomain").NotNull();
			_appDomain = appDomain;
			_appDomain.AssemblyResolve += AppDomainAssemblyResolve;
			_appDomain.AssemblyLoad += AppDomainAssemblyLoad;
			InitializeAssemblies();
		}

		/// <summary>
		/// Inicializa os assemblies.
		/// </summary>
		private void InitializeAssemblies()
		{
			_loadedAssemblies = _appDomain.GetAssemblies().Select(f => f.GetName().Name).OrderBy(f => f).ToList();
		}

		/// <summary>
		/// Método acionado quando for necessário a resolução de um assembly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private System.Reflection.Assembly AppDomainAssemblyResolve(object sender, ResolveEventArgs args)
		{
			System.Reflection.Assembly assembly = null;
			var name = new System.Reflection.AssemblyName(args.Name);
			var assemblyName = name.Name.GetAssemblyNameWithoutExtension();
			lock (_assemblies)
				if(_assemblies.TryGetValue(assemblyName, out assembly))
					return assembly;
			Exception lastException = null;
			foreach (var i in _resolvers.ToArray())
			{
				Exception error = null;
				try
				{
					if(i.IsValid && i.Resolve(args, out assembly, out error))
					{
						lock (_assemblies)
							if(!_assemblies.ContainsKey(assemblyName))
								_assemblies.Add(assemblyName, assembly);
							else
								return _assemblies[assemblyName];
						return assembly;
					}
				}
				catch(Exception ex)
				{
					error = ex;
				}
				if(error != null)
					lastException = error;
			}
			if(lastException != null)
				throw lastException;
			return null;
		}

		/// <summary>
		/// Método acionado quando um assembly é carregado.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AppDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			var assemblyName = args.LoadedAssembly.GetName().Name;
			lock (_loadedAssemblies)
			{
				var index = _loadedAssemblies.BinarySearch(assemblyName);
				if(index < 0)
					_loadedAssemblies.Insert(~index, assemblyName);
			}
			lock (_assemblies)
				if(!_assemblies.ContainsKey(assemblyName))
					_assemblies.Add(assemblyName, args.LoadedAssembly);
		}

		/// <summary>
		/// Método acionado quando o resolver do assembly for carregado
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AssemblyResolverExtLoaded(object sender, AssemblyResolverLoadEventArgs e)
		{
			lock (_assemblies)
				foreach (var i in e.Result)
					if(i.Assembly != null)
					{
						var name = i.Assembly.GetName().Name;
						if(!_assemblies.ContainsKey(name))
							_assemblies.Add(name, i.Assembly);
					}
		}

		/// <summary>
		/// Verifica se o assembly associado com o nome, já foi carregado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public bool CheckAssembly(string assemblyName)
		{
			lock (_loadedAssemblies)
				return _loadedAssemblies.BinarySearch(assemblyName, StringComparer.InvariantCultureIgnoreCase) >= 0;
		}

		/// <summary>
		/// Adiciona um novo resolvedor na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="resolver"></param>
		public void Insert(int index, IAssemblyResolver resolver)
		{
			resolver.Require("resolver").NotNull();
			lock (_objLock)
			{
				if(resolver is IAssemblyResolverExt)
					((IAssemblyResolverExt)resolver).Loaded += AssemblyResolverExtLoaded;
				_resolvers.Insert(index, resolver);
			}
		}

		/// <summary>
		/// Adiciona um novo resolvedor para o gerenciador.
		/// </summary>
		/// <param name="resolver"></param>
		public void Add(IAssemblyResolver resolver)
		{
			resolver.Require("resolver").NotNull();
			lock (_objLock)
			{
				if(resolver is IAssemblyResolverExt)
					((IAssemblyResolverExt)resolver).Loaded += AssemblyResolverExtLoaded;
				_resolvers.Add(resolver);
			}
		}

		/// <summary>
		/// Remove a instancia do resolvedor do gerenciador.
		/// </summary>
		/// <param name="resolver"></param>
		public void Remove(IAssemblyResolver resolver)
		{
			resolver.Require("resolver").NotNull();
			lock (_objLock)
			{
				if(resolver is IAssemblyResolverExt)
					((IAssemblyResolverExt)resolver).Loaded -= AssemblyResolverExtLoaded;
				_resolvers.Remove(resolver);
			}
		}

		/// <summary>
		/// Remove todos items do gerenciador.
		/// </summary>
		public void Clear()
		{
			lock (_objLock)
			{
				foreach (var i in _resolvers)
					if(i is IAssemblyResolverExt)
						((IAssemblyResolverExt)i).Loaded -= AssemblyResolverExtLoaded;
				_resolvers.Clear();
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, ControlAppDomain = true)]
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, ControlAppDomain = true)]
		protected virtual void Dispose(bool disposing)
		{
			if(_appDomain != null)
			{
				_appDomain.AssemblyResolve -= AppDomainAssemblyResolve;
				_appDomain.AssemblyLoad -= AppDomainAssemblyLoad;
				_appDomain = null;
				_assemblies.Clear();
			}
			Clear();
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IAssemblyResolver> GetEnumerator()
		{
			return _resolvers.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _resolvers.GetEnumerator();
		}
	}
}
