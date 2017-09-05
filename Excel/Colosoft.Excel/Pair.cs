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
using System.Text;

namespace QiHe.CodeLib
{
	public class Pair<TLeft, TRight> : IEquatable<Pair<TLeft, TRight>>
	{
		public TLeft Left;

		public TRight Right;

		public Pair(TLeft left, TRight right)
		{
			Left = left;
			Right = right;
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", Left, Right);
		}

		public override int GetHashCode()
		{
			return Left.GetHashCode() + Right.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj is Pair<TLeft, TRight>)
			{
				return this.Equals((Pair<TLeft, TRight>)obj);
			}
			else
			{
				return false;
			}
		}

		public bool Equals(Pair<TLeft, TRight> other)
		{
			return this.Left.Equals(other.Left) && this.Right.Equals(other.Right);
		}

		public static bool operator ==(Pair<TLeft, TRight> one, Pair<TLeft, TRight> other)
		{
			return one.Equals(other);
		}

		public static bool operator !=(Pair<TLeft, TRight> one, Pair<TLeft, TRight> other)
		{
			return !(one == other);
		}
	}
}
