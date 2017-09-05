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

using Colosoft.Owin.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Colosoft.Web
{
	[Flags]
	internal enum VirtualPathOptions
	{
		AllowAbsolutePath = 4,
		AllowAllPath = 0x1c,
		AllowAppRelativePath = 8,
		AllowNull = 1,
		AllowRelativePath = 0x10,
		EnsureTrailingSlash = 2,
		FailIfMalformed = 0x20
	}
	/// <summary>
	/// Representa um caminho virtual.
	/// </summary>
	[Serializable]
	internal sealed class VirtualPath : IComparable
	{
		private string _appRelativeVirtualPath;

		private string _virtualPath;

		private Util.SimpleBitVector32 flags;

		internal static VirtualPath RootVirtualPath = Create("/");

		private VirtualPath()
		{
		}

		private VirtualPath(string virtualPath)
		{
			if(UrlPath.IsAppRelativePath(virtualPath))
			{
				this._appRelativeVirtualPath = virtualPath;
			}
			else
			{
				this._virtualPath = virtualPath;
			}
		}

		public VirtualPath Combine(VirtualPath relativePath)
		{
			if(relativePath == null)
			{
				throw new ArgumentNullException("relativePath");
			}
			if(!relativePath.IsRelative)
			{
				return relativePath;
			}
			this.FailIfRelativePath();
			return new VirtualPath(UrlPath.Combine(this.VirtualPathStringWhicheverAvailable, relativePath.VirtualPathString));
		}

		internal static VirtualPath Combine(VirtualPath v1, VirtualPath v2)
		{
			if(v1 == null)
			{
				v1 = new VirtualPath(HttpRuntime.AppDomainAppVirtualPath);
			}
			if(v1 == null)
			{
				v2.FailIfRelativePath();
				return v2;
			}
			return v1.Combine(v2);
		}

		public VirtualPath CombineWithAppRoot()
		{
			return new VirtualPath(HttpRuntime.AppDomainAppVirtualPath).Combine(this);
		}

		private void CopyFlagsFrom(VirtualPath virtualPath, int mask)
		{
			this.flags.IntegerValue |= virtualPath.flags.IntegerValue & mask;
		}

		public static VirtualPath Create(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAllPath);
		}

		public static VirtualPath Create(string virtualPath, VirtualPathOptions options)
		{
			if(virtualPath != null)
			{
				virtualPath = virtualPath.Trim();
			}
			if(string.IsNullOrEmpty(virtualPath))
			{
				if((options & VirtualPathOptions.AllowNull) == 0)
				{
					throw new ArgumentNullException("virtualPath");
				}
				return null;
			}
			bool flag = false;
			bool flag2 = false;
			int length = virtualPath.Length;
			{
				var chPtr = virtualPath;
				if(chPtr != null)
				{
					chPtr = chPtr.Substring(RuntimeHelpers.OffsetToStringData);
				}
				for(int i = 0; i < length; i++)
				{
					switch(chPtr[i])
					{
					case '\0':
					{
						object[] args = new object[] {
							virtualPath
						};
						throw new HttpException(string.Format("Invalid vpath {0}", args));
					}
					case '.':
						flag2 = true;
						break;
					case '/':
						if((i > 0) && (chPtr[i - 1] == '/'))
						{
							flag = true;
						}
						break;
					case '\\':
						flag = true;
						break;
					}
				}
			}
			if(flag)
			{
				if((options & VirtualPathOptions.FailIfMalformed) != 0)
				{
					object[] objArray2 = new object[] {
						virtualPath
					};
					throw new HttpException(string.Format("Invalid vpath {0}", objArray2));
				}
				virtualPath = UrlPath.FixVirtualPathSlashes(virtualPath);
			}
			if((options & VirtualPathOptions.EnsureTrailingSlash) != 0)
			{
				virtualPath = UrlPath.AppendSlashToPathIfNeeded(virtualPath);
			}
			VirtualPath path = new VirtualPath();
			if(UrlPath.IsAppRelativePath(virtualPath))
			{
				if(flag2)
				{
					virtualPath = UrlPath.ReduceVirtualPath(virtualPath);
				}
				if(virtualPath[0] == '~')
				{
					if((options & VirtualPathOptions.AllowAppRelativePath) == 0)
					{
						object[] objArray3 = new object[] {
							virtualPath
						};
						throw new ArgumentException(string.Format("VirtualPath allow app relative path {0}", objArray3));
					}
					path._appRelativeVirtualPath = virtualPath;
					return path;
				}
				if((options & VirtualPathOptions.AllowAbsolutePath) == 0)
				{
					object[] objArray4 = new object[] {
						virtualPath
					};
					throw new ArgumentException(string.Format("VirtualPath allow absolute path {0}", objArray4));
				}
				path._virtualPath = virtualPath;
				return path;
			}
			if(virtualPath[0] != '/')
			{
				if((options & VirtualPathOptions.AllowRelativePath) == 0)
				{
					object[] objArray5 = new object[] {
						virtualPath
					};
					throw new ArgumentException(string.Format("VirtualPath allow relative path {0}", objArray5));
				}
				path._virtualPath = virtualPath;
				return path;
			}
			if((options & VirtualPathOptions.AllowAbsolutePath) == 0)
			{
				object[] objArray6 = new object[] {
					virtualPath
				};
				throw new ArgumentException(string.Format("VirtualPath allow absolute path {0}", objArray6));
			}
			if(flag2)
			{
				virtualPath = UrlPath.ReduceVirtualPath(virtualPath);
			}
			path._virtualPath = virtualPath;
			return path;
		}

		public static VirtualPath CreateAbsolute(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAbsolutePath);
		}

		public static VirtualPath CreateAbsoluteAllowNull(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.AllowNull);
		}

		public static VirtualPath CreateAbsoluteTrailingSlash(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.EnsureTrailingSlash);
		}

		public static VirtualPath CreateAllowNull(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAllPath | VirtualPathOptions.AllowNull);
		}

		public static VirtualPath CreateNonRelative(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath);
		}

		public static VirtualPath CreateNonRelativeAllowNull(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.AllowNull);
		}

		public static VirtualPath CreateNonRelativeTrailingSlash(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.EnsureTrailingSlash);
		}

		public static VirtualPath CreateNonRelativeTrailingSlashAllowNull(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.EnsureTrailingSlash | VirtualPathOptions.AllowNull);
		}

		public static VirtualPath CreateTrailingSlash(string virtualPath)
		{
			return Create(virtualPath, VirtualPathOptions.AllowAllPath | VirtualPathOptions.EnsureTrailingSlash);
		}

		public bool DirectoryExists()
		{
			return System.Web.Hosting.HostingEnvironment.VirtualPathProvider.DirectoryExists(this.VirtualPathString);
		}

		public override bool Equals(object value)
		{
			if(value == null)
			{
				return false;
			}
			VirtualPath path = value as VirtualPath;
			if(path == null)
			{
				return false;
			}
			return EqualsHelper(path, this);
		}

		public static bool Equals(VirtualPath v1, VirtualPath v2)
		{
			return ((v1 == v2) || (((v1 != null) && (v2 != null)) && EqualsHelper(v1, v2)));
		}

		private static bool EqualsHelper(VirtualPath v1, VirtualPath v2)
		{
			return (StringComparer.InvariantCultureIgnoreCase.Compare(v1.VirtualPathString, v2.VirtualPathString) == 0);
		}

		internal void FailIfNotWithinAppRoot()
		{
			if(!this.IsWithinAppRoot)
			{
				object[] args = new object[] {
					this.VirtualPathString
				};
				throw new ArgumentException(string.Format("Cross app not allowed {0}", args));
			}
		}

		internal void FailIfRelativePath()
		{
			if(this.IsRelative)
			{
				object[] args = new object[] {
					this._virtualPath
				};
				throw new ArgumentException(string.Format("VirtualPath allow relative path {0}", args));
			}
		}

		public bool FileExists()
		{
			return System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(this.VirtualPathString);
		}

		internal static string GetAppRelativeVirtualPathString(VirtualPath virtualPath)
		{
			if(virtualPath != null)
			{
				return virtualPath.AppRelativeVirtualPathString;
			}
			return null;
		}

		internal static string GetAppRelativeVirtualPathStringOrEmpty(VirtualPath virtualPath)
		{
			if(virtualPath != null)
			{
				return virtualPath.AppRelativeVirtualPathString;
			}
			return string.Empty;
		}

		public CacheDependency GetCacheDependency(IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			return HostingEnvironment.VirtualPathProvider.GetCacheDependency(this.VirtualPathString, virtualPathDependencies, utcStart);
		}

		public string GetCacheKey()
		{
			return HostingEnvironment.VirtualPathProvider.GetCacheKey(this.VirtualPathString);
		}

		public VirtualDirectory GetDirectory()
		{
			return HostingEnvironment.VirtualPathProvider.GetDirectory(this.VirtualPathString);
		}

		public VirtualFile GetFile()
		{
			return HostingEnvironment.VirtualPathProvider.GetFile(this.VirtualPathString);
		}

		public string GetFileHash(IEnumerable virtualPathDependencies)
		{
			return HostingEnvironment.VirtualPathProvider.GetFileHash(this.VirtualPathString, virtualPathDependencies);
		}

		public override int GetHashCode()
		{
			return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.VirtualPathString);
		}

		internal static string GetVirtualPathString(VirtualPath virtualPath)
		{
			if(virtualPath != null)
			{
				return virtualPath.VirtualPathString;
			}
			return null;
		}

		internal static string GetVirtualPathStringNoTrailingSlash(VirtualPath virtualPath)
		{
			if(virtualPath != null)
			{
				return virtualPath.VirtualPathStringNoTrailingSlash;
			}
			return null;
		}

		public VirtualPath MakeRelative(VirtualPath toVirtualPath)
		{
			VirtualPath path = new VirtualPath();
			this.FailIfRelativePath();
			toVirtualPath.FailIfRelativePath();
			path._virtualPath = UrlPath.MakeRelative(this.VirtualPathString, toVirtualPath.VirtualPathString);
			return path;
		}

		public string MapPath()
		{
			return HostingEnvironment.MapPath(this.VirtualPathString);
		}

		public static bool operator ==(VirtualPath v1, VirtualPath v2)
		{
			return Equals(v1, v2);
		}

		public static bool operator !=(VirtualPath v1, VirtualPath v2)
		{
			return !Equals(v1, v2);
		}

		public System.IO.Stream OpenFile()
		{
			return VirtualPathProvider.OpenFile(this.VirtualPathString);
		}

		internal VirtualPath SimpleCombine(string relativePath)
		{
			return this.SimpleCombine(relativePath, false);
		}

		private VirtualPath SimpleCombine(string filename, bool addTrailingSlash)
		{
			string virtualPath = this.VirtualPathStringWhicheverAvailable + filename;
			if(addTrailingSlash)
			{
				virtualPath = virtualPath + "/";
			}
			VirtualPath path = new VirtualPath(virtualPath);
			path.CopyFlagsFrom(this, 7);
			return path;
		}

		internal VirtualPath SimpleCombineWithDir(string directoryName)
		{
			return this.SimpleCombine(directoryName, true);
		}

		int IComparable.CompareTo(object obj)
		{
			VirtualPath path = obj as VirtualPath;
			if(path == null)
			{
				throw new ArgumentException();
			}
			if(path == this)
			{
				return 0;
			}
			return StringComparer.InvariantCultureIgnoreCase.Compare(this.VirtualPathString, path.VirtualPathString);
		}

		public override string ToString()
		{
			return this.VirtualPathString;
		}

		public string AppRelativeVirtualPathString
		{
			get
			{
				string appRelativeVirtualPathStringOrNull = this.AppRelativeVirtualPathStringOrNull;
				if(appRelativeVirtualPathStringOrNull == null)
				{
					return this._virtualPath;
				}
				return appRelativeVirtualPathStringOrNull;
			}
		}

		internal string AppRelativeVirtualPathStringIfAvailable
		{
			get
			{
				return this._appRelativeVirtualPath;
			}
		}

		internal string AppRelativeVirtualPathStringOrNull
		{
			get
			{
				if(this._appRelativeVirtualPath == null)
				{
					if(this.flags[4])
					{
						return null;
					}
					_appRelativeVirtualPath = UrlPath.MakeVirtualPathAppRelativeOrNull(this._virtualPath);
					this.flags[4] = true;
					if(_appRelativeVirtualPath == null)
					{
						return null;
					}
				}
				return this._appRelativeVirtualPath;
			}
		}

		public string Extension
		{
			get
			{
				return UrlPath.GetExtension(this.VirtualPathString);
			}
		}

		public string FileName
		{
			get
			{
				return UrlPath.GetFileName(this.VirtualPathStringNoTrailingSlash);
			}
		}

		internal bool HasTrailingSlash
		{
			get
			{
				if(this._virtualPath != null)
				{
					return UrlPath.HasTrailingSlash(this._virtualPath);
				}
				return UrlPath.HasTrailingSlash(this._appRelativeVirtualPath);
			}
		}

		public bool IsRelative
		{
			get
			{
				return ((this._virtualPath != null) && (this._virtualPath[0] != '/'));
			}
		}

		public bool IsRoot
		{
			get
			{
				return (this._virtualPath == "/");
			}
		}

		public bool IsWithinAppRoot
		{
			get
			{
				if(!this.flags[1])
				{
					if(this.flags[4])
					{
						this.flags[2] = _appRelativeVirtualPath != null;
					}
					else
					{
					}
					this.flags[1] = true;
				}
				return this.flags[2];
			}
		}

		public VirtualPath Parent
		{
			get
			{
				this.FailIfRelativePath();
				if(this.IsRoot)
				{
					return null;
				}
				string virtualPathStringNoTrailingSlash = UrlPath.RemoveSlashFromPathIfNeeded(this.VirtualPathStringWhicheverAvailable);
				if(virtualPathStringNoTrailingSlash == "~")
				{
					virtualPathStringNoTrailingSlash = this.VirtualPathStringNoTrailingSlash;
				}
				int num = virtualPathStringNoTrailingSlash.LastIndexOf('/');
				if(num == 0)
				{
					return RootVirtualPath;
				}
				return new VirtualPath(virtualPathStringNoTrailingSlash.Substring(0, num + 1));
			}
		}

		public string VirtualPathString
		{
			get
			{
				if(this._virtualPath == null)
				{
					if(this._appRelativeVirtualPath.Length == 1)
					{
						this._virtualPath = HttpRuntime.AppDomainAppVirtualPath;
					}
					else
					{
						this._virtualPath = HttpRuntime.AppDomainAppVirtualPath + this._appRelativeVirtualPath.Substring(2);
					}
				}
				return this._virtualPath;
			}
		}

		internal string VirtualPathStringIfAvailable
		{
			get
			{
				return this._virtualPath;
			}
		}

		internal string VirtualPathStringNoTrailingSlash
		{
			get
			{
				return UrlPath.RemoveSlashFromPathIfNeeded(this.VirtualPathString);
			}
		}

		internal string VirtualPathStringWhicheverAvailable
		{
			get
			{
				if(this._virtualPath == null)
				{
					return this._appRelativeVirtualPath;
				}
				return this._virtualPath;
			}
		}
	}
}
