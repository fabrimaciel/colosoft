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

namespace Colosoft.Query
{
	/// <summary>
	/// Assinatura da classe que é reponsável por observar o processamento 
	/// do resultado de uma consulta.
	/// </summary>
	public interface IQueryResultObserver
	{
		/// <summary>
		/// Método acionado quando inicia o processamento do resultado.
		/// </summary>
		/// <param name="result"></param>
		void BeginProcessing(IQueryResult result);

		/// <summary>
		/// Método acionado quando um registro for carregado.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="record"></param>
		void LoadRecord(IQueryResult result, Record record);

		/// <summary>
		/// Método acionado quando o processamento do resultado for finalizado.
		/// </summary>
		/// <param name="result"></param>
		void EndProcessing(IQueryResult result);

		/// <summary>
		/// Método acionado quando ocorrer algum erro no processamento do resultado.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="exception">Instancia do erro ocorrido.</param>
		void Error(IQueryResult result, Exception exception);
	}
}
