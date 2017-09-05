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

namespace Colosoft.Domain
{
	/// <summary>
	/// Implementação para a referencia de um <see cref="Delegate"/>.
	/// </summary>
	public class DelegateReference : IDelegateReference
	{
		private readonly Delegate _delegate;

		private readonly WeakReference _weakReference;

		private readonly System.Reflection.MethodInfo _method;

		private readonly Type _delegateType;

		/// <summary>
		/// Recupera o objeto <see cref="Delegate"/> referenciado.
		/// </summary>
		public Delegate Target
		{
			get
			{
				if(_delegate != null)
					return _delegate;
				else
					return TryGetDelegate();
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="delegate">Instancia do original do <see cref="Delegate"/> para criar uma referencia..</param>
		/// <param name="keepReferenceAlive"><see langword="false" /> se for para criar uma referencia fraca para o delegate, permitindo o garbage collected. 
		/// Ou mantem uma referancia forte para a instancia.</param>
		public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
		{
			@delegate.Require("delegate").NotNull();
			if(keepReferenceAlive)
				this._delegate = @delegate;
			else
			{
				_weakReference = new WeakReference(@delegate.Target);
				_method = @delegate.Method;
				_delegateType = @delegate.GetType();
			}
		}

		/// <summary>
		/// Tenta recupera a instancia do delegate.
		/// </summary>
		/// <returns></returns>
		private Delegate TryGetDelegate()
		{
			if(_method.IsStatic)
				return Delegate.CreateDelegate(_delegateType, null, _method);
			object target = _weakReference.Target;
			if(target != null)
				return Delegate.CreateDelegate(_delegateType, target, _method);
			return null;
		}
	}
}
