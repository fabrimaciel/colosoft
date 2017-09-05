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

namespace Colosoft.Security.CaptchaSupport
{
	/// <summary>
	/// Classe de configuração do captcha.
	/// </summary>
	public class CaptchaSettings
	{
		/// <summary>
		/// Construtor com inicialização da configuração
		/// </summary>
		/// <param name="height">Altura</param>
		/// <param name="width">Largura</param>
		/// <param name="font">Fonte</param>
		/// <param name="numChars">Número de caracteres</param>
		/// <param name="isCaseSensitive">Se tratará caso</param>
		public CaptchaSettings(int height, int width, string font, int numChars, bool isCaseSensitive)
		{
			Height = height;
			Width = width;
			Font = font;
			NumChars = numChars;
			IsCaseSensitive = isCaseSensitive;
		}

		/// <summary>
		/// Altura da imagem que será gerada.
		/// </summary>
		public int Height
		{
			get;
			set;
		}

		/// <summary>
		/// Largura da imagem que será gerada.
		/// </summary>
		/// <value>The allowed chars.</value>
		public int Width
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da fonte que será utilizada.
		/// </summary>
		public string Font
		{
			get;
			set;
		}

		/// <summary>
		/// Quatidade de letras que será feitas
		/// </summary>
		public int NumChars
		{
			get;
			set;
		}

		/// <summary>
		/// Determina se a validação do captcha é CaseSensitive.
		/// </summary>
		public bool IsCaseSensitive
		{
			get;
			set;
		}
	}
}
