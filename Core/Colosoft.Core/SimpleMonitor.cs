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

namespace Colosoft.Threading
{
	/// <summary>
	/// Classe usada para monitorar o acesso de entrada em zonas criticas.
	/// </summary>
	public class SimpleMonitor : IDisposable, System.ComponentModel.INotifyPropertyChanged
	{
		private int _busyCount;

		/// <summary>
		/// Evento acionado quando alguma propriedade sofrer alterações.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Quantidade de solicitação do monitor.
		/// </summary>
		protected int BusyCount
		{
			get
			{
				return _busyCount;
			}
		}

		/// <summary>
		/// Identifica se a instancia está ocupada.
		/// </summary>
		public bool Busy
		{
			get
			{
				return (_busyCount > 0);
			}
		}

		/// <summary>
		/// Destrutor da classe
		/// </summary>
		~SimpleMonitor()
		{
			Dispose(false);
		}

		/// <summary>
		/// Dispara que uma propriedade for alterada.
		/// </summary>
		/// <param name="propertyName"></param>
		private void RaisePropertyChanged(string propertyName)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose()
		{
			Dispose(true);
			if(_busyCount > 0)
				_busyCount--;
			RaisePropertyChanged("Busy");
		}

		/// <summary>
		/// Identifica a entrada de algum operação.
		/// </summary>
		public void Enter()
		{
			InnerEnter();
			_busyCount++;
			RaisePropertyChanged("Busy");
		}

		/// <summary>
		/// Identifica a entrada de algum operação.
		/// </summary>
		protected virtual void InnerEnter()
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
