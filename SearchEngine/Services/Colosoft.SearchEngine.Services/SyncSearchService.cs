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
using System.ServiceModel;

namespace Colosoft.SearchEngine.Services
{
	/// <summary>
	/// Representa o serviço de sincronização dos dados da pesquisa.
	/// </summary>
	public class SyncSearchService : IDisposable
	{
		private readonly object lockObject = new object();

		private bool _isRun = false;

		/// <summary>
		/// Instancia do gerenciador das pesquisas;
		/// </summary>
		private ISearcherMaintenance _maintenance;

		private System.Threading.Thread _managerThread;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SyncSearchService(ISearcherMaintenance maintenance)
		{
			if(maintenance == null)
				throw new ArgumentNullException("maintenance");
			_maintenance = maintenance;
		}

		/// <summary>
		/// Método do robo que fica realizando a manutenção dos dados do pesquisador.
		/// </summary>
		private void Robot()
		{
			try
			{
				_isRun = true;
				while (_isRun)
				{
					try
					{
						_maintenance.ProcessRemovedElements();
					}
					catch(Exception ex)
					{
						System.Diagnostics.Trace.WriteLine("Falha ao processar os elementos para serem removidos. Ex: " + ex.Message);
						System.Diagnostics.EventLog.WriteEntry("SearchService", "Falha ao processar os elementos para serem removidos. Ex: " + ex.Message);
					}
					try
					{
						_maintenance.ProcessNewElements();
					}
					catch(Exception ex)
					{
						System.Diagnostics.Trace.WriteLine("Falha ao processar os novos elementos. Ex: " + ex.Message);
					}
					System.Threading.Thread.Sleep(30000);
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
		}

		/// <summary>
		/// Inicia o robo.
		/// </summary>
		public void Start()
		{
			if(_managerThread == null)
			{
				lock (lockObject)
				{
					if(_managerThread == null)
					{
						_managerThread = new System.Threading.Thread(this.Robot);
						_managerThread.Start();
					}
				}
			}
		}

		/// <summary>
		/// Para o robo.
		/// </summary>
		public void Stop()
		{
			lock (lockObject)
				try
				{
					_isRun = false;
					_managerThread.Abort();
				}
				catch
				{
				}
				finally
				{
					_managerThread = null;
				}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Stop();
		}
	}
}
