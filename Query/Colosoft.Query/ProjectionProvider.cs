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

namespace Colosoft.Query
{
	/// <summary>
	/// Armazena do resultado de uma pesquisa de projeção.
	/// </summary>
	public class ProjectionSearcherResult : IEnumerable<ProjectionSearcherResult.Entry>
	{
		private List<Entry> _entries;

		/// <summary>
		/// Recupera a entrada pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Entry this[int index]
		{
			get
			{
				return _entries[index];
			}
		}

		/// <summary>
		/// Quantidade de itens no resultado.
		/// </summary>
		public int Count
		{
			get
			{
				return _entries.Count;
			}
		}

		/// <summary>
		/// Cria a instancia com os nomes da colunas para a projeção.
		/// </summary>
		/// <param name="columns"></param>
		public ProjectionSearcherResult(IEnumerable<string> columns)
		{
			columns.Require("columns").NotNull();
			_entries = new List<Entry>(columns.Select(f => new Entry(f)));
		}

		/// <summary>
		/// Cria a instancia com a relação das entradas.
		/// </summary>
		/// <param name="entries"></param>
		public ProjectionSearcherResult(IEnumerable<Entry> entries)
		{
			entries.Require("entries").NotNull();
			_entries = new List<Entry>(entries);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Join("; ", _entries.Select(f => f.ToString()).ToArray());
		}

		/// <summary>
		/// Recupera o enumerador das entradas.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ProjectionSearcherResult.Entry> GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das entradas
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		/// <summary>
		/// Representa uma entrada do resultado.
		/// </summary>
		public class Entry
		{
			private string _columnName;

			private string _alias;

			/// <summary>
			/// Nome da coluna associada.
			/// </summary>
			public string ColumnName
			{
				get
				{
					return _columnName;
				}
			}

			/// <summary>
			/// Apelido da coluna.
			/// </summary>
			public string Alias
			{
				get
				{
					return _alias;
				}
			}

			/// <summary>
			/// Cria uma entrada com o nome da coluna.
			/// </summary>
			/// <param name="columnName"></param>
			public Entry(string columnName) : this(columnName, null)
			{
			}

			/// <summary>
			/// Cria uma entrada com o nome da coluna e seu apelido.
			/// </summary>
			/// <param name="columnName"></param>
			/// <param name="alias"></param>
			public Entry(string columnName, string alias)
			{
				_columnName = columnName;
				_alias = alias;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				if(!string.IsNullOrEmpty(Alias))
					return string.Format("[{0} AS {1}]", ColumnName, Alias);
				return string.Format("[{0}]", ColumnName);
			}
		}
	}
	/// <summary>
	/// Assinatura de pesquisador de projeções.
	/// </summary>
	public interface IProjectionSearcher
	{
		/// <summary>
		/// Localiza as projeções que pode ser aplicadas sobre as informações da entidade informada.
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <returns></returns>
		ProjectionSearcherResult Search(EntityInfo entityInfo);
	}
	/// <summary>
	/// Classe usada para auxialiar na montagem de projeção para 
	/// tipos consultados.
	/// </summary>
	public class ProjectionProvider : IEnumerable<IProjectionSearcher>
	{
		private static object _objLock = new object();

		private static ProjectionProvider _instance;

		private List<IProjectionSearcher> _searchers = new List<IProjectionSearcher>();

		/// <summary>
		/// Instancia do provedor.
		/// </summary>
		public static ProjectionProvider Instance
		{
			get
			{
				if(_instance == null)
					lock (_objLock)
						if(_instance == null)
							_instance = new ProjectionProvider();
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Recupera o pesquisador no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IProjectionSearcher this[int index]
		{
			get
			{
				return _searchers[index];
			}
		}

		/// <summary>
		/// Quantidade de pesquisadores.
		/// </summary>
		public int Count
		{
			get
			{
				return _searchers.Count;
			}
		}

		/// <summary>
		/// Adiciona um pesquisador.
		/// </summary>
		/// <param name="searcher"></param>
		public void Add(IProjectionSearcher searcher)
		{
			_searchers.Add(searcher);
		}

		/// <summary>
		/// Remove o pesquisador.
		/// </summary>
		/// <param name="searcher"></param>
		/// <returns></returns>
		public bool Remove(IProjectionSearcher searcher)
		{
			return _searchers.Remove(searcher);
		}

		/// <summary>
		/// Pesquisa a projeção para as informações da entidade.
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <returns></returns>
		public ProjectionSearcherResult Search(EntityInfo entityInfo)
		{
			foreach (var i in _searchers)
			{
				var result = i.Search(entityInfo);
				if(result != null && result.Count > 0)
					return result;
			}
			return new ProjectionSearcherResult(new string[0]);
		}

		/// <summary>
		/// Recupera o enumerador dos pesquisadores.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IProjectionSearcher> GetEnumerator()
		{
			return _searchers.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos pesquisadores.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _searchers.GetEnumerator();
		}
	}
}
