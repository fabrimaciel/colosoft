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
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class SearchDialogSettings : IStateManager
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

		[Category("Appearance"), NotifyParentProperty(true), Description("Determines if the search dialog window should be draggable. Default is true."), DefaultValue(true)]
		public bool Draggable
		{
			get
			{
				object obj2 = this.ViewState["Draggable"];
				return ((obj2 == null) || ((bool)obj2));
			}
			set
			{
				this.ViewState["Draggable"] = value;
			}
		}

		[NotifyParentProperty(true), Description("The text label of the 'Find' button in search dialog. Defaults to current localization settings, setting it will override localization default."), DefaultValue(""), Category("Appearance")]
		public string FindButtonText
		{
			get
			{
				object obj2 = this.ViewState["FindButtonText"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["FindButtonText"] = value;
			}
		}

		[Description("The height of the search dialog. Default is 300. Accepts only integer numbers."), NotifyParentProperty(true), DefaultValue(300), Category("Appearance")]
		public int Height
		{
			get
			{
				object obj2 = this.ViewState["Height"];
				if(obj2 == null)
				{
					return 300;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["Height"] = value;
			}
		}

		[DefaultValue(0), NotifyParentProperty(true), Description("The left (X) offset of the dialog window, relative to the grid upper left corner. Accepts negative values. "), Category("Appearance")]
		public int LeftOffset
		{
			get
			{
				object obj2 = this.ViewState["LeftOffset"];
				if(obj2 == null)
				{
					return 0;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["LeftOffset"] = value;
			}
		}

		[DefaultValue(false), NotifyParentProperty(true), Description("Determines if the search dialog should be modal or not. Default is false."), Category("Appearance")]
		public bool Modal
		{
			get
			{
				object obj2 = this.ViewState["Modal"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["Modal"] = value;
			}
		}

		[Category("Appearance"), Description("Sets multiple search mode on. Multiple search adds the ability to search based on multiple criteria."), NotifyParentProperty(true), DefaultValue(false)]
		public bool MultipleSearch
		{
			get
			{
				object obj2 = this.ViewState["MultipleSearch"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["MultipleSearch"] = value;
			}
		}

		[NotifyParentProperty(true), Category("Appearance"), Description("The text label of the 'Reset' button in search dialog. Defaults to current localization settings, setting it will override localization default."), DefaultValue("")]
		public string ResetButtonText
		{
			get
			{
				object obj2 = this.ViewState["ResetButtonText"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["ResetButtonText"] = value;
			}
		}

		[Category("Appearance"), Description("Gets or sets a boolean property controlling if the search dialog window is resizable."), DefaultValue(true), NotifyParentProperty(true)]
		public bool Resizable
		{
			get
			{
				object obj2 = this.ViewState["Resizable"];
				return ((obj2 == null) || ((bool)obj2));
			}
			set
			{
				this.ViewState["Resizable"] = value;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTracking;
			}
		}

		[Description("The top (Y) offset of the dialog window, relative to the grid upper left corner. Accepts negative values."), Category("Appearance"), NotifyParentProperty(true), DefaultValue(0)]
		public int TopOffset
		{
			get
			{
				object obj2 = this.ViewState["TopOffset"];
				if(obj2 == null)
				{
					return 0;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["TopOffset"] = value;
			}
		}

		[DefaultValue(false), NotifyParentProperty(true), Description("Determines if validation should be applied to search fields prior to submitting to server."), Category("Appearance")]
		public bool ValidateInput
		{
			get
			{
				object obj2 = this.ViewState["ValidateInput"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ValidateInput"] = value;
			}
		}

		private StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}

		[NotifyParentProperty(true), Description("The width of the search dialog. Default is 300. Accepts only integer numbers."), DefaultValue(300), Category("Appearance")]
		public int Width
		{
			get
			{
				object obj2 = this.ViewState["Width"];
				if(obj2 == null)
				{
					return 300;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["Width"] = value;
			}
		}
	}
}
