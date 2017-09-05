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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Infrastructure
{
	/// <summary>
	/// Representa um grupo.
	/// </summary>
	public class Group : IGroup
	{
		private System.Collections.ObjectModel.ReadOnlyCollection<IGroup> _subgroups;

		/// <summary>
		/// Identifica se possui subgrupos.
		/// </summary>
		public bool HasSubgroups
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de itens.
		/// </summary>
		[System.Web.Script.Serialization.ScriptIgnore]
		public int ItemCount
		{
			get;
			set;
		}

		/// <summary>
		/// Itens.
		/// </summary>
		public System.Collections.IEnumerable Items
		{
			get;
			set;
		}

		/// <summary>
		/// Chave.
		/// </summary>
		public object Key
		{
			get;
			set;
		}

		/// <summary>
		/// Membro.
		/// </summary>
		public string Member
		{
			get;
			set;
		}

		/// <summary>
		/// Subgrupos;
		/// </summary>
		[System.Runtime.Serialization.IgnoreDataMember]
		public System.Collections.ObjectModel.ReadOnlyCollection<IGroup> Subgroups
		{
			get
			{
				if(_subgroups == null)
					InitializeSubgroups();
				return _subgroups;
			}
		}

		/// <summary>
		/// Inicializa os subgrupos.
		/// </summary>
		private void InitializeSubgroups()
		{
			var subgroups = new List<IGroup>();
			if(this.HasSubgroups)
				subgroups.AddRange(Items.OfType<IGroup>());
			_subgroups = new System.Collections.ObjectModel.ReadOnlyCollection<IGroup>(subgroups);
		}
	}
}
