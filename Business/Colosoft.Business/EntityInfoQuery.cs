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
	/// Armazena a consulta das informações de uma entidade..
	/// </summary>
	class EntityInfoQuery
	{
		/// <summary>
		/// Tipo da entidade associada com o resultado.
		/// </summary>
		public Type EntityType
		{
			get;
			private set;
		}

		/// <summary>
		/// Tipo do modelo de dados associado com o resultado.
		/// </summary>
		public Type DataModelType
		{
			get;
			private set;
		}

		/// <summary>
		/// Consulta.
		/// </summary>
		public Colosoft.Query.Queryable Query
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataModelType">Tipo do modelo de dados.</param>
		/// <param name="entityType">Tipo da entidade do resultado.</param>
		/// <param name="query">Consulta.</param>
		public EntityInfoQuery(Type dataModelType, Type entityType, Colosoft.Query.Queryable query)
		{
			this.DataModelType = dataModelType;
			this.EntityType = entityType;
			this.Query = query;
		}
	}
}
