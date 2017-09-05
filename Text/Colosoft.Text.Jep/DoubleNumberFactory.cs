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

namespace Colosoft.Text.Jep.Standard
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.Types;
	using System;

	public class DoubleNumberFactory : INumberFactory, IJepComponent
	{
		public static readonly JepDouble MINUSONE = new JepDouble(-1.0);

		public static readonly JepDouble ONE = new JepDouble(1.0);

		public static readonly JepDouble TWO = new JepDouble(2.0);

		public static readonly JepDouble ZERO = new JepDouble(0.0);

		public object CreateNumber(Complex value)
		{
			throw new ParseException("Cannot create a number from a Complex value");
		}

		public object CreateNumber(IJepNumber value)
		{
			return new JepDouble(value.DoubleValue);
		}

		public object CreateNumber(bool value)
		{
			if(!value)
			{
				return ZERO;
			}
			return ONE;
		}

		public object CreateNumber(double value)
		{
			return new JepDouble(value);
		}

		public object CreateNumber(short value)
		{
			return new JepDouble((double)value);
		}

		public object CreateNumber(int value)
		{
			return new JepDouble((double)value);
		}

		public object CreateNumber(long value)
		{
			return new JepDouble((double)value);
		}

		public object CreateNumber(float value)
		{
			return new JepDouble((double)value);
		}

		public object CreateNumber(string value)
		{
			return JepDouble.Parse(value);
		}

		public IJepComponent GetLightWeightInstance()
		{
			return this;
		}

		public object GetMinusOne()
		{
			return MINUSONE;
		}

		public object GetOne()
		{
			return ONE;
		}

		public object GetTwo()
		{
			return TWO;
		}

		public object GetZero()
		{
			return ZERO;
		}

		public void Init(JepInstance jep)
		{
		}
	}
}
