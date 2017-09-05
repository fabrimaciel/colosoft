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

namespace Colosoft.Query
{
	/// <summary>
	/// Implementação da estratégia de vinculação dinâmica.
	/// </summary>
	public class DynamicBindStrategy<TModel> : IQueryResultBindStrategy
	{
		private Func<IEnumerable<IRecord>, BindStrategyMode, IEnumerable<TModel>> _func;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="func">Ponteiro do método que será usado na vinculação.</param>
		public DynamicBindStrategy(Func<IEnumerable<IRecord>, BindStrategyMode, IEnumerable<TModel>> func)
		{
			func.Require("func").NotNull();
			_func = func;
		}

		/// <summary>
		/// Cria uma sessão de vinculação.
		/// </summary>
		/// <param name="recordDescriptor"></param>
		/// <returns></returns>
		IQueryResultBindStrategySession IQueryResultBindStrategy.CreateSession(Record.RecordDescriptor recordDescriptor)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		IEnumerable<string> IQueryResultBindStrategy.Bind(IRecord record, BindStrategyMode mode, ref object instance)
		{
			var enumerator = _func(new[] {
				record
			}, mode).GetEnumerator();
			try
			{
				if(enumerator.MoveNext())
					instance = enumerator.Current;
			}
			finally
			{
				if(enumerator is IDisposable)
					((IDisposable)enumerator).Dispose();
			}
			return new string[0];
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado na operação.</typeparam>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		IEnumerable<string> IQueryResultBindStrategy.Bind<T>(IRecord record, BindStrategyMode mode, ref T instance)
		{
			var enumerator = _func(new[] {
				record
			}, mode).GetEnumerator();
			try
			{
				if(enumerator.MoveNext())
					instance = (T)(object)enumerator.Current;
			}
			finally
			{
				if(enumerator is IDisposable)
					((IDisposable)enumerator).Dispose();
			}
			return new string[0];
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos na enumeração de registro informados
		/// e retorna uma enumeração das instancias preechidas.
		/// </summary>
		/// <param name="records">Registros que serão processados.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="creator">Instancia responsável pela criação dos objetos.</param>
		/// <returns></returns>
		System.Collections.IEnumerable IQueryResultBindStrategy.Bind(IEnumerable<IRecord> records, BindStrategyMode mode, IQueryResultObjectCreator creator)
		{
			return _func(records, mode);
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos na enumeração de registro informados
		/// e retorna uma enumeração das instancias preechidas.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado na operação.</typeparam>
		/// <param name="records">Registros que serão processados.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="creator">Instancia responsável pela criação dos objetos.</param>
		/// <returns></returns>
		IEnumerable<T> IQueryResultBindStrategy.Bind<T>(IEnumerable<IRecord> records, BindStrategyMode mode, IQueryResultObjectCreator creator)
		{
			return new Enumerable<T>(_func(records, mode));
		}

		/// <summary>
		/// Implementação para encapsular um Enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class Enumerable<T> : IEnumerable<T>, IDisposable
		{
			private System.Collections.IEnumerable _enumerable;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerable"></param>
			public Enumerable(System.Collections.IEnumerable enumerable)
			{
				_enumerable = enumerable;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~Enumerable()
			{
				Dispose();
			}

			/// <summary>
			/// Recupera o enumerador da instancia.
			/// </summary>
			/// <returns></returns>
			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return new Enumerator<T>(_enumerable.GetEnumerator());
			}

			/// <summary>
			/// Recupera o enumerador da instancia.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return new Enumerator<T>(_enumerable.GetEnumerator());
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_enumerable is IDisposable)
					((IDisposable)_enumerable).Dispose();
			}
		}

		/// <summary>
		/// Implementação base do Enumerado encapsulado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		class Enumerator<T> : IEnumerator<T>, IDisposable
		{
			private System.Collections.IEnumerator _enumerator;

			/// <summary>
			/// Current.
			/// </summary>
			public T Current
			{
				get
				{
					return (T)_enumerator.Current;
				}
			}

			/// <summary>
			/// Current.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _enumerator.Current;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="enumerator"></param>
			public Enumerator(System.Collections.IEnumerator enumerator)
			{
				_enumerator = enumerator;
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~Enumerator()
			{
				Dispose();
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_enumerator is IDisposable)
					((IDisposable)_enumerator).Dispose();
			}

			/// <summary>
			/// Move para o próximo item.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			/// <summary>
			/// Reset.
			/// </summary>
			public void Reset()
			{
				_enumerator.Reset();
			}
		}
	}
}
