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

	public class VariableFactory : IJepComponent
	{
		protected object _defaultValue;

		public Variable CopyVariable(Variable var)
		{
			return new Variable(var);
		}

		public Variable CreateVariable(string name)
		{
			if(this._defaultValue != null)
			{
				return new Variable(name, this._defaultValue);
			}
			return new Variable(name);
		}

		public Variable CreateVariable(string name, object value)
		{
			return new Variable(name, value);
		}

		public IJepComponent GetLightWeightInstance()
		{
			return this;
		}

		public void Init(JepInstance j)
		{
		}

		public object DefaultValue
		{
			get
			{
				return this._defaultValue;
			}
			set
			{
				this._defaultValue = value;
			}
		}
	}
}
