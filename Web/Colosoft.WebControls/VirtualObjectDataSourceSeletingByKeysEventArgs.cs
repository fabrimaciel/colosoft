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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Assinatura do evento acionadp quando os itens estiverem sendo selecionados
	/// pelas chaves informadas.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void VirtualObjectDataSourceSeletingByKeysEventHandler (object sender, VirtualObjectDataSourceSeletingByKeysEventArgs e);
	/// <summary>
	/// Argumentos do evento acionado quando estiver sendo selecionados
	/// os dados pelas chaves.
	/// </summary>
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class VirtualObjectDataSourceSeletingByKeysEventArgs : VirtualObjectDataSourceMethodEventArgs
	{
		private IDictionary _keys;

		/// <summary>
		/// Chaves que serão usadas na seleção.
		/// </summary>
		public IDictionary Keys
		{
			get
			{
				return _keys;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="inputParameters">Parametros de entrada.</param>
		/// <param name="keys">Chaves que serão utilizadas.</param>
		public VirtualObjectDataSourceSeletingByKeysEventArgs(IOrderedDictionary inputParameters, IDictionary keys) : base(inputParameters)
		{
			_keys = keys;
		}
	}
}
