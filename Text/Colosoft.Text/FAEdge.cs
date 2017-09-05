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
	/// Representa o autonomo finito.
	/// </summary>
	internal class FAEdge
	{
		private string _characters;

		private int _targetIndex;

		/// <summary>
		/// Letras da instancia.
		/// </summary>
		public string Characters
		{
			get
			{
				return _characters;
			}
			set
			{
				_characters = value;
			}
		}

		/// <summary>
		/// Indice onde a instancia foi introduzida.
		/// </summary>
		public int TargetIndex
		{
			get
			{
				return _targetIndex;
			}
			set
			{
				_targetIndex = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="characters">Letras do autonomo.</param>
		/// <param name="targetIndex"></param>
		public FAEdge(string characters, int targetIndex)
		{
			_characters = characters;
			_targetIndex = targetIndex;
		}

		/// <summary>
		/// Adiciona mais letras para a instancia.
		/// </summary>
		/// <param name="characters"></param>
		public void AddCharacters(string characters)
		{
			_characters += characters;
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Concat(new object[] {
				"DFA edge [chars=[",
				this._characters,
				"],action=",
				this._targetIndex,
				"]"
			});
		}
	}
}
