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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Armazena os dados do compartilhamento de dados.
	/// </summary>
	[Serializable]
	public class DataSharing : ICloneable
	{
		private List<Type> _typesList;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			DataSharing sharing = new DataSharing();
			sharing.Types = (Types != null) ? ((Type[])Types.Clone()) : null;
			return sharing;
		}

		/// <summary>
		/// Tipos que será compartilhados.
		/// </summary>
		[ConfigurationSection("type")]
		public Type[] Types
		{
			get
			{
				if(_typesList != null)
				{
					return _typesList.ToArray();
				}
				return null;
			}
			set
			{
				if(_typesList == null)
				{
					_typesList = new List<Type>();
				}
				_typesList.Clear();
				if(value != null)
				{
					_typesList.AddRange(value);
				}
			}
		}

		/// <summary>
		/// Lista dos tipos compartilhados.
		/// </summary>
		public List<Type> TypesList
		{
			get
			{
				return _typesList;
			}
			set
			{
				_typesList = value;
			}
		}
	}
}
