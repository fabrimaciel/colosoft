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
using System.Web.UI;
using System.ComponentModel;

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Armazena a configuração de um grupo.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter)), System.Web.AspNetHostingPermission(System.Security.Permissions.SecurityAction.LinkDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal), System.Web.AspNetHostingPermission(System.Security.Permissions.SecurityAction.InheritanceDemand, Level = System.Web.AspNetHostingPermissionLevel.Minimal)]
	public sealed class GroupSettings : IStateManager
	{
		private GroupFieldCollection _groupFields;

		private bool _isTracking;

		private StateBag _viewState = new System.Web.UI.StateBag();

		/// <summary>
		/// Identifica se o grupos pode ser encolhidos.
		/// </summary>
		[NotifyParentProperty(true), Category("Behavior"), Description("The initial state of the groups - collapsed or expanded."), DefaultValue(false)]
		public bool CollapseGroups
		{
			get
			{
				return ((this.ViewState["CollapseGroups"] != null) && ((bool)this.ViewState["CollapseGroups"]));
			}
			set
			{
				this.ViewState["CollapseGroups"] = value;
			}
		}

		/// <summary>
		/// Coleção do campos;
		/// </summary>
		[Category("Behavior"), NotifyParentProperty(true), DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty), Description("The list of fields that take part in grouping")]
		public GroupFieldCollection GroupFields
		{
			get
			{
				if(_groupFields == null)
				{
					_groupFields = new GroupFieldCollection();
					((IStateManager)_groupFields).TrackViewState();
				}
				return this._groupFields;
			}
		}

		/// <summary>
		/// Identifica se é para mostrar o resumo do grupo.
		/// </summary>
		[Description("The initial state of the groups - collapsed or expanded."), NotifyParentProperty(true), DefaultValue(false), Category("Show or hide the summary (footer) row when we collapse the group.")]
		public bool ShowSummaryOnHide
		{
			get
			{
				return ((this.ViewState["ShowSummaryOnHide"] != null) && ((bool)this.ViewState["ShowSummaryOnHide"]));
			}
			set
			{
				this.ViewState["ShowSummaryOnHide"] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		/// <summary>
		/// Estado do componente.
		/// </summary>
		private StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}

		void IStateManager.LoadViewState(object state)
		{
			if(state != null)
			{
				((IStateManager)this.ViewState).LoadViewState(state);
			}
		}

		object IStateManager.SaveViewState()
		{
			return ((IStateManager)this.ViewState).SaveViewState();
		}

		void IStateManager.TrackViewState()
		{
			this._isTracking = true;
			((IStateManager)this.ViewState).TrackViewState();
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Empty;
		}
	}
}
