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

namespace Colosoft.Reports
{
	/// <summary>
	/// Representa a coleção de parametros do DataSource.
	/// </summary>
	public class ReportDataSourceParameterCollection : IEnumerable<ReportDataSourceParameter>
	{
		private List<ReportDataSourceParameter> _parameters = new List<ReportDataSourceParameter>();

		/// <summary>
		/// Comparador padrão dos nomes do parametro.
		/// </summary>
		private static readonly IEqualityComparer<string> _defaultParameterNameComparer = StringComparer.InvariantCultureIgnoreCase;

		private IEqualityComparer<string> _parameterNameEqualityComparer;

		/// <summary>
		/// Comparador de igualdade dos nomes dos parametros da coleção.
		/// </summary>
		public IEqualityComparer<string> ParameterNameEqualityComparer
		{
			get
			{
				return _parameterNameEqualityComparer ?? _defaultParameterNameComparer;
			}
			set
			{
				_parameterNameEqualityComparer = value;
			}
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Quantidade de parametros na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _parameters.Count;
			}
		}

		/// <summary>
		/// Recupera e define o valor do parametro pela chave informada.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object this[string name]
		{
			get
			{
				return _parameters.Where(f => ParameterNameEqualityComparer.Equals(f.Name, name)).Select(f => f.Value).FirstOrDefault();
			}
			set
			{
				name.Require("name").NotNull();
				var parameter = _parameters.Where(f => ParameterNameEqualityComparer.Equals(f.Name, name)).FirstOrDefault();
				if(parameter == null)
					Add(name, value);
				else
					parameter.Value = value;
			}
		}

		/// <summary>
		/// Recupera o parametro pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ReportDataSourceParameter this[int index]
		{
			get
			{
				return _parameters[index];
			}
			set
			{
				_parameters[index] = value;
			}
		}

		/// <summary>
		/// Preenche a coleção com os parametros informados.
		/// </summary>
		/// <param name="parameters"></param>
		public void Fill(System.Collections.Specialized.NameValueCollection parameters)
		{
			if(parameters == null)
				return;
			for(var i = 0; i < parameters.Count; i++)
				this.Add(parameters.Keys[i], parameters.GetValues(i).FirstOrDefault());
		}

		/// <summary>
		/// Adiciona um novo parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public void Add(string name, object value)
		{
			name.Require("name").NotNull();
			Add(new ReportDataSourceParameter(name, value));
		}

		/// <summary>
		/// Adiciona o parametro para a coleção.
		/// </summary>
		/// <param name="parameter"></param>
		public void Add(ReportDataSourceParameter parameter)
		{
			if(_parameters.Any(f => ParameterNameEqualityComparer.Equals(f.Name, parameter.Name)))
				throw new ArgumentException("An element with the same key already exists");
			_parameters.Add(parameter);
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		public void Clear()
		{
			_parameters.Clear();
		}

		/// <summary>
		/// Verifica se existe alguma parametro com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			return _parameters.Any(f => ParameterNameEqualityComparer.Equals(f.Name, name));
		}

		/// <summary>
		/// Remove o parametro associado ao nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			var index = _parameters.FindIndex(f => ParameterNameEqualityComparer.Equals(f.Name, name));
			if(index >= 0)
				_parameters.RemoveAt(index);
		}

		/// <summary>
		/// Recupera o Enumerator dos parametros.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		/// <summary>
		/// Recupera o Enumerator dos parametros.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ReportDataSourceParameter> GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}
	}
	/// <summary>
	/// Representa o parametro do DataSource.
	/// </summary>
	public class ReportDataSourceParameter
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		public object Value
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public ReportDataSourceParameter(string name, object value)
		{
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Name: {0}; Value: {1}", Name, Value);
		}
	}
	/// <summary>
	/// Implementação do comparadopr dos parametros do DataSource.
	/// </summary>
	public class ReportDataSourceParameterComparer : IComparer<ReportDataSourceParameter>, IEqualityComparer<ReportDataSourceParameter>
	{
		/// <summary>
		/// Compara a duas instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(ReportDataSourceParameter x, ReportDataSourceParameter y)
		{
			if(x == null && y == null)
				return 0;
			else if(x == null && y != null)
				return -1;
			else if(x != null && y == null)
				return 1;
			return string.Compare(x.Name, y.Name);
		}

		/// <summary>
		/// Verifica se as duas instancias são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(ReportDataSourceParameter x, ReportDataSourceParameter y)
		{
			return ((x == null && y == null) || (x != null && y != null && string.Equals(x.Name, y.Name)));
		}

		/// <summary>
		/// Recupera o HashCode da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(ReportDataSourceParameter obj)
		{
			return obj == null || obj.Name == null ? 0 : obj.Name.GetHashCode();
		}
	}
}
