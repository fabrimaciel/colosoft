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
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Colosoft.WebControls.Security.Store;
using System.Threading;

namespace Colosoft.WebControls.Security
{
	/// <summary>
	/// Summary description for XmlProfileProvider
	/// TODO: implement some of the methods with respect of ProfileAuthenticationOption and result paging
	/// </summary>
	public class XmlProfileProvider : ProfileProvider
	{
		public static string DefaultFileName = "Profile.xml";

		public static string DefaultProviderName = "XmlProfileProvider";

		public static string DefaultProviderDescription = "XML Profile Provider";

		string _applicationName;

		string _fileName;

		XmlProfileStore _store;

		static readonly object _syncRoot = new object();

		/// <summary>
		/// Gets or sets the name of the currently running application.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.String"></see> that contains the application's shortened name, which does not contain a full path or extension, for example, SimpleAppSettings.</returns>
		public override string ApplicationName
		{
			get
			{
				return _applicationName;
			}
			set
			{
				_applicationName = value;
			}
		}

		/// <summary>
		/// Gets the profiles store.
		/// </summary>
		/// <value>The store.</value>
		protected XmlProfileStore Store
		{
			get
			{
				return _store ?? (_store = new XmlProfileStore(_fileName));
			}
		}

		/// <summary>
		/// Gets the sync root.
		/// </summary>
		/// <value>The sync root.</value>
		protected internal object SyncRoot
		{
			get
			{
				return _syncRoot;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlProfileProvider"/> class.
		/// </summary>
		public XmlProfileProvider()
		{
		}

		/// <summary>
		/// When overridden in a derived class, deletes all user-profile data for profiles in which the last activity date occurred before the specified date.
		/// </summary>
		/// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"></see> values, specifying whether anonymous, authenticated, or both types of profiles are deleted.</param>
		/// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"></see> that identifies which user profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"></see>  value of a user profile occurs on or before this date and time, the profile is considered inactive.</param>
		/// <returns>
		/// The number of profiles deleted from the data source.
		/// </returns>
		public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
		{
			int totalRecords = 0;
			try
			{
				lock (SyncRoot)
				{
					ProfileInfoCollection coll = GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, 0, int.MaxValue, out totalRecords);
					foreach (ProfileInfo info in coll)
					{
						this.Store.RemoveByUserName(info.UserName);
					}
					this.Store.Save();
				}
			}
			catch
			{
				throw;
			}
			return totalRecords;
		}

		/// <summary>
		/// When overridden in a derived class, deletes profile properties and information for profiles that match the supplied list of user names.
		/// </summary>
		/// <param name="usernames">A string array of user names for profiles to be deleted.</param>
		/// <returns>
		/// The number of profiles deleted from the data source.
		/// </returns>
		public override int DeleteProfiles(string[] usernames)
		{
			if(usernames == null)
				throw new ArgumentNullException("usernames");
			try
			{
				lock (SyncRoot)
				{
					foreach (string username in usernames)
					{
						this.Store.RemoveByUserName(username);
					}
				}
			}
			catch
			{
				throw;
			}
			return usernames.Length;
		}

		/// <summary>
		/// When overridden in a derived class, deletes profile properties and information for the supplied list of profiles.
		/// </summary>
		/// <param name="profiles">A <see cref="T:System.Web.Profile.ProfileInfoCollection"></see>  of information about profiles that are to be deleted.</param>
		/// <returns>
		/// The number of profiles deleted from the data source.
		/// </returns>
		public override int DeleteProfiles(ProfileInfoCollection profiles)
		{
			if(profiles == null)
				throw new ArgumentNullException("profiles");
			try
			{
				lock (SyncRoot)
				{
					foreach (ProfileInfo info in profiles)
					{
						this.Store.RemoveByUserName(info.UserName);
					}
				}
			}
			catch
			{
				throw;
			}
			return profiles.Count;
		}

