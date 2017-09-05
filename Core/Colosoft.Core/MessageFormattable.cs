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
	/// Implementação base da interface <see cref="IMessageFormattable"/>.
	/// </summary>
	public class MessageFormattable : IMessageFormattable
	{
		/// <summary>
		/// Mensagem vazia.
		/// </summary>
		public static readonly IMessageFormattable Empty = "".GetFormatter();

		private Message[] _messages;

		private object[] _parameters;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="messages"></param>
		/// <param name="parameters"></param>
		public MessageFormattable(Message[] messages, params object[] parameters)
		{
			messages.Require("message").NotNull().NotEmptyCollection();
			_messages = messages;
			_parameters = parameters;
		}

		/// <summary>
		/// Junta a mensagem com outra.
		/// </summary>
		/// <param name="separator">Separador que será usado.</param>
		/// <param name="message">Mensagem que será anexada.</param>
		/// <returns></returns>
		public IMessageFormattable Join(string separator, IMessageFormattable message)
		{
			return new Text.JoinMessageFormattable(this, separator, message);
		}

		/// <summary>
		/// Formata a mensagem.
		/// </summary>
		/// <returns>Texto da mensagem formatada.</returns>
		string IMessageFormattable.Format()
		{
			return Format(System.Globalization.CultureInfo.CurrentCulture, null);
		}

		/// <summary>
		/// Formata a mensagem na cultura informada.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		string IMessageFormattable.Format(System.Globalization.CultureInfo culture)
		{
			return Format(culture, _parameters);
		}

		/// <summary>
		/// Formata a mensagem na cultura informada usando os parametros.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <param name="parameters">Parametros que serão usados na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format(System.Globalization.CultureInfo culture, params object[] parameters)
		{
			foreach (var i in _messages)
				if((culture == null && i.CultureInfo == null) || (culture != null && i.CultureInfo == culture.Name))
					return i.Format(parameters);
			var message = _messages.Where(f => string.IsNullOrEmpty(f.CultureInfo) || f.CultureInfo == "ivl").FirstOrDefault();
			return message == null ? null : message.Format(parameters);
		}

		/// <summary>
		/// Retorna um valor indicando se a linguagem da descrição 
		/// da suporte a cultura informada.
		/// </summary>
		/// <param name="culture">Instancia da cultura que será comparado com a linguagem da mensagem.</param>
		/// <returns></returns>
		public bool Matches(System.Globalization.CultureInfo culture)
		{
			foreach (var i in _messages)
				if((culture == null && i.CultureInfo == null) || (culture != null && i.CultureInfo == culture.Name))
					return true;
			return _messages.Where(f => string.IsNullOrEmpty(f.CultureInfo) || f.CultureInfo == "ivl").Any();
		}

		/// <summary>
		/// Verifica se as instancia são equivalentes
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IMessageFormattable other)
		{
			if(other == null)
				return false;
			return StringComparer.Ordinal.Equals(this.Format(System.Globalization.CultureInfo.InvariantCulture, _parameters), other.Format(System.Globalization.CultureInfo.InvariantCulture, _parameters));
		}

		/// <summary>
		/// Armazena os dados de uma mensagem.
		/// </summary>
		public class Message
		{
			private string _text;

			private string _cultureInfo;

			/// <summary>
			/// Texto da mensagem.
			/// </summary>
			public string Text
			{
				get
				{
					return _text;
				}
			}

			/// <summary>
			/// Cultura associada.
			/// </summary>
			public string CultureInfo
			{
				get
				{
					return _cultureInfo;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="text"></param>
			/// <param name="cultureInfo"></param>
			public Message(string text, string cultureInfo)
			{
				_text = text;
				_cultureInfo = cultureInfo;
			}

			/// <summary>
			/// Formata a mensagem.
			/// </summary>
			/// <param name="args"></param>
			/// <returns></returns>
			public string Format(params object[] args)
			{
				if(string.IsNullOrEmpty(_text))
					return "";
				if(string.IsNullOrEmpty(_cultureInfo))
					return string.Format(_text, args);
				return string.Format(System.Globalization.CultureInfo.GetCultureInfo(_cultureInfo), _text, args);
			}
		}
	}
}
