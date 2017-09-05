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

namespace Colosoft
{
	/// <summary>
	/// Assinatura das classes responsáveis por formatar mensagens.
	/// </summary>
	public interface IMessageFormattable : IEquatable<IMessageFormattable>
	{
		/// <summary>
		/// Formata a mensagem.
		/// </summary>
		/// <returns>Texto da mensagem formatada.</returns>
		string Format();

		/// <summary>
		/// Formata a mensagem na cultura informada.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		string Format(System.Globalization.CultureInfo culture);

		/// <summary>
		/// Formata a mensagem na cultura informada usando os parametros.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <param name="parameters">Parametros que serão usados na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		string Format(System.Globalization.CultureInfo culture, params object[] parameters);

		/// <summary>
		/// Junta a mensagem com outra.
		/// </summary>
		/// <param name="separator">Separador que será usado.</param>
		/// <param name="message">Mensagem que será anexada.</param>
		/// <returns></returns>
		IMessageFormattable Join(string separator, IMessageFormattable message);

		/// <summary>
		/// Retorna um valor indicando se a linguagem da descrição 
		/// da suporte a cultura informada.
		/// </summary>
		/// <param name="culture">Instancia da cultura que será comparado com a linguagem da mensagem.</param>
		/// <returns></returns>
		bool Matches(System.Globalization.CultureInfo culture);
	}
}
