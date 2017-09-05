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

namespace Colosoft.Collections
{
	/// <summary>
	/// Armazena os dados da descrição do parametro de pesquisa.
	/// </summary>
	public class SearchParamerterDescription : ICloneable
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string ParameterName
		{
			get;
			set;
		}

		/// <summary>
		/// Descrição do parametro.
		/// </summary>
		public virtual IMessageFormattable Description
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		/// <param name="parameterName"></param>
		protected SearchParamerterDescription(string parameterName)
		{
			this.ParameterName = parameterName;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameterName">Nome do parametro.</param>
		/// <param name="description">Descrição do parametro.</param>
		public SearchParamerterDescription(string parameterName, IMessageFormattable description)
		{
			this.ParameterName = parameterName;
			this.Description = description;
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			var description = Description is ICloneable ? (IMessageFormattable)((ICloneable)Description).Clone() : Description;
			return new SearchParamerterDescription(ParameterName, description);
		}
	}
	/// <summary>
	/// Reprenseta o parametro de pesquisa com carga tardia.
	/// </summary>
	public class SearchParameterDescriptionLazy : SearchParamerterDescription
	{
		private Lazy<IMessageFormattable> _description;

		/// <summary>
		/// Descrição.
		/// </summary>
		public override IMessageFormattable Description
		{
			get
			{
				if(_description != null)
					return _description.Value;
				return null;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="description"></param>
		public SearchParameterDescriptionLazy(string parameterName, Lazy<IMessageFormattable> description) : base(parameterName)
		{
			_description = description;
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return base.Clone();
		}
	}
	/// <summary>
	/// Assinatura de um container com a descrição dos parametros de pesquisa.
	/// </summary>
	public interface ISearchParameterDescriptionContainer
	{
		/// <summary>
		/// Relação das descrições dos parametros de pesquisa.
		/// </summary>
		SearchParameterDescriptionCollection SearchParameterDescriptions
		{
			get;
		}
	}
	/// <summary>
	/// Assinatura de um container com a descrição dos parametros de pesquisa.
	/// </summary>
	public class SearchParameterDescriptionCollection : IEnumerable<SearchParamerterDescription>, ICloneable
	{
		private List<SearchParamerterDescription> _parameters = new List<SearchParamerterDescription>();

		/// <summary>
		/// Quantidade de parametros do container.
		/// </summary>
		public int Count
		{
			get
			{
				return _parameters.Count;
			}
		}

		/// <summary>
		/// Recupera e define o parametro no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SearchParamerterDescription this[int index]
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
		/// Adiciona uma descrição para a coleção.
		/// </summary>
		/// <param name="parameterName">Nome do parametro.</param>
		/// <param name="description">Descrição do parametro.</param>
		public void Add(string parameterName, IMessageFormattable description)
		{
			_parameters.Add(new SearchParamerterDescription(parameterName, description));
		}

		/// <summary>
		/// Adiciona uma descrição para a coleção.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="description"></param>
		public void Add(string parameterName, Lazy<IMessageFormattable> description)
		{
			_parameters.Add(new SearchParameterDescriptionLazy(parameterName, description));
		}

		/// <summary>
		/// Adiciona a descrição do parametro.
		/// </summary>
		/// <param name="description"></param>
		public void Add(SearchParamerterDescription description)
		{
			description.Require("description").NotNull();
			_parameters.Add(description);
		}

		/// <summary>
		/// Recupera a descrição do parametro.
		/// </summary>
		/// <param name="description"></param>
		/// <returns></returns>
		public bool Remove(SearchParamerterDescription description)
		{
			return _parameters.Remove(description);
		}

		/// <summary>
		/// Remove o parametro na posição informada.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			_parameters.RemoveAt(index);
		}

		/// <summary>
		/// Limpa o container.
		/// </summary>
		public void Clear()
		{
			_parameters.Clear();
		}

		/// <summary>
		/// Recupera o enumerador dos parametros.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<SearchParamerterDescription> GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos parametros.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var collection = new SearchParameterDescriptionCollection();
			foreach (var i in this)
				collection.Add((SearchParamerterDescription)i.Clone());
			return collection;
		}
	}
}
