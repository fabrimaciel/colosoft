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

	public class ComponentSet
	{
		protected IEvaluator evaluator;

		protected Colosoft.Text.Jep.FunctionTable funTab;

		protected Colosoft.Text.Jep.NodeFactory nodeFac;

		protected INumberFactory numFac;

		protected Colosoft.Text.Jep.OperatorTable opTab;

		protected IParser parser;

		protected Colosoft.Text.Jep.PrintVisitor pv;

		protected Colosoft.Text.Jep.VariableFactory varFac;

		protected Colosoft.Text.Jep.VariableTable varTab;

		public ComponentSet()
		{
		}

		public ComponentSet(JepInstance j)
		{
			this.numFac = j.NumFac;
			this.varFac = j.VarFac;
			this.nodeFac = j.NodeFac;
			this.varTab = j.VarTab;
			this.funTab = j.FunTab;
			this.opTab = j.OpTab;
			this.parser = j.Parser;
			this.evaluator = j.Evaluator;
			this.pv = j.PrintVisitor;
		}

		public IJepComponent[] GetComponents()
		{
			return new IJepComponent[] {
				this.numFac,
				this.varFac,
				this.nodeFac,
				this.varTab,
				this.funTab,
				this.opTab,
				this.parser,
				this.evaluator,
				this.pv
			};
		}

		public IEvaluator Evaluator
		{
			get
			{
				return this.evaluator;
			}
			set
			{
				this.evaluator = value;
			}
		}

		public Colosoft.Text.Jep.FunctionTable FunctionTable
		{
			get
			{
				return this.funTab;
			}
			set
			{
				this.funTab = value;
			}
		}

		public Colosoft.Text.Jep.NodeFactory NodeFactory
		{
			get
			{
				return this.nodeFac;
			}
			set
			{
				this.nodeFac = value;
			}
		}

		public INumberFactory NumberFactory
		{
			get
			{
				return this.numFac;
			}
			set
			{
				this.numFac = value;
			}
		}

		public Colosoft.Text.Jep.OperatorTable OperatorTable
		{
			get
			{
				return this.opTab;
			}
			set
			{
				this.opTab = value;
			}
		}

		public IParser Parser
		{
			get
			{
				return this.parser;
			}
			set
			{
				this.parser = value;
			}
		}

		public Colosoft.Text.Jep.PrintVisitor PrintVisitor
		{
			get
			{
				return this.pv;
			}
			set
			{
				this.pv = value;
			}
		}

		public Colosoft.Text.Jep.VariableFactory VariableFactory
		{
			get
			{
				return this.varFac;
			}
			set
			{
				this.varFac = value;
			}
		}

		public Colosoft.Text.Jep.VariableTable VariableTable
		{
			get
			{
				return this.varTab;
			}
			set
			{
				this.varTab = value;
			}
		}
	}
}
