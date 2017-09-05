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
	/// Implementação base para a interface <see cref="ITraceableModel"/>.
	/// </summary>
	public abstract class BaseTraceableModel : BaseVersionedModel, ITraceableModel
	{
		/// <summary>
		/// Data de criação do registro.
		/// </summary>
		public DateTimeOffset CreatedDate
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do perfil usado na criação dos dados.
		/// </summary>
		public int CreatedProfileId
		{
			get;
			set;
		}

		/// <summary>
		/// Data de ativação dos dados.
		/// </summary>
		public DateTimeOffset ActivatedDate
		{
			get;
			set;
		}

		/// <summary>
		/// Data que os dados expiraram.
		/// </summary>
		public DateTimeOffset? ExpiredDate
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se a instância está expirada
		/// </summary>
		public bool IsExpired
		{
			get
			{
				return ExpiredDate.HasValue && (ServerData.GetDateTimeOffSet() > ExpiredDate);
			}
		}

		/// <summary>
		/// Indica se a instância está expirada
		/// </summary>
		public bool IsActive
		{
			get
			{
				return ActivatedDate < ServerData.GetDateTimeOffSet() && !IsExpired;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public BaseTraceableModel()
		{
			CreatedDate = ServerData.GetDateTimeOffSet();
			ActivatedDate = ServerData.GetDateTimeOffSet();
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			var instance = base.Clone() as BaseTraceableModel;
			instance.CreatedDate = CreatedDate;
			instance.ActivatedDate = ActivatedDate;
			instance.CreatedProfileId = CreatedProfileId;
			instance.ExpiredDate = ExpiredDate;
			return instance;
		}
	}
}
