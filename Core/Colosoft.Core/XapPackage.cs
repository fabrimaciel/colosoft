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
using Colosoft.Reflection;

namespace Colosoft.IO.Xap
{
	/// <summary>
	/// Implementação de um pacote Xap.
	/// </summary>
	public static class XapPackage
	{
		/// <summary>
		/// Recupera as partes de implementação.
		/// </summary>
		/// <param name="zipArchive"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private static IEnumerable<AssemblyPart> GetDeploymentParts(Colosoft.IO.Compression.ZipArchive zipArchive)
		{
			var file = zipArchive.Files.Where(f => f.Name == "AppManifest.xaml").FirstOrDefault();
			if(file == null)
				throw new InvalidOperationException("AppManifest not found.");
			var list = new List<AssemblyPart>();
			System.IO.Stream resourceStream = null;
			try
			{
				using (var stream = file.OpenRead())
				{
					resourceStream = new System.IO.MemoryStream();
					var buffer = new byte[1024];
					var read = 0;
					while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
						resourceStream.Write(buffer, 0, read);
					resourceStream.Seek(0, System.IO.SeekOrigin.Begin);
				}
				using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(resourceStream))
				{
					if(!reader.ReadToFollowing("AssemblyPart"))
						return list;
					do
					{
						string attribute = reader.GetAttribute("Source");
						if(attribute != null)
						{
							AssemblyPart item = new AssemblyPart();
							item.Source = attribute;
							list.Add(item);
						}
					}
					while (reader.ReadToNextSibling("AssemblyPart"));
				}
			}
			finally
			{
				if(resourceStream != null)
					resourceStream.Dispose();
			}
			return list;
		}

		/// <summary>
		/// Recupera as partes de implementação contidas na stream do pacote informado.
		/// </summary>
		/// <param name="packageStream"></param>
		/// <returns></returns>
		public static IEnumerable<AssemblyPart> GetDeploymentParts(System.IO.Stream packageStream)
		{
			using (var zipArchive = new IO.Compression.ZipArchive(packageStream, System.IO.FileAccess.Read))
				return GetDeploymentParts(zipArchive);
		}

		/// <summary>
		/// Recupera os assemblies contidos na stream informada.
		/// </summary>
		/// <param name="resolverManager"></param>
		/// <param name="assemblyRepositoryDirectory"></param>
		/// <param name="packageUid">Identificador do pacote.</param>
		/// <param name="packageStream"></param>
		/// <param name="canOverride">Identifica se é para sobreescreve os arquivos.</param>
		/// <param name="aggregateException"></param>
		/// <returns></returns>
		public static IEnumerable<System.Reflection.Assembly> LoadPackagedAssemblies(AssemblyResolverManager resolverManager, string assemblyRepositoryDirectory, Guid packageUid, System.IO.Stream packageStream, bool canOverride, out AggregateException aggregateException)
		{
			resolverManager.Require("resolverManager").NotNull();
			packageStream.Require("packageStream").NotNull();
			var exceptions = new List<Exception>();
			Dictionary<string, System.Reflection.Assembly> domainAssemblies = new Dictionary<string, System.Reflection.Assembly>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var i in resolverManager.AppDomain.GetAssemblies())
			{
				var key = string.Format("{0}.dll", i.GetName().Name);
				if(!domainAssemblies.ContainsKey(key))
					domainAssemblies.Add(key, i);
				else
					domainAssemblies[key] = i;
			}
			string packageDirectory = null;
			if(!string.IsNullOrEmpty(assemblyRepositoryDirectory))
			{
				packageDirectory = System.IO.Path.Combine(assemblyRepositoryDirectory, packageUid.ToString());
				if(!System.IO.Directory.Exists(packageDirectory))
					System.IO.Directory.CreateDirectory(packageDirectory);
			}
			using (var zipArchive = new IO.Compression.ZipArchive(packageStream, System.IO.FileAccess.Read))
			{
				if(packageDirectory != null)
					zipArchive.CopyToDirectory("", packageDirectory, canOverride);
				var list = new List<System.Reflection.Assembly>();
				IEnumerable<AssemblyPart> deploymentParts = GetDeploymentParts(zipArchive);
				var resolver = new LoadPackageAssemblyResolver(resolverManager.AppDomain, domainAssemblies, deploymentParts, zipArchive, packageDirectory);
				resolverManager.Add(resolver);
				try
				{
					foreach (AssemblyPart part in deploymentParts)
					{
						System.Reflection.Assembly assembly = null;
						if(!domainAssemblies.TryGetValue(part.Source, out assembly))
						{
							if(packageDirectory == null)
							{
								var fileInfo = zipArchive.Files.Where(f => f.Name == part.Source).FirstOrDefault();
								var raw = new byte[fileInfo.Length];
								using (var file = zipArchive.OpenRead(part.Source))
									file.Read(raw, 0, raw.Length);
								try
								{
									assembly = part.Load(resolverManager.AppDomain, raw);
								}
								catch(Exception ex)
								{
									exceptions.Add(ex);
									continue;
								}
							}
							else
							{
								try
								{
									assembly = part.Load(resolverManager.AppDomain, System.IO.Path.Combine(packageDirectory, part.Source));
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
							}
							if(!domainAssemblies.ContainsKey(part.Source))
								domainAssemblies.Add(part.Source, assembly);
						}
						list.Add(assembly);
					}
				}
				finally
				{
					resolverManager.Remove(resolver);
				}
				if(exceptions.Count > 0)
					aggregateException = new AggregateException(exceptions);
				else
					aggregateException = null;
				return list;
			}
		}

