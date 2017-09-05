using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference{
	[Serializable]
	public sealed class AuthenticationSource : IXmlSerializable	{
		public int Uid {
			get;
			set;
		}
		public string FullName {
			get;
			set;
		}
		public bool IsActive {
			get;
			set;
		}
		public AuthenticationSource ()		{
		}
		public AuthenticationSource (Colosoft.Security.IAuthenticationSource a)		{
			this.FullName = a.FullName;
		}
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema ()		{
			throw new NotImplementedException ();
		}
		void IXmlSerializable.ReadXml (System.Xml.XmlReader a)		{
			a.ReadStartElement ();
			FullName = a.ReadElementString ("FullName");
			Uid = int.Parse (a.ReadElementString ("Uid"));
			IsActive = bool.Parse (a.ReadElementString ("IsActive"));
			a.ReadEndElement ();
		}
		void IXmlSerializable.WriteXml (System.Xml.XmlWriter a)		{
			a.WriteElementString ("FullName", FullName);
			a.WriteElementString ("Uid", Uid.ToString ());
			a.WriteElementString ("IsActive", IsActive.ToString ());
		}
	}
}
