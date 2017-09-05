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
	/// Implementação da adaptação para o callback do resultado da consulta
	/// utilizando o BindStrategy.
	/// </summary>
	class BindableQueryCallBackWrapper : QueryCallBackWrapper
	{
		private IQueryResultBindStrategy _bindStategy;

		private IQueryResultObjectCreator _objectCreator;

		private BindableQueryCallBack _queryCallBack;

		/// <summary>
		/// Estratégia de binding
		/// </summary>
		public IQueryResultBindStrategy BindStrategy
		{
			get
			{
				return _bindStategy;
			}
			set
			{
				_bindStategy = value;
			}
		}

		/// <summary>
		/// Criador de objetos
		/// </summary>
		public IQueryResultObjectCreator ObjectCreator
		{
			get
			{
				return _objectCreator;
			}
			set
			{
				_objectCreator = value;
			}
		}

		/// <summary>
		/// Objeto do Callback
		/// </summary>
		public override object QueryCallBack
		{
			get
			{
				return _queryCallBack;
			}
			set
			{
				_queryCallBack = (BindableQueryCallBack)value;
			}
		}

		/// <summary>
		/// Executa callback
		/// </summary>
		/// <param name="sender">Objeto que disparou a execução</param>
		/// <param name="info">Informações da consulta</param>
		/// <param name="result">Resultado da consulta</param>
		public override void ExecuteCallBack(MultiQueryable sender, QueryInfo info, IQueryResult result)
		{
			if(_queryCallBack != null)
				_queryCallBack(sender, info, new BindableQueryResult(result, BindStrategy, ObjectCreator));
		}

		/// <summary>
		/// Remove callback do wrapper
		/// </summary>
		public override void RemoveCallBack()
		{
			_queryCallBack = null;
		}
	}
}
