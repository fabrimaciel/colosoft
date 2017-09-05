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

namespace Colosoft
{
	/// <summary>
	/// Classe que contém os método de extensão para a coleção do sistema.
	/// </summary>
	public static class EnumerableExtentions
	{
		/// <summary>
		/// Transforma a enumeração em uma coleção observada.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.IObservableCollection<TSource> ToObservableCollection<TSource>(this IEnumerable<TSource> source)
		{
			source.Require("source").NotNull();
			return new Collections.BaseObservableCollection<TSource>(source);
		}

		/// <summary>
		/// Faz o cast da coleção informada para a coleção do tipo TDestination.
		/// </summary>
		/// <typeparam name="TDestination"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static Collections.IObservableCollection<TDestination> CastObservableCollection<TDestination>(this Collections.IObservableCollection source)
		{
			if(source == null)
				return null;
			return new Collections.ObservableCollectionWrapper<TDestination>(source);
		}

		/// <summary>
		/// Verifica se a sequência é nula ou está vazia.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> instance)
		{
			return (instance == null) || (!instance.Any());
		}

		/// <summary>
		/// Verifica se a sequência é nula ou está vazia.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this System.Collections.IEnumerable instance)
		{
			return (instance == null) || (!instance.GetEnumerator().MoveNext());
		}

		/// <summary>
		/// Indica se a sequência é não nula e possui itens.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool HasItems<T>(this IEnumerable<T> instance)
		{
			return (instance != null) && instance.Any();
		}

		/// <summary>
		/// Indica se a sequência é não nula e possui itens.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool HasItems(this System.Collections.IEnumerable instance)
		{
			return (instance != null) && instance.GetEnumerator().MoveNext();
		}

		/// <summary>
		/// Cria uma coleção ordena.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">Coleção de origem dos dados.</param>
		/// <param name="properties">Propriedade que serão utilizadas na ordenação.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static Collections.IObservableCollection<T> OrderByObservableCollection<T>(this Collections.IObservableCollection<T> source, params System.Linq.Expressions.Expression<Func<T, object>>[] properties)
		{
			source.Require("source").NotNull();
			properties.Require("properties").NotNull().NotEmptyCollection();
			return new Collections.SortableObservableCollection<T>(source, new PropertiesComparer<T>(properties));
		}

		/// <summary>
		/// Cria uma coleção ordena.
		/// </summary>
		/// <param name="source">Coleção de origem dos dados.</param>
		/// <param name="properties">Propriedade que serão utilizadas na ordenação.</param>
		/// <returns></returns>
		public static Collections.IObservableCollection OrderByObservableCollection(this Collections.IObservableCollection source, params string[] properties)
		{
			source.Require("source").NotNull();
			properties.Require("properties").NotNull().NotEmptyCollection();
			var type = source.GetType().FindInterfaces((t, m) => t.IsGenericType && typeof(Collections.IObservableCollection<>).IsAssignableFrom(t.GetGenericTypeDefinition()), null).Select(f => f.GetGenericArguments().FirstOrDefault()).FirstOrDefault();
			if(type == null)
				throw new InvalidOperationException("Invalid source");
			var comparer = typeof(PropertiesComparer<>).MakeGenericType(type).GetConstructor(new Type[] {
				typeof(string[])
			}).Invoke(new object[] {
				properties
			});
			return (Collections.IObservableCollection)Activator.CreateInstance(typeof(Collections.SortableObservableCollection<>).MakeGenericType(type), source, comparer);
		}

		/// <summary>
		/// Realiza uma pesquisa binária.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="value"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int BinarySearch<T>(this IList<T> array, T value, IComparer<T> comparer)
		{
			array.Require("array").NotNull();
			return BinarySearch<T>(array, 0, array.Count, value, comparer);
		}

		/// <summary>
		/// Realiza uma pesquisa binária.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int BinarySearch<T>(this IList<T> array, T value)
		{
			array.Require("array").NotNull();
			return BinarySearch<T>(array, 0, array.Count, value, null);
		}

