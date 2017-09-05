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

namespace Colosoft.Text.Parser
{
	/// <summary>
	/// Representa um token do parser.
	/// </summary>
	public class Token : Symbol
	{
		private object _data;

		private int _state;

		/// <summary>
		/// Instancia com os dados do token.
		/// </summary>
		public object Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// Estado do token
		/// </summary>
		internal int State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		internal Token()
		{
			this._state = -1;
			this._data = "";
		}

		/// <summary>
		/// Cria uma nova instancia do token com base em dados já existentes.
		/// </summary>
		/// <param name="symbol"></param>
		internal Token(Symbol symbol) : this()
		{
			SetParent(symbol);
		}

		/// <summary>
		/// Define o simbolo pai do token.
		/// </summary>
		/// <param name="p_symbol"></param>
		internal void SetParent(Symbol p_symbol)
		{
			base.CopyData(p_symbol);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return base.ToString();
		}
	}
}
