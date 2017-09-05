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

namespace Colosoft.SearchEngine.Services
{
	/// <summary>
	/// Campos do canal.
	/// </summary>
	public class ChannelFields
	{
		/// <summary>
		/// Idenficador
		/// </summary>
		public int ChannelId
		{
			get;
			set;
		}

		/// <summary>
		/// Campos do canal.
		/// </summary>
		public FieldInfo[] Fields
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Informações do campo.
	/// </summary>
	public class FieldInfo
	{
		/// <summary>
		/// Nome do campo.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo do campo.
		/// </summary>
		public string TypeFullName
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Item do resultado da pesquisa.
	/// </summary>
	[System.Xml.Serialization.XmlType("Item")]
	public class SearchResultItem
	{
		/// <summary>
		/// Identificador do item.
		/// </summary>
		public int Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Identificado do canal do item.
		/// </summary>
		public int ChannelId
		{
			get;
			set;
		}

		/// <summary>
		/// Valores do item.
		/// </summary>
		public string[] Values
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Armazena os dados do resultado da pesquisa.
	/// </summary>
	public class SearchResult
	{
		/// <summary>
		/// Nome dos campos do resultado.
		/// </summary>
		public ChannelFields[] ChannelsFields
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de itens do resultado da pesquisa.
		/// </summary>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// Linha de inicio do resultado.
		/// </summary>
		public int StartRow
		{
			get;
			set;
		}

		/// <summary>
		/// Sumários do resultado.
		/// </summary>
		public SummaryResult[] Summaries
		{
			get;
			set;
		}

		/// <summary>
		/// Itens do resultado da pesquisa.
		/// </summary>
		public SearchResultItem[] Items
		{
			get;
			set;
		}
	}
}
