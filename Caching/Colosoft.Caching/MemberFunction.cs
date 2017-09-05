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
	/// Implementação de um <see cref="IFunctor"/> para o membro de um tipo.
	/// </summary>
	public class MemberFunction : IFunctor, IComparable, Colosoft.Caching.Queries.Filters.IMemberFunction
	{
		private string _memberName;

		/// <summary>
		/// Nome do membro.
		/// </summary>
		public string MemberName
		{
			get
			{
				return _memberName;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="memberName">Nome do membro</param>
		public MemberFunction(string memberName)
		{
			_memberName = memberName;
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is MemberFunction)
			{
				MemberFunction function = (MemberFunction)obj;
				return _memberName.CompareTo(function._memberName);
			}
			return -1;
		}

		/// <summary>
		/// Calcula o valor com base na instancia informada.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public object Evaluate(object o)
		{
			object obj2 = o;
			var field = obj2.GetType().GetField(_memberName);
			if(field != null)
				return field.GetValue(obj2);
			var property = obj2.GetType().GetProperty(_memberName);
			if(property == null)
				throw new ArgumentException(obj2.GetType() + " contains no field or property named " + _memberName);
			return property.GetValue(obj2, null);
		}

		/// <summary>
		/// Recupera o armazenamento do membro a partir do indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IIndexStore GetStore(AttributeIndex index)
		{
			if(index != null)
				return index.GetStore(_memberName);
			return null;
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
			return store.GetData(key, comparisonType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ("GetMember(" + _memberName + ")");
		}
	}
}
