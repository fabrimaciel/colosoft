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
using Colosoft.Validation;

namespace Colosoft
{
	/// <summary>
	/// 
	/// </summary>
	public static class ValidatorOfTExtensions
	{
		/// <summary>
		/// Aplica a validação para verificar se o objeto é o tipo experado.
		/// </summary>
		/// <typeparam name="T">Tipo do argumento que será validado.</typeparam>
		/// <param name="target">Instancia do objeto que será validado.</param>
		/// <param name="desiredType">O <see cref="Type"/> para verifica se o tipo é compatível.</param>
		/// <exception cref="InvalidCastException">Caso o target não seja do tipo especificado pelo parametro <paramref name="desiredType"/>.</exception>
		public static NativeValidator<T> IsOfType<T>(this NativeValidator<T> target, Type desiredType)
		{
			if(target.Value.GetType() != desiredType)
			{
				throw new InvalidCastException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidationErrors_IsOfType, target.Name, desiredType.FullName).Format());
			}
			return target;
		}

		/// <summary>
		/// Aplica a validação para verifica se a string não é vazia. 
		/// </summary>
		/// <param name="target">Instancia do objeto que será validado.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Caso o tamanho da string seja zero.</exception>
		[System.Diagnostics.DebuggerHidden]
		public static NativeValidator<string> NotEmpty(this NativeValidator<string> target)
		{
			if(target.Value.Length == 0)
			{
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidationErrors_NotEmpty, target.Name).Format());
			}
			return target;
		}

		/// <summary>
		/// Aplica a validação para verifica se a instancia informada não é nula.
		/// </summary>
		/// <typeparam name="T">Tipo do argumento que será validado.</typeparam>
		/// <param name="target">Instancia do objeto que será validado.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Caso a instancia seja nula.</exception>
		[System.Diagnostics.DebuggerHidden]
		public static NativeValidator<T> NotNull<T>(this NativeValidator<T> target)
		{
			if(target.Value == null)
			{
				throw new ArgumentNullException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidationErrors_NotNull, target.Name).Format());
			}
			return target;
		}

		/// <summary>
		/// Aplica a validação para verificar se o vetor informado está vazio.
		/// </summary>
		/// <typeparam name="T">Tipo do vetor que será validado.</typeparam>
		/// <param name="target">Instancia do objeto que será validado.</param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerHidden]
		public static NativeValidator<T> NotEmptyCollection<T>(this NativeValidator<T> target) where T : System.Collections.ICollection
		{
			if(target.Value.Count == 0)
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidationErrors_NotEmptyCollection, target.Name).Format());
			return target;
		}

		/// <summary>
		/// Aplica a validação para verificar se o valor menor que o intervalo minimo.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="minimum"></param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerHidden]
		public static NativeValidator<int> CheckForOutOfRange(this NativeValidator<int> target, int minimum)
		{
			if(target.Value < minimum)
				throw new ArgumentOutOfRangeException(target.Name, target.Value, ResourceMessageFormatter.Create(() => Properties.Resources.ValidationErrors_OutOfRange, target.Name).Format());
			return target;
		}
	}
}
