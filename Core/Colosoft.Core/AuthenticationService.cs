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
	/// Assinatura do evento acionado quando o nome do serviço de autenticação 
	/// for alterado.
	/// </summary>
	public delegate void AutheticationServiceNameChangedHandler ();
	/// <summary>
	/// Armazena as informações do serviço de autenticação.
	/// </summary>
	public class AuthenticationService
	{
		private static string _name;

		/// <summary>
		/// Evento acionado quando o nome do serviço de autenticação for alterado.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public static event AutheticationServiceNameChangedHandler NameChanged;

		/// <summary>
		/// Nome do serviço de autenticação.
		/// </summary>
		public static string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(_name != value)
				{
					_name = value;
					if(NameChanged != null)
						NameChanged();
				}
			}
		}
	}
}
