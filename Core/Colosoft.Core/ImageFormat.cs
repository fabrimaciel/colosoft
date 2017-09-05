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

namespace Colosoft.Media.Drawing.Imaging
{
	/// <summary>
	/// Formatos de imagem suportados.
	/// </summary>
	public enum ImageFormat
	{
		/// <summary>
		/// Recupera o formato Bitmap (BMP).
		/// </summary>
		Bmp,
		/// <summary>
		/// Recupera o formato Graphics Interchange Format (GIF).
		/// </summary>
		Gif,
		/// <summary>
		/// Recupera o formato Joint Photographic Experts Group (JPEG). 
		/// </summary>
		Jpeg,
		/// <summary>
		/// Recupera o formato W3C Portable Network Graphics (PNG).
		/// </summary>
		Png,
		/// <summary>
		/// Recupera o formato Tagged Image File Format (TIFF).
		/// </summary>
		Tiff,
		/// <summary>
		/// Recupera o formato Microsoft Windows Media Photo.
		/// </summary>
		Wmp,
	}
}
