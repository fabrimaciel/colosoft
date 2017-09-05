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
	/// Representa uma fonte.
	/// </summary>
	public class Font
	{
		static Font[] _boldSystemFonts = new Font[0];

		static Font[] _systemFonts = new Font[0];

		static Font[] _userFixedPitchFonts = new Font[0];

		static Font[] _boldUserFixedPitchFonts = new Font[0];

		/// <summary>
		/// Família da fonte.
		/// </summary>
		public string FontFamily
		{
			get;
			private set;
		}

		/// <summary>
		/// Opções.
		/// </summary>
		public FontOptions Options
		{
			get;
			private set;
		}

		/// <summary>
		/// Tamanho.
		/// </summary>
		public int Size
		{
			get;
			private set;
		}

		/// <summary>
		/// Tag.
		/// </summary>
		public object Tag
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é negrito.
		/// </summary>
		public bool IsBold
		{
			get
			{
				return (Options & FontOptions.Bold) != 0;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fontFamily"></param>
		/// <param name="options"></param>
		/// <param name="size"></param>
		public Font(string fontFamily, FontOptions options, int size)
		{
			FontFamily = fontFamily;
			Options = options;
			Size = size;
		}

		/// <summary>
		/// Recupera a fonte do sistema em negrito com o tamanho informado.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Font BoldSystemFontOfSize(int size)
		{
			if(size <= 0)
				return BoldSystemFontOfSize(1);
			if(size >= _boldSystemFonts.Length)
			{
				return new Font("SystemFont", FontOptions.Bold, size);
			}
			else
			{
				var f = _boldSystemFonts[size];
				if(f == null)
				{
					f = new Font("SystemFont", FontOptions.Bold, size);
					_boldSystemFonts[size] = f;
				}
				return f;
			}
		}

		/// <summary>
		/// Recupera a fonte do sistema com o tamanho informado.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Font SystemFontOfSize(int size)
		{
			if(size >= _systemFonts.Length)
			{
				return new Font("SystemFont", FontOptions.None, size);
			}
			else
			{
				var f = _systemFonts[size];
				if(f == null)
				{
					f = new Font("SystemFont", FontOptions.None, size);
					_systemFonts[size] = f;
				}
				return f;
			}
		}

		/// <summary>
		/// UserFixedPitchFontOfSize
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Font UserFixedPitchFontOfSize(int size)
		{
			if(size >= _userFixedPitchFonts.Length)
			{
				return new Font("Monospace", FontOptions.None, size);
			}
			else
			{
				var f = _userFixedPitchFonts[size];
				if(f == null)
				{
					f = new Font("Monospace", FontOptions.None, size);
					_userFixedPitchFonts[size] = f;
				}
				return f;
			}
		}

		/// <summary>
		/// BoldUserFixedPitchFontOfSize
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Font BoldUserFixedPitchFontOfSize(int size)
		{
			if(size >= _boldUserFixedPitchFonts.Length)
			{
				return new Font("Monospace", FontOptions.Bold, size);
			}
			else
			{
				var f = _boldUserFixedPitchFonts[size];
				if(f == null)
				{
					f = new Font("Monospace", FontOptions.Bold, size);
					_boldUserFixedPitchFonts[size] = f;
				}
				return f;
			}
		}

		/// <summary>
		/// Recupera a fonte pelo nome.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Font FromName(string name, int size)
		{
			return new Font(name, FontOptions.None, size);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Font: FontFamily={0}, Options={1}, Size={2}, Tag={3}]", FontFamily, Options, Size, Tag);
		}
	}
}
