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
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.ComponentModel;
using System.Security.Permissions;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.IO;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Controle SlideShow.
	/// </summary>
	[ParseChildren(true, "Items"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), ToolboxData("<{0}:SlideShow runat=\"server\"> </{0}:SlideShow>")]
	public class SlideShow : CompositeDataBoundControl
	{
		private PagerSettings _pagerSettings;

		private bool _allowPaging;

		private SlideShowItemCollection _items;

		private TableItemStyle _navigatorStyle;

		private string _propertyTitle;

		private string _propertyDescription;

		private string _propertyImageUrl;

		private string _propertyNavigateUrl;

		/// <summary>
		/// Nome da propriedade do DataSource que sera vinculada ao titulo do Item do SlideShow.
		/// </summary>
		[Bindable(true), Category("Data"), DefaultValue(""), Description("Nome da propriedade do DataSource que sera vinculada ao titulo do Item do SlideShow."), Localizable(true), PersistenceMode(PersistenceMode.Attribute)]
		public string PropertyTitle
		{
			get
			{
				return _propertyTitle;
			}
			set
			{
				_propertyTitle = value;
			}
		}

		/// <summary>
		/// Nome da propriedade do DataSource que sera vinculada a descricao do item no SlideShow.
		/// </summary>
		[Bindable(true), Category("Data"), DefaultValue(""), Description("Nome da propriedade do DataSource que sera vinculada a descricao do item no SlideShow."), Localizable(true), PersistenceMode(PersistenceMode.Attribute)]
		public string PropertyDescription
		{
			get
			{
				return _propertyDescription;
			}
			set
			{
				_propertyDescription = value;
			}
		}

		/// <summary>
		/// Nome da proprieade da DataSource que sera vinculada a URL da imagem do item do SlideShow.
		/// </summary>
		[Bindable(true), Category("Data"), DefaultValue(""), Description("Nome da proprieade da DataSource que sera vinculada a URL da imagem do item do SlideShow."), Localizable(true), PersistenceMode(PersistenceMode.Attribute)]
		public string PropertyImageUrl
		{
			get
			{
				return _propertyImageUrl;
			}
			set
			{
				_propertyImageUrl = value;
			}
		}

		/// <summary>
		/// Nome da propridade da DataSource que sera vinculada a navegacao do item do SlideShow.
		/// </summary>
		[Bindable(true), Category("Data"), DefaultValue(""), Description("Nome da propridade da DataSource que sera vinculada a navegacao do item do SlideShow."), Localizable(true), PersistenceMode(PersistenceMode.Attribute)]
		public string PropertyNavigateUrl
		{
			get
			{
				return _propertyNavigateUrl;
			}
			set
			{
				_propertyNavigateUrl = value;
			}
		}

		[DefaultValue((string)null), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public TableItemStyle NavigatorStyle
		{
			get
			{
				if(this._navigatorStyle == null)
				{
					this._navigatorStyle = new TableItemStyle();
					this._navigatorStyle.Height = new Unit(30, UnitType.Pixel);
					if(base.IsTrackingViewState)
					{
						((IStateManager)this._navigatorStyle).TrackViewState();
					}
				}
				return this._navigatorStyle;
			}
		}

		/// <summary>
		/// Configuracao da paginacao do controle.
		/// </summary>
		[System.ComponentModel.Category("Paging"), PersistenceMode(PersistenceMode.InnerProperty), System.ComponentModel.Description("GridView_PagerSettings"), System.ComponentModel.NotifyParentProperty(true), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public PagerSettings PagerSettings
		{
			get
			{
				if(_pagerSettings == null)
				{
					_pagerSettings = new PagerSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)this._pagerSettings).TrackViewState();
					}
					this._pagerSettings.PropertyChanged += new EventHandler(this.OnPagerPropertyChanged);
				}
				return this._pagerSettings;
			}
		}

		/// <summary>
		/// Determina se o SlideShow tera paginacao.
		/// </summary>
		public bool AllowPaging
		{
			get
			{
				return _allowPaging;
			}
			set
			{
				_allowPaging = value;
			}
		}

		/// <summary>
		/// Colecao dos itens do controle.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), DefaultValue((string)null), Category("Data"), Description("Items")]
		public SlideShowItemCollection Items
		{
			get
			{
				if(this._items == null)
				{
					this._items = new SlideShowItemCollection();
				}
				return this._items;
			}
		}

		/// <summary>
		/// Metodo acionado quando as propriedades da paginacao forem alteradas.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPagerPropertyChanged(object sender, EventArgs e)
		{
			if(base.Initialized)
			{
				base.RequiresDataBinding = true;
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Colosoft.WebControls.Resources.SlideShow.slide.js");
			StringBuilder style = new StringBuilder("<style>\r\n");
			Style navUl = new Style();
			style.Append("#" + this.ClientID + "_nav {").Append(NavigatorStyle.GetStyleAttributes(null).Value).Append("}").Append("\r\n").Append("#" + this.ClientID + "_nav ul {padding: 0px; margin: 3px 0px 0px 0px;}").Append("\r\n").Append("#" + this.ClientID + "_nav ul li { display: inline; float: left; list-style-type: none; margin: 0px; padding: 5px; }").Append("\r\n").Append("#" + this.ClientID + "_pic { margin-bottom: 3px; margin: 0px; width: ").Append(this.Width.ToString()).Append("; height: ").Append((int)(this.Height.Value - this.NavigatorStyle.Height.Value)).Append("px; }").Append("\r\n").Append("#" + this.ClientID + "_pic .img { border: 1px solid #999; padding: 3px; background: #eee; }").Append("\r\n").Append("#" + this.ClientID + "_items {visibility: hidden;}").Append("#" + this.ClientID + "_titleSlide {margin-top: 4px;}").Append("#" + this.ClientID + "_textSlide { margin-top: 6px; margin-bottom: 6px;}").Append("</style>");
			string scriptStartup = string.Format("<script>var {0} = new SlideShow(\"{0}\"); {0}.start();</script>", this.ClientID);
			this.Page.ClientScript.RegisterStartupScript(typeof(string), this.ClientID + "_script", scriptStartup);
			this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), this.ClientID + "_cssStyle", style.ToString());
		}

		protected override void Render(HtmlTextWriter writer)
		{
			CompositeDataBoundControl slideDiv = this;
			HtmlGenericControl pictureDiv = new HtmlGenericControl("div");
			pictureDiv.ID = "pic";
			HyperLink slideHyperlink = new HyperLink();
			slideHyperlink.ID = "linkSlide";
			Image slideImage = new Image();
			slideImage.ID = "slideImage";
			HtmlGenericControl titleSlideDiv = new HtmlGenericControl("div");
			titleSlideDiv.ID = "titleSlide";
			slideHyperlink.Controls.Add(slideImage);
			slideHyperlink.Controls.Add(titleSlideDiv);
			pictureDiv.Controls.Add(slideHyperlink);
			HtmlGenericControl textSlideDiv = new HtmlGenericControl("div");
			textSlideDiv.ID = "textSlide";
			pictureDiv.Controls.Add(textSlideDiv);
			HtmlGenericControl navDiv = new HtmlGenericControl("div");
			navDiv.ID = "nav";
			if(Items.Count > 0)
			{
				HtmlGenericControl pageUl = new HtmlGenericControl("ul");
				if(this.PagerSettings.Mode == PagerButtons.NextPrevious || this.PagerSettings.Mode == PagerButtons.NextPreviousFirstLast)
				{
					HtmlGenericControl previousLi = new HtmlGenericControl("li");
					HyperLink previousHyperlink = new HyperLink();
					previousHyperlink.NavigateUrl = "#";
					previousHyperlink.Attributes.Add("onclick", this.ClientID + ".previous()");
					previousHyperlink.Text = PagerSettings.PreviousPageText;
					previousLi.Controls.Add(previousHyperlink);
					pageUl.Controls.Add(previousLi);
				}
				for(int i = 1; i <= Items.Count; i++)
				{
					HtmlGenericControl itemLi = new HtmlGenericControl("li");
					HyperLink itemHyperlink = new HyperLink();
					itemHyperlink.NavigateUrl = "#";
					itemHyperlink.Attributes.Add("onclick", this.ClientID + ".position(" + (i - 1) + ")");
					itemHyperlink.Text = i.ToString();
					itemLi.Controls.Add(itemHyperlink);
					pageUl.Controls.Add(itemLi);
				}
				if(this.PagerSettings.Mode == PagerButtons.NextPrevious || this.PagerSettings.Mode == PagerButtons.NextPreviousFirstLast)
				{
					HtmlGenericControl nextLi = new HtmlGenericControl("li");
					HyperLink nextHyperlink = new HyperLink();
					nextHyperlink.NavigateUrl = "#";
					nextHyperlink.Attributes.Add("onclick", this.ClientID + ".next()");
					nextHyperlink.Text = PagerSettings.NextPageText;
					nextLi.Controls.Add(nextHyperlink);
					pageUl.Controls.Add(nextLi);
				}
				navDiv.Controls.Add(pageUl);
			}
			slideDiv.Controls.Add(pictureDiv);
			slideDiv.Controls.Add(navDiv);
			if(!this.DesignMode)
			{
				HtmlGenericControl itemsDiv = new HtmlGenericControl("div");
				itemsDiv.ID = "items";
				for(int i = 0; i < Items.Count; i++)
				{
					SlideShowItem item = Items[i];
					HtmlGenericControl itemDiv = new HtmlGenericControl("div");
					itemDiv.ID = "item" + i;
					itemDiv.Attributes.Add("image", item.ImageUrl);
					itemDiv.Attributes.Add("href", (string.IsNullOrEmpty(item.NavigateUrl) ? "#" : Page.ResolveUrl(item.NavigateUrl)));
					itemDiv.Attributes.Add("title", item.Title);
					itemDiv.InnerText = item.Description;
					itemsDiv.Controls.Add(itemDiv);
				}
				slideDiv.Controls.Add(itemsDiv);
			}
			this.RenderBeginTag(writer);
			this.RenderChildren(writer);
			this.RenderEndTag(writer);
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			if(base.DesignMode)
				writer.WriteEncodedText("vida");
		}

		protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			if(this.DesignMode)
				return 0;
			ICollection items = dataSource as ICollection;
			if(items != null && items.Count > 0)
			{
				IEnumerator ienum = items.GetEnumerator();
				ienum.Reset();
				ienum.MoveNext();
				object firstObject = ienum.Current;
				Type itemType = firstObject.GetType();
				PropertyInfo piTitle = (!string.IsNullOrEmpty(PropertyTitle) ? itemType.GetProperty(PropertyTitle) : null);
				PropertyInfo piDescription = (!string.IsNullOrEmpty(PropertyDescription) ? itemType.GetProperty(PropertyDescription) : null);
				PropertyInfo piImageUrl = (!string.IsNullOrEmpty(PropertyImageUrl) ? itemType.GetProperty(PropertyImageUrl) : null);
				PropertyInfo piNavigateUrl = (!string.IsNullOrEmpty(PropertyNavigateUrl) ? itemType.GetProperty(PropertyNavigateUrl) : null);
				foreach (object obj in items)
				{
					SlideShowItem sItem = new SlideShowItem();
					object value = null;
					if(piTitle != null)
					{
						value = piTitle.GetValue(obj, null);
						sItem.Title = (value != null ? value.ToString() : "");
					}
					if(piDescription != null)
					{
						value = piDescription.GetValue(obj, null);
						sItem.Description = (value != null ? value.ToString() : "");
					}
					if(piImageUrl != null)
					{
						value = piImageUrl.GetValue(obj, null);
						sItem.ImageUrl = (value != null ? value.ToString() : "");
					}
					if(piNavigateUrl != null)
					{
						value = piNavigateUrl.GetValue(obj, null);
						sItem.NavigateUrl = (value != null ? value.ToString() : "");
					}
					this.Items.Add(sItem);
				}
			}
			return 0;
		}
	}
}
