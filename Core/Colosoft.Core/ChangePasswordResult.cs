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
using Colosoft.Security.CaptchaSupport;

namespace Colosoft.Security
{
	/// <summary>
	/// Resultado da troca de senha
	/// </summary>
	public class ChangePasswordResult
	{
		/// <summary>
		/// Indica se a troca foi realizada
		/// </summary>
		public ChangePasswordStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem retornada pela auteração de senha
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Indica que a próxima tentativa de login deverá conter o captcha
		/// </summary>
		public CaptchaInfo Captcha
		{
			get;
			set;
		}
	}
}
