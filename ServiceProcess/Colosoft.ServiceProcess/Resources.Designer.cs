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

namespace Colosoft.ServiceProcess.Properties
{
	using System;

	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources
	{
		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources()
		{
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if(object.ReferenceEquals(resourceMan, null))
				{
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Colosoft.ServiceProcess.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Build Service experienced http communication failure. The service will be stopped.
		///You need to reconfigure the service through the Administration Console.
		///Details: {0}.
		///.
		/// </summary>
		internal static string AddressAccessDeniedException
		{
			get
			{
				return ResourceManager.GetString("AddressAccessDeniedException", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Could not open http channel.
		/// </summary>
		internal static string CouldNotOpenHttpServiceHost
		{
			get
			{
				return ResourceManager.GetString("CouldNotOpenHttpServiceHost", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to &apos;{0}&apos; must be a non-abstract class with a parameterless or default constructor in order to use it as parameter &apos;T&apos; in GetService&lt;T&gt;()..
		/// </summary>
		internal static string GetServiceArgumentError
		{
			get
			{
				return ResourceManager.GetString("GetServiceArgumentError", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Http communication failure: 
		///{0}
		///Build service will be stopped..
		/// </summary>
		internal static string HTTPCommunicationFailure
		{
			get
			{
				return ResourceManager.GetString("HTTPCommunicationFailure", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to HttpService cannot be null..
		/// </summary>
		internal static string HttpServiceHost_HttpServiceCannotBeNull
		{
			get
			{
				return ResourceManager.GetString("HttpServiceHost_HttpServiceCannotBeNull", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to ServiceHost description behavior cannot be null..
		/// </summary>
		internal static string HttpServiceHost_ServiceHostDescriptionBehaviorCannotBeNull
		{
			get
			{
				return ResourceManager.GetString("HttpServiceHost_ServiceHostDescriptionBehaviorCannotBeNull", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to ServiceHost description cannot be null..
		/// </summary>
		internal static string HttpServiceHost_ServiceHostDescriptionCannotBeNull
		{
			get
			{
				return ResourceManager.GetString("HttpServiceHost_ServiceHostDescriptionCannotBeNull", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to ServiceHost stops. {0}.
		/// </summary>
		internal static string HttpServiceHost_ServiceHostStops
		{
			get
			{
				return ResourceManager.GetString("HttpServiceHost_ServiceHostStops", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An unhandled exception occurred..
		/// </summary>
		internal static string UnhandledExceptionError
		{
			get
			{
				return ResourceManager.GetString("UnhandledExceptionError", resourceCulture);
			}
		}
	}
}
