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

	[Serializable]
	public class Variable
	{
		private bool _isConstant;

		protected string _name;

		private object _value;

		private bool validValue;

		internal Variable(Variable var) : this(var.Name, var.Value)
		{
			this.IsConstant = var.IsConstant;
		}

		internal Variable(string name)
		{
			this._name = name;
			this._value = null;
			this.validValue = false;
		}

		internal Variable(string name, object value)
		{
			this._name = name;
			this._value = value;
			this.validValue = value != null;
		}

		public bool HasValidValue()
		{
			return this.validValue;
		}

		public void SetValidValue(bool val)
		{
			if(!this.IsConstant)
			{
				this.validValue = val;
			}
		}

		public bool SetValue(object obj)
		{
			if(!this.SetValueRaw(obj))
			{
				return false;
			}
			return true;
		}

		protected bool SetValueRaw(object obj)
		{
			if(this._isConstant)
			{
				return false;
			}
			this.validValue = true;
			this._value = obj;
			return true;
		}

		public override string ToString()
		{
			if(!this.validValue || (this._value == null))
			{
				return (this._name + ": null");
			}
			if(this._isConstant)
			{
				return (this._name + ": " + this._value.ToString() + " (Constant)");
			}
			return (this._name + ": " + this._value.ToString());
		}

		public bool IsConstant
		{
			get
			{
				return this._isConstant;
			}
			set
			{
				this._isConstant = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
		}
	}
}
