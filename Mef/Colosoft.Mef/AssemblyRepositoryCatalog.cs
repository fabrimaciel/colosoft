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
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;

namespace Colosoft.Mef
{
	/// <summary>
	/// Implementação de um catalogo do repositório de assemblies.
	/// </summary>
	public class AssemblyRepositoryCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
	{
		private Lazy<Colosoft.Reflection.IAssemblyRepository> _assemblyRepository;

		private Lazy<Colosoft.Reflection.Composition.IExportManager> _exportManager;

		private string _uiContext;

		private AggregateCatalog _catalogCollection;

		private AggregateAssemblyRepositoryCatalogObserver _observer;

		private object _lock = new object();

		private volatile bool _isDisposed;

		/// <summary>
		/// Tipos registrados no catálogos.
		/// </summary>
		private List<ContractInfo> _contracts = new List<ContractInfo>();

		/// <summary>
		/// Evento acionado quando o catálogo for alterado.
		/// </summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		/// <summary>
		/// Evento acionado quando o catálogo estiver sendo alterado.
		/// </summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		/// <summary>
		/// Evento acionado quando ocorre uma falha na carga de um tipo.
		/// </summary>
		public event FailOnLoadTypeHandler FailOnLoadType;

		/// <summary>
		/// Evento acionado quando ocorre uma falha na carga do assembly.
		/// </summary>
		public event FailOnLoadAssemblyHandler FailOnLoadAssembly;

		/// <summary>
		/// Evento acionado quando ocorre uma falha na carga dos pacotes.
		/// </summary>
		public event FailOnLoadPackagesHandler FailOnLoadPackages;

		/// <summary>
		/// Evento acionado quando o assembly do export não for encontrado.
		/// </summary>
		public event AssemblyFromExportNotFoundHandler AssemblyFromExportNotFound;

		/// <summary>
		/// Instancia do repositório carregado.
		/// </summary>
		protected Colosoft.Reflection.IAssemblyRepository AssemblyRepository
		{
			get
			{
				var repos = _assemblyRepository.Value;
				if(repos == null)
					throw new InvalidOperationException("AssemblyRepository undefined.");
				return repos;
			}
		}

		/// <summary>
		/// Instancia do gerenciador de exportação.
		/// </summary>
		public Colosoft.Reflection.Composition.IExportManager ExportManager
		{
			get
			{
				var manager = _exportManager.Value;
				if(manager == null)
					throw new InvalidOperationException("IExportManager undefined.");
				return manager;
			}
		}

