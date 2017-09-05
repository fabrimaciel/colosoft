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

namespace Colosoft.Media.Drawing
{
	/// <summary>
	/// Representa o resultado do redimensionamento.
	/// </summary>
	public struct ResizeResult
	{
		private double _width;

		private double _height;

		/// <summary>
		/// Identifica se o tamanho está vazio.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return ((_width == 0) && (_height == 0));
			}
		}

		/// <summary>
		/// Largura.
		/// </summary>
		public double Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		/// <summary>
		/// Altura.
		/// </summary>
		public double Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public ResizeResult(double width, double height)
		{
			_width = width;
			_height = height;
		}
	}
	/// <summary>
	/// Classe com métodos para auxiliar no redimensionamento de imagens.
	/// </summary>
	public static class Resizer
	{
		/// <summary>
		/// Recupera o tamanho da imagem.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="maxHeight"></param>
		/// <param name="maxWidth"></param>
		/// <param name="height"></param>
		/// <param name="percentual"></param>
		/// <returns></returns>
		public static ResizeResult Resize(double width, double height, double maxWidth, double maxHeight, float percentual = 1f)
		{
			maxHeight = maxHeight > 0 ? maxHeight : height;
			maxWidth = maxWidth > 0 ? maxWidth : width;
			if(width > height)
			{
				if(width <= maxWidth)
				{
					maxWidth = width * percentual;
					maxHeight = height * percentual;
				}
				else
					maxHeight = (maxWidth / width) * height;
			}
			else
			{
				if(height <= maxHeight)
				{
					maxHeight = height * percentual;
					maxWidth = width * percentual;
				}
				else
					maxWidth = (maxHeight / height) * width;
			}
			return new ResizeResult(maxWidth, maxHeight);
		}
	}
}