		/// <summary>
		/// Realiza uma pesquisa binária
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">Itens onde será feita a pesquisa.</param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		/// <param name="value"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static int BinarySearch<T>(this IList<T> array, int index, int length, T value, IComparer<T> comparer)
		{
			array.Require("array").NotNull();
			if((index < 0) || (length < 0))
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "length", "Need non negative number");
			if((array.Count - index) < length)
				throw new ArgumentException("Invalid offLen");
			int num;
			try
			{
				if(comparer == null)
					comparer = Comparer<T>.Default;
				num = InternalBinarySearch(array, index, length, value, comparer);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Comparer failed", exception);
			}
			return num;
		}

		/// <summary>
		/// Realiza uma pesquisa binaria na lista informada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="index">Index inicial.</param>
		/// <param name="length">Tamanho.</param>
		/// <param name="value">Valor que será pesquisado.</param>
		/// <param name="comparer">Instancia do comparador.</param>
		/// <returns></returns>
		internal static int InternalBinarySearch<T>(this IList<T> array, int index, int length, T value, IComparer<T> comparer)
		{
			int num = index;
			int num2 = (index + length) - 1;
			while (num <= num2)
			{
				int num3 = num + ((num2 - num) >> 1);
				int num4 = comparer.Compare(array[num3], value);
				if(num4 == 0)
					return num3;
				if(num4 < 0)
					num = num3 + 1;
				else
					num2 = num3 - 1;
			}
			return ~num;
		}

		/// <summary>
		/// Determina se os objetos sequência são iguais.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsSameSequence<T>(this IEnumerable<T> instance, IEnumerable<T> other)
		{
			var aNull = instance == null;
			var bNull = other == null;
			if(aNull && bNull)
			{
				return true;
			}
			if(aNull ^ bNull)
			{
				return false;
			}
			return Enumerable.SequenceEqual(instance, other);
		}

		/// <summary>
		/// Cria um indice do tipo hash para a coleção.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="PropertyType"></typeparam>
		/// <param name="collection"></param>
		/// <param name="property"></param>
		public static void CreateHashIndex<T, PropertyType>(this Collections.IObservableCollection<T> collection, System.Linq.Expressions.Expression<Func<T, PropertyType>> property)
		{
			var indexedCollection = collection as Collections.IIndexedObservableCollection<T>;
			if(indexedCollection == null)
				return;
			indexedCollection.CreateIndex<PropertyType>(property, Collections.ObservableCollectionIndexType.Hash);
		}

		/// <summary>
		/// Aplica o filtro sobre a coleção.
		/// </summary>
		/// <typeparam name="T">Tipo que é armazenado na coleção.</typeparam>
		/// <param name="collection">Coleção que será filtrada.</param>
		/// <param name="filter">Predicato que será utilizado.</param>
		public static Collections.FilteredObservableCollection<T> ApplyFilter<T>(this Collections.IObservableCollection<T> collection, Predicate<T> filter)
		{
			if(collection == null)
				return null;
			return new Collections.FilteredObservableCollection<T>(collection, filter);
		}

		/// <summary>
		/// Recupera a descrições dos parametros de pesquisa associados com o
		/// enumerador informado.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable<Collections.SearchParamerterDescription> GetSearchParameterDescriptions<TSource>(this IEnumerable<TSource> source)
		{
			if(source is Collections.ISearchParameterDescriptionContainer)
				return ((Collections.ISearchParameterDescriptionContainer)source).SearchParameterDescriptions;
			return new Collections.SearchParamerterDescription[0];
		}

		/// <summary>
		/// Junta as descrições dos parametros.
		/// </summary>
		/// <param name="descriptions"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static IMessageFormattable Join(this IEnumerable<Collections.SearchParamerterDescription> descriptions, string separator)
		{
			if(descriptions != null)
				return descriptions.Select(f => f.Description).Join(separator);
			return MessageFormattable.Empty;
		}

		/// <summary>
		/// Aplica as diferenças da coleção da direita na coleção da esquerda.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left">Coleção onde serão aplicadas as diferenças.</param>
		/// <param name="right">Coleção com as diferenças</param>
		/// <param name="predicate">Predicado usado para comparar os itens das coleções.</param>
		public static void ApplyDiff<T>(this IList<T> left, IList<T> right, Func<T, T, bool> predicate)
		{
			var aux = new List<T>(right);
			for(var i = 0; i < left.Count; i++)
			{
				var leftValue = left[i];
				var rightValue = aux.FirstOrDefault(f => predicate(leftValue, f));
				if(rightValue == null)
					left.RemoveAt(i--);
				else
				{
					left[i] = rightValue;
					aux.Remove(rightValue);
				}
			}
			foreach (var rightValue in aux)
				left.Add(rightValue);
		}

