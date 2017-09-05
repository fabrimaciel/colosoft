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
	/// Table das <see cref="LRAction"/>.
	/// </summary>
	internal class LRActionTable
	{
		private List<LRAction> _members = new List<LRAction>();

		/// <summary>
		/// Quantidade de itens na tabela.
		/// </summary>
		public int Count
		{
			get
			{
				return _members.Count;
			}
		}

		/// <summary>
		/// Relação dos membros da table.
		/// </summary>
		public List<LRAction> Members
		{
			get
			{
				return _members;
			}
		}

		/// <summary>
		/// Adiciona um novo item
		/// </summary>
		/// <param name="symbol"></param>
		/// <param name="action"></param>
		/// <param name="value"></param>
		public void AddItem(Symbol symbol, Text.Parser.Action action, int value)
		{
			LRAction lrAction = new LRAction();
			lrAction.Symbol = symbol;
			lrAction.Action = action;
			lrAction.Value = value;
			_members.Add(lrAction);
		}

		/// <summary>
		/// Recupera a ação para o indice do símbolo.
		/// </summary>
		/// <param name="symbolIndex">Indice do símbolo.</param>
		/// <returns></returns>
		public LRAction GetActionForSymbol(int symbolIndex)
		{
			foreach (LRAction action in _members)
				if(action.Symbol.TableIndex == symbolIndex)
					return action;
			return null;
		}

		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public LRAction GetItem(int index)
		{
			if(index >= 0 && index < _members.Count)
				return _members[index];
			return null;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("LALR table:\n");
			foreach (LRAction action in _members)
				builder.Append("- ").Append(action.ToString() + "\n");
			return builder.ToString();
		}
	}
}
