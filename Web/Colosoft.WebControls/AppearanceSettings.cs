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
using System.Web.UI;
using System.Web;
using System.Security.Permissions;

namespace Colosoft.WebControls.GridView
{
	[TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public sealed class AppearanceSettings : IStateManager
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

		[NotifyParentProperty(true), Description("Controls if the background should have alternate background for odd/even rows."), DefaultValue(false), Category("Appearance")]
		public bool AlternateRowBackground
		{
			get
			{
				object obj2 = this.ViewState["AlternateRowBackground"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["AlternateRowBackground"] = value;
			}
		}

		[Description("The caption of the grid. Appears on top and can collapse expand the grid by clicking on it."), NotifyParentProperty(true), DefaultValue(""), Category("Appearance")]
		public string Caption
		{
			get
			{
				object obj2 = this.ViewState["Caption"];
				if(obj2 == null)
				{
					return string.Empty;
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["Caption"] = value;
			}
		}

		[Description("Controls if the background color of a row will change when end-users hover the mouse over it."), Category("Appearance"), NotifyParentProperty(true), DefaultValue(false)]
		public bool HighlightRowsOnHover
		{
			get
			{
				object obj2 = this.ViewState["HighlightRowsOnHover"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["HighlightRowsOnHover"] = value;
			}
		}

		[NotifyParentProperty(true), DefaultValue(false), Category("Appearance"), Description("Sets the grid in RightToLeft (RTL) mode.")]
		public bool RightToLeft
		{
			get
			{
				object obj2 = this.ViewState["RightToLeft"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["RightToLeft"] = value;
			}
		}

		[DefaultValue(0x19), NotifyParentProperty(true), Description("Set the width of the row numbers column (ShowRowNumbers must be true). Default is 25."), Category("Appearance")]
		public int RowNumbersColumnWidth
		{
			get
			{
				object obj2 = this.ViewState["RowNumbersColumnWidth"];
				if(obj2 == null)
				{
					return 0x19;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["RowNumbersColumnWidth"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(0x12), Description("The caption of the grid. Appears on top and can collapse expand the grid by clicking on it."), NotifyParentProperty(true)]
		public int ScrollBarOffset
		{
			get
			{
				object obj2 = this.ViewState["ScrollBarOffset"];
				if(obj2 == null)
				{
					return 0x12;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["ScrollBarOffset"] = value;
			}
		}

		[Category("Appearance"), DefaultValue(false), NotifyParentProperty(true), Description("Shows an expand/collapse button in the caption of the grid which toggles grid visibility.")]
		public bool ShowCaptionGridToggleButton
		{
			get
			{
				object obj2 = this.ViewState["ShowCaptionGridToggleButton"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowCaptionGridToggleButton"] = value;
			}
		}

		[Description("Displayes a static footer for each grid column. You can store custom information there, like formulas, sum, count, etc."), Category("Appearance"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowFooter
		{
			get
			{
				object obj2 = this.ViewState["ShowFooter"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowFooter"] = value;
			}
		}

		[Description("Show row numbers (index) in the leftmost column of the grid"), Category("Appearance"), NotifyParentProperty(true), DefaultValue(false)]
		public bool ShowRowNumbers
		{
			get
			{
				object obj2 = this.ViewState["ShowRowNumbers"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ShowRowNumbers"] = value;
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
