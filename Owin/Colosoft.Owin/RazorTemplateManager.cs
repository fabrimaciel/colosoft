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

using RazorEngine.Templating;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Owin.Razor
{
	/// <summary>
	/// Assinatura do método usado para recupera a chave do modelo.
	/// </summary>
	/// <param name="name">Nome da modelo.</param>
	/// <param name="resolveType">Tipo de resolução. resolve</param>
	/// <param name="context"></param>
	/// <returns></returns>
	public delegate ITemplateKey TemplateGetKeyHandle (string name, ResolveType resolveType = 0, ITemplateKey context = null);
	/// <summary>
	/// Implementação do gerenciador de template.
	/// </summary>
	class RazorTemplateManager : ITemplateManager
	{
		private readonly ConcurrentDictionary<ITemplateKey, ITemplateSource> _dynamicTemplates = new ConcurrentDictionary<ITemplateKey, ITemplateSource>();

		private readonly TemplateGetKeyHandle _templateGetKey;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="templateGetKey">Instancia do método para recupera a chave do modelo.</param>
		public RazorTemplateManager(TemplateGetKeyHandle templateGetKey)
		{
			_templateGetKey = templateGetKey;
		}

		/// <summary>
		/// Adiciona um fonte dinamica.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="source"></param>
		public void AddDynamic(ITemplateKey key, ITemplateSource source)
		{
			_dynamicTemplates.AddOrUpdate(key, source, delegate(ITemplateKey k, ITemplateSource oldSource) {
				if(oldSource.Template != source.Template)
					throw new InvalidOperationException("The same key was used for another template!");
				return source;
			});
		}

		/// <summary>
		/// Recupera a chave do template.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="templateType"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public ITemplateKey GetKey(string name, ResolveType templateType, ITemplateKey context)
		{
			if(System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(name))
			{
				var virtualFile = System.Web.Hosting.HostingEnvironment.VirtualPathProvider.GetFile(name);
				var cache = virtualFile as ITemplateCacheSupport;
				RazorEngine.Templating.ITemplateKey templateKey = null;
				if(cache != null)
				{
					if(cache.TemplateKey == null)
					{
						templateKey = new NameOnlyTemplateKey(name, templateType, context);
						cache.Register(templateKey);
					}
					else
						templateKey = cache.TemplateKey;
				}
				else
					templateKey = new NameOnlyTemplateKey(name, templateType, context);
				return templateKey;
			}
			return new NameOnlyTemplateKey(name, templateType, context);
		}

		/// <summary>
		/// Resolve o nome do modelo.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string Resolve(string name)
		{
			if(System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(name))
			{
				var virtualFile = System.Web.Hosting.HostingEnvironment.VirtualPathProvider.GetFile(name);
				var cache = virtualFile as ITemplateCacheSupport;
				RazorEngine.Templating.ITemplateKey templateKey = null;
				if(cache != null)
				{
					if(cache.TemplateKey == null)
					{
						templateKey = _templateGetKey(name, RazorEngine.Templating.ResolveType.Global);
						cache.Register(templateKey);
					}
					else
						templateKey = cache.TemplateKey;
				}
				using (var stream = virtualFile.Open())
				{
					var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
					return reader.ReadToEnd();
				}
			}
			return "";
		}

		/// <summary>
		/// Resolve a chave informada.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public ITemplateSource Resolve(ITemplateKey key)
		{
			ITemplateSource source;
			if(this._dynamicTemplates.TryGetValue(key, out source))
				return source;
			var content = Resolve(key.Name);
			return new LoadedTemplateSource(content, null);
		}
	}
}
