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

namespace Colosoft.Mef
{
	/// <summary>
	/// Armazena as informações de um membro do tipo campo.
	/// </summary>
	class ComposableField : ComposableMember
	{
		/// <summary>
		/// Instancia do campo associado.
		/// </summary>
		public System.Reflection.FieldInfo Field
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
				return !this.Field.IsStatic;
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
				return !this.Field.IsInitOnly && !this.Field.IsLiteral;
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
				return this.Field.FieldType;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="member"></param>
		public ComposableField(System.Reflection.MemberInfo member) : base(member)
		{
			var info = member as System.Reflection.FieldInfo;
			if(info == null)
				throw new InvalidOperationException("The specified value for the member parameter was not a FieldInfo instance.");
			this.Field = info;
			this.ValueGetter = getInstance => this.Field.GetValue(getInstance);
			this.ValueSetter = (setInstance, setValue) => this.Field.SetValue(setInstance, setValue);
		}
	}
}
