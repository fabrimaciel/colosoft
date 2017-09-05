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
	/// Representa os argumentos do evento acionado quando a entidade estive sendo apagada.
	/// </summary>
	public class EntityDeletingWithPersistenceSessionEventArgs : EventArgs, IEntityEventArgs
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
		/// Sessão de persistencia associada.
		/// </summary>
		public Data.IPersistenceSession Session
		{
			get;
			private set;
		}

		/// <summary>
		/// Sessão de persistencia que será processada anteriormente.
		/// </summary>
		public Data.IPersistenceSession BeforeSession
		{
			get;
			private set;
		}

		/// <summary>
		/// Sessão de persistencia que será processada posteriormente.
		/// </summary>
		public Data.IPersistenceSession AfterSession
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="session"></param>
		/// <param name="beforeSession"></param>
		/// <param name="afterSession"></param>
		public EntityDeletingWithPersistenceSessionEventArgs(IEntity entity, Data.IPersistenceSession session, Data.IPersistenceSession beforeSession, Data.IPersistenceSession afterSession)
		{
			this.Entity = entity;
			this.Session = session;
			this.BeforeSession = beforeSession;
			this.AfterSession = afterSession;
		}
	}
	/// <summary>
	/// Repreesnta o evento acionado quando a entidade estiver 
	/// sendo execluída com a sessão de persistencia.
	/// </summary>
	public class EntityDeletingWithPersistenceSessionEvent : Domain.CompositeDomainEvent<EntityDeletingWithPersistenceSessionEventArgs>
	{
	}
}
