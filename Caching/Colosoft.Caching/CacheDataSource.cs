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
using Colosoft.Query;
using Microsoft.Practices.ServiceLocation;
using System.Collections;
using System.Reflection;
using Colosoft.Caching.Queries;
using Colosoft.Data.Schema;

namespace Colosoft.Caching
{
	/// <summary>
	/// Implementação do <see cref="IQueryDataSource"/> para o cache.
	/// </summary>
	public class CacheDataSource : IQueryDataSource
	{
		private ICacheProvider _cacheProvider;

		private ITypeSchema _typeSchema;

		private IQueryTranslator _translator;

		private static long _executeCount = 1;

		/// <summary>
		/// Instancia associada do cache.
		/// </summary>
		public Cache Cache
		{
			get
			{
				return _cacheProvider.Cache;
			}
		}

		/// <summary>
		/// Identifica se a instancia foi inicializada.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				return _cacheProvider.IsInitialized;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="cacheProvider">Instancia do provedor do cache.</param>
		/// <param name="typeSchema">Esquema dos tipos do sistema.</param>
		public CacheDataSource(ICacheProvider cacheProvider, ITypeSchema typeSchema)
		{
			cacheProvider.Require("cacheProvider").NotNull();
			typeSchema.Require("typeSchema").NotNull();
			_cacheProvider = cacheProvider;
			_typeSchema = typeSchema;
			_translator = new CacheQueryTranslator(_typeSchema);
		}

		/// <summary>
		/// Executa a consulta informada retornando resultado do cache.
		/// </summary>
		/// <param name="query">Informações da consulta</param>
		/// <returns>Resultado do cache</returns>
		public QueryResultSet ExecuteInCache(QueryInfo query)
		{
			query.Require("query").NotNull();
			string commandText;
			Hashtable parameters;
			Parse(query, out commandText, out parameters);
			if(query.Entities.Length > 1)
			{
				LinkResult endResult = null;
				if(query.WhereClause != null && query.WhereClause.ConditionalsCount > 0)
					endResult = ProcessLinkResult(query, query.WhereClause);
				else
				{
					Parse(new QueryInfo {
						Entities = new EntityInfo[] {
							query.Entities.FirstOrDefault()
						},
					}, out commandText, out parameters);
					var searchResult = _cacheProvider.Cache.Search(commandText, parameters);
					endResult = Convert(searchResult, query.Entities.FirstOrDefault());
				}
				foreach (var join in query.Joins)
				{
					var res = ProcessJoin(query, join);
					endResult = Merge(join.Type == JoinType.Inner ? MergeType.Intersect : MergeType.LeftJoin, endResult, res);
				}
				if(endResult == null)
				{
					string command = "SELECT " + query.Entities[0].FullName;
					endResult = Convert(Cache.Search(command, parameters), query.Entities[0]);
				}
				var defaultEntityIndex = Array.IndexOf(endResult.Entities, query.Entities[0].FullName);
				if(defaultEntityIndex < 0)
					defaultEntityIndex = 0;
				return new MultipleKeysQueryResultSet(endResult, defaultEntityIndex);
			}
			else
				return Cache.Search(commandText, parameters);
		}

		/// <summary>
		/// Executa a consulta informada.
		/// </summary>
		/// <param name="query">Informações da consulta a ser executada</param>
		/// <returns>Retorna o resultado da query</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IQueryResult Execute(QueryInfo query)
		{
			query.Require("query").NotNull();
			#if DEBUG
			            var queryText = query.ToString().Replace("\r\n", " ");
            var executeNumber = _executeCount++;

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif
			try
			{
				var cacheResult = ExecuteInCache(query);
				Record.RecordDescriptor descriptor = new Record.RecordDescriptor();
				if(query.Projection != null && query.Projection.Count == 1 && IsCountFunction(query.Projection[0]))
				{
					var projectionColumn = query.Projection[0];
					descriptor.Add(new Record.Field(projectionColumn.Alias, typeof(int)));
					return new QueryResult(descriptor, new Record[] {
						descriptor.CreateRecord(new object[] {
							cacheResult.SearchKeysResult.Count
						})
					}, query, Execute);
				}
				var records = new List<Record>();
				IComparer<Record> comparer = null;
				var comparerInitialized = false;
				using (var recordsEnumerator = new QueryResultSetRecordEnumerator(this._typeSchema, this.Cache, cacheResult, query))
				{
					while (recordsEnumerator.MoveNext())
					{
						var record = recordsEnumerator.Current;
						descriptor = recordsEnumerator.Descriptor;
						if(!comparerInitialized && query.Sort != null)
						{
							comparerInitialized = true;
							comparer = CreateSortComparer(query, descriptor);
						}
						if(comparerInitialized)
						{
							var index = records.BinarySearch(record, comparer);
							if(index < 0)
								index = ~index;
							records.Insert(index, record);
						}
						else
							records.Add(record);
					}
				}
				return new QueryResult(descriptor, new RecordsResult(records), query, Execute);
			}
			catch(Exception)
			{
				throw;
			}
			finally
			{
				#if DEBUG
				                stopwatch.Stop();
                RegisterEndExecute(stopwatch, queryText, executeNumber);
#endif
			}
		}

