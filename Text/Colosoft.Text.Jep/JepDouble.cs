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

namespace Colosoft.Text.Jep.Types
{
	using System;

	[Serializable]
	public class JepDouble : IJepNumber
	{
		private double _num;

		public JepDouble(double input)
		{
			this._num = input;
		}

		public static JepDouble Parse(string str)
		{
			return new JepDouble(double.Parse(str));
		}

		public override string ToString()
		{
			return this._num.ToString();
		}

		public virtual double DoubleValue
		{
			get
			{
				return this._num;
			}
		}

		public virtual int IntValue
		{
			get
			{
				return (int)this._num;
			}
		}

		public virtual short ShortValue
		{
			get
			{
				return (short)this._num;
			}
		}

		public virtual double Value
		{
			get
			{
				return this._num;
			}
			set
			{
				this._num = value;
			}
		}
	}
}
