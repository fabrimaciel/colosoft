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
	/// Armazena os argumento do evento acionado quando
	/// uma propriedade da entidade estiver sendo alterada.
	/// </summary>
	public class EntityPropertyChangingEventArgs : System.ComponentModel.PropertyChangingEventArgs, IEntityEventArgs
	{
		/// <summary>
		/// Instancia da entidade.
		/// </summary>
		public IEntity Entity
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se é cancelar o evento.
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
		/// <param name="entity">Instancia da entidade.</param>
		/// <param name="propertyName">Nome da propriedade que está sendo alterada.</param>
		public EntityPropertyChangingEventArgs(IEntity entity, string propertyName) : base(propertyName)
		{
			this.Entity = entity;
		}
	}
	/// <summary>
	/// Armazena os argumento do evento acionado quando
	/// uma propriedade da entidade estiver sendo alterada.
	/// </summary>
	public class EntityPropertyChangingEventArgs<TEntity> : EntityEventArgs<TEntity> where TEntity : IEntity
	{
		private EntityPropertyChangingEventArgs _eventArgs;

		/// <summary>
		/// Identifica se é cancelar o evento.
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
		public EntityPropertyChangingEventArgs(EntityPropertyChangingEventArgs eventArgs) : base(eventArgs)
		{
			_eventArgs = eventArgs;
		}
	}
	/// <summary>
	/// Represnta o evento acionado quando uma propriedade da entidade estiver sendo alterada.
	/// </summary>
	public class EntityPropertyChangingEvent : Domain.CompositeDomainEvent<EntityPropertyChangingEventArgs>
	{
	}
}
