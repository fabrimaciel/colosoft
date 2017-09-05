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
	/// Implementação de um agregador de esquemas de tipo.
	/// </summary>
	public class AggregateTypeSchema : ITypeSchema
	{
		private List<ITypeSchema> _schemas = new List<ITypeSchema>();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="schemas"></param>
		public AggregateTypeSchema(IEnumerable<ITypeSchema> schemas)
		{
			schemas.Require("schemas").NotNull();
			_schemas.AddRange(schemas);
		}

		/// <summary>
		/// Recupera os metadados do tipo.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private ITypeMetadata GetTypeMetadata(ITypeMetadata result)
		{
			var typeMetadata = result as Local.TypeMetadata;
			if(typeMetadata != null && typeMetadata.BaseTypes.Any())
				result = new AggregateTypeMetadata(new[] {
					result
				}.Concat(typeMetadata.BaseTypes.Select(f => _schemas.Select(x => x.GetTypeMetadata(f.FullName)).FirstOrDefault(x => x != null)).Where(f => f != null)));
			return result;
		}

		/// <summary>
		/// Recupera os metadados do tipo com o nome informado.
		/// </summary>
		/// <param name="fullName">Nome completo do tipo.</param>
		/// <returns>Instancia dos metadados do tipo.</returns>
		public ITypeMetadata GetTypeMetadata(string fullName)
		{
			foreach (var i in _schemas)
			{
				var result = i.GetTypeMetadata(fullName);
				if(result != null)
					return GetTypeMetadata(result);
			}
			return null;
		}

		/// <summary>
		/// Recupera os metadados do tipo com base no código informado.
		/// </summary>
		/// <param name="typeCode">Código do tipo.</param>
		/// <returns></returns>
		public ITypeMetadata GetTypeMetadata(int typeCode)
		{
			foreach (var i in _schemas)
			{
				var result = i.GetTypeMetadata(typeCode);
				if(result != null)
					return GetTypeMetadata(result);
			}
			return null;
		}

		/// <summary>
		/// Recupera os metadados de uma propriedade pelo código informado.
		/// </summary>
		/// <param name="propertyCode">Código da propriedade.</param>
		/// <returns>Instancia dos metadados da propriedade.</returns>
		public IPropertyMetadata GetPropertyMetadata(int propertyCode)
		{
			foreach (var i in _schemas)
			{
				var result = i.GetPropertyMetadata(propertyCode);
				if(result != null)
					return result;
			}
			return null;
		}

		/// <summary>
		/// Recupera os metadados de todos o tipos registrados
		/// </summary>
		/// <returns>Intâncias dos metadados dos tipos</returns>
		public IEnumerable<ITypeMetadata> GetTypeMetadatas()
		{
			foreach (var i in _schemas)
				foreach (var j in i.GetTypeMetadatas())
					yield return GetTypeMetadata(j);
		}

		/// <summary>
		/// Recupera os metadados dos tipos associados com o assembly informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly onde os tipo estão inseridos.</param>
		/// <returns>Intâncias dos metadados dos tipos</returns>
		public IEnumerable<ITypeMetadata> GetTypeMetadatas(string assemblyName)
		{
			foreach (var i in _schemas)
				foreach (var j in i.GetTypeMetadatas(assemblyName))
					yield return GetTypeMetadata(j);
		}

		/// <summary>
		/// Recarrega informações.
		/// </summary>
		public void Reload()
		{
			foreach (var i in _schemas)
				i.Reload();
		}
	}
}
