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
	/// Assinatura do observer do repositório das informações de assembly.
	/// </summary>
	public interface IAssemblyInfoRepositoryObserver
	{
		/// <summary>
		/// Método acionad quando o progresso de analize dos assemblies é alterado. do repositório é alterado.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="percentage"></param>
		void OnAnalysisAssemblyProgressChanged(IMessageFormattable message, int percentage);

		/// <summary>
		/// Método acionado quando os arquivos de assembly estão sendo carregados.
		/// </summary>
		void OnLoadingAssemblyFiles();

		/// <summary>
		/// Evento acionado quando o repositório for carregado.
		/// </summary>
		void OnLoaded();
	}
}
