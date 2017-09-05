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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação do localizador de projeção que utiliza o esquema com base.
	/// </summary>
	public class SchemaProjectionSearcher : Colosoft.Query.IProjectionSearcher
	{
		private ITypeSchema _typeSchema;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema"></param>
		public SchemaProjectionSearcher(ITypeSchema typeSchema)
		{
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Pesquisa a projeção para as informações da entidade.
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <returns></returns>
		public Query.ProjectionSearcherResult Search(Query.EntityInfo entityInfo)
		{
			if(entityInfo != null)
			{
				var typeMetadata = _typeSchema.GetTypeMetadata(entityInfo.FullName);
				if(typeMetadata != null)
				{
					var entries = typeMetadata.Where(f => f.Direction == Data.Schema.DirectionParameter.Input || f.Direction == Data.Schema.DirectionParameter.InputOutput).Select(f => new Colosoft.Query.ProjectionSearcherResult.Entry(f.Name, null));
					if(typeMetadata.IsVersioned)
						entries = entries.Concat(new Colosoft.Query.ProjectionSearcherResult.Entry[] {
							new Query.ProjectionSearcherResult.Entry(Query.DataAccessConstants.RowVersionPropertyName, null)
						});
					return new Query.ProjectionSearcherResult(entries);
				}
			}
			return null;
		}
	}
}
