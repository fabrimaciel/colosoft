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
using System.Collections;
using Colosoft.Caching.Queries.Filters;
using Colosoft.Logging;
using Colosoft.Text.Parser;
using Colosoft.Caching.Util;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Representa o predicado titular.
	/// </summary>
	internal class PredicateHolder
	{
		private IDictionary _attributeValues;

		[NonSerialized]
		private Predicate _predicate;

		/// <summary>
		/// Valores dos atributos associados.
		/// </summary>
		internal IDictionary AttributeValues
		{
			get
			{
				return MiscUtil.DeepClone(_attributeValues);
			}
			set
			{
				_attributeValues = value;
			}
		}

		/// <summary>
		/// Texto do comando.
		/// </summary>
		internal string CommandText
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo do objeto de retorno.
		/// </summary>
		internal string ObjectType
		{
			get;
			set;
		}

		/// <summary>
		/// Predicado associado.
		/// </summary>
		internal Predicate Predicate
		{
			get
			{
				return _predicate;
			}
			set
			{
				_predicate = value;
			}
		}

		/// <summary>
		/// Identificador da consulta.
		/// </summary>
		internal string QueryId
		{
			get;
			set;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="logger"></param>
		internal void Initialize(ILogger logger)
		{
			var helper = new ParserHelper(logger);
			if(helper.Parse(CommandText) == ParseMessage.Accept)
			{
				Reduction currentReduction = helper.CurrentReduction;
				_predicate = currentReduction.Tag as Predicate;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			PredicateHolder holder = obj as PredicateHolder;
			return ((holder != null) && this.QueryId.Equals(holder.QueryId));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
