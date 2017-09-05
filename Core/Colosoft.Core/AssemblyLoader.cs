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
	/// Classe responsável por fazer a carga de assemblies.
	/// </summary>
	public class AssemblyLoader : IAssemblyLoader
	{
		private IAssemblyRepository _assemblyRepository;

		private AppDomain _domain;

		private static IAssemblyLoader _instance;

		private static object _objLock = new object();

		private static IAssemblyLoaderContext _context;

		private static object _contextObjLock = new object();

		/// <summary>
		/// Recupera a instancia do loader de assembly.
		/// </summary>
		public static IAssemblyLoader Instance
		{
			get
			{
				if(_instance == null)
					lock (_objLock)
					{
						if(_instance == null)
						{
							ServiceLocatorValidator.Validate();
							var serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
							try
							{
								_instance = serviceLocator.GetInstance<IAssemblyLoader>();
							}
							catch
							{
								_instance = new AssemblyLoader(AppDomain.CurrentDomain, serviceLocator.GetInstance<IAssemblyRepository>());
							}
						}
					}
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Recupera a instancia do contexto do loader.
		/// </summary>
		public static IAssemblyLoaderContext Context
		{
			get
			{
				if(_context == null)
					lock (_contextObjLock)
					{
						if(_context == null)
						{
							ServiceLocatorValidator.Validate();
							var serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
							try
							{
								_context = serviceLocator.GetInstance<IAssemblyLoaderContext>();
							}
							catch(Exception ex)
							{
								throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyLoader_GetContextError, Diagnostics.ExceptionFormatter.FormatException(ex, true)).Format(), ex);
							}
						}
					}
				return _context;
			}
			set
			{
				_context = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="assemblyRepository"></param>
		public AssemblyLoader(AppDomain domain, IAssemblyRepository assemblyRepository)
		{
			assemblyRepository.Require("assemblyRepository").NotNull();
			_domain = domain;
			_assemblyRepository = assemblyRepository;
		}

		/// <summary>
		/// Tenta carregar o assembly associado com o nome informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly.</param>
		/// <param name="assembly">Instancia do assembly carregado.</param>
		/// <returns>True caso o assembly tenha sido carregado com sucesso.</returns>
		public bool TryGet(string assemblyName, out System.Reflection.Assembly assembly)
		{
			Exception exception = null;
			return TryGet(assemblyName, out assembly, out exception);
		}

		/// <summary>
		/// Tenta carregar o assembly associado com o nome informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly.</param>
		/// <param name="assembly">Instancia do assembly carregado.</param>
		/// <param name="exception">Error ocorrido</param>
		/// <returns>True caso o assembly tenha sido carregado com sucesso.</returns>
		public bool TryGet(string assemblyName, out System.Reflection.Assembly assembly, out Exception exception)
		{
			var assemblyName2 = new System.Reflection.AssemblyName(assemblyName);
			System.Reflection.Assembly assembly2 = null;
			exception = null;
			try
			{
				assembly2 = _domain.Load(assemblyName2);
			}
			catch(Exception ex)
			{
				exception = ex;
				assembly = null;
			}
			if(assembly2 != null)
			{
				assembly = assembly2;
				return true;
			}
			var assemblyPart = new AssemblyPart(assemblyName);
			IAssemblyPackage package = null;
			try
			{
				var container = _assemblyRepository.GetAssemblyPackages(new AssemblyPart[] {
					assemblyPart
				});
				package = container.FirstOrDefault();
			}
			catch(Exception ex)
			{
				exception = ex;
				assembly = null;
				return false;
			}
			if(package != null)
			{
				assembly2 = package.GetAssembly(assemblyPart);
				assembly = assembly2;
				exception = null;
				return assembly2 != null;
			}
			else
			{
				assembly = null;
				return false;
			}
		}

		/// <summary>
		/// Tenta carrega os assemblies informados.
		/// </summary>
		/// <param name="assemblyNames">Nomes dos assemblies que serão carregados.</param>
		/// <returns></returns>
		public AssemblyLoaderGetResult Get(string[] assemblyNames)
		{
			var resultEntries = new List<AssemblyLoaderGetResult.Entry>();
			var assemblyNames2 = new List<string>();
			foreach (var assemblyName in assemblyNames.Distinct())
			{
				var assemblyName2 = new System.Reflection.AssemblyName(assemblyName);
				System.Reflection.Assembly assembly2 = null;
				try
				{
					assembly2 = _domain.Load(assemblyName2);
				}
				catch(Exception)
				{
				}
				if(assembly2 == null)
					assemblyNames2.Add(assemblyName);
			}
			AssemblyPackageContainer packagesContainer = null;
			var assemblyParts = assemblyNames2.Select(f => new AssemblyPart(f)).ToArray();
			try
			{
				packagesContainer = _assemblyRepository.GetAssemblyPackages(assemblyParts);
			}
			catch(Exception ex)
			{
				foreach (var assemblyName in assemblyNames2)
				{
					resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyName, null, false, ex));
				}
			}
			if(packagesContainer != null)
			{
				foreach (var assemblyPart in assemblyParts)
				{
					var packageFound = false;
					foreach (var package in packagesContainer)
					{
						if(package.Contains(assemblyPart))
						{
							packageFound = true;
							System.Reflection.Assembly assembly = null;
							try
							{
								assembly = package.GetAssembly(assemblyPart);
							}
							catch(Exception ex)
							{
								resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source, null, false, ex));
								continue;
							}
							resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source, assembly, true, null));
							break;
						}
					}
					if(!packageFound)
					{
						resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source, null, true, null));
					}
				}
			}
			else
			{
				foreach (var assemblyName in assemblyNames2)
				{
					resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyName, null, true, null));
				}
			}
			return new AssemblyLoaderGetResult(resultEntries);
		}
	}
}
