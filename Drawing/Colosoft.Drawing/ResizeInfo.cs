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
	/// Informações do redimensionamento realizado.
	/// </summary>
	public class ResizeInfo
	{
		/// <summary>
		/// Posição X da imagem que será redimensionada.
		/// </summary>
		public int X
		{
			get;
			set;
		}

		/// <summary>
		/// Posição Y da imagem que será redimensionada.
		/// </summary>
		public int Y
		{
			get;
			set;
		}

		/// <summary>
		/// Largura de corte da imagem.
		/// </summary>
		public int CutWidth
		{
			get;
			set;
		}

		/// <summary>
		/// Altura de corte da imagem.
		/// </summary>
		public int CutHeight
		{
			get;
			set;
		}

		/// <summary>
		/// Largura de destino da imagem.
		/// </summary>
		public int Width
		{
			get;
			set;
		}

		/// <summary>
		/// Altura de destinho da imagem.
		/// </summary>
		public int Height
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="x">Posição X da imagem.</param>
		/// <param name="y">Posição Y da imagem.</param>
		/// <param name="cutWidth">Corte de largura da imagem.</param>
		/// <param name="cutHeight">Corte de altura da imagem.</param>
		/// <param name="width">Largura da imagem que será gerada.</param>
		/// <param name="height">Altura da imagem que será gerada.</param>
		public ResizeInfo(int x, int y, int cutWidth, int cutHeight, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.CutWidth = cutWidth;
			this.CutHeight = cutHeight;
			this.Width = width;
			this.Height = height;
		}
	}
}
