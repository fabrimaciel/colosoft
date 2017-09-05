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
	/// Se implemetada gera uma classe que monta as estatísticas da busca
	/// </summary>
	public interface ISearchStatistics
	{
		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Incrementa os contadores das palavras
		/// </summary>
		/// <param name="text">Texto contendo as palavras</param>
		void IncrementCountWords(string text);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schemeFieldId">Campo do texto</param>
		/// <param name="channel">Canal onde foi feita a pesquisa</param>
		/// <param name="text">Texto contendo as palavras</param>
		void IncrementCountWords(int schemeFieldId, Channel channel, string text);

		/// <summary>
		/// Retorna as palavras mais pesquisadas
		/// </summary>
		/// <param name="quantity">quantidade de palavras</param>
		/// <returns>Array com as palavras</returns>
		IEnumerable<WordRank> GetWords(int quantity);

		/// <summary>
		/// Retorna as palavras mais pesquisadas
		/// </summary>
		/// <returns>Array com as palavras</returns>
		IEnumerable<IndexRank> GetWordsIndexRank();
	}
	/// <summary>
	/// Assinatura da classe que gerencia as estatisticas de pesquisa.
	/// </summary>
	public interface ISearchStatistics2
	{
		/// <summary>
		/// Incrementa os contadores das palavras
		/// </summary>
		/// <param name="text">Texto contendo as palavras</param>
		/// <param name="parameters">Parametros usados como filtros.</param>
		void IncrementCountWords(string text, IEnumerable<Parameter> parameters);
	}
}
