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
using System.Collections;

namespace Colosoft.Caching
{
	/// <summary>
	/// Representa um conjunto de recursos.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class ResourcePool : IDisposable
	{
		private Hashtable _resourceTable = new Hashtable();

		/// <summary>
		/// Quantidade de items no pool.
		/// </summary>
		public int Count
		{
			get
			{
				return _resourceTable.Count;
			}
		}

		/// <summary>
		/// Chaves dos recursos da instancia.
		/// </summary>
		public ICollection Keys
		{
			get
			{
				return _resourceTable.Keys;
			}
		}

		/// <summary>
		/// Adiciona um novo recurso.
		/// </summary>
		/// <param name="key">Chave que identifica o recurso.</param>
		/// <param name="value">Valor do recurso.</param>
		public void AddResource(object key, object value)
		{
			ResourceInfo info = _resourceTable[key] as ResourceInfo;
			if(info != null)
			{
				if(value != null)
					info.Object = value;
			}
			else
				info = new ResourceInfo(value);
			_resourceTable[key] = info;
			info.AddRef();
		}

		/// <summary>
		/// Adiciona um novo recurso.
		/// </summary>
		/// <param name="key">Chave que identifica o recurso.</param>
		/// <param name="value">Valor do recurso.</param>
		/// <param name="numberOfCallbacks">Número de chamadas de retorno para o recurso adicionado.</param>
		public void AddResource(object key, object value, int numberOfCallbacks)
		{
			ResourceInfo info = _resourceTable[key] as ResourceInfo;
			if(info != null)
			{
				if(value != null)
					info.Object = value;
			}
			else
			{
				info = new ResourceInfo(value);
			}
			_resourceTable[key] = info;
			for(int i = 0; i < numberOfCallbacks; i++)
				info.AddRef();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public void Dispose()
		{
			lock (this)
			{
				IDictionaryEnumerator enumerator = _resourceTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ResourceInfo info = (ResourceInfo)enumerator.Value;
					this.DisposeResource(info.Object);
				}
			}
		}

		/// <summary>
		/// Libera a instancia de um recurso.
		/// </summary>
		/// <param name="res">Instancia que será liberada.</param>
		private void DisposeResource(object res)
		{
			if(res is IDisposable)
			{
				try
				{
					((IDisposable)res).Dispose();
				}
				catch(Exception)
				{
				}
			}
		}

		/// <summary>
		/// Recupera o valor do recurso pela chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetResource(object key)
		{
			ResourceInfo info = _resourceTable[key] as ResourceInfo;
			if(info != null)
				return info.Object;
			return null;
		}

		/// <summary>
		/// Remove todas os recursos do Pool.
		/// </summary>
		public void RemoveAllResources()
		{
			lock (this)
			{
				IEnumerator enumerator = _resourceTable.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ResourceInfo info = _resourceTable[enumerator.Current] as ResourceInfo;
					if(info != null)
						DisposeResource(info.Object);
				}
				_resourceTable.Clear();
			}
		}

		/// <summary>
		/// Remove o recurso associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave que representa o recuros.</param>
		/// <returns>Instancia do recurso removido.</returns>
		public object RemoveResource(object key)
		{
			ResourceInfo info = _resourceTable[key] as ResourceInfo;
			if(info == null)
				return null;
			if(info.Release() == 0)
			{
				_resourceTable.Remove(key);
				DisposeResource(info.Object);
			}
			return info.Object;
		}

		/// <summary>
		/// Remove o recurso associado com a chave informada.
		/// </summary>
		/// <param name="key">Chave que representa o recuros.</param>
		/// <param name="numberOfCallbacks">Número de chamadas de retorno que serão aplicadas.</param>
		/// <returns>Instancia do recurso removido.</returns>
		public object RemoveResource(object key, int numberOfCallbacks)
		{
			ResourceInfo info = _resourceTable[key] as ResourceInfo;
			if(info == null)
				return null;
			for(int i = 0; i < numberOfCallbacks; i++)
			{
				if(info.Release() == 0)
				{
					_resourceTable.Remove(key);
					DisposeResource(info.Object);
				}
			}
			return info.Object;
		}

		/// <summary>
		/// Remove o recurso com a chave informada.
		/// </summary>
		/// <param name="key"></param>
		public void RemoveSeveredResource(object key)
		{
			ResourceInfo info = _resourceTable[key] as ResourceInfo;
			if(info != null)
			{
				_resourceTable.Remove(key);
				this.DisposeResource(info.Object);
			}
		}

		/// <summary>
		/// Representa as informações do recurso.
		/// </summary>
		public class ResourceInfo
		{
			private int _refCount;

			private object _resource;

			/// <summary>
			/// Objeto que representa o recurso.
			/// </summary>
			public object Object
			{
				get
				{
					return _resource;
				}
				set
				{
					_resource = value;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="resource"></param>
			public ResourceInfo(object resource)
			{
				_resource = resource;
			}

			/// <summary>
			/// Incrementa uma nova referencia para o recurso.
			/// </summary>
			/// <returns></returns>
			public int AddRef()
			{
				return ++_refCount;
			}

			/// <summary>
			/// Libera uma referencia para o recurso.
			/// </summary>
			/// <returns></returns>
			public int Release()
			{
				if(_refCount > 0)
					_refCount--;
				return _refCount;
			}
		}
	}
}
