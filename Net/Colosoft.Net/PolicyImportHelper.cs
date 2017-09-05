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
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace Colosoft.ServiceModel.Channels
{
	internal static class PolicyImportHelper
	{
        internal const string SecurityPolicyNS = "http://schemas.xmlsoap.org/ws/2005/07/securitypolicy";
        internal const string PolicyNS = "http://schemas.xmlsoap.org/ws/2004/09/policy";
        internal const string MimeSerializationNS = "http://schemas.xmlsoap.org/ws/2004/09/policy/optimizedmimeserialization";
        internal const string HttpAuthNS = "http://schemas.microsoft.com/ws/06/2004/policy/http";

        internal const string FramingPolicyNS = "http://schemas.microsoft.com/ws/2006/05/framing/policy";
        internal const string NetBinaryEncodingNS = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netbinary1";

        internal const string WSSecurityNS = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

        internal static XmlElement GetTransportBindingPolicy(PolicyAssertionCollection collection)
		{
			return FindAndRemove(collection, "TransportBinding", "http://schemas.xmlsoap.org/ws/2005/07/securitypolicy");
		}

		internal static XmlElement GetStreamedMessageFramingPolicy(PolicyAssertionCollection collection)
		{
			return FindAndRemove(collection, "Streamed", "http://schemas.microsoft.com/ws/2006/05/framing/policy");
		}

		internal static XmlElement GetBinaryMessageEncodingPolicy(PolicyAssertionCollection collection)
		{
			return FindAndRemove(collection, "BinaryEncoding", "http://schemas.microsoft.com/ws/06/2004/mspolicy/netbinary1");
		}

		internal static XmlElement GetMtomMessageEncodingPolicy(PolicyAssertionCollection collection)
		{
			return FindAndRemove(collection, "OptimizedMimeSerialization", "http://schemas.xmlsoap.org/ws/2004/09/policy/optimizedmimeserialization");
		}

		static XmlElement FindAndRemove(PolicyAssertionCollection collection, string name, string ns)
		{
			var element = collection.Find(name, ns);
			if(element != null)
				collection.Remove(element);
			return element;
		}

		internal static List<XmlElement> FindAssertionByNS(PolicyAssertionCollection collection, string ns)
		{
			var list = new List<XmlElement>();
			foreach (var assertion in collection)
			{
				if(assertion.NamespaceURI.Equals(ns))
					list.Add(assertion);
			}
			return list;
		}

		internal static List<XmlElement> GetPolicyElements(XmlElement root, out bool error)
		{
			XmlElement policy = null;
			var list = new List<XmlElement>();
			foreach (var node in root.ChildNodes)
			{
				var e = node as XmlElement;
				if(e == null)
					continue;
				if(!PolicyNS.Equals(e.NamespaceURI) || !e.LocalName.Equals("Policy"))
				{
					error = true;
					return list;
				}
				if(policy != null)
				{
					error = true;
					return list;
				}
				policy = e;
			}
			if(policy == null)
			{
				error = true;
				return list;
			}
			foreach (var node in policy.ChildNodes)
			{
				var e = node as XmlElement;
				if(e != null)
					list.Add(e);
			}
			error = false;
			return list;
		}

		internal static XmlElement WrapPolicy(XmlElement element)
		{
			var policy = element.OwnerDocument.CreateElement("wsp", "Policy", PolicyNS);
			policy.AppendChild(element);
			return policy;
		}

		internal static void AddWrappedPolicyElement(XmlElement root, XmlElement element)
		{
			if(root.OwnerDocument != element.OwnerDocument)
				element = (XmlElement)root.OwnerDocument.ImportNode(element, true);
			if(!element.NamespaceURI.Equals(PolicyNS) || !element.LocalName.Equals("Policy"))
				element = WrapPolicy(element);
			root.AppendChild(element);
		}

		internal static void AddWrappedPolicyElements(XmlElement root, params XmlElement[] elements)
		{
			var policy = root.OwnerDocument.CreateElement("wsp", "Policy", PolicyNS);
			root.AppendChild(policy);
			foreach (var element in elements)
			{
				XmlElement imported;
				if(root.OwnerDocument != element.OwnerDocument)
					imported = (XmlElement)root.OwnerDocument.ImportNode(element, true);
				else
					imported = element;
				policy.AppendChild(element);
			}
		}
	}
}
