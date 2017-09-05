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

namespace Colosoft.Data
{
	/// <summary>
	/// Implementação do descritor da entidade que é inicializado
	/// com os dados de um registro que contém os dados para ratreamento.
	/// </summary>
	public class TraceableEntityDescriptor : EntityDescriptor, IEntityDescriptor
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="record">Instancia do registro que será trabalhado.</param>
		/// <param name="idFieldName"></param>
		/// <param name="nameFieldName"></param>
		/// <param name="descriptionFieldName"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public TraceableEntityDescriptor(Colosoft.Query.Record record, string idFieldName, string nameFieldName, string descriptionFieldName = null)
		{
			record.Require("record").NotNull();
			idFieldName.Require("idFieldName").NotNull();
			nameFieldName.Require("nameFieldName").NotNull();
			var idFieldIndex = record.Descriptor.GetFieldPosition(idFieldName);
			var nameFieldIndex = record.Descriptor.GetFieldPosition(nameFieldName);
			var descriptionFieldIndex = record.Descriptor.GetFieldPosition(descriptionFieldName);
			Initialize(record, idFieldIndex, nameFieldIndex, descriptionFieldIndex);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="record">Instancia do registro que será trabalhado.</param>
		/// <param name="idFieldIndex">Indice do campo ID que será recuperado do registro.</param>
		/// <param name="nameFieldIndex">Indice do campo Name que será recuperado do registro.</param>
		/// <param name="descriptionFieldIndex">Indice do campo Description que será recuperado do registro</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public TraceableEntityDescriptor(Colosoft.Query.Record record, int idFieldIndex, int nameFieldIndex, int descriptionFieldIndex = -1)
		{
			record.Require("record").NotNull();
			Initialize(record, idFieldIndex, nameFieldIndex, descriptionFieldIndex);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="idFieldIndex"></param>
		/// <param name="nameFieldIndex"></param>
		/// <param name="descriptionFieldIndex"></param>
		private void Initialize(Colosoft.Query.Record record, int idFieldIndex, int nameFieldIndex, int descriptionFieldIndex)
		{
			this.Id = record[idFieldIndex];
			this.Name = record[nameFieldIndex];
			if(descriptionFieldIndex >= 0)
				this.Description = record[descriptionFieldIndex];
			if(record.Descriptor.Contains("ActivatedDate") && record.Descriptor.Contains("ExpiredDate"))
			{
				DateTimeOffset activatedDate = record["ActivatedDate"];
				DateTimeOffset? expiredDate = record["ExpiredDate"];
				IsExpired = expiredDate.HasValue && (ServerData.GetDateTimeOffSet() > expiredDate);
				IsActive = activatedDate < ServerData.GetDateTimeOffSet() && !IsExpired;
			}
		}
	}
}
