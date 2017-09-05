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
	/// Armazena os argumentos do evento acionado quando a entidade for inicializada.
	/// </summary>
	public class EntityInitializedEventArgs : EventArgs, IEntityEventArgs
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
		/// Construtor padrão.
		/// </summary>
		/// <param name="entity"></param>
		public EntityInitializedEventArgs(IEntity entity)
		{
			entity.Require("entity").NotNull();
			this.Entity = entity;
		}
	}
	/// <summary>
	/// Armazena os argumentos dos evento Iniciaalized.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class EntityInicializedEventArgs<TEntity> : EntityEventArgs<TEntity> where TEntity : IEntity
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="eventArgs"></param>
		public EntityInicializedEventArgs(IEntityEventArgs eventArgs) : base(eventArgs)
		{
		}
	}
	/// <summary>
	/// Representa o evento acionado qaundo a entidade for inicializada.
	/// </summary>
	public class EntityInitializedEvent : Domain.CompositeDomainEvent<EntityInitializedEventArgs>
	{
	}
}
