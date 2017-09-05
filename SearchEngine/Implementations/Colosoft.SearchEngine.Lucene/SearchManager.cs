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

namespace TecMS.SearchEngine.Lucene
{
	/// <summary>
	/// Classe padrão de gerenciamento de buscas
	/// </summary>
	[Export(typeof(ISearchManager))]
	public class SearchManager : ISearchManager
	{
		private readonly Dictionary<string, List<Element>> _cache;

		/// <summary>
		/// Serializa os parametros.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private string SerializeSearch(SearchParameter[] parameters)
		{
			return string.Join("", parameters.Distinct(new SearchParameter.NameEqualityComparer()).Select(f => f.ToString()).ToArray());
		}

		/// <summary>
		/// Constrói a pesquisa.
		/// </summary>
		/// <param name="channel">Instancia do canal onde a pesquisa será realizada.</param>
		/// <param name="elements"></param>
		/// <param name="searchParameters">Parametros que serão usados na pesquisa.</param>
		/// <param name="forceIntersect"></param>
		/// <param name="dictionary"></param>
		/// <param name="searchString"></param>
		/// <returns></returns>
		private List<Element> BuildSearch(Channel channel, IEnumerable<Element> elements, SearchParameter[] searchParameters, bool forceIntersect, IDictionary<string, string> dictionary, ref string searchString)
		{
			foreach (var sp in searchParameters)
			{
				for(int i = 0; i < sp.Values.Length; i++)
				{
					if(dictionary.ContainsKey(sp.Values[i].ToUpper()))
						sp.Values[i] = dictionary[sp.Values[i]];
				}
			}
			searchString = SerializeSearch(searchParameters);
			if(_cache.ContainsKey(searchString))
				return _cache[searchString];
			else
			{
				SortIndex(searchParameters, channel);
				var tempList = searchParameters.ToList();
				if(!forceIntersect)
				{
					for(int i = 0; i < tempList.Count; i++)
					{
						List<SearchParameter> equals = searchParameters.Where(f => f.Value.Equals(tempList[i].Value)).ToList();
						if(equals.Count > 1)
						{
							for(int j = 1; j < equals.Count; j++)
							{
								foreach (Element el in equals[j].Elements.Values)
								{
									if(!tempList[i].Elements.ContainsKey(el.Uid))
										tempList[i].Elements.Add(el.Uid, el);
								}
								tempList.Remove(equals[j]);
							}
						}
					}
					searchParameters = tempList.ToArray();
					SortIndex(searchParameters, channel);
				}
				SortedList<int, Element> auxList = searchParameters[0].Elements;
				List<Element> myList = auxList.Values.ToList();
				if((searchParameters.Length > 1) && (myList != null))
				{
					bool isOk;
					List<Element> result = new List<Element>();
					foreach (Element el in myList)
					{
						isOk = true;
						for(int i = 1; i < searchParameters.Length; i++)
						{
							if(!searchParameters[i].Elements.ContainsKey(el.Uid))
							{
								isOk = false;
								break;
							}
						}
						if(isOk)
						{
							result.Add(el);
						}
					}
					lock (_cache)
					{
						if(!_cache.ContainsKey(searchString))
							_cache.Add(searchString, result);
					}
					return result;
				}
				else
				{
					lock (_cache)
					{
						if(!_cache.ContainsKey(searchString))
							_cache.Add(searchString, myList);
					}
					return myList;
				}
			}
		}

		private void SortIndex(SearchParameter[] parameters, Channel channel)
		{
			for(int i = 0; i < parameters.Length; i++)
			{
				parameters[i].Elements = null;
			}
			for(int i = 0; i < parameters.Length; i++)
			{
				PutElementsInParams(ref parameters[i], channel);
				for(int j = i + 1; j < parameters.Length; j++)
				{
					PutElementsInParams(ref parameters[j], channel);
					if((parameters[i].Elements != null ? parameters[i].Elements.Count : Int32.MaxValue) > ((parameters[j].Elements != null ? parameters[j].Elements.Count : Int32.MaxValue)))
					{
						SearchParameter temp = parameters[j];
						parameters[j] = new SearchParameter(parameters[i].Name, parameters[i].Values, parameters[i].Elements);
						parameters[i] = new SearchParameter(temp.Name, temp.Values, temp.Elements);
					}
				}
			}
		}

