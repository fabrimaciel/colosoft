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
using Colosoft.Data.Schema;
using System.Reflection;
using Colosoft.Query;
using System.Collections;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Implementação do enumerador usado para recupera os registros
	/// de um resulta do cache.
	/// </summary>
	public class QueryResultSetRecordEnumerator : IEnumerator<Record>
	{
		private Record.RecordDescriptor _descriptor;

		private QueryInfo _query;

		private QueryResultSet _result;

		private ITypeSchema _typeSchema;

		private Cache _cache;

		private IEnumerator<object[]> _multipleKeysEnumerator;

		private IEnumerator _keysEnumerator;

		private Record _current;

		/// <summary>
		/// Armazena a referencia para recupera os dados da projeção do registro.
		/// </summary>
		private List<KeyValuePair<int, string>> _projectionPositions;

		/// <summary>
		/// Instancia do descritor associado.
		/// </summary>
		public Record.RecordDescriptor Descriptor
		{
			get
			{
				return _descriptor;
			}
		}

		/// <summary>
		/// Recupera a chave associada ao atual registro.
		/// </summary>
		public object CurrentKey
		{
			get
			{
				return _multipleKeysEnumerator != null ? _multipleKeysEnumerator.Current : _keysEnumerator != null ? _keysEnumerator.Current : null;
			}
		}

		/// <summary>
		/// Instancia do atual registro.
		/// </summary>
		public Record Current
		{
			get
			{
				return _current;
			}
		}

		/// <summary>
		/// Instancia do atual registro.
		/// </summary>
		object IEnumerator.Current
		{
			get
			{
				return _current;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema">Esquema com os tipos.</param>
		/// <param name="cache">Instancia do cache.</param>
		/// <param name="result"></param>
		/// <param name="query"></param>
		public QueryResultSetRecordEnumerator(ITypeSchema typeSchema, Cache cache, QueryResultSet result, QueryInfo query)
		{
			typeSchema.Require("typeSchema").NotNull();
			cache.Require("cache").NotNull();
			result.Require("result").NotNull();
			_typeSchema = typeSchema;
			_cache = cache;
			_result = result;
			_query = query;
			if(result is MultipleKeysQueryResultSet)
			{
				var resultSet = (MultipleKeysQueryResultSet)result;
				_multipleKeysEnumerator = resultSet.Result.Items.GetEnumerator();
			}
			else
			{
				_keysEnumerator = result.SearchKeysResult.GetEnumerator();
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~QueryResultSetRecordEnumerator()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera os metadados associados com a entidade informada.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private ITypeMetadata GetTypeMetadata(EntityInfo entity)
		{
			return _typeSchema.GetTypeMetadata(entity.FullName);
		}

		/// <summary>
		/// Cria um registro com base nas chaves do resultado da consulta.
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="keys"></param>
		/// <returns></returns>
		private Record CreateRecord(EntityInfo[] entities, object[] keys)
		{
			var entries = new object[entities.Length];
			for(var i = 0; i < keys.Length; i++)
			{
				if(keys[i] != null)
				{
					var entry = _cache[keys[i]];
					if(entry is UserBinaryObject)
					{
						var binaryObject = (UserBinaryObject)entry;
						using (var ms = new System.IO.MemoryStream(binaryObject.GetFullObject(), false))
							entry = Colosoft.Serialization.Formatters.CompactBinaryFormatter.Deserialize(ms, _cache.Name);
					}
					entries[i] = entry;
				}
			}
			PropertyInfo[][] propertyInfos = null;
			if(_descriptor == null)
			{
				var descriptor = new Record.RecordDescriptor();
				_projectionPositions = new List<KeyValuePair<int, string>>();
				if(_query == null || _query.Projection == null || _query.Projection.Count == 0)
				{
					var entitiesIndex = new List<int>();
					if(_query != null)
					{
						foreach (var info in _query.Entities)
						{
							var index = Array.FindIndex(entities, f => !string.IsNullOrEmpty(f.Alias) ? f.Alias == info.Alias : f.FullName == info.FullName);
							if(index >= 0)
								entitiesIndex.Add(index);
						}
					}
					else
						for(var i = 0; i < entities.Length; i++)
							entitiesIndex.Add(i);
					foreach (var i in entitiesIndex)
					{
						var typeMetadata = GetTypeMetadata(entities[i]);
						if(typeMetadata == null)
							throw new InvalidOperationException(string.Format("Not found TypeMetadata for entity '{0}'", entities[i]));
						foreach (var property in typeMetadata)
						{
							var index = 0;
							while (descriptor.Contains(property.Name + (index > 0 ? index.ToString() : "")))
								index++;
							descriptor.Add(new Record.Field(property.Name + (index > 0 ? index.ToString() : ""), Type.GetType(property.PropertyType, true)));
							_projectionPositions.Add(new KeyValuePair<int, string>(i, property.Name));
						}
						if(typeMetadata.IsVersioned && !descriptor.Contains("RowVersion"))
						{
							descriptor.Add(new Record.Field("RowVersion", typeof(long)));
							_projectionPositions.Add(new KeyValuePair<int, string>(i, "RowVersion"));
						}
					}
				}
				else
				{
					var typeMetadatas = new Dictionary<int, ITypeMetadata>();
					foreach (var projectionEntry in _query.Projection)
					{
						var columnInfo = projectionEntry.GetColumnInfo();
						if(columnInfo == null)
							continue;
						var entityInfo = _query.Entities.Where(f => f.Alias == columnInfo.Owner).FirstOrDefault();
						if(entityInfo == null)
							entityInfo = _query.Entities.FirstOrDefault();
						var entityIndex = -1;
						for(var i = 0; i < entities.Length; i++)
							if(EntityInfoAliasComparer.Instance.Equals(entities[i], entityInfo))
							{
								entityIndex = i;
								break;
							}
						var name = columnInfo.Alias ?? columnInfo.Name;
						var index = 0;
						while (descriptor.Contains(name + (index > 0 ? index.ToString() : "")))
							index++;
						if(index > 0)
							name += index.ToString();
						if(entityIndex < 0)
						{
							descriptor.Add(new Record.Field(name, typeof(string)));
							_projectionPositions.Add(new KeyValuePair<int, string>(-1, null));
							continue;
						}
						ITypeMetadata typeMetadata = null;
						if(!typeMetadatas.TryGetValue(entityIndex, out typeMetadata))
						{
							typeMetadata = GetTypeMetadata(entities[entityIndex]);
							if(typeMetadata == null)
								throw new InvalidOperationException(string.Format("Not found TypeMetadata for entity '{0}'", entities[entityIndex]));
							typeMetadatas.Add(entityIndex, typeMetadata);
						}
						if(columnInfo.Name == "TableId")
						{
							var identityProperty = typeMetadata.GetKeyProperties().FirstOrDefault();
							if(identityProperty != null)
							{
								descriptor.Add(new Record.Field(name, Type.GetType(identityProperty.PropertyType)));
								_projectionPositions.Add(new KeyValuePair<int, string>(entityIndex, identityProperty.Name));
							}
						}
						else if(columnInfo.Name == Colosoft.Query.DataAccessConstants.RowVersionPropertyName && typeMetadata.IsVersioned)
						{
							descriptor.Add(new Record.Field(name, typeof(long)));
							_projectionPositions.Add(new KeyValuePair<int, string>(entityIndex, Colosoft.Query.DataAccessConstants.RowVersionPropertyName));
							break;
						}
						else
						{
							foreach (var property in typeMetadata)
							{
								if(property.Name == columnInfo.Name)
								{
									descriptor.Add(new Record.Field(name, Type.GetType(property.PropertyType)));
									_projectionPositions.Add(new KeyValuePair<int, string>(entityIndex, property.Name));
									break;
								}
							}
						}
					}
				}
				_descriptor = descriptor;
			}
			object[] values = new object[_descriptor.Count];
			for(var projIndex = 0; projIndex < _projectionPositions.Count; projIndex++)
			{
				var projPosition = _projectionPositions[projIndex];
				if(projPosition.Key < 0)
					continue;
				var entry = entries[projPosition.Key];
				if(entry == null)
					continue;
				if(entry is ICacheItemRecord)
				{
					var itemRecord = (ICacheItemRecord)entry;
					if(itemRecord.Descriptor.Contains(projPosition.Value))
						values[projIndex] = itemRecord.GetValue(projPosition.Value);
				}
				else
				{
					if(propertyInfos == null)
						propertyInfos = new PropertyInfo[entries.Length][];
					if(propertyInfos[projPosition.Key] == null && entry != null)
						propertyInfos[projPosition.Key] = entry.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name).ToArray();
					if(entry != null)
					{
						var propertyInfo = propertyInfos[projPosition.Key].Where(f => f.Name == projPosition.Value).FirstOrDefault();
						if(propertyInfo != null)
							values[projIndex] = propertyInfo.GetValue(entry, null);
					}
				}
			}
			return _descriptor.CreateRecord(values);
		}

		/// <summary>
		/// Move para o próximo registro.
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			if(_multipleKeysEnumerator != null)
			{
				if(_multipleKeysEnumerator.MoveNext())
				{
					var resultSet = (Colosoft.Caching.Queries.MultipleKeysQueryResultSet)_result;
					_current = CreateRecord(resultSet.Result.Entities, _multipleKeysEnumerator.Current);
					return true;
				}
				else
					return false;
			}
			if(_keysEnumerator.MoveNext())
			{
				_current = CreateRecord(_query.Entities, new object[] {
					_keysEnumerator.Current
				});
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Reseta o enumerator.
		/// </summary>
		public void Reset()
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_multipleKeysEnumerator != null)
				_multipleKeysEnumerator.Dispose();
			else if(_keysEnumerator is IDisposable)
				((IDisposable)_keysEnumerator).Dispose();
			_keysEnumerator = null;
			_multipleKeysEnumerator = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
