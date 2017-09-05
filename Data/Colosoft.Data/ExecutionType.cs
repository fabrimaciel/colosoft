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
	/// Determina o tipo de execução da operação de persistência.
	/// </summary>
	public enum ExecutionType : int
	{
		/// <summary>
		/// Execução padrão.
		/// </summary>
		Default = 1,
		/// <summary>
		/// Contiua a executar as ações de persistência mesmo após um erro e da rollback no final.
		/// </summary>
		ContinueOnError = 2,
		/// <summary>
		/// Executa as operações de persistência e da rollback independente de sucesso ou falha. 
		/// </summary>
		ExecuteAndRollback = 3
	}
}
