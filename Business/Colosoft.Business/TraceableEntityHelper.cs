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
	/// Classe com método para auxiliar as entidades com rastreamento.
	/// </summary>
	public static class TraceableEntityHelper
	{
		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="dataModel"></param>
		public static void InitializeTraceableDataModel(Colosoft.Data.ITraceableModel dataModel)
		{
			var storageControl = dataModel as Colosoft.Data.IStorageControl;
			if(storageControl == null || !storageControl.ExistsInStorage)
			{
				dataModel.ActivatedDate = Colosoft.ServerData.GetDateTimeOffSet();
				dataModel.CreatedDate = Colosoft.ServerData.GetDateTimeOffSet();
				var profile = Colosoft.Security.Profile.ProfileManager.CurrentProfileInfo;
				if(profile != null)
				{
					dataModel.CreatedProfileId = profile.ProfileId;
				}
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="entity"></param>
		public static void InitializeTraceableEntity(ITraceableEntity entity)
		{
			entity.Require("entity").NotNull();
			if(!entity.ExistsInStorage)
			{
				entity.ActivatedDate = Colosoft.ServerData.GetDateTimeOffSet();
				entity.CreatedDate = Colosoft.ServerData.GetDateTimeOffSet();
				var profile = Colosoft.Security.Profile.ProfileManager.CurrentProfileInfo;
				if(profile != null)
					entity.CreatedProfileId = profile.ProfileId;
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="dataModel"></param>
		public static void InitializeTraceableDataModel(Colosoft.Data.ISimpleTraceableModel dataModel)
		{
			var storageControl = dataModel as Colosoft.Data.IStorageControl;
			if(storageControl == null || !storageControl.ExistsInStorage)
			{
				dataModel.CreatedDate = Colosoft.ServerData.GetDateTimeOffSet();
				var profile = Colosoft.Security.Profile.ProfileManager.CurrentProfileInfo;
				if(profile != null)
				{
					dataModel.CreatedProfileId = profile.ProfileId;
				}
			}
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="entity"></param>
		public static void InitializeTraceableEntity(ISimpleTraceableEntity entity)
		{
			entity.Require("entity").NotNull();
			if(!entity.ExistsInStorage)
			{
				entity.CreatedDate = Colosoft.ServerData.GetDateTimeOffSet();
				var profile = Colosoft.Security.Profile.ProfileManager.CurrentProfileInfo;
				if(profile != null)
					entity.CreatedProfileId = profile.ProfileId;
			}
		}
	}
}
