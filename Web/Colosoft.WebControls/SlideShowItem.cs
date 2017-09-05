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
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Descreve os dados do item do slideshow.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter)), ParseChildren(true, "Description"), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class SlideShowItem
	{
		private string _imageUrl;

		private string _title;

		private string _description;

		private string _navigateUrl;

		/// <summary>
		/// Url da imagem do item.
		/// </summary>
		[Category("Navigate"), PersistenceMode(PersistenceMode.Attribute), DefaultValue("")]
		public string ImageUrl
		{
			get
			{
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
			}
		}

		/// <summary>
		/// Titulo do item.
		/// </summary>
		[Category("Behavior"), PersistenceMode(PersistenceMode.Attribute), DefaultValue("")]
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		/// <summary>
		/// Descricao do item.
		/// </summary>
		[System.ComponentModel.Localizable(true), PersistenceMode(PersistenceMode.InnerDefaultProperty), Category("Behavior"), DefaultValue("")]
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Url de navegacao do item.
		/// </summary>
		[Category("Navigate"), PersistenceMode(PersistenceMode.Attribute), DefaultValue("")]
		public string NavigateUrl
		{
			get
			{
				return _navigateUrl;
			}
			set
			{
				_navigateUrl = value;
			}
		}

		/// <summary>
		/// Construtor padrao.
		/// </summary>
		public SlideShowItem() : this("", "", "", "")
		{
		}

		public SlideShowItem(string imageUrl) : this("", "", imageUrl, "")
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="title">Titulo do item.</param>
		/// <param name="description">Descricao do item.</param>
		/// <param name="imageUrl">Url da imagem do item.</param>
		public SlideShowItem(string title, string description, string imageUrl) : this(title, description, imageUrl, "")
		{
		}

		/// <param name="title">Titulo do item.</param>
		/// <param name="description">Descricao do item.</param>
		/// <param name="imageUrl">Url da imagem do item.</param>
		/// <param name="navigateUrl">Url de navegacao.</param>
		public SlideShowItem(string title, string description, string imageUrl, string navigateUrl)
		{
			_title = title;
			_description = description;
			_imageUrl = imageUrl;
			_navigateUrl = navigateUrl;
		}
	}
}
