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

namespace Colosoft.Query
{
	/// <summary>
	/// Assinatura das classe de extensão do resultado de uma consulta.
	/// </summary>
	public interface IQueryResultExt : IQueryResult
	{
		/// <summary>
		/// Informações da consulta associada.
		/// </summary>
		QueryInfo QueryInfo
		{
			get;
		}

		/// <summary>
		/// Valores de referencia da consulta.
		/// </summary>
		ReferenceParameterValueCollection ReferenceValues
		{
			get;
		}
	}
	/// <summary>
	/// Assinatura do resultado de uma consulta.
	/// </summary>
	public interface IQueryResult : IEnumerable<Record>, IDisposable
	{
		/// <summary>
		/// Identificador do resultado da consulta.
		/// </summary>
		int Id
		{
			get;
		}

		/// <summary>
		/// Descritpr do resultado da consulta.
		/// </summary>
		Record.RecordDescriptor Descriptor
		{
			get;
		}
	}
	/// <summary>
	/// Assinatura de um container de subconsultas.
	/// </summary>
	public interface INestedQueryContainer
	{
		/// <summary>
		/// Recupera a relação dos parametros de referencia.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ReferenceParameter> GetReferenceParameters();

		/// <summary>
		/// Recupera as subconsultas com base nos valores de referencia.
		/// </summary>
		/// <param name="referenceValues">Valores de referencia do registro.</param>
		/// <returns></returns>
		IEnumerable<Tuple<QueryInfo, IQueryResult>> GetQueryResults(object[] referenceValues);
	}
}
