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
	/// Assinatura das classe que fazem acesso aos dados estatisticos.
	/// </summary>
	public interface IDataStatistics
	{
		/// <summary>
		/// Incrementa os contadores das palavras
		/// </summary>
		/// <param name="word">Palavra</param>
		void IncrementCountWords(string word);

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
		/// <returns>Array com as palavras</returns>
		IEnumerable<WordRank> GetWords();

		/// <summary>
		/// Retorna as palavras mais pesquisadas
		/// </summary>       
		/// <returns>Array com as palavras</returns>
		IEnumerable<IndexRank> GetWordsIndexRank();
	}
	/// <summary>
	/// Assinatura das classes que fazem acesso aos dados estatisticos.
	/// </summary>
	public interface IDataStatistics2
	{
		/// <summary>
		/// Incrementa os contadores das palavras
		/// </summary>
		/// <param name="text">Texto contendo as palavras</param>
		/// <param name="parameters">Parametros usados como filtros.</param>
		void IncrementCountWords(string text, IEnumerable<Parameter> parameters);

		/// <summary>
		/// Recupera as palavras mais pesquisadas 2.
		/// </summary>
		/// <returns></returns>
		IEnumerable<WordRank2> GetWords2();
	}
}
