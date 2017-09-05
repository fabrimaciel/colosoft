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
	/// Observer usado para resultados de consulta com apenas uma entidade.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	class SingleEntityQueryResultChangedObserver<TEntity> : BusinessQueryResultChangedObserver<TEntity> where TEntity : IEntity
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityLoader">Instancia do loader da entidade associada.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador dos tipos de entidades.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="typeName">Nome do associado com o observer.</param>
		/// <param name="collection">Coleção que sera observada.</param>
		public SingleEntityQueryResultChangedObserver(IEntityLoader entityLoader, IEntityTypeManager entityTypeManager, Query.ISourceContext sourceContext, string uiContext, Colosoft.Reflection.TypeName typeName, System.Collections.IList collection) : base(entityLoader, entityTypeManager, sourceContext, uiContext, typeName, collection)
		{
		}

		/// <summary>
		/// Avalia se o registro pertence a coleção observada.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public override bool Evaluate(Query.IRecord record)
		{
			return IsAlive;
		}
	}
}