		/// <summary>
		/// Instancia do observer associado.
		/// </summary>
		public AggregateAssemblyRepositoryCatalogObserver Observer
		{
			get
			{
				return _observer;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblyRepository">Repositório dos assemblies.</param>
		/// <param name="exportManager">Gerenciador das exportações.</param>
		/// <param name="uiContext">Contexto de interface com o usuário que será utilizado.</param>
		public AssemblyRepositoryCatalog(Lazy<Colosoft.Reflection.IAssemblyRepository> assemblyRepository, Lazy<Colosoft.Reflection.Composition.IExportManager> exportManager, string uiContext)
		{
			assemblyRepository.Require("assemblyRepository").NotNull();
			exportManager.Require("exportManager").NotNull();
			_assemblyRepository = assemblyRepository;
			_exportManager = exportManager;
			_uiContext = uiContext;
			_observer = new AggregateAssemblyRepositoryCatalogObserver();
		}

		/// <summary>
		/// Dispara uma exception caso a instancia já tenha sido liberada.
		/// </summary>
		private void ThrowIfDisposed()
		{
			if(_isDisposed)
				throw new ObjectDisposedException(this.GetType().ToString());
		}

		/// <summary>
		/// Método acioando quando ocorre uma falha ao carrega o tipo.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="exception"></param>
		protected virtual void OnFailLoadType(Colosoft.Reflection.TypeName type, Exception exception)
		{
			var e = new FailOnLoadTypeEventArgs(type, exception);
			if(FailOnLoadType != null)
				FailOnLoadType(this, e);
			_observer.FailOnLoadType(e);
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar o assemlbgyu
		/// </summary>
		/// <param name="name"></param>
		/// <param name="exception"></param>
		protected virtual void OnFailLoadAssembly(System.Reflection.AssemblyName name, Exception exception)
		{
			var e = new FailOnLoadAssemblyArgs(name, exception);
			if(FailOnLoadAssembly != null)
				FailOnLoadAssembly(this, e);
			_observer.FailOnLoadAssembly(e);
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha na carga dos pacotes.
		/// </summary>
		/// <param name="assemblyParts">Partes dos assemblies que serão usadas na carga.</param>
		/// <param name="error">Exception ocorrida.</param>
		protected virtual void OnFailLoadPackages(Colosoft.Reflection.AssemblyPart[] assemblyParts, Exception error)
		{
			var e = new FailOnLoadPackagesArgs(assemblyParts, error);
			if(FailOnLoadPackages != null)
				FailOnLoadPackages(this, e);
			_observer.FailOnLoadPackages(e);
		}

		/// <summary>
		/// Método acionado quando o assembly do export não for encontrado.
		/// </summary>
		/// <param name="export"></param>
		/// <param name="assemblyName"></param>
		/// <param name="exception">Erro associado.</param>
		protected virtual void OnAssemblyFromExportNotFound(Colosoft.Reflection.Composition.IExport export, System.Reflection.AssemblyName assemblyName, Exception exception)
		{
			if(AssemblyFromExportNotFound != null)
			{
				var args = new AssemblyFromExportNotFoundEventArgs(export, assemblyName, exception);
				AssemblyFromExportNotFound(this, args);
				_observer.AssemblyFromExportNotFound(args);
				if(args.IsErrorHandled)
					return;
			}
			throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.InvalidOperation_AssemblyFromExportNotFound, assemblyName, export).Format(), exception);
		}

		/// <summary>
		/// Método acionado quando o catálogo for alterado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			if(Changed != null)
				Changed(this, e);
		}

		/// <summary>
		/// Método acionado quando o catálogo estiver sendo alterado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			if(Changing != null)
				Changing(this, e);
		}

