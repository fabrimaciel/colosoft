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
	/// Assinatura de uma classe de imagem.
	/// </summary>
	public interface IImage
	{
		/// <summary>
		/// Obtém a largura do bitmap.
		/// </summary>
		double Width
		{
			get;
		}

		/// <summary>
		/// Obtém a altura do bitmap.
		/// </summary>
		double Height
		{
			get;
		}

		/// <summary>
		/// Salva a imagem na <see cref="System.IO.Stream"/> usando o formato informado.
		/// </summary>
		/// <param name="stream">Stream onde a imagem será salva.</param>
		/// <param name="imageFormat">Formato da imagens que será salva.</param>
		void Save(System.IO.Stream stream, Imaging.ImageFormat imageFormat);
	}
}
