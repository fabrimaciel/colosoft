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

namespace Colosoft.Reports
{
	/// <summary>
	/// Severidade do erro.
	/// </summary>
	public enum Severity
	{
		/// <summary>
		/// Warning
		/// </summary>
		Warning = 0,
		/// <summary>
		/// Error
		/// </summary>
		Error = 1,
	}
	/// <summary>
	/// Armzena as informações de uma alerta na geração do relatório.
	/// </summary>
	public class Warning
	{
		/// <summary>
		/// Código do erro assinalado.
		/// </summary>
		public string Code
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem descrevendo o erro.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do objeto no qual foi atribuido o Warning.
		/// </summary>
		public string ObjectName
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do objeto
		/// </summary>
		public string ObjectType
		{
			get;
			set;
		}

		/// <summary>
		/// Severidade o acontecido.
		/// </summary>
		public Severity Severity
		{
			get;
			set;
		}
	}
}
