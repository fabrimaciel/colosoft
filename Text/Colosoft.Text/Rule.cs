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
using System.Collections;

namespace Colosoft.Text.Parser
{
	/// <summary>
	/// Representa um regra do parser.
	/// </summary>
	public class Rule
	{
		private Symbol _ruleNT;

		private List<Symbol> _ruleSymbols = new List<Symbol>();

		private int _tableIndex;

		/// <summary>
		/// Identifica se a instancia possui algum simbolo não terminal.
		/// </summary>
		internal bool ContainsOneNonTerminal
		{
			get
			{
				return this._ruleSymbols.Count == 1 && _ruleSymbols[0].Kind == SymbolType.NonTerminal;
			}
		}

		/// <summary>
		/// Recupera a definição da regra.
		/// </summary>
		internal string Definition
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				foreach (var symb in _ruleSymbols)
					builder.Append(symb.ToString()).Append(" ");
				return builder.ToString();
			}
		}

		/// <summary>
		/// Nome da regra.
		/// </summary>
		internal string Name
		{
			get
			{
				return ("<" + _ruleNT.Name + ">");
			}
		}

		/// <summary>
		/// Simbolo não terminal da regra.
		/// </summary>
		public Symbol RuleNonTerminal
		{
			get
			{
				return _ruleNT;
			}
		}

		/// <summary>
		/// Quantidade de simbolos na regra.
		/// </summary>
		public int SymbolCount
		{
			get
			{
				return _ruleSymbols.Count;
			}
		}

		/// <summary>
		/// Indice da instancia.
		/// </summary>
		public int TableIndex
		{
			get
			{
				return _tableIndex;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="tableIndex"></param>
		/// <param name="head"></param>
		internal Rule(int tableIndex, Symbol head)
		{
			_tableIndex = tableIndex;
			_ruleNT = head;
		}

		/// <summary>
		/// Adiciona um novo simbolo para a regra.
		/// </summary>
		/// <param name="symbol"></param>
		internal void AddItem(Symbol symbol)
		{
			_ruleSymbols.Add(symbol);
		}

		/// <summary>
		/// Recupera um símbolo associado com a instancia na posição informada.
		/// </summary>
		/// <param name="index">Posição do simbolo na instancia.</param>
		/// <returns></returns>
		public Symbol GetSymbol(int index)
		{
			if((index >= 0) && (index < _ruleSymbols.Count))
				return _ruleSymbols[index];
			return null;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (this.Name + " ::= " + this.Definition);
		}
	}
}
