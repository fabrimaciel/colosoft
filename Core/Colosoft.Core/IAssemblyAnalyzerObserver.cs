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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Observer utilizado na analize.
	/// </summary>
	public interface IAssemblyAnalyzerObserver
	{
		/// <summary>
		/// Atual progresso.
		/// </summary>
		int Progress
		{
			get;
			set;
		}

		/// <summary>
		/// Texto atual.
		/// </summary>
		IMessageFormattable Text
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a operação está pendente para ser cancelada.
		/// </summary>
		bool CancellationPending
		{
			get;
		}

		/// <summary>
		/// Reporta o progresso.
		/// </summary>
		/// <param name="progressPercentage"></param>
		/// <param name="actionText">Texto da ação.</param>
		void ReportProgress(int progressPercentage, IMessageFormattable actionText);

		/// <summary>
		/// Reporta uma mensagem de erro.
		/// </summary>
		/// <param name="message"></param>
		void ReportErrorMessage(IMessageFormattable message);
	}
}
