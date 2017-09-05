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
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação do elemento da extensão do comportamento do GZip.
	/// </summary>
	public class GzipBehaviorExtensionElement : BehaviorExtensionElement
	{
		/// <summary>
		/// Tipo do comportamento.
		/// </summary>
		public override Type BehaviorType
		{
			get
			{
				return typeof(GZipBehavior);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GzipBehaviorExtensionElement()
		{
		}

		/// <summary>
		/// Cria o comportamento.
		/// </summary>
		/// <returns></returns>
		protected override object CreateBehavior()
		{
			return new GZipBehavior();
		}
	}
}
