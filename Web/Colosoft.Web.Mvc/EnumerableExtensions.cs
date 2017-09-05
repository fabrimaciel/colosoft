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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Extensions
{
	static class EnumerableExtensions
	{
		/// <summary>
		/// Converter o enumerable informado par aum enumerable genérico.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IEnumerable AsGenericEnumerable(this IEnumerable source)
		{
			Type type = typeof(object);
			if(source.GetType().FindGenericType(typeof(IEnumerable<>)) != null)
				return source;
			IEnumerator enumerator = source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if(enumerator.Current != null)
				{
					type = enumerator.Current.GetType();
					try
					{
						enumerator.Reset();
						break;
					}
					catch
					{
						break;
					}
				}
			}
			Type type3 = typeof(GenericEnumerable<>).MakeGenericType(new Type[] {
				type
			});
			object[] args = new object[] {
				source
			};
			return (IEnumerable)Activator.CreateInstance(type3, args);
		}

		/// <summary>
		/// Consolida os itens.
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="resultSelector"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> Consolidate<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			if(first == null)
			{
				throw new ArgumentNullException("first");
			}
			if(second == null)
			{
				throw new ArgumentNullException("second");
			}
			if(resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			return ZipIterator<TFirst, TSecond, TResult>(first, second, resultSelector);
		}

		/// <summary>
		/// Executa a ação informada para cada iten na enumeração.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="action"></param>
		public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
		{
			foreach (T local in instance)
			{
				action(local);
			}
		}

		/// <summary>
		/// Executa a ação informada para cada iten na enumeração.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="action"></param>
		public static void Each<T>(this IEnumerable<T> instance, Action<T, int> action)
		{
			int num = 0;
			foreach (T local in instance)
			{
				action(local, num++);
			}
		}

		/// <summary>
		/// Recupera o elemento da enumeração no indice informado.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static object ElementAt(this IEnumerable source, int index)
		{
			if(index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			IList list = source as IList;
			if((list != null) && (list.Count > 0))
			{
				return list[index];
			}
			foreach (object obj2 in source)
			{
				if(index == 0)
				{
					return obj2;
				}
				index--;
			}
			return null;
		}

		/// <summary>
		/// Recupera o indice do elemento informado na enumeração.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		internal static int IndexOf(this IEnumerable source, object item)
		{
			int num = 0;
			foreach (object obj2 in source)
			{
				if(object.Equals(obj2, item))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		/// <summary>
		/// Seleciona os itens recursivamente.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="recursiveSelector"></param>
		/// <returns></returns>
		public static IEnumerable<TSource> SelectRecursive<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> recursiveSelector)
		{
			Stack<IEnumerator<TSource>> iteratorVariable0 = new Stack<IEnumerator<TSource>>();
			iteratorVariable0.Push(source.GetEnumerator());
			while (iteratorVariable0.Count > 0)
			{
				if(iteratorVariable0.Peek().MoveNext())
				{
					TSource current = iteratorVariable0.Peek().Current;
					yield return current;
					IEnumerable<TSource> iteratorVariable2 = recursiveSelector(current);
					if(iteratorVariable2 != null)
					{
						iteratorVariable0.Push(iteratorVariable2.GetEnumerator());
					}
				}
				else
				{
					iteratorVariable0.Pop().Dispose();
				}
			}
		}

		/// <summary>
		/// Converte a sequencia informada em uma coleção somente leitura.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence)
		{
			if(sequence == null)
			{
				return DefaultReadOnlyCollection<T>.Empty;
			}
			ReadOnlyCollection<T> onlys = sequence as ReadOnlyCollection<T>;
			if(onlys != null)
			{
				return onlys;
			}
			return new ReadOnlyCollection<T>(sequence.ToArray<T>());
		}

		/// <summary>
		/// Realiza um ZipInterator.
		/// </summary>
		/// <typeparam name="TFirst"></typeparam>
		/// <typeparam name="TSecond"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="resultSelector"></param>
		/// <returns></returns>
		private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			using (IEnumerator<TFirst> iteratorVariable0 = first.GetEnumerator())
			{
				using (IEnumerator<TSecond> iteratorVariable1 = second.GetEnumerator())
				{
					while (iteratorVariable0.MoveNext() && iteratorVariable1.MoveNext())
					{
						yield return resultSelector(iteratorVariable0.Current, iteratorVariable1.Current);
					}
				}
			}
		}

		/// <summary>
		/// Classe com método para tratar coleção somente leitura.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private static class DefaultReadOnlyCollection<T>
		{
			private static ReadOnlyCollection<T> defaultCollection;

			/// <summary>
			/// Instancia de uma coleção somente leitura vazia.
			/// </summary>
			public static ReadOnlyCollection<T> Empty
			{
				get
				{
					if(EnumerableExtensions.DefaultReadOnlyCollection<T>.defaultCollection == null)
					{
						EnumerableExtensions.DefaultReadOnlyCollection<T>.defaultCollection = new ReadOnlyCollection<T>(new T[0]);
					}
					return EnumerableExtensions.DefaultReadOnlyCollection<T>.defaultCollection;
				}
			}
		}

		/// <summary>
		/// Implementação de uma enumeração genérica.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private class GenericEnumerable<T> : IEnumerable<T>, IEnumerable
		{
			private readonly IEnumerable source;

			public GenericEnumerable(IEnumerable source)
			{
				this.source = source;
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				IEnumerator enumerator = this.source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T current = (T)enumerator.Current;
					yield return current;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.source.GetEnumerator();
			}
		}
	}
}
