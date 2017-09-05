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

namespace Colosoft.Business
{
	/// <summary>
	/// Representa os parâmetros passados aos eventos de conclusão alteração de propriedade
	/// </summary>
	public class EntityPropertyChangedEventArgs : EventArgs, IEntityEventArgs
	{
		/// <summary>
		/// Instancia da entidade associada.
		/// </summary>
		public IEntity Entity
		{
			get;
			private set;
		}

		/// <summary>
		/// Nome da propriedade
		/// </summary>
		public string PropertyName
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entity">Instancia da entidade.</param>
		/// <param name="propertyName">Nome da propriedade.</param>
		public EntityPropertyChangedEventArgs(IEntity entity, string propertyName)
		{
			this.Entity = entity;
			this.PropertyName = propertyName;
		}
	}
	/// <summary>
	/// Representa os parâmetros passados aos eventos de conclusão alteração de propriedade
	/// </summary>
	public class EntityPropertyChangedEventArgs<TEntity> : EntityEventArgs<TEntity> where TEntity : IEntity
	{
		private EntityPropertyChangedEventArgs _eventArgs;

		/// <summary>
		/// Nome da propriedade
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _eventArgs.PropertyName;
			}
			set
			{
				_eventArgs.PropertyName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="eventArgs"></param>
		public EntityPropertyChangedEventArgs(EntityPropertyChangedEventArgs eventArgs) : base(eventArgs)
		{
			_eventArgs = eventArgs;
		}
	}
	/// <summary>
	/// Representa o evento acionado quando uma propriedade da entidade for alterada.
	/// </summary>
	public class EntityPropertyChangedEvent : Domain.CompositeDomainEvent<EntityPropertyChangedEventArgs>
	{
	}
}
