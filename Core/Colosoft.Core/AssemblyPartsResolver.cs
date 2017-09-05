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
	/// Implementação padrão para o AssemblyResolver.
	/// </summary>
	public class AssemblyPartsResolver : IAssemblyResolverExt, IDisposable
	{
		private IAssemblyRepository _assemblyRepository;

		private IEnumerable<AssemblyPart> _assemblyParts;

		private AssemblyLoaderGetResult _loadAssembliesResult;

		private object _objLock = new object();

		private bool _isValid = true;

		/// <summary>
		/// Evento acionado quando a instancia for carregada.
		/// </summary>
		public event AssemblyResolverLoadHandler Loaded;

		/// <summary>
		/// Identifica se a instancia está em um estado válido.
		/// </summary>
		public bool IsValid
		{
			get
			{
				return _isValid;
			}
		}

		/// <summary>
		/// Resulta da carga dos assemblies.
		/// </summary>
		public AssemblyLoaderGetResult LoadAssembliesResult
		{
			get
			{
				return _loadAssembliesResult;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblyRepository">Instancia do repositório de assemblies.</param>
		/// <param name="assemblyParts">Relação dos nomes dos assemblies que devem ser resolvidos.</param>
		public AssemblyPartsResolver(IAssemblyRepository assemblyRepository, IEnumerable<AssemblyPart> assemblyParts)
		{
			assemblyRepository.Require("assemblyRepository").NotNull();
			assemblyParts.Require("assemblyNames").NotNull();
			_assemblyRepository = assemblyRepository;
			_assemblyParts = assemblyParts;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~AssemblyPartsResolver()
		{
			Dispose(false);
		}

		/// <summary>
		/// Carrega os assemblies.
		/// </summary>
		/// <returns></returns>
		private AssemblyLoaderGetResult LoadAssemblies()
		{
			var resultEntries = new List<AssemblyLoaderGetResult.Entry>();
			AssemblyPackageContainer packagesContainer = null;
			var assemblyParts = _assemblyParts == null ? new AssemblyPart[0] : _assemblyParts.ToArray();
			try
			{
				packagesContainer = _assemblyRepository.GetAssemblyPackages(assemblyParts);
			}
			catch(Exception ex)
			{
				if(ex is Colosoft.Net.DownloaderException)
				{
					ex = new AssemblyResolverException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyResolver_GetAssemblyDownloaderException, ex.Message).Format(), ex);
				}
				else
					ex = new AssemblyResolverException(ex.Message, ex);
				foreach (var assemblyPart in _assemblyParts)
				{
					resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source.GetAssemblyNameWithoutExtension(), null, false, ex));
				}
				return new AssemblyLoaderGetResult(resultEntries);
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
								resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source.GetAssemblyNameWithoutExtension(), null, false, ex));
								continue;
							}
							resultEntries.Add(new AssemblyLoaderGetResult.Entry(assembly.GetName().Name, assembly, true, null));
							break;
						}
					}
					if(!packageFound)
					{
						resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source.GetAssemblyNameWithoutExtension(), null, true, null));
					}
				}
			}
			else
			{
				foreach (var assemblyPart in _assemblyParts)
				{
					resultEntries.Add(new AssemblyLoaderGetResult.Entry(assemblyPart.Source.GetAssemblyNameWithoutExtension(), null, true, null));
				}
			}
			return new AssemblyLoaderGetResult(resultEntries);
		}

		/// <summary>
		/// Resolve o assembly.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="assembly"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public bool Resolve(ResolveEventArgs args, out System.Reflection.Assembly assembly, out Exception error)
		{
			if(!IsValid)
			{
				assembly = null;
				error = null;
				return false;
			}
			_isValid = false;
			try
			{
				lock (_objLock)
				{
					if(_loadAssembliesResult == null)
					{
						try
						{
							_loadAssembliesResult = LoadAssemblies();
						}
						catch(Exception ex)
						{
							error = ex;
							assembly = null;
							return false;
						}
						if(Loaded != null)
							try
							{
								Loaded(this, new AssemblyResolverLoadEventArgs {
									Result = _loadAssembliesResult
								});
							}
							catch(Exception ex)
							{
								error = ex;
								assembly = null;
								return false;
							}
					}
					var assemblyName = new System.Reflection.AssemblyName(args.Name).Name.GetAssemblyNameWithoutExtension();
					var entry = _loadAssembliesResult.Where(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.AssemblyName, assemblyName)).FirstOrDefault();
					if(entry == null || entry.Error != null)
					{
						assembly = null;
						error = entry != null ? entry.Error : null;
						return false;
					}
					assembly = entry.Assembly;
					error = null;
					if(assembly != null)
						return true;
					return false;
				}
			}
			finally
			{
				_isValid = true;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_loadAssembliesResult = null;
			_assemblyParts = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
