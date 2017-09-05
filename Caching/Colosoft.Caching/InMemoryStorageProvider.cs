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
using System.Collections;

namespace Colosoft.Caching.Storage
{
	/// <summary>
	/// Implementação de um provedor de armazenamento em memória.
	/// </summary>
	internal class InMemoryStorageProvider : MmfStorageProvider
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="evictionEnabled"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public InMemoryStorageProvider(IDictionary properties, bool evictionEnabled)
		{
			this.Initialize(properties, evictionEnabled);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="evictionEnabled"></param>
		public override void Initialize(IDictionary properties, bool evictionEnabled)
		{
			if(properties == null)
				throw new ArgumentNullException("properties");
			try
			{
				properties.Remove("file-name");
				properties["num-views"] = 1;
				uint num = 0x10;
				if(properties.Contains("max-size"))
				{
					num = Convert.ToUInt32(properties["max-size"]);
				}
				properties["view-size"] = num * 0x100000;
				properties["initial-size-mb"] = num;
				base.Initialize(properties, evictionEnabled);
			}
			catch(Exception)
			{
				throw;
			}
		}
	}
}
