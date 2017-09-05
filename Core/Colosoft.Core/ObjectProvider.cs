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

namespace Colosoft.Runtime
{
	/// <summary>
	/// Classe responsável por prover instancia de objetos.
	/// </summary>
	public abstract class ObjectProvider
	{
		private Hashtable _available;

		private ArrayList _availableRentIds;

		/// <summary>
		/// Tamanho inicial.
		/// </summary>
		protected int _initialSize;

		/// <summary>
		/// Tipo do objeto associado.
		/// </summary>
		protected Type _objectType;

		private Hashtable _rented;

		private int _rentid;

		private object _sync;

		private ArrayList _list;

		/// <summary>
		/// Quantidade de itens disponíveis.
		/// </summary>
		public int AvailableCount
		{
			get
			{
				return _available.Count;
			}
		}

		/// <summary>
		/// Tamanho inicial.
		/// </summary>
		public int InitialSize
		{
			get
			{
				return _initialSize;
			}
			set
			{
				_initialSize = value;
			}
		}

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// Tipo de objeto gerenciado pela instancia.
		/// </summary>
		public abstract Type ObjectType
		{
			get;
		}

		/// <summary>
		/// Total de instancia alugadas.
		/// </summary>
		public int RentCount
		{
			get
			{
				return _rented.Count;
			}
		}

		/// <summary>
		/// Quantidade total de objetos.
		/// </summary>
		public int TotalObjects
		{
			get
			{
				return (_rented.Count + _available.Count);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ObjectProvider()
		{
			_available = new Hashtable();
			_rented = new Hashtable();
			_availableRentIds = new ArrayList();
			_initialSize = 30;
			_sync = new object();
			_rentid = 1;
			_list = new ArrayList();
			Initialize();
		}

		/// <summary>
		/// Cria uma instancia com o tamanho inicial.
		/// </summary>
		/// <param name="initialSize"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ObjectProvider(int initialSize)
		{
			_available = new Hashtable();
			_rented = new Hashtable();
			_availableRentIds = new ArrayList();
			_initialSize = 30;
			_sync = new object();
			_rentid = 1;
			_list = new ArrayList();
			_initialSize = initialSize;
			Initialize();
		}

		/// <summary>
		/// Reseta a instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		protected abstract void ResetObject(object obj);

		/// <summary>
		/// Cria uma instancia para o objeto gerenciado pela instancia.
		/// </summary>
		/// <returns></returns>
		protected abstract IRentableObject CreateObject();

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		public void Initialize()
		{
			IRentableObject obj2 = null;
			lock (_sync)
			{
				for(int i = 0; i < _initialSize; i++)
				{
					obj2 = CreateObject();
					if(obj2 != null)
					{
						ResetObject(obj2);
						_list.Add(obj2);
					}
				}
			}
		}

		/// <summary>
		/// Cria uma instancia alugada.
		/// </summary>
		/// <returns></returns>
		public IRentableObject RentAnObject()
		{
			IRentableObject obj2 = null;
			lock (_sync)
			{
				if(_available.Count > 0)
				{
					obj2 = (IRentableObject)_available[_availableRentIds[0]];
					_available.Remove(obj2.RentId);
					_availableRentIds.Remove(obj2.RentId);
					_rented.Add(obj2.RentId, obj2);
					return obj2;
				}
				obj2 = this.CreateObject();
				obj2.RentId = _rentid++;
				if(obj2 != null)
					_rented.Add(obj2.RentId, obj2);
			}
			return obj2;
		}

		/// <summary>
		/// Adiciona o objeto informado para instancia.
		/// </summary>
		/// <param name="obj"></param>
		public void SubmittObject(IRentableObject obj)
		{
			lock (this._sync)
			{
				if(_rented.Contains(obj.RentId))
				{
					_rented.Remove(obj.RentId);
					ResetObject(obj);
					_available.Add(obj.RentId, obj);
					_availableRentIds.Add(obj.RentId);
				}
			}
		}
	}
}
