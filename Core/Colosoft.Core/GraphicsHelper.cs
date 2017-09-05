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
	/// Classe com métodos que auxiliam os calculos gráficos.
	/// </summary>
	public class GraphicsHelper
	{
		/// <summary>
		/// Converte angulo para radianos.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static double ConvertToRadians(double angle)
		{
			return (Math.PI / 180) * angle;
		}

		/// <summary>
		/// Raytracing 
		/// </summary>
		/// <param name="x0"></param>
		/// <param name="y0"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="visit"></param>
		/// <remarks>http://playtechs.blogspot.com.br/2007/03/raytracing-on-grid.html</remarks>
		public static void Raytrace(double x0, double y0, double x1, double y1, Action<int, int> visit)
		{
			double dx = Math.Abs(x1 - x0);
			double dy = Math.Abs(y1 - y0);
			int x = (int)(Math.Floor(x0));
			int y = (int)(Math.Floor(y0));
			int n = 1;
			int x_inc, y_inc;
			double error;
			if(dx == 0)
			{
				x_inc = 0;
				error = double.MaxValue;
			}
			else if(x1 > x0)
			{
				x_inc = 1;
				n += (int)(Math.Floor(x1)) - x;
				error = (Math.Floor(x0) + 1 - x0) * dy;
			}
			else
			{
				x_inc = -1;
				n += x - (int)(Math.Floor(x1));
				error = (x0 - Math.Floor(x0)) * dy;
			}
			if(dy == 0)
			{
				y_inc = 0;
				error -= double.MaxValue;
			}
			else if(y1 > y0)
			{
				y_inc = 1;
				n += (int)(Math.Floor(y1)) - y;
				error -= (Math.Floor(y0) + 1 - y0) * dx;
			}
			else
			{
				y_inc = -1;
				n += y - (int)(Math.Floor(y1));
				error -= (y0 - Math.Floor(y0)) * dx;
			}
			for(; n > 0; --n)
			{
				visit(x, y);
				if(error > 0)
				{
					y += y_inc;
					error -= dx;
				}
				else
				{
					x += x_inc;
					error += dy;
				}
			}
		}
	}
}
