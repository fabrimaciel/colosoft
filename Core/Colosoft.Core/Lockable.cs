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

namespace Colosoft.Lock
{
	/// <summary>
	/// Classe do objeto locado.
	/// </summary>
	[Serializable]
	public class Lockable
	{
		private bool _isLockedToEdit;

		private string _modelType;

		private long _rowVersion;

		private int _uid;

		/// <summary>
		/// Verifica se pode ser locado ou não o objecto.
		/// </summary>
		public bool IsLockedToEdit
		{
			get
			{
				return _isLockedToEdit;
			}
			set
			{
				_isLockedToEdit = value;
			}
		}

		/// <summary>
		/// Tipo de dado.
		/// </summary>
		public string ModelType
		{
			get
			{
				return _modelType;
			}
		}

		/// <summary>
		/// Versão.
		/// </summary>
		public long RowVersion
		{
			get
			{
				return _rowVersion;
			}
		}

		/// <summary>
		/// Identificador.
		/// </summary>
		public int Uid
		{
			get
			{
				return _uid;
			}
		}

		[NonSerialized]
		private ILockable _realObject;

		/// <summary>
		/// Objeto real a ser locado.
		/// </summary>
		public ILockable RealObject
		{
			get
			{
				return _realObject;
			}
			set
			{
				_realObject = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="isLockedToEdit"></param>
		/// <param name="modelType"></param>
		/// <param name="rowVersion"></param>
		/// <param name="uid"></param>
		public Lockable(bool isLockedToEdit, string modelType, long rowVersion, int uid)
		{
			_isLockedToEdit = isLockedToEdit;
			_modelType = modelType;
			_rowVersion = rowVersion;
			_uid = uid;
		}
	}
}
