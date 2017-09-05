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
	/// Assinatura da classe usada para observar a execução de uma consulta.
	/// </summary>
	public interface IQueryExecuteObserver
	{
		/// <summary>
		/// Identifica se o tratamento de erro está ativo.
		/// </summary>
		bool ErrorHandlerEnabled
		{
			get;
		}

		/// <summary>
		/// Método acionado quando a consulta for executada.
		/// </summary>
		/// <param name="info">Informações da consulta.</param>
		/// <param name="referenceValues"></param>
		/// <param name="result">Resultado da consulta.</param>
		void Executed(QueryInfo info, ReferenceParameterValueCollection referenceValues, IQueryResult result);

		/// <summary>
		/// Método acionado quando ocorre uma falha na execução da consulta.
		/// </summary>
		/// <param name="info">Informações da consulta.</param>
		/// <param name="fail">Dados da falha.</param>
		void Error(QueryInfo info, QueryFailedInfo fail);
	}
}
