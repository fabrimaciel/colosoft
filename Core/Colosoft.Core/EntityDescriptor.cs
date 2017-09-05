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

namespace Colosoft
{
	/// <summary>
	/// Representa o nome e o id de uma entidade do sistema.
	/// </summary>
	public class EntityDescriptor : IEntityDescriptor, IEquatable<IEntityDescriptor>, IComparable<IEntityDescriptor>
	{
		private bool _isActive = true;

		private bool _isExpired = false;

		/// <summary>
		/// Identificador da entidade.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da entidade.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Descrição da entidade.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a instancia está ativa.
		/// </summary>
		public virtual bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				_isActive = value;
			}
		}

		/// <summary>
		/// Identifica se a instancia está expirada.
		/// </summary>
		public virtual bool IsExpired
		{
			get
			{
				return _isExpired;
			}
			set
			{
				_isExpired = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EntityDescriptor()
		{
		}

		/// <summary>
		/// Cria a instancia com o identificador e o nome.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		public EntityDescriptor(int id, string name)
		{
			this.Id = id;
			this.Name = name;
		}

		/// <summary>
		/// Configura o controle de status associado.
		/// </summary>
		/// <param name="isActive">Identifica se os dados estão ativos.</param>
		/// <param name="isExpired">Identifica se os dados foram expirados.</param>
		public void ConfigureStatusControl(bool isActive, bool isExpired)
		{
			_isActive = isActive;
			_isExpired = isExpired;
		}

		/// <summary>
		/// Compara a instancia com outro descritor.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IEntityDescriptor other)
		{
			return EntityDescriptorIdComparer.Instance.Equals(this, other);
		}

		/// <summary>
		/// Compara a instancia com o descritor informado.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(IEntityDescriptor other)
		{
			return EntityDescriptorIdComparer.Instance.Compare(this, other);
		}
	}
	/// <summary>
	/// Implementação do comparador que utiliza o Id do entity descriptor
	/// como referencia.
	/// </summary>
	public class EntityDescriptorIdComparer : IEqualityComparer<IEntityDescriptor>, System.Collections.IEqualityComparer, IComparer<IEntityDescriptor>, System.Collections.IComparer
	{
		/// <summary>
		/// Instancia padrão.
		/// </summary>
		public static readonly EntityDescriptorIdComparer Instance = new EntityDescriptorIdComparer();

		/// <summary>
		/// Verifica se a instancias informadas são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(IEntityDescriptor x, IEntityDescriptor y)
		{
			return (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null)) || (!object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null) && x.Id == y.Id);
		}

		/// <summary>
		/// Recupera o código hash da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(IEntityDescriptor obj)
		{
			if(obj != null)
				obj.Id.GetHashCode();
			return 0;
		}

		/// <summary>
		/// Compara as instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(IEntityDescriptor x, IEntityDescriptor y)
		{
			return object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null) ? 0 : object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null) ? -1 : object.ReferenceEquals(y, null) && !object.ReferenceEquals(x, null) ? 1 : x.Id > y.Id ? 1 : x.Id < y.Id ? -1 : 0;
		}

		/// <summary>
		/// Compara as instancia informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool System.Collections.IEqualityComparer.Equals(object x, object y)
		{
			return Equals(x as IEntityDescriptor, y as IEntityDescriptor);
		}

		/// <summary>
		/// Recupera o hash code da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		int System.Collections.IEqualityComparer.GetHashCode(object obj)
		{
			return GetHashCode(obj as IEntityDescriptor);
		}

		/// <summary>
		/// Compara as instancia informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		int System.Collections.IComparer.Compare(object x, object y)
		{
			return Compare(x as IEntityDescriptor, y as IEntityDescriptor);
		}
	}
}
