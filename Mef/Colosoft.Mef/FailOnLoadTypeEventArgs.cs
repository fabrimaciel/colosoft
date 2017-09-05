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

namespace Colosoft.Mef
{
	/// <summary>
	/// Armazena os dados do argumento da falha para a carga de um tipo.
	/// </summary>
	public class FailOnLoadTypeEventArgs : EventArgs
	{
		/// <summary>
		/// Nome do tipo;
		/// </summary>
		public Colosoft.Reflection.TypeName Type
		{
			get;
			set;
		}

		/// <summary>
		/// Erro ocorrido.
		/// </summary>
		public Exception Error
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="exception"></param>
		public FailOnLoadTypeEventArgs(Colosoft.Reflection.TypeName type, Exception exception)
		{
			this.Type = type;
			this.Error = exception;
		}
	}
	/// <summary>
	/// Representa os eventos acionados quando ocorre uma falha ao carregar um tipo.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void FailOnLoadTypeHandler (object sender, FailOnLoadTypeEventArgs e);
}
