﻿/* 
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
	/// Assinatura do evento acionado quando a entidade estiver sendo validada.
	/// </summary>
	public interface IEntityValidatingEventSubscription : Domain.IExecuteSubscription<EntityValidatingEventArgs>
	{
	}
	/// <summary>
	/// Classe abstrada para tratar o evento Validating da Entidade.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	[EntityEventSubscriptionMetadata]
	public abstract class EntityValidatingEventSubcription<TEntity> : IEntityValidatingEventSubscription where TEntity : IEntity
	{
		/// <summary>
		/// Método executado quando ocorrer o evento Validating na entidade.
		/// </summary>
		/// <param name="args"></param>
		public abstract void Execute(EntityValidatingEventArgs<TEntity> args);

		void Domain.IExecuteSubscription<EntityValidatingEventArgs>.Execute(EntityValidatingEventArgs args)
		{
			this.Execute(new EntityValidatingEventArgs<TEntity>(args));
		}
	}
}
