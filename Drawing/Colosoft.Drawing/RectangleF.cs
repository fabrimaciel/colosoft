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
	/// Representa um retangulo com valores flutuantes.
	/// </summary>
	public struct RectangleF
	{
		/// <summary>
		/// Posição no eixo X.
		/// </summary>
		public float X;

		/// <summary>
		/// Posição no eixo Y.
		/// </summary>
		public float Y;

		/// <summary>
		/// Largura.
		/// </summary>
		public float Width;

		/// <summary>
		/// Altura.
		/// </summary>
		public float Height;

		/// <summary>
		/// Posição em relação da esquerda.
		/// </summary>
		public float Left
		{
			get
			{
				return X;
			}
		}

		/// <summary>
		/// Posição em relação ao topo
		/// </summary>
		public float Top
		{
			get
			{
				return Y;
			}
		}

		/// <summary>
		/// Posição em relação a direita.
		/// </summary>
		public float Right
		{
			get
			{
				return X + Width;
			}
		}

		/// <summary>
		/// Posição em relação a base.
		/// </summary>
		public float Bottom
		{
			get
			{
				return Y + Height;
			}
		}

		/// <summary>
		/// Cria a instancia com os valores inicias.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public RectangleF(float left, float top, float width, float height)
		{
			X = left;
			Y = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Infla o retangula com a nova dimensão.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Inflate(float width, float height)
		{
			Inflate(new SizeF(width, height));
		}

		/// <summary>
		/// Infla o retangulo com o tamanho informado.
		/// </summary>
		/// <param name="size"></param>
		public void Inflate(SizeF size)
		{
			X -= size.Width;
			Y -= size.Height;
			Width += size.Width * 2;
			Height += size.Height * 2;
		}

		/// <summary>
		/// Verifica se refaz interseção com o retangulo informado.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public bool IntersectsWith(RectangleF rect)
		{
			return !((Left >= rect.Right) || (Right <= rect.Left) || (Top >= rect.Bottom) || (Bottom <= rect.Top));
		}

		/// <summary>
		/// Verifica se contém o ponto informado.
		/// </summary>
		/// <param name="loc"></param>
		/// <returns></returns>
		public bool Contains(PointF loc)
		{
			return (X <= loc.X && loc.X < (X + Width) && Y <= loc.Y && loc.Y < (Y + Height));
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[RectangleF: Left={0} Top={1} Width={2} Height={3}]", Left, Top, Width, Height);
		}
	}
}
