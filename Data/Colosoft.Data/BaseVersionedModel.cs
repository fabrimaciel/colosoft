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
	/// Implementação base para a interface <see cref="IVersionedModel"/>.
	/// </summary>
	public abstract class BaseVersionedModel : BaseModel, IVersionedModel
	{
		/// <summary>
		/// Versão da linha dos dados.
		/// </summary>
		public long RowVersion
		{
			get;
			protected set;
		}

		/// <summary>
		/// Subescreve o campo RowVersion
		/// </summary>
		/// <param name="rowVersion">Valor que será armazenado</param>
		public void SetRowVersion(long rowVersion)
		{
			RowVersion = rowVersion;
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			var instance = (BaseVersionedModel)base.Clone();
			instance.SetRowVersion(RowVersion);
			return instance;
		}
	}
}
