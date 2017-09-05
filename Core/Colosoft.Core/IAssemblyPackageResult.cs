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
	/// Representa o resultado de um pacote de assemblies.
	/// </summary>
	public interface IAssemblyPackageResult : IDisposable
	{
		/// <summary>
		/// Extraí os arquivos do pacote.
		/// </summary>
		/// <param name="outputDirectory">Diretório de saída.</param>
		/// <param name="canOverride">True para sobreescrever os arquivos que existirem.</param>
		/// <returns></returns>
		bool ExtractPackageFiles(string outputDirectory, bool canOverride);

		/// <summary>
		/// Recupera o stream do assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		System.IO.Stream GetAssemblyStream(AssemblyPart name);

		/// <summary>
		/// Recupera o assembly.
		/// </summary>
		/// <param name="name">Nome do assembly que será recuperado.</param>
		/// <returns></returns>
		System.Reflection.Assembly GetAssembly(AssemblyPart name);

		/// <summary>
		/// Carrega o assembly guardado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="exception">Exception caso ocorra.</param>
		/// <returns></returns>
		System.Reflection.Assembly LoadAssemblyGuarded(AssemblyPart name, out Exception exception);
	}
}
