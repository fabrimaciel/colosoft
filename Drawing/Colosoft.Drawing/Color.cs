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
	/// Cor.
	/// </summary>
	public class Color
	{
		/// <summary>
		/// Valor do vermelho.
		/// </summary>
		public readonly int Red;

		/// <summary>
		/// Valor do verde.
		/// </summary>
		public readonly int Green;

		/// <summary>
		/// Valor do azul.
		/// </summary>
		public readonly int Blue;

		/// <summary>
		/// Valor do alfa.
		/// </summary>
		public readonly int Alpha;

		/// <summary>
		/// Tag associada.
		/// </summary>
		public object Tag;

		/// <summary>
		/// Valor do vermelho.
		/// </summary>
		public float RedValue
		{
			get
			{
				return Red / 255.0f;
			}
		}

		/// <summary>
		/// Valor do verde.
		/// </summary>
		public float GreenValue
		{
			get
			{
				return Green / 255.0f;
			}
		}

		/// <summary>
		/// Valor do azul.
		/// </summary>
		public float BlueValue
		{
			get
			{
				return Blue / 255.0f;
			}
		}

		/// <summary>
		/// Valor do alfa.
		/// </summary>
		public float AlphaValue
		{
			get
			{
				return Alpha / 255.0f;
			}
		}

		/// <summary>
		/// Intensidade.
		/// </summary>
		public int Intensity
		{
			get
			{
				return (Red + Green + Blue) / 3;
			}
		}

		/// <summary>
		/// Identifica se é branco.
		/// </summary>
		public bool IsWhite
		{
			get
			{
				return (Red == 255) && (Green == 255) && (Blue == 255);
			}
		}

		/// <summary>
		/// Identifica se é preto.
		/// </summary>
		public bool IsBlack
		{
			get
			{
				return (Red == 0) && (Green == 0) && (Blue == 0);
			}
		}

		/// <summary>
		/// Cria a instancia com os valores do RGB.
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		public Color(int red, int green, int blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = 255;
		}

		/// <summary>
		/// Cria a instancia com os valores do RBG mais o alfa.
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <param name="alpha"></param>
		public Color(int red, int green, int blue, int alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		/// <summary>
		/// Recupera o cor invertida.
		/// </summary>
		/// <returns></returns>
		public Color GetInvertedColor()
		{
			return new Color(255 - Red, 255 - Green, 255 - Blue, Alpha);
		}

		/// <summary>
		/// Recodifica o texto para a cor.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static Color Decode(string text)
		{
			return Colors.Black;
		}

		/// <summary>
		/// Verifica se as cores são iguais.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool AreEqual(Color a, Color b)
		{
			if(a == null && b == null)
				return true;
			if(a == null && b != null)
				return false;
			if(a != null && b == null)
				return false;
			return (a.Red == b.Red && a.Green == b.Green && a.Blue == b.Blue && a.Alpha == b.Alpha);
		}

		/// <summary>
		/// Recupera cor com o alfa informado.
		/// </summary>
		/// <param name="aa"></param>
		/// <returns></returns>
		public Color WithAlpha(int aa)
		{
			return new Color(Red, Green, Blue, aa);
		}

		/// <summary>
		/// Verifica se o objeto informado é igual a isntancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var o = obj as Color;
			return (o != null) && (o.Red == Red) && (o.Green == Green) && (o.Blue == Blue) && (o.Alpha == Alpha);
		}

		/// <summary>
		/// Recupera o hashcode da instanci.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (Red + Green + Blue + Alpha).GetHashCode();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Color: RedValue={0}, GreenValue={1}, BlueValue={2}, AlphaValue={3}]", RedValue, GreenValue, BlueValue, AlphaValue);
		}
	}
}
