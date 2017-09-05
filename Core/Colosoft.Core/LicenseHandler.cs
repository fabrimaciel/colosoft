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
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Colosoft.Licensing
{
	/// <summary>
	/// Usage Guide:
	/// Command for creating the certificate
	/// >> makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=<YourCertName>" -sky Signature
	/// Then export the cert with private key from key store with a password
	/// Also export another cert with only public key
	/// </summary>
	public class LicenseHandler
	{
		public static string GenerateUID(string appName)
		{
			return HardwareInfo.GenerateUID(appName);
		}

		public static string GenerateLicenseBASE64String(LicenseEntity lic, byte[] certPrivateKeyData, SecureString certFilePwd)
		{
			XmlDocument _licenseObject = new XmlDocument();
			using (StringWriter _writer = new StringWriter())
			{
				var _serializer = new XmlSerializer(typeof(LicenseEntity), new Type[] {
					lic.GetType()
				});
				_serializer.Serialize(_writer, lic);
				_licenseObject.LoadXml(_writer.ToString());
			}
			var cert = new X509Certificate2(certPrivateKeyData, certFilePwd);
			var rsaKey = (RSACryptoServiceProvider)cert.PrivateKey;
			SignXML(_licenseObject, rsaKey);
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(_licenseObject.OuterXml));
		}

		public static LicenseEntity ParseLicenseFromBASE64String(Type licenseObjType, string licenseString, byte[] certPubKeyData, out LicenseStatus licStatus, out string validationMsg)
		{
			validationMsg = string.Empty;
			licStatus = LicenseStatus.UNDEFINED;
			if(string.IsNullOrWhiteSpace(licenseString))
			{
				licStatus = LicenseStatus.CRACKED;
				return null;
			}
			string _licXML = string.Empty;
			LicenseEntity _lic = null;
			try
			{
				X509Certificate2 cert = new X509Certificate2(certPubKeyData);
				RSACryptoServiceProvider rsaKey = (RSACryptoServiceProvider)cert.PublicKey.Key;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.PreserveWhitespace = true;
				xmlDoc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(licenseString)));
				if(VerifyXml(xmlDoc, rsaKey))
				{
					XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
					xmlDoc.DocumentElement.RemoveChild(nodeList[0]);
					_licXML = xmlDoc.OuterXml;
					XmlSerializer _serializer = new XmlSerializer(typeof(LicenseEntity), new Type[] {
						licenseObjType
					});
					using (StringReader _reader = new StringReader(_licXML))
					{
						_lic = (LicenseEntity)_serializer.Deserialize(_reader);
					}
					licStatus = _lic.DoExtraValidation(out validationMsg);
				}
				else
				{
					licStatus = LicenseStatus.INVALID;
				}
			}
			catch
			{
				licStatus = LicenseStatus.CRACKED;
			}
			return _lic;
		}

		private static void SignXML(XmlDocument xmlDoc, RSA Key)
		{
			if(xmlDoc == null)
				throw new ArgumentException("xmlDoc");
			if(Key == null)
				throw new ArgumentException("Key");
			var signedXml = new SignedXml(xmlDoc);
			signedXml.SigningKey = Key;
			Reference reference = new Reference();
			reference.Uri = "";
			XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(env);
			signedXml.AddReference(reference);
			signedXml.ComputeSignature();
			XmlElement xmlDigitalSignature = signedXml.GetXml();
			xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
		}

		private static Boolean VerifyXml(XmlDocument Doc, RSA Key)
		{
			if(Doc == null)
				throw new ArgumentException("Doc");
			if(Key == null)
				throw new ArgumentException("Key");
			SignedXml signedXml = new SignedXml(Doc);
			XmlNodeList nodeList = Doc.GetElementsByTagName("Signature");
			if(nodeList.Count <= 0)
			{
				throw new CryptographicException("Verification failed: No Signature was found in the document.");
			}
			if(nodeList.Count >= 2)
			{
				throw new CryptographicException("Verification failed: More that one signature was found for the document.");
			}
			signedXml.LoadXml((XmlElement)nodeList[0]);
			return signedXml.CheckSignature(Key);
		}

		public static bool ValidateUIDFormat(string UID)
		{
			return HardwareInfo.ValidateUIDFormat(UID);
		}
	}
}
