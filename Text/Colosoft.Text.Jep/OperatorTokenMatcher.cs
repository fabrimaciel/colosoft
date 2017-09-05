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

namespace Colosoft.Text.Jep.ConfigurableParser.Matchers
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.ConfigurableParser;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;

	public class OperatorTokenMatcher : ITokenMatcher
	{
		[NonSerialized]
		private SortedList<string, List<Operator>> map;

		[NonSerialized]
		private SortedList<string, OperatorToken> tokens;

		private bool CheckCompleteWord(string s, string key)
		{
			Regex regex = new Regex("^[a-zA-Z]+");
			return (!regex.IsMatch(s) || regex.Match(s).Value.Equals(key));
		}

		public void Init(JepInstance j)
		{
			this.Init(j.OpTab);
		}

		public void Init(OperatorTable ot)
		{
			IComparer<string> comparer = new OTMComp();
			this.map = new SortedList<string, List<Operator>>(comparer);
			foreach (Operator @operator in ot.GetOperators())
			{
				string symbol = @operator.GetSymbol();
				Operator item = @operator;
				if(!item.NotInParser())
				{
					List<Operator> list;
					if(this.map.ContainsKey(symbol))
					{
						list = this.map[symbol];
						list.Add(item);
					}
					else
					{
						list = new List<Operator> {
							item
						};
						this.map.Add(symbol, list);
					}
					if(item.IsTernary())
					{
						string rhsSymbol = ((TernaryOperator)@operator).GetRhsSymbol();
						if(!item.NotInParser())
						{
							list = this.map[rhsSymbol];
							if(list == null)
							{
								list = new List<Operator>();
							}
							list.Add(((TernaryOperator)@operator).GetRhsOperator());
							this.map.Add(rhsSymbol, list);
						}
					}
				}
			}
			this.tokens = new SortedList<string, OperatorToken>(comparer);
			foreach (string str3 in this.map.Keys)
			{
				this.tokens.Add(str3, new OperatorToken(this.map[str3]));
			}
		}

		public virtual Token Match(string s)
		{
			foreach (string str in this.tokens.Keys)
			{
				if(s.StartsWith(str) && this.CheckCompleteWord(s, str))
				{
					return this.tokens[str].CloneToken();
				}
			}
			return null;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (string str in this.map.Keys)
			{
				builder.Append(str + ",");
			}
			return builder.ToString();
		}

		[Serializable]
		private class OTMComp : IComparer<string>
		{
			public int Compare(string s1, string s2)
			{
				int num = s2.Length - s1.Length;
				if(num == 0)
				{
					return s1.CompareTo(s2);
				}
				return num;
			}
		}
	}
}
