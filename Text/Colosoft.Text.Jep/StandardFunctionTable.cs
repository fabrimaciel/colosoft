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

	public class StandardFunctionTable : FunctionTable
	{
		public StandardFunctionTable()
		{
			base.AddFunction("sin", new Sine());
			base.AddFunction("cos", new Cosine());
			base.AddFunction("tan", new Tangent());
			base.AddFunction("asin", new ArcSine());
			base.AddFunction("acos", new ArcCosine());
			base.AddFunction("atan", new ArcTangent());
			base.AddFunction("atan2", new ArcTangent2());
			base.AddFunction("sinh", new SineH());
			base.AddFunction("cosh", new CosineH());
			base.AddFunction("tanh", new TanH());
			base.AddFunction("asinh", new ArcSineH());
			base.AddFunction("acosh", new ArcCosineH());
			base.AddFunction("atanh", new ArcTanH());
			base.AddFunction("log", new Logarithm());
			base.AddFunction("ln", new NaturalLogarithm());
			base.AddFunction("lg", new LogBase2());
			base.AddFunction("exp", new Exp());
			base.AddFunction("pow", new Power());
			base.AddFunction("round", new Round());
			base.AddFunction("floor", new Floor());
			base.AddFunction("ceil", new Ceil());
			base.AddFunction("re", new Real());
			base.AddFunction("im", new Imaginary());
			base.AddFunction("arg", new Arg());
			base.AddFunction("cmod", new Abs());
			base.AddFunction("complex", new ComplexPFMC());
			base.AddFunction("polar", new Polar());
			base.AddFunction("conj", new Conjugate());
			base.AddFunction("avg", new Average());
			base.AddFunction("min", new MinMax(true));
			base.AddFunction("max", new MinMax(false));
			base.AddFunction("sqrt", new SquareRoot());
			base.AddFunction("abs", new Abs());
			base.AddFunction("mod", new Modulus());
			base.AddFunction("sum", new Sum());
			base.AddFunction("rand", new Rand());
			base.AddFunction("if", new If());
			base.AddFunction("str", new Str());
			base.AddFunction("binom", new Binomial());
		}

		public override FunctionTable ShallowCopy()
		{
			FunctionTable table = base.ShallowCopy();
			table.AddFunction("rand", new Rand());
			return table;
		}
	}
}
