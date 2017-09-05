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
using System.Text;
using Colosoft.Security.CaptchaSupport;
using System.IO;
using System.Drawing.Imaging;

namespace Colosoft.Security.Captcha
{
	/// <summary>
	/// Classe administradora do captcha
	/// </summary>
	public class CaptchaImpl : ICaptcha
	{
		/// <summary>
		/// Estrutura auxiliar de armazenamento
		/// </summary>
		internal struct CaptchaStore
		{
			/// <summary>
			/// Texto
			/// </summary>
			public string Text
			{
				get;
				set;
			}

			/// <summary>
			/// Validação de caso
			/// </summary>
			public bool IsCaseSensitive
			{
				get;
				set;
			}
		}

		private static Dictionary<Guid, CaptchaStore> _pendingCaptcha = new Dictionary<Guid, CaptchaStore>();

		private CaptchaSettings _settings;

		/// <summary>
		/// Gera um novo captcha
		/// </summary>
		/// <param name="settings">parâmetros para a geração</param>
		/// <returns></returns>
		public CaptchaInfo Generate(CaptchaSettings settings)
		{
			_settings = settings;
			return Generate(GetRandomText(), settings);
		}

		/// <summary>
		/// Gera um texto randômico para o captcha
		/// </summary>
		/// <returns></returns>
		public string GetRandomText()
		{
			RandomTextGenerator textGenerator = new RandomTextGenerator();
			textGenerator.Settings.Length = _settings.NumChars;
			return textGenerator.Generate();
		}

		/// <summary>
		/// Válida a string digitada pelo usuário
		/// </summary>
		/// <param name="uid">identificador</param>
		/// <param name="userInput">string</param>
		/// <returns></returns>
		public bool IsCorrect(Guid uid, string userInput)
		{
			bool result;
			if(_pendingCaptcha.ContainsKey(uid))
			{
				if(_pendingCaptcha[uid].IsCaseSensitive)
				{
					result = userInput.Equals(_pendingCaptcha[uid].Text);
				}
				else
				{
					result = userInput.ToUpper().Equals(_pendingCaptcha[uid].Text.ToUpper());
				}
				_pendingCaptcha.Remove(uid);
			}
			else
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Gera um novo captcha
		/// </summary>
		/// <param name="randomText">Texto do captcha</param>
		/// <param name="settings">parâmetros para a geração</param>
		/// <returns></returns>
		public CaptchaInfo Generate(string randomText, CaptchaSettings settings)
		{
			_settings = settings;
			CaptchaInfo result = new CaptchaInfo();
			CaptchaImage captcha = new CaptchaImage(randomText, _settings.Width, _settings.Height, _settings.Font);
			using (MemoryStream ms = new MemoryStream())
			{
				captcha.Image.Save(ms, ImageFormat.Jpeg);
				result.Image = ms.ToArray();
				result.Uid = Guid.NewGuid();
				_pendingCaptcha.Add(result.Uid, new CaptchaStore() {
					Text = randomText,
					IsCaseSensitive = _settings.IsCaseSensitive
				});
			}
			return result;
		}
	}
}
