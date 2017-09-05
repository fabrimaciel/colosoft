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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Assinatura de um gerenciador de construção de tipos.
	/// </summary>
	interface IBuildManager
	{
		/// <summary>
		/// Verifica se o arquivo existe.
		/// </summary>
		/// <param name="virtualPath">Caminho virtual para o arquivo.</param>
		/// <returns></returns>
		bool FileExists(string virtualPath);

		/// <summary>
		/// Recupera o tipo compilado para o caminho informado.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		Type GetCompiledType(string virtualPath);

		/// <summary>
		/// Recupera a relação de assemblies referenciados.
		/// </summary>
		/// <returns></returns>
		ICollection GetReferencedAssemblies();

		/// <summary>
		/// Lê o arquivo armazenado no cache.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		Stream ReadCachedFile(string fileName);

		/// <summary>
		/// Cria um cache para o arquivo.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		Stream CreateCachedFile(string fileName);
	}
}
