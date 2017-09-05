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

namespace Colosoft.Query.Database.Generic
{
	/// <summary>
	/// Implementação do localizador de provedores de acesso.
	/// </summary>
	public class ProviderLocator : IProviderLocator
	{
		private Data.Schema.ITypeSchema _typeSchema;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema"></param>
		public ProviderLocator(Data.Schema.ITypeSchema typeSchema)
		{
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Recupera o nome do provedor associado com o queryinfo informado.
		/// </summary>
		/// <param name="queryInfo"></param>
		/// <returns></returns>
		public string GetProviderName(QueryInfo queryInfo)
		{
			if(queryInfo.StoredProcedureName != null)
				return ((!string.IsNullOrEmpty(queryInfo.StoredProcedureProvider)) ? queryInfo.StoredProcedureProvider : (queryInfo.ProviderName ?? GDA.GDASettings.DefaultProviderName)) ?? (queryInfo.ProviderName ?? GDA.GDASettings.DefaultProviderName);
			else if(!string.IsNullOrEmpty(queryInfo.ProviderName))
				return queryInfo.ProviderName;
			else if(!string.IsNullOrEmpty(queryInfo.Entities[0].FullName))
			{
				var typeMetadata = _typeSchema.GetTypeMetadata(queryInfo.Entities[0].FullName);
				return (typeMetadata == null || typeMetadata.TableName == null || string.IsNullOrEmpty(typeMetadata.TableName.Catalog) ? GDA.GDASettings.DefaultProviderName : typeMetadata.TableName.Catalog) ?? GDA.GDASettings.DefaultProviderName;
			}
			else
				return GetProviderName(queryInfo.Entities[0].SubQuery) ?? GDA.GDASettings.DefaultProviderName;
		}

		/// <summary>
		/// Recupera o nome do provedor associado com
		/// </summary>
		/// <param name="entityFullName"></param>
		/// <returns></returns>
		public string GetProviderName(string entityFullName)
		{
			var typeMetadata = _typeSchema.GetTypeMetadata(entityFullName);
			return typeMetadata == null ? GDA.GDASettings.DefaultProviderName : typeMetadata.TableName.Catalog;
		}

		/// <summary>
		/// Recupera o nome do provedor associado com o nom e da storedprocedure.
		/// </summary>
		/// <param name="storedProcedureName">Nome do storedprocedure.</param>
		/// <returns></returns>
		public string GetProviderName(StoredProcedureName storedProcedureName)
		{
			return GDA.GDASettings.DefaultProviderName;
		}
	}
}
