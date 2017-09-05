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

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Armazena os dados de um campos do grupo.
	/// </summary>
	[System.Web.AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal), System.Web.AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal)]
	public class GroupField : System.Web.UI.IStateManager
	{
		private bool _isTracking;

		private System.Web.UI.StateBag _viewState = new System.Web.UI.StateBag();

		/// <summary>
		/// Nome do campo de dados.
		/// </summary>
		public string DataField
		{
			get;
			set;
		}

		/// <summary>
		/// Direção da ordenação do grupo.
		/// </summary>
		public SortDirection GroupSortDirection
		{
			get;
			set;
		}

		/// <summary>
		/// Texto do cabeçalho.
		/// </summary>
		public string HeaderText
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é para exibir a coluna do grupo.
		/// </summary>
		public bool ShowGroupColumn
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é para exibir o resumo do grupo.
		/// </summary>
		public bool ShowGroupSummary
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se suporta TrackingViewState.
		/// </summary>
		bool System.Web.UI.IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		/// <summary>
		/// Estado do controle.
		/// </summary>
		private System.Web.UI.StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GroupField()
		{
			this.DataField = "";
			this.HeaderText = "<b>{0}</b>";
			this.ShowGroupColumn = true;
			this.GroupSortDirection = SortDirection.Asc;
			this.ShowGroupSummary = false;
		}

		void System.Web.UI.IStateManager.LoadViewState(object state)
		{
			if(state != null)
				((System.Web.UI.IStateManager)this.ViewState).LoadViewState(state);
		}

		object System.Web.UI.IStateManager.SaveViewState()
		{
			return ((System.Web.UI.IStateManager)this.ViewState).SaveViewState();
		}

		void System.Web.UI.IStateManager.TrackViewState()
		{
			this._isTracking = true;
			((System.Web.UI.IStateManager)this.ViewState).TrackViewState();
		}
	}
}
