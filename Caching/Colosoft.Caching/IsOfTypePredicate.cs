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
using Colosoft.Text.Parser;

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do predicado de comparação de tipo.
	/// </summary>
	public class IsOfTypePredicate : Predicate, IComparable
	{
		private string _typeName;

		/// <summary>
		/// Nome do tipo associado com o predicado.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				_typeName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será comparado no predicado.</param>
		public IsOfTypePredicate(string typeName)
		{
			_typeName = typeName;
		}

		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			if(_typeName == "*")
				throw new ParserException(ResourceMessageFormatter.Create(() => Properties.Resources.ParserException_StartIsNotSupported).Format());
			return (o.GetType().Name == _typeName);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is IsOfTypePredicate)
			{
				IsOfTypePredicate predicate = (IsOfTypePredicate)obj;
				if(base.Inverse == predicate.Inverse)
					return _typeName.CompareTo(predicate._typeName);
			}
			return -1;
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext">Contexto da consulta.</param>
		/// <param name="nextPredicate"></param>
		internal override void Execute(QueryContext queryContext, Predicate nextPredicate)
		{
			if(_typeName == "*")
				throw new ParserException(ResourceMessageFormatter.Create(() => Properties.Resources.ParserException_StartIsNotSupported).Format());
			if(queryContext.IndexManager == null)
				throw new ParserException(ResourceMessageFormatter.Create(() => Properties.Resources.ParserException_IndexIsNotDefined, _typeName).Format());
			queryContext.TypeName = _typeName;
			if(queryContext.Index == null)
			{
				if((queryContext.AttributeValues == null || queryContext.AttributeValues.Count != 1))
					throw new ParserException(ResourceMessageFormatter.Create(() => Properties.Resources.ParserException_IndexIsNotDefined, _typeName).Format());
				queryContext.Index = new AttributeIndex(null, queryContext.Cache.Context.CacheRoot.Name);
			}
			else if(nextPredicate == null && queryContext.PopulateTree)
			{
				queryContext.Tree.Populate(queryContext.Index.GetEnumerator(_typeName, false));
				queryContext.Tree.Populate(queryContext.Index.GetEnumerator(_typeName, true));
			}
			else
				nextPredicate.Execute(queryContext, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ("typeof(Value)" + (base.Inverse ? " != " : " == ") + _typeName);
		}
	}
}
