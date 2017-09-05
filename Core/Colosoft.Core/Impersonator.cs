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

namespace Colosoft.Net
{
	/// <summary>
	/// Allows code to be executed under the security context of a specified user account.
	/// </summary>
	/// <remarks> 
	/// Implements IDispose, so can be used via a using-directive or method calls;
	///  ...
	///
	///  var imp = new Impersonator( "myUsername", "myDomainname", "myPassword" );
	///  imp.UndoImpersonation();
	///
	///  ...
	///
	///   var imp = new Impersonator();
	///  imp.Impersonate("myUsername", "myDomainname", "myPassword");
	///  imp.UndoImpersonation();
	///
	///  ...
	///
	///  using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
	///  {
	///   ...
	///   1
	///   ...
	///  }
	///
	///  ...
	/// </remarks>
	public class Impersonator : IDisposable
	{
		private System.Security.Principal.WindowsImpersonationContext _wic;

		/// <summary>
		/// Begins impersonation with the given credentials, Logon type and Logon provider.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="domainName">Name of the domain.</param>
		/// <param name="password">The password. <see cref="System.String"/></param>
		/// <param name="logonType">Type of the logon.</param>
		/// <param name="logonProvider">The logon provider.</param>
		public Impersonator(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
		{
			Impersonate(userName, domainName, password, logonType, logonProvider);
		}

		/// <summary>
		/// Begins impersonation with the given credentials.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="domainName">Name of the domain.</param>
		/// <param name="password">The password. <see cref="System.String"/></param>
		public Impersonator(string userName, string domainName, string password)
		{
			Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Impersonator"/> class.
		/// </summary>
		public Impersonator()
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			UndoImpersonation();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Impersonates the specified user account.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="domainName">Name of the domain.</param>
		/// <param name="password">The password. <see cref="System.String"/></param>
		public void Impersonate(string userName, string domainName, string password)
		{
			Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
		}

		/// <summary>
		/// Impersonates the specified user account.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="domainName">Name of the domain.</param>
		/// <param name="password">The password. <see cref="System.String"/></param>
		/// <param name="logonType">Type of the logon.</param>
		/// <param name="logonProvider">The logon provider.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void Impersonate(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
		{
			UndoImpersonation();
			IntPtr logonToken = IntPtr.Zero;
			IntPtr logonTokenDuplicate = IntPtr.Zero;
			try
			{
				_wic = System.Security.Principal.WindowsIdentity.Impersonate(IntPtr.Zero);
				if(Win32NativeMethods.LogonUser(userName, domainName, password, (int)logonType, (int)logonProvider, ref logonToken) != 0)
				{
					if(Win32NativeMethods.DuplicateToken(logonToken, (int)ImpersonationLevel.SecurityImpersonation, ref logonTokenDuplicate) != 0)
					{
						var wi = new System.Security.Principal.WindowsIdentity(logonTokenDuplicate);
						wi.Impersonate();
					}
					else
						throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
				}
				else
					throw new System.ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
			}
			finally
			{
				if(logonToken != IntPtr.Zero)
					Win32NativeMethods.CloseHandle(logonToken);
				if(logonTokenDuplicate != IntPtr.Zero)
					Win32NativeMethods.CloseHandle(logonTokenDuplicate);
			}
		}

		/// <summary>
		/// Stops impersonation.
		/// </summary>
		private void UndoImpersonation()
		{
			if(_wic != null)
				_wic.Undo();
			_wic = null;
		}
	}
}
