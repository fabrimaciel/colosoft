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

	public class VariableTable : IJepComponent
	{
		protected Hashtable table = new Hashtable();

		[NonSerialized]
		protected VariableFactory vf;

		public Variable AddConstant(string name, object value)
		{
			Variable variable = this.AddVariable(name, value);
			if(variable == null)
			{
				return null;
			}
			variable.IsConstant = true;
			return variable;
		}

		public Variable AddVariable(string name)
		{
			Variable variable = (Variable)this.table[name];
			if(variable == null)
			{
				variable = this.vf.CreateVariable(name);
				this.table.Add(name, variable);
			}
			return variable;
		}

		public Variable AddVariable(string name, object value)
		{
			Variable variable = (Variable)this.table[name];
			if(variable == null)
			{
				variable = this.vf.CreateVariable(name, value);
				this.table.Add(name, variable);
				return variable;
			}
			if(!variable.SetValue(value))
			{
				throw new JepException("Attempt to set the value of a constant variable");
			}
			return variable;
		}

		public void Clear()
		{
			this.table.Clear();
		}

		public bool ContainsKey(string key)
		{
			return this.table.ContainsKey(key);
		}

		public bool ContainsVariable(Variable value)
		{
			return this.table.ContainsValue(value);
		}

		public void CopyConstantsFrom(VariableTable vt)
		{
			foreach (Variable variable in vt.GetVariables())
			{
				if(variable.IsConstant)
				{
					Variable variable2 = this.vf.CopyVariable(variable);
					this.table.Add(variable.Name, variable2);
				}
			}
		}

		public void CopyVariablesFrom(VariableTable vt)
		{
			foreach (Variable variable in vt.GetVariables())
			{
				Variable variable2 = this.vf.CopyVariable(variable);
				this.table.Add(variable.Name, variable2);
			}
		}

		public IJepComponent GetLightWeightInstance()
		{
			return new VariableTable();
		}

		public Variable GetVariable(string name)
		{
			return (Variable)this.table[name];
		}

		public ICollection GetVariables()
		{
			return this.table.Values;
		}

		public void Init(JepInstance j)
		{
			this.SetVariableFactory(j.VarFac);
		}

		public bool IsEmpty()
		{
			return (this.table.Count == 0);
		}

		public ICollection KeySet()
		{
			return this.table.Keys;
		}

		public Variable Remove(Variable var)
		{
			return this.Remove(var.Name);
		}

		public Variable Remove(string varname)
		{
			Variable variable = (Variable)this.table[varname];
			this.table.Remove(varname);
			return variable;
		}

		public void SetVariableFactory(VariableFactory vf)
		{
			this.vf = vf;
		}

		public int Size()
		{
			return this.table.Count;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append('[');
			int num = 0;
			foreach (Variable variable in this.GetVariables())
			{
				if(num++ > 0)
				{
					builder.Append(',');
				}
				builder.Append(variable);
			}
			builder.Append(']');
			return builder.ToString();
		}

		public ICollection Values()
		{
			return this.table.Values;
		}
	}
}
