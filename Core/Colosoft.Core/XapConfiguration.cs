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

namespace Colosoft.IO.Xap
{
	/// <summary>
	/// Configuração do Xap.
	/// </summary>
	public class XapConfiguration : IXapConfiguration
	{
		private IAppManifestTemplate _manifestTemplate;

		private Dictionary<string, LanguageInfo> _languages;

		private string _urlPrefix;

		/// <summary>
		/// Recupera o modelo para o arquivo do Manifest.
		/// </summary>
		public IAppManifestTemplate ManifestTemplate
		{
			get
			{
				return _manifestTemplate;
			}
		}

		/// <summary>
		/// Linguagens usadas na configuração.
		/// </summary>
		public Dictionary<string, LanguageInfo> Languages
		{
			get
			{
				return _languages;
			}
		}

		/// <summary>
		/// Préfixo da url.
		/// </summary>
		public string UrlPrefix
		{
			get
			{
				return _urlPrefix;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="manifestTemplate"></param>
		/// <param name="languages"></param>
		/// <param name="urlPrefix"></param>
		public XapConfiguration(IAppManifestTemplate manifestTemplate, IEnumerable<LanguageInfo> languages, string urlPrefix)
		{
			manifestTemplate.Require("manifestTemplate").NotNull();
			languages.Require("languages").NotNull();
			_manifestTemplate = manifestTemplate;
			_languages = new Dictionary<string, LanguageInfo>();
			foreach (var i in languages)
				foreach (var ext in i.Extensions)
					_languages.Add(ext, i);
			_urlPrefix = urlPrefix;
		}
	}
}
