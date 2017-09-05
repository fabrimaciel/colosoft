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
	/// Representa uma pilha de tokens.
	/// </summary>
	public class TokenStack : IEnumerable<Token>
	{
		private List<Token> _items = new List<Token>();

		/// <summary>
		/// Quantidade de itens na pilha.
		/// </summary>
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		/// <summary>
		/// Recupera o token que está no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Token this[int index]
		{
			get
			{
				return _items[index];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		internal TokenStack()
		{
		}

		/// <summary>
		/// Limpa todos os itens da pilha.
		/// </summary>
		public void Clear()
		{
			_items.Clear();
		}

		/// <summary>
		/// Recupera o token no indice informado.
		/// </summary>
		/// <param name="index">Indice do token que será recuperado.</param>
		/// <returns></returns>
		public Token GetToken(int index)
		{
			return _items[index];
		}

		/// <summary>
		/// Recupera o token que está no topo da pilha.
		/// </summary>
		/// <returns></returns>
		public Token PeekToken()
		{
			int num = _items.Count - 1;
			if(num >= 0)
				return _items[num];
			return null;
		}

		/// <summary>
		/// Recupera e remove o token que está no topo da pilha.
		/// </summary>
		/// <returns></returns>
		public Token PopToken()
		{
			int index = _items.Count - 1;
			if(index < 0)
				return null;
			Token token = _items[index];
			_items.RemoveAt(index);
			return token;
		}

		/// <summary>
		/// Remove os tokens que estão no topo da pilha e adiciona
		/// a quantidade informada para a redução.
		/// </summary>
		/// <param name="reduction"></param>
		/// <param name="count">Quantidade de itens que serão recuperados,</param>
		public void PopTokensInto(Reduction reduction, int count)
		{
			int index = _items.Count - count;
			int count2 = _items.Count;
			for(int i = index; i < count2; i++)
				reduction.AddToken(_items[i]);
			_items.RemoveRange(index, count);
		}

		/// <summary>
		/// Adiciona um novo token para a pilha.
		/// </summary>
		/// <param name="p_token"></param>
		public void PushToken(Token p_token)
		{
			this._items.Add(p_token);
		}

		/// <summary>
		/// Recupera o enumerador da instancia.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Token> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}