		/// <summary>
		/// Recupera os tipos do catálogo.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void DiscoverTypes(IEnumerable<ContractInfo> contracts)
		{
			lock (_lock)
			{
				if(_catalogCollection == null)
					_catalogCollection = new AggregateCatalog();
				var exports = new List<Tuple<ContractInfo, Colosoft.Reflection.Composition.IExport>>();
				foreach (var i in contracts)
				{
					if(i.Types.Count == 0)
					{
						var tuple = new Tuple<ContractInfo, Reflection.Composition.IExport>(i, ExportManager.GetExport(i.ContractType, i.ContractName, _uiContext));
						if(tuple.Item2 != null)
							exports.Add(tuple);
					}
					else
					{
						var contractExports = ExportManager.GetExports(i.ContractType, i.ContractName, _uiContext);
						int contractCount = 0;
						foreach (var j in contractExports)
						{
							if(i.Types.Contains(j.Type, Colosoft.Reflection.TypeName.TypeNameEqualityComparer.Instance))
							{
								contractCount++;
								var tuple = new Tuple<ContractInfo, Reflection.Composition.IExport>(i, j);
								exports.Add(tuple);
							}
						}
					}
				}
				if(exports.Count > 0)
				{
					var pkgParts = new List<Colosoft.Reflection.AssemblyPart>();
					var partDescriptions = new List<PartDescription>();
					foreach (var i in exports)
					{
						var assemblyPart = new Colosoft.Reflection.AssemblyPart(string.Format("{0}.dll", i.Item2.Type.AssemblyName.Name));
						if(!pkgParts.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Source, assemblyPart.Source)))
							pkgParts.Add(assemblyPart);
						System.ComponentModel.Composition.CreationPolicy creationPolicy = System.ComponentModel.Composition.CreationPolicy.Any;
						switch(i.Item2.CreationPolicy)
						{
						case Reflection.Composition.CreationPolicy.NonShared:
							creationPolicy = System.ComponentModel.Composition.CreationPolicy.NonShared;
							break;
						case Reflection.Composition.CreationPolicy.Shared:
							creationPolicy = System.ComponentModel.Composition.CreationPolicy.Shared;
							break;
						}
						ImportingConstructorDescription importingConstructor = null;
						if(i.Item2.ImportingConstructor)
							importingConstructor = new ImportingConstructorDescription(i.Item2.Type, null);
						var partDescription = new PartDescription(i.Item2.Type, new ExportDescription[] {
							new ExportDescription {
								ContractTypeName = i.Item1.ContractType,
								ContractName = string.IsNullOrEmpty(i.Item2.ContractName) ? null : i.Item2.ContractName.Trim(),
								Metadata = i.Item2.Metadata
							}
						}, new ImportDescription[0], i.Item2.UseDispatcher, importingConstructor, creationPolicy);
						partDescriptions.Add(partDescription);
					}
					var catalog = new DefinitionCatalog(new DiscoverAssembliesContainer(this, _assemblyRepository, pkgParts.ToArray(), exports), partDescriptions);
					var addedDefinitions = catalog.Parts;
					using (AtomicComposition composition = new AtomicComposition())
					{
						var args = new ComposablePartCatalogChangeEventArgs(addedDefinitions, Enumerable.Empty<ComposablePartDefinition>(), composition);
						this.OnChanging(args);
						_catalogCollection.Catalogs.Add(catalog);
						composition.Complete();
					}
					ComposablePartCatalogChangeEventArgs e = new ComposablePartCatalogChangeEventArgs(addedDefinitions, Enumerable.Empty<ComposablePartDefinition>(), null);
					this.OnChanged(e);
				}
			}
		}

		/// <summary>
		/// Método acionado quando a instancia estiver sido inicializada.
		/// </summary>
		protected void Initialized()
		{
			var exportManager = _exportManager.Value;
			var exports = exportManager.GetExports(_uiContext);
			var groups = new Dictionary<string, IList<Colosoft.Reflection.Composition.IExport>>();
			foreach (var export in exports)
			{
				string typeAssembly = export.Type.AssemblyName.FullName;
				if(!groups.ContainsKey(typeAssembly))
				{
					var list = new List<Colosoft.Reflection.Composition.IExport>();
					list.Add(export);
					groups.Add(typeAssembly, list);
				}
				else
				{
					var list = groups[typeAssembly];
					var index = list.BinarySearch(export, Colosoft.Reflection.Composition.ExportComparer.Instance);
					if(index < 0)
						list.Insert(~index, export);
					else
					{
						var item = list[index];
						if(string.IsNullOrEmpty(item.ContractName) && string.IsNullOrEmpty(export.ContractName))
							list.Insert(index, export);
						else
							throw new AssemblyRepositoryCatalogException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepositoryCatalog_DuplicateExport, export.ContractType, export.ContractName).Format());
					}
				}
			}
			foreach (var group in groups)
			{
				var catalogRegister = new AssemblyRepositoryCatalogRegister();
				foreach (var export in group.Value)
					catalogRegister.Add(export.ContractType, export.ContractName, export.Type);
				var conflict = Add(catalogRegister).FirstOrDefault();
				if(conflict != null)
					throw new AssemblyRepositoryCatalogException(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyRepositoryCatalog_DuplicateExport, conflict.ContractType, conflict.ContractName).Format());
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
				_isDisposed = true;
			_catalogCollection.Dispose();
			base.Dispose(disposing);
		}

		/// <summary>
		/// Adiciona um novo registrador para o catálogo.
		/// </summary>
		/// <param name="register"></param>
		public AssemblyRepositoryCatalogConflict[] Add(AssemblyRepositoryCatalogRegister register)
		{
			var contractsTemp = new List<ContractInfo>();
			var conflicts = new List<AssemblyRepositoryCatalogConflict>();
			foreach (var i in register.Contracts)
			{
				var index = _contracts.FindIndex(f => ContractInfoEqualityComparer.Instance.Equals(f, i));
				if(index < 0)
				{
					contractsTemp.Add(i);
					_contracts.Add(i);
				}
				else
				{
					conflicts.Add(new AssemblyRepositoryCatalogConflict(i.ContractName, i.ContractType));
				}
			}
			if(_catalogCollection != null && contractsTemp.Count > 0)
				DiscoverTypes(contractsTemp);
			return conflicts.ToArray();
		}

		/// <summary>
		/// Partes carregadas pelo catálogo.
		/// </summary>
		public override IQueryable<ComposablePartDefinition> Parts
		{
			get
			{
				this.ThrowIfDisposed();
				if(_catalogCollection == null)
				{
					DiscoverTypes(_contracts);
					Initialized();
				}
				return _catalogCollection.Parts;
			}
		}

		/// <summary>
		/// Recupera os exports com base na definição de import informada.
		/// </summary>
		/// <param name="definition"></param>
		/// <returns></returns>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			var exports = base.GetExports(definition).ToList();
			return exports;
		}

		/// <summary>
		/// Armazena as informações do contrato.
		/// </summary>
		internal class ContractInfo
		{
			private Colosoft.Reflection.TypeName _contractTypeName;

			private string _contractName;

			private List<Colosoft.Reflection.TypeName> _types = new List<Reflection.TypeName>();

			/// <summary>
			/// Nome do tipo de contrato.
			/// </summary>
			public Colosoft.Reflection.TypeName ContractType
			{
				get
				{
					return _contractTypeName;
				}
			}

			/// <summary>
			/// Nome do contrato.
			/// </summary>
			public string ContractName
			{
				get
				{
					return _contractName;
				}
			}

			/// <summary>
			/// Relação dos tipos associados com o contrato.
			/// </summary>
			public List<Colosoft.Reflection.TypeName> Types
			{
				get
				{
					return _types;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="contractTypeName"></param>
			/// <param name="contractName"></param>
			/// <param name="type"></param>
			public ContractInfo(Colosoft.Reflection.TypeName contractTypeName, string contractName, Colosoft.Reflection.TypeName type)
			{
				_contractName = contractName;
				_contractTypeName = contractTypeName;
				if(type != null)
					_types.Add(type);
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("[ContractName: {0}, ContractType: {1}]", _contractName, _contractTypeName);
			}
		}

		/// <summary>
		/// Implementação usada para compara as informações do contrato.
		/// </summary>
		class ContractInfoEqualityComparer : IEqualityComparer<ContractInfo>
		{
			/// <summary>
			/// Instancia única do comparador.
			/// </summary>
			public readonly static ContractInfoEqualityComparer Instance = new ContractInfoEqualityComparer();

			/// <summary>
			/// Verifica se as instancia informadas são compatíveis.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(ContractInfo x, ContractInfo y)
			{
				return ((x == null && y == null) || (x != null && y != null && x.ContractName == y.ContractName && Colosoft.Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(x.ContractType, y.ContractType) && (x.Types.Count == 0 || y.Types.Count == 0 || x.Types.Intersect(y.Types, Colosoft.Reflection.TypeName.TypeNameEqualityComparer.Instance).Any())));
			}

			/// <summary>
			/// HashCode.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(ContractInfo obj)
			{
				if(obj == null)
					return 0;
				return string.Format("[{0} : {1}]", obj.ContractType, obj.ContractName).GetHashCode();
			}
		}

		/// <summary>
		/// Representa o container dos assemblies descobertos.
		/// </summary>
		class DiscoverAssembliesContainer : IAssemblyContainer
		{
			private AssemblyRepositoryCatalog _assemblyRepositoryCatalog;

			private Colosoft.Reflection.AssemblyPart[] _packageParts;

			private IEnumerable<Tuple<ContractInfo, Colosoft.Reflection.Composition.IExport>> _exports;

			private Lazy<Colosoft.Reflection.IAssemblyRepository> _assemblyRepository;

			private Dictionary<string, System.Reflection.Assembly> _assemblies;

			private object _objLock = new object();

			private bool _isInitializing = false;

			/// <summary>
			/// Assemblies associados.
			/// </summary>
			private Dictionary<string, System.Reflection.Assembly> Assemblies
			{
				get
				{
					if(_isInitializing || (_assemblies == null || _assemblies.Count == 0))
						lock (_objLock)
							if(!_isInitializing || _assemblies == null || _assemblies.Count == 0)
							{
								_isInitializing = true;
								using (Colosoft.Diagnostics.Trace.CreateOperation("DISCOVER ASSEMBLIES PARTS: [{0}]", string.Join(",", _packageParts.Select(f => string.Format("\"{0}\"", f.Source)).ToArray())))
									Initialize();
							}
					return _assemblies;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="assemblyRepositoryCatalog">Catalogo do repositório de assemblies.</param>
			/// <param name="assemblyRepository">Instancia do repositório dos assemblies.</param>
			/// <param name="packageParts">Partes do pacote que deverá ser descoberto.</param>
			/// <param name="exports">Exports.</param>
			public DiscoverAssembliesContainer(AssemblyRepositoryCatalog assemblyRepositoryCatalog, Lazy<Colosoft.Reflection.IAssemblyRepository> assemblyRepository, Colosoft.Reflection.AssemblyPart[] packageParts, IEnumerable<Tuple<ContractInfo, Colosoft.Reflection.Composition.IExport>> exports)
			{
				_assemblyRepositoryCatalog = assemblyRepositoryCatalog;
				_assemblyRepository = assemblyRepository;
				_packageParts = packageParts;
				_exports = exports;
			}

			/// <summary>
			/// Inicializa a instancia.
			/// </summary>
			private void Initialize()
			{
				_assemblies = new Dictionary<string, System.Reflection.Assembly>(StringComparer.InvariantCultureIgnoreCase);
				var assemblyParts = new List<Colosoft.Reflection.AssemblyPart>(_packageParts);
				var resolverManager = _assemblyRepository.Value.AssemblyResolverManager;
				for(var i = 0; i < assemblyParts.Count; i++)
				{
					var assemblyName = System.IO.Path.GetFileNameWithoutExtension(assemblyParts[i].Source);
					if(resolverManager.CheckAssembly(assemblyParts[i].Source))
					{
						try
						{
							var asm = resolverManager.AppDomain.Load(assemblyName);
							if(asm != null)
							{
								if(!_assemblies.ContainsKey(assemblyName))
									_assemblies.Add(assemblyName, asm);
								assemblyParts.RemoveAt(i--);
							}
						}
						catch(Exception ex)
						{
							Exception exception = ex;
							if(exception is AggregateException)
								exception = ((AggregateException)exception).InnerException;
							if(exception is System.Reflection.ReflectionTypeLoadException)
								ex = exception;
							_assemblyRepositoryCatalog.OnFailLoadAssembly(new System.Reflection.AssemblyName(assemblyParts[i].Source), ex);
						}
					}
				}
				using (var resolver = new Colosoft.Reflection.AssemblyPartsResolver(_assemblyRepository.Value, assemblyParts))
				{
					resolverManager.Insert(0, resolver);
					try
					{
						foreach (var assemblyName in assemblyParts.Select(f => f.Source))
						{
							System.Reflection.Assembly assembly = null;
							try
							{
								assembly = resolverManager.AppDomain.Load(new System.Reflection.AssemblyName(assemblyName));
							}
							catch(Exception ex)
							{
								if(ex.InnerException is Colosoft.Reflection.AssemblyResolverException)
									ex = ex.InnerException;
								if(ex.InnerException is Colosoft.IO.Xap.LoadXapPackageAssembliesException)
									ex = ex.InnerException;
								Exception exception = ex;
								if(exception is AggregateException)
									exception = ((AggregateException)exception).InnerException;
								if(exception is System.Reflection.ReflectionTypeLoadException)
									ex = exception;
								_assemblyRepositoryCatalog.OnFailLoadAssembly(new System.Reflection.AssemblyName(assemblyName), ex);
								continue;
							}
							_assemblies.Add(assembly.GetName().Name, assembly);
						}
					}
					finally
					{
						resolverManager.Remove(resolver);
					}
				}
				var partDescriptions = new List<PartDescription>();
				foreach (var i in _exports)
				{
					System.Reflection.Assembly assembly = null;
					if(!_assemblies.TryGetValue(i.Item2.Type.AssemblyName.Name, out assembly))
					{
						Exception lastException = null;
						try
						{
							assembly = resolverManager.AppDomain.Load(i.Item2.Type.AssemblyName.Name);
						}
						catch(Exception ex)
						{
							lastException = ex;
						}
						if(assembly == null)
						{
							_assemblyRepositoryCatalog.OnAssemblyFromExportNotFound(i.Item2, i.Item2.Type.AssemblyName, lastException);
							continue;
						}
					}
					Type partType = null;
					try
					{
						partType = assembly.GetType(i.Item2.Type.FullName, true);
					}
					catch(Exception ex)
					{
						_assemblyRepositoryCatalog.OnFailLoadType(i.Item2.Type, ex);
						continue;
					}
					if(partType == null)
					{
						_assemblyRepositoryCatalog.OnFailLoadType(i.Item2.Type, null);
						continue;
					}
				}
			}

			/// <summary>
			/// Tenta recuperar o assembly pelo nome informado.
			/// </summary>
			/// <param name="assemblyName">Nome do assembly que deverá ser recuperado.</param>
			/// <param name="assembly"></param>
			/// <returns></returns>
			public bool TryGet(string assemblyName, out System.Reflection.Assembly assembly)
			{
				return Assemblies.TryGetValue(assemblyName, out assembly);
			}

			/// <summary>
			/// Tenta recuperar o assembly pelo nome informado.
			/// </summary>
			/// <param name="assemblyName">Nome do assembly que deverá ser recuperado.</param>
			/// <param name="assembly">Instancia do assembly recuperado.</param>
			/// <param name="exception">Error ocorrido na recuperação do assembly.</param>
			/// <returns></returns>
			public bool TryGet(string assemblyName, out System.Reflection.Assembly assembly, out Exception exception)
			{
				exception = null;
				return Assemblies.TryGetValue(assemblyName, out assembly);
			}

			/// <summary>
			/// Tenta carrega os assemblies informados.
			/// </summary>
			/// <param name="assemblyNames">Nomes dos assemblies que serão carregados.</param>
			/// <returns></returns>
			public Colosoft.Reflection.AssemblyLoaderGetResult Get(string[] assemblyNames)
			{
				var result = new List<Colosoft.Reflection.AssemblyLoaderGetResult.Entry>();
				foreach (var assemblyName in assemblyNames)
				{
					System.Reflection.Assembly assembly = null;
					Exception exception = null;
					var getResult = TryGet(assemblyName, out assembly, out exception);
					result.Add(new Reflection.AssemblyLoaderGetResult.Entry(assemblyName, assembly, getResult, exception));
				}
				return new Reflection.AssemblyLoaderGetResult(result);
			}

			/// <summary>
			/// Recupera o enumerador dos assemblies.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<System.Reflection.Assembly> GetEnumerator()
			{
				return Assemblies.Values.GetEnumerator();
			}

			/// <summary>
			/// Recupera o enumerador dos assemblies.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return Assemblies.Values.GetEnumerator();
			}
		}
	}
}