		/// <summary>
		/// Retorna o resultado de várias queries recebe os dados de uma query
		/// </summary>
		/// <param name="queries">Informações das queries</param>
		/// <returns>Retorna o resultado da queries</returns>
		public IEnumerable<IQueryResult> Execute(QueryInfo[] queries)
		{
			queries.Require("queries").NotNull();
			IQueryResult[] result = new IQueryResult[queries.Length];
			for(int i = 0; i < queries.Length; i++)
				result[i] = Execute(queries[i]);
			return result;
		}

		/// <summary>
		/// Verifica se a consulta é compatível com o cache.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static bool IsCompatible(QueryInfo query)
		{
			query.Require("query").NotNull();
			if(query.Projection == null || (query.Projection.Count == 1 && IsCountFunction(query.Projection.First())))
				return true;
			foreach (var entry in query.Projection)
			{
				if(entry.CheckSimpleEntry())
					continue;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Verifica se a entrada é uma função COUNT.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		private static bool IsCountFunction(ProjectionEntry entry)
		{
			return entry.Term is FunctionCall && StringComparer.InvariantCultureIgnoreCase.Equals(((FunctionCall)entry.Term).Call.ToString(), "COUNT");
		}

		/// <summary>
		/// Cria o comparador para ordena os resultado da consulta.
		/// </summary>
		/// <param name="query">Informações da consulta.</param>
		/// <param name="descriptor">Descritor com assinatura dos dados do resultado.</param>
		/// <returns></returns>
		private static IComparer<Record> CreateSortComparer(QueryInfo query, Record.RecordDescriptor descriptor)
		{
			IComparer<Record> comparer = null;
			var fields = new List<Colosoft.Query.RecordSortComparer.Field>();
			foreach (var sortPart in query.Sort)
			{
				var sortPartName = sortPart.Name;
				if(string.IsNullOrEmpty(sortPartName))
				{
					throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.NotSupportedException_InvalidSortEntryFormat).Format());
				}
				for(var i = 0; i < descriptor.Count; i++)
					if(descriptor[i].Name == sortPartName)
					{
						fields.Add(new Colosoft.Query.RecordSortComparer.Field(i, sortPart.Reverse));
						break;
					}
			}
			if(fields.Count > 0)
				comparer = new Colosoft.Query.RecordSortComparer(fields);
			return comparer;
		}

		/// <summary>
		/// Realiza o Parse das informações da consulta para uma consulta do cache.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="commandText"></param>
		/// <param name="parameters"></param>
		private void Parse(QueryInfo query, out string commandText, out Hashtable parameters)
		{
			var parser = new CacheQueryParser(_translator, query);
			commandText = parser.GetText();
			parameters = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			if(query.Parameters != null)
				foreach (var param in query.Parameters.GroupBy(p => p.Name))
				{
					var list = new ArrayList();
					foreach (var cacheParameter in param)
					{
						var stringParameter = cacheParameter.Value as string;
						if(stringParameter != null)
							cacheParameter.Value = stringParameter.ToLower();
						list.Add(cacheParameter.Value);
					}
					parameters.Add(param.Key, list);
				}
			foreach (DictionaryEntry param in parser.CacheParameters)
				parameters.Add(param.Key, param.Value);
		}

		/// <summary>
		/// Processa a junção.
		/// </summary>
		/// <param name="query">Informações da consulta.</param>
		/// <param name="join">Informações do join.</param>
		private LinkResult ProcessJoin(QueryInfo query, JoinInfo join)
		{
			LinkResult endResult = ProcessLinkResult(query, join.Conditional);
			;
			return endResult;
		}