		private void PutElementsInParams(ref SearchParameter parameters, Channel channel)
		{
			string baseKeyName;
			if(parameters.Elements == null)
			{
				parameters.Elements = new SortedList<int, Element>();
				if(parameters.SearchType == SearchParameterType.Multiple)
				{
					foreach (string value in parameters.Values)
					{
						if(channel.GetSchemaIndex(parameters.Name.ToUpper()).Type == IndexType.String)
						{
							baseKeyName = String.Format("{0}::{1}", parameters.Name.ToUpper(), value.ToUpper());
							foreach (KeyValuePair<int, Element> kp in channel.StringIndex[baseKeyName])
							{
								if(!parameters.Elements.ContainsKey(kp.Key))
									parameters.Elements.Add(kp.Key, kp.Value);
							}
						}
						else
						{
							baseKeyName = String.Format("{0}", parameters.Name.ToUpper());
							int startValue = Convert.ToInt32(parameters.Values[0]);
							int stopValue = Convert.ToInt32(parameters.Values[1]);
							if(stopValue < startValue)
							{
								int aux = stopValue;
								stopValue = startValue;
								startValue = aux;
							}
							var result = channel.ValueIndex[baseKeyName].Where(f => ((f.Key >= startValue) && (f.Key <= stopValue)));
							foreach (KeyValuePair<int, SortedList<int, Element>> kp in result)
							{
								foreach (Element el in kp.Value.Values)
								{
									if(!parameters.Elements.ContainsKey(el.Uid))
										parameters.Elements.Add(el.Uid, el);
								}
							}
						}
					}
				}
				else
				{
					if(channel.GetSchemaIndex(parameters.Name.ToUpper()).Type == IndexType.String)
					{
						baseKeyName = String.Format("{0}::{1}", parameters.Name.ToUpper(), parameters.Value.ToUpper());
						SortedList<int, Element> elements = null;
						if(!channel.StringIndex.TryGetValue(baseKeyName, out elements))
							elements = new SortedList<int, Element>();
						parameters.Elements = elements;
					}
					else
					{
						baseKeyName = String.Format("{0}", parameters.Name.ToUpper());
						int startValue = Convert.ToInt32(parameters.Values[0]);
						foreach (Element el in channel.ValueIndex[baseKeyName][startValue].Values)
						{
							if(!parameters.Elements.ContainsKey(el.Uid))
								parameters.Elements.Add(el.Uid, el);
						}
					}
				}
			}
		}

		private SearchParameter[] ParametersCalculate(Channel channel, string text, IDictionary<string, string> dictionary, ref List<string> unfoundWords)
		{
			unfoundWords = new List<string>();
			List<SearchParameter> parans = new List<SearchParameter>();
			string[] allStrings = text.GetDistinctWords();
			foreach (string str in allStrings)
			{
				string strConv = (dictionary.ContainsKey(str.ToUpper()) ? dictionary[str.ToUpper()] : str).ToUpper();
				string[] results = null;
				if(IsNumericString(strConv))
				{
					int value = Convert.ToInt32(strConv);
					results = channel.StringIndex.Keys.Where(f => f.EndsWith(string.Format("::{0}", strConv.ToUpper()))).Union(channel.ValueIndex.Keys.Where(f => channel.ValueIndex[f].ContainsKey(value))).ToArray();
				}
				else
				{
					results = channel.StringIndex.Keys.Where(f => f.EndsWith(string.Format("::{0}", strConv.ToUpper()))).ToArray();
				}
				if(results.Length >= 1)
				{
					foreach (string key in results)
					{
						if(key.Contains("::"))
						{
							parans.Add(new SearchParameter(key.Substring(0, key.IndexOf("::")), key.Substring(key.IndexOf("::") + 2)));
						}
						else
						{
							parans.Add(new SearchParameter(key, strConv));
						}
					}
				}
				else
					unfoundWords.Add(strConv);
			}
			return parans.ToArray();
		}

		private bool IsNumericString(string str)
		{
			for(int index = 0; index < str.Length; index++)
			{
				if(!char.IsDigit(str, index))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SearchManager()
		{
			_cache = new Dictionary<string, List<Element>>();
		}

		public IEnumerable<Element> Search(IEnumerable<SearchParameter> parameters)
		{
			return null;
		}

		/// <summary>
		/// Busca os elementos em seu respectivo canal
		/// </summary>
		/// <param name="channel">Canal</param>
		/// <param name="elements">lista de elementos</param>
		/// <param name="paransArray">parâmetros de busca</param>
		/// <returns>Array de elementos que atendem à busca</returns>
		public Element[] SearchInChannel(Channel channel, IEnumerable<Element> elements, SearchParameter[] parameters, IDictionary<string, string> dictionary, ref string searchString)
		{
			return BuildSearch(channel, elements, parameters, true, dictionary, ref searchString).ToArray();
		}

		/// <summary>
		/// Busca os elementos em seu respectivo canal recebendo texto bruto
		/// </summary>
		/// <param name="channel">canal</param>
		/// <param name="elements">lista de elementos</param>
		/// <param name="text">texto pesquisado</param>
		/// <returns>Array de elementos que atendem à busca</returns>
		public Element[] SearchInChannel(Channel channel, IEnumerable<Element> elements, string text, IDictionary<string, string> dictionary, ref string searchString)
		{
			List<string> unfoundWords = new List<string>();
			SearchParameter[] parans = ParametersCalculate(channel, text, dictionary, ref unfoundWords);
			return BuildSearch(channel, elements, parans, false, dictionary, ref searchString).ToArray();
		}
	}
}
