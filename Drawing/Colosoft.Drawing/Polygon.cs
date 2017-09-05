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
	/// Representa um poligono.
	/// </summary>
	public class Polygon
	{
		/// <summary>
		/// Pontos.
		/// </summary>
		public readonly List<PointF> Points;

		/// <summary>
		/// Tag.
		/// </summary>
		public object Tag
		{
			get;
			set;
		}

		/// <summary>
		/// Versão.
		/// </summary>
		public int Version
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de pontos.
		/// </summary>
		public int Count
		{
			get
			{
				return Points.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Polygon()
		{
			Points = new List<PointF>();
		}

		/// <summary>
		/// Cria a instancia com os parametros informados.
		/// </summary>
		/// <param name="xs"></param>
		/// <param name="ys"></param>
		/// <param name="c"></param>
		public Polygon(int[] xs, int[] ys, int c)
		{
			Points = new List<PointF>(c);
			for(var i = 0; i < c; i++)
			{
				Points.Add(new PointF(xs[i], ys[i]));
			}
		}

		/// <summary>
		/// Limpa os pontos.
		/// </summary>
		public void Clear()
		{
			Points.Clear();
			Version++;
		}

		/// <summary>
		/// Adiciona um ponto.
		/// </summary>
		/// <param name="p"></param>
		public void AddPoint(PointF p)
		{
			Points.Add(p);
			Version++;
		}

		/// <summary>
		/// Adiciona um ponto.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void AddPoint(float x, float y)
		{
			Points.Add(new PointF(x, y));
			Version++;
		}
	}
}
