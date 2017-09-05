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

namespace Colosoft.Reflection.Local
{
	/// <summary>
	/// Pacote de assemblies representado por um diretório.
	/// </summary>
	class DirectoryAssemblyPackage : IAssemblyPackage
	{
		private Guid _uid = Guid.NewGuid();

		private IEnumerable<string> _assemblyFiles;

		private AssemblyResolverManager _assemblyResolverManager;

		private Dictionary<string, System.Reflection.Assembly> _assemblies;

		private IEnumerable<string> _assemblyPaths;

		/// <summary>
		/// Assemblies associados.
		/// </summary>
		private Dictionary<string, System.Reflection.Assembly> Assemblies
		{
			get
			{
				if(_assemblies == null)
					_assemblies = LoadAssemblies();
				return _assemblies;
			}
		}

		/// <summary>
		/// Identificador do pacote.
		/// </summary>
		public Guid Uid
		{
			get
			{
				return _uid;
			}
		}

		public int Count
		{
			get
			{
				return _assemblyFiles.Count();
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public AssemblyPart this[int index]
		{
			get
			{
				var fileName = _assemblyFiles.ElementAt(index);
				return new AssemblyPart(System.IO.Path.GetFileName(fileName));
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblyResolveManager"></param>
		/// <param name="assemblyFiles"></param>
		public DirectoryAssemblyPackage(AssemblyResolverManager assemblyResolveManager, IEnumerable<string> assemblyFiles)
		{
			_assemblyResolverManager = assemblyResolveManager;
			var directories = assemblyFiles.Select(f => System.IO.Path.GetDirectoryName(f)).Distinct().Select(f => new System.IO.DirectoryInfo(f)).Where(f => f.Exists);
			var files = new List<string>();
			foreach (var dir in directories)
				files.AddRange(dir.GetFiles("*.dll", System.IO.SearchOption.TopDirectoryOnly).Select(f => f.FullName));
			_assemblyPaths = assemblyFiles;
			_assemblyFiles = files;
		}

		/// <summary>
		/// Recupera o nome do arquivo do assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private string GetFileName(AssemblyPart name)
		{
			var fileName = _assemblyFiles.First(f => StringComparer.InvariantCultureIgnoreCase.Equals(name.Source, System.IO.Path.GetFileName(f)));
			return fileName;
		}

		/// <summary>
		/// Carrega os assemblies
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, System.Reflection.Assembly> LoadAssemblies()
		{
			var exceptions = new List<Exception>();
			Dictionary<string, System.Reflection.Assembly> domainAssemblies = new Dictionary<string, System.Reflection.Assembly>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var i in _assemblyResolverManager.AppDomain.GetAssemblies())
			{
				var key = string.Format("{0}.dll", i.GetName().Name);
				if(!domainAssemblies.ContainsKey(key))
					domainAssemblies.Add(key, i);
				else
					domainAssemblies[key] = i;
			}
			var list = new Dictionary<string, System.Reflection.Assembly>();
			var resolver = new LoadAssemblyResolver(_assemblyResolverManager.AppDomain, domainAssemblies, _assemblyFiles);
			_assemblyResolverManager.Add(resolver);
			try
			{
				foreach (var part in _assemblyPaths)
				{
					System.Reflection.Assembly assembly = null;
					if(!domainAssemblies.TryGetValue(System.IO.Path.GetFileName(part), out assembly))
					{
						try
						{
							var name = System.Reflection.AssemblyName.GetAssemblyName(part);
							assembly = _assemblyResolverManager.AppDomain.Load(name);
						}
						catch(Exception ex)
						{
							exceptions.Add(ex);
							continue;
						}
						try
						{
							assembly.GetTypes();
						}
						catch(System.Reflection.ReflectionTypeLoadException ex)
						{
							exceptions.Add(new System.Reflection.ReflectionTypeLoadException(ex.Types, ex.LoaderExceptions, string.Format("An error ocurred when load types from assembly '{0}'", assembly.FullName)));
							continue;
						}
						catch(Exception ex)
						{
							exceptions.Add(new Exception(string.Format("An error ocurred when load types from assembly '{0}'", assembly.FullName), ex));
							continue;
						}
						if(!domainAssemblies.ContainsKey(System.IO.Path.GetFileName(part)))
							domainAssemblies.Add(System.IO.Path.GetFileName(part), assembly);
					}
					list.Add(System.IO.Path.GetFileName(part), assembly);
				}
			}
			finally
			{
				_assemblyResolverManager.Remove(resolver);
			}
			if(exceptions.Count > 0)
				throw new AggregateException(exceptions);
			return list;
		}

		/// <summary>
		/// Recupera o assembly associado com a parte informada.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public System.Reflection.Assembly GetAssembly(AssemblyPart name)
		{
			return Assemblies[name.Source];
		}

		/// <summary>
		/// Carrega o assembly guardado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="exception">Exception caso ocorra.</param>
		/// <returns></returns>
		public System.Reflection.Assembly LoadAssemblyGuarded(AssemblyPart name, out Exception exception)
		{
			try
			{
				exception = null;
				return Assemblies[name.Source];
			}
			catch(Exception ex)
			{
				exception = ex;
				return null;
			}
		}

		/// <summary>
		/// Recupera o stream do assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public System.IO.Stream GetAssemblyStream(AssemblyPart name)
		{
			return System.IO.File.OpenRead(GetFileName(name));
		}

		/// <summary>
		/// Extraí os arquivos do pacote.
		/// </summary>
		/// <param name="outputDirectory">Diretório de saída.</param>
		/// <param name="canOverride">True para sobreescrever os arquivos que existirem.</param>
		/// <rereturns>True caso a operação tenha sido realizada com sucesso.</rereturns>
		public bool ExtractPackageFiles(string outputDirectory, bool canOverride)
		{
			return true;
		}

		/// <summary>
		/// Verifica se existe no pacote uma parte compatível com a informada.
		/// </summary>
		/// <param name="assemblyPart">Parte que será comparada.</param>
		/// <returns></returns>
		public bool Contains(AssemblyPart assemblyPart)
		{
			return _assemblyPaths.Any(f => StringComparer.InvariantCultureIgnoreCase.Equals(assemblyPart.Source, System.IO.Path.GetFileName(f)));
		}

		/// <summary>
		/// Recupera o enumerador das partes.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyPart> GetEnumerator()
		{
			return _assemblyPaths.Select(f => new AssemblyPart(f)).GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das partes.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _assemblyPaths.Select(f => new AssemblyPart(f)).GetEnumerator();
		}

		/// <summary>
		/// Classe usada para resolver os assemblies.
		/// </summary>
		class LoadAssemblyResolver : IAssemblyResolver
		{
			private AppDomain _appDomain;

			/// <summary>
			/// Relação das partes de assemblies que pode ser carregadas.
			/// </summary>
			private IEnumerable<string> _deploymentParts;

			/// <summary>
			/// Dicionário dos assemblies já carregados.
			/// </summary>
			private Dictionary<string, System.Reflection.Assembly> _assemblies;

			/// <summary>
			/// Identifica se a instancia está em um estado válido.
			/// </summary>
			public bool IsValid
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="appDomain"></param>
			/// <param name="assemblies"></param>
			/// <param name="deploymentParts"></param>
			public LoadAssemblyResolver(AppDomain appDomain, Dictionary<string, System.Reflection.Assembly> assemblies, IEnumerable<string> deploymentParts)
			{
				_appDomain = appDomain;
				_assemblies = assemblies;
				_deploymentParts = deploymentParts;
			}

			/// <summary>
			/// Resolve o assembly informado.
			/// </summary>
			/// <param name="args"></param>
			/// <param name="assembly"></param>
			/// <param name="error">Erro ocorrido</param>
			/// <returns></returns>
			public bool Resolve(ResolveEventArgs args, out System.Reflection.Assembly assembly, out Exception error)
			{
				var libraryName = new System.Reflection.AssemblyName(args.Name).Name;
				if(!libraryName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
					libraryName = string.Concat(libraryName, ".dll");
				if(_assemblies.TryGetValue(libraryName, out assembly))
				{
					error = null;
					return true;
				}
				var part = _deploymentParts.Where(f => string.Compare(System.IO.Path.GetFileName(f), libraryName, true) == 0).FirstOrDefault();
				if(part != null)
				{
					try
					{
						var name = System.Reflection.AssemblyName.GetAssemblyName(part);
						assembly = _appDomain.Load(name);
						assembly.GetTypes();
					}
					catch(Exception ex)
					{
						error = ex;
						return false;
					}
					_assemblies.Add(libraryName, assembly);
					error = null;
					return true;
				}
				error = null;
				return false;
			}
		}
	}
}
