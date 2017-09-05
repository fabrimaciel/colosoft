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
using Colosoft.Text.Parser;

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do predicado "Is in list"
	/// </summary>
	public class IsInListPredicate : Predicate, IComparable
	{
		private IFunctor _functor;

		/// <summary>
		/// Lista do itens que serão comparados.
		/// </summary>
		private ArrayList _members = new ArrayList();

		/// <summary>
		/// <see cref="IFunctor"/> associado.
		/// </summary>
		public IFunctor Functor
		{
			set
			{
				_functor = value;
			}
		}

		/// <summary>
		/// Lista dos valores que serão comparados.
		/// </summary>
		public ArrayList Values
		{
			get
			{
				return _members;
			}
		}

		/// <summary>
		/// Anexa um novo item para a comparação.
		/// </summary>
		/// <param name="item"></param>
		public void Append(object item)
		{
			object obj2 = ((IGenerator)item).Evaluate();
			if(!_members.Contains(obj2))
			{
				_members.Add(obj2);
				_members.Sort();
			}
		}

		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			object item = _functor.Evaluate(o);
			if(base.Inverse)
				return !_members.Contains(item);
			return _members.Contains(item);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is IsInListPredicate)
			{
				IsInListPredicate predicate = (IsInListPredicate)obj;
				if((base.Inverse == predicate.Inverse) && (_members.Count == predicate._members.Count))
				{
					for(int i = 0; i < _members.Count; i++)
						if(_members[i] != predicate._members[i])
							return -1;
					return 0;
				}
			}
			return -1;
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="nextPredicate"></param>
		internal override void Execute(QueryContext queryContext, Predicate nextPredicate)
		{
			AttributeIndex index = queryContext.Index;
			IIndexStore store = ((MemberFunction)_functor).GetStore(index);
			if(store == null)
			{
				throw new ParserException("Index is not defined for attribute '" + ((MemberFunction)_functor).MemberName + "'");
			}
			_members = queryContext.AttributeValues[((MemberFunction)_functor).MemberName] as ArrayList;
			if(_members == null)
			{
				if(queryContext.AttributeValues.Count <= 0)
					throw new Exception("Value(s) not specified for indexed attribute " + ((MemberFunction)_functor).MemberName + ".");
				_members = new ArrayList();
				_members.Add(queryContext.AttributeValues[((MemberFunction)_functor).MemberName]);
			}
			ArrayList list = new ArrayList();
			if(!base.Inverse)
			{
				for(int i = 0; i < _members.Count; i++)
				{
					ArrayList data = store.GetData(_members[i], ComparisonType.EQUALS);
					if((data != null) && (data.Count > 0))
						list.AddRange(data);
				}
			}
			else
			{
				ArrayList c = store.GetData(_members[0], ComparisonType.NOT_EQUALS);
				if((c != null) && (c.Count > 0))
				{
					for(int j = 1; j < _members.Count; j++)
					{
						ArrayList list4 = store.GetData(_members[j], ComparisonType.EQUALS);
						if(list4 != null)
						{
							IEnumerator enumerator = list4.GetEnumerator();
							if(enumerator != null)
								while (enumerator.MoveNext())
									if(c.Contains(enumerator.Current))
										c.Remove(enumerator.Current);
						}
					}
					list.AddRange(c);
				}
			}
			if((list != null) && (list.Count > 0))
			{
				IEnumerator enumerator2 = list.GetEnumerator();
				if(!queryContext.PopulateTree)
				{
					while (enumerator2.MoveNext())
					{
						if(queryContext.Tree.LeftList.Contains(enumerator2.Current))
						{
							queryContext.Tree.Shift(enumerator2.Current);
						}
					}
				}
				else
				{
					queryContext.Tree.RightList = list;
					queryContext.PopulateTree = false;
				}
			}
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="list"></param>
		internal override void ExecuteInternal(QueryContext queryContext, ref SortedList list)
		{
			AttributeIndex index = queryContext.Index;
			IIndexStore store = ((MemberFunction)_functor).GetStore(index);
			ArrayList list2 = new ArrayList();
			if(store == null)
				throw new ParserException("Index is not defined for attribute '" + ((MemberFunction)_functor).MemberName + "'");
			_members = queryContext.AttributeValues[((MemberFunction)_functor).MemberName] as ArrayList;
			if(_members == null)
			{
				if(queryContext.AttributeValues.Count <= 0)
					throw new Exception("Value(s) not specified for indexed attribute " + ((MemberFunction)_functor).MemberName + ".");
				_members = new ArrayList();
				_members.Add(queryContext.AttributeValues[((MemberFunction)_functor).MemberName]);
			}
			if(!base.Inverse)
			{
				for(int i = 0; i < _members.Count; i++)
				{
					ArrayList data = store.GetData(_members[i], ComparisonType.EQUALS);
					if((data != null) && (data.Count > 0))
						list2.AddRange(data);
				}
			}
			else
			{
				ArrayList c = store.GetData(_members[0], ComparisonType.NOT_EQUALS);
				if((c != null) && (c.Count > 0))
				{
					for(int j = 1; j < _members.Count; j++)
					{
						ArrayList list5 = store.GetData(_members[j], ComparisonType.EQUALS);
						if(list5 != null)
						{
							IEnumerator enumerator = list5.GetEnumerator();
							if(enumerator != null)
								while (enumerator.MoveNext())
									if(c.Contains(enumerator.Current))
										c.Remove(enumerator.Current);
						}
					}
					list2.AddRange(c);
				}
			}
			if(list2 != null)
				list.Add(list2.Count, list2);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = base.Inverse ? "is not in (" : "is in (";
			for(int i = 0; i < _members.Count; i++)
			{
				if(i > 0)
				{
					str = str + ", ";
				}
				str = str + _members[i].ToString();
			}
			return (str + ")");
		}
	}
}
