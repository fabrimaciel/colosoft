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
	/// Assituras das classes que gerenciam o captcha.
	/// </summary>
	/// 
	public interface ICaptcha : ICaptchaGenerator
	{
		/// <summary>
		/// Determina se o atual captcha é correto.
		/// </summary>
		/// <param name="uid">Identificador do captcha no sistema.</param>
		/// <param name="userInput">Dados da entrada do usuário.</param>
		/// <returns></returns>
		bool IsCorrect(Guid uid, string userInput);

		/// <summary>
		/// Gera uma imagem do captcha.
		/// </summary>
		///<param name="settings">Configurações</param>
		/// <returns></returns>
		CaptchaInfo Generate(CaptchaSettings settings);

		/// <summary>
		/// Recupera um texto aleatório.
		/// </summary>
		/// <returns></returns>
		string GetRandomText();
	}
}
