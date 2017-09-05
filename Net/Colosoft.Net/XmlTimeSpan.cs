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

namespace Colosoft.Net.Serialization.Xml
{
	/// <summary>
	/// Representa um TimeSpan com suporte a serialização XML.
	/// </summary>
	public class XmlTimeSpan
	{
		private TimeSpan _value = TimeSpan.Zero;

		/// <summary>
		/// Valor.
		/// </summary>
		[System.Xml.Serialization.XmlText]
		public string Default
		{
			get
			{
				return _value.ToString();
			}
			set
			{
				_value = TimeSpan.Parse(value);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public XmlTimeSpan()
		{
		}

		/// <summary>
		/// Cria a instancia com o valor do timespan informado.
		/// </summary>
		/// <param name="source"></param>
		public XmlTimeSpan(TimeSpan source)
		{
			_value = source;
		}

		/// <summary>
		/// Converte os valores de forma implicita.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static implicit operator TimeSpan?(XmlTimeSpan o)
		{
			return o == null ? default(TimeSpan?) : o._value;
		}

		/// <summary>
		/// Converte os valores de forma implicita.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static implicit operator XmlTimeSpan(TimeSpan? o)
		{
			return o == null ? null : new XmlTimeSpan(o.Value);
		}

		/// <summary>
		/// Converte os valores de forma implicita.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static implicit operator TimeSpan(XmlTimeSpan o)
		{
			return o == null ? default(TimeSpan) : o._value;
		}

		/// <summary>
		/// Converte os valores de forma implicita.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static implicit operator XmlTimeSpan(TimeSpan o)
		{
			return o == default(TimeSpan) ? null : new XmlTimeSpan(o);
		}
	}
}
