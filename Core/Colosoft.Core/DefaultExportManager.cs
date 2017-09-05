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
	/// Implementação padrão do gerenciador das exportações.
	/// </summary>
	public class DefaultExportManager : IExportManager
	{
		private bool _isStarted;

		private string[] _uiContexts;

		private List<IExport> _exports;

		/// <summary>
		/// Evento acionado quando o gerenciador for iniciado.
		/// </summary>
		public event EventHandler Started;

		/// <summary>
		/// Evento acionado qaudno ocorre um erro no inicialização do gerenciador.
		/// </summary>
		public event EventHandler<ExportManagerStartErrorArgs> StartError;

		/// <summary>
		/// Identifica se o gerenciador foi iniciado.
		/// </summary>
		public bool IsStarted
		{
			get
			{
				return _isStarted;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DefaultExportManager()
		{
			_exports = new List<IExport>();
		}

		/// <summary>
		/// Cria o gerenciador e inicializa já com a lista de exports.
		/// </summary>
		/// <param name="exports"></param>
		public DefaultExportManager(IEnumerable<IExport> exports)
		{
			exports.Require("exports").NotNull();
			_exports = new List<IExport>(exports);
		}

		/// <summary>
		/// Método acionado quando a instancia for iniciada.
		/// </summary>
		protected virtual void OnStarted()
		{
			if(Started != null)
				Started(this, EventArgs.Empty);
		}

		/// <summary>
		/// Método acionado quando ocorre um erro na iniciação.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="uiContexts"></param>
		protected virtual void OnStartError(Exception exception, string[] uiContexts)
		{
			if(StartError != null)
				StartError(this, new ExportManagerStartErrorArgs(exception, uiContexts));
		}

		/// <summary>
		/// Recupera a exportação pelo nome do contrato.
		/// </summary>
		/// <param name="contractTypeName">Nome do tipo do contrato.</param>
		/// <param name="contractName">Nome do contrato.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		public IExport GetExport(TypeName contractTypeName, string contractName, string uiContext)
		{
			return GetExports(contractTypeName, contractName, uiContext).FirstOrDefault();
		}

		/// <summary>
		/// Recupera a relação das exportações do tipo de contrato informado no contexto visual.
		/// </summary>
		/// <param name="contractTypeName">Nome do tipo do contrato.</param>
		/// <param name="contractName">Nome do contrato.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		public IEnumerable<IExport> GetExports(TypeName contractTypeName, string contractName, string uiContext)
		{
			foreach (var export in GetExports(uiContext))
				if(export.ContractName == contractName && TypeName.TypeNameEqualityComparer.Instance.Equals(export.ContractType, contractTypeName))
					yield return export;
		}

		/// <summary>
		/// Recupera todos os exports associados com o contexto intercafe com o usuário.
		/// </summary>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <returns></returns>
		public IEnumerable<IExport> GetExports(string uiContext)
		{
			if(string.IsNullOrEmpty(uiContext))
				foreach (var i in _exports)
					yield return i;
			foreach (var i in _exports)
			{
				var export2 = i as IExport2;
				if(export2.UIContext == uiContext && (_uiContexts == null || (!string.IsNullOrEmpty(uiContext) && _uiContexts.Contains(uiContext))))
					yield return i;
			}
		}

		/// <summary>
		/// Inicializa o gerenciador.
		/// </summary>
		/// <param name="uiContexts">Contextos de interface com o usuário que serão carregados.</param>
		/// <param name="throwError">Identifica se é para dispara o erro caso ocorra.</param>
		public void Start(string[] uiContexts, bool throwError)
		{
			_isStarted = true;
			_uiContexts = uiContexts;
			OnStarted();
		}
	}
}
