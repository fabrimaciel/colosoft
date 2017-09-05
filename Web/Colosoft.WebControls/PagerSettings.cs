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
using System.ComponentModel;
using System.Web;
using System.Security.Permissions;
using System.Web.UI;

namespace Colosoft.WebControls.GridView
{
	/// <summary>
	/// Representa a configuração da paginação.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class PagerSettings : IStateManager
	{
		private bool _isTracking;

		private StateBag _viewState = new StateBag();

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

		public override string ToString()
		{
			return string.Empty;
		}

		[Description("Current starting page. Default is 1."), Category("Appearance"), NotifyParentProperty(true), DefaultValue(1)]
		public int CurrentPage
		{
			get
			{
				object obj2 = this.ViewState["CurrentPage"];
				if(obj2 == null)
				{
					return 1;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["CurrentPage"] = value;
			}
		}

		[Description("The message that will be shown when there are no rows in the grid."), DefaultValue(""), Category("Appearance"), NotifyParentProperty(true)]
		public string NoRowsMessage
		{
			get
			{
				object obj2 = this.ViewState["NoRowsMessage"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["NoRowsMessage"] = value;
			}
		}

		[Category("Appearance"), Description("The page size (number of rows per page). Default is 10."), DefaultValue(10), NotifyParentProperty(true)]
		public int PageSize
		{
			get
			{
				object obj2 = this.ViewState["PageSize"];
				if(obj2 == null)
				{
					return 10;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["PageSize"] = value;
			}
		}

		[DefaultValue("[10,20,30]"), Category("Appearance"), Description("The page-size options in the dropdown. Should be string in square brackers, comma separated. Default is [10,20,30]."), NotifyParentProperty(true)]
		public string PageSizeOptions
		{
			get
			{
				object obj2 = this.ViewState["PageSizeOptions"];
				if(obj2 == null)
				{
					return "[10,20,30]";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["PageSizeOptions"] = value;
			}
		}

		[NotifyParentProperty(true), Description("The message that is shown while the grid is paging."), DefaultValue(""), Category("Appearance")]
		public string PagingMessage
		{
			get
			{
				object obj2 = this.ViewState["PagingMessage"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["PagingMessage"] = value;
			}
		}

		[Category("Appearance"), Description("Enables a special paging mode - paging with scrollbar (virtual scrolling). Default is false;"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ScrollBarPaging
		{
			get
			{
				object obj2 = this.ViewState["ScrollBarPaging"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ScrollBarPaging"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		private StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}
	}
}
