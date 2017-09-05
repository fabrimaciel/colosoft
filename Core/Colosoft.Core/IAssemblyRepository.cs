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
	/// Argumentos para o evento que identifica a inicialização do repositório.
	/// </summary>
	public class AssemblyRepositoryStartedArgs : EventArgs
	{
		/// <summary>
		/// Exception caso tenha ocorrido.
		/// </summary>
		public Exception[] Exceptions
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="exceptions"></param>
		public AssemblyRepositoryStartedArgs(Exception[] exceptions)
		{
			this.Exceptions = exceptions;
		}
	}
	/// <summary>
	/// Representa o evento acionado quando o repositório for iniciado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void AssemblyRepositoryStartedHandler (object sender, AssemblyRepositoryStartedArgs e);
	/// <summary>
	/// Assinatura de um repositório de assemblies.
	/// </summary>
	public interface IAssemblyRepository : IDisposable
	{
		/// <summary>
		/// Evento acionado quando o repositório for iniciado.
		/// </summary>
		event AssemblyRepositoryStartedHandler Started;

		/// <summary>
		/// Identifica se o repositório já foi iniciado.
		/// </summary>
		bool IsStarted
		{
			get;
		}

		/// <summary>
		/// Instancia do gerencia de resolução de assemblies associado com repositório.
		/// </summary>
		AssemblyResolverManager AssemblyResolverManager
		{
			get;
		}

		/// <summary>
		/// Inicializa o repositório.
		/// </summary>
		void Start();

		/// <summary>
		/// Adiciona um novo pacote para o repositório.
		/// </summary>
		/// <param name="uid">Identificador do pacote.</param>
		/// <param name="inputStream">Stream com os dados do pacote.</param>
		void Add(Guid uid, System.IO.Stream inputStream);

		/// <summary>
		/// Recupera a stream do pacote de assembly associado com as parts de assembly informadas.
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <returns></returns>
		AssemblyPackageContainer GetAssemblyPackages(IEnumerable<AssemblyPart> assemblyParts);

		/// <summary>
		/// Inicializa o processo para recuperar a stream do pacote de 
		/// assembly associado com as parts de assembly informadas.
		/// </summary>
		/// <param name="assemblyParts"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult BeginGetAssemblyPackages(IEnumerable<AssemblyPart> assemblyParts, AsyncCallback callback, object state);

		/// <summary>
		/// Finaliza o processa para recuperar a stream do pacote de 
		/// assembly associado com as parts de assembly informadas.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns></returns>
		AssemblyPackageContainer EndGetAssemblyPackages(IAsyncResult ar);

		/// <summary>
		/// Recupera o assembly do pacote informado.
		/// </summary>
		/// <param name="package"></param>
		/// <returns></returns>
		System.IO.Stream GetAssemblyPackageStream(IAssemblyPackage package);

		/// <summary>
		/// Recupera o pacote pelo identificador informado.
		/// </summary>
		/// <param name="assemblyPackageUid">Identificador do pacote que será recuperado.</param>
		/// <returns></returns>
		IAssemblyPackage GetAssemblyPackage(Guid assemblyPackageUid);

		/// <summary>
		/// Valida o repositório.
		/// </summary>
		AssemblyRepositoryValidateResult Validate();
	}
}
