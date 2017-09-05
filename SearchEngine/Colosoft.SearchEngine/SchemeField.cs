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
	/// Objeto com as informações do campo
	/// </summary>
	public class SchemeField
	{
		private string _description;

		private static NameComparer _nameComparer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SchemeField()
		{
		}

		/// <summary>Construtor padrão</summary>
		/// <param name="name">Nome do campo</param>
		/// <param name="type">Tipo de dados do campo</param>
		/// <param name="description">Descrição do campo</param>
		/// <param name="includeInFullText"></param>
		public SchemeField(string name, Type type, string description, bool includeInFullText)
		{
			Name = name;
			Type = type;
			_description = description;
			IncludeInFullText = includeInFullText;
		}

		/// <summary>Construtor padrão</summary>
		/// <param name="name">Nome do campo</param>
		/// <param name="type">Tipo de dados do campo</param>
		/// <param name="description">Descrição do campo</param>
		/// <param name="includeInFullText"></param>
		/// <param name="isSegmentValue">Identifica se os valores do campo são segmentos.</param>
		public SchemeField(string name, Type type, string description, bool includeInFullText, bool isSegmentValue)
		{
			Name = name;
			Type = type;
			_description = description;
			IncludeInFullText = includeInFullText;
			IsSegmentValue = isSegmentValue;
		}

		/// <summary>
		/// Id do SchemaIndex
		/// </summary>
		public int SchemeIndexId
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do campo
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Descritivo do campo
		/// </summary>
		public string Description
		{
			get
			{
				return String.IsNullOrEmpty(_description) ? Name : _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Tipo de dados
		/// </summary>
		public Type Type
		{
			get;
			set;
		}

		/// <summary>
		/// Informa se o campo será ou não incluído no FullText.
		/// </summary>
		public bool IncludeInFullText
		{
			get;
			set;
		}

		/// <summary>
		/// Informa se o campo será ou não incluído nas estatisticas.
		/// </summary>
		public bool IncludeInStatistics
		{
			get;
			set;
		}

		/// <summary>
		/// Informa se o campo será incluido na estatistica de rank
		/// </summary>
		public bool IncludeInRankStatistics
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se os valores do campo são usados como segmento.
		/// </summary>
		public bool IsSegmentValue
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do comparador do esquema.
		/// </summary>
		public static NameComparer NameComparerSchemeField
		{
			get
			{
				if(_nameComparer == null)
					_nameComparer = new NameComparer();
				return _nameComparer;
			}
		}

		/// <summary>
		/// Classe responsável por comparar novo do campo do esquema.
		/// </summary>
		public class NameComparer : IComparer<SchemeField>
		{
			/// <summary>
			/// Compara os dois campos informados.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(SchemeField x, SchemeField y)
			{
				return string.Compare(x.Name, y.Name);
			}
		}
	}
}
