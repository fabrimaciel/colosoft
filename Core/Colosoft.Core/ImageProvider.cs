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
	/// Classe que prover acesso a elementos de image.
	/// </summary>
	public class ImageProvider
	{
		private static ImageProvider _instance = new ImageProvider();

		private object _objLock = new object();

		private List<IImageConverter> _converters = new List<IImageConverter>();

		/// <summary>
		/// Instancia única do provider.
		/// </summary>
		public static ImageProvider Instance
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Relação dos converters da instancia.
		/// </summary>
		public IEnumerable<IImageConverter> Converters
		{
			get
			{
				return _converters.ToArray();
			}
		}

		/// <summary>
		/// Construtor privado.
		/// </summary>
		private ImageProvider()
		{
		}

		/// <summary>
		/// Recupera o formato de imagem com base na extensão do arquivo informada.
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <returns></returns>
		public static Colosoft.Media.Drawing.Imaging.ImageFormat GetImageFormat(string fileExtension)
		{
			fileExtension.Require("fileExtension").NotEmpty().NotEmpty();
			fileExtension = fileExtension.ToLower();
			switch(fileExtension)
			{
			case ".bmp":
				return Imaging.ImageFormat.Bmp;
			case ".jpeg":
			case ".jpg":
				return Imaging.ImageFormat.Jpeg;
			case ".gif":
				return Imaging.ImageFormat.Gif;
			case ".png":
				return Imaging.ImageFormat.Png;
			case ".tiff":
				return Imaging.ImageFormat.Tiff;
			case ".Wmp":
				return Imaging.ImageFormat.Wmp;
			}
			return Imaging.ImageFormat.Jpeg;
		}

		/// <summary>
		/// Adiciona um conversor para a instancia.
		/// </summary>
		/// <param name="converter"></param>
		public void Add(IImageConverter converter)
		{
			lock (_objLock)
				_converters.Add(converter);
		}

		/// <summary>
		/// Remove o converter.
		/// </summary>
		/// <param name="converter"></param>
		/// <returns></returns>
		public bool Remove(IImageConverter converter)
		{
			lock (_objLock)
				return _converters.Remove(converter);
		}

		/// <summary>
		/// Converte o valor informado em uma <see cref="IImage"/>.
		/// </summary>
		/// <param name="value">Valor que será convertido.</param>
		/// <returns></returns>
		public IImage Convert(object value)
		{
			foreach (var converter in _converters)
				if(converter.IsCompatible(value))
					return converter.Convert(value);
			return null;
		}
	}
}
