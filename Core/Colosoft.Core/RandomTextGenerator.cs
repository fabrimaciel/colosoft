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
	/// Classe responsável por gerar textos aleatórios.
	/// </summary>
	public class RandomTextGenerator : IRandomTextGenerator
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RandomTextGenerator()
		{
			Settings = new RandomTextGeneratorSettings();
			Settings.Length = 5;
			Settings.AllowedChars = "ABCDEFGHJKLMNPRSTUVWXY3456789";
		}

		/// <summary>
		/// Configuração do gerador.
		/// </summary>
		public RandomTextGeneratorSettings Settings
		{
			get;
			set;
		}

		/// <summary>
		/// Gera o texto aleatório.
		/// </summary>
		/// <returns></returns>
		public string Generate()
		{
			Byte[] randomBytes = new Byte[Settings.Length];
			using (System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
			{
				rng.GetBytes(randomBytes);
			}
			char[] chars = new char[Settings.Length];
			int allowedCharCount = Settings.AllowedChars.Length;
			string allowedChars = Settings.AllowedChars;
			for(int i = 0; i < Settings.Length; i++)
			{
				chars[i] = allowedChars[(int)randomBytes[i] % allowedCharCount];
			}
			return new string(chars);
		}
	}
}
