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
using Colosoft.Serialization.Surrogates;
using Colosoft.Caching.Policies;
using System.Collections;
using Colosoft.Caching.Expiration;
using Colosoft.Caching.Data;
using Colosoft.Caching.Statistics;

namespace Colosoft.Caching
{
	/// <summary>
	/// Classe com utilidade diversas.
	/// </summary>
	public class MiscUtil
	{
		/// <summary>
		/// Cria um clone de profundidade para o dicionário informado.
		/// </summary>
		/// <param name="dic"></param>
		/// <returns></returns>
		public static IDictionary DeepClone(IDictionary dic)
		{
			Hashtable hashtable = new Hashtable();
			foreach (DictionaryEntry entry in dic)
			{
				ICloneable cloneable = entry.Value as ICloneable;
				if(cloneable != null)
					hashtable[entry.Key] = cloneable.Clone();
				else
					hashtable[entry.Key] = entry.Value;
			}
			return hashtable;
		}

		/// <summary>
		/// Preenche os dos vetores com os dados informados.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="entries">Instancias das entradas.</param>
		/// <param name="available">Vetor com as instancias disponíveis.</param>
		/// <param name="data">Dados das entradas.</param>
		/// <param name="list"></param>
		public static void FillArrays(object[] keys, CacheEntry[] entries, object[] available, CacheEntry[] data, ArrayList list)
		{
			Hashtable table = new Hashtable();
			foreach (object obj2 in list)
				table.Add(obj2, "");
			FillArrays(keys, entries, available, data, table);
		}

		/// <summary>
		/// Preenche os dos vetores com os dados informados.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="entries">Instancias das entradas.</param>
		/// <param name="available">Vetor com as instancias disponíveis.</param>
		/// <param name="data">Dados das entradas.</param>
		/// <param name="table"></param>
		public static void FillArrays(object[] keys, CacheEntry[] entries, object[] available, CacheEntry[] data, Hashtable table)
		{
			int index = 0;
			int num2 = 0;
			foreach (object obj2 in keys)
			{
				if(!table.Contains(obj2))
				{
					available[index] = obj2;
					data[index] = entries[num2];
					index++;
				}
				num2++;
			}
		}

		/// <summary>
		/// Preenche os dos vetores com os dados informados.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="entries">Instancias das entradas.</param>
		/// <param name="unAvailable">Vetor com as instancias indisponíveis.</param>
		/// <param name="available">Vetor com as instancias disponíveis.</param>
		/// <param name="data">Dados das entradas.</param>
		/// <param name="list"></param>
		public static void FillArrays(object[] keys, CacheEntry[] entries, object[] unAvailable, object[] available, CacheEntry[] data, ArrayList list)
		{
			Hashtable table = new Hashtable();
			foreach (object obj2 in list)
			{
				table.Add(obj2, "");
			}
			FillArrays(keys, entries, unAvailable, available, data, table);
		}

		/// <summary>
		/// Preenche os dos vetores com os dados informados.
		/// </summary>
		/// <param name="keys">Chaves das entradas.</param>
		/// <param name="entries">Instancias das entradas.</param>
		/// <param name="unAvailable">Vetor com as instancias indisponíveis.</param>
		/// <param name="available">Vetor com as instancias disponíveis.</param>
		/// <param name="data">Dados das entradas.</param>
		/// <param name="table"></param>
		public static void FillArrays(object[] keys, CacheEntry[] entries, object[] unAvailable, object[] available, CacheEntry[] data, Hashtable table)
		{
			int index = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (object obj2 in keys)
			{
				if(!table.Contains(obj2))
				{
					available[index] = obj2;
					data[index] = entries[num3];
					index++;
				}
				else
				{
					unAvailable[num2] = obj2;
					num2++;
				}
				num3++;
			}
		}

