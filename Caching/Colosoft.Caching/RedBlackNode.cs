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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa o nó da arvore.
	/// </summary>
	public class RedBlackNode
	{
		/// <summary>
		/// Constante do lado preto.
		/// </summary>
		public const int BLACK = 1;

		/// <summary>
		/// Constante do lado vermelho.
		/// </summary>
		public const int RED = 0;

		/// <summary>
		/// Color do nó.
		/// </summary>
		public int Color
		{
			get;
			set;
		}

		/// <summary>
		/// Dados associados.
		/// </summary>
		public Hashtable Data
		{
			get;
			set;
		}

		/// <summary>
		/// Chave que representa o nó.
		/// </summary>
		public IComparable Key
		{
			get;
			set;
		}

		/// <summary>
		/// Nó da esquerda.
		/// </summary>
		public RedBlackNode Left
		{
			get;
			set;
		}

		/// <summary>
		/// Nó pai.
		/// </summary>
		public RedBlackNode Parent
		{
			get;
			set;
		}

		/// <summary>
		/// Nó da direita.
		/// </summary>
		public RedBlackNode Right
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RedBlackNode()
		{
			Color = RED;
			Data = new Hashtable();
		}
	}
}
