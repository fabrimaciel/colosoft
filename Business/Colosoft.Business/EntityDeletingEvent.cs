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
	/// Representa os argumentos do evento acionado quando a entidade
	/// estiver sendo apagada.
	/// </summary>
	public class EntityDeletingEventArgs : EventArgs, IEntityEventArgs
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
		/// Identifica se é para cancelar a operação.
		/// </summary>
		public bool Cancel
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem.
		/// </summary>
		public IMessageFormattable Message
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entity"></param>
		public EntityDeletingEventArgs(IEntity entity)
		{
			this.Entity = entity;
		}
	}
	/// <summary>
	/// Representa os argumentos do evento acionado quando a entidade
	/// estiver sendo apagada.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class EntityDeletingEventArgs<TEntity> : EntityEventArgs<TEntity> where TEntity : IEntity
	{
		private EntityDeletingEventArgs _eventArgs;

		/// <summary>
		/// Identifica se é para cancelar a operação.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _eventArgs.Cancel;
			}
			set
			{
				_eventArgs.Cancel = value;
			}
		}

		/// <summary>
		/// Mensagem.
		/// </summary>
		public IMessageFormattable Message
		{
			get
			{
				return _eventArgs.Message;
			}
			set
			{
				_eventArgs.Message = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="eventArgs"></param>
		public EntityDeletingEventArgs(EntityDeletingEventArgs eventArgs) : base(eventArgs)
		{
			_eventArgs = eventArgs;
		}
	}
	/// <summary>
	/// Evento acionado quando a entidade estiver sendo apagada.
	/// </summary>
	public class EntityDeletingEvent : Domain.CompositeDomainEvent<EntityDeletingEventArgs>
	{
	}
}
