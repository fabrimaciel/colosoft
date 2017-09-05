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
	/// Argumentos para o método que cria uma entidade do filho com base na entidade do link.
	/// </summary>
	public class EntityFromLinkCreatorArgs : EventArgs
	{
		/// <summary>
		/// Instancia da entidade do link.
		/// </summary>
		public IEntity LinkEntity
		{
			get;
			private set;
		}

		/// <summary>
		/// Instancia do método para criar a entidade do filho.
		/// </summary>
		public EntityFromModelCreatorHandler EntityCreator
		{
			get;
			private set;
		}

		/// <summary>
		/// Contexto de interface do usuário.
		/// </summary>
		public string UIContext
		{
			get;
			private set;
		}

		/// <summary>
		/// Instancia do gerenciador de tipo de entidades.
		/// </summary>
		public IEntityTypeManager EntityTypeManager
		{
			get;
			private set;
		}

		/// <summary>
		/// Instancia da origem do contexto de dados.
		/// </summary>
		public Colosoft.Query.ISourceContext SourceContext
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="linkEntity"></param>
		/// <param name="entityCreator"></param>
		/// <param name="uiContext"></param>
		/// <param name="entityTypeManager"></param>
		/// <param name="sourceContext"></param>
		public EntityFromLinkCreatorArgs(IEntity linkEntity, EntityFromModelCreatorHandler entityCreator, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext)
		{
			this.LinkEntity = linkEntity;
			this.EntityCreator = entityCreator;
			this.UIContext = uiContext;
			this.EntityTypeManager = entityTypeManager;
			this.SourceContext = sourceContext;
		}
	}
	/// <summary>
	/// Assinatura do método usado para criar uma entidade do filho com base na entidade do link.
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public delegate IEntity EntityFromLinkCreatorHandler (EntityFromLinkCreatorArgs e);
}
