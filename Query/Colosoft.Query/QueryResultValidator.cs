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

namespace Colosoft.Query
{
	/// <summary>
	/// Classe com método para validar o resultado da consulta.
	/// </summary>
	public static class QueryResultValidator
	{
		/// <summary>
		/// Valida o resultado da consulta.
		/// </summary>
		/// <param name="queryResult"></param>
		/// <returns></returns>
		public static ValidationQueryResult Validate(this IQueryResult queryResult)
		{
			QueryInfo queryInfo = null;
			Record.RecordDescriptor descriptor = null;
			if(queryResult is IQueryResultExt)
				queryInfo = ((IQueryResultExt)queryResult).QueryInfo;
			if(queryInfo == null)
				return new ValidationQueryResult(ValidationQueryResult.ValidationError.None, null);
			descriptor = queryResult.Descriptor;
			if(descriptor != null && queryInfo.Projection != null && queryInfo.Projection.Count > 0)
			{
				var projection = queryInfo.Projection;
				var isMatch = (descriptor.Count == projection.Count);
				if(isMatch)
				{
					for(var i = 0; i < projection.Count; i++)
					{
						var fieldName = GetFieldName(projection[i]);
						if(string.IsNullOrEmpty(fieldName))
							continue;
						if(!StringComparer.InvariantCultureIgnoreCase.Equals(fieldName, descriptor[i].Name))
						{
							isMatch = false;
							break;
						}
					}
				}
				if(!isMatch)
				{
					var message = ResourceMessageFormatter.Create(() => Properties.Resources.ValidationQueryResult_NotMatchFields, string.Join("; ", projection.Select(f => GetFieldName(f)).ToArray()), string.Join("; ", descriptor.Select(f => f.Name).ToArray()));
					return new ValidationQueryResult(ValidationQueryResult.ValidationError.InvalidFields, message);
				}
			}
			return new ValidationQueryResult(ValidationQueryResult.ValidationError.None, null);
		}

		/// <summary>
		/// Dispara uma exception caso o resultado informado seja inválido.
		/// </summary>
		/// <param name="validationResult"></param>
		public static void ThrowInvalid(this ValidationQueryResult validationResult)
		{
			if(validationResult != null && !validationResult)
				throw new InvalidQueryResultException(validationResult);
		}

		/// <summary>
		/// Valida os dados do registro.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="queryInfo"></param>
		/// <returns></returns>
		public static ValidationQueryResult Validate(this Record record, QueryInfo queryInfo)
		{
			Record.RecordDescriptor descriptor = null;
			if(record == null || queryInfo == null)
				return new ValidationQueryResult(ValidationQueryResult.ValidationError.None, null);
			descriptor = record.Descriptor;
			if(descriptor == null)
				return new ValidationQueryResult(ValidationQueryResult.ValidationError.None, null);
			if(descriptor != null && queryInfo.Projection != null && queryInfo.Projection.Count > 0)
			{
				var projection = queryInfo.Projection;
				var isMatch = (descriptor.Count == projection.Count);
				if(isMatch)
				{
					for(var i = 0; i < projection.Count; i++)
					{
						var fieldName = GetFieldName(projection[i]);
						if(string.IsNullOrEmpty(fieldName))
							continue;
						if(!StringComparer.InvariantCultureIgnoreCase.Equals(fieldName, descriptor[i].Name))
						{
							isMatch = false;
							break;
						}
					}
				}
				if(!isMatch)
				{
					var message = ResourceMessageFormatter.Create(() => Properties.Resources.ValidationQueryResult_NotMatchFields, string.Join("; ", projection.Select(f => GetFieldName(f)).ToArray()), string.Join("; ", descriptor.Select(f => f.Name).ToArray()));
					return new ValidationQueryResult(ValidationQueryResult.ValidationError.InvalidFields, message);
				}
			}
			return new ValidationQueryResult(ValidationQueryResult.ValidationError.None, null);
		}

		/// <summary>
		/// Recupera o nome do campo da projeção.
		/// </summary>
		/// <param name="projectionEntry"></param>
		/// <returns></returns>
		private static string GetFieldName(ProjectionEntry projectionEntry)
		{
			return projectionEntry.Alias;
		}
	}
}
