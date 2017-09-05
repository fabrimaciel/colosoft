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
	/// Representa uma redução.
	/// </summary>
	public class Reduction
	{
		private Rule _parentRule;

		private object _tag;

		private List<Token> _tokens = new List<Token>();

		/// <summary>
		/// Regra pai.
		/// </summary>
		public Rule ParentRule
		{
			get
			{
				return _parentRule;
			}
			set
			{
				_parentRule = value;
			}
		}

		/// <summary>
		/// Tag que representa os dados da instancia.
		/// </summary>
		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		/// <summary>
		/// Tokens associados com a instancia.
		/// </summary>
		public List<Token> Tokens
		{
			get
			{
				return _tokens;
			}
		}

		/// <summary>
		/// Aceita e processa o visitador.
		/// </summary>
		/// <param name="visitor"></param>
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this);
		}

		/// <summary>
		/// Adiciona um novo token para a redução.
		/// </summary>
		/// <param name="token"></param>
		internal void AddToken(Token token)
		{
			_tokens.Add(token);
		}

		/// <summary>
		/// Realiza a visita nos tokens filhos.
		/// </summary>
		/// <param name="visitor"></param>
		public void ChildrenAccept(IVisitor visitor)
		{
			foreach (Token token in _tokens)
			{
				if(token.Kind == SymbolType.NonTerminal)
					(token.Data as Reduction).Accept(visitor);
			}
		}

		/// <summary>
		/// Recupera o token com base no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Token GetToken(int index)
		{
			return _tokens[index];
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _parentRule.ToString();
		}
	}
}
