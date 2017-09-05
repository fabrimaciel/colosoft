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
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Colosoft.Web.Hosting
{
	class SimpleApplicationHost : MarshalByRefObject, System.Web.Hosting.IApplicationHost
	{
		private string _appPhysicalPath;

		private VirtualPath _appVirtualPath;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <param name="physicalPath"></param>
		public SimpleApplicationHost(VirtualPath virtualPath, string physicalPath)
		{
			if(string.IsNullOrEmpty(physicalPath))
				throw new ArgumentNullException("physicalPath");
			if(Util.FileUtil.IsSuspiciousPhysicalPath(physicalPath))
				throw new ArgumentException("Invalid parameter physicalPath");
			_appVirtualPath = virtualPath;
			_appPhysicalPath = Colosoft.Owin.Server.StringUtil.StringEndsWith(physicalPath, '\\') ? physicalPath : (physicalPath + @"\");
		}

		public string GetVirtualPath()
		{
			return this._appVirtualPath.VirtualPathString;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void MessageReceived()
		{
		}

		/// <summary>
		/// Recupera o factory da configuração do mapa de caminhos.
		/// </summary>
		/// <returns></returns>
		System.Web.Configuration.IConfigMapPathFactory IApplicationHost.GetConfigMapPathFactory()
		{
			return new SimpleConfigMapPathFactory();
		}

		/// <summary>
		/// Recupera o token de configuração.
		/// </summary>
		/// <returns></returns>
		IntPtr IApplicationHost.GetConfigToken()
		{
			return IntPtr.Zero;
		}

		/// <summary>
		/// Recupera o caminho fisico.
		/// </summary>
		/// <returns></returns>
		string IApplicationHost.GetPhysicalPath()
		{
			return this._appPhysicalPath;
		}

		/// <summary>
		/// Recupera o identificador do site.
		/// </summary>
		/// <returns></returns>
		string IApplicationHost.GetSiteID()
		{
			return "1";
		}

		/// <summary>
		/// Recupera o nome do site.
		/// </summary>
		/// <returns></returns>
		string IApplicationHost.GetSiteName()
		{
			return "DefaultSiteName";
		}
	}
}
