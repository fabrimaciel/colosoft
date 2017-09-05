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
	/// Implementação do repositório a partir do dominio da palicação.
	/// </summary>
	public class AppDomainAssemblyRepository : IAssemblyRepository
	{
		private bool _isStarted;

		private Dictionary<Guid, AssemblyPackage> _packages = new Dictionary<Guid, AssemblyPackage>();

		private AssemblyResolverManager _assemblyResolverManager;

		/// <summary>
		/// Evento acioando quando a instancia for iniciada.
		/// </summary>
		public event AssemblyRepositoryStartedHandler Started;

		/// <summary>
		/// Identifica se a instancia já foi iniciada.
		/// </summary>
		public bool IsStarted
		{
			get
			{
				return _isStarted;
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
		public AppDomainAssemblyRepository() : this(AppDomain.CurrentDomain)
		{
		}

		/// <summary>
		/// Cria a instancia para o domínio informado.
		/// </summary>
		/// <param name="domain"></param>
		public AppDomainAssemblyRepository(AppDomain domain)
		{
			domain.Require("domain").NotNull();
			_assemblyResolverManager = new AssemblyResolverManager(domain);
		}

		/// <summary>
		/// Cria a instancia do o gerenciador de resolução de assemblies.
		/// </summary>
		/// <param name="assemblyResolverManager"></param>
		public AppDomainAssemblyRepository(AssemblyResolverManager assemblyResolverManager)
		{
			assemblyResolverManager.Require("assemblyResolverManager").NotNull();
			_assemblyResolverManager = assemblyResolverManager;
		}

		/// <summary>
		/// Método acioando quando a instancia
		/// </summary>
		/// <param name="e"></param>
		protected void OnStarted(AssemblyRepositoryStartedArgs e)
		{
			if(Started != null)
				Started(this, e);
		}

		/// <summary>
		/// Execute a recuperação dos pacotes de assembly.
		/// </summary>
		/// <param name="callState"></param>
		private void DoGetAssemblyPackages(object callState)
		{
			var arguments = (object[])callState;
			var asyncResult = (Threading.AsyncResult<AssemblyPackageContainer>)arguments[0];
			var assemblyParts = (IEnumerable<AssemblyPart>)arguments[1];
			AssemblyPackageContainer packageContainer = null;
			try
			{
				packageContainer = GetAssemblyPackages(assemblyParts);
			}
			catch(Exception ex)
			{
				asyncResult.HandleException(ex, false);
				return;
			}
			asyncResult.Complete(packageContainer, false);
		}

		/// <summary>
		/// Inicia a instancia.
		/// </summary>
		public void Start()
		{
			if(_isStarted)
				return;
			_isStarted = true;
			OnStarted(new AssemblyRepositoryStartedArgs(null));
		}

		/// <summary>
		/// Adiciona um novo pacote para o repositório.
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="inputStream"></param>
		public void Add(Guid uid, System.IO.Stream inputStream)
		{
			throw new NotSupportedException();
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
		/// Recupera os dados do pacote
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public AssemblyPackageContainer GetAssemblyPackages(IEnumerable<AssemblyPart> assemblyParts)
		{
			var sourceParts = assemblyParts.ToList();
			var assemblies = new List<AssemblyPart>();
			foreach (var i in AssemblyResolverManager.AppDomain.GetAssemblies())
			{
				for(var j = 0; j < sourceParts.Count; j++)
					if(string.Compare(sourceParts[j].Source, string.Concat(i.GetName().Name, ".dll"), true) == 0)
					{
						sourceParts[j].Assembly = i;
						assemblies.Add(sourceParts[j]);
						sourceParts.RemoveAt(j--);
					}
			}
			var pkg = new AssemblyPackage(assemblies) {
				Uid = Guid.NewGuid()
			};
			_packages.Add(pkg.Uid, pkg);
			return new AssemblyPackageContainer(pkg);
		}

		/// <summary>
		/// Recupera a stream do pacote do assembly.
		/// </summary>
		/// <param name="package"></param>
		/// <returns></returns>
		public System.IO.Stream GetAssemblyPackageStream(IAssemblyPackage package)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera o pacote do assembly.
		/// </summary>
		/// <param name="assemblyPackageUid"></param>
		/// <returns></returns>
		public IAssemblyPackage GetAssemblyPackage(Guid assemblyPackageUid)
		{
			AssemblyPackage pkg = null;
			if(_packages.TryGetValue(assemblyPackageUid, out pkg))
				return pkg;
			return null;
		}

		/// <summary>
		/// Valida o repositório.
		/// </summary>
		public AssemblyRepositoryValidateResult Validate()
		{
			return new AssemblyRepositoryValidateResult(new AssemblyRepositoryValidateResult.Entry[0]);
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_assemblyResolverManager")]
		protected virtual void Dispose(bool disposing)
		{
			_packages.Clear();
		}
	}
}
