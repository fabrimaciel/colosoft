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
	/// Armazena as informações do captcha gerado.
	/// </summary>
	public class CaptchaInfo
	{
		/// <summary>
		/// Identificador do captcha no sistema.
		/// </summary>
		public Guid Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Vetor que representa a imagem do captcha.
		/// </summary>
		public byte[] Image
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Assinatura para as classes responsáveis por gerar o Captcha.
	/// </summary>
	public interface ICaptchaGenerator
	{
		/// <summary>
		/// Gera um captcha para o texto aleatório informado.
		/// </summary>
		/// <param name="randomText">Texto aleatório.</param>
		///<param name="settings">Configurações</param>
		/// <returns></returns>
		CaptchaInfo Generate(string randomText, CaptchaSettings settings);
	}
}