		/// <summary>
		/// Processa a junção.
		/// </summary>
		/// <param name="query">Consulta que será analizada.</param>
		/// <param name="term">Termo condicional que será processado.</param>
		/// <returns></returns>
		private LinkResult ProcessLinkResult(QueryInfo query, ConditionalTerm term)
		{
			if(term is Conditional)
			{
				var conditional = (Conditional)term;
				var leftColumn = conditional.Left as Column;
				var rightColumn = conditional.Right as Column;
				EntityInfo leftEntity = null;
				EntityInfo rightEntity = null;
				if(leftColumn != null)
					leftEntity = query.Entities.Where(f => f.Alias == leftColumn.Owner).FirstOrDefault() ?? query.Entities.FirstOrDefault();
				if(rightColumn != null)
					rightEntity = query.Entities.Where(f => f.Alias == rightColumn.Owner).FirstOrDefault() ?? query.Entities.FirstOrDefault();
				if(leftColumn != null && rightColumn != null)
				{
					ComparisonType comparisonType = ComparisonType.EQUALS;
					switch(conditional.Operator.Op)
					{
					case "==":
					case "=":
						comparisonType = ComparisonType.EQUALS;
						break;
					case "<>":
					case "!=":
						comparisonType = ComparisonType.NOT_EQUALS;
						break;
					case ">":
						comparisonType = ComparisonType.GREATER_THAN;
						break;
					case ">=":
						comparisonType = ComparisonType.GREATER_THAN_EQUALS;
						break;
					case "<":
						comparisonType = ComparisonType.LESS_THAN;
						break;
					case "<=":
						comparisonType = ComparisonType.LESS_THAN_EQUALS;
						break;
					default:
						throw new InvalidOperationException("Unsupported operator " + conditional.Operator.Op);
					}
					return new LinkResult {
						Entities = new EntityInfo[] {
							leftEntity,
							rightEntity
						},
						Items = _cacheProvider.Cache.JoinIndex(leftEntity.FullName, leftColumn.Name, rightEntity.FullName, rightColumn.Name, comparisonType).ToArray()
					};
				}
				else
				{
					EntityInfo entity = leftEntity != null ? leftEntity : rightEntity;
					string commandText = null;
					Hashtable parameters = null;
					Parse(new QueryInfo {
						Entities = new EntityInfo[] {
							entity
						},
						WhereClause = new ConditionalContainer(conditional),
						Parameters = query.Parameters
					}, out commandText, out parameters);
					var searchResult = _cacheProvider.Cache.Search(commandText, parameters);
					return Convert(searchResult, entity);
				}
			}
			else if(term is ConditionalContainer)
			{
				var container = (ConditionalContainer)term;
				var logicOperators = new Queue<LogicalOperator>(container.LogicalOperators);
				LinkResult joinResult = null;
				foreach (var term2 in container.Conditionals)
					if(joinResult == null)
						joinResult = ProcessLinkResult(query, term2);
					else
						joinResult = Merge(logicOperators.Dequeue() == LogicalOperator.And ? MergeType.Intersect : MergeType.Union, joinResult, ProcessLinkResult(query, term2));
				return joinResult;
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Método usado para converter o resultado de pesquisa para o resultado de link.
		/// </summary>
		/// <param name="searchResult">Dados do resultado.</param>
		/// <param name="entity">Entitida associada.</param>
		/// <returns></returns>
		private static LinkResult Convert(QueryResultSet searchResult, EntityInfo entity)
		{
			var items = new object[searchResult.SearchKeysResult.Count][];
			for(var i = 0; i < searchResult.SearchKeysResult.Count; i++)
				items[i] = new object[] {
					searchResult.SearchKeysResult[i]
				};
			return new LinkResult {
				Entities = new EntityInfo[] {
					entity
				},
				Items = items
			};
		}

		/// <summary>
		/// Mescla o dois resultados informados.
		/// </summary>
		/// <param name="mergeType">Tipo do Merge.</param>
		/// <param name="leftResult"></param>
		/// <param name="rightResult"></param>
		/// <returns></returns>
		private static LinkResult Merge(MergeType mergeType, LinkResult leftResult, LinkResult rightResult)
		{
			var intersectEntities = new List<int[]>();
			if(rightResult != null)
				for(var i = 0; i < leftResult.Entities.Length; i++)
					for(var j = 0; j < rightResult.Entities.Length; j++)
						if(leftResult.Entities[i] == rightResult.Entities[j])
							intersectEntities.Add(new int[] {
								i,
								j
							});
			var entities = rightResult != null ? leftResult.Entities.Union(rightResult.Entities).ToArray() : leftResult.Entities;
			var items = new List<object[]>();
			if(mergeType == MergeType.Intersect)
				foreach (var rightItem in rightResult.Items)
					foreach (var leftItem in leftResult.Items)
					{
						if(IsIntersect(intersectEntities, leftItem, rightItem))
						{
							items.Add(Concat(leftResult.Entities, leftItem, rightResult.Entities, rightItem, entities));
						}
					}
			else if(mergeType == MergeType.Union)
			{
				if((rightResult.Entities.Length - intersectEntities.Count) > (leftResult.Entities.Length - intersectEntities.Count))
				{
					var aux = leftResult;
					leftResult = rightResult;
					rightResult = aux;
				}
				var leftItems = new List<object[]>();
				foreach (var leftItem in leftResult.Items)
					leftItems.Add(Concat(leftResult.Entities, leftItem, null, null, entities));
				foreach (var rightItem in rightResult.Items)
				{
					var isIntersect = false;
					foreach (var leftItem in leftItems)
						if(IsIntersect(intersectEntities, leftItem, rightItem))
						{
							isIntersect = true;
							break;
						}
					if(!isIntersect)
					{
						if((rightResult.Entities.Length - intersectEntities.Count) != (leftResult.Entities.Length - intersectEntities.Count))
						{
							foreach (var leftItem in leftItems)
								items.Add(Concat(rightResult.Entities, rightItem, leftResult.Entities, leftItem, entities));
						}
						else
							items.Add(Concat(rightResult.Entities, rightItem, null, null, entities));
					}
				}
				items.AddRange(leftItems);
			}
			else if(mergeType == MergeType.LeftJoin)
			{
				foreach (var leftItem in leftResult.Items)
				{
					bool isIntersect = false;
					foreach (var rightItem in rightResult.Items)
					{
						if(IsIntersect(intersectEntities, leftItem, rightItem))
						{
							items.Add(Concat(leftResult.Entities, leftItem, rightResult.Entities, rightItem, entities));
							isIntersect = true;
							break;
						}
					}
					if(!isIntersect)
					{
						items.Add(Concat(leftResult.Entities, leftItem, null, null, entities));
					}
				}
			}
			leftResult = new LinkResult {
				Entities = entities,
				Items = items
			};
			return leftResult;
		}

		/// <summary>
		/// Verifica se existe uma interseção entre os itens informados.
		/// </summary>
		/// <param name="intersectEntities">Posições das entidades que serão utilizadas para verifica a interseção.</param>
		/// <param name="leftItem">Dados do item da esquerda.</param>
		/// <param name="rightItem">Dados do item da direita.</param>
		/// <returns></returns>
		private static bool IsIntersect(IEnumerable<int[]> intersectEntities, object[] leftItem, object[] rightItem)
		{
			var isIntersect = true;
			foreach (var position in intersectEntities)
			{
				if(leftItem[position[0]] != rightItem[position[1]])
				{
					isIntersect = false;
					break;
				}
			}
			return isIntersect;
		}

		/// <summary>
		/// Método usado para concatenar os dados dos itens informados.
		/// </summary>
		/// <param name="leftEntities">Nomes das entidades contidas no item da esquerda.</param>
		/// <param name="leftItem">Item com os dados da esquerda.</param>
		/// <param name="rightEntities">Nomes das entidades contidas no item da direita.</param>
		/// <param name="rightItem">Item com os dados da direita.</param>
		/// <param name="resultEntities">Nome das entidades do resultado final.</param>
		/// <returns></returns>
		private static object[] Concat(EntityInfo[] leftEntities, object[] leftItem, EntityInfo[] rightEntities, object[] rightItem, EntityInfo[] resultEntities)
		{
			var item = new object[resultEntities.Length];
			for(var i = 0; i < resultEntities.Length; i++)
			{
				var j = 0;
				for(; j < leftEntities.Length; j++)
					if(Colosoft.Query.EntityInfoAliasComparer.Instance.Equals(leftEntities[j], resultEntities[i]))
					{
						item[i] = leftItem[j];
						j = -1;
						break;
					}
				if(j >= 0 && (rightEntities != null))
					for(j = 0; j < rightEntities.Length; j++)
						if(Colosoft.Query.EntityInfoAliasComparer.Instance.Equals(rightEntities[j], resultEntities[i]))
						{
							item[i] = rightItem[j];
							j = -1;
							break;
						}
			}
			return item;
		}

		/// <summary>
		/// Registra o fin da execução da consulta.
		/// </summary>
		/// <param name="stopwatch"></param>
		/// <param name="queryText"></param>
		/// <param name="executeNumber"></param>
		private static void RegisterEndExecute(System.Diagnostics.Stopwatch stopwatch, string queryText, long executeNumber)
		{
		}

		/// <summary>
		/// Cria um record a partir de um valor.
		/// </summary>
		/// <param name="cache">Instancia do cache onde está os dados.</param>
		/// <param name="key">Chave da entrada do cache.</param>
		/// <param name="descriptor">Descritor do resultado.</param>
		/// <param name="info"></param>
		/// <returns></returns>
		internal static Record CreateRecord(Cache cache, object key, ref Record.RecordDescriptor descriptor, QueryInfo info)
		{
			var resultValue = cache[key];
			if(resultValue is UserBinaryObject)
			{
				var binaryObject = (UserBinaryObject)resultValue;
				using (var ms = new System.IO.MemoryStream(binaryObject.GetFullObject(), false))
					resultValue = Colosoft.Serialization.Formatters.CompactBinaryFormatter.Deserialize(ms, cache.Name);
			}
			if(resultValue is ICacheItemRecord)
			{
				var itemRecord = (ICacheItemRecord)resultValue;
				if(descriptor.Count == 0)
				{
					if(info != null && info.Projection != null)
					{
						descriptor = new Record.RecordDescriptor();
						foreach (var field in itemRecord.Descriptor)
						{
							if(info.Projection.Any(p =>  {
								var column = p.GetColumnInfo();
								return column != null && column.Name == field.Name;
							}))
								descriptor.Add(field);
						}
					}
					else
						descriptor = itemRecord.Descriptor;
				}
				object[] values = new object[descriptor.Count];
				for(int i = 0; i < descriptor.Count; i++)
				{
					values[i] = itemRecord.GetValue(descriptor[i].Name);
				}
				return descriptor.CreateRecord(values);
			}
			else
			{
				var propertyInfos = resultValue.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name).ToArray();
				if(descriptor.Count == 0)
				{
					List<Record.Field> fields = new List<Record.Field>();
					for(int i = 0; i < propertyInfos.Length; i++)
						fields.Add(new Record.Field(propertyInfos[i].Name, propertyInfos[i].ReflectedType));
					descriptor = new Record.RecordDescriptor("descriptor", fields);
				}
				object[] values = new object[descriptor.Count];
				for(int i = 0; i < values.Length; i++)
					values[i] = propertyInfos[i].GetValue(resultValue, null);
				return descriptor.CreateRecord(values);
			}
		}

		/// <summary>
		/// Implemenção do Enumerable dos registros.
		/// </summary>
		class RecordsResult : IEnumerable<Record>, IDisposable
		{
			private List<Record> _records;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="records"></param>
			public RecordsResult(List<Record> records)
			{
				_records = records;
			}

			/// <summary>
			/// Recupera o enumerador dos registros.
			/// </summary>
			/// <returns></returns>
			public IEnumerator<Record> GetEnumerator()
			{
				return _records.GetEnumerator();
			}

			/// <summary>
			/// Recupera o enumerador dos registros.
			/// </summary>
			/// <returns></returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return _records.GetEnumerator();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				_records.Clear();
			}
		}

		/// <summary>
		/// Possíveis tipos de merge que podem ser processados.
		/// </summary>
		enum MergeType
		{
			Intersect,
			Union,
			LeftJoin
		}

		/// <summary>
		/// Armazena o resulta da ligação das chaves do registros do cache.
		/// </summary>
		internal class LinkResult
		{
			/// <summary>
			/// Nome das entidades de cada linha dos itens.
			/// </summary>
			public EntityInfo[] Entities
			{
				get;
				set;
			}

			/// <summary>
			/// Relação dos itens.
			/// </summary>
			public IEnumerable<object[]> Items
			{
				get;
				set;
			}
		}
	}
}
