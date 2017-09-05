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
using System.ComponentModel.Composition;

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Implementação das estatisticas de pesquisa.
	/// </summary>
	[Export(typeof(ISearchStatistics))]
	public class SearchStatistics : ISearchStatistics, ISearchStatistics2, IDisposable
	{
		private readonly object lockObject = new object();

		private readonly IDataStatistics _dataStatistics;

		private List<WordRank> _wordRanks;

		private List<IndexRank> _wordIndexRanks;

		private List<WordRank2> _wordRanks2;

		private List<string> _updateWords;

		private List<string> _updateWordsIndexRanks;

		private List<WordRank2> _updateWordRanks2;

		private bool _isRun = false;

		private System.Threading.Thread _statisticsThread;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataStatistics"></param>
		[ImportingConstructor]
		public SearchStatistics(IDataStatistics dataStatistics)
		{
			_dataStatistics = dataStatistics;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		public void Initialize()
		{
			_wordRanks = new List<WordRank>();
			_updateWords = new List<string>();
			_updateWordsIndexRanks = new List<string>();
			_updateWordRanks2 = new List<WordRank2>();
			_wordRanks = _dataStatistics.GetWords().ToList();
			_wordIndexRanks = _dataStatistics.GetWordsIndexRank().ToList();
			if(_dataStatistics is IDataStatistics2)
				_wordRanks2 = ((IDataStatistics2)_dataStatistics).GetWords2().ToList();
			else
				_wordRanks2 = new List<WordRank2>();
			StartRobot();
		}

		/// <summary>
		/// Incrementa a quantidade de palavra.
		/// </summary>
		/// <param name="text"></param>
		public void IncrementCountWords(string text)
		{
			string[] words = text.ToUpper().GetDistinctWords();
			foreach (string word in words)
			{
				var work = _wordRanks.Where(f => f.Word == text).FirstOrDefault();
				if(work != null)
					work.Count++;
				else
					_wordRanks.Add(new WordRank {
						Count = 1,
						Word = word
					});
				_updateWords.Add(word);
			}
		}

		/// <summary>
		/// Incrementa a quantidade de palavras.
		/// </summary>
		/// <param name="schemeIndexId"></param>
		/// <param name="channel"></param>
		/// <param name="text"></param>
		public void IncrementCountWords(int schemeIndexId, Channel channel, string text)
		{
			string[] words = text.ToUpper().GetDistinctWords();
			foreach (string word in words)
			{
				var work = _wordIndexRanks.Where(f => f.Word == text).FirstOrDefault();
				if(work != null)
				{
					work.Count++;
				}
				else
					_wordIndexRanks.Add(new IndexRank {
						Count = 1,
						Word = text,
						ChannelId = channel.ChannelId,
						SchemeindexId = schemeIndexId
					});
				_dataStatistics.IncrementCountWords(schemeIndexId, channel, text);
			}
		}

		/// <summary>
		/// Recupera o enumerador para a quantidade de palavras informadas.
		/// </summary>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public IEnumerable<WordRank> GetWords(int quantity)
		{
			return _wordRanks.Take(quantity);
		}

		/// <summary>
		/// Recupera o rank das palavras indexadas.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IndexRank> GetWordsIndexRank()
		{
			return _wordIndexRanks;
		}

		/// <summary>
		/// Inicia o robo.
		/// </summary>
		private void StartRobot()
		{
			if(_statisticsThread == null)
			{
				lock (lockObject)
				{
					_statisticsThread = new System.Threading.Thread(this.Robot);
					_statisticsThread.Start();
				}
			}
		}

		/// <summary>
		/// Método do robo que monta as estatísticas da busca.
		/// </summary>
		private void Robot()
		{
			try
			{
				_isRun = true;
				List<string> errorWords = null;
				while (_isRun)
				{
					try
					{
						errorWords = new List<string>();
						if(_updateWords.Count > 0)
						{
							while (_updateWords.Count > 0)
							{
								var i = _updateWords[0];
								try
								{
									_dataStatistics.IncrementCountWords(i);
								}
								catch
								{
									errorWords.Add(i);
								}
								_updateWords.RemoveAt(0);
							}
							if(errorWords.Count > 0)
								_updateWords = errorWords;
						}
						if(_updateWordRanks2.Count > 0)
						{
							while (_updateWordRanks2.Count > 0)
							{
								var i = _updateWordRanks2[0];
								if(_dataStatistics is IDataStatistics2)
									try
									{
										((IDataStatistics2)_dataStatistics).IncrementCountWords(i.Word, i.Parameters);
									}
									catch
									{
									}
								_updateWordRanks2.RemoveAt(0);
							}
						}
					}
					catch(Exception ex)
					{
						System.Diagnostics.Trace.WriteLine("Falha ao incrementar os contadores das palavras Ex: " + ex.Message);
					}
					System.Threading.Thread.Sleep(30000);
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			try
			{
				_isRun = false;
				_statisticsThread.Abort();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Incrementa os contadores das palavras
		/// </summary>
		/// <param name="text">Texto contendo as palavras</param>
		/// <param name="parameters">Parametros usados como filtros.</param>
		public void IncrementCountWords(string text, IEnumerable<Parameter> parameters)
		{
			var parameters2 = parameters.ToArray();
			string[] words = text.ToUpper().GetDistinctWords();
			foreach (string word in words)
			{
				var wordRank = _wordRanks2.Where(f => f.Word == text && f.CompareParameters(parameters)).FirstOrDefault();
				if(wordRank != null)
					wordRank.Count++;
				else
				{
					wordRank = new WordRank2(word, 1, 0, parameters2);
					_wordRanks2.Add(wordRank);
				}
				_updateWordRanks2.Add((WordRank2)wordRank.Clone());
			}
		}
	}
}
