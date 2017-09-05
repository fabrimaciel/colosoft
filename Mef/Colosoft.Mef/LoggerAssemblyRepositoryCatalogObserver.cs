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
	/// Implementação do Observer que registra os dados no log.
	/// </summary>
	public class LoggerAssemblyRepositoryCatalogObserver : IAssemblyRepositoryCatalogObserver
	{
		private Colosoft.Logging.ILogger _logger;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger"></param>
		public LoggerAssemblyRepositoryCatalogObserver(Colosoft.Logging.ILogger logger)
		{
			logger.Require("logger").NotNull();
			_logger = logger;
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar
		/// um tipo para o sistema.
		/// </summary>
		/// <param name="e"></param>
		public void FailOnLoadType(FailOnLoadTypeEventArgs e)
		{
			_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.LoggerAssemblyRepositoryCatalogObserver_FailOnLoadType, e.Type.FullName, e.Error.Message), e.Error);
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carregar
		/// um assembly para o sistema.
		/// </summary>
		/// <param name="e"></param>
		public void FailOnLoadAssembly(FailOnLoadAssemblyArgs e)
		{
			_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.LoggerAssemblyRepositoryCatalogObserver_FailOnAssembly, e.AssemblyName.FullName, e.Error.Message), e.Error);
		}

		/// <summary>
		/// Método acionado quando ocorre uma falha ao carrega 
		/// os pacotes para o sistema
		/// </summary>
		/// <param name="e"></param>
		public void FailOnLoadPackages(FailOnLoadPackagesArgs e)
		{
			_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.LoggerAssemblyRepositoryCatalogObserver_FailOnLoadPackages, string.Join(",", e.AssemblyParts.Select(f => "'" + f.Source + "'").ToArray()), e.Error.Message), e.Error);
		}

		/// <summary>
		/// Método acionado quando não for encontrado o assembly
		/// para o export informado.
		/// </summary>
		/// <param name="e"></param>
		public void AssemblyFromExportNotFound(AssemblyFromExportNotFoundEventArgs e)
		{
			_logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.LoggerAssemblyRepositoryCatalogObserver_AssemblyFromExportNotFound, e.AssemblyName.FullName));
		}
	}
}
