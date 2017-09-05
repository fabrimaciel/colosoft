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
	/// Argumentos do callback da subquery.
	/// </summary>
	public class SubQueryCallBackArgs : EventArgs
	{
		private QueryInfo _info;

		private ReferenceParameterValueCollection _referenceValues;

		private IQueryResult _result;

		/// <summary>
		/// Informações da consulta.
		/// </summary>
		public QueryInfo Info
		{
			get
			{
				return _info;
			}
		}

		/// <summary>
		/// Valores de referencia.
		/// </summary>
		public ReferenceParameterValueCollection ReferenceValues
		{
			get
			{
				return _referenceValues;
			}
		}

		/// <summary>
		/// Resultado do consulta.
		/// </summary>
		public IQueryResult Result
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="referenceValues"></param>
		/// <param name="result"></param>
		public SubQueryCallBackArgs(QueryInfo info, ReferenceParameterValueCollection referenceValues, IQueryResult result)
		{
			_info = info;
			_referenceValues = referenceValues;
			_result = result;
		}
	}
	/// <summary>
	/// Armazena os argumentos do callback de falha da execução da subquery.
	/// </summary>
	public class SubQueryCallBackFailedArgs : EventArgs
	{
		private QueryInfo _info;

		private QueryFailedInfo _result;

		/// <summary>
		/// Informações da consulta.
		/// </summary>
		public QueryInfo Info
		{
			get
			{
				return _info;
			}
		}

		/// <summary>
		/// Resultado da falha.
		/// </summary>
		public QueryFailedInfo Result
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="result"></param>
		public SubQueryCallBackFailedArgs(QueryInfo info, QueryFailedInfo result)
		{
			_info = info;
			_result = result;
		}
	}
	/// <summary>
	/// Delegate do callback da execução das subqueries.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void SubQueryCallBack (object sender, SubQueryCallBackArgs e);
	/// <summary>
	/// Callback acionado quando ocorre um falha a execução de uma consulta.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void SubQueryFailedCallBack (object sender, SubQueryCallBackFailedArgs e);
}
