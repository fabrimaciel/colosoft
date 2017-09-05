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

using Colosoft.Data.Schema;
using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Data.Database.Oracle
{
	/// <summary>
	/// Implementação do repositório de chave primária do Oracle.
	/// </summary>
	public class OraclePrimaryKeyRepository : IOraclePrimaryKeyRepository
	{
		private ITypeSchema _typeSchema;

		Random _rnd = new Random();

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		public string ProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="typeSchema">Classe de metadados do sistema</param>
		public OraclePrimaryKeyRepository(ITypeSchema typeSchema)
		{
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Navega pelo cursor.
		/// </summary>
		/// <param name="refCursorParameter"></param>
		/// <returns></returns>
		private static IEnumerable<GDADataRecord> NavigateRefCursor(GDAParameter refCursorParameter)
		{
			if(refCursorParameter.Value != null)
			{
				#if UNMANAGED
				                var cursor = (global::Oracle.DataAccess.Types.OracleRefCursor)refCursorParameter.Value;
#else
				var cursor = (global::Oracle.ManagedDataAccess.Types.OracleRefCursor)refCursorParameter.Value;
				#endif
				var reader = cursor.GetDataReader();
				while (reader.Read())
				{
					yield return new GDADataRecord(reader, null);
				}
			}
		}

		/// <summary>
		/// Retorna se o id é criado antes da execução do comando ou após a execução do comando     
		/// </summary>
		/// <param name="entityName">Nome da entidade</param>
		/// <returns>Retorna true se for após e false caso contrário</returns>
		public bool IsPosCommand(string entityName)
		{
			return false;
		}

		/// <summary>
		/// Retorna o parâmetro correspondente a chave primária
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="entityName">Nome da entidade</param>
		/// <returns>Pâremetro correspondente a chave primária</returns>
		public object GetPrimaryKey(IPersistenceTransactionExecuter transaction, string entityName)
		{
			var metadata = _typeSchema.GetTypeMetadata(entityName);
			var identityMetadata = metadata.GetKeyProperties().FirstOrDefault();
			using (var session = new GDASession(GDASettings.GetProviderConfiguration(ProviderName)))
			{
				DataAccess da = session != null ? new DataAccess(session.ProviderConfiguration) : new DataAccess();
				var indexes = new List<int>();
				var script = "BEGIN \"CONTROL\".\"GETIDENTITY\"(:1,:2,:3,:4); END;";
				var parameters = new GDA.Collections.GDAParameterCollection();
				parameters.Add(new GDAParameter(":1", metadata.TableName.Schema, System.Data.ParameterDirection.Input));
				parameters.Add(new GDAParameter(":2", metadata.TableName.Name, System.Data.ParameterDirection.Input));
				parameters.Add(new GDAParameter(":3", identityMetadata.ColumnName, System.Data.ParameterDirection.Input));
				var resultParameter = new GDAParameter(":4", null) {
					DbType = System.Data.DbType.Int32,
					Direction = System.Data.ParameterDirection.Output
				};
				parameters.Add(resultParameter);
				da.ExecuteCommand(session, script, parameters.ToArray());
				return resultParameter.Value;
			}
		}
	}
}
