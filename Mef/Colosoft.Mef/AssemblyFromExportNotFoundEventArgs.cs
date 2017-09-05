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
	/// Armazena os dados dos argumentos do evento 
	/// acionado quando o assembly não é encontrado.
	/// </summary>
	public class AssemblyFromExportNotFoundEventArgs : EventArgs
	{
		/// <summary>
		/// <see cref="Colosoft.Reflection.Composition.IExport"/> associado.
		/// </summary>
		public Colosoft.Reflection.Composition.IExport Export
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do assembly não encontrado.
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
		/// Identifica se o erro foi tratado.
		/// </summary>
		public bool IsErrorHandled
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="export"></param>
		/// <param name="assemblyName"></param>
		/// <param name="error"></param>
		public AssemblyFromExportNotFoundEventArgs(Colosoft.Reflection.Composition.IExport export, System.Reflection.AssemblyName assemblyName, Exception error)
		{
			this.Export = export;
			this.AssemblyName = assemblyName;
			this.Error = error;
		}
	}
	/// <summary>
	/// Representa os eventos acionados quando o assembly do <see cref="Colosoft.Reflection.Composition.IExport"/> 
	/// não é encontrado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void AssemblyFromExportNotFoundHandler (object sender, AssemblyFromExportNotFoundEventArgs e);
}
