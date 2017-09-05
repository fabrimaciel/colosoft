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

namespace Colosoft.SearchEngine.Default
{
	/// <summary>
	/// Objeto responsável por gerenciar buscas e indexações
	/// </summary>
	[Export(typeof(ISearcher))]
	public class Searcher : ISearcher
	{
		private bool _initialized = false;

		private readonly IStructRepository _structRepository;

		private readonly IDataRepository _dataRepository;

		private readonly IChannelManager _channelManager;

		private readonly ISearchManager _searchManager;

		private readonly ISummaryManager _summaryManager;

		private readonly ISearchStatistics _statisticsManager;

		private IDictionary<string, string> _dataRespositoryDictionary;

		/// <summary>
		/// Construtor com a inicialização de objetos que serão utilizados
		/// </summary>
		/// <param name="structRepository">Repositor de estrutura</param>
		/// <param name="dataRespository">Repositor de dados</param>
		/// <param name="channelManager">Gerenciador de canais</param>
		/// <param name="searchManager">Gerenciador de busca</param>
		/// <param name="summaryManager">Gerenciador de sumarização</param>,
		/// <param name="statisticsManager">Gerencia os contadores do site</param>
		[ImportingConstructor]
		public Searcher(IStructRepository structRepository, IDataRepository dataRespository, IChannelManager channelManager, ISearchManager searchManager, ISummaryManager summaryManager, ISearchStatistics statisticsManager)
		{
			_structRepository = structRepository;
			_dataRepository = dataRespository;
			_channelManager = channelManager;
			_searchManager = searchManager;
			_summaryManager = summaryManager;
			_statisticsManager = statisticsManager;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		public void Initialize()
		{
			if(!_initialized)
			{
				_dataRepository.Initialize();
				_structRepository.Initialize();
				if(_dataRespositoryDictionary == null)
					_dataRespositoryDictionary = _dataRepository.LoadDictionary();
				var channels = _structRepository.GetChannels();
				var elements = _dataRepository.GetElements();
				foreach (var currentChannel in channels)
				{
					_channelManager.BuildIndexSummary(elements.Where(n => n.ChannelId == currentChannel.ChannelId), currentChannel, _dataRespositoryDictionary);
				}
			}
		}

		#if ProcessamentoParalelo
		        /// <summary>
        /// Realiza a bus
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="searchParans"></param>
        /// <param name="buildSummary"></param>
        /// <returns></returns>
        public SearchResult Search(byte channelId, SearchParam[] searchParans, bool buildSummary)
        {
            SearchResult result = null;
            System.Threading.Tasks.Parallel.Invoke(
            delegate { IncStatistics(searchParans); },
            delegate { BuildSearch(channelId, searchParans, buildSummary, ref result); });
            return result;
        }

        private void IncStatistics(SearchParam[] searchParans)
        {
            StringBuilder words = new StringBuilder();
            foreach (SearchParam ap in searchParans)
            {
                words.Append(" ");
                words.Append(ap.Value);
            }
            _statisticsManager.IncCountWords(words.ToString());
        }

        private void BuildSearch(byte channelId, SearchParam[] searchParans, bool buildSummary, ref SearchResult result)
        {
            if (_allChannels.ContainsKey(channelId))
            {
                result = new SearchResult();
                result.Scheme = _allChannels[channelId].Scheme;
                result.Elements = _searchManager.SearchInChannel(_allChannels[channelId],
                    _allElements.ToArray(), searchParans);
                if (buildSummary)
                {
                    result.Summary = _summaryManager.BuildSummary(result.Elements,
                        _allChannels[channelId]);
                }
            }
            else
                throw new ArgumentException("channlId");
        }

        private void BuildSearch(byte channelId, string text, bool buildSummary, ref SearchResult result)
        {
            if (_allChannels.ContainsKey(channelId))
            {
                result = new SearchResult();
                result.Scheme = _allChannels[channelId].Scheme;
                result.Elements = _searchManager.SearchInChannel(_allChannels[channelId],
                    _allElements.ToArray(), text);
                if (buildSummary)
                {
                    result.Summary = _summaryManager.BuildSummary(result.Elements,
                        _allChannels[channelId]);
                }
            }
            else
                throw new ArgumentException("channlId");
        }
        
        /// <summary>
        /// Realiza a bus
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="searchParans"></param>
        /// <param name="buildSummary"></param>
        /// <returns></returns>
        public SearchResult Search(byte channelId, string text, bool buildSummary)
        {
            SearchResult result = null;
            System.Threading.Tasks.Parallel.Invoke(
            delegate { _statisticsManager.IncCountWords(text); },
            delegate { BuildSearch(channelId, text, buildSummary, ref result); });
            return result;
        }
#else
		/// <summary>
		/// Realiza a bus
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="searchParans"></param>
		/// <param name="buildSummary"></param>
		/// <returns></returns>
		public ISearchResult Search(byte channelId, SearchParameter[] searchParameters, bool buildSummary)
		{
			StringBuilder words = new StringBuilder();
			foreach (SearchParameter ap in searchParameters)
			{
				words.Append(string.Join(" ", ap.Values)).Append(" ");
			}
			_statisticsManager.IncrementCountWords(words.ToString());
			var channel = _structRepository.GetChannel(channelId);
			if(channel != null)
			{
				string searchString = string.Empty;
				var result = new SearchResult();
				result.Scheme = channel.Scheme;
				result.Elements = _searchManager.SearchInChannel(channel, _dataRepository.GetElements(), searchParameters, _dataRespositoryDictionary, ref searchString);
				if(buildSummary)
					result.Summary = _summaryManager.BuildSummary(result.Elements, channel, searchString);
				return result;
			}
			else
				throw new ArgumentException("channelId");
		}

		/// <summary>
		/// Realiza a bus
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="searchParans"></param>
		/// <param name="buildSummary"></param>
		/// <returns></returns>
		public ISearchResult Search(byte channelId, string text, bool buildSummary)
		{
			var channel = _structRepository.GetChannel(channelId);
			if(channel != null)
			{
				string searchString = String.Empty;
				SearchResult result = new SearchResult();
				result.Scheme = channel.Scheme;
				result.Elements = _searchManager.SearchInChannel(channel, _dataRepository.GetElements(), text, _dataRespositoryDictionary, ref searchString);
				if(buildSummary)
				{
					result.Summary = _summaryManager.BuildSummary(result.Elements, channel, searchString);
				}
				return result;
			}
			else
				throw new ArgumentException("channelId");
		}

		#endif
		/// <summary>
		/// Retorna as palavras mais pesquisadas
		/// </summary>
		/// <param name="quantity">quantidade de palavras</param>
		/// <returns>Array com as palavras</returns>
		public IEnumerable<WordRank> GetWords(int quantity)
		{
			return _statisticsManager.GetWords(quantity);
		}
	}
}
