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
	/// Armazena os argumentos do evento acionado quando a entidade for apagada.
	/// </summary>
	public class EntityDeletedEventArgs : EventArgs, IEntityEventArgs
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
		/// Identifica se a entidade foi apagada com sucesso.
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem associada.
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
		/// <param name="success">Identifica se a entidade foi apagada com sucesso.</param>
		/// <param name="message">Mensagem associada.</param>
		public EntityDeletedEventArgs(IEntity entity, bool success, IMessageFormattable message)
		{
			this.Entity = entity;
			this.Success = success;
			this.Message = message;
		}
	}
	/// <summary>
	/// Armazena os argumentos do evento acionado quando a entidade for apagada.
	/// </summary>
	public class EntityDeletedEventArgs<TEntity> : EntityEventArgs<TEntity> where TEntity : IEntity
	{
		private EntityDeletedEventArgs _eventArgs;

		/// <summary>
		/// Identifica se a entidade foi apagada com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return _eventArgs.Success;
			}
			set
			{
				_eventArgs.Success = value;
			}
		}

		/// <summary>
		/// Mensagem associada.
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
		public EntityDeletedEventArgs(EntityDeletedEventArgs eventArgs) : base(eventArgs)
		{
			_eventArgs = eventArgs;
		}
	}
	/// <summary>
	/// Evento acionado quando a entidade for salva.
	/// </summary>
	public class EntityDeletedEvent : Domain.CompositeDomainEvent<EntityDeletedEventArgs>
	{
	}
}
