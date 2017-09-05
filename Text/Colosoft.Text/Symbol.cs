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
	/// Possíveis tipos de simbolos.
	/// </summary>
	public enum SymbolType
	{
		/// <summary>
		/// Símbolo não terminal.
		/// </summary>
		NonTerminal,
		/// <summary>
		/// Símbolo terminal.
		/// </summary>
		Terminal,
		/// <summary>
		/// Espaço em branco.
		/// </summary>
		Whitespace,
		/// <summary>
		/// Símbolo de fim.
		/// </summary>
		End,
		/// <summary>
		/// Início de comentário.
		/// </summary>
		CommentStart,
		/// <summary>
		/// Fim de comentário.
		/// </summary>
		CommentEnd,
		/// <summary>
		/// Linha de cometário.
		/// </summary>
		CommentLine,
		/// <summary>
		/// Erro.
		/// </summary>
		Error
	}
	/// <summary>
	/// Representa um simbolo do resultado do parser.
	/// </summary>
	public class Symbol
	{
		private SymbolType _kind;

		private string _name;

		private int _tableIndex;

		/// <summary>
		/// Tipo do simbolo.
		/// </summary>
		public SymbolType Kind
		{
			get
			{
				return _kind;
			}
		}

		/// <summary>
		/// Nome do símbolo.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Indice do símbolo na tabela.
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
		protected internal Symbol() : this(-1, "", SymbolType.Error)
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo so valores iniciais.
		/// </summary>
		/// <param name="index">Indice do símbolo.</param>
		/// <param name="name">Nome associado.</param>
		/// <param name="kind">Tipo de símbolo.</param>
		internal Symbol(int index, string name, SymbolType kind)
		{
			_tableIndex = index;
			_name = name;
			_kind = kind;
		}

		/// <summary>
		/// Copia dos dados do símbolo informado para a instancia.
		/// </summary>
		/// <param name="symbol"></param>
		protected internal void CopyData(Symbol symbol)
		{
			_name = symbol.Name;
			_kind = symbol.Kind;
			_tableIndex = symbol.TableIndex;
		}

		/// <summary>
		/// Método usado para compara um objeto com os dados da instancia.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			Symbol symbol = (Symbol)other;
			return (_name.Equals(symbol.Name) && (_kind == symbol.Kind));
		}

		/// <summary>
		/// Recupera o hashcode que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (_name + "||" + _kind).GetHashCode();
		}

		/// <summary>
		/// Formata a origem no padrão.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		private void PatternFormat(string source, StringBuilder target)
		{
			for(int i = 0; i < source.Length; i++)
			{
				char ch = source[i];
				if(ch == '\'')
					target.Append("''");
				else if("|-+*?()[]{}<>!\"".IndexOf(ch) != -1)
					target.Append("'").Append(ch).Append("'");
				else
					target.Append(ch);
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if(this._kind == SymbolType.NonTerminal)
				builder.Append("<").Append(this._name).Append(">");
			else if(this._kind == SymbolType.Terminal)
				builder.Append(this._name);
			else
				builder.Append("(").Append(this._name).Append(")");
			return builder.ToString();
		}
	}
}
