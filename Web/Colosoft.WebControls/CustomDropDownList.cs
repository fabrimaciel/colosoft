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
using System.Collections.Specialized;
using System.Web;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;
using System.Security.Permissions;

namespace Colosoft.WebControls
{
	/// <summary>
	/// DropDownList customizada.
	/// </summary>
	[ValidationProperty("SelectedItem"), SupportsEventValidation, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class CustomDropDownList : CustomListControl, IPostBackDataHandler
	{
		/// <summary>
		/// Cor da borda.
		/// </summary>
		[Browsable(false)]
		public override Color BorderColor
		{
			get
			{
				return base.BorderColor;
			}
			set
			{
				base.BorderColor = value;
			}
		}

		/// <summary>
		/// Estilo da borda.
		/// </summary>
		[Browsable(false)]
		public override BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		/// <summary>
		/// Largura da borda.
		/// </summary>
		[Browsable(false)]
		public override Unit BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				base.BorderWidth = value;
			}
		}

		/// <summary>
		/// Indice selecionado.
		/// </summary>
		[DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SelectedIndex
		{
			get
			{
				int selectedIndex = base.SelectedIndex;
				if((selectedIndex < 0) && (this.Items.Count > 0))
				{
					this.Items[0].Selected = true;
					selectedIndex = 0;
				}
				return selectedIndex;
			}
			set
			{
				base.SelectedIndex = value;
			}
		}

		/// <summary>
		/// Lista dos indices selecionados.
		/// </summary>
		internal override ArrayList SelectedIndicesInternal
		{
			get
			{
				int selectedIndex = this.SelectedIndex;
				return base.SelectedIndicesInternal;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CustomDropDownList() : base()
		{
		}

		/// <summary>
		/// Inicializa o control
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			if(!this.LoadPostData(UniqueID, Context.Request.Form))
				this.LoadPostData(UniqueID, Context.Request.QueryString);
			base.OnInit(e);
		}

		/// <summary>
		/// Adiciona o atributos que serão renderizados com o controle.
		/// </summary>
		/// <param name="writer"></param>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			string uniqueID = this.UniqueID;
			if(uniqueID != null)
				writer.AddAttribute(HtmlTextWriterAttribute.Name, uniqueID);
			base.AddAttributesToRender(writer);
		}

		/// <summary>
		/// Cria um contro para a coleção.
		/// </summary>
		/// <returns></returns>
		protected override ControlCollection CreateControlCollection()
		{
			return new EmptyControlCollection(this);
		}

		/// <summary>
		/// Verifica se suporte multi-seleção.
		/// </summary>
		protected override void VerifyMultiSelect()
		{
			throw new HttpException(string.Format("Cannot have multiple items selected in a {0}.", "DropDownList"));
		}

		/// <summary>
		/// Valida o evento.
		/// </summary>
		/// <param name="uniqueID"></param>
		/// <param name="eventArgument"></param>
		private void ValidateEvent(string uniqueID, string eventArgument)
		{
		}

		/// <summary>
		/// Localiza pelo valor interno.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="includeDisabled"></param>
		/// <returns></returns>
		public int FindByValueInternal(string value, bool includeDisabled)
		{
			int num = 0;
			foreach (ListItem item in this.Items)
			{
				if(item.Value.Equals(value) && (includeDisabled || item.Enabled))
					return num;
				num++;
			}
			return -1;
		}

		/// <summary>
		/// Carrega o valor postado.
		/// </summary>
		/// <param name="postDataKey"></param>
		/// <param name="postCollection"></param>
		/// <returns></returns>
		public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			string[] values = postCollection.GetValues(postDataKey);
			EnsureDataBound();
			if(values != null)
			{
				ValidateEvent(postDataKey, values[0]);
				int selectedIndex = FindByValueInternal(values[0], false);
				if(this.SelectedIndex != selectedIndex)
				{
					base.SetPostDataSelection(selectedIndex);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RaisePostDataChangedEvent()
		{
			this.OnSelectedIndexChanged(EventArgs.Empty);
		}
	}
}
