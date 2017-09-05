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
	public class Complex
	{
		private double _im;

		private double _re;

		public Complex()
		{
			this._re = 0.0;
			this._im = 0.0;
		}

		public Complex(Complex z)
		{
			this._re = z._re;
			this._im = z._im;
		}

		public Complex(JepDouble re_in)
		{
			this._re = re_in.DoubleValue;
			this._im = 0.0;
		}

		public Complex(double re_in)
		{
			this._re = re_in;
			this._im = 0.0;
		}

		public Complex(double re_in, double im_in)
		{
			this._re = re_in;
			this._im = im_in;
		}

		public double Abs()
		{
			return Math.Sqrt((this._re * this._re) + (this._im * this._im));
		}

		public double Abs2()
		{
			return ((this._re * this._re) + (this._im * this._im));
		}

		public Complex Acos()
		{
			double num = 1.0 - ((this._re * this._re) - (this._im * this._im));
			double num2 = 0.0 - ((this._re * this._im) + (this._im * this._re));
			Complex complex = new Complex(num, num2);
			complex = complex.Sqrt();
			num = -complex._im;
			num2 = complex._re;
			complex._re = this._re + num;
			complex._im = this._im + num2;
			num = Math.Log(complex.Abs());
			num2 = complex.Arg();
			complex._re = num2;
			complex._im = -num;
			return complex;
		}

		public Complex Acosh()
		{
			Complex complex = new Complex(((this._re * this._re) - (this._im * this._im)) - 1.0, (this._re * this._im) + (this._im * this._re));
			complex = complex.Sqrt();
			complex._re += this._re;
			complex._im += this._im;
			double num = complex.Arg();
			complex._re = Math.Log(complex.Abs());
			complex._im = num;
			return complex;
		}

		public Complex Add(Complex b)
		{
			return new Complex(this._re + b._re, this._im + b._im);
		}

		public double Arg()
		{
			return Math.Atan2(this._im, this._re);
		}

		public Complex Asin()
		{
			double num = 1.0 - ((this._re * this._re) - (this._im * this._im));
			double num2 = 0.0 - ((this._re * this._im) + (this._im * this._re));
			Complex complex = new Complex(num, num2);
			complex = complex.Sqrt();
			complex._re += -this._im;
			complex._im += this._re;
			num = Math.Log(complex.Abs());
			num2 = complex.Arg();
			complex._re = num2;
			complex._im = -num;
			return complex;
		}

		public Complex Asinh()
		{
			Complex complex = new Complex(((this._re * this._re) - (this._im * this._im)) + 1.0, (this._re * this._im) + (this._im * this._re));
			complex = complex.Sqrt();
			complex._re += this._re;
			complex._im += this._im;
			double num = complex.Arg();
			complex._re = Math.Log(complex.Abs());
			complex._im = num;
			return complex;
		}

		public Complex Atan()
		{
			Complex complex = new Complex(-this._re, 1.0 - this._im);
			double num = this._re;
			double num2 = 1.0 + this._im;
			complex = complex.Div(new Complex(num, num2));
			num = Math.Log(complex.Abs());
			num2 = complex.Arg();
			complex._re = 0.5 * num2;
			complex._im = -0.5 * num;
			return complex;
		}

		public Complex Atanh()
		{
			Complex complex = new Complex(1.0 + this._re, this._im);
			double num = 1.0 - this._re;
			double num2 = -this._im;
			complex = complex.Div(new Complex(num, num2));
			num = Math.Log(complex.Abs());
			num2 = complex.Arg();
			complex._re = 0.5 * num;
			complex._im = 0.5 * num2;
			return complex;
		}

		public Complex Conj()
		{
			return new Complex(this._re, -this._im);
		}

		public Complex Cos()
		{
			double d = -this._im;
			double num2 = this._re;
			double num7 = Math.Exp(d);
			double num3 = num7 * Math.Cos(num2);
			double num4 = num7 * Math.Sin(num2);
			num7 = Math.Exp(-d);
			double num5 = num7 * Math.Cos(-num2);
			double num6 = num7 * Math.Sin(-num2);
			num3 += num5;
			num4 += num6;
			return new Complex(0.5 * num3, 0.5 * num4);
		}

		public Complex Cosh()
		{
			double num = Math.Exp(this._re);
			double num2 = num * Math.Cos(this._im);
			double num3 = num * Math.Sin(this._im);
			num = Math.Exp(-this._re);
			double num4 = num * Math.Cos(-this._im);
			double num5 = num * Math.Sin(-this._im);
			num2 += num4;
			num3 += num5;
			return new Complex(0.5 * num2, 0.5 * num3);
		}

		public Complex Div(Complex b)
		{
			double num;
			double num2;
			double num3;
			double num4;
			if(Math.Abs(b._re) >= Math.Abs(b._im))
			{
				num3 = b._im / b._re;
				num4 = b._re + (num3 * b._im);
				num = (this._re + (num3 * this._im)) / num4;
				num2 = (this._im - (num3 * this._re)) / num4;
			}
			else
			{
				num3 = b._re / b._im;
				num4 = b._im + (num3 * b._re);
				num = ((this._re * num3) + this._im) / num4;
				num2 = ((this._im * num3) - this._re) / num4;
			}
			return new Complex(num, num2);
		}

		public double DoubleValue()
		{
			return this._re;
		}

		public float FloatValue()
		{
			return (float)this._re;
		}

		public int HashCode()
		{
			int num = 0x11;
			long num2 = BitConverter.DoubleToInt64Bits(this._re);
			long num3 = BitConverter.DoubleToInt64Bits(this._im);
			int num4 = (int)(num2 ^ (num2 >> 0x20));
			int num5 = (int)(num3 ^ (num3 >> 0x20));
			num = (0x25 * num) + num4;
			return ((0x25 * num) + num5);
		}

		public int IntValue()
		{
			return (int)this._re;
		}

		public bool IsEqual(object o)
		{
			if(!(o is Complex))
			{
				return false;
			}
			Complex complex = (Complex)o;
			return ((BitConverter.DoubleToInt64Bits(this._re) == BitConverter.DoubleToInt64Bits(complex._re)) && (BitConverter.DoubleToInt64Bits(this._im) == BitConverter.DoubleToInt64Bits(complex._im)));
		}

		public bool IsEqual(Complex b, double tolerance)
		{
			double num = this._re - b._re;
			double num2 = this._im - b._im;
			return (((num * num) + (num2 * num2)) <= (tolerance * tolerance));
		}

		public bool IsInfinite()
		{
			if(!double.IsInfinity(this._re))
			{
				return double.IsInfinity(this._im);
			}
			return true;
		}

		public bool IsNaN()
		{
			if(!double.IsNaN(this._re))
			{
				return double.IsNaN(this._im);
			}
			return true;
		}

		public Complex Log()
		{
			return new Complex(Math.Log(this.Abs()), this.Arg());
		}

		public long LongValue()
		{
			return (long)this._re;
		}

		public Complex Mul(Complex b)
		{
			return new Complex((this._re * b._re) - (this._im * b._im), (this._im * b._re) + (this._re * b._im));
		}

		public Complex Mul(double b)
		{
			return new Complex(this._re * b, this._im * b);
		}

		public Complex Neg()
		{
			return new Complex(-this._re, -this._im);
		}

		public static Complex PolarValueOf(JepDouble r, JepDouble theta)
		{
			double num = r.Value;
			double d = theta.Value;
			return new Complex(num * Math.Cos(d), num * Math.Sin(d));
		}

		public Complex Pow(Complex exponent)
		{
			if(exponent._im == 0.0)
			{
				return this.Pow(exponent._re);
			}
			double num = Math.Log(this.Abs());
			double num2 = this.Arg();
			double d = (num * exponent._re) - (num2 * exponent._im);
			double num4 = (num * exponent._im) + (num2 * exponent._re);
			double num5 = Math.Exp(d);
			return new Complex(num5 * Math.Cos(num4), num5 * Math.Sin(num4));
		}

		public Complex Pow(double exponent)
		{
			double num = Math.Pow(this.Abs(), exponent);
			bool flag = false;
			int num2 = 0;
			if((this._im == 0.0) && (this._re < 0.0))
			{
				flag = true;
				num2 = 2;
			}
			if((this._re == 0.0) && (this._im > 0.0))
			{
				flag = true;
				num2 = 1;
			}
			if((this._re == 0.0) && (this._im < 0.0))
			{
				flag = true;
				num2 = -1;
			}
			if(flag && ((num2 * exponent) == ((int)(num2 * exponent))))
			{
				short[] numArray3 = new short[4];
				numArray3[1] = 1;
				numArray3[3] = -1;
				short[] numArray = numArray3;
				short[] numArray4 = new short[4];
				numArray4[0] = 1;
				numArray4[2] = -1;
				short[] numArray2 = numArray4;
				int index = ((int)(num2 * exponent)) % 4;
				if(index < 0)
				{
					index = 4 + index;
				}
				return new Complex(num * numArray2[index], num * numArray[index]);
			}
			double d = exponent * this.Arg();
			return new Complex(num * Math.Cos(d), num * Math.Sin(d));
		}

		public void Set(Complex z)
		{
			this._re = z._re;
			this._im = z._im;
		}

		public void Set(double re_in, double im_in)
		{
			this._re = re_in;
			this._im = im_in;
		}

		public Complex Sin()
		{
			double d = -this._im;
			double num2 = this._re;
			double num7 = Math.Exp(d);
			double num3 = num7 * Math.Cos(num2);
			double num4 = num7 * Math.Sin(num2);
			num7 = Math.Exp(-d);
			double num5 = num7 * Math.Cos(-num2);
			double num6 = num7 * Math.Sin(-num2);
			num3 -= num5;
			num4 -= num6;
			return new Complex(0.5 * num4, -0.5 * num3);
		}

		public Complex Sinh()
		{
			double num = Math.Exp(this._re);
			double num2 = num * Math.Cos(this._im);
			double num3 = num * Math.Sin(this._im);
			num = Math.Exp(-this._re);
			double num4 = num * Math.Cos(-this._im);
			double num5 = num * Math.Sin(-this._im);
			num2 -= num4;
			num3 -= num5;
			return new Complex(0.5 * num2, 0.5 * num3);
		}

		public Complex Sqrt()
		{
			double num3;
			double num4;
			if((this._re == 0.0) && (this._im == 0.0))
			{
				return new Complex(0.0, 0.0);
			}
			double d = Math.Abs(this._re);
			double num2 = Math.Abs(this._im);
			if(d >= num2)
			{
				num4 = num2 / d;
				num3 = Math.Sqrt(d) * Math.Sqrt(0.5 * (1.0 + Math.Sqrt(1.0 + (num4 * num4))));
			}
			else
			{
				num4 = d / num2;
				num3 = Math.Sqrt(num2) * Math.Sqrt(0.5 * (num4 + Math.Sqrt(1.0 + (num4 * num4))));
			}
			if(this._re >= 0.0)
			{
				return new Complex(num3, this._im / (2.0 * num3));
			}
			if(this._im < 0.0)
			{
				num3 = -num3;
			}
			return new Complex(this._im / (2.0 * num3), num3);
		}

		public Complex Sub(Complex b)
		{
			return new Complex(this._re - b._re, this._im - b._im);
		}

		public Complex Tan()
		{
			double d = -this._im;
			double num2 = this._re;
			double num7 = Math.Exp(d);
			double num3 = num7 * Math.Cos(num2);
			double num4 = num7 * Math.Sin(num2);
			num7 = Math.Exp(-d);
			double num5 = num7 * Math.Cos(-num2);
			double num6 = num7 * Math.Sin(-num2);
			num3 -= num5;
			num4 -= num6;
			Complex complex = new Complex(0.5 * num3, 0.5 * num4);
			d = -this._im;
			num2 = this._re;
			num7 = Math.Exp(d);
			num3 = num7 * Math.Cos(num2);
			num4 = num7 * Math.Sin(num2);
			num7 = Math.Exp(-d);
			num5 = num7 * Math.Cos(-num2);
			num6 = num7 * Math.Sin(-num2);
			num3 += num5;
			num4 += num6;
			Complex b = new Complex(0.5 * num3, 0.5 * num4);
			return complex.Div(b);
		}

		public Complex Tanh()
		{
			double num = Math.Exp(this._re);
			double num2 = num * Math.Cos(this._im);
			double num3 = num * Math.Sin(this._im);
			num = Math.Exp(-this._re);
			double num4 = num * Math.Cos(-this._im);
			double num5 = num * Math.Sin(-this._im);
			num2 -= num4;
			num3 -= num5;
			Complex complex = new Complex(0.5 * num2, 0.5 * num3);
			num = Math.Exp(this._re);
			num2 = num * Math.Cos(this._im);
			num3 = num * Math.Sin(this._im);
			num = Math.Exp(-this._re);
			num4 = num * Math.Cos(-this._im);
			num5 = num * Math.Sin(-this._im);
			num2 += num4;
			num3 += num5;
			Complex b = new Complex(0.5 * num2, 0.5 * num3);
			return complex.Div(b);
		}

		public override string ToString()
		{
			return string.Concat(new object[] {
				"(",
				this._re,
				", ",
				this._im,
				")"
			});
		}

		public double Im
		{
			get
			{
				return this._im;
			}
			set
			{
				this._im = value;
			}
		}

		public double Re
		{
			get
			{
				return this._re;
			}
			set
			{
				this._re = value;
			}
		}
	}
}