		/// <summary>
		/// Recupera um vetor da coleção informada.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public static object[] GetArrayFromCollection(ICollection col)
		{
			if(col == null)
				return null;
			object[] array = new object[col.Count];
			col.CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// Recupera o conjunto de chaves de cache informado.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		internal static object[] GetKeyset(CacheBase cache, int timeout)
		{
			ulong num = 0;
			object[] objArray = null;
			cache.Sync.AcquireWriterLock(timeout);
			try
			{
				if(!cache.Sync.IsWriterLockHeld || (cache.Count < 1))
					return objArray;
				objArray = new object[cache.Count];
				IEnumerator enumerator = cache.GetEnumerator();
				while (enumerator.MoveNext())
				{
					num++;
					DictionaryEntry current = (DictionaryEntry)enumerator.Current;
					objArray[(int)((IntPtr)num)] = current.Key;
				}
			}
			finally
			{
				cache.Sync.ReleaseWriterLock();
			}
			return objArray;
		}

		/// <summary>
		/// Recupera as chave que não estão disponíveis.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static object[] GetNotAvailableKeys(object[] keys, ArrayList list)
		{
			Hashtable table = new Hashtable();
			foreach (object obj2 in list)
				table.Add(obj2, "");
			return GetNotAvailableKeys(keys, table);
		}

		/// <summary>
		/// Recupera a chaves que não estão disponíveis.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public static object[] GetNotAvailableKeys(object[] keys, Hashtable table)
		{
			object[] objArray = new object[keys.Length - table.Count];
			int index = 0;
			foreach (object obj2 in keys)
			{
				if(!table.Contains(obj2))
				{
					objArray[index] = obj2;
					index++;
				}
			}
			return objArray;
		}

		/// <summary>
		/// Registra os tipos compactos.
		/// </summary>
		public static void RegisterCompactTypes()
		{
			TypeSurrogateSelector.RegisterTypeSurrogate(new ArraySerializationSurrogate(typeof(CacheEntry[])));
			TypeSurrogateSelector.RegisterTypeSurrogate(new ArraySerializationSurrogate(typeof(WriteBehindAsyncProcessor.IWriteBehindTask[])));
			TypeSurrogateSelector.RegisterTypeSurrogate(new CustomArraySerializationSurrogate(typeof(CustomArraySerializationSurrogate)));
			CompactFormatterServices.RegisterCompactType(typeof(CacheEntry), 0x3d);
			CompactFormatterServices.RegisterCompactType(typeof(CounterHint), 0x3e);
			CompactFormatterServices.RegisterCompactType(typeof(TimestampHint), 0x3f);
			CompactFormatterServices.RegisterCompactType(typeof(PriorityEvictionHint), 0x40);
			CompactFormatterServices.RegisterCompactType(typeof(AggregateExpirationHint), 0x44);
			CompactFormatterServices.RegisterCompactType(typeof(IdleExpiration), 0x45);
			CompactFormatterServices.RegisterCompactType(typeof(LockExpiration), 0x87);
			CompactFormatterServices.RegisterCompactType(typeof(FixedExpiration), 70);
			CompactFormatterServices.RegisterCompactType(typeof(KeyDependency), 0x47);
			CompactFormatterServices.RegisterCompactType(typeof(FixedIdleExpiration), 0x48);
			CompactFormatterServices.RegisterCompactType(typeof(DependencyHint), 0x49);
			CompactFormatterServices.RegisterCompactType(typeof(CompactCacheEntry), 0x69);
			CompactFormatterServices.RegisterCompactType(typeof(CallbackEntry), 0x6b);
			CompactFormatterServices.RegisterCompactType(typeof(CallbackInfo), 0x6f);
			CompactFormatterServices.RegisterCompactType(typeof(AsyncCallbackInfo), 0x70);
			CompactFormatterServices.RegisterCompactType(typeof(Colosoft.Caching.Synchronization.CacheSyncDependency), 0x71);
			CompactFormatterServices.RegisterCompactType(typeof(CacheInsResultWithEntry), 0x76);
			CompactFormatterServices.RegisterCompactType(typeof(ExtensibleDependency), 0x77);
			CompactFormatterServices.RegisterCompactType(typeof(WriteThruProviderManager.WriteBehindTask), 120);
			CompactFormatterServices.RegisterCompactType(typeof(WriteThruProviderManager.BulkWriteBehindTask), 0x79);
			CompactFormatterServices.RegisterCompactType(typeof(UserBinaryObject), 0x7d);
		}
	}
}
