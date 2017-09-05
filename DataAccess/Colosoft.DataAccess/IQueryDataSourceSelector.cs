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

namespace Colosoft.DataAccess
{
	/// <summary>
	/// Assinatura da classe responsável por selecionar
	/// a origem de dados das consultas.
	/// </summary>
	public interface IQueryDataSourceSelector : IDisposable
	{
		/// <summary>
		/// Recupera a origem de dados com base nas informações da consulta.
		/// </summary>
		/// <param name="query">Informações da consulta que será executada.</param>
		/// <returns></returns>
		Colosoft.Query.IQueryDataSource GetDataSource(Colosoft.Query.QueryInfo query);

		/// <summary>
		/// Recupera a origem de dados com base nas informações das consultas.
		/// </summary>
		/// <param name="queries">Informações das consultas que serão executadas.</param>
		/// <returns></returns>
		Colosoft.Query.IQueryDataSource GetDataSource(Colosoft.Query.QueryInfo[] queries);

		/// <summary>
		/// Apaga todos os registros de DataSource em cache.
		/// </summary>
		void Reset();
	}
}
