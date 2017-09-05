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
	using Colosoft.Text.Jep.Functions;
	using System;

	public class StandardOperatorTable : OperatorTable
	{
		public StandardOperatorTable()
		{
			base.NumOps = 9;
			base.AddOperator(10, new Operator(">", new Comparative(1), 0x106));
			base.AddOperator(11, new Operator("<", new Comparative(0), 0x106));
			base.AddOperator(12, new Operator("==", new Comparative(5), 0x1c6));
			base.AddOperator(13, new Operator("<=", new Comparative(2), 0x146));
			base.AddOperator(14, new Operator(">=", new Comparative(3), 0x146));
			base.AddOperator(15, new Operator("!=", new Comparative(4), 0x86));
			base.AddOperator(0x10, new Operator("&&", new LazyLogical(0), 0x2036));
			base.AddOperator(0x11, new Operator("||", new LazyLogical(1), 0x36));
			base.AddOperator(0x12, new Operator("!", new Not(), 0xa09));
			base.AddOperator(1, new Operator("+", new Add(), 0x36));
			base.AddOperator(2, new Operator("-", new Subtract(), 0x3006));
			base.AddOperator(3, new Operator("UMinus", "-", new UMinus(), 0xa09));
			base.AddOperator(4, new Operator("UPlus", "+", new Add(), 0x209));
			base.AddOperator(5, new Operator("*", new Multiply(), 0x36));
			base.AddOperator(6, new Operator("/", new Divide(), 0x1006));
			base.AddOperator(7, new Operator("%", new Modulus(), 6));
			base.AddOperator(8, new Operator("UDivide", "^-1", null, 0x4a09));
			base.AddOperator(9, new Operator("^", new Power(), 10));
			base.AddOperator(0x13, new Operator("=", new Assign(), 10));
			base.AddOperator(20, new Operator(".", new Dot(), 6));
			base.AddOperator(0x15, new Operator("^^", new Cross(), 6));
			base.AddOperator(0x16, new Operator("LIST", new List(), 0x400b));
			base.AddOperator(0x17, new Operator("[]", new Ele(), 0x400b));
			base.SetPrecedenceTable(new int[][] {
				new int[] {
					9
				},
				new int[] {
					3,
					4,
					0x12
				},
				new int[] {
					5,
					6,
					7,
					20,
					0x15
				},
				new int[] {
					1,
					2
				},
				new int[] {
					11,
					13,
					10,
					14
				},
				new int[] {
					12,
					15
				},
				new int[] {
					0x10
				},
				new int[] {
					0x11
				},
				new int[] {
					0x13
				}
			});
			base.SetStandardOperatorRelations();
		}

		public override void Init(JepInstance jep)
		{
		}
	}
}