		/// <summary>
		/// Extraí dos assemblies do pacote.
		/// </summary>
		/// <param name="packageStream">Stream do pacote.</param>
		/// <param name="outputDirectory">Diretório de saída dos arquivos do pacote.</param>
		/// <param name="canOverride">Identifica se pode sobreescreve os arquivos existentes.</param>
		public static void ExtractPackageAssemblies(System.IO.Stream packageStream, string outputDirectory, bool canOverride = false)
		{
			packageStream.Require("packageStream").NotNull();
			using (var zipArchive = new IO.Compression.ZipArchive(packageStream, System.IO.FileAccess.Read))
				zipArchive.CopyToDirectory("", outputDirectory, canOverride);
		}

		/// <summary>
		/// Recupera a stream do assembly.
		/// </summary>
		/// <param name="packageStream">Stream do pacote.</param>
		/// <param name="assemblyStream">Stream onde será salvos os dados do assembly.</param>
		/// <param name="part">Part com as informações do assembly.</param>
		/// <return>True caso o assembly tenha sido carregado.</return>
		public static bool GetAssembly(System.IO.Stream packageStream, System.IO.Stream assemblyStream, AssemblyPart part)
		{
			using (var zipArchive = new IO.Compression.ZipArchive(packageStream, System.IO.FileAccess.Read))
			{
				foreach (var i in zipArchive.Files)
					if(i.Name == part.Source)
					{
						using (var stream = i.OpenRead())
						{
							var buffer = new byte[1024];
							var read = 0;
							while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
								assemblyStream.Write(buffer, 0, read);
						}
						return true;
					}
			}
			return false;
		}

		class LoadPackageAssemblyResolver : IAssemblyResolver
		{
			private string _packageDirectory;

			private AppDomain _appDomain;

			private IO.Compression.ZipArchive _zipArchive;

			/// <summary>
			/// Relação das partes de assemblies que pode ser carregadas.
			/// </summary>
			private IEnumerable<AssemblyPart> _deploymentParts;

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
			/// <param name="zipArchive"></param>
			/// <param name="packageDirectory"></param>
			public LoadPackageAssemblyResolver(AppDomain appDomain, Dictionary<string, System.Reflection.Assembly> assemblies, IEnumerable<AssemblyPart> deploymentParts, IO.Compression.ZipArchive zipArchive, string packageDirectory)
			{
				_appDomain = appDomain;
				_assemblies = assemblies;
				_deploymentParts = deploymentParts;
				_zipArchive = zipArchive;
				_packageDirectory = packageDirectory;
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
				var part = _deploymentParts.Where(f => string.Compare(f.Source, libraryName, true) == 0).FirstOrDefault();
				if(part != null)
				{
					try
					{
						if(_packageDirectory == null)
						{
							var fileInfo = _zipArchive.Files.Where(f => f.Name == part.Source).FirstOrDefault();
							var buffer = new byte[fileInfo.Length];
							using (var file = _zipArchive.OpenRead(part.Source))
								file.Read(buffer, 0, buffer.Length);
							assembly = part.Load(_appDomain, buffer);
						}
						else
							assembly = part.Load(_appDomain, System.IO.Path.Combine(_packageDirectory, part.Source));
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
