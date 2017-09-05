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
	/// Armazena as informações do Loader do serviço.
	/// </summary>
	class WCFServiceClientsLoader : IServiceClientsLoader
	{
		/// <summary>
		/// Instancia responsável por carregar a instancia do serviço.
		/// </summary>
		private Func<System.ServiceModel.ICommunicationObject> _loader;

		/// <summary>
		/// Tempo de duração da instancia criada.
		/// </summary>
		private int _cacheDurationInSeconds;

		/// <summary>
		/// Data que a instancia irá expirar.
		/// </summary>
		private DateTime _expireInstance;

		/// <summary>
		/// Instancia do cliente do serviço.
		/// </summary>
		private System.ServiceModel.ICommunicationObject _instance;

		/// <summary>
		/// Recupera a instancia do serviço.
		/// </summary>
		public object Instance
		{
			get
			{
				if(_instance == null || (_cacheDurationInSeconds > 0 && DateTime.Now > _expireInstance))
				{
					_instance = _loader();
					ResetExpiration();
				}
				else if(_instance.State == System.ServiceModel.CommunicationState.Faulted)
				{
					try
					{
						_instance.Abort();
						_instance.Close();
					}
					catch
					{
					}
					finally
					{
						_instance = null;
					}
					_instance = _loader();
					ResetExpiration();
				}
				return _instance;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public WCFServiceClientsLoader(Func<System.ServiceModel.ICommunicationObject> loader, int cacheDurationInSeconds)
		{
			_loader = loader;
			_cacheDurationInSeconds = cacheDurationInSeconds;
		}

		/// <summary>
		/// Reseta a data de expiração.
		/// </summary>
		private void ResetExpiration()
		{
			_expireInstance = DateTime.Now.AddSeconds(_cacheDurationInSeconds);
		}

		/// <summary>
		/// Reseta a instancia do Loader.
		/// </summary>
		public void Reset()
		{
			if(_instance != null)
			{
				try
				{
					_instance.Abort();
					_instance.Close();
				}
				catch
				{
				}
				finally
				{
					_instance = null;
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			if(_instance != null)
			{
				try
				{
					_instance.Abort();
					_instance.Close();
				}
				catch
				{
				}
				finally
				{
					_instance = null;
				}
			}
		}
	}
}
