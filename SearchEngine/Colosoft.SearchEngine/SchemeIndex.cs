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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Objeto que define o Schema utilizado pelo Índice
	/// </summary>
	public class SchemeIndex
	{
		private string _indexName;

		private string _indexDescription;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		public SchemeIndex()
		{
		}

		/// <param name="fields">Campos do indice.</param>
		/// <param name="type">Tipo de índice</param>
		/// <param name="includeInSummary">Indica se o índice será ou não incluído no sumário</param>
		/// <param name="description">Descrição que será apresentada do índice</param>
		/// <param name="name">Nome do índice</param>
		/// <param name="isRanked"></param>
		public SchemeIndex(string name, string description, IndexType type, SchemeField[] fields, bool includeInSummary, bool isRanked)
		{
			FieldSchema = fields;
			Type = type;
			IncludeInSummary = includeInSummary;
			IsRanked = isRanked;
			if(name == null)
				_indexName = null;
			else
				_indexName = name;
			if(description == null)
				_indexDescription = null;
			else
				_indexDescription = description;
		}

		/// <summary>
		/// Schema do campo utilizado pelo índice
		/// </summary>
		public SchemeField[] FieldSchema
		{
			get;
			set;
		}

		/// <summary>
		/// id do scheme index
		/// </summary>
		public int SchemeIndexId
		{
			get;
			set;
		}

		/// <summary>
		/// Informa se o índice será ou não incluído no sumário
		/// </summary>
		public bool IncludeInSummary
		{
			get;
			set;
		}

		/// <summary>
		/// Monta o nome do índice
		/// </summary>
		public string Name
		{
			get
			{
				if(String.IsNullOrEmpty(_indexName))
				{
					StringBuilder builder = new StringBuilder();
					foreach (SchemeField sf in FieldSchema)
					{
						builder.Append(sf.Name);
					}
					return builder.ToString();
				}
				else
				{
					return _indexName;
				}
			}
			set
			{
				_indexName = value;
			}
		}

		/// <summary>
		/// Monta a descrição do índice
		/// </summary>
		public string Description
		{
			get
			{
				return String.IsNullOrEmpty(_indexDescription) ? Name : _indexDescription;
			}
			set
			{
				_indexDescription = value;
			}
		}

		/// <summary>
		/// Tipo do índice
		/// </summary>
		public IndexType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Define se a palavra será rankeada
		/// </summary>
		public bool IsRanked
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Enumerador com os tipos de índices
	/// </summary>
	public enum IndexType
	{
		/// <summary>
		/// Índices de texto
		/// </summary>
		String = 1,
		/// <summary>
		/// Índice de valor
		/// </summary>
		Value = 2
	}
}