		/// <summary>
		/// When overridden in a derived class, retrieves profile information for profiles in which the last activity date occurred on or before the specified date and the user name matches the specified user name.
		/// </summary>
		/// <param name="authenticationOption">One of the <see cref="T:System.Web.Profile.ProfileAuthenticationOption"></see> values, specifying whether anonymous, authenticated, or both types of profiles are returned.</param>
		/// <param name="usernameToMatch">The user name to search for.</param>
		/// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"></see> that identifies which user profiles are considered inactive. If the <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"></see> value of a user profile occurs on or before this date and time, the profile is considered inactive.</param>
		/// <param name="pageIndex">The index of the page of results to return.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">When this method returns, contains the total number of profiles.</param>
		/// <returns>
		/// A <see cref="T:System.Web.Profile.ProfileInfoCollection"></see> containing user profile information for inactive profiles where the user name matches the supplied usernameToMatch parameter.
		/// </returns>
		public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
		{
			try
			{
				lock (SyncRoot)
				{
					List<XmlProfile> profiles = this.Store.Profiles.FindAll(delegate(XmlProfile profile) {
						return (profile.LastUpdated <= userInactiveSinceDate) && (Membership.GetUser(profile.UserKey).UserName.StartsWith(usernameToMatch));
					});
					///
					totalRecords = profiles.Count;
					return CreateProfileInfoCollection(profiles);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="usernameToMatch"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			try
			{
				lock (SyncRoot)
				{
					List<XmlProfile> profiles = this.Store.Profiles.FindAll(delegate(XmlProfile profile) {
						return Membership.GetUser(profile.UserKey).UserName.StartsWith(usernameToMatch);
					});
					///
					totalRecords = profiles.Count;
					return CreateProfileInfoCollection(profiles);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="userInactiveSinceDate"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
		{
			try
			{
				lock (SyncRoot)
				{
					List<XmlProfile> profiles = this.Store.Profiles.FindAll(delegate(XmlProfile profile) {
						return profile.LastUpdated <= userInactiveSinceDate;
					});
					///
					totalRecords = profiles.Count;
					return CreateProfileInfoCollection(profiles);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="totalRecords"></param>
		/// <returns></returns>
		public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
		{
			try
			{
				lock (SyncRoot)
				{
					totalRecords = this.Store.Profiles.Count;
					return CreateProfileInfoCollection(this.Store.Profiles);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="authenticationOption"></param>
		/// <param name="userInactiveSinceDate"></param>
		/// <returns></returns>
		public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
		{
			try
			{
				int cout = 0;
				lock (SyncRoot)
				{
					GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, 0, int.MaxValue, out cout);
				}
				return cout;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
		{
			try
			{
				SettingsPropertyValueCollection coll = new SettingsPropertyValueCollection();
				if(collection.Count >= 1)
				{
					string userName = (string)context["UserName"];
					foreach (SettingsProperty prop in collection)
					{
						if(prop.SerializeAs == SettingsSerializeAs.ProviderSpecific)
						{
							if(prop.PropertyType.IsPrimitive || (prop.PropertyType == typeof(string)))
							{
								prop.SerializeAs = SettingsSerializeAs.String;
							}
							else
							{
								prop.SerializeAs = SettingsSerializeAs.Xml;
							}
						}
						coll.Add(new SettingsPropertyValue(prop));
					}
					if(!string.IsNullOrEmpty(userName))
						lock (SyncRoot)
						{
							bool isAuthenticated = Convert.ToBoolean(context["IsAuthenticated"]);
							GetPropertyValues(userName, coll, isAuthenticated);
						}
				}
				return coll;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="collection"></param>
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
		{
			try
			{
				string userName = context["UserName"] as string;
				bool isAuthenticated = Convert.ToBoolean(context["IsAuthenticated"]);
				if(!string.IsNullOrEmpty(userName) && collection.Count > 0)
				{
					string names = string.Empty;
					string valuesString = string.Empty;
					byte[] valuesBinary = null;
					/// prepare data for saving
					PrepareDataForSaving(ref names, ref valuesString, ref valuesBinary, true, collection, isAuthenticated);
					/// save data
					if(!string.IsNullOrEmpty(valuesString) || valuesBinary != null)
					{
						lock (SyncRoot)
						{
							XmlProfile profile = null;
							if(isAuthenticated)
							{
								MembershipUser user = Membership.GetUser(userName);
								if(user != null)
								{
									profile = this.Store.GetByUserKey((Guid)user.ProviderUserKey);
								}
								if(profile == null)
								{
									profile = new XmlProfile();
									profile.UserKey = (user != null) ? (Guid)user.ProviderUserKey : Guid.NewGuid();
									this.Store.Profiles.Add(profile);
								}
							}
							else
							{
								Guid g = new Guid(userName);
								profile = this.Store.GetByUserKey(g);
								if(profile == null)
								{
									profile = new XmlProfile();
									profile.UserKey = g;
									this.Store.Profiles.Add(profile);
								}
							}
							profile.LastUpdated = DateTime.Now;
							/// encode
							Encoding encoding = Encoding.UTF8;
							profile.Names = Convert.ToBase64String(encoding.GetBytes(names));
							profile.ValuesBinary = (valuesBinary != null) ? Convert.ToBase64String(valuesBinary) : "";
							profile.ValuesString = Convert.ToBase64String(encoding.GetBytes(valuesString));
							///
							this.Store.Save();
						}
					}
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="profiles"></param>
		/// <returns></returns>
		protected internal ProfileInfoCollection CreateProfileInfoCollection(List<XmlProfile> profiles)
		{
			ProfileInfoCollection coll = new ProfileInfoCollection();
			MembershipUser user;
			foreach (XmlProfile profile in profiles)
			{
				user = Membership.GetUser(profile.UserKey);
				if(user != null)
				{
					coll.Add(new ProfileInfo(user.UserName, false, user.LastActivityDate, profile.LastUpdated, profile.ValuesBinary.Length));
				}
			}
			///
			return coll;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="svc"></param>
		protected internal void GetPropertyValues(string userName, SettingsPropertyValueCollection svc, bool isAuthenticated)
		{
			XmlProfile profile = null;
			if(isAuthenticated)
				profile = this.Store.GetByUserName(userName);
			else
			{
				Guid g = new Guid(userName);
				profile = this.Store.GetByUserKey(g);
			}
			if(profile != null)
			{
				/// decode
				Encoding encoding = Encoding.UTF8;
				string[] names = encoding.GetString(Convert.FromBase64String(profile.Names)).Split(':');
				byte[] valuesBinary = null;
				if(!string.IsNullOrEmpty(profile.ValuesBinary))
				{
					valuesBinary = Convert.FromBase64String(profile.ValuesBinary);
				}
				string valuesString = null;
				if(!string.IsNullOrEmpty(profile.ValuesString))
				{
					valuesString = encoding.GetString(Convert.FromBase64String(profile.ValuesString));
				}
				///
				ParseProfileData(names, valuesString, valuesBinary, svc);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="names"></param>
		/// <param name="values"></param>
		/// <param name="buf"></param>
		/// <param name="properties"></param>
		protected internal void ParseProfileData(string[] names, string values, byte[] buf, SettingsPropertyValueCollection properties)
		{
			if(((names != null) && (values != null)) || ((buf != null) && (properties != null)))
			{
				try
				{
					for(int num1 = 0; num1 < (names.Length / 4); num1++)
					{
						string text1 = names[num1 * 4];
						SettingsPropertyValue value1 = properties[text1];
						if(value1 != null)
						{
							int num2 = int.Parse(names[(num1 * 4) + 2], CultureInfo.InvariantCulture);
							int num3 = int.Parse(names[(num1 * 4) + 3], CultureInfo.InvariantCulture);
							if((num3 == -1) && !value1.Property.PropertyType.IsValueType)
							{
								value1.PropertyValue = null;
								value1.IsDirty = false;
								value1.Deserialized = true;
							}
							if(((names[(num1 * 4) + 1] == "S") && (num2 >= 0)) && ((num3 > 0) && (values.Length >= (num2 + num3))))
							{
								value1.SerializedValue = values.Substring(num2, num3);
							}
							if(((names[(num1 * 4) + 1] == "B") && (num2 >= 0)) && ((num3 > 0) && (buf.Length >= (num2 + num3))))
							{
								byte[] buffer1 = new byte[num3];
								Buffer.BlockCopy(buf, num2, buffer1, 0, num3);
								value1.SerializedValue = buffer1;
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="allNames"></param>
		/// <param name="allValues"></param>
		/// <param name="buf"></param>
		/// <param name="binarySupported"></param>
		/// <param name="properties"></param>
		/// <param name="userIsAuthenticated"></param>
		protected internal void PrepareDataForSaving(ref string allNames, ref string allValues, ref byte[] buf, bool binarySupported, SettingsPropertyValueCollection properties, bool userIsAuthenticated)
		{
			StringBuilder builder1 = new StringBuilder();
			StringBuilder builder2 = new StringBuilder();
			MemoryStream stream1 = binarySupported ? new MemoryStream() : null;
			try
			{
				try
				{
					bool flag1 = false;
					foreach (SettingsPropertyValue value1 in properties)
					{
						if(!value1.IsDirty)
						{
							continue;
						}
						if(userIsAuthenticated || ((bool)value1.Property.Attributes["AllowAnonymous"]))
						{
							flag1 = true;
							break;
						}
					}
					if(!flag1)
					{
						return;
					}
					foreach (SettingsPropertyValue value2 in properties)
					{
						if(!userIsAuthenticated && !((bool)value2.Property.Attributes["AllowAnonymous"]))
						{
							continue;
						}
						if(value2.IsDirty || !value2.UsingDefaultValue)
						{
							int num1 = 0;
							int num2 = 0;
							string text1 = null;
							if(value2.Deserialized && (value2.PropertyValue == null))
							{
								num1 = -1;
							}
							else
							{
								object obj1 = value2.SerializedValue;
								if(obj1 == null)
								{
									num1 = -1;
								}
								else
								{
									if(!(obj1 is string) && !binarySupported)
									{
										obj1 = Convert.ToBase64String((byte[])obj1);
									}
									if(obj1 is string)
									{
										text1 = (string)obj1;
										num1 = text1.Length;
										num2 = builder2.Length;
									}
									else
									{
										byte[] buffer1 = (byte[])obj1;
										num2 = (int)stream1.Position;
										stream1.Write(buffer1, 0, buffer1.Length);
										stream1.Position = num2 + buffer1.Length;
										num1 = buffer1.Length;
									}
								}
							}
							string[] textArray1 = new string[8] {
								value2.Name,
								":",
								(text1 != null) ? "S" : "B",
								":",
								num2.ToString(CultureInfo.InvariantCulture),
								":",
								num1.ToString(CultureInfo.InvariantCulture),
								":"
							};
							builder1.Append(string.Concat(textArray1));
							if(text1 != null)
							{
								builder2.Append(text1);
							}
						}
					}
					if(binarySupported)
					{
						buf = stream1.ToArray();
					}
				}
				finally
				{
					if(stream1 != null)
					{
						stream1.Close();
					}
				}
			}
			catch
			{
				throw;
			}
			allNames = builder1.ToString();
			allValues = builder2.ToString();
		}

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		/// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"></see> on a provider after the provider has already been initialized.</exception>
		/// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			if(config == null)
				throw new ArgumentNullException("config");
			SecurityUtil.EnsureDataFoler();
			/// prerequisite
			if(string.IsNullOrEmpty(name))
			{
				name = DefaultProviderName;
			}
			if(string.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", DefaultProviderDescription);
			}
			base.Initialize(name, config);
			_applicationName = SecurityUtil.GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			string fileName = SecurityUtil.GetConfigValue(config["fileName"], DefaultFileName);
			_fileName = SecurityUtil.MapPath(string.Format("~/App_Data/{0}", fileName));
		}
	}
}
