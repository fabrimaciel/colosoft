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
	/// Wrapper para callback
	/// </summary>
	class QueryCallBackWrapper
	{
		private QueryCallBack _queryCallBack;

		private QueryFailedCallBack _queryFailedCallBack;

		/// <summary>
		/// Objeto do callback
		/// </summary>
		public virtual object QueryCallBack
		{
			get
			{
				return _queryCallBack;
			}
			set
			{
				_queryCallBack = (QueryCallBack)value;
			}
		}

		/// <summary>
		/// CallBack acionado quando ocorreu uma falha a execução da consulta.
		/// </summary>
		public QueryFailedCallBack QueryFailedCallBack
		{
			get
			{
				return _queryFailedCallBack;
			}
			set
			{
				_queryFailedCallBack = value;
			}
		}

		/// <summary>
		/// Executa callback
		/// </summary>
		/// <param name="sender">Objeto que disparou a execução</param>
		/// <param name="info">Informações da consulta</param>
		/// <param name="result">Resultado da consulta</param>
		public virtual void ExecuteCallBack(MultiQueryable sender, QueryInfo info, IQueryResult result)
		{
			if(_queryCallBack != null)
				_queryCallBack(sender, info, result);
		}

		/// <summary>
		/// Executa o callback.
		/// </summary>
		/// <param name="sender">Objeto que disparou a execução</param>
		/// <param name="info">Informações da consulta</param>
		/// <param name="result"></param>
		public virtual void ExecuteFailedCallBack(MultiQueryable sender, QueryInfo info, QueryFailedInfo result)
		{
			if(_queryFailedCallBack != null)
				_queryFailedCallBack(sender, info, result);
		}

		/// <summary>
		/// Remove callback do wrapper
		/// </summary>
		public virtual void RemoveCallBack()
		{
			_queryCallBack = null;
			_queryFailedCallBack = null;
		}
	}
}
