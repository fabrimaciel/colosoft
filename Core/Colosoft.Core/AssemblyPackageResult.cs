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
	/// Armazena o resulta de um pacote.
	/// </summary>
	class AssemblyPackageResult : IAssemblyPackageResult
	{
		private Guid _packageUid;

		private string _packageFileName;

		private Dictionary<string, System.Reflection.Assembly> _assemblies;

		private AssemblyResolverManager _assemblyResolverManager;

		private AggregateException _extractPackageAssembliesException;

		/// <summary>
		/// Dominio da aplicação.
		/// </summary>
		private AssemblyResolverManager AssemblyResolverManager
		{
			get
			{
				return _assemblyResolverManager;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblyResolverManager"></param>
		/// <param name="uid"></param>
		/// <param name="packageFileName">Nome do arquivo do pacote.</param>
		public AssemblyPackageResult(AssemblyResolverManager assemblyResolverManager, Guid uid, string packageFileName)
		{
			_assemblyResolverManager = assemblyResolverManager;
			_packageUid = uid;
			_packageFileName = packageFileName;
		}

		/// <summary>
		/// Destrutor;
		/// </summary>
		~AssemblyPackageResult()
		{
			Dispose();
		}

		/// <summary>
		/// Extrai os assemblies do pacote.
		/// </summary>
		private void ExtractPackageAssemblies()
		{
			if(_assemblies != null)
				return;
			var assemblies = new Dictionary<string, System.Reflection.Assembly>();
			if(!string.IsNullOrEmpty(_packageFileName) && System.IO.File.Exists(_packageFileName))
				using (var stream = System.IO.File.OpenRead(_packageFileName))
				{
					foreach (var assembly in IO.Xap.XapPackage.LoadPackagedAssemblies(this.AssemblyResolverManager, System.IO.Path.GetDirectoryName(_packageFileName), _packageUid, stream, false, out _extractPackageAssembliesException))
						assemblies.Add(string.Format("{0}.dll", assembly.GetName().Name), assembly);
				}
			_assemblies = assemblies;
		}

		/// <summary>
		/// Extraí os arquivos do pacote.
		/// </summary>
		/// <param name="outputDirectory">Diretório de saída.</param>
		/// <param name="canOverride">True para sobreescrever os arquivos que existirem.</param>
		/// <returns></returns>
		public bool ExtractPackageFiles(string outputDirectory, bool canOverride)
		{
			outputDirectory.Require("outputDirectory").NotNull().NotEmpty();
			if(!string.IsNullOrEmpty(_packageFileName) && System.IO.File.Exists(_packageFileName))
				using (var stream = System.IO.File.OpenRead(_packageFileName))
					IO.Xap.XapPackage.ExtractPackageAssemblies(stream, outputDirectory, canOverride);
			return true;
		}

		/// <summary>
		/// Recupera o stream do assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public System.IO.Stream GetAssemblyStream(AssemblyPart name)
		{
			System.IO.Stream result = null;
			if(!string.IsNullOrEmpty(_packageFileName) && System.IO.File.Exists(_packageFileName))
			{
				var fileInfo = new System.IO.FileInfo(_packageFileName);
				if(fileInfo.Length > 0)
				{
					using (var packageStream = System.IO.File.OpenRead(_packageFileName))
					{
						result = new System.IO.MemoryStream();
						if(Colosoft.IO.Xap.XapPackage.GetAssembly(packageStream, result, name))
							result.Seek(0, System.IO.SeekOrigin.Begin);
						else
						{
							result.Dispose();
							result = null;
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Carrega o assembly informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public System.Reflection.Assembly LoadAssembly(AssemblyPart name)
		{
			name.Require("codeBase").NotNull();
			ExtractPackageAssemblies();
			if(_extractPackageAssembliesException != null)
				throw _extractPackageAssembliesException;
			System.Reflection.Assembly assembly = null;
			if(_assemblies != null && _assemblies.TryGetValue(name.Source, out assembly))
				return assembly;
			return null;
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
				return LoadAssembly(name);
			}
			catch(System.IO.FileNotFoundException exception2)
			{
				exception = exception2;
			}
			catch(System.IO.FileLoadException exception3)
			{
				exception = exception3;
			}
			catch(BadImageFormatException exception4)
			{
				exception = exception4;
			}
			catch(System.Reflection.ReflectionTypeLoadException exception5)
			{
				exception = exception5;
			}
			catch(Exception ex)
			{
				exception = ex;
			}
			return null;
		}

		/// <summary>
		/// Recupera o assembly.
		/// </summary>
		/// <param name="name">Nome do assembly que será recuperado.</param>
		/// <returns></returns>
		public System.Reflection.Assembly GetAssembly(AssemblyPart name)
		{
			return LoadAssembly(name);
		}

		/// <summary>
		/// Cria uma nova instancia do pacote.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public AssemblyPackage CreatePackage()
		{
			List<AssemblyPart> assemblyNames = null;
			DateTime createdDate = DateTime.Now;
			if(!string.IsNullOrEmpty(_packageFileName))
			{
				var fileInfo = new System.IO.FileInfo(_packageFileName);
				if(fileInfo.Exists)
				{
					using (var stream = System.IO.File.OpenRead(_packageFileName))
						assemblyNames = IO.Xap.XapPackage.GetDeploymentParts(stream).ToList();
					createdDate = fileInfo.LastWriteTime;
				}
			}
			if(assemblyNames == null)
				assemblyNames = new List<AssemblyPart>();
			return new AssemblyPackage(assemblyNames) {
				Uid = _packageUid,
				CreateTime = createdDate,
				Result = this
			};
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
