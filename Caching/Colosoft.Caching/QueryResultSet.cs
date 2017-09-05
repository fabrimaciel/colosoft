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
using Colosoft.Serialization;
using System.Collections;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Representa o conjunto do resultado de uma consulta.
	/// </summary>
	public class QueryResultSet : ICompactSerializable
	{
		private DictionaryEntry _aggregateFunctionResult;

		private AggregateFunctionType _aggregateFunctionType = AggregateFunctionType.NOTAPPLICABLE;

		private string _cqId;

		private bool _isInitialized;

		private QueryType _queryType;

		private Hashtable _searchEntriesResult;

		private ArrayList _searchKeysResult;

		/// <summary>
		/// Resulta do da função de agregação.
		/// </summary>
		public DictionaryEntry AggregateFunctionResult
		{
			get
			{
				return _aggregateFunctionResult;
			}
			set
			{
				_aggregateFunctionResult = value;
			}
		}

		/// <summary>
		/// Tipo da função de agregação
		/// </summary>
		public AggregateFunctionType AggregateFunctionType
		{
			get
			{
				return _aggregateFunctionType;
			}
			set
			{
				_aggregateFunctionType = value;
			}
		}

		/// <summary>
		/// Identificador unico da consulta.
		/// </summary>
		public string CQUniqueId
		{
			get
			{
				return _cqId;
			}
			set
			{
				_cqId = value;
			}
		}

		/// <summary>
		/// Identifica se já foi initializado.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		/// <summary>
		/// Entradas do resultado.
		/// </summary>
		public Hashtable SearchEntriesResult
		{
			get
			{
				return _searchEntriesResult;
			}
			set
			{
				_searchEntriesResult = value;
			}
		}

		/// <summary>
		/// Chave do resultado.
		/// </summary>
		public virtual ArrayList SearchKeysResult
		{
			get
			{
				return _searchKeysResult;
			}
			set
			{
				_searchKeysResult = value;
			}
		}

		/// <summary>
		/// Tipo de pesquisa.
		/// </summary>
		public QueryType Type
		{
			get
			{
				return _queryType;
			}
			set
			{
				_queryType = value;
			}
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public virtual void Deserialize(CompactReader reader)
		{
			_aggregateFunctionResult = (DictionaryEntry)reader.ReadObject();
			_searchKeysResult = reader.ReadObject() as ArrayList;
			_searchEntriesResult = reader.ReadObject() as Hashtable;
			_queryType = (QueryType)reader.ReadInt32();
			_aggregateFunctionType = (AggregateFunctionType)reader.ReadInt32();
			_cqId = reader.ReadString();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_aggregateFunctionResult);
			writer.WriteObject(_searchKeysResult);
			writer.WriteObject(_searchEntriesResult);
			writer.Write(Convert.ToInt32(_queryType));
			writer.Write(Convert.ToInt32(_aggregateFunctionType));
			writer.Write(this.CQUniqueId);
		}

		/// <summary>
		/// Compila o resultado.
		/// </summary>
		/// <param name="resultSet">Resultado que será usado na compilação.</param>
		public void Compile(QueryResultSet resultSet)
		{
			if(!_isInitialized)
				Initialize(resultSet);
			else
			{
				switch(this.Type)
				{
				case QueryType.SearchKeys:
					if(SearchKeysResult != null)
					{
						SearchKeysResult.AddRange(resultSet.SearchKeysResult);
						return;
					}
					this.SearchKeysResult = resultSet.SearchKeysResult;
					return;
				case QueryType.SearchEntries:
					if(this.SearchEntriesResult != null)
					{
						IDictionaryEnumerator enumerator = resultSet.SearchEntriesResult.GetEnumerator();
						while (enumerator.MoveNext())
							SearchEntriesResult[enumerator.Key] = enumerator.Value;
						return;
					}
					this.SearchEntriesResult = resultSet.SearchEntriesResult;
					return;
				case QueryType.AggregateFunction:
					decimal num;
					decimal num2;
					object obj2;
					object obj3;
					IComparable comparable;
					IComparable comparable2;
					switch(((AggregateFunctionType)this.AggregateFunctionResult.Key))
					{
					case AggregateFunctionType.SUM:
					{
						obj2 = this.AggregateFunctionResult.Value;
						obj3 = resultSet.AggregateFunctionResult.Value;
						decimal? nullable = null;
						if((obj2 == null) && (obj3 != null))
							nullable = new decimal?((decimal)obj3);
						else if((obj2 != null) && (obj3 == null))
							nullable = new decimal?((decimal)obj2);
						else if((obj2 != null) && (obj3 != null))
						{
							num = (decimal)obj2;
							num2 = (decimal)obj3;
							nullable = new decimal?(num + num2);
						}
						if(nullable.HasValue)
						{
							this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.SUM, nullable);
							return;
						}
						this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.SUM, null);
						return;
					}
					case AggregateFunctionType.COUNT:
					{
						num = (decimal)this.AggregateFunctionResult.Value;
						num2 = (decimal)resultSet.AggregateFunctionResult.Value;
						decimal num3 = num + num2;
						this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.COUNT, num3);
						return;
					}
					case AggregateFunctionType.MIN:
					{
						comparable = (IComparable)this.AggregateFunctionResult.Value;
						comparable2 = (IComparable)resultSet.AggregateFunctionResult.Value;
						IComparable comparable3 = comparable;
						if((comparable == null) && (comparable2 != null))
							comparable3 = comparable2;
						else if((comparable != null) && (comparable2 == null))
							comparable3 = comparable;
						else if((comparable == null) && (comparable2 == null))
							comparable3 = null;
						else if(comparable2.CompareTo(comparable) < 0)
							comparable3 = comparable2;
						this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.MIN, comparable3);
						return;
					}
					case AggregateFunctionType.MAX:
					{
						comparable = (IComparable)this.AggregateFunctionResult.Value;
						comparable2 = (IComparable)resultSet.AggregateFunctionResult.Value;
						IComparable comparable4 = comparable;
						if((comparable == null) && (comparable2 != null))
							comparable4 = comparable2;
						else if((comparable != null) && (comparable2 == null))
							comparable4 = comparable;
						else if((comparable == null) && (comparable2 == null))
							comparable4 = null;
						else if(comparable2.CompareTo(comparable) > 0)
							comparable4 = comparable2;
						this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.MAX, comparable4);
						return;
					}
					case AggregateFunctionType.AVG:
					{
						obj2 = this.AggregateFunctionResult.Value;
						obj3 = resultSet.AggregateFunctionResult.Value;
						AverageResult result = null;
						if((obj2 == null) && (obj3 != null))
							result = (AverageResult)obj3;
						else if((obj2 != null) && (obj3 == null))
							result = (AverageResult)obj2;
						else if((obj2 != null) && (obj3 != null))
						{
							AverageResult result2 = (AverageResult)obj2;
							AverageResult result3 = (AverageResult)obj3;
							result = new AverageResult();
							result.Sum = result2.Sum + result3.Sum;
							result.Count = result2.Count + result3.Count;
						}
						if(result != null)
						{
							this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.AVG, result);
							return;
						}
						this.AggregateFunctionResult = new DictionaryEntry(AggregateFunctionType.AVG, null);
						return;
					}
					}
					return;
				}
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="resultSet"></param>
		public void Initialize(QueryResultSet resultSet)
		{
			if(!_isInitialized)
			{
				this.Type = resultSet.Type;
				this.AggregateFunctionType = resultSet.AggregateFunctionType;
				this.AggregateFunctionResult = resultSet.AggregateFunctionResult;
				this.SearchKeysResult = resultSet.SearchKeysResult;
				this.SearchEntriesResult = resultSet.SearchEntriesResult;
				_isInitialized = true;
			}
		}
	}
}
