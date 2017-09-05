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

namespace Colosoft.Collections
{
	/// <summary>
	/// Assinatura da classe responsável por registrar um observer.
	/// </summary>
	public interface INotifyCollectionChangedObserverRegister
	{
		/// <summary>
		/// Registra os observers no container.
		/// </summary>
		/// <param name="container"></param>
		void Register(INotifyCollectionChangedObserverContainer container);
	}
	/// <summary>
	/// Implementação usada para realiza um wrapper do enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NotifyCollectionChangedObserverRegisterEnumerable<T> : IEnumerable<T>, INotifyCollectionChangedObserverRegister
	{
		private System.Collections.IEnumerable _enumerable;

		private Func<object, T> _converter;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumerable"></param>
		/// <param name="converter">Instancia usada para converter o item do enumerador.</param>
		public NotifyCollectionChangedObserverRegisterEnumerable(System.Collections.IEnumerable enumerable, Func<object, T> converter = null)
		{
			_enumerable = enumerable;
			_converter = converter;
		}

		/// <summary>
		/// Método usado para regitrar o container.
		/// </summary>
		/// <param name="container"></param>
		public void Register(INotifyCollectionChangedObserverContainer container)
		{
			if(_enumerable is INotifyCollectionChangedObserverRegister)
				((INotifyCollectionChangedObserverRegister)_enumerable).Register(container);
		}

		/// <summary>
		/// Recupera o enumerador.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			foreach (var i in _enumerable)
				yield return (_converter != null ? _converter(i) : (T)i);
		}

		/// <summary>
		/// Recupera o enumerador.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
