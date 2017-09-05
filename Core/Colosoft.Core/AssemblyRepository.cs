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
	/// Implementação de um repositório local de assemblies.
	/// </summary>
	public class AssemblyRepository : IAssemblyRepository
	{
		private IEnumerable<IAssemblyRepositoryMaintenance> _maintenanceInstances = new IAssemblyRepositoryMaintenance[0];

		private AssemblyResolverManager _assemblyResolverManager;

		private IAssemblyPackageValidator _validator;

		private IAssemblyPackageDownloader _downloader;

		private IAssemblyInfoRepository _assemblyInfoRepository;

		private object _downloaderLock = new object();

		private string _repositoryDirectory;

		private string[] _assemblyFilesDirectories;

		private bool _isStarted;

		/// <summary>
		/// Identifica se o repositório esta iniciando.
		/// </summary>
		private bool _isStarting;

		private bool _useDirectoryAssemblyPackages;

		private System.Threading.Thread _startThread;

		private object _objLock = new object();

		/// <summary>
		/// Pacotes carregados.
		/// </summary>
		private List<AssemblyPackageCacheEntry> _packages = new List<AssemblyPackageCacheEntry>();

		/// <summary>
		/// Evento acionado quando o repositório for iniciado.
		/// </summary>
		public event AssemblyRepositoryStartedHandler Started;

		/// <summary>
		/// Identifica se é para usar pacotes de assemblies de diretório.
		/// </summary>
		public bool UseDirectoryAssemblyPackages
		{
			get
			{
				return _useDirectoryAssemblyPackages;
			}
			set
			{
				_useDirectoryAssemblyPackages = value;
			}
		}

		/// <summary>
		/// Identifica se o repositório já foi iniciado.
		/// </summary>
		public bool IsStarted
		{
			get
			{
				return _isStarted;
			}
		}

		/// <summary>
		/// Diretório padrão do repositório.
		/// </summary>
		public static string DefaultRepositoryDirectory
		{
			get
			{
				return System.IO.Path.Combine(IO.IsolatedStorage.IsolatedStorage.AuthenticationContextDirectory, @"Reflection\Assemblies");
			}
		}

		/// <summary>
		/// Instancia do gerencia de resolução dos assemblies.
		/// </summary>
		public AssemblyResolverManager AssemblyResolverManager
		{
			get
			{
				return _assemblyResolverManager;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, ControlAppDomain = true)]
		public AssemblyRepository() : this(null, new string[0], new AssemblyResolverManager(AppDomain.CurrentDomain), null, null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="repositoryDirectory">Diretório base do respositório.</param>
		/// <param name="assemblyFilesDirectories">Diretórios dos arquivos de assembly.</param>
		/// <param name="assemblyResolverManager">Instancia responsável por resolver os assemblies.</param>
		/// <param name="downloader">Instancia responsável pelo download dos assemblies.</param>
		/// <param name="validator">Instancia do validador dos pacotes.</param>
		public AssemblyRepository(string repositoryDirectory, string[] assemblyFilesDirectories, AssemblyResolverManager assemblyResolverManager, IAssemblyPackageDownloader downloader, IAssemblyPackageValidator validator) : this(repositoryDirectory, assemblyFilesDirectories, assemblyResolverManager, downloader, validator, null)
		{
		}

		/// <summary>
		/// Construtor completo.
		/// </summary>
		/// <param name="repositoryDirectory">Diretório base do respositório.</param>
		/// <param name="assemblyFilesDirectories">Diretórios dos arquivos de assembly.</param>
		/// <param name="assemblyResolverManager">Instancia responsável por resolver os assemblies.</param>
		/// <param name="downloader">Instancia responsável pelo download dos assemblies.</param>
		/// <param name="validator">Instancia do validador dos pacotes.</param>
		/// <param name="assemblyInfoRepository">Repositório das informações do assembly.</param>
		public AssemblyRepository(string repositoryDirectory, string[] assemblyFilesDirectories, AssemblyResolverManager assemblyResolverManager, IAssemblyPackageDownloader downloader, IAssemblyPackageValidator validator, IAssemblyInfoRepository assemblyInfoRepository)
		{
			repositoryDirectory = repositoryDirectory ?? DefaultRepositoryDirectory;
			if(!System.IO.Directory.Exists(repositoryDirectory))
				System.IO.Directory.CreateDirectory(repositoryDirectory);
			_assemblyFilesDirectories = assemblyFilesDirectories ?? new string[0];
			_repositoryDirectory = repositoryDirectory;
			_assemblyResolverManager = assemblyResolverManager;
			_downloader = downloader;
			_validator = validator;
			_assemblyInfoRepository = assemblyInfoRepository;
			if(!System.IO.Directory.Exists(GetRepositoryFolder()))
				System.IO.Directory.CreateDirectory(GetRepositoryFolder());
		}

		/// <summary>
		/// Realiza a inicialização do respositório.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void DoStart()
		{
			try
			{
				var exceptions = new List<Exception>();
				try
				{
					var repositoryFolder = GetRepositoryFolder();
					var repositoryDirectories = System.IO.Directory.GetDirectories(repositoryFolder);
					var files = System.IO.Directory.GetFiles(repositoryFolder, "*.xap");
					var packageDirectories = files.Select(f => System.IO.Path.Combine(repositoryFolder, System.IO.Path.GetFileNameWithoutExtension(f))).ToArray();
					foreach (var invalidDirectory in repositoryDirectories.Except(packageDirectories, StringComparer.InvariantCultureIgnoreCase))
					{
						try
						{
							System.IO.Directory.Delete(invalidDirectory, true);
						}
						catch
						{
							if(System.IO.Directory.Exists(invalidDirectory))
							{
								foreach (var file in System.IO.Directory.GetFiles(invalidDirectory))
									try
									{
										System.IO.File.Delete(file);
									}
									catch
									{
									}
							}
						}
					}
					var packages = new List<AssemblyPackageCacheEntry>(files.Length);
					for(var i = 0; i < files.Length; i++)
					{
						var file = files[i];
						try
						{
							var uid = Guid.Parse(System.IO.Path.GetFileNameWithoutExtension(file));
							var pkg = new AssemblyPackageResult(_assemblyResolverManager, uid, file).CreatePackage();
							packages.Add(new AssemblyPackageCacheEntry(pkg, null, file));
						}
						catch(Exception ex)
						{
							exceptions.Add(ex);
						}
					}
					if(_validator != null)
					{
						var validateResult = _validator.Validate(packages.Select(f => f.Package).ToArray());
						lock (_objLock)
						{
							for(var i = 0; i < packages.Count; i++)
								if(validateResult[i])
									_packages.Add(packages[i]);
								else
								{
									try
									{
										packages[i].Dispose();
										var packageDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(files[i]), System.IO.Path.GetFileNameWithoutExtension(files[i]));
										if(System.IO.Directory.Exists(packageDirectory))
											try
											{
												System.IO.Directory.Delete(packageDirectory, true);
											}
											catch
											{
											}
										System.IO.File.Delete(files[i]);
									}
									catch
									{
									}
								}
						}
					}
					else
						lock (_objLock)
						{
							foreach (var i in packages)
								_packages.Add(i);
						}
					_maintenanceInstances = new List<IAssemblyRepositoryMaintenance>(Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<IAssemblyRepositoryMaintenance>());
				}
				catch(System.Threading.ThreadAbortException)
				{
					return;
				}
				catch(Exception ex)
				{
					exceptions.Add(ex);
				}
				finally
				{
					OnStartedInternal(new AssemblyRepositoryStartedArgs(exceptions.ToArray()));
				}
				if(_maintenanceInstances != null)
					MaintenanceRobot();
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
		}

		/// <summary>
		/// Representa o robo que realiza a manutenção do repositório.
		/// </summary>
		private void MaintenanceRobot()
		{
			return;
		}

		/// <summary>
		/// Recupera a stream associado com o pacote de assembly.
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		private System.IO.Stream GetAssemblyPackageStream(Guid uid)
		{
			var fileName = GetAssemblyPackageLocalFileName(uid);
			if(System.IO.File.Exists(fileName))
				return System.IO.File.OpenRead(fileName);
			return null;
		}

		/// <summary>
		/// Recupera o caminho do arquivo do pacote.
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		private string GetAssemblyPackageLocalFileName(Guid uid)
		{
			return System.IO.Path.Combine(GetRepositoryFolder(), string.Format("{0}.xap", uid));
		}

		/// <summary>
		/// Recupera o pacote a partir do cache.
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <returns></returns>
		private List<Tuple<AssemblyPart, AssemblyPackageCacheEntry>> GetAssemblyPackagesFromCache(IEnumerable<AssemblyPart> assemblyParts)
		{
			while (_isStarting)
				System.Threading.Thread.Sleep(500);
			lock (_objLock)
			{
				var packagesToValidate = new List<AssemblyPackageCacheEntry>();
				var result = new List<Tuple<AssemblyPart, AssemblyPackageCacheEntry>>();
				foreach (var assemblyPart in assemblyParts)
				{
					var found = false;
					for(var i = 0; i < _packages.Count; i++)
					{
						var package = _packages[i];
						using (var enumerator = package.GetEnumerator())
							while (!found && enumerator.MoveNext())
							{
								found = AssemblyPartEqualityComparer.Instance.Equals(enumerator.Current, assemblyPart);
								if(found)
									break;
							}
						if(found)
						{
							packagesToValidate.Add(package);
							result.Add(new Tuple<AssemblyPart, AssemblyPackageCacheEntry>(assemblyPart, package));
							break;
						}
					}
					if(!found)
						result.Add(new Tuple<AssemblyPart, AssemblyPackageCacheEntry>(assemblyPart, null));
				}
				for(var i = 0; i < packagesToValidate.Count; i++)
				{
					var package = packagesToValidate[i];
					if(package.Package != null)
					{
						bool isValid = false;
						try
						{
							isValid = package.IsValid(this);
						}
						catch(Exception ex)
						{
							throw new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepository_ValidateAssemblyPackageCacheEntryError), ex);
						}
						if(!isValid)
						{
							for(var j = 0; j < result.Count; j++)
								if(result[j].Item2 == package)
									result[j] = new Tuple<AssemblyPart, AssemblyPackageCacheEntry>(result[j].Item1, null);
							packagesToValidate.RemoveAt(i--);
							_packages.Remove(package);
							package.Destroy();
						}
					}
				}
				if(_validator != null)
				{
					bool[] validateResult = null;
					try
					{
						validateResult = _validator.Validate(packagesToValidate.Select(f => f.Package).ToArray());
					}
					catch(Exception ex)
					{
						throw new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepository_ValidateAssemblyPackageError), ex);
					}
					for(var i = 0; i < packagesToValidate.Count; i++)
					{
						if(!validateResult[i])
						{
							var package = packagesToValidate[i];
							for(var j = 0; j < result.Count; j++)
								if(result[j].Item2 == package)
									result[j] = new Tuple<AssemblyPart, AssemblyPackageCacheEntry>(result[j].Item1, null);
							_packages.Remove(package);
							package.Destroy();
						}
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Método acionado quando o download for concluído.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void Downloader_DownloadCompleted(object sender, Net.DownloadCompletedEventArgs e)
		{
			var e2 = (AssemblyPackageDownloadCompletedEventArgs)e;
			var state = (DownloaderState)e.UserState;
			if(state == null)
				return;
			if(e2 == null)
			{
				if(state != null)
					state.Release();
				return;
			}
			var packages = new List<AssemblyPackageCacheEntry>();
			Exception exception = null;
			try
			{
				var buffer = new byte[1024];
				var read = 0;
				if(e.Error == null && e2.PackagesResult != null)
				{
					try
					{
						foreach (var i in e2.PackagesResult)
						{
							var packageFile = new System.IO.FileInfo(GetAssemblyPackageLocalFileName(i.Uid));
							if(packageFile.Exists)
								packageFile.Delete();
							var inStream = i.Stream;
							using (var outStream = packageFile.Create())
								while ((read = inStream.Read(buffer, 0, buffer.Length)) > 0)
									outStream.Write(buffer, 0, read);
							packageFile.LastAccessTime = i.LastWriteTime;
							var pkg = new AssemblyPackageResult(_assemblyResolverManager, i.Uid, packageFile.FullName).CreatePackage();
							if(pkg.Count > 0)
								packages.Add(new AssemblyPackageCacheEntry(pkg, null, packageFile.FullName));
							else
							{
								pkg.Dispose();
								packageFile.Delete();
							}
						}
					}
					catch(Exception ex)
					{
						exception = ex;
					}
				}
				else if(e.Error != null)
					exception = e.Error;
				if(_packages != null)
					lock (_objLock)
						_packages.AddRange(packages);
			}
			catch(Exception ex)
			{
				exception = ex;
			}
			finally
			{
				state.DownloadEntries = packages;
				state.Exception = exception;
				state.Release();
			}
		}

		/// <summary>
		/// Recupera o pacote das pastas locais.
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private AssemblyPackageContainer GetAssemblyPackageFromLocal(IEnumerable<AssemblyPart> assemblyParts)
		{
			var assemblyNames = new List<string>();
			List<string> files = new List<string>();
			foreach (var dir in _assemblyFilesDirectories)
			{
				try
				{
					foreach (var file in System.IO.Directory.GetFiles(dir).Select(f => System.IO.Path.GetFileName(f)).OrderBy(f => f))
					{
						var index = files.BinarySearch(file, StringComparer.OrdinalIgnoreCase);
						if(index < 0)
							files.Insert(~index, file);
					}
				}
				catch(Exception ex)
				{
					throw new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepository_GetFilesFromRepositoryDirectoryError, dir), ex);
				}
			}
			var parts2 = assemblyParts.ToArray();
			IComparer<string> comparer = StringComparer.Create(System.Globalization.CultureInfo.InvariantCulture, true);
			foreach (var part in parts2)
			{
				var index = files.BinarySearch(part.Source, comparer);
				if(index >= 0)
					assemblyNames.Add(files[index]);
			}
			var analyzer = new AssemblyAnalyzer();
			var assemblyPaths = new List<string>();
			var names = new List<string>();
			if(_assemblyInfoRepository != null)
			{
				AssemblyInfo info = null;
				Exception exception = null;
				var infos = new Queue<AssemblyInfo>();
				var pending = new Queue<string>(assemblyNames);
				while (pending.Count > 0)
				{
					var assemblyName = pending.Dequeue();
					if(names.FindIndex(f => StringComparer.InvariantCultureIgnoreCase.Equals(f, assemblyName)) < 0)
					{
						if(!_assemblyInfoRepository.TryGet(System.IO.Path.GetFileNameWithoutExtension(assemblyName), out info, out exception))
							continue;
						foreach (var i in info.References)
							pending.Enqueue(i + ".dll");
						names.Add(assemblyName);
						foreach (var dir in _assemblyFilesDirectories)
							if(System.IO.File.Exists(System.IO.Path.Combine(dir, assemblyName)))
								assemblyPaths.Add(System.IO.Path.Combine(dir, assemblyName));
					}
				}
			}
			else
			{
				foreach (var assemblyName in assemblyNames)
				{
					string assemblyPath = null;
					foreach (var dir in _assemblyFilesDirectories)
					{
						assemblyPath = System.IO.Path.Combine(dir, assemblyName);
						if(System.IO.File.Exists(assemblyPath))
							break;
					}
					AsmData data = null;
					try
					{
						data = analyzer.AnalyzeRootAssembly(assemblyPath);
					}
					catch(Exception ex)
					{
						throw new DetailsException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepository_AnalyzeAssemblyError, assemblyName), ex);
					}
					var queue = new Queue<AsmData>();
					queue.Enqueue(data);
					while (queue.Count > 0)
					{
						data = queue.Dequeue();
						if(string.IsNullOrEmpty(data.Path))
							continue;
						var fileName = System.IO.Path.GetFileName(data.Path);
						var index = names.FindIndex(f => StringComparer.InvariantCultureIgnoreCase.Equals(f, fileName));
						if(index < 0)
						{
							names.Add(fileName);
							assemblyPaths.Add(data.Path);
						}
						foreach (var i in data.References)
							if(!string.IsNullOrEmpty(i.Path) && names.FindIndex(f => f == System.IO.Path.GetFileName(i.Path)) < 0)
								queue.Enqueue(i);
					}
				}
			}
			names.Reverse();
			var languages = new IO.Xap.LanguageInfo[] {
				new IO.Xap.LanguageInfo(new string[] {
					".dll"
				}, names.ToArray(), "")
			};
			var configuration = new IO.Xap.XapConfiguration(new IO.Xap.AppManifestTemplate(), languages, null);
			if(assemblyPaths.Count > 0 && UseDirectoryAssemblyPackages)
			{
				return new AssemblyPackageContainer(new DirectoryAssemblyPackage(_assemblyResolverManager, assemblyPaths));
			}
			else if(assemblyPaths.Count > 0)
			{
				assemblyPaths.Reverse();
				var entries = assemblyPaths.Select(f =>  {
					var fileInfo = new System.IO.FileInfo(f);
					return new IO.Xap.XapBuilder.XapEntry(fileInfo.Name, new Lazy<System.IO.Stream>(() => fileInfo.OpenRead()), fileInfo.LastWriteTime);
				}).ToArray();
				var pkgUid = Guid.NewGuid();
				try
				{
					var fileName = GetAssemblyPackageLocalFileName(pkgUid);
					if(System.IO.File.Exists(fileName))
						System.IO.File.Delete(fileName);
					IO.Xap.XapBuilder.XapToDisk(configuration, entries, fileName);
					var pkg = new AssemblyPackageResult(_assemblyResolverManager, pkgUid, fileName).CreatePackage();
					lock (_objLock)
						_packages.Add(new AssemblyPackageCacheEntry(pkg, null, fileName));
					return new AssemblyPackageContainer(pkg);
				}
				finally
				{
					foreach (var i in entries)
						i.Dispose();
				}
			}
			else
			{
				lock (_objLock)
					_packages.Add(new AssemblyPackageCacheEntry(null, parts2, null));
				return null;
			}
		}

		/// <summary>
		/// Método acioando quando o repost
		/// </summary>
		/// <param name="e"></param>
		private void OnStartedInternal(AssemblyRepositoryStartedArgs e)
		{
			_isStarting = false;
			_isStarted = true;
			if(Started != null)
				Started(this, e);
			OnStarted(e);
		}

		/// <summary>
		/// Método acioando quando a instancia
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnStarted(AssemblyRepositoryStartedArgs e)
		{
		}

		/// <summary>
		/// Pasta do repositório.
		/// </summary>
		public string GetRepositoryFolder()
		{
			return _repositoryDirectory;
		}

		/// <summary>
		/// Inicializa o repositório.
		/// </summary>
		public void Start()
		{
			if(!_isStarted && _startThread == null)
			{
				_isStarting = true;
				_startThread = new System.Threading.Thread(DoStart);
				_startThread.Start();
			}
		}

		/// <summary>
		/// Adiciona um novo pacote para o repositório.
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="inputStream"></param>
		[System.Security.SecuritySafeCritical]
		public void Add(Guid uid, System.IO.Stream inputStream)
		{
			var packageFileName = System.IO.Path.Combine(GetRepositoryFolder(), string.Format("{0}.xap", uid));
			if(System.IO.File.Exists(packageFileName))
				System.IO.File.Delete(packageFileName);
			var buffer = new byte[1024];
			var read = 0;
			using (var outputStream = System.IO.File.OpenWrite(packageFileName))
				while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
					outputStream.Write(buffer, 0, read);
		}

		/// <summary>
		/// Recupera o assembly do pacote informado.
		/// </summary>
		/// <param name="package"></param>
		/// <returns></returns>
		public System.IO.Stream GetAssemblyPackageStream(IAssemblyPackage package)
		{
			package.Require("package").NotNull();
			var fileName = GetAssemblyPackageLocalFileName(package.Uid);
			if(System.IO.File.Exists(fileName))
				return System.IO.File.OpenRead(fileName);
			return null;
		}

		/// <summary>
		/// Execute a recuperação dos pacotes de assembly.
		/// </summary>
		/// <param name="callState"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void DoGetAssemblyPackages(object callState)
		{
			var arguments = (object[])callState;
			var asyncResult = (Threading.AsyncResult<AssemblyPackageContainer>)arguments[0];
			var assemblyParts = (IEnumerable<AssemblyPart>)arguments[1];
			AssemblyPackageContainer packageContainer = null;
			try
			{
				assemblyParts = assemblyParts.Distinct(AssemblyPartEqualityComparer.Instance);
				var result = new List<IAssemblyPackage>();
				var assemblyPartsPackage = GetAssemblyPackagesFromCache(assemblyParts);
				var assemblyParts2 = new List<AssemblyPart>();
				foreach (var package in assemblyPartsPackage.GroupBy(f => f.Item2))
				{
					if(package.Key != null)
					{
						result.Add(package.Key.Package);
					}
					else
						assemblyParts2.AddRange(package.Select(f => f.Item1));
				}
				if(assemblyParts2.Count == 0)
				{
					packageContainer = new AssemblyPackageContainer(result);
				}
				else
				{
					if(_assemblyFilesDirectories.Length > 0)
					{
						var container = GetAssemblyPackageFromLocal(assemblyParts2);
						if(container != null)
							result.AddRange(container);
					}
					else if(_downloader != null)
					{
						DownloaderState state = null;
						lock (_downloaderLock)
						{
							var start = DateTime.Now.AddSeconds(60);
							while (_downloader.IsBusy && start > DateTime.Now)
								System.Threading.Thread.Sleep(200);
							if(!_downloader.IsBusy)
							{
								_downloader.Add(new AssemblyPackage(assemblyParts2));
								state = new DownloaderState(this, result, asyncResult);
								_downloader.DownloadCompleted += Downloader_DownloadCompleted;
								_downloader.RunAsync(state);
							}
						}
						return;
					}
					packageContainer = new AssemblyPackageContainer(result.Distinct());
				}
			}
			catch(Exception ex3)
			{
				asyncResult.HandleException(ex3, false);
				return;
			}
			asyncResult.Complete(packageContainer, false);
		}

		/// <summary>
		/// Inicializa o processo para recuperar a stream do pacote de 
		/// assembly associado com as parts de assembly informadas.
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IAsyncResult BeginGetAssemblyPackages(IEnumerable<AssemblyPart> assemblyParts, AsyncCallback callback, object state)
		{
			var asyncResult = new Threading.AsyncResult<AssemblyPackageContainer>(callback, state);
			var arguments = new object[] {
				asyncResult,
				assemblyParts
			};
			if(!System.Threading.ThreadPool.QueueUserWorkItem(DoGetAssemblyPackages, arguments))
				DoGetAssemblyPackages(arguments);
			return asyncResult;
		}

		/// <summary>
		/// Finaliza o processa para recuperar a stream do pacote de 
		/// assembly associado com as parts de assembly informadas.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns></returns>
		public AssemblyPackageContainer EndGetAssemblyPackages(IAsyncResult ar)
		{
			var asyncResult = (Threading.AsyncResult<AssemblyPackageContainer>)ar;
			if(asyncResult.Exception != null)
				throw asyncResult.Exception;
			return asyncResult.Result;
		}

		/// <summary>
		/// Recupera o pacote que se enquadra nas partes informadas.
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public AssemblyPackageContainer GetAssemblyPackages(IEnumerable<AssemblyPart> assemblyParts)
		{
			var allDone = new System.Threading.ManualResetEvent(false);
			var asyncResult = new Threading.AsyncResult<AssemblyPackageContainer>(ar =>  {
				try
				{
					allDone.Set();
				}
				catch(System.IO.IOException)
				{
					try
					{
						allDone.Dispose();
					}
					catch
					{
					}
				}
			}, null);
			var arguments = new object[] {
				asyncResult,
				assemblyParts
			};
			DoGetAssemblyPackages(arguments);
			try
			{
				allDone.WaitOne();
			}
			catch(ObjectDisposedException)
			{
			}
			if(asyncResult.Exception != null)
				throw asyncResult.Exception;
			return asyncResult.Result;
		}

		/// <summary>
		/// Recupera o pacote pelo identificador informado.
		/// </summary>
		/// <param name="assemblyPackageUid">Identificador do pacote que será recuperado.</param>
		/// <returns></returns>
		public IAssemblyPackage GetAssemblyPackage(Guid assemblyPackageUid)
		{
			AssemblyPackageCacheEntry item = null;
			lock (_objLock)
				item = _packages.Find(f => f.Package != null && f.Package.Uid == assemblyPackageUid);
			if(item != null)
				return item.Package;
			return null;
		}

		/// <summary>
		/// Verifica se os dados são válidos.
		/// </summary>
		public AssemblyRepositoryValidateResult Validate()
		{
			var result = new List<AssemblyRepositoryValidateResult.Entry>();
			foreach (var instance in _maintenanceInstances)
				try
				{
					var executeResult = instance.Execute();
					if(executeResult.HasError)
					{
						result.Add(new AssemblyRepositoryValidateResult.Entry(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepository_ValidateMaintenanceError, instance.Name), AssemblyRepositoryValidateResult.EntryType.Error));
						foreach (var i in executeResult.Where(f => f.Type == AssemblyRepositoryMaintenanceExecuteResult.EntryType.Error))
							result.Add(new AssemblyRepositoryValidateResult.Entry(i.Message, AssemblyRepositoryValidateResult.EntryType.Error, i.Error));
					}
				}
				catch(Exception ex)
				{
					result.Add(new AssemblyRepositoryValidateResult.Entry(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepository_MaintenanceError, instance.Name, Diagnostics.ExceptionFormatter.FormatException(ex, true)), AssemblyRepositoryValidateResult.EntryType.Error, ex));
				}
			return new AssemblyRepositoryValidateResult(result);
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
			if(_downloader != null)
			{
				_downloader.Dispose();
				_downloader = null;
			}
			if(_startThread != null)
			{
				_startThread.Abort();
				_startThread = null;
			}
			if(_assemblyResolverManager != null)
			{
				_assemblyResolverManager.Dispose();
				_assemblyResolverManager = null;
			}
			foreach (var i in _packages)
				i.Dispose();
			_packages.Clear();
		}

		/// <summary>
		/// Armazena os dados do estado do download.
		/// </summary>
		class DownloaderState
		{
			/// <summary>
			/// Relação dos pacotes já carregados anteriormente.
			/// </summary>
			public IEnumerable<IAssemblyPackage> Packages;

			/// <summary>
			/// Relação da entradas de pacotes de foram carregadas do download.
			/// </summary>
			public IEnumerable<AssemblyPackageCacheEntry> DownloadEntries;

			public Exception Exception;

			public Threading.AsyncResult<AssemblyPackageContainer> AsyncResult;

			public AssemblyRepository Owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			/// <param name="packages"></param>
			/// <param name="ar"></param>
			public DownloaderState(AssemblyRepository owner, IEnumerable<IAssemblyPackage> packages, Threading.AsyncResult<AssemblyPackageContainer> ar)
			{
				this.Owner = owner;
				this.Packages = packages;
				this.AsyncResult = ar;
			}

			/// <summary>
			/// Libera o estado.
			/// </summary>
			public void Release()
			{
				var result = new List<IAssemblyPackage>(Packages);
				Owner._downloader.DownloadCompleted -= Owner.Downloader_DownloadCompleted;
				if(Exception != null)
				{
					AsyncResult.HandleException(Exception, false);
					return;
				}
				if(DownloadEntries != null)
					foreach (var package in DownloadEntries)
						if(!result.Exists(f => f.Uid == package.Package.Uid))
							result.Add(package.Package);
				AsyncResult.Complete(new AssemblyPackageContainer(result.Distinct()), false);
			}
		}

		/// <summary>
		/// Representa uma entrada do cache de um pacote.
		/// </summary>
		sealed class AssemblyPackageCacheEntry : IEnumerable<AssemblyPart>, IDisposable
		{
			private AssemblyPackage _package;

			private IEnumerable<AssemblyPart> _parts;

			/// <summary>
			/// Nome do arquivo do pacote.
			/// </summary>
			private string _fileName;

			/// <summary>
			/// Instancia do pacote.
			/// </summary>
			public AssemblyPackage Package
			{
				get
				{
					return _package;
				}
			}

			/// <summary>
			/// Parte da entrada.
			/// </summary>
			public IEnumerable<AssemblyPart> Parts
			{
				get
				{
					return _parts ?? _package;
				}
			}

			/// <summary>
			/// Cria uma nova instancia com base nos dados do pacote.
			/// </summary>
			/// <param name="package"></param>
			public AssemblyPackageCacheEntry(AssemblyPackage package)
			{
				_package = package;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="package">Instancia do pacote associado.</param>
			/// <param name="parts">Partes do pacote.</param>
			/// <param name="fileName">Caminho do arquivo do pacote.</param>
			public AssemblyPackageCacheEntry(AssemblyPackage package, IEnumerable<AssemblyPart> parts, string fileName)
			{
				_package = package;
				_parts = parts;
				_fileName = fileName;
			}

			/// <summary>
			/// Verifica se a entrada é válida.
			/// </summary>
			/// <param name="assemblyRepository"></param>
			/// <returns></returns>
			public bool IsValid(AssemblyRepository assemblyRepository)
			{
				var assembliesDirectories = assemblyRepository._assemblyFilesDirectories.Where(f => !string.IsNullOrEmpty(f) && System.IO.Directory.Exists(f)).ToList();
				if(!string.IsNullOrEmpty(_fileName) && assembliesDirectories.Count > 0)
				{
					var fileInfo = new System.IO.FileInfo(_fileName);
					if(!fileInfo.Exists)
						return false;
					foreach (var i in Parts.Select(f => f.Source))
					{
						foreach (var directory in assembliesDirectories)
						{
							var partFileInfo = new System.IO.FileInfo(System.IO.Path.Combine(directory, i));
							if(partFileInfo.Exists)
								return partFileInfo.LastWriteTime == fileInfo.LastWriteTime;
						}
					}
				}
				else if(Package != null)
					return true;
				return false;
			}

			/// <summary>
			/// Destrói o pacote.
			/// </summary>
			public void Destroy()
			{
				if(!string.IsNullOrEmpty(_fileName))
				{
					try
					{
						System.IO.File.Delete(_fileName);
					}
					catch
					{
					}
				}
			}

			/// <summary>
			/// Recupera o enumerador das partes.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<AssemblyPart> GetEnumerator()
			{
				return (_parts ?? _package).GetEnumerator();
			}

			/// <summary>
			/// Recupera o enumerador das partes.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return (_parts ?? _package).GetEnumerator();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_package != null)
				{
					_package.Dispose();
					_package = null;
				}
			}
		}
	}
}
