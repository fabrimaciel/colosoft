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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Compilation;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Adaptador do BuildManager.
	/// </summary>
	sealed class BuildManagerWrapper : IBuildManager
	{
		bool IBuildManager.FileExists(string virtualPath)
		{
			return BuildManager.GetObjectFactory(virtualPath, false) != null;
		}

		Type IBuildManager.GetCompiledType(string virtualPath)
		{
			return BuildManager.GetCompiledType(virtualPath);
		}

		ICollection IBuildManager.GetReferencedAssemblies()
		{
			return BuildManager.GetReferencedAssemblies();
		}

		Stream IBuildManager.ReadCachedFile(string fileName)
		{
			return BuildManager.ReadCachedFile(fileName);
		}

		Stream IBuildManager.CreateCachedFile(string fileName)
		{
			return BuildManager.CreateCachedFile(fileName);
		}
	}
}
