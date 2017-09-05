﻿/* 
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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace Colosoft.WebControls
{
	public class ExtendedHtmlUtility
	{
		/// <summary>
		/// Dicionário contendo as extensões e seus respectivos tipos de conteúdo.
		/// </summary>
		private static Dictionary<string, string> contentTypes = new Dictionary<string, string>() {
			{
				"",
				"application/octet-stream"
			},
			{
				".",
				"application/octet-stream"
			},
			{
				".323",
				"text/h323"
			},
			{
				".acx",
				"application/internet-property-stream"
			},
			{
				".ai",
				"application/postscript"
			},
			{
				".aif",
				"audio/x-aiff"
			},
			{
				".aifc",
				"audio/x-aiff"
			},
			{
				".aiff",
				"audio/x-aiff"
			},
			{
				".asf",
				"video/x-ms-asf"
			},
			{
				".asr",
				"video/x-ms-asf"
			},
			{
				".asx",
				"video/x-ms-asf"
			},
			{
				".au",
				"audio/basic"
			},
			{
				".avi",
				"video/x-msvideo"
			},
			{
				".axs",
				"application/olescript"
			},
			{
				".bas",
				"text/plain"
			},
			{
				".bcpio",
				"application/x-bcpio"
			},
			{
				".bin",
				"application/octet-stream"
			},
			{
				".bmp",
				"image/bmp"
			},
			{
				".c",
				"text/plain"
			},
			{
				".cat",
				"application/vnd.ms-pkiseccat"
			},
			{
				".cdf",
				"application/x-cdf"
			},
			{
				".cer",
				"application/x-x509-ca-cert"
			},
			{
				".class",
				"application/octet-stream"
			},
			{
				".clp",
				"application/x-msclip"
			},
			{
				".cmx",
				"image/x-cmx"
			},
			{
				".cod",
				"image/cis-cod"
			},
			{
				".cpio",
				"application/x-cpio"
			},
			{
				".crd",
				"application/x-mscardfile"
			},
			{
				".crl",
				"application/pkix-crl"
			},
			{
				".crt",
				"application/x-x509-ca-cert"
			},
			{
				".csh",
				"application/x-csh"
			},
			{
				".css",
				"text/css"
			},
			{
				".dcr",
				"application/x-director"
			},
			{
				".der",
				"application/x-x509-ca-cert"
			},
			{
				".dir",
				"application/x-director"
			},
			{
				".dll",
				"application/x-msdownload"
			},
			{
				".dms",
				"application/octet-stream"
			},
			{
				".doc",
				"application/msword"
			},
			{
				".dot",
				"application/msword"
			},
			{
				".dvi",
				"application/x-dvi"
			},
			{
				".dxr",
				"application/x-director"
			},
			{
				".eps",
				"application/postscript"
			},
			{
				".etx",
				"text/x-setext"
			},
			{
				".evy",
				"application/envoy"
			},
			{
				".exe",
				"application/octet-stream"
			},
			{
				".fif",
				"application/fractals"
			},
			{
				".flr",
				"x-world/x-vrml"
			},
			{
				".gif",
				"image/gif"
			},
			{
				".gtar",
				"application/x-gtar"
			},
			{
				".gz",
				"application/x-gzip"
			},
			{
				".h",
				"text/plain"
			},
			{
				".hdf",
				"application/x-hdf"
			},
			{
				".hlp",
				"application/winhlp"
			},
			{
				".hqx",
				"application/mac-binhex40"
			},
			{
				".hta",
				"application/hta"
			},
			{
				".htc",
				"text/x-component"
			},
			{
				".htm",
				"text/html"
			},
			{
				".html",
				"text/html"
			},
			{
				".htt",
				"text/webviewhtml"
			},
			{
				".ico",
				"image/x-icon"
			},
			{
				".ief",
				"image/ief"
			},
			{
				".iii",
				"application/x-iphone"
			},
			{
				".ins",
				"application/x-internet-signup"
			},
			{
				".isp",
				"application/x-internet-signup"
			},
			{
				".jfif",
				"image/pipeg"
			},
			{
				".jpe",
				"image/jpeg"
			},
			{
				".jpeg",
				"image/jpeg"
			},
			{
				".jpg",
				"image/jpeg"
			},
			{
				".js",
				"application/x-javascript"
			},
			{
				".latex",
				"application/x-latex"
			},
			{
				".lha",
				"application/octet-stream"
			},
			{
				".lsf",
				"video/x-la-asf"
			},
			{
				".lsx",
				"video/x-la-asf"
			},
			{
				".lzh",
				"application/octet-stream"
			},
			{
				".m13",
				"application/x-msmediaview"
			},
			{
				".m14",
				"application/x-msmediaview"
			},
			{
				".m3u",
				"audio/x-mpegurl"
			},
			{
				".man",
				"application/x-troff-man"
			},
			{
				".mdb",
				"application/x-msaccess"
			},
			{
				".me",
				"application/x-troff-me"
			},
			{
				".mht",
				"message/rfc822"
			},
			{
				".mhtml",
				"message/rfc822"
			},
			{
				".mid",
				"audio/mid"
			},
			{
				".mny",
				"application/x-msmoney"
			},
			{
				".mov",
				"video/quicktime"
			},
			{
				".movie",
				"video/x-sgi-movie"
			},
			{
				".mp2",
				"video/mpeg"
			},
			{
				".mp3",
				"audio/mpeg"
			},
			{
				".mpa",
				"video/mpeg"
			},
			{
				".mpe",
				"video/mpeg"
			},
			{
				".mpeg",
				"video/mpeg"
			},
			{
				".mpg",
				"video/mpeg"
			},
			{
				".mpp",
				"application/vnd.ms-project"
			},
			{
				".mpv2",
				"video/mpeg"
			},
			{
				".ms",
				"application/x-troff-ms"
			},
			{
				".mvb",
				"application/x-msmediaview"
			},
			{
				".nws",
				"message/rfc822"
			},
			{
				".oda",
				"application/oda"
			},
			{
				".p10",
				"application/pkcs10"
			},
			{
				".p12",
				"application/x-pkcs12"
			},
			{
				".p7b",
				"application/x-pkcs7-certificates"
			},
			{
				".p7c",
				"application/x-pkcs7-mime"
			},
			{
				".p7m",
				"application/x-pkcs7-mime"
			},
			{
				".p7r",
				"application/x-pkcs7-certreqresp"
			},
			{
				".p7s",
				"application/x-pkcs7-signature"
			},
			{
				".pbm",
				"image/x-portable-bitmap"
			},
			{
				".pdf",
				"application/pdf"
			},
			{
				".pfx",
				"application/x-pkcs12"
			},
			{
				".pgm",
				"image/x-portable-graymap"
			},
			{
				".pko",
				"application/ynd.ms-pkipko"
			},
			{
				".pma",
				"application/x-perfmon"
			},
			{
				".pmc",
				"application/x-perfmon"
			},
			{
				".pml",
				"application/x-perfmon"
			},
			{
				".pmr",
				"application/x-perfmon"
			},
			{
				".pmw",
				"application/x-perfmon"
			},
			{
				".png",
				"image/png"
			},
			{
				".pnm",
				"image/x-portable-anymap"
			},
			{
				".pot",
				"application/vnd.ms-powerpoint"
			},
			{
				".ppm",
				"image/x-portable-pixmap"
			},
			{
				".pps",
				"application/vnd.ms-powerpoint"
			},
			{
				".ppt",
				"application/vnd.ms-powerpoint"
			},
			{
				".prf",
				"application/pics-rules"
			},
			{
				".ps",
				"application/postscript"
			},
			{
				".pub",
				"application/x-mspublisher"
			},
			{
				".qt",
				"video/quicktime"
			},
			{
				".ra",
				"audio/x-pn-realaudio"
			},
			{
				".ram",
				"audio/x-pn-realaudio"
			},
			{
				".ras",
				"image/x-cmu-raster"
			},
			{
				".rgb",
				"image/x-rgb"
			},
			{
				".rmi",
				"audio/mid"
			},
			{
				".roff",
				"application/x-troff"
			},
			{
				".rtf",
				"application/rtf"
			},
			{
				".rtx",
				"text/richtext"
			},
			{
				".scd",
				"application/x-msschedule"
			},
			{
				".sct",
				"text/scriptlet"
			},
			{
				".setpay",
				"application/set-payment-initiation"
			},
			{
				".setreg",
				"application/set-registration-initiation"
			},
			{
				".sh",
				"application/x-sh"
			},
			{
				".shar",
				"application/x-shar"
			},
			{
				".sit",
				"application/x-stuffit"
			},
			{
				".snd",
				"audio/basic"
			},
			{
				".spc",
				"application/x-pkcs7-certificates"
			},
			{
				".spl",
				"application/futuresplash"
			},
			{
				".src",
				"application/x-wais-source"
			},
			{
				".sst",
				"application/vnd.ms-pkicertstore"
			},
			{
				".stl",
				"application/vnd.ms-pkistl"
			},
			{
				".stm",
				"text/html"
			},
			{
				".svg",
				"image/svg+xml"
			},
			{
				".sv4cpio",
				"application/x-sv4cpio"
			},
			{
				".sv4crc",
				"application/x-sv4crc"
			},
			{
				".swf",
				"application/x-shockwave-flash"
			},
			{
				".t",
				"application/x-troff"
			},
			{
				".tar",
				"application/x-tar"
			},
			{
				".tcl",
				"application/x-tcl"
			},
			{
				".tex",
				"application/x-tex"
			},
			{
				".texi",
				"application/x-texinfo"
			},
			{
				".texinfo",
				"application/x-texinfo"
			},
			{
				".tgz",
				"application/x-compressed"
			},
			{
				".tif",
				"image/tiff"
			},
			{
				".tiff",
				"image/tiff"
			},
			{
				".tr",
				"application/x-troff"
			},
			{
				".trm",
				"application/x-msterminal"
			},
			{
				".tsv",
				"text/tab-separated-values"
			},
			{
				".txt",
				"text/plain"
			},
			{
				".uls",
				"text/iuls"
			},
			{
				".ustar",
				"application/x-ustar"
			},
			{
				".vcf",
				"text/x-vcard"
			},
			{
				".vrml",
				"x-world/x-vrml"
			},
			{
				".wav",
				"audio/x-wav"
			},
			{
				".wcm",
				"application/vnd.ms-works"
			},
			{
				".wdb",
				"application/vnd.ms-works"
			},
			{
				".wks",
				"application/vnd.ms-works"
			},
			{
				".wmf",
				"application/x-msmetafile"
			},
			{
				".wps",
				"application/vnd.ms-works"
			},
			{
				".wri",
				"application/x-mswrite"
			},
			{
				".wrl",
				"x-world/x-vrml"
			},
			{
				".wrz",
				"x-world/x-vrml"
			},
			{
				".xaf",
				"x-world/x-vrml"
			},
			{
				".xbm",
				"image/x-xbitmap"
			},
			{
				".xla",
				"application/vnd.ms-excel"
			},
			{
				".xlc",
				"application/vnd.ms-excel"
			},
			{
				".xlm",
				"application/vnd.ms-excel"
			},
			{
				".xls",
				"application/vnd.ms-excel"
			},
			{
				".xlt",
				"application/vnd.ms-excel"
			},
			{
				".xlw",
				"application/vnd.ms-excel"
			},
			{
				".xof",
				"x-world/x-vrml"
			},
			{
				".xpm",
				"image/x-xpixmap"
			},
			{
				".xwd",
				"image/x-xwindowdump"
			},
			{
				".z",
				"application/x-compress"
			},
			{
				".zip",
				"application/zip"
			}
		};

		/// <summary>
		/// Traduz a extensão do arquivo para um content/type.
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <returns></returns>
		public static string TranslateContentType(string fileExtension)
		{
			if(!string.IsNullOrEmpty(fileExtension) && fileExtension[0] != '.')
				fileExtension = '.' + fileExtension;
			var ct = "application/octet-stream";
			contentTypes.TryGetValue(fileExtension, out ct);
			return ct;
		}

		/// <summary>
		/// Extrai as palavras do HTML.
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string[] ExtractWords(string html)
		{
			if(string.IsNullOrEmpty(html))
				return new string[] {

				};
			html = ExtendedHtmlUtility.StripHtml(html);
			Regex r = new Regex(@"\s+");
			html = r.Replace(html, " ");
			string[] wordsOnlyA = html.Split(' ');
			List<string> validWords = new List<string>();
			foreach (string word in wordsOnlyA)
			{
				string key = word.Trim(' ', '?', '\"', ',', '\'', ';', ':', '.', '(', ')').ToLower();
				if(key.Length > 0)
					validWords.Add(key);
			}
			return validWords.ToArray();
		}

		/// <summary>
		/// Recupera o texto contido no codigo html.
		/// </summary>
		/// <param name="Html"></param>
		/// <returns></returns>
		public static string StripHtml(string Html)
		{
			string scriptregex = @"<scr" + @"ipt[^>.]*>[\s\S]*?</sc" + @"ript>";
			System.Text.RegularExpressions.Regex scripts = new System.Text.RegularExpressions.Regex(scriptregex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
			string scriptless = scripts.Replace(Html, " ");
			string styleregex = @"<style[^>.]*>[\s\S]*?</style>";
			System.Text.RegularExpressions.Regex styles = new System.Text.RegularExpressions.Regex(styleregex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
			string styleless = styles.Replace(scriptless, " ");
			System.Text.RegularExpressions.Regex objRegExp = new System.Text.RegularExpressions.Regex("<(.|\n)+?>", RegexOptions.IgnoreCase);
			string strOutput = objRegExp.Replace(styleless, " ");
			strOutput = ExtendedHtmlUtility.HtmlEntityDecode(strOutput, true);
			strOutput = strOutput.Replace("<", "&lt;");
			strOutput = strOutput.Replace(">", "&gt;");
			objRegExp = null;
			return strOutput;
		}

		/// <summary>
		/// Based on the 'reflected' code (from the Framework System.Web.HttpServerUtility)
		/// listed on this page
		/// UrlEncode vs. HtmlEncode
		/// http://www.aspnetresources.com/blog/encoding_forms.aspx
		///
		/// PDF of unicode characters in the 0-127 (dec) range
		/// http://www.unicode.org/charts/PDF/U0000.pdf
		/// </summary>
		/// <param name="unicodeText"></param>
		/// <returns>
		/// &amp; becomes &amp;amp;  (encoded for XML Comments - don't be confused)
		/// 1-9a-zA-Z and some punctuation (ASCII, basically) remain unchanged
		/// </returns>
		public static string HtmlEntityEncode(string unicodeText)
		{
			return HtmlEntityEncode(unicodeText, true);
		}

		/// <param name="includeTagsEntities">whether to encode &amp; &lt; and &gt; which will
		/// cause the entire string to be 'displayable' as HTML. true is the default value.
		/// Setting this to false will result in a string where the non-ASCII characters
		/// are encoded, but HTML tags remain in-tact for display in a browser.</param>
		public static string HtmlEntityEncode(string unicodeText, bool includeTagsEntities)
		{
			int unicodeVal;
			string encoded = String.Empty;
			foreach (char c in unicodeText)
			{
				unicodeVal = c;
				switch(unicodeVal)
				{
				case '&':
					if(includeTagsEntities)
						encoded += "&amp;";
					break;
				case '<':
					if(includeTagsEntities)
						encoded += "&lt;";
					break;
				case '>':
					if(includeTagsEntities)
						encoded += "&gt;";
					break;
				default:
					if((c >= ' ') && (c <= 0x007E))
					{
						encoded += c;
					}
					else
					{
						encoded += string.Concat("&#", unicodeVal.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), ";");
					}
					break;
				}
			}
			return encoded;
		}

		/// <summary>
		/// Converts Html Entities back to their 'underlying' Unicode characters
		/// </summary>
		/// <param name="encodedText"></param>
		/// <returns>
		/// &amp;amp; becomes &amp;  (encoded for XML Comments - don't be confused)
		/// 1-9a-zA-Z and some punctuation (ASCII, basically) remain unchanged
		/// </returns>
		public static string HtmlEntityDecode(string encodedText, bool includeTagsEntities)
		{
			return entityResolver.Replace(encodedText, new MatchEvaluator(ResolveEntity));
		}

		public static string HtmlEntityDecode(string encodedText)
		{
			return entityResolver.Replace(encodedText, new MatchEvaluator(ResolveEntity));
		}

		/// <summary>
		/// Static Regular Expression to match Html Entities in encoded text
		/// </summary>
		private static Regex entityResolver = new Regex(@"([&][#](?'unicode'\d+);)|([&](?'html'\w+);)");

		/// <summary>
		/// List of entities from here
		/// http://www.vigay.com/inet/acorn/browse-html2.html#entities
		/// </summary>
		private static string[,] entityLookupArray =  {
			{
				"aacute",
				Convert.ToChar(0x00C1).ToString()
			},
			{
				"aacute",
				Convert.ToChar(0x00E1).ToString()
			},
			{
				"acirc",
				Convert.ToChar(0x00E2).ToString()
			},
			{
				"acirc",
				Convert.ToChar(0x00C2).ToString()
			},
			{
				"acute",
				Convert.ToChar(0x00B4).ToString()
			},
			{
				"aelig",
				Convert.ToChar(0x00C6).ToString()
			},
			{
				"aelig",
				Convert.ToChar(0x00E6).ToString()
			},
			{
				"agrave",
				Convert.ToChar(0x00C0).ToString()
			},
			{
				"agrave",
				Convert.ToChar(0x00E0).ToString()
			},
			{
				"alefsym",
				Convert.ToChar(0x2135).ToString()
			},
			{
				"alpha",
				Convert.ToChar(0x0391).ToString()
			},
			{
				"alpha",
				Convert.ToChar(0x03B1).ToString()
			},
			{
				"amp",
				Convert.ToChar(0x0026).ToString()
			},
			{
				"and",
				Convert.ToChar(0x2227).ToString()
			},
			{
				"ang",
				Convert.ToChar(0x2220).ToString()
			},
			{
				"aring",
				Convert.ToChar(0x00E5).ToString()
			},
			{
				"aring",
				Convert.ToChar(0x00C5).ToString()
			},
			{
				"asymp",
				Convert.ToChar(0x2248).ToString()
			},
			{
				"atilde",
				Convert.ToChar(0x00C3).ToString()
			},
			{
				"atilde",
				Convert.ToChar(0x00E3).ToString()
			},
			{
				"auml",
				Convert.ToChar(0x00E4).ToString()
			},
			{
				"auml",
				Convert.ToChar(0x00C4).ToString()
			},
			{
				"bdquo",
				Convert.ToChar(0x201E).ToString()
			},
			{
				"beta",
				Convert.ToChar(0x0392).ToString()
			},
			{
				"beta",
				Convert.ToChar(0x03B2).ToString()
			},
			{
				"brvbar",
				Convert.ToChar(0x00A6).ToString()
			},
			{
				"bull",
				Convert.ToChar(0x2022).ToString()
			},
			{
				"cap",
				Convert.ToChar(0x2229).ToString()
			},
			{
				"ccedil",
				Convert.ToChar(0x00C7).ToString()
			},
			{
				"ccedil",
				Convert.ToChar(0x00E7).ToString()
			},
			{
				"cedil",
				Convert.ToChar(0x00B8).ToString()
			},
			{
				"cent",
				Convert.ToChar(0x00A2).ToString()
			},
			{
				"chi",
				Convert.ToChar(0x03C7).ToString()
			},
			{
				"chi",
				Convert.ToChar(0x03A7).ToString()
			},
			{
				"circ",
				Convert.ToChar(0x02C6).ToString()
			},
			{
				"clubs",
				Convert.ToChar(0x2663).ToString()
			},
			{
				"cong",
				Convert.ToChar(0x2245).ToString()
			},
			{
				"copy",
				Convert.ToChar(0x00A9).ToString()
			},
			{
				"crarr",
				Convert.ToChar(0x21B5).ToString()
			},
			{
				"cup",
				Convert.ToChar(0x222A).ToString()
			},
			{
				"curren",
				Convert.ToChar(0x00A4).ToString()
			},
			{
				"dagger",
				Convert.ToChar(0x2020).ToString()
			},
			{
				"dagger",
				Convert.ToChar(0x2021).ToString()
			},
			{
				"darr",
				Convert.ToChar(0x2193).ToString()
			},
			{
				"darr",
				Convert.ToChar(0x21D3).ToString()
			},
			{
				"deg",
				Convert.ToChar(0x00B0).ToString()
			},
			{
				"delta",
				Convert.ToChar(0x0394).ToString()
			},
			{
				"delta",
				Convert.ToChar(0x03B4).ToString()
			},
			{
				"diams",
				Convert.ToChar(0x2666).ToString()
			},
			{
				"divide",
				Convert.ToChar(0x00F7).ToString()
			},
			{
				"eacute",
				Convert.ToChar(0x00E9).ToString()
			},
			{
				"eacute",
				Convert.ToChar(0x00C9).ToString()
			},
			{
				"ecirc",
				Convert.ToChar(0x00CA).ToString()
			},
			{
				"ecirc",
				Convert.ToChar(0x00EA).ToString()
			},
			{
				"egrave",
				Convert.ToChar(0x00C8).ToString()
			},
			{
				"egrave",
				Convert.ToChar(0x00E8).ToString()
			},
			{
				"empty",
				Convert.ToChar(0x2205).ToString()
			},
			{
				"emsp",
				Convert.ToChar(0x2003).ToString()
			},
			{
				"ensp",
				Convert.ToChar(0x2002).ToString()
			},
			{
				"epsilon",
				Convert.ToChar(0x03B5).ToString()
			},
			{
				"epsilon",
				Convert.ToChar(0x0395).ToString()
			},
			{
				"equiv",
				Convert.ToChar(0x2261).ToString()
			},
			{
				"eta",
				Convert.ToChar(0x0397).ToString()
			},
			{
				"eta",
				Convert.ToChar(0x03B7).ToString()
			},
			{
				"eth",
				Convert.ToChar(0x00F0).ToString()
			},
			{
				"eth",
				Convert.ToChar(0x00D0).ToString()
			},
			{
				"euml",
				Convert.ToChar(0x00CB).ToString()
			},
			{
				"euml",
				Convert.ToChar(0x00EB).ToString()
			},
			{
				"euro",
				Convert.ToChar(0x20AC).ToString()
			},
			{
				"exist",
				Convert.ToChar(0x2203).ToString()
			},
			{
				"fnof",
				Convert.ToChar(0x0192).ToString()
			},
			{
				"forall",
				Convert.ToChar(0x2200).ToString()
			},
			{
				"frac12",
				Convert.ToChar(0x00BD).ToString()
			},
			{
				"frac14",
				Convert.ToChar(0x00BC).ToString()
			},
			{
				"frac34",
				Convert.ToChar(0x00BE).ToString()
			},
			{
				"frasl",
				Convert.ToChar(0x2044).ToString()
			},
			{
				"gamma",
				Convert.ToChar(0x03B3).ToString()
			},
			{
				"gamma",
				Convert.ToChar(0x393).ToString()
			},
			{
				"ge",
				Convert.ToChar(0x2265).ToString()
			},
			{
				"gt",
				Convert.ToChar(0x003E).ToString()
			},
			{
				"harr",
				Convert.ToChar(0x21D4).ToString()
			},
			{
				"harr",
				Convert.ToChar(0x2194).ToString()
			},
			{
				"hearts",
				Convert.ToChar(0x2665).ToString()
			},
			{
				"hellip",
				Convert.ToChar(0x2026).ToString()
			},
			{
				"iacute",
				Convert.ToChar(0x00CD).ToString()
			},
			{
				"iacute",
				Convert.ToChar(0x00ED).ToString()
			},
			{
				"icirc",
				Convert.ToChar(0x00EE).ToString()
			},
			{
				"icirc",
				Convert.ToChar(0x00CE).ToString()
			},
			{
				"iexcl",
				Convert.ToChar(0x00A1).ToString()
			},
			{
				"igrave",
				Convert.ToChar(0x00CC).ToString()
			},
			{
				"igrave",
				Convert.ToChar(0x00EC).ToString()
			},
			{
				"image",
				Convert.ToChar(0x2111).ToString()
			},
			{
				"infin",
				Convert.ToChar(0x221E).ToString()
			},
			{
				"int",
				Convert.ToChar(0x222B).ToString()
			},
			{
				"iota",
				Convert.ToChar(0x0399).ToString()
			},
			{
				"iota",
				Convert.ToChar(0x03B9).ToString()
			},
			{
				"iquest",
				Convert.ToChar(0x00BF).ToString()
			},
			{
				"isin",
				Convert.ToChar(0x2208).ToString()
			},
			{
				"iuml",
				Convert.ToChar(0x00EF).ToString()
			},
			{
				"iuml",
				Convert.ToChar(0x00CF).ToString()
			},
			{
				"kappa",
				Convert.ToChar(0x03BA).ToString()
			},
			{
				"kappa",
				Convert.ToChar(0x039A).ToString()
			},
			{
				"lambda",
				Convert.ToChar(0x039B).ToString()
			},
			{
				"lambda",
				Convert.ToChar(0x03BB).ToString()
			},
			{
				"lang",
				Convert.ToChar(0x2329).ToString()
			},
			{
				"laquo",
				Convert.ToChar(0x00AB).ToString()
			},
			{
				"larr",
				Convert.ToChar(0x2190).ToString()
			},
			{
				"larr",
				Convert.ToChar(0x21D0).ToString()
			},
			{
				"lceil",
				Convert.ToChar(0x2308).ToString()
			},
			{
				"ldquo",
				Convert.ToChar(0x201C).ToString()
			},
			{
				"le",
				Convert.ToChar(0x2264).ToString()
			},
			{
				"lfloor",
				Convert.ToChar(0x230A).ToString()
			},
			{
				"lowast",
				Convert.ToChar(0x2217).ToString()
			},
			{
				"loz",
				Convert.ToChar(0x25CA).ToString()
			},
			{
				"lrm",
				Convert.ToChar(0x200E).ToString()
			},
			{
				"lsaquo",
				Convert.ToChar(0x2039).ToString()
			},
			{
				"lsquo",
				Convert.ToChar(0x2018).ToString()
			},
			{
				"lt",
				Convert.ToChar(0x003C).ToString()
			},
			{
				"macr",
				Convert.ToChar(0x00AF).ToString()
			},
			{
				"mdash",
				Convert.ToChar(0x2014).ToString()
			},
			{
				"micro",
				Convert.ToChar(0x00B5).ToString()
			},
			{
				"middot",
				Convert.ToChar(0x00B7).ToString()
			},
			{
				"minus",
				Convert.ToChar(0x2212).ToString()
			},
			{
				"mu",
				Convert.ToChar(0x039C).ToString()
			},
			{
				"mu",
				Convert.ToChar(0x03BC).ToString()
			},
			{
				"nabla",
				Convert.ToChar(0x2207).ToString()
			},
			{
				"nbsp",
				Convert.ToChar(0x00A0).ToString()
			},
			{
				"ndash",
				Convert.ToChar(0x2013).ToString()
			},
			{
				"ne",
				Convert.ToChar(0x2260).ToString()
			},
			{
				"ni",
				Convert.ToChar(0x220B).ToString()
			},
			{
				"not",
				Convert.ToChar(0x00AC).ToString()
			},
			{
				"notin",
				Convert.ToChar(0x2209).ToString()
			},
			{
				"nsub",
				Convert.ToChar(0x2284).ToString()
			},
			{
				"ntilde",
				Convert.ToChar(0x00F1).ToString()
			},
			{
				"ntilde",
				Convert.ToChar(0x00D1).ToString()
			},
			{
				"nu",
				Convert.ToChar(0x039D).ToString()
			},
			{
				"nu",
				Convert.ToChar(0x03BD).ToString()
			},
			{
				"oacute",
				Convert.ToChar(0x00F3).ToString()
			},
			{
				"oacute",
				Convert.ToChar(0x00D3).ToString()
			},
			{
				"ocirc",
				Convert.ToChar(0x00D4).ToString()
			},
			{
				"ocirc",
				Convert.ToChar(0x00F4).ToString()
			},
			{
				"oelig",
				Convert.ToChar(0x0152).ToString()
			},
			{
				"oelig",
				Convert.ToChar(0x0153).ToString()
			},
			{
				"ograve",
				Convert.ToChar(0x00F2).ToString()
			},
			{
				"ograve",
				Convert.ToChar(0x00D2).ToString()
			},
			{
				"oline",
				Convert.ToChar(0x203E).ToString()
			},
			{
				"omega",
				Convert.ToChar(0x03A9).ToString()
			},
			{
				"omega",
				Convert.ToChar(0x03C9).ToString()
			},
			{
				"omicron",
				Convert.ToChar(0x039F).ToString()
			},
			{
				"omicron",
				Convert.ToChar(0x03BF).ToString()
			},
			{
				"oplus",
				Convert.ToChar(0x2295).ToString()
			},
			{
				"or",
				Convert.ToChar(0x2228).ToString()
			},
			{
				"ordf",
				Convert.ToChar(0x00AA).ToString()
			},
			{
				"ordm",
				Convert.ToChar(0x00BA).ToString()
			},
			{
				"oslash",
				Convert.ToChar(0x00D8).ToString()
			},
			{
				"oslash",
				Convert.ToChar(0x00F8).ToString()
			},
			{
				"otilde",
				Convert.ToChar(0x00F5).ToString()
			},
			{
				"otilde",
				Convert.ToChar(0x00D5).ToString()
			},
			{
				"otimes",
				Convert.ToChar(0x2297).ToString()
			},
			{
				"ouml",
				Convert.ToChar(0x00D6).ToString()
			},
			{
				"ouml",
				Convert.ToChar(0x00F6).ToString()
			},
			{
				"para",
				Convert.ToChar(0x00B6).ToString()
			},
			{
				"part",
				Convert.ToChar(0x2202).ToString()
			},
			{
				"permil",
				Convert.ToChar(0x2030).ToString()
			},
			{
				"perp",
				Convert.ToChar(0x22A5).ToString()
			},
			{
				"phi",
				Convert.ToChar(0x03A6).ToString()
			},
			{
				"phi",
				Convert.ToChar(0x03C6).ToString()
			},
			{
				"pi",
				Convert.ToChar(0x03A0).ToString()
			},
			{
				"pi",
				Convert.ToChar(0x03C0).ToString()
			},
			{
				"piv",
				Convert.ToChar(0x03D6).ToString()
			},
			{
				"plusmn",
				Convert.ToChar(0x00B1).ToString()
			},
			{
				"pound",
				Convert.ToChar(0x00A3).ToString()
			},
			{
				"prime",
				Convert.ToChar(0x2033).ToString()
			},
			{
				"prime",
				Convert.ToChar(0x2032).ToString()
			},
			{
				"prod",
				Convert.ToChar(0x220F).ToString()
			},
			{
				"prop",
				Convert.ToChar(0x221D).ToString()
			},
			{
				"psi",
				Convert.ToChar(0x03C8).ToString()
			},
			{
				"psi",
				Convert.ToChar(0x03A8).ToString()
			},
			{
				"quot",
				Convert.ToChar(0x0022).ToString()
			},
			{
				"radic",
				Convert.ToChar(0x221A).ToString()
			},
			{
				"rang",
				Convert.ToChar(0x232A).ToString()
			},
			{
				"raquo",
				Convert.ToChar(0x00BB).ToString()
			},
			{
				"rarr",
				Convert.ToChar(0x2192).ToString()
			},
			{
				"rarr",
				Convert.ToChar(0x21D2).ToString()
			},
			{
				"rceil",
				Convert.ToChar(0x2309).ToString()
			},
			{
				"rdquo",
				Convert.ToChar(0x201D).ToString()
			},
			{
				"real",
				Convert.ToChar(0x211C).ToString()
			},
			{
				"reg",
				Convert.ToChar(0x00AE).ToString()
			},
			{
				"rfloor",
				Convert.ToChar(0x230B).ToString()
			},
			{
				"rho",
				Convert.ToChar(0x03C1).ToString()
			},
			{
				"rho",
				Convert.ToChar(0x03A1).ToString()
			},
			{
				"rlm",
				Convert.ToChar(0x200F).ToString()
			},
			{
				"rsaquo",
				Convert.ToChar(0x203A).ToString()
			},
			{
				"rsquo",
				Convert.ToChar(0x2019).ToString()
			},
			{
				"sbquo",
				Convert.ToChar(0x201A).ToString()
			},
			{
				"scaron",
				Convert.ToChar(0x0160).ToString()
			},
			{
				"scaron",
				Convert.ToChar(0x0161).ToString()
			},
			{
				"sdot",
				Convert.ToChar(0x22C5).ToString()
			},
			{
				"sect",
				Convert.ToChar(0x00A7).ToString()
			},
			{
				"shy",
				Convert.ToChar(0x00AD).ToString()
			},
			{
				"sigma",
				Convert.ToChar(0x03C3).ToString()
			},
			{
				"sigma",
				Convert.ToChar(0x03A3).ToString()
			},
			{
				"sigmaf",
				Convert.ToChar(0x03C2).ToString()
			},
			{
				"sim",
				Convert.ToChar(0x223C).ToString()
			},
			{
				"spades",
				Convert.ToChar(0x2660).ToString()
			},
			{
				"sub",
				Convert.ToChar(0x2282).ToString()
			},
			{
				"sube",
				Convert.ToChar(0x2286).ToString()
			},
			{
				"sum",
				Convert.ToChar(0x2211).ToString()
			},
			{
				"sup",
				Convert.ToChar(0x2283).ToString()
			},
			{
				"sup1",
				Convert.ToChar(0x00B9).ToString()
			},
			{
				"sup2",
				Convert.ToChar(0x00B2).ToString()
			},
			{
				"sup3",
				Convert.ToChar(0x00B3).ToString()
			},
			{
				"supe",
				Convert.ToChar(0x2287).ToString()
			},
			{
				"szlig",
				Convert.ToChar(0x00DF).ToString()
			},
			{
				"tau",
				Convert.ToChar(0x03A4).ToString()
			},
			{
				"tau",
				Convert.ToChar(0x03C4).ToString()
			},
			{
				"there4",
				Convert.ToChar(0x2234).ToString()
			},
			{
				"theta",
				Convert.ToChar(0x03B8).ToString()
			},
			{
				"theta",
				Convert.ToChar(0x0398).ToString()
			},
			{
				"thetasym",
				Convert.ToChar(0x03D1).ToString()
			},
			{
				"thinsp",
				Convert.ToChar(0x2009).ToString()
			},
			{
				"thorn",
				Convert.ToChar(0x00FE).ToString()
			},
			{
				"thorn",
				Convert.ToChar(0x00DE).ToString()
			},
			{
				"tilde",
				Convert.ToChar(0x02DC).ToString()
			},
			{
				"times",
				Convert.ToChar(0x00D7).ToString()
			},
			{
				"trade",
				Convert.ToChar(0x2122).ToString()
			},
			{
				"uacute",
				Convert.ToChar(0x00DA).ToString()
			},
			{
				"uacute",
				Convert.ToChar(0x00FA).ToString()
			},
			{
				"uarr",
				Convert.ToChar(0x2191).ToString()
			},
			{
				"uarr",
				Convert.ToChar(0x21D1).ToString()
			},
			{
				"ucirc",
				Convert.ToChar(0x00DB).ToString()
			},
			{
				"ucirc",
				Convert.ToChar(0x00FB).ToString()
			},
			{
				"ugrave",
				Convert.ToChar(0x00D9).ToString()
			},
			{
				"ugrave",
				Convert.ToChar(0x00F9).ToString()
			},
			{
				"uml",
				Convert.ToChar(0x00A8).ToString()
			},
			{
				"upsih",
				Convert.ToChar(0x03D2).ToString()
			},
			{
				"upsilon",
				Convert.ToChar(0x03A5).ToString()
			},
			{
				"upsilon",
				Convert.ToChar(0x03C5).ToString()
			},
			{
				"uuml",
				Convert.ToChar(0x00DC).ToString()
			},
			{
				"uuml",
				Convert.ToChar(0x00FC).ToString()
			},
			{
				"weierp",
				Convert.ToChar(0x2118).ToString()
			},
			{
				"xi",
				Convert.ToChar(0x039E).ToString()
			},
			{
				"xi",
				Convert.ToChar(0x03BE).ToString()
			},
			{
				"yacute",
				Convert.ToChar(0x00FD).ToString()
			},
			{
				"yacute",
				Convert.ToChar(0x00DD).ToString()
			},
			{
				"yen",
				Convert.ToChar(0x00A5).ToString()
			},
			{
				"yuml",
				Convert.ToChar(0x0178).ToString()
			},
			{
				"yuml",
				Convert.ToChar(0x00FF).ToString()
			},
			{
				"zeta",
				Convert.ToChar(0x03B6).ToString()
			},
			{
				"zeta",
				Convert.ToChar(0x0396).ToString()
			},
			{
				"zwj",
				Convert.ToChar(0x200D).ToString()
			},
			{
				"zwnj",
				Convert.ToChar(0x200C).ToString()
			}
		};

		private static StringDictionary m_EntityLookup;

		protected static StringDictionary EntityLookup
		{
			get
			{
				if(m_EntityLookup == null)
				{
					m_EntityLookup = new StringDictionary();
					for(int i = 0; i < entityLookupArray.GetLength(0); i++)
					{
						if(!m_EntityLookup.ContainsKey(entityLookupArray[i, 0]))
							m_EntityLookup.Add(entityLookupArray[i, 0], entityLookupArray[i, 1]);
					}
				}
				return m_EntityLookup;
			}
		}

		/// <summary>
		/// Regex Match processing delegate to replace the Entities with their
		/// underlying Unicode character.
		/// </summary>
		/// <param name="matchToProcess">Regular Expression Match</param>
		/// <returns>
		/// &amp;amp; becomes &amp;  (encoded for XML Comments - don't be confused)
		/// and &amp;eacute; becomes é
		/// </returns>
		private static string ResolveEntity(System.Text.RegularExpressions.Match matchToProcess)
		{
			bool includeTagsEntities = false;
			string x = "";
			if(matchToProcess.Groups["unicode"].Success)
			{
				x = Convert.ToChar(Convert.ToInt32(matchToProcess.Groups["unicode"].Value)).ToString();
			}
			else
			{
				if(matchToProcess.Groups["html"].Success)
				{
					string entity = matchToProcess.Groups["html"].Value.ToLower();
					switch(entity)
					{
					case "lt":
					case "gt":
					case "amp":
						if(includeTagsEntities)
							x = EntityLookup[matchToProcess.Groups["html"].Value.ToLower()];
						else
							x = "&" + entity + ";";
						break;
					default:
						x = EntityLookup[matchToProcess.Groups["html"].Value.ToLower()];
						break;
					}
				}
			}
			return x;
		}
	}
}
