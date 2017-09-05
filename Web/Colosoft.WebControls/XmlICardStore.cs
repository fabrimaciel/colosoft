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
using System.Text;

namespace Colosoft.WebControls.Security.Store
{
	/// <summary>
	/// TODO change XmlUserStore Delete to remove the associated ICard, if any.
	/// </summary>
	public class XmlICardStore : Persistable<List<XmlICard>>
	{
		/// <summary>
		/// Gets the entire collection of information cards.
		/// </summary>
		/// <value>The cards.</value>
		public virtual List<XmlICard> Cards
		{
			get
			{
				return base.Value ?? (base.Value = new List<XmlICard>());
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlICardStore"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public XmlICardStore(string fileName) : base(fileName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlICardStore"/> class.
		/// </summary>
		public XmlICardStore() : base(null)
		{
		}

		/// <summary>
		/// Associates the specified user to specified information card.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="uniqueID">The unique ID.</param>
		/// <param name="ppID">The pp ID.</param>
		public void Associate(string userName, string uniqueID, string ppID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the information card PPID by user name.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns></returns>
		public string FindPPIDForUser(string userName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the information card PPID and unique ID by user name.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="uniqueID">The unique ID.</param>
		/// <returns></returns>
		public string FindPPIDForUser(string userName, out string uniqueID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the information card by user name.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns></returns>
		public XmlICardStore GetByUser(string userName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lookups by specified unique card ID and returns the coresponding user name.
		/// </summary>
		/// <param name="uniqueID">The unique ID.</param>
		/// <returns></returns>
		public string Lookup(string uniqueID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the information card specified by its unique ID.
		/// </summary>
		/// <param name="uniqueID">The unique ID.</param>
		public void Remove(string uniqueID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the association of the specified user to specified information card.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="ppID">The pp ID.</param>
		public void UnAssociate(string userName, string ppID)
		{
			throw new NotImplementedException();
		}
	}
}
