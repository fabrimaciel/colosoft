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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Representa o descritor das funções de agregação.
	/// </summary>
	public class AggregateDescriptor : IDescriptor
	{
		private readonly IDictionary<string, Func<AggregateFunction>> _aggregateFactories;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AggregateDescriptor()
		{
			Func<AggregateFunction> func = null;
			Func<AggregateFunction> func2 = null;
			Func<AggregateFunction> func3 = null;
			Func<AggregateFunction> func4 = null;
			Func<AggregateFunction> func5 = null;
			this.Aggregates = new List<AggregateFunction>();
			Dictionary<string, Func<AggregateFunction>> dictionary = new Dictionary<string, Func<AggregateFunction>>();
			if(func == null)
			{
				func = () => new SumFunction {
					SourceField = this.Member
				};
			}
			dictionary.Add("sum", func);
			if(func2 == null)
			{
				func2 = () => new CountFunction {
					SourceField = this.Member
				};
			}
			dictionary.Add("count", func2);
			if(func3 == null)
			{
				func3 = () => new AverageFunction {
					SourceField = this.Member
				};
			}
			dictionary.Add("average", func3);
			if(func4 == null)
			{
				func4 = () => new MinFunction {
					SourceField = this.Member
				};
			}
			dictionary.Add("min", func4);
			if(func5 == null)
			{
				func5 = () => new MaxFunction {
					SourceField = this.Member
				};
			}
			dictionary.Add("max", func5);
			this._aggregateFactories = dictionary;
		}

		/// <summary>
		/// Deserializa a fonte informada.
		/// </summary>
		/// <param name="source"></param>
		public void Deserialize(string source)
		{
			string[] items = source.Split(new char[] {
				'-'
			});
			if(items.Any<string>())
			{
				this.Member = items[0];
				for(int i = 1; i < items.Length; i++)
				{
					DeserializeAggregate(items[i]);
				}
			}
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			StringBuilder builder = new StringBuilder(Member);
			foreach (string str in from aggregate in Aggregates
			select aggregate.FunctionName.Split('_')[0].ToLowerInvariant())
			{
				builder.Append("-");
				builder.Append(str);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Deserializa os dados agregados.
		/// </summary>
		/// <param name="aggregate"></param>
		private void DeserializeAggregate(string aggregate)
		{
			Func<AggregateFunction> func;
			if(_aggregateFactories.TryGetValue(aggregate, out func))
				Aggregates.Add(func());
		}

		/// <summary>
		/// Funções de agregação.
		/// </summary>
		public ICollection<AggregateFunction> Aggregates
		{
			get;
			private set;
		}

		/// <summary>
		/// Membro.
		/// </summary>
		public string Member
		{
			get;
			set;
		}
	}
}
