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
	/// Representa um retangulo.
	/// </summary>
	public struct Rectangle
	{
		/// <summary>
		/// Posição no eixo X.
		/// </summary>
		public int X;

		/// <summary>
		/// Posição no eixo Y.
		/// </summary>
		public int Y;

		/// <summary>
		/// Largura.
		/// </summary>
		public int Width;

		/// <summary>
		/// Altura.
		/// </summary>
		public int Height;

		/// <summary>
		/// Posição em relação da esquerda.
		/// </summary>
		public int Left
		{
			get
			{
				return X;
			}
		}

		/// <summary>
		/// Posição em relação ao topo
		/// </summary>
		public int Top
		{
			get
			{
				return Y;
			}
		}

		/// <summary>
		/// Posição em relação a base.
		/// </summary>
		public int Bottom
		{
			get
			{
				return Top + Height;
			}
		}

		/// <summary>
		/// Posição em relação a direita.
		/// </summary>
		public int Right
		{
			get
			{
				return Left + Width;
			}
		}

		/// <summary>
		/// Cria a instancia com os valores inicias.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Rectangle(int left, int top, int width, int height)
		{
			X = left;
			Y = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Ajusta o offset do retangulo.
		/// </summary>
		/// <param name="dx"></param>
		/// <param name="dy"></param>
		public void Offset(int dx, int dy)
		{
			X += dx;
			Y += dy;
		}

		/// <summary>
		/// Verifica se a coordena informada está contida no retangulo.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Contains(int x, int y)
		{
			return (x >= X && x < X + Width) && (y >= Y && y < Y + Height);
		}

		/// <summary>
		/// Realiza a união dos retangulos informados.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Rectangle Union(Rectangle a, Rectangle b)
		{
			var left = Math.Min(a.Left, b.Left);
			var top = Math.Min(a.Top, b.Top);
			return new Rectangle(left, top, Math.Max(a.Right, b.Right) - left, Math.Max(a.Bottom, b.Bottom) - top);
		}

		/// <summary>
		/// Verifica se faz intersação com o retangulo informado.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public bool IntersectsWith(Rectangle rect)
		{
			return !((Left >= rect.Right) || (Right <= rect.Left) || (Top >= rect.Bottom) || (Bottom <= rect.Top));
		}

		/// <summary>
		/// Infla o retangula com a nova dimensão.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Inflate(int width, int height)
		{
			Inflate(new Size(width, height));
		}

		/// <summary>
		/// Infla o retangulo com o tamanho informado.
		/// </summary>
		/// <param name="size"></param>
		public void Inflate(Size size)
		{
			X -= size.Width;
			Y -= size.Height;
			Width += size.Width * 2;
			Height += size.Height * 2;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Rectangle: Left={0} Top={1} Width={2} Height={3}]", Left, Top, Width, Height);
		}
	}
}
