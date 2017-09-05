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
	/// Assinatura de um reposiório com as informações dos assemblies.
	/// </summary>
	public interface IAssemblyInfoRepository : IEnumerable<AssemblyInfo>
	{
		/// <summary>
		/// Evento acionado quando o repositório for carregado.
		/// </summary>
		event EventHandler Loaded;

		/// <summary>
		/// Identifica se o repositório foi carregado.
		/// </summary>
		bool IsLoaded
		{
			get;
		}

		/// <summary>
		/// Quantidade de assemblies carregados no repositório.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Identifica se o repositório sofreu alguma alteração.
		/// </summary>
		bool IsChanged
		{
			get;
		}

		/// <summary>
		/// Atualiza o repositório.
		/// </summary>
		/// <param name="executeAnalyzer">Identifica se é para executar o analizador.</param>
		void Refresh(bool executeAnalyzer);

		/// <summary>
		/// Recupera as informações do assembly pelo nome informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly.</param>
		/// <param name="assemblyInfo">Informações do assembly encontrado.</param>
		/// <param name="exception">Error ocorrido na operação.</param>
		/// <returns>True se as informações do assembly forem recuperadas com sucesso.</returns>
		bool TryGet(string assemblyName, out AssemblyInfo assemblyInfo, out Exception exception);

		/// <summary>
		/// Verifica se no repositório existe as informações do assembly informado.
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		bool Contains(string assemblyName);
	}
}
