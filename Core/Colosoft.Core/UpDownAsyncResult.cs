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
	/// Estágios do download.
	/// </summary>
	public enum DownloadStage
	{
		/// <summary>
		/// Desconhecido.
		/// </summary>
		Unknown,
		/// <summary>
		/// Criação da requisição.
		/// </summary>
		RequestCreation,
		/// <summary>
		/// Requisição iniciada.
		/// </summary>
		RequestStart,
		/// <summary>
		/// Download iniciado.
		/// </summary>
		DownloadStarted,
		/// <summary>
		/// Download em progresso.
		/// </summary>
		DownloadInProgress,
		/// <summary>
		/// Gravação local em processo.
		/// </summary>
		LocalIOInProgress,
		/// <summary>
		/// Move e resetando atributos
		/// </summary>
		MoveAndAttributesReset
	}
	/// <summary>
	/// Implementação da resultado de uma operação assincrona de upload e download.
	/// </summary>
	public class UpDownAsyncResult : IAsyncResult, IDisposable
	{
		private Exception _exception;

		private bool _isCompleted;

		private System.IO.Stream _resultStream;

		private DownloadStage _stage;

		private object _stateObject;

		private AsyncCallback _userCallback;

		private System.Threading.ManualResetEvent _waitHandle;

		/// <summary>
		/// Erro ocorrido.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
			set
			{
				_exception = value;
			}
		}

		/// <summary>
		/// Estado.
		/// </summary>
		public object AsyncState
		{
			get
			{
				return _stateObject;
			}
		}

		/// <summary>
		/// Manipulador de espera.
		/// </summary>
		public System.Threading.WaitHandle AsyncWaitHandle
		{
			get
			{
				if(_waitHandle == null)
				{
					lock (this)
						if(_waitHandle == null)
							_waitHandle = new System.Threading.ManualResetEvent(_isCompleted);
				}
				return _waitHandle;
			}
		}

		/// <summary>
		/// Identifica se será completado de forma sincronizada.
		/// </summary>
		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Identifica se foi executado
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				return _isCompleted;
			}
		}

		/// <summary>
		/// Stream do resultado.
		/// </summary>
		public System.IO.Stream ResultStream
		{
			get
			{
				return _resultStream;
			}
		}

		/// <summary>
		/// Estágio do download.
		/// </summary>
		public DownloadStage Stage
		{
			get
			{
				return _stage;
			}
			set
			{
				_stage = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="userCallback"></param>
		/// <param name="stateObject"></param>
		public UpDownAsyncResult(AsyncCallback userCallback, object stateObject)
		{
			_userCallback = userCallback;
			_stateObject = stateObject;
		}

		/// <summary>
		/// Identifica que a operação foir finalizada.
		/// </summary>
		/// <param name="asyncException"></param>
		/// <param name="resultStream"></param>
		public void Completed(Exception asyncException, System.IO.Stream resultStream)
		{
			_exception = asyncException;
			_resultStream = resultStream;
			if(_exception != null)
			{
			}
			try
			{
				lock (this)
				{
					_isCompleted = true;
					if(_waitHandle != null)
						_waitHandle.Set();
				}
			}
			catch(Exception)
			{
			}
			if(_userCallback != null)
				_userCallback(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_waitHandle != null)
				_waitHandle.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
	}
}
