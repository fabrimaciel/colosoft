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
	/// Assinatura de um pacote de assemblies.
	/// </summary>
	public interface IAssemblyPackage : IEnumerable<AssemblyPart>
	{
		/// <summary>
		/// Identificador unico do pacote.
		/// </summary>
		Guid Uid
		{
			get;
		}

		/// <summary>
		/// Quantidade de partes do pacote.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Data de criação do pacote.
		/// </summary>
		DateTime CreateTime
		{
			get;
		}

		/// <summary>
		/// Recupera a parte do assembly pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		AssemblyPart this[int index]
		{
			get;
		}

		/// <summary>
		/// Recupera a instancia do assembly carregado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		System.Reflection.Assembly GetAssembly(AssemblyPart name);

		/// <summary>
		/// Carrega o assembly guardado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="exception">Exception caso ocorra.</param>
		/// <returns></returns>
		System.Reflection.Assembly LoadAssemblyGuarded(AssemblyPart name, out Exception exception);

		/// <summary>
		/// Recupera o stream do assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		System.IO.Stream GetAssemblyStream(AssemblyPart name);

		/// <summary>
		/// Extraí os arquivos do pacote.
		/// </summary>
		/// <param name="outputDirectory">Diretório de saída.</param>
		/// <param name="canOverride">True para sobreescrever os arquivos que existirem.</param>
		/// <rereturns>True caso a operação tenha sido realizada com sucesso.</rereturns>
		bool ExtractPackageFiles(string outputDirectory, bool canOverride);

		/// <summary>
		/// Verifica se existe no pacote uma parte compatível com a informada.
		/// </summary>
		/// <param name="assemblyPart">Parte que será comparada.</param>
		/// <returns></returns>
		bool Contains(AssemblyPart assemblyPart);
	}
}
