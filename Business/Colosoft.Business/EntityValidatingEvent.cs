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
	/// Representa os argumentos do evento acionado quando a entidade estiver
	/// sendo validada.
	/// </summary>
	public class EntityValidatingEventArgs : EventArgs, IEntityEventArgs
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
		/// Instancia do resultado da validação.
		/// </summary>
		public Validation.ValidationResult ValidationResult
		{
			get;
			set;
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
		/// Construtor padrão.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="validationResult">Instancia do resultado da validação.</param>
		public EntityValidatingEventArgs(IEntity entity, Validation.ValidationResult validationResult)
		{
			this.Entity = entity;
			this.ValidationResult = validationResult;
		}
	}
	/// <summary>
	/// Representa os argumentos do evento acionado quando a entidade estiver
	/// sendo validada.
	/// </summary>
	public class EntityValidatingEventArgs<TEntity> : EntityEventArgs<TEntity> where TEntity : IEntity
	{
		private EntityValidatingEventArgs _eventArgs;

		/// <summary>
		/// Instancia do resultado da validação.
		/// </summary>
		public Validation.ValidationResult ValidationResult
		{
			get
			{
				return _eventArgs.ValidationResult;
			}
			set
			{
				_eventArgs.ValidationResult = value;
			}
		}

		/// <summary>
		/// Identitica se é para cancelar a operação.
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
		/// Construtor padrão.
		/// </summary>
		/// <param name="eventArgs"></param>
		public EntityValidatingEventArgs(EntityValidatingEventArgs eventArgs) : base(eventArgs)
		{
			_eventArgs = eventArgs;
		}
	}
	/// <summary>
	/// Evento acionado quando uma entidade estiver sendo validada.
	/// </summary>
	public class EntityValidatingEvent : Domain.CompositeDomainEvent<EntityValidatingEventArgs>
	{
	}
}
