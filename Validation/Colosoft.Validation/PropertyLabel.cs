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
using System.Linq;
using System.Text;

namespace Colosoft.Validation
{
	/// <summary>
	/// Implementação padrão da interface <see cref="IPropertyLabel"/>.
	/// </summary>
	public class PropertyLabel : IPropertyLabel
	{
		private IMessageFormattable _title;

		private IMessageFormattable _description;

		/// <summary>
		/// Título do label.
		/// </summary>
		public IMessageFormattable Title
		{
			get
			{
				return _title;
			}
		}

		/// <summary>
		/// Descrição do label.
		/// </summary>
		public IMessageFormattable Description
		{
			get
			{
				return _description;
			}
		}

		/// <summary>
		/// Constroturo padrão.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="description"></param>
		public PropertyLabel(IMessageFormattable title, IMessageFormattable description)
		{
			_title = title;
			_description = description;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (Title ?? Colosoft.MessageFormattable.Empty).Format();
		}
	}
}
