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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Representa uma palavra de ranqueamento.
	/// </summary>
	public class WordRank : ICloneable
	{
		/// <summary>
		/// Palavra da instancia.
		/// </summary>
		public string Word
		{
			get;
			set;
		}

		/// <summary>
		/// Peso da palavra.
		/// </summary>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// Peso da palavra.
		/// </summary>
		public int ChannelId
		{
			get;
			set;
		}

		/// <summary>
		/// Clona a instancia.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			return new WordRank {
				Word = Word,
				Count = Count,
				ChannelId = ChannelId
			};
		}
	}
	/// <summary>
	/// Implementa��o do rank de palavras com os filtros informados.
	/// </summary>
	public class WordRank2 : WordRank
	{
		/// <summary>
		/// Construtor padr�o.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="count"></param>
		/// <param name="channelId"></param>
		/// <param name="parameters"></param>
		public WordRank2(string word, int count, int channelId, System.Collections.Generic.IEnumerable<Parameter> parameters)
		{
			Word = word;
			Count = count;
			ChannelId = channelId;
			if(parameters != null)
				Parameters = parameters.OrderBy(f => f.Name).ToArray();
			else
				Parameters = new Parameter[0];
		}

		/// <summary>
		/// Parametros associados.
		/// </summary>
		public Parameter[] Parameters
		{
			get;
			set;
		}

		/// <summary>
		/// Compara os parametros informados com os parametros da instancia.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public bool CompareParameters(System.Collections.Generic.IEnumerable<Parameter> parameters)
		{
			if(parameters == null && (Parameters == null || Parameters.Length == 0))
				return true;
			if(parameters == null)
				return false;
			foreach (var i in parameters)
			{
				var index = Array.BinarySearch(this.Parameters, i, ParameterNameComparer.Instance);
				if(index < 0)
					return false;
				var p = Parameters[index];
				if(System.Collections.Comparer.Default.Compare(p.Value, i.Value) < 0)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new WordRank2(Word, Count, ChannelId, Parameters);
		}

		class ParameterNameComparer : IComparer<Parameter>
		{
			/// <summary>
			/// Instancia unica do tipo.
			/// </summary>
			public readonly static ParameterNameComparer Instance = new ParameterNameComparer();

			/// <summary>
			/// Compara os dois parametros informados.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(Parameter x, Parameter y)
			{
				if(x != null && y != null)
					return string.Compare(x.Name, y.Name);
				return -1;
			}
		}
	}
}
