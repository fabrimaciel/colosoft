using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Colosoft.Security.Profile;
namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference{
	[Serializable]
	public sealed class Profile : IXmlSerializable	{
		public int ProfileId {
			get;
			set;
		}
		public string FullName {
			get;
			set;
		}
		public bool IsAnonymous {
			get;
			set;
		}
		public ProfileSearchMode SearchMode {
			get;
			set;
		}
		public DateTimeOffset LastActivityDate {
			get;
			set;
		}
		public DateTimeOffset LastUpdatedDate {
			get;
			set;
		}
		public string UserName {
			get;
			set;
		}
		public AuthenticationSource Source {
			get;
			set;
		}
		public ProfileRoleSet RoleSet {
			get;
			set;
		}
		public ProfileProperty[] Properties {
			get;
			set;
		}
		public int SourceId {
			get;
			set;
		}
		public int UserId {
			get;
			set;
		}
		public int GroupRoleId {
			get;
			set;
		}
		public int DefaultValueGroupId {
			get;
			set;
		}
		public int? MarkGroupId {
			get;
			set;
		}
		public int? SellerTreeId {
			get;
			set;
		}
		public int? IntermediateId {
			get;
			set;
		}
		public Profile ()		{
		}
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema ()		{
			throw new NotImplementedException ();
		}
		void IXmlSerializable.ReadXml (System.Xml.XmlReader a)		{
			a.ReadStartElement ();
			DefaultValueGroupId = int.Parse (a.ReadElementString ("DefaultValueGroupId"));
			FullName = a.ReadElementString ("FullName");
			GroupRoleId = int.Parse (a.ReadElementString ("GroupRoleId"));
			IsAnonymous = a.ReadElementContentAsBoolean ();
			ProfileSearchMode b = ProfileSearchMode.All;
			if (Enum.TryParse<ProfileSearchMode> (a.ReadElementString ("SearchMode"), out b))
				SearchMode = b;
			LastActivityDate = a.ReadDateTimeOffset ();
			LastUpdatedDate = a.ReadDateTimeOffset ();
			ProfileId = a.ReadElementContentAsInt ();
			if (!a.IsEmptyElement) {
				a.ReadStartElement ("Properties");
				var c = new List<ProfileProperty> ();
				var d = new XmlSerializer (typeof(ProfileProperty), new XmlRootAttribute ("ProfileProperty"));
				while (a.LocalName == "ProfileProperty" && a.NodeType != System.Xml.XmlNodeType.EndElement) {
					c.Add ((ProfileProperty)d.Deserialize (a));
				}
				this.Properties = c.ToArray ();
				a.ReadEndElement ();
			}
			else
				a.Skip ();
			if (!a.IsEmptyElement) {
				RoleSet = new ProfileRoleSet ();
				((IXmlSerializable)RoleSet).ReadXml (a);
			}
			else
				a.Skip ();
			if (!a.IsEmptyElement) {
				Source = new AuthenticationSource ();
				((IXmlSerializable)Source).ReadXml (a);
			}
			else
				a.Skip ();
			SourceId = a.ReadElementContentAsInt ();
			UserId = a.ReadElementContentAsInt ();
			UserName = a.ReadElementString ("UserName");
			if (!a.IsEmptyElement)
				MarkGroupId = a.ReadElementContentAsInt ();
			else
				a.Skip ();
			if (!a.IsEmptyElement)
				SellerTreeId = a.ReadElementContentAsInt ();
			else
				a.Skip ();
			if (!a.IsEmptyElement)
				IntermediateId = a.ReadElementContentAsInt ();
			else
				a.Skip ();
			a.ReadEndElement ();
		}
		void IXmlSerializable.WriteXml (System.Xml.XmlWriter a)		{
			a.WriteElementString ("DefaultValueGroupId", DefaultValueGroupId.ToString ());
			a.WriteElementString ("FullName", FullName);
			a.WriteElementString ("GroupRoleId", GroupRoleId.ToString ());
			a.WriteStartElement ("IsAnonymous");
			a.WriteValue (IsAnonymous);
			a.WriteEndElement ();
			a.WriteElementString ("SearchMode", SearchMode.ToString ());
			a.WriteDateTimeOffset (LastActivityDate, "LastActivityDate");
			a.WriteDateTimeOffset (LastUpdatedDate, "LastUpdatedDate");
			a.WriteElementString ("ProfileId", ProfileId.ToString ());
			a.WriteStartElement ("Properties");
			if (Properties != null) {
				var b = new XmlSerializer (typeof(ProfileProperty), new XmlRootAttribute ("ProfileProperty"));
				foreach (IXmlSerializable i in Properties)
					b.Serialize (a, i);
			}
			a.WriteEndElement ();
			a.WriteStartElement ("RoleSet");
			if (RoleSet != null)
				((IXmlSerializable)RoleSet).WriteXml (a);
			a.WriteEndElement ();
			a.WriteStartElement ("Source");
			if (Source != null)
				((IXmlSerializable)Source).WriteXml (a);
			a.WriteEndElement ();
			a.WriteElementString ("SourceId", SourceId.ToString ());
			a.WriteElementString ("UserId", UserId.ToString ());
			a.WriteElementString ("UserName", UserName);
			a.WriteStartElement ("MarkGroupId");
			if (MarkGroupId.HasValue)
				a.WriteValue (MarkGroupId.Value);
			a.WriteEndElement ();
			a.WriteStartElement ("SellerTreeId");
			if (SellerTreeId.HasValue)
				a.WriteValue (SellerTreeId.Value);
			a.WriteEndElement ();
			a.WriteStartElement ("IntermediateId");
			if (IntermediateId.HasValue)
				a.WriteValue (IntermediateId.Value);
			a.WriteEndElement ();
		}
	}
}
