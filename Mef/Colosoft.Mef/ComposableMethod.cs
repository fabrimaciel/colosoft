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
using System.Diagnostics;
using Colosoft.Reflection;

namespace Colosoft.Mef
{
	/// <summary>
	/// Prover uma abstração para trabalhar com um método a nível de refleção.
	/// </summary>
	class ComposableMethod : ComposableMember
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="member"></param>
		public ComposableMethod(System.Reflection.MemberInfo member) : base(member)
		{
			var info = member as System.Reflection.MethodInfo;
			if(info == null)
				throw new InvalidOperationException("The specified value for the member parameter was not a MethodInfo instance.");
			this.Method = info;
			this.ValueGetter = getInstance => this.Method.CreateDelegate(getInstance);
		}

		/// <summary>
		/// Instancia do método aciado.
		/// </summary>
		public System.Reflection.MethodInfo Method
		{
			get;
			protected set;
		}

		/// <summary>
		/// Identifica se o tipo possui cargar tardia.
		/// </summary>
		public override bool IsLazyType
		{
			get
			{
				return this.ReturnType.IsLazyType();
			}
		}

		/// <summary>
		/// Identifica o valor pode ser recuperado do membro.
		/// </summary>
		public override bool IsReadable
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se é necessário uma instancia para o valor ser recuperado ou definido.
		/// </summary>
		public override bool IsInstanceNeeded
		{
			[DebuggerStepThrough]
			get
			{
				return !this.Method.IsStatic;
			}
		}

		/// <summary>
		/// Identifica se o valor pode ser definido para o membro.
		/// </summary>
		public override bool IsWritable
		{
			[DebuggerStepThrough]
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Tipo de retorno.
		/// </summary>
		public override Type ReturnType
		{
			[DebuggerStepThrough]
			get
			{
				return this.Method.ReturnType;
			}
		}
	}
}
