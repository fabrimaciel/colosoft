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

namespace Colosoft.Query.Database.Oracle
{
	/// <summary>
	/// Implementação do tradutor de consulta do Oracle.
	/// </summary>
	public class OracleQueryTranslator : SqlQueryTranslator
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema">Objeto de recuperação de metadados</param>
		public OracleQueryTranslator(Data.Schema.ITypeSchema typeSchema) : base(typeSchema)
		{
		}

		/// <summary>
		/// Recupera o nome da entidade
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public override ITranslatedName GetName(EntityInfo entity)
		{
			return this.GetName(entity, false);
		}

		/// <summary>
		/// Recupera o nome da entidade
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="ignoreTypeSchema"></param>
		/// <returns></returns>
		public override ITranslatedName GetName(EntityInfo entity, bool ignoreTypeSchema)
		{
			var name = (TranslatedTableName)base.GetName(entity, ignoreTypeSchema);
			return new TranslatedTableName((name.Schema ?? "").ToUpper(), name.Name.ToUpper());
		}

		/// <summary>
		/// Recupera o nome associado com a propridade
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public override ITranslatedName GetName(EntityInfo entity, string propertyName)
		{
			return this.GetName(entity, propertyName, false);
		}

		/// <summary>
		/// Recupera o nome associado com a propridade
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <param name="ignoreTypeSchema"></param>
		/// <returns></returns>
		public override ITranslatedName GetName(EntityInfo entity, string propertyName, bool ignoreTypeSchema)
		{
			if(propertyName == Query.DataAccessConstants.RowVersionPropertyName)
				return new TranslatedSelectPart(string.Format("CAST({0}ORA_ROWSCN AS NUMBER(18,0))", !string.IsNullOrEmpty(entity.Alias) ? string.Format("\"{0}\".", entity.Alias) : ""));
			var name = base.GetName(entity, propertyName, ignoreTypeSchema) as TranslatedColumnName;
			return new TranslatedColumnName(name.Name.ToUpper(), name.TableAlias, name.PropertyType);
		}

		/// <summary>
		/// Pega o nome da storedprocedure.
		/// </summary>
		/// <param name="storedProcedureName">Nome da stored procedure.</param>
		/// <returns></returns>
		public override ITranslatedName GetName(StoredProcedureName storedProcedureName)
		{
			return new TranslatedStoredProcedureName((storedProcedureName.Name ?? "").ToUpper(), (storedProcedureName.Schema ?? "").ToUpper());
		}
	}
}
