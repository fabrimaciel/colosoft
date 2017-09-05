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
	/// Possíveis ações.
	/// </summary>
	internal enum Action
	{
		/// <summary>
		/// Aceitação.
		/// </summary>
		Accept = 4,
		/// <summary>
		/// Error.
		/// </summary>
		Error = 5,
		/// <summary>
		/// Ir para.
		/// </summary>
		Goto = 3,
		/// <summary>
		/// Reduzir.
		/// </summary>
		Reduce = 2,
		/// <summary>
		/// Shift.
		/// </summary>
		Shift = 1
	}
	/// <summary>
	/// Left-to-right Right-derivation parsing.
	/// </summary>
	internal class LRAction
	{
		private Text.Parser.Action _action;

		private Symbol _symbol;

		private int _value;

		/// <summary>
		/// Ação associada.
		/// </summary>
		public Text.Parser.Action Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		/// <summary>
		/// Símbolo associado.
		/// </summary>
		public Symbol Symbol
		{
			get
			{
				return _symbol;
			}
			set
			{
				_symbol = value;
			}
		}

		/// <summary>
		/// Valor associado.
		/// </summary>
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Concat(new object[] {
				"LALR action [symbol=",
				this._symbol,
				",action=",
				this._action,
				",value=",
				this._value,
				"]"
			});
		}
	}
}
