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

namespace Colosoft.SearchEngine.Lucene
{
	/// <summary>
	/// Implementação a fonte do comparador de campos do tipo FullText.
	/// </summary>
	class FullTextFieldComparatorSource : global::Lucene.Net.Search.FieldComparatorSource
	{
		private IDataRepository _dataRepository;

		private IList<string> _sortTerms;

		private global::Lucene.Net.Analysis.Standard.StandardAnalyzer _analyzer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="dataRepository">Repositorio de dados associado.</param>
		/// <param name="sortTerms">Termos que serão usados para a ordenação.</param>
		public FullTextFieldComparatorSource(global::Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer, IDataRepository dataRepository, IList<string> sortTerms)
		{
			_analyzer = analyzer;
			_dataRepository = dataRepository;
			_sortTerms = sortTerms;
		}

		/// <summary>
		/// Recupera o novo comparador.
		/// </summary>
		/// <param name="fieldname"></param>
		/// <param name="numHits"></param>
		/// <param name="sortPos"></param>
		/// <param name="reversed"></param>
		/// <returns></returns>
		public override global::Lucene.Net.Search.FieldComparator NewComparator(string fieldname, int numHits, int sortPos, bool reversed)
		{
			return new FullTextFieldComparator(_analyzer, _dataRepository, _sortTerms, numHits, fieldname);
		}
	}
	/// <summary>
	/// Implementação do comparador para o campo FullText.
	/// </summary>
	class FullTextFieldComparator : global::Lucene.Net.Search.FieldComparator
	{
		private IDataRepository _dataRepository;

		private global::Lucene.Net.Index.IndexReader _currentReader;

		private IList<string> _sortTerms;

		private List<string>[] _values;

		private string _field;

		private List<string> _bottom;

		/// <summary>
		/// Analizador padrão da instancia.
		/// </summary>
		private global::Lucene.Net.Analysis.Standard.StandardAnalyzer _analyzer;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="analyzer"></param>
		/// <param name="dataRepository"></param>
		/// <param name="sortTerms">Termos que serão usados na ordenação.</param>
		/// <param name="numHits">Total de itens do resultado.</param>
		/// <param name="field">Nome do campos.</param>
		internal FullTextFieldComparator(global::Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer, IDataRepository dataRepository, IList<string> sortTerms, int numHits, string field)
		{
			_analyzer = analyzer;
			_dataRepository = dataRepository;
			_sortTerms = sortTerms;
			_values = new List<string>[numHits];
			_field = field;
		}

		/// <summary>
		/// Compara os valores contidos nos slots informados.
		/// </summary>
		/// <param name="slot1"></param>
		/// <param name="slot2"></param>
		/// <returns></returns>
		public override int Compare(int slot1, int slot2)
		{
			var val1 = _values[slot1];
			var val2 = _values[slot2];
			if(val1 == null)
			{
				if(val2 == null)
					return 0;
				return -1;
			}
			else if(val2 == null)
				return 1;
			var count1 = val1.Intersect(_sortTerms).Count();
			var count2 = val2.Intersect(_sortTerms).Count();
			if(count1 > count2)
				return 1;
			else if(count1 == count2)
			{
				if(count1 > 0)
				{
					for(var i = 0; i < count1; i++)
					{
						var term = _sortTerms[i];
						var index1 = val1.BinarySearch(term);
						var index2 = val2.BinarySearch(term);
						if(index1 >= 0 && index2 < 0)
							return 1;
						else if(index2 >= 0 && index1 < 0)
							return -1;
					}
				}
				return 0;
			}
			else
				return -1;
		}

		/// <summary>
		/// Compara o documento com o item inferior.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public override int CompareBottom(int doc)
		{
			var val2 = ExtractTerms(_currentReader.Document(doc).Get(_field));
			if(_bottom == null)
			{
				if(val2 == null)
					return 0;
				return -1;
			}
			else if(val2 == null)
				return 1;
			var count1 = _bottom.Intersect(_sortTerms).Count();
			var count2 = val2.Intersect(_sortTerms).Count();
			if(count1 > count2)
				return 1;
			else if(count1 == count2)
			{
				for(var i = 0; i < count1; i++)
				{
					var term = _sortTerms[i];
					var index1 = _bottom.BinarySearch(term);
					var index2 = val2.BinarySearch(term);
					if(index1 >= 0 && index2 < 0)
						return 1;
					else if(index2 >= 0 && index1 < 0)
						return -1;
				}
				return 0;
			}
			else
				return -1;
		}

		/// <summary>
		/// Copia o valor do documento para o slot informado.
		/// </summary>
		/// <param name="slot">Slot para onde o valor será copiado.</param>
		/// <param name="doc">Número do documento.</param>
		public override void Copy(int slot, int doc)
		{
			var value = _currentReader.Document(doc).Get(_field);
			_values[slot] = ExtractTerms(value);
		}

		/// <summary>
		/// Extrai os termos do valor informado.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private List<string> ExtractTerms(string value)
		{
			var terms = new List<string>();
			using (var reader = new System.IO.StringReader(value))
			{
				var tokenStream = _analyzer.TokenStream(_field, reader);
				var token = tokenStream.Next();
				while (token != null)
				{
					var term = token.TermText();
					terms.Add(term);
					token = tokenStream.Next();
				}
			}
			return terms;
		}

		/// <summary>
		/// Define o próximo leitor.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="docBase"></param>
		public override void SetNextReader(global::Lucene.Net.Index.IndexReader reader, int docBase)
		{
			_currentReader = reader;
		}

		/// <summary>
		/// Define o item inferior.
		/// </summary>
		/// <param name="bottom"></param>
		public override void SetBottom(int bottom)
		{
			_bottom = _values[bottom];
		}

		/// <summary>
		/// Recupera o valor no slot informado.
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		public override System.IComparable Value(int slot)
		{
			return new FullTextComparable(this, slot);
		}

		class FullTextComparable : IComparable
		{
			private FullTextFieldComparator _comparator;

			private int _slot;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="comparator"></param>
			public FullTextComparable(FullTextFieldComparator comparator, int slot)
			{
				_comparator = comparator;
				_slot = slot;
			}

			public int CompareTo(object obj)
			{
				return string.Join(" ", _comparator._values[_slot].ToArray()).CompareTo(obj);
			}
		}
	}
}
