using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference{
	static class Helpers	{
		public static DateTimeOffset ReadDateTimeOffset (this System.Xml.XmlReader a)		{
			a.ReadStartElement ();
			a.ReadStartElement ("DateTime");
			var b = a.ReadContentAsDateTime ();
			a.ReadEndElement ();
			a.ReadStartElement ("OffsetMinutes");
			var c = (short)a.ReadContentAsInt ();
			a.ReadEndElement ();
			a.ReadEndElement ();
			return new DateTimeOffset (b, TimeSpan.FromMinutes (c));
		}
		public static void WriteDateTimeOffset (this System.Xml.XmlWriter a, DateTimeOffset b, string c)		{
			a.WriteStartElement (c);
			a.WriteStartElement ("DateTime");
			a.WriteValue (b.DateTime);
			a.WriteEndElement ();
			a.WriteStartElement ("OffsetMinutes");
			a.WriteValue (b.Offset.TotalMinutes);
			a.WriteEndElement ();
			a.WriteEndElement ();
		}
	}
}
