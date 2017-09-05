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
	/// Informa o tipo de parâmetro
	/// </summary>
	public enum SearchType
	{
		/// <summary>
		/// Igual.
		/// </summary>
		Equal,
		/// <summary>
		/// Contém
		/// </summary>
		Like,
		/// <summary>
		/// Maior.
		/// </summary>
		Greater,
		/// <summary>
		/// Maior igual.
		/// </summary>
		GreaterEqual,
		/// <summary>
		/// Menor.
		/// </summary>
		Less,
		/// <summary>
		/// Menor igual.
		/// </summary>
		LessEqual,
		/// <summary>
		/// Entre.
		/// </summary>
		Between,
		/// <summary>
		/// E.
		/// </summary>
		And,
		/// <summary>
		/// Dentro.
		/// </summary>
		In,
		/// <summary>
		/// Ou.
		/// </summary>
		Or,
		/// <summary>
		/// Desconhecido.
		/// </summary>
		Unknown
	}
	/// <summary>
	/// Classe que armazena os parâmetros de busca
	/// </summary>
	public abstract class Parameter
	{
		private string _name;

		private string[] _values;

		private SearchType _searchType = SearchType.Equal;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public Parameter(string name, string value)
		{
			_name = name;
			_values = new string[] {
				value
			};
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="name"></param>
		/// <param name="values"></param>
		public Parameter(string name, string[] values)
		{
			_name = name;
			_values = values;
		}

		/// <summary>
		/// Nome do índice
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Valor a ser buscado
		/// </summary>
		public string[] Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;
			}
		}

		/// <summary>
		/// Retorna o valor do parâmetro para parâmetros simples
		/// </summary>
		public string Value
		{
			get
			{
				return _values.Length == 1 ? _values[0] : null;
			}
		}

		/// <summary>
		/// Indica se o tipo de parâmetro é simples ou composto
		/// </summary>
		public SearchType SearchType
		{
			get
			{
				return _searchType;
			}
			set
			{
				_searchType = value;
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("<{0}><{1}>", Name, string.Join("$", Values.Select(f => System.Security.SecurityElement.Escape(f)).ToArray()));
		}

		/// <summary>
		/// Classe usada para comparar nomes de parametros.
		/// </summary>
		public class NameEqualityComparer : IEqualityComparer<Parameter>
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(Parameter x, Parameter y)
			{
				return (x != null && y != null && x.Name == y.Name);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(Parameter obj)
			{
				return obj == null ? 0 : obj.Name.GetHashCode();
			}
		}
	}
	/// <summary>
	/// Representa um parametro de pesquisa.
	/// </summary>
	public class SearchParameter : Parameter
	{
		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="name">Nome de índice da bisca</param>
		/// <param name="value">Valor do Índice</param>
		public SearchParameter(string name, string value) : base(name, value)
		{
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="name">Nome de índice da bisca</param>
		/// <param name="values">Valor do Índice</param>
		public SearchParameter(string name, string[] values) : base(name, values)
		{
		}
	}
	/// <summary>
	/// Representa um parametro de filtro.
	/// </summary>
	public class FilterParameter : Parameter
	{
		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public FilterParameter(string name, string value) : base(name, value)
		{
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="values">Valores do parametro.</param>
		public FilterParameter(string name, string[] values) : base(name, values)
		{
		}
	}
}
