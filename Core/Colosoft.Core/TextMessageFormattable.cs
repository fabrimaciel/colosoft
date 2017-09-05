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
using System.Runtime.Serialization;

namespace Colosoft.Text
{
	/// <summary>
	/// Implementação de uma mensagem de texto formatada.
	/// </summary>
	[Serializable]
	public class TextMessageFormattable : IMessageFormattable, ICloneable
	{
		private string _text;

		private object[] _parameters;

		/// <summary>
		/// Texto da formatação.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Parametro que serão usados na formatação.
		/// </summary>
		public object[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="parameters"></param>
		public TextMessageFormattable(string text, params object[] parameters)
		{
			_text = text;
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
			return new JoinMessageFormattable(this, separator, message);
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
			if(string.IsNullOrEmpty(_text))
				return null;
			object[] values = null;
			if(parameters != null)
			{
				values = new object[parameters.Length];
				for(int i = 0; i < parameters.Length; i++)
				{
					var obj = parameters[i];
					if(obj is IMessageFormattable)
						values[i] = ((IMessageFormattable)obj).Format(culture);
					else
						values[i] = obj;
				}
			}
			if(values != null && values.Length > 0)
				return string.Format(culture, _text, values);
			return _text;
		}

		/// <summary>
		/// Retorna um valor indicando se a linguagem da descrição 
		/// da suporte a cultura informada.
		/// </summary>
		/// <param name="culture">Instancia da cultura que será comparado com a linguagem da mensagem.</param>
		/// <returns></returns>
		public bool Matches(System.Globalization.CultureInfo culture)
		{
			return true;
		}

		/// <summary>
		/// Compara as instancia da mensagem informado.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IMessageFormattable other)
		{
			var other2 = other as TextMessageFormattable;
			if(other2 != null)
			{
				if(other2._text == _text)
				{
					if(other2._parameters == null && _parameters == null)
						return true;
					if(other2._parameters.Length != _parameters.Length)
						return false;
					for(var i = 0; i < _parameters.Length; i++)
						if(_parameters[i] != other2._parameters[i])
							return false;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Recupera o texto da mensagem.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Format(System.Globalization.CultureInfo.CurrentCulture, null);
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new TextMessageFormattable(_text, _parameters);
		}
	}
}
