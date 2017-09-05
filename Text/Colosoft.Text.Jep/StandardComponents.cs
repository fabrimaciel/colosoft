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
	using Colosoft.Text.Jep.ConfigurableParser;
	using System;

	public class StandardComponents : ComponentSet
	{
		public StandardComponents()
		{
			base.numFac = new DoubleNumberFactory();
			base.varFac = new VariableFactory();
			base.nodeFac = new NodeFactory();
			base.funTab = new StandardFunctionTable();
			base.varTab = new StandardVariableTable(base.varFac);
			base.opTab = new StandardOperatorTable();
			base.parser = new StandardConfigurableParser();
			base.evaluator = new StandardEvaluator();
			base.pv = new PrintVisitor();
		}
	}
}
