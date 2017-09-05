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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Representa um indice de rank.
	/// </summary>
	public class IndexRank
	{
		/// <summary>
		/// id do rank
		/// </summary>
		public int IndexrankId
		{
			get;
			set;
		}

		/// <summary>
		/// id do schemeindex
		/// </summary>
		public int SchemeindexId
		{
			get;
			set;
		}

		/// <summary>
		/// id do canal
		/// </summary>
		public int ChannelId
		{
			get;
			set;
		}

		/// <summary>
		/// palavra
		/// </summary>
		public string Word
		{
			get;
			set;
		}

		/// <summary>
		/// contador de pesquisa
		/// </summary>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// contador sum
		/// </summary>
		public int TodayCount
		{
			get;
			set;
		}
	}
}
