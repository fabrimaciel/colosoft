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
	/// Classe que encapsula o uso do captcha no sistema.
	/// </summary>
	public static class Captcha
	{
		private static ICaptcha _current;

		/// <summary>
		/// Instancia do captcha que está sendo usada pelo sistema.
		/// </summary>
		public static ICaptcha Current
		{
			get
			{
				return _current;
			}
		}

		/// <summary>
		/// Inicializa a instancia do catcha que será usada pelo sistema.
		/// </summary>
		/// <param name="captcha"></param>
		public static void Init(ICaptcha captcha)
		{
			_current = captcha;
		}

		/// <summary>
		/// Recupera um texto aleatório.
		/// </summary>
		/// <returns></returns>
		public static string GetRandomText()
		{
			return _current.GetRandomText();
		}

		/// <summary>
		/// Determina se os dados informados validam o captcha.
		/// </summary>
		/// <param name="uid">Identificador unico do captcha.</param>
		/// <param name="userInput">Texto de entrada do usuário.</param>
		/// <returns></returns>
		public static bool IsCorrect(Guid uid, string userInput)
		{
			return _current.IsCorrect(uid, userInput);
		}

		/// <summary>
		/// Gera um catpcha.
		/// </summary>
		/// <param name="settings">Configurações para a geração do captcha</param>
		/// <returns></returns>
		public static CaptchaInfo Generate(CaptchaSettings settings)
		{
			return _current.Generate(settings);
		}

		/// <summary>
		/// Gera um captcha.
		/// </summary>
		/// <param name="settings">Configurações para a geração do captcha</param>
		/// <param name="randomText">Texto do captcha</param>
		/// <returns></returns>
		public static CaptchaInfo Generate(string randomText, CaptchaSettings settings)
		{
			return _current.Generate(randomText, settings);
		}
	}
}
