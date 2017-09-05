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
	/// Objeto padrão de construção de índices
	/// </summary>
	[Export(typeof(IChannelManager))]
	public class ChannelManager : IChannelManager
	{
		/// <summary>
		/// Monta o sumário dos índices do canal
		/// </summary>
		/// <param name="elements">Lista de elementos do canal</param>
		/// <param name="channel">canal</param>
		/// <returns>verdadeiro se criou corretamente o sumário</returns>
		public bool BuildIndexSummary(IEnumerable<Element> elements, Channel channel, IDictionary<string, string> dictionary)
		{
			channel.StringIndex = new SortedDictionary<string, SortedList<int, Element>>();
			channel.ValueIndex = new SortedDictionary<string, SortedDictionary<int, SortedList<int, Element>>>();
			foreach (SchemeIndex si in channel.Scheme.Indexes)
			{
				if(si.IdxType == IndexType.String)
				{
					foreach (Element el in elements)
					{
						string[] words = channel.Scheme.IndexHandle(si.IndexName)(el).GetDistinctWords();
						foreach (string currentWord in words)
						{
							string checkWord = (dictionary.ContainsKey(currentWord ?? "NONONONO") ? dictionary[currentWord] : currentWord).ToUpper();
							string indexSummary = String.Format("{0}::{1}", si.IndexName.ToUpper(), checkWord);
							if(!channel.StringIndex.ContainsKey(indexSummary))
								channel.StringIndex.Add(indexSummary, new SortedList<int, Element>());
							if(!channel.StringIndex[indexSummary].ContainsKey(el.Uid))
								channel.StringIndex[indexSummary].Add(el.Uid, el);
						}
					}
				}
				else if(si.IdxType == IndexType.Value)
				{
					if(si.FieldSchema.Length > 1)
						throw new IndexOutOfRangeException("Índice por valor pode ter apenas um campo");
					string indexSummary = si.IndexName.ToUpper();
					foreach (Element el in elements)
					{
						string valueStr = channel.Scheme.IndexHandle(si.IndexName)(el);
						if(!String.IsNullOrEmpty(valueStr))
							valueStr = valueStr.ToUpper();
						int value = Convert.ToInt32(Convert.ToDouble(dictionary.ContainsKey(valueStr ?? "NONONONO") ? dictionary[valueStr] : valueStr));
						if(!channel.ValueIndex.ContainsKey(indexSummary))
						{
							SortedList<int, Element> sorted = new SortedList<int, Element>();
							sorted.Add(el.Uid, el);
							SortedDictionary<int, SortedList<int, Element>> dic = new SortedDictionary<int, SortedList<int, Element>>();
							dic.Add(value, sorted);
							channel.ValueIndex.Add(indexSummary, dic);
						}
						else
						{
							if(!channel.ValueIndex[indexSummary].ContainsKey(value))
							{
								SortedList<int, Element> sorted = new SortedList<int, Element>();
								sorted.Add(el.Uid, el);
								channel.ValueIndex[indexSummary].Add(value, sorted);
							}
							else
							{
								if(!channel.ValueIndex[indexSummary][value].ContainsKey(el.Uid))
									channel.ValueIndex[indexSummary][value].Add(el.Uid, el);
							}
						}
					}
				}
			}
			return true;
		}
	}
}
