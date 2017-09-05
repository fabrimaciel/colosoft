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
using System.ServiceModel.Description;
using System.Xml;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação da extensão de importação do GZipMessageEncodingBindingElement.
	/// </summary>
	public class GZipMessageEncodingBindingElementImporter : IPolicyImportExtension
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GZipMessageEncodingBindingElementImporter()
		{
		}

		/// <summary>
		/// Executa a importação.
		/// </summary>
		/// <param name="importer"></param>
		/// <param name="context"></param>
		void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
		{
			if(importer == null)
			{
				throw new ArgumentNullException("importer");
			}
			if(context == null)
			{
				throw new ArgumentNullException("context");
			}
			ICollection<XmlElement> assertions = context.GetBindingAssertions();
			foreach (XmlElement assertion in assertions)
			{
				if((assertion.NamespaceURI == GZipMessageEncodingPolicyConstants.GZipEncodingNamespace) && (assertion.LocalName == GZipMessageEncodingPolicyConstants.GZipEncodingName))
				{
					assertions.Remove(assertion);
					context.BindingElements.Add(new GZipMessageEncodingBindingElement());
					break;
				}
			}
		}
	}
}
