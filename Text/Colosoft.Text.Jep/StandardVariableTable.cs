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

	public class StandardVariableTable : VariableTable
	{
		public StandardVariableTable(VariableFactory varFac)
		{
			base.SetVariableFactory(varFac);
			try
			{
				base.AddConstant("pi", new JepDouble(3.1415926535897931));
				base.AddConstant("e", new JepDouble(2.7182818284590451));
				base.AddConstant("i", new Complex(0.0, 1.0));
				base.AddConstant("true", true);
				base.AddConstant("false", false);
			}
			catch(JepException exception)
			{
				Console.WriteLine("Error creating standard variable table. " + exception);
			}
		}
	}
}
