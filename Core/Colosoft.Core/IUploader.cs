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

namespace Colosoft.Net
{
	/// <summary>
	/// Resultado a operação de upload.
	/// </summary>
	public class UploaderOperationResult
	{
		private Exception _error;

		private bool _canceled;

		/// <summary>
		/// Erro ocorrido na operação.
		/// </summary>
		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Identifica se a operação foi cancelada.
		/// </summary>
		public bool Canceled
		{
			get
			{
				return _canceled;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="error">Erro ocorrido.</param>
		/// <param name="canceled">Identifica se a operação de upload foi cancelada.</param>
		public UploaderOperationResult(Exception error, bool canceled)
		{
			_error = error;
			_canceled = canceled;
		}
	}
	/// <summary>
	/// Assinatura da classe responsável for fazer upload de dados
	/// para o servidor.
	/// </summary>
	public interface IUploader
	{
		/// <summary>
		/// Evento disparado quando o progresso do upload se altera.
		/// </summary>
		event UploadProgressEventHandler ProgressChanged;

		/// <summary>
		/// Evento disparado quando o upload for finalizado.
		/// </summary>
		event UploadCompletedEventHandler Completed;

		/// <summary>
		/// Observadores da instancia.
		/// </summary>
		AggregateUploaderObserver Observers
		{
			get;
		}

		/// <summary>
		/// Relação dos itens do Uploader.
		/// </summary>
		List<IUploaderItem> Items
		{
			get;
		}

		/// <summary>
		/// Identifica se a instancia está ocupada.
		/// </summary>
		bool IsBusy
		{
			get;
		}

		/// <summary>
		/// Cancela o atual upload.
		/// </summary>
		void Cancel();

		/// <summary>
		/// Inicializa o processo de upload.
		/// </summary>
		/// <param name="callback">Método de retorno.</param>
		/// <param name="state"></param>
		/// <returns></returns>
		IAsyncResult BeginUpload(AsyncCallback callback, object state);

		/// <summary>
		/// Recupera o resultado a operação assincrona do upload.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns></returns>
		UploaderOperationResult EndUpload(IAsyncResult ar);
	}
}
