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
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Util
{
	class FileUtil
	{
		private static char[] _invalidFileNameChars;

		private static int _maxPathLength;

		private static readonly char[] s_invalidPathChars;

		static FileUtil()
		{
			_maxPathLength = 0x103;
			s_invalidPathChars = System.IO.Path.GetInvalidPathChars();
			_invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
		}

		[FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.PathDiscovery)]
		internal static bool IsSuspiciousPhysicalPath(string physicalPath, out bool pathTooLong)
		{
			bool flag;
			if((physicalPath != null) && ((((physicalPath.Length > _maxPathLength) || (physicalPath.IndexOfAny(s_invalidPathChars) != -1)) || ((physicalPath.Length > 0) && (physicalPath[0] == ':'))) || ((physicalPath.Length > 2) && (physicalPath.IndexOf(':', 2) > 0))))
			{
				pathTooLong = true;
				return true;
			}
			try
			{
				flag = !string.IsNullOrEmpty(physicalPath) && (string.Compare(physicalPath, System.IO.Path.GetFullPath(physicalPath), StringComparison.OrdinalIgnoreCase) > 0);
				pathTooLong = false;
			}
			catch(System.IO.PathTooLongException)
			{
				flag = true;
				pathTooLong = true;
			}
			catch(NotSupportedException)
			{
				flag = true;
				pathTooLong = true;
			}
			catch(ArgumentException)
			{
				flag = true;
				pathTooLong = true;
			}
			return flag;
		}

		internal static bool IsSuspiciousPhysicalPath(string physicalPath)
		{
			bool flag;
			if(!IsSuspiciousPhysicalPath(physicalPath, out flag))
			{
				return false;
			}
			if(flag)
			{
				if(physicalPath.IndexOf('/') >= 0)
				{
					return true;
				}
				string str = @"\..";
				int index = physicalPath.IndexOf(str, StringComparison.Ordinal);
				if((index >= 0) && ((physicalPath.Length == (index + str.Length)) || (physicalPath[index + str.Length] == '\\')))
				{
					return true;
				}
				for(int i = physicalPath.LastIndexOf('\\'); i >= 0; i = physicalPath.LastIndexOf('\\', i - 1))
				{
					if(!IsSuspiciousPhysicalPath(physicalPath.Substring(0, i), out flag))
					{
						return false;
					}
					if(!flag)
					{
						return true;
					}
				}
			}
			return true;
		}
	}
}
