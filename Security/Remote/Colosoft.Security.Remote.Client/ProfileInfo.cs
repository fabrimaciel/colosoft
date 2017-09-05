using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference{
	[System.Xml.Serialization.XmlRoot ("ProfileInfo")]
	[System.Xml.Serialization.XmlSchemaProvider ("GetMySchema")]
	[Serializable]
	public sealed class ProfileInfo : IXmlSerializable	{
		public int ProfileId {
			get;
			set;
		}
		public string UserName {
			get;
			set;
		}
		public string FullName {
			get;
			set;
		}
		public Colosoft.Security.Profile.ProfileSearchMode SearchMode {
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
		public AuthenticationSource Source {
			get;
			set;
		}
		public bool IsAnonymous {
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
		public ProfileInfo ()		{
		}
		public ProfileInfo (Colosoft.Security.Profile.ProfileInfo a)		{
			this.ProfileId = a.ProfileId;
			this.FullName = a.FullName;
			this.IsAnonymous = a.IsAnonymous;
			this.LastActivityDate = a.LastActivityDate;
			this.LastUpdatedDate = a.LastUpdatedDate;
			this.Source = a.Source == null ? null : new AuthenticationSource (a.Source);
			this.UserName = a.UserName;
			this.MarkGroupId = a.MarkGroupId;
		}
		public static System.Xml.XmlQualifiedName GetMySchema (System.Xml.Schema.XmlSchemaSet a)		{
			Colosoft.Security.RolePermissionNamespace.ResolveRolePermissionSchema (a);
			Namespaces.ResolveQuerySchema (a);
			return new System.Xml.XmlQualifiedName ("ProfileInfo", Namespaces.Data);
		}
		void IXmlSerializable.ReadXml (System.Xml.XmlReader a)		{
			a.ReadStartElement ();
			this.ProfileId = int.Parse (a.ReadElementString ("ProfileId"));
			this.UserName = a.ReadElementString ("UserName");
			this.FullName = a.ReadElementString ("FullName");
			var b = Security.Profile.ProfileSearchMode.All;
			if (Enum.TryParse<Colosoft.Security.Profile.ProfileSearchMode> (a.ReadElementString ("SearchMode"), out b))
				this.SearchMode = b;
			LastActivityDate = a.ReadDateTimeOffset ();
			LastUpdatedDate = a.ReadDateTimeOffset ();
			if (!a.IsEmptyElement) {
				Source = new AuthenticationSource ();
				((IXmlSerializable)this.Source).ReadXml (a);
			}
			else
				a.Skip ();
			if (a.LocalName == "IsAnonymous" && !a.IsEmptyElement)
				this.IsAnonymous = a.ReadElementContentAsBoolean ();
			else
				a.Skip ();
			if (!a.IsEmptyElement)
				this.MarkGroupId = a.ReadElementContentAsInt ();
			else
				a.Skip ();
			if (!a.IsEmptyElement)
				this.SellerTreeId = a.ReadElementContentAsInt ();
			else
				a.Skip ();
			if (!a.IsEmptyElement)
				this.IntermediateId = a.ReadElementContentAsInt ();
			else
				a.Skip ();
			a.ReadEndElement ();
		}
		void IXmlSerializable.WriteXml (System.Xml.XmlWriter a)		{
			a.WriteElementString ("ProfileId", ProfileId.ToString ());
			a.WriteElementString ("UserName", this.UserName);
			a.WriteElementString ("FullName", this.FullName);
			a.WriteElementString ("SearchMode", this.SearchMode.ToString ());
			a.WriteDateTimeOffset (LastActivityDate, "LastActivityDate");
			a.WriteDateTimeOffset (LastUpdatedDate, "LastUpdatedDate");
			a.WriteStartElement ("Source", null);
			if (Source != null)
				((IXmlSerializable)Source).WriteXml (a);
			a.WriteEndElement ();
			a.WriteStartElement ("IsAnonymous", null);
			a.WriteValue (IsAnonymous);
			a.WriteEndElement ();
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
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema ()		{
			throw new NotImplementedException ();
		}
	}
}
