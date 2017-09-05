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

namespace Colosoft.Data
{
	/// <summary>
	/// Assinatura do evento acionado quando a execução de uma sessão de persistencia for concluída.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void PersistenceSessionExecutedHandler (object sender, PersistenceSessionExecutedEventArgs e);
	/// <summary>
	/// Armazena os argumentos da execução de uma sessão de persistencia.
	/// </summary>
	public class PersistenceSessionExecutedEventArgs : EventArgs
	{
		/// <summary>
		/// Resultado da execução das ações.
		/// </summary>
		public ExecuteActionsResult Result
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result">Resultado da execução.</param>
		public PersistenceSessionExecutedEventArgs(ExecuteActionsResult result)
		{
			this.Result = result;
		}
	}
}
