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
using System.Linq;
using System.Text;

namespace Colosoft.Drawing
{
	/// <summary>
	/// Representa uma linha.
	/// </summary>
	public class Line
	{
		/// <summary>
		/// X do ponto inicial.
		/// </summary>
		public double X1
		{
			get;
			set;
		}

		/// <summary>
		/// Y do ponto inicial.
		/// </summary>
		public double Y1
		{
			get;
			set;
		}

		/// <summary>
		/// X do ponto final.
		/// </summary>
		public double X2
		{
			get;
			set;
		}

		/// <summary>
		/// Y do ponto final.
		/// </summary>
		public double Y2
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Line()
		{
		}

		/// <summary>
		/// Cria a instancia com os valores iniciais.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		public Line(double x1, double y1, double x2, double y2)
		{
			X1 = x1;
			Y1 = y1;
			X2 = x2;
			Y2 = y2;
		}

		/// <summary>
		/// Calculates intersection - if any - of two lines
		/// </summary>
		/// <param name="otherLine"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>True for ok</returns>
		/// <remarks>Taken from http://tog.acm.org/resources/GraphicsGems/gemsii/xlines.c </remarks>
		public bool Intersection(Line otherLine, out double x, out double y)
		{
			var a1 = Y2 - Y1;
			var b1 = X1 - X2;
			var c1 = X2 * Y1 - X1 * Y2;
			var r3 = a1 * otherLine.X1 + b1 * otherLine.Y1 + c1;
			var r4 = a1 * otherLine.X2 + b1 * otherLine.Y2 + c1;
			if(r3 != 0 && r4 != 0 && Math.Sign(r3) == Math.Sign(r4))
			{
				x = y = 0;
				return false;
			}
			var a2 = otherLine.Y2 - otherLine.Y1;
			var b2 = otherLine.X1 - otherLine.X2;
			var c2 = otherLine.X2 * otherLine.Y1 - otherLine.X1 * otherLine.Y2;
			var r1 = a2 * X1 + b2 * Y1 + c2;
			var r2 = a2 * X2 + b2 * Y2 + c2;
			if(r1 != 0 && r2 != 0 && Math.Sign(r1) == Math.Sign(r2))
			{
				x = y = 0;
				return false;
			}
			var denom = a1 * b2 - a2 * b1;
			if(denom == 0)
			{
				x = y = 0;
				return false;
			}
			var offset = denom < 0 ? -denom / 2 : denom / 2;
			var num = b1 * c2 - b2 * c1;
			x = (num < 0 ? num - offset : num + offset) / denom;
			num = a2 * c1 - a1 * c2;
			y = (num < 0 ? num - offset : num + offset) / denom;
			return true;
		}
	}
}
