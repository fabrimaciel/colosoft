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

namespace Colosoft.Text.Jep
{
	using System;
	using System.Collections;
	using System.Text;

	public class FunctionTable : IJepComponent
	{
		protected Hashtable table = new Hashtable();

		public IPostfixMathCommand AddFunction(string name, IPostfixMathCommand pfmc)
		{
			this.table.Add(name, pfmc);
			return pfmc;
		}

		public void Clear()
		{
			this.table.Clear();
		}

		public bool ContainsKey(string key)
		{
			return this.table.ContainsKey(key);
		}

		public bool ContainsValue(IPostfixMathCommand value)
		{
			return this.table.ContainsValue(value);
		}

		public IPostfixMathCommand GetFunction(string name)
		{
			return (IPostfixMathCommand)this.table[name];
		}

		public IJepComponent GetLightWeightInstance()
		{
			return this;
		}

		public void Init(JepInstance jep)
		{
		}

		public bool IsEmpty()
		{
			return (this.table.Count == 0);
		}

		public ICollection KeySet()
		{
			return this.table.Keys;
		}

		public IPostfixMathCommand Remove(string key)
		{
			IPostfixMathCommand command = (IPostfixMathCommand)this.table[key];
			this.table.Remove(key);
			return command;
		}

		public virtual FunctionTable ShallowCopy()
		{
			FunctionTable table = new FunctionTable();
			foreach (DictionaryEntry entry in table.table)
			{
				table.AddFunction((string)entry.Key, (IPostfixMathCommand)entry.Value);
			}
			return table;
		}

		public int Size()
		{
			return this.table.Count;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("Functions: ");
			int num = 0;
			foreach (string str in this.table.Keys)
			{
				if(num++ != 0)
				{
					builder.Append(',');
				}
				builder.Append(str);
			}
			builder.Append('\n');
			return builder.ToString();
		}

		public ICollection Values()
		{
			return this.table.Values;
		}
	}
}
