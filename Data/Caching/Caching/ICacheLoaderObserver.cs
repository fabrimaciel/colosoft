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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Armazena os argumentos de um erro ocorrido no CacheLoader.
	/// </summary>
	public class CacheLoaderErrorEventArgs : Colosoft.Caching.CacheErrorEventArgs
	{
		private IMessageFormattable _message;

		/// <summary>
		/// Mensagem do erro ocorrido.
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Mensagem do erro.</param>
		/// <param name="error">Instancia do erro ocorrido.</param>
		public CacheLoaderErrorEventArgs(IMessageFormattable message, Exception error) : base(error)
		{
			_message = message;
		}
	}
	/// <summary>
	/// Assinatura do observer usado para monitorar a carga das entradads de dados do cache.
	/// </summary>
	public interface ICacheLoaderObserver : Colosoft.Net.IDownloaderObserver
	{
		/// <summary>
		/// Método acioando quando o processo for finalizado.
		/// </summary>
		/// <param name="e"></param>
		void OnLoadComplete(ApplicationLoaderCompletedEventArgs e);

		/// <summary>
		/// Define o progresso total.
		/// </summary>
		/// <param name="e"></param>
		void OnTotalProgressChanged(System.ComponentModel.ProgressChangedEventArgs e);

		/// <summary>
		/// Define o progresso do atual estágio.
		/// </summary>
		/// <param name="e"></param>
		void OnCurrentProgressChanged(System.ComponentModel.ProgressChangedEventArgs e);

		/// <summary>
		/// Acionado quando o estágio for alterado.
		/// </summary>
		/// <param name="state"></param>
		void OnStageChanged(CacheLoaderStage state);

		/// <summary>
		/// Método acionado quando ocorre um erro na carga do cache.
		/// </summary>
		/// <param name="e"></param>
		void OnLoadError(CacheLoaderErrorEventArgs e);
	}
}
