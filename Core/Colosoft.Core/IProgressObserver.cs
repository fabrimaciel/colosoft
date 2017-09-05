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

namespace Colosoft.Progress
{
	/// <summary>
	/// Contrato para acompanhadores de progresso.
	/// </summary>
	public interface IProgressObserver
	{
		/// <summary>
		/// Acionado quando o trabalho de processamento é iniciado.
		/// </summary>
		void OnStart(object userState);

		/// <summary>
		/// Define o progresso do processamento de dados.
		/// </summary>
		/// <param name="e"></param>
		void OnProgressChanged(ProgressChangedEventArgs e);

		/// <summary>
		/// Acionado quando o progresso é finalizado.
		/// </summary>
		/// <param name="e"></param>
		void OnProgressCompleted(ProgressCompletedEventArgs e);
	}
	/// <summary>
	/// Assinatura do observer de progresso com suporte a mensagem.
	/// </summary>
	public interface IProgressWithMessageObserver : IProgressObserver
	{
		/// <summary>
		/// Método acionado quando o estado da mensagem for alterado.
		/// </summary>
		/// <param name="e"></param>
		void OnProgressMessageChanged(Colosoft.Progress.ProgressMessageChangedEventArgs e);
	}
}
