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
using System.Runtime.Serialization;
using System.Text;
using Colosoft.Web.Mvc.Extensions;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// Grupo de funções de agregação.
	/// </summary>
	[KnownType(typeof(AggregateFunctionsGroup))]
	public class AggregateFunctionsGroup : Group
	{
		/// <summary>
		/// Projeção das funções de agregação.
		/// </summary>
		[System.Web.Script.Serialization.ScriptIgnore]
		public object AggregateFunctionsProjection
		{
			get;
			set;
		}

		/// <summary>
		/// Agregações.
		/// </summary>
		public IDictionary<string, object> Aggregates
		{
			get
			{
				if(this.AggregateFunctionsProjection == null)
				{
					return new Dictionary<string, object>();
				}
				return ExtractPropertyValues(this.AggregateFunctionsProjection).GroupBy<KeyValuePair<string, object>, string>(delegate(KeyValuePair<string, object> entry) {
					int index = entry.Key.IndexOf('_');
					return entry.Key.Substring(index + 1, (entry.Key.LastIndexOf('_') - index) - 1);
				}).ToDictionary<IGrouping<string, KeyValuePair<string, object>>, string, object>(g => g.Key, g => g.ToDictionary<KeyValuePair<string, object>, string, object>(entry => entry.Key.Split(new char[] {
					'_'
				}).First<string>(), entry => entry.Value));
			}
		}

		/// <summary>
		/// Cria resultados agregados para os valores das propriedades.
		/// </summary>
		/// <param name="functions"></param>
		/// <param name="propertyValues"></param>
		/// <returns></returns>
		private static IEnumerable<AggregateResult> CreateAggregateResultsForPropertyValues(IEnumerable<AggregateFunction> functions, IDictionary<string, object> propertyValues)
		{
			foreach (AggregateFunction i in functions)
			{
				string functionName = i.FunctionName;
				if(propertyValues.ContainsKey(functionName))
				{
					object j = propertyValues[functionName];
					AggregateResult x = new AggregateResult(j, i);
					yield return x;
				}
			}
		}

		/// <summary>
		/// Extraí os valores das propriedades do objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static IDictionary<string, object> ExtractPropertyValues(object obj)
		{
			return (from p in obj.GetType().GetProperties()
			let value = p.GetValue(obj, null)
			select new {
				Key = p.Name,
				Value = value
			}).ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		/// <summary>
		/// Recupera os resultados das agregações.
		/// </summary>
		/// <param name="functions"></param>
		/// <returns></returns>
		public AggregateResultCollection GetAggregateResults(IEnumerable<AggregateFunction> functions)
		{
			if(functions == null)
			{
				throw new ArgumentNullException("functions");
			}
			AggregateResultCollection instance = new AggregateResultCollection();
			if(this.AggregateFunctionsProjection != null)
			{
				IDictionary<string, object> propertyValues = ExtractPropertyValues(this.AggregateFunctionsProjection);
				IEnumerable<AggregateResult> collection = CreateAggregateResultsForPropertyValues(functions, propertyValues);
				instance.AddRange(collection);
			}
			return instance;
		}
	}
}
