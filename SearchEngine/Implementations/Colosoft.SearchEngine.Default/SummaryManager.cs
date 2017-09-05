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
	/// Classe padrão para a montagem de sumário
	/// </summary>
	[Export(typeof(ISummaryManager))]
	public class SummaryManager : ISummaryManager
	{
		private Dictionary<string, SummaryResult[]> _cache;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SummaryManager()
		{
			_cache = new Dictionary<string, SummaryResult[]>();
		}

		/// <summary>
		/// Copnstroi o sumário da busca realizada
		/// </summary>
		/// <param name="summaryList">Lista de elementos retornados pela busca</param>
		/// <param name="channel">Canal</param>
		/// <returns>Lista se resultado de sumário</returns>
		public SummaryResult[] BuildSummary(IEnumerable<Element> summaryList, Channel channel, string searchString)
		{
			if(_cache.ContainsKey(searchString))
				return _cache[searchString];
			else
			{
				SummaryResult[] result;
				Dictionary<string, Dictionary<string, int>> summary = new Dictionary<string, Dictionary<string, int>>();
				foreach (Element currentElement in summaryList)
				{
					foreach (SchemeIndex currentSchemeIndex in channel.Scheme.Indexes)
					{
						string IndexDescription = currentSchemeIndex.IndexDescription;
						if(currentSchemeIndex.IncludeInSummary)
						{
							foreach (SchemeField currentSchemeField in currentSchemeIndex.FieldSchema)
							{
								string text = channel.Scheme.ValueHandle(currentSchemeField.FieldName)(currentElement);
								if(!String.IsNullOrEmpty(text))
								{
									if(summary.ContainsKey(IndexDescription))
									{
										if(summary[IndexDescription].ContainsKey(text))
										{
											summary[IndexDescription][text]++;
										}
										else
										{
											summary[IndexDescription].Add(text, 1);
										}
									}
									else
									{
										summary.Add(IndexDescription, new Dictionary<string, int>());
										summary[IndexDescription].Add(text, 1);
									}
								}
							}
						}
					}
				}
				result = new SummaryResult[summary.Count];
				int index = 0;
				foreach (string currentKey in summary.Keys)
				{
					result[index] = new SummaryResult(currentKey, summary[currentKey]);
					index++;
				}
				lock (_cache)
				{
					if(!_cache.ContainsKey(searchString))
						_cache.Add(searchString, result);
				}
				return result;
			}
		}
	}
}
