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

namespace Colosoft.Text
{
	/// <summary>
	/// Implementação da junção de mensagens.
	/// </summary>
	public class JoinMessageFormattable : IMessageFormattable, ICloneable
	{
		private IMessageFormattable _leftMessage;

		private string _separator;

		private IMessageFormattable _rightMessage;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="leftMessage">Instancia da mensagem a esquerda.</param>
		/// <param name="separator">Seperador da mensagem.</param>
		/// <param name="rightMessage">Instancia da mensagem da direita.</param>
		public JoinMessageFormattable(IMessageFormattable leftMessage, string separator, IMessageFormattable rightMessage)
		{
			leftMessage.Require("leftMessage").NotNull();
			rightMessage.Require("rightMessage").NotNull();
			_leftMessage = leftMessage;
			_separator = separator;
			_rightMessage = rightMessage;
		}

		/// <summary>
		/// Formata a mensagem.
		/// </summary>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format()
		{
			return string.Format("{0}{1}{2}", _leftMessage.Format(), _separator, _rightMessage.Format());
		}

		/// <summary>
		/// Formata a mensagem na cultura informada.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format(System.Globalization.CultureInfo culture)
		{
			return string.Format("{0}{1}{2}", _leftMessage.Format(culture), _separator, _rightMessage.Format(culture));
		}

		/// <summary>
		/// Formata a mensagem na cultura informada usando os parametros.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <param name="parameters">Parametros que serão usados na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format(System.Globalization.CultureInfo culture, params object[] parameters)
		{
			return string.Format("{0}{1}{2}", _leftMessage.Format(culture, parameters), _separator, _rightMessage.Format(culture, parameters));
		}

		/// <summary>
		/// Junta a mensagem com outra.
		/// </summary>
		/// <param name="separator">Separador que será usado.</param>
		/// <param name="message">Mensagem que será anexada.</param>
		/// <returns></returns>
		public IMessageFormattable Join(string separator, IMessageFormattable message)
		{
			return new JoinMessageFormattable(this, separator, message);
		}

		/// <summary>
		/// Retorna um valor indicando se a linguagem da descrição 
		/// da suporte a cultura informada.
		/// </summary>
		/// <param name="culture">Instancia da cultura que será comparado com a linguagem da mensagem.</param>
		/// <returns></returns>
		public bool Matches(System.Globalization.CultureInfo culture)
		{
			return _leftMessage.Matches(culture) && _rightMessage.Matches(culture);
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
			var other2 = other as JoinMessageFormattable;
			if(other2 != null)
			{
				return (((_leftMessage != null && other2._leftMessage != null && _leftMessage.Equals(other2._leftMessage)) || (_leftMessage == null && other2._leftMessage == null)) && ((_rightMessage != null && other2._rightMessage != null && _rightMessage.Equals(other2._rightMessage)) || (_rightMessage == null && other2._rightMessage == null)));
			}
			return false;
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var left = (_leftMessage is ICloneable) ? (IMessageFormattable)((ICloneable)_leftMessage).Clone() : _leftMessage;
			var right = (_rightMessage is ICloneable) ? (IMessageFormattable)((ICloneable)_rightMessage).Clone() : _rightMessage;
			return new JoinMessageFormattable(left, _separator, right);
		}
	}
}
