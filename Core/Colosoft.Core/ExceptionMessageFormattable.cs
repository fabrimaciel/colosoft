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

namespace Colosoft.Logging
{
	/// <summary>
	/// Formatação de mensagens para exceptions.
	/// </summary>
	public class ExceptionMessageFormattable : IMessageFormattable
	{
		private Exception _exception;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="exception"></param>
		public ExceptionMessageFormattable(Exception exception)
		{
			exception.Require("ex").NotNull();
			_exception = exception;
		}

		/// <summary>
		/// Junta a mensagem com outra.
		/// </summary>
		/// <param name="separator">Separador que será usado.</param>
		/// <param name="message">Mensagem que será anexada.</param>
		/// <returns></returns>
		public IMessageFormattable Join(string separator, IMessageFormattable message)
		{
			return new Colosoft.Text.JoinMessageFormattable(this, separator, message);
		}

		/// <summary>
		/// Formata a mensagem.
		/// </summary>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format()
		{
			return Format(System.Globalization.CultureInfo.CurrentCulture, null);
		}

		/// <summary>
		/// Recupera o texto formatado.
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public string Format(System.Globalization.CultureInfo culture)
		{
			return _exception.ToString();
		}

		/// <summary>
		/// Recupera o texto formatado.
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public string Format(System.Globalization.CultureInfo culture, params object[] parameters)
		{
			return _exception.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public bool Matches(System.Globalization.CultureInfo culture)
		{
			return true;
		}

		/// <summary>
		/// Verifica se é igual a instancia informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IMessageFormattable other)
		{
			if(other == null)
				return false;
			var other2 = other as ExceptionMessageFormattable;
			if(other2 != null)
				return _exception.ToString() == other2._exception.ToString();
			return false;
		}
	}
}