		/// <summary>
		/// Aplica as diferenças da coleção da direita na coleção da esquerda.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left">Coleção onde serão aplicadas as diferenças.</param>
		/// <param name="right">Coleção com as diferenças</param>
		/// <param name="predicate">Predicado usado para comparar os itens das coleções.</param>
		public static void ApplyDiff<T>(this System.Collections.IList left, System.Collections.IList right, Func<T, T, bool> predicate)
		{
			var aux = new List<object>();
			foreach (var i in right)
				aux.Add(i);
			for(var i = 0; i < left.Count; i++)
			{
				var leftValue = left[i];
				var rightValue = aux.FirstOrDefault(f => predicate((T)leftValue, (T)f));
				if(rightValue == null)
					left.RemoveAt(i--);
				else
				{
					left[i] = rightValue;
					aux.Remove(rightValue);
				}
			}
			foreach (var rightValue in aux)
				left.Add(rightValue);
		}

		/// <summary>
		/// Anexa mais itens para a coleção informada.
		/// </summary>
		/// <typeparam name="T1">Tipo da coleção</typeparam>
		/// <typeparam name="T2">Tipo do item da coleção.</typeparam>
		/// <param name="collection"></param>
		/// <param name="items">Itens que serão adicionados a coleção.</param>
		/// <returns></returns>
		public static T1 Append<T1, T2>(this T1 collection, params T2[] items) where T1 : ICollection<T2>
		{
			collection.Require("collection").NotNull();
			if(items != null)
				foreach (var i in items)
					collection.Add(i);
			return collection;
		}

		/// <summary>
		/// Segmenta os itens informados em grupos com a quantidade máxima por grupo.
		/// </summary>
		/// <typeparam name="T">Tipo dos itens.</typeparam>
		/// <param name="items">Itens que serão segmentados.</param>
		/// <param name="count">Quantidade que será segmentadas por grupo.</param>
		/// <returns></returns>
		public static IEnumerable<IEnumerable<T>> Break<T>(this IEnumerable<T> items, int count)
		{
			if(items != null)
			{
				var items2 = new List<T>();
				var counter = 1;
				foreach (var i in items)
				{
					items2.Add(i);
					if(counter == count)
					{
						counter = 1;
						yield return items2;
						items2 = new List<T>();
					}
					else
						counter++;
				}
				if(items2.Count > 0)
					yield return items2;
			}
		}

		/// <summary>
		/// Junta os segmentos informados.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="segment1"></param>
		/// <param name="segment2"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<IEnumerable<T>, IEnumerable<T>>> JoinSegments<T>(this IEnumerable<IEnumerable<T>> segment1, IEnumerable<IEnumerable<T>> segment2)
		{
			var first = segment1 != null ? segment1 : segment2;
			var second = segment2 != null ? segment2 : null;
			IEnumerator<IEnumerable<T>> secondEnumerator = second != null ? second.GetEnumerator() : null;
			foreach (var i in first)
			{
				var secondItem = secondEnumerator != null && secondEnumerator.MoveNext() ? secondEnumerator.Current : null;
				yield return new Tuple<IEnumerable<T>, IEnumerable<T>>(first == segment1 ? i : secondItem, first == segment1 ? secondItem : i);
			}
		}

		/// <summary>
		/// Transforma a coleção informado e uma lista virtual.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public static Colosoft.Collections.VirtualList<T> ToVirtualList<T>(this IEnumerable<T> items, int pageSize = 0)
		{
			return new Collections.VirtualList<T>(pageSize, (sender, e) =>  {
				if(e.NeedItemsCount)
					return new Colosoft.Collections.VirtualListLoaderResult<T>(null, items.Count());
				items = items.Skip(e.StartRow).Take(e.PageSize);
				return new Colosoft.Collections.VirtualListLoaderResult<T>(items);
			}, null);
		}
	}
}
