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
using System.ComponentModel;

namespace Colosoft.Collections
{
	/// <summary>
	/// Wrapper para dados de lista assíncrona.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataWrapper<T> : INotifyPropertyChanged
	{
		private int _index;

		private bool _loadError = false;

		private T _data;

		private bool _isLoading = true;

		/// <summary>
		/// Evento de mudança de propriedade.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Índicie do dado.
		/// </summary>
		public int Index
		{
			get
			{
				return _index;
			}
			internal set
			{
				_index = value;
			}
		}

		/// <summary>
		/// Número do item.
		/// </summary>
		public int ItemNumber
		{
			get
			{
				return _index + 1;
			}
		}

		/// <summary>
		/// Define se o item está carregando.
		/// </summary>
		public bool IsLoading
		{
			get
			{
				return _isLoading;
			}
		}

		/// <summary>
		/// Define se ocorre o erro do wrapper.
		/// </summary>
		public bool LoadError
		{
			get
			{
				return _loadError;
			}
			set
			{
				_loadError = value;
			}
		}

		/// <summary>
		/// Dado do item contido no wrapper.
		/// </summary>
		public T Data
		{
			get
			{
				return _data;
			}
		}

		/// <summary>
		/// Define se o item está sendo utilizado.
		/// </summary>
		public bool IsInUse
		{
			get
			{
				return this.PropertyChanged != null;
			}
		}

		/// <summary>
		/// Contrutor do wrapper
		/// </summary>
		/// <param name="index">Índice do dado.</param>
		public DataWrapper(int index)
		{
			_index = index;
		}

		/// <summary>
		/// Cria uma instancia já com o dado inicializado.
		/// </summary>
		/// <param name="index">Índice do dado.</param>
		/// <param name="data">Instancia do dado.</param>
		public DataWrapper(int index, T data)
		{
			_index = index;
			_data = data;
			_isLoading = false;
		}

		/// <summary>
		/// Define a instancia com os dados.
		/// </summary>
		/// <param name="data"></param>
		public void SetData(T data)
		{
			_data = data;
			_isLoading = false;
			RaisePropertyChanged("Data", "IsLoading");
		}

		/// <summary>
		/// Dispara o evento de mudança de propriedade.
		/// </summary>
		/// <param name="propertyNames">Nomes das propriedades.</param>
		private void RaisePropertyChanged(params string[] propertyNames)
		{
			var handler = this.PropertyChanged;
			if(handler != null)
				foreach (var i in propertyNames)
					handler(this, new PropertyChangedEventArgs(i));
		}
	}
}
