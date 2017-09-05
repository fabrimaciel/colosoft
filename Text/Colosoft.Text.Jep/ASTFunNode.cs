﻿/* 
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

namespace Colosoft.Text.Jep.Parser
{
	using Colosoft.Text.Jep;
	using System;

	public class ASTFunNode : SimpleNode
	{
		private string name;

		private IPostfixMathCommand pfmc;

		public ASTFunNode(int id) : base(id)
		{
		}

		public ASTFunNode(IParser p, int id) : base(p, id)
		{
		}

		public override string GetName()
		{
			return this.name;
		}

		public override Operator GetOperator()
		{
			return null;
		}

		public override IPostfixMathCommand GetPFMC()
		{
			return this.pfmc;
		}

		public override object JjtAccept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public void SetFunction(string name_in, IPostfixMathCommand pfmc_in)
		{
			this.name = name_in;
			this.pfmc = pfmc_in;
		}

		public override string ToString()
		{
			return ("Function: \"" + this.name + "\"");
		}
	}
}
