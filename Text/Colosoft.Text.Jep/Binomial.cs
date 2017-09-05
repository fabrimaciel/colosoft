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

namespace Colosoft.Text.Jep.Functions
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	public class Binomial : PostfixMathCommand
	{
		private static readonly int initN = 20;

		private static int[][] coeffs = new int[initN + 1][];

		static Binomial()
		{
			coeffs[0] = new int[] {
				1
			};
			coeffs[1] = new int[] {
				1,
				1
			};
			for(int i = 2; i <= initN; i++)
			{
				coeffs[i] = new int[i + 1];
				coeffs[i][0] = 1;
				coeffs[i][i] = 1;
				for(int j = 1; j < i; j++)
				{
					coeffs[i][j] = coeffs[i - 1][j - 1] + coeffs[i - 1][j];
				}
			}
		}

		public Binomial()
		{
			base.numberOfParameters = 2;
		}

		public static int Binom(int n, int i)
		{
			Expand(n);
			return GetCoeff(i, n);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void Expand(int N)
		{
			int num = coeffs.Length - 1;
			if(N > num)
			{
				int[][] numArray = new int[N + 1][];
				for(int i = 0; i <= num; i++)
				{
					numArray[i] = coeffs[i];
				}
				for(int j = num + 1; j <= N; j++)
				{
					numArray[j] = new int[j + 1];
					numArray[j][0] = 1;
					numArray[j][j] = 1;
					for(int k = 1; k < j; k++)
					{
						numArray[j][k] = numArray[j - 1][k - 1] + numArray[j - 1][k];
					}
				}
				coeffs = numArray;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static int GetCoeff(int i, int n)
		{
			return coeffs[n][i];
		}

		public override void Run(Stack<object> s)
		{
			object obj2 = s.Pop();
			object obj3 = s.Pop();
			if(!(obj2 is JepDouble) || !(obj3 is JepDouble))
			{
				throw new EvaluationException(string.Concat(new object[] {
					"Binomial: both arguments must be integers. They are ",
					obj3,
					"(",
					obj3.GetType().Name,
					") and ",
					obj2,
					"(",
					obj2.GetType().Name,
					")"
				}));
			}
			int intValue = ((JepDouble)obj2).IntValue;
			int n = ((JepDouble)obj3).IntValue;
			if(((n < 0) || (intValue < 0)) || (intValue > n))
			{
				throw new EvaluationException(string.Concat(new object[] {
					"Binomial: illegal values for arguments 0<i<n. They are ",
					obj3,
					" and ",
					obj2
				}));
			}
			Expand(n);
			int coeff = GetCoeff(intValue, n);
			s.Push(new JepDouble((double)coeff));
		}
	}
}
