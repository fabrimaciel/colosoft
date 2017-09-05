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

namespace Colosoft.Security.Authentication
{
	/// <summary>
	/// Objeto com o provedor de acesso
	/// </summary>
	public class IdentityProvider
	{
		private Type _type;

		private string _typeString;

		/// <summary>
		/// Identificador
		/// </summary>
		public int IdentityProviderId
		{
			get;
			set;
		}

		/// <summary>
		/// Nome
		/// </summary>
		public string FullName
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo aonde está implementado o provedor
		/// </summary>
		public Type Type
		{
			get
			{
				return _type ?? Type.GetType(TypeString);
			}
			set
			{
				if(value != null)
					_typeString = value.FullName;
				else
					_typeString = null;
				_type = value;
			}
		}

		/// <summary>
		/// Strnig com a definição do tipo
		/// </summary>
		public string TypeString
		{
			get
			{
				return _typeString;
			}
			set
			{
				_typeString = value;
				_type = null;
			}
		}

		/// <summary>
		/// Dias de antecedência para o alerta de alteração de senha
		/// </summary>
		public int WarningDays
		{
			get;
			set;
		}
	}
}
