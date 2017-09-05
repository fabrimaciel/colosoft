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
using GDA;

namespace Colosoft.Data.Database.Generic
{
	/// <summary>
	/// Implementação de transação usando o GDA
	/// </summary>
	public class PersistenceTransactionExecuter : IPersistenceTransactionExecuter
	{
		private GDATransaction _transaction;

		private bool _isDisposed;

		/// <summary>
		/// Evento acionado antes de executar o commit da transação.
		/// </summary>
		public event EventHandler Committing;

		/// <summary>
		/// Evento acionado depois do commit for realizado.
		/// </summary>
		public event EventHandler Commited;

		/// <summary>
		/// Evento acionado antes do rollback.
		/// </summary>
		public event EventHandler Rollbacking;

		/// <summary>
		/// Evento acionado depois de ocorrer o rollback da transação.
		/// </summary>
		public event EventHandler Rollbacked;

		/// <summary>
		/// Nome do provedor de conexão.
		/// </summary>
		public string ProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Configuração do provedor.
		/// </summary>
		public GDA.Interfaces.IProviderConfiguration ProviderConfiguration
		{
			get;
			set;
		}

		/// <summary>
		/// Verifica se a instancia foi liberada.
		/// </summary>
		private void CheckDisposed()
		{
			if(_isDisposed)
				throw new ObjectDisposedException("PersistenceTransactionExecuter");
		}

		/// <summary>
		/// Transação real do GDA
		/// </summary>
		public GDATransaction Transaction
		{
			get
			{
				CheckDisposed();
				if(_transaction == null)
				{
					GDATransaction transaction = null;
					transaction = new GDATransaction(ProviderConfiguration ?? GDASettings.GetProviderConfiguration(ProviderName));
					try
					{
						transaction.BeginTransaction();
					}
					catch
					{
						transaction.Dispose();
						throw;
					}
					_transaction = transaction;
				}
				return _transaction;
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~PersistenceTransactionExecuter()
		{
			Dispose(false);
		}

		/// <summary>
		/// Da commit na transação
		/// </summary>
		public void Commit()
		{
			CheckDisposed();
			if(Committing != null)
				Committing(this, EventArgs.Empty);
			if(_transaction != null)
				_transaction.Commit();
			if(Commited != null)
				Commited(this, EventArgs.Empty);
		}

		/// <summary>
		/// Da rollback na transação
		/// </summary>
		public void Rollback()
		{
			CheckDisposed();
			if(Rollbacking != null)
				Rollbacking(this, EventArgs.Empty);
			if(_transaction != null)
				_transaction.Rollback();
			if(Rollbacked != null)
				Rollbacked(this, EventArgs.Empty);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_transaction != null)
				_transaction.Dispose();
			_transaction = null;
			_isDisposed = true;
		}

		/// <summary>
		/// Da dispose na transação
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
