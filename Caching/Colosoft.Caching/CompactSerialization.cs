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
	/// Armazena os dados da serialização compacta.
	/// </summary>
	[Serializable]
	public class CompactSerialization : ICloneable
	{
		private List<CompactClass> _compactClassList;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			CompactSerialization serialization = new CompactSerialization();
			serialization.CompactClasses = (this.CompactClasses != null) ? ((CompactClass[])this.CompactClasses.Clone()) : null;
			return serialization;
		}

		/// <summary>
		/// Classes compactars.
		/// </summary>
		[ConfigurationSection("compact-class")]
		public CompactClass[] CompactClasses
		{
			get
			{
				if(_compactClassList != null)
					return _compactClassList.ToArray();
				return null;
			}
			set
			{
				if(_compactClassList == null)
					_compactClassList = new List<CompactClass>();
				_compactClassList.Clear();
				if(value != null)
					_compactClassList.AddRange(value);
			}
		}

		/// <summary>
		/// Lista das classes associadas.
		/// </summary>
		public List<CompactClass> CompactClassList
		{
			get
			{
				return _compactClassList;
			}
			set
			{
				_compactClassList = value;
			}
		}
	}
}
