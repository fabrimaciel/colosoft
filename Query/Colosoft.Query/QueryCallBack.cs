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
	/// Armazena a razão por ter havido uma falha na consulta.
	/// </summary>
	public enum QueryFailedReason
	{
		/// <summary>
		/// Ocorreu um erro na consulta no servidor.
		/// </summary>
		Error,
		/// <summary>
		/// Alguma consulta anterior, no caso de mult-consulta, falhou.
		/// </summary>
		PreviousQueryFailed,
		/// <summary>
		/// Consulta abortada.
		/// </summary>
		Aborted,
	}
	/// <summary>
	/// Armazena as informações da falha de uma consulta.
	/// </summary>
	public class QueryFailedInfo
	{
		/// <summary>
		/// Razão pela falha.
		/// </summary>
		public QueryFailedReason Reason
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem associada.
		/// </summary>
		public IMessageFormattable Message
		{
			get;
			set;
		}

		/// <summary>
		/// Erro associado.
		/// </summary>
		public Exception Error
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public QueryFailedInfo()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="message"></param>
		/// <param name="error"></param>
		public QueryFailedInfo(QueryFailedReason reason, IMessageFormattable message, Exception error)
		{
			this.Reason = reason;
			this.Message = message;
			this.Error = error;
		}
	}
	/// <summary>
	/// Callback da execução de uma consulta.
	/// </summary>
	/// <param name="sender">Disparador do callbakc</param>
	/// <param name="info">Informações da consulta</param>
	/// <param name="result">Resultado da consulta</param>
	public delegate void QueryCallBack (MultiQueryable sender, QueryInfo info, IQueryResult result);
	/// <summary>
	/// Callback da execução de uma consulta.
	/// </summary>
	/// <param name="sender">Disparador do callback</param>
	/// <param name="info">Informações da consulta</param>
	/// <param name="result">Resultado da consulta</param>
	public delegate void QueryCallBack<TModel> (MultiQueryable sender, QueryInfo info, QueryResult<TModel> result);
	/// <summary>
	/// CallBack da execução de uma consulta.
	/// </summary>
	/// <param name="sender">Disparador do callback.</param>
	/// <param name="info">Informações da consulta.</param>
	/// <param name="result">Resultado da consulta.</param>
	public delegate void BindableQueryCallBack (MultiQueryable sender, QueryInfo info, BindableQueryResult result);
	/// <summary>
	/// Callback acionado quando ocorre um falha a execução de uma consulta.
	/// </summary>
	/// <param name="sender">Disparador do callback</param>
	/// <param name="info">Informações da consulta</param>
	/// <param name="result">Detalhes da falha ocorrida.</param>
	public delegate void QueryFailedCallBack (MultiQueryable sender, QueryInfo info, QueryFailedInfo result);
}
