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
	/// Armazena os argumentos do evento acionado quando ocorre um falha ao carregar o assembly.
	/// </summary>
	public class FailOnLoadAssemblyArgs : EventArgs
	{
		/// <summary>
		/// Nome do assembly.
		/// </summary>
		public System.Reflection.AssemblyName AssemblyName
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
		/// <param name="assemblyName">Nome do assembly.</param>
		/// <param name="error">Erro ocorrido.</param>
		public FailOnLoadAssemblyArgs(System.Reflection.AssemblyName assemblyName, Exception error)
		{
			this.AssemblyName = assemblyName;
			this.Error = error;
		}
	}
	/// <summary>
	/// Representa os eventos acioando quando ocorre um erro ao carregar o assembly.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void FailOnLoadAssemblyHandler (object sender, FailOnLoadAssemblyArgs e);
}
