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

namespace Colosoft.Reflection.Composition
{
	/// <summary>
	/// Armazena os argumentos de quando ocorre um erro ao iniciar o gerenciador.
	/// </summary>
	public class ExportManagerStartErrorArgs : EventArgs
	{
		/// <summary>
		/// Instancia do erro ocorrido.
		/// </summary>
		public Exception Error
		{
			get;
			set;
		}

		/// <summary>
		/// Contextos de interface com o usuário.
		/// </summary>
		public string[] UIContexts
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="uiContexts"></param>
		public ExportManagerStartErrorArgs(Exception error, string[] uiContexts)
		{
			this.Error = error;
			this.UIContexts = uiContexts;
		}
	}
	/// <summary>
	/// Assinatura do gerenciador das exportações.
	/// </summary>
	public interface IExportManager
	{
		/// <summary>
		/// Evento acionado quando o gerenciador for iniciado.
		/// </summary>
		event EventHandler Started;

		/// <summary>
		/// Evento acionado qaudno ocorre um erro no inicialização do gerenciador.
		/// </summary>
		event EventHandler<ExportManagerStartErrorArgs> StartError;

		/// <summary>
		/// Identifica se o gerenciador foi iniciado.
		/// </summary>
		bool IsStarted
		{
			get;
		}

		/// <summary>
		/// Recupera a exportação pelo nome do contrato.
		/// </summary>
		/// <param name="contractTypeName">Nome do tipo do contrato.</param>
		/// <param name="contractName">Nome do contrato.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		IExport GetExport(TypeName contractTypeName, string contractName, string uiContext);

		/// <summary>
		/// Recupera a relação das exportações do tipo de contrato informado no contexto visual.
		/// </summary>
		/// <param name="contractTypeName">Nome do tipo do contrato.</param>
		/// <param name="contractName">Nome do contrato.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		IEnumerable<IExport> GetExports(TypeName contractTypeName, string contractName, string uiContext);

		/// <summary>
		/// Recupera todos os exports associados com o contexto intercafe com o usuário.
		/// </summary>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		IEnumerable<IExport> GetExports(string uiContext);

		/// <summary>
		/// Inicializa o gerenciador.
		/// </summary>
		/// <param name="uiContexts">Contextos de interface com o usuário que serão carregados.</param>
		/// <param name="throwError">Identifica se é para dispara o erro caso ocorra.</param>
		void Start(string[] uiContexts, bool throwError);
	}
}
