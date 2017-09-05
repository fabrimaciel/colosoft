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
	/// Representa o estado do autonomo finito.
	/// </summary>
	internal class FAState
	{
		private int _acceptSymbol = -1;

		private List<FAEdge> _edges = new List<FAEdge>();

		/// <summary>
		/// Quantidade de símbolos aceitos.
		/// </summary>
		public int AcceptSymbol
		{
			get
			{
				return _acceptSymbol;
			}
			set
			{
				_acceptSymbol = value;
			}
		}

		/// <summary>
		/// Quantidade de bordas.
		/// </summary>
		public int EdgeCount
		{
			get
			{
				return _edges.Count;
			}
		}

		/// <summary>
		/// Lista das bordas.
		/// </summary>
		public List<FAEdge> Edges
		{
			get
			{
				return _edges;
			}
		}

		/// <summary>
		/// Adiciona uma nova borda.
		/// </summary>
		/// <param name="characters"></param>
		/// <param name="targetIndex"></param>
		public void AddEdge(string characters, int targetIndex)
		{
			if(characters == "")
			{
				FAEdge edge = new FAEdge(characters, targetIndex);
				_edges.Add(edge);
			}
			else
			{
				int num = -1;
				int count = _edges.Count;
				for(int i = 0; (i < count) && (num == -1); i++)
				{
					FAEdge edge2 = _edges[i];
					if(edge2.TargetIndex == targetIndex)
						num = i;
				}
				if(num == -1)
				{
					FAEdge edge3 = new FAEdge(characters, targetIndex);
					_edges.Add(edge3);
				}
				else
					_edges[num].AddCharacters(characters);
			}
		}

		/// <summary>
		/// Recupera a borda na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public FAEdge GetEdge(int index)
		{
			if((index >= 0) && (index < _edges.Count))
				return _edges[index];
			return null;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("DFA state:\n");
			foreach (FAEdge edge in this._edges)
				builder.Append("- ").Append(edge).Append("\n");
			if(_acceptSymbol != -1)
				builder.Append("- accept symbol: ").Append(_acceptSymbol).Append("\n");
			return builder.ToString();
		}
	}
}
