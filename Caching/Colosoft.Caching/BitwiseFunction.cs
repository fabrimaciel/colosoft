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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Possíveis operadores bit a bit.
	/// </summary>
	public enum BitwiseOperator
	{
		/// <summary>
		/// Operador And
		/// </summary>
		And,
		/// <summary>
		/// Operador Or
		/// </summary>
		Or
	}
	/// <summary>
	/// Representa a função para uma operação bit a bit.
	/// </summary>
	public class BitwiseFunction : IMemberFunction
	{
		private BitwiseOperator _operator;

		private IFunctor _functor;

		private IGenerator _generator;

		/// <summary>
		/// Nome do membro associado.
		/// </summary>
		public string MemberName
		{
			get
			{
				if(_functor is MemberFunction)
					return ((MemberFunction)_functor).MemberName;
				return null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="functor"></param>
		/// <param name="generator"></param>
		/// <param name="bitwiseOperator"></param>
		public BitwiseFunction(IFunctor functor, IGenerator generator, BitwiseOperator bitwiseOperator)
		{
			_functor = functor;
			_operator = bitwiseOperator;
			_generator = generator;
		}

		/// <summary>
		/// Recupera o armazenamento do membro a partir do indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IIndexStore GetStore(AttributeIndex index)
		{
			if(_functor is MemberFunction)
				return ((MemberFunction)_functor).GetStore(index);
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera o valor
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public object Evaluate(object o)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera os dados filtrando pelo
		/// </summary>
		/// <param name="store">Armazenamento.</param>
		/// <param name="values">Valores dos parametros.</param>
		/// <param name="comparisonType">Tipo de comparação.</param>
		/// <param name="key">Chave que será usada na comparação.</param>
		/// <returns></returns>
		public System.Collections.ArrayList GetData(IIndexStore store, System.Collections.IDictionary values, ComparisonType comparisonType, object key)
		{
			var result = new System.Collections.Hashtable();
			var parameterValue = Convert.ToInt32(_generator.Evaluate(values));
			var keyValue = Convert.ToInt32(key);
			var enumerator = store.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var leftValue = Convert.ToInt32(enumerator.Key);
				if((_operator == BitwiseOperator.And && (leftValue & parameterValue) == keyValue) || (_operator == BitwiseOperator.Or && (leftValue | parameterValue) == keyValue))
				{
					var enumerator4 = (enumerator.Value as System.Collections.Hashtable).GetEnumerator();
					while (enumerator4.MoveNext())
						result[enumerator4.Key] = enumerator4.Value;
				}
			}
			return new System.Collections.ArrayList(result.Keys);
		}
	}
}
