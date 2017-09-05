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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Implementação do operador lógico AND.
	/// </summary>
	/// <remarks>O inverso é o operador lógico OR.</remarks>
	public class LogicalAndPredicate : Predicate, IComparable
	{
		private ArrayList _members = new ArrayList();

		/// <summary>
		/// Lista dos membros filhos.
		/// </summary>
		public ArrayList Children
		{
			get
			{
				return _members;
			}
		}

		/// <summary>
		/// Recupera a interseção com a lista informada.
		/// </summary>
		/// <param name="list">Lista para onde será feita a interseção.</param>
		/// <returns></returns>
		private ArrayList GetIntersection(SortedList list)
		{
			Hashtable hashtable = new Hashtable();
			if(list.Count > 0)
			{
				ArrayList byIndex = list.GetByIndex(0) as ArrayList;
				for(int i = 0; i < byIndex.Count; i++)
				{
					hashtable[byIndex[i]] = null;
				}
				for(int j = 1; j < list.Count; j++)
				{
					Hashtable hashtable2 = new Hashtable();
					byIndex = list.GetByIndex(j) as ArrayList;
					if(byIndex != null)
						for(int k = 0; k < byIndex.Count; k++)
						{
							object key = byIndex[k];
							if(hashtable.ContainsKey(key))
								hashtable2[key] = null;
						}
					hashtable = hashtable2;
				}
			}
			return new ArrayList(hashtable.Keys);
		}

		/// <summary>
		/// Recupera a união com a lista informada.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private ArrayList GetUnion(SortedList list)
		{
			Hashtable hashtable = new Hashtable();
			if(list.Count > 0)
			{
				ArrayList byIndex = list.GetByIndex(0) as ArrayList;
				for(int i = 0; i < byIndex.Count; i++)
					hashtable[byIndex[i]] = null;
				for(int j = 1; j < list.Count; j++)
				{
					ArrayList list3 = list.GetByIndex(j) as ArrayList;
					if((list3 != null) && (list3.Count > 0))
						for(int k = 0; k < list3.Count; k++)
							hashtable[list3[k]] = null;
				}
			}
			return new ArrayList(hashtable.Keys);
		}

		/// <summary>
		/// Aplica o predicado.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool ApplyPredicate(object o)
		{
			for(int i = 0; i < _members.Count; i++)
				if(((Predicate)_members[i]).Evaluate(o) == base.Inverse)
					return base.Inverse;
			return !base.Inverse;
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if(obj is LogicalAndPredicate)
			{
				LogicalAndPredicate predicate = (LogicalAndPredicate)obj;
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
			bool ascending = true;
			bool flag2 = true;
			if(base.Inverse)
				ascending = false;
			SortedList list = new SortedList(new QueryResultComparer(ascending));
			for(int i = 0; i < _members.Count; i++)
			{
				Predicate predicate = (Predicate)_members[i];
				if(predicate is IsOfTypePredicate)
				{
					predicate.Execute(queryContext, (Predicate)_members[++i]);
					flag2 = false;
				}
				else
					predicate.ExecuteInternal(queryContext, ref list);
			}
			if(flag2)
			{
				if(base.Inverse)
					queryContext.Tree.RightList = this.GetUnion(list);
				else
					queryContext.Tree.RightList = this.GetIntersection(list);
			}
		}

		/// <summary>
		/// Executa o predicado.
		/// </summary>
		/// <param name="queryContext"></param>
		/// <param name="list"></param>
		internal override void ExecuteInternal(QueryContext queryContext, ref SortedList list)
		{
			bool ascending = true;
			ArrayList union = new ArrayList();
			if(base.Inverse)
				ascending = false;
			SortedList list3 = new SortedList(new QueryResultComparer(ascending));
			for(int i = 0; i < _members.Count; i++)
				((Predicate)_members[i]).ExecuteInternal(queryContext, ref list3);
			if(base.Inverse)
				union = this.GetUnion(list3);
			else
				union = this.GetIntersection(list3);
			if(union != null)
				list.Add(union.Count, union);
		}

		/// <summary>
		/// Inverte o predicado.
		/// </summary>
		public override void Invert()
		{
			base.Invert();
			for(int i = 0; i < _members.Count; i++)
				((Predicate)_members[i]).Invert();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = base.Inverse ? "(" : "(";
			for(int i = 0; i < _members.Count; i++)
			{
				if(i > 0)
					str = str + (base.Inverse ? " or " : " and ");
				str = str + _members[i].ToString();
			}
			return (str + ")");
		}
	}
}
