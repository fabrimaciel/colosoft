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

namespace Colosoft.Mef
{
	/// <summary>
	/// Assinatura do observer do catalogo do repositório de assemblies do sistema.
	/// </summary>
	public interface IAssemblyRepositoryCatalogObserver
	{
		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar
		/// um tipo para o sistema.
		/// </summary>
		/// <param name="e"></param>
		void FailOnLoadType(FailOnLoadTypeEventArgs e);

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar
		/// um assembly para o sistema.
		/// </summary>
		/// <param name="e"></param>
		void FailOnLoadAssembly(FailOnLoadAssemblyArgs e);

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carrega 
		/// os pacotes para o sistema
		/// </summary>
		/// <param name="e"></param>
		void FailOnLoadPackages(FailOnLoadPackagesArgs e);

		/// <summary>
		/// Método acionado quando não for encontrado o assembly
		/// para o export informado.
		/// </summary>
		/// <param name="e"></param>
		void AssemblyFromExportNotFound(AssemblyFromExportNotFoundEventArgs e);
	}
	/// <summary>
	/// Implementação do agregador do observer <see cref="IAssemblyRepositoryCatalogObserver"/>.
	/// </summary>
	public class AggregateAssemblyRepositoryCatalogObserver : AggregateObserver<IAssemblyRepositoryCatalogObserver>
	{
		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar
		/// um tipo para o sistema.
		/// </summary>
		/// <param name="e"></param>
		public void FailOnLoadType(FailOnLoadTypeEventArgs e)
		{
			foreach (var i in Observers)
				i.FailOnLoadType(e);
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar
		/// um assembly para o sistema.
		/// </summary>
		/// <param name="e"></param>
		public void FailOnLoadAssembly(FailOnLoadAssemblyArgs e)
		{
			foreach (var i in Observers)
				i.FailOnLoadAssembly(e);
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carrega 
		/// os pacotes para o sistema
		/// </summary>
		/// <param name="e"></param>
		public void FailOnLoadPackages(FailOnLoadPackagesArgs e)
		{
			foreach (var i in Observers)
				i.FailOnLoadPackages(e);
		}

		/// <summary>
		/// Método acionado quando não for encontrado o assembly
		/// para o export informado.
		/// </summary>
		/// <param name="e"></param>
		public void AssemblyFromExportNotFound(AssemblyFromExportNotFoundEventArgs e)
		{
			foreach (var i in Observers)
				i.AssemblyFromExportNotFound(e);
		}
	}
}
