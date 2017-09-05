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
using System.Web;
using System.Security.Permissions;
using System.Web.UI;
using System.Collections.Specialized;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Assinatura do evento acionado quando os dados estiverem sendo selecionados pelo DataSource.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void VirtualObjectDataSourceSelectingEventHandler (object sender, VirtualObjectDataSourceSelectingEventArgs e);
	/// <summary>
	/// Argumentos do evento acionado quando os dados estiverem sendo selecionaos pelo DataSource.
	/// </summary>
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class VirtualObjectDataSourceSelectingEventArgs : VirtualObjectDataSourceMethodEventArgs
	{
		/// <summary>
		/// Argumentos da seleção.
		/// </summary>
		public DataSourceSelectArguments Arguments
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se está executando do seleção da quantidade de itens.
		/// </summary>
		public bool ExecutingSelectCount
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="inputParameters"></param>
		/// <param name="arguments"></param>
		/// <param name="executingSelectCount"></param>
		public VirtualObjectDataSourceSelectingEventArgs(IOrderedDictionary inputParameters, DataSourceSelectArguments arguments, bool executingSelectCount) : base(inputParameters)
		{
			Arguments = arguments;
			ExecutingSelectCount = executingSelectCount;
		}
	}
}
