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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Colosoft.Web
{
	/// <summary>
	///     Provides methods to parse a
	///     <see href="http://www.ietf.org/rfc/rfc2388.txt">
	///         <c>multipart/form-data</c>
	///     </see>
	///     stream into it's parameters and file data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A parameter is defined as any non-file data passed in the multipart stream. For example
	///         any form fields would be considered a parameter.
	///     </para>
	///     <para>
	///         The parser determines if a section is a file or not based on the presence or absence
	///         of the filename argument for the Content-Type header. If filename is set then the section
	///         is assumed to be a file, otherwise it is assumed to be parameter data.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code lang="C#"> 
	///       Stream multipartStream = GetTheMultipartStream();
	///       string boundary = GetTheBoundary();
	///       var parser = new StreamingMultipartFormDataParser(multipartStream, boundary, Encoding.UTF8);
	///
	///       // Set up our delegates for how we want to handle recieved data. 
	///       // In our case parameters will be written to a dictionary and files
	///       // will be written to a filestream
	///       parser.ParameterHandler += parameter => AddToDictionary(parameter);
	///       parser.FileHandler += (name, fileName, type, disposition, buffer, bytes) => WriteDataToFile(fileName, buffer, bytes);
	///       parser.Run();
	///   </code>
	/// </example>
	public class StreamingMultipartFormDataParser
	{
		/// <summary>
		///     The stream we are parsing.
		/// </summary>
		private readonly Stream stream;

		/// <summary>
		///     The boundary of the multipart message  as a string.
		/// </summary>
		private string boundary;

		/// <summary>
		///     The boundary of the multipart message as a byte string
		///     encoded with CurrentEncoding
		/// </summary>
		private byte[] boundaryBinary;

		/// <summary>
		///     The end boundary of the multipart message as a string.
		/// </summary>
		private string endBoundary;

		/// <summary>
		///     The end boundary of the multipart message as a byte string
		///     encoded with CurrentEncoding
		/// </summary>
		private byte[] endBoundaryBinary;

		/// <summary>
		///     Determines if we have consumed the end boundary binary and determines
		///     if we are done parsing.
		/// </summary>
		private bool readEndBoundary;

		/// <summary>
		///     Initializes a new instance of the <see cref="StreamingMultipartFormDataParser" /> class
		///     with an input stream. Boundary will be automatically detected based on the
		///     first line of input.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		public StreamingMultipartFormDataParser(Stream stream) : this(stream, null, Encoding.UTF8, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="StreamingMultipartFormDataParser" /> class
		///     with the boundary and input stream.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		public StreamingMultipartFormDataParser(Stream stream, string boundary) : this(stream, boundary, Encoding.UTF8, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="StreamingMultipartFormDataParser" /> class
		///     with the input stream and stream encoding. Boundary is automatically
		///     detected.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		public StreamingMultipartFormDataParser(Stream stream, Encoding encoding) : this(stream, null, encoding, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="StreamingMultipartFormDataParser" /> class
		///     with the boundary, input stream and stream encoding.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		public StreamingMultipartFormDataParser(Stream stream, string boundary, Encoding encoding) : this(stream, boundary, encoding, 4096)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="StreamingMultipartFormDataParser" /> class
		///     with the stream, input encoding and buffer size. Boundary is automatically
		///     detected.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		/// <param name="binaryBufferSize">
		///     The size of the buffer to use for parsing the multipart form data. This must be larger
		///     then (size of boundary + 4 + # bytes in newline).
		/// </param>
		public StreamingMultipartFormDataParser(Stream stream, Encoding encoding, int binaryBufferSize) : this(stream, null, encoding, binaryBufferSize)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="StreamingMultipartFormDataParser" /> class
		///     with the boundary, stream, input encoding and buffer size.
		/// </summary>
		/// <param name="stream">
		///     The stream containing the multipart data
		/// </param>
		/// <param name="boundary">
		///     The multipart/form-data boundary. This should be the value
		///     returned by the request header.
		/// </param>
		/// <param name="encoding">
		///     The encoding of the multipart data
		/// </param>
		/// <param name="binaryBufferSize">
		///     The size of the buffer to use for parsing the multipart form data. This must be larger
		///     then (size of boundary + 4 + # bytes in newline).
		/// </param>
		public StreamingMultipartFormDataParser(Stream stream, string boundary, Encoding encoding, int binaryBufferSize)
		{
			if(stream == null || stream == Stream.Null)
			{
				throw new ArgumentNullException("stream");
			}
			if(encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			this.stream = stream;
			this.boundary = boundary;
			Encoding = encoding;
			BinaryBufferSize = binaryBufferSize;
			readEndBoundary = false;
		}

		/// <summary>
		///     Begins executing the parser. This should be called after all handlers have been set.
		/// </summary>
		public void Run()
		{
			var reader = new RebufferableBinaryReader(stream, Encoding, BinaryBufferSize);
			if(boundary == null)
			{
				boundary = DetectBoundary(reader);
			}
			boundary = "--" + boundary;
			endBoundary = boundary + "--";
			boundaryBinary = Encoding.GetBytes(boundary);
			endBoundaryBinary = Encoding.GetBytes(endBoundary);
			Debug.Assert(BinaryBufferSize >= endBoundaryBinary.Length, "binaryBufferSize must be bigger then the boundary");
			Parse(reader);
		}

		/// <summary>
		/// The FileStreamDelegate defining functions that can handle file stream data from this parser.
		///
		/// Delegates can assume that the data is sequential i.e. the data recieved by any delegates will be
		/// the data immediately following any previously recieved data.
		/// </summary>
		/// <param name="name">The name of the multipart data</param>
		/// <param name="fileName">The name of the file</param>
		/// <param name="contentType">The content type of the multipart data</param>
		/// <param name="contentDisposition">The content disposition of the multipart data</param>
		/// <param name="buffer">Some of the data from the file (not neccecarily all of the data)</param>
		/// <param name="bytes">The length of data in buffer</param>
		public delegate void FileStreamDelegate (string name, string fileName, string contentType, string contentDisposition, byte[] buffer, int bytes);

		/// <summary>
		/// Assinatura do evento acionado quando o stream for fechada.
		/// </summary>
		public delegate void StreamClosedDelegate ();

		/// <summary>
		/// The ParameterDelegate defining functions that can handle multipart parameter data
		/// </summary>
		/// <param name="part">The parsed parameter part</param>
		public delegate void ParameterDelegate (ParameterPart part);

		/// <summary>
		///     Gets or sets the binary buffer size.
		/// </summary>
		public int BinaryBufferSize
		{
			get;
			set;
		}

		/// <summary>
		///     Gets the encoding.
		/// </summary>
		public Encoding Encoding
		{
			get;
			private set;
		}

		/// <summary>
		/// The FileHandler. Delegates attached to this property will recieve sequential file stream data from this parser.
		/// </summary>
		public FileStreamDelegate FileHandler
		{
			get;
			set;
		}

		/// <summary>
		/// The ParameterHandler. Delegates attached to this property will recieve parameter data.
		/// </summary>
		public ParameterDelegate ParameterHandler
		{
			get;
			set;
		}

		/// <summary>
		/// The StreamClosedHandler. Delegates attached to this property will be notified when the source stream is exhausted.
		/// </summary>
		public StreamClosedDelegate StreamClosedHandler
		{
			get;
			set;
		}

		/// <summary>
		///     Detects the boundary from the input stream. Assumes that the
		///     current position of the reader is the start of the file and therefore
		///     the beginning of the boundary.
		/// </summary>
		/// <param name="reader">
		///     The binary reader to parse
		/// </param>
		/// <returns>
		///     The boundary string
		/// </returns>
		private static string DetectBoundary(RebufferableBinaryReader reader)
		{
			string boundary = string.Concat(reader.ReadLine().Skip(2));
			reader.Buffer("--" + boundary + "\n");
			return boundary;
		}

		/// <summary>
		///     Finds the next sequence of newlines in the input stream.
		/// </summary>
		/// <param name="data">The data to search</param>
		/// <param name="offset">The offset to start searching at</param>
		/// <param name="maxBytes">The maximum number of bytes (starting from offset) to search.</param>
		/// <returns>The offset of the next newline</returns>
		private int FindNextNewline(ref byte[] data, int offset, int maxBytes)
		{
			byte[][] newlinePatterns =  {
				Encoding.GetBytes("\r\n"),
				Encoding.GetBytes("\n")
			};
			Array.Sort(newlinePatterns, (first, second) => second.Length.CompareTo(first.Length));
			byte[] dataRef = data;
			if(offset != 0)
			{
				dataRef = data.Skip(offset).ToArray();
			}
			foreach (var pattern in newlinePatterns)
			{
				int position = SubsequenceFinder.Search(dataRef, pattern, maxBytes);
				if(position != -1)
				{
					return position + offset;
				}
			}
			return -1;
		}

		/// <summary>
		///     Calculates the length of the next found newline.
		///     data[offset] is the start of the space to search.
		/// </summary>
		/// <param name="data">
		///     The data containing the newline
		/// </param>
		/// <param name="offset">
		///     The offset of the start of the newline
		/// </param>
		/// <returns>
		///     The length in bytes of the newline sequence
		/// </returns>
		private int CalculateNewlineLength(ref byte[] data, int offset)
		{
			byte[][] newlinePatterns =  {
				Encoding.GetBytes("\r\n"),
				Encoding.GetBytes("\n")
			};
			foreach (var pattern in newlinePatterns)
			{
				bool found = false;
				for(int i = 0; i < pattern.Length; ++i)
				{
					if(pattern[i] != data[offset + i])
					{
						found = false;
						break;
					}
					found = true;
				}
				if(found)
				{
					return pattern.Length;
				}
			}
			return 0;
		}

		/// <summary>
		///     Begins the parsing of the stream into objects.
		/// </summary>
		/// <param name="reader">
		///     The multipart/form-data binary reader to parse from.
		/// </param>
		/// <exception cref="MultipartParseException">
		///     thrown on finding unexpected data such as a boundary before we are ready for one.
		/// </exception>
		private void Parse(RebufferableBinaryReader reader)
		{
			while (true)
			{
				string line = reader.ReadLine();
				if(line == boundary)
				{
					break;
				}
				if(line == null)
				{
					throw new MultipartParseException("Could not find expected boundary");
				}
			}
			while (!readEndBoundary)
			{
				ParseSection(reader);
			}
			if(StreamClosedHandler != null)
			{
				StreamClosedHandler();
			}
		}

		/// <summary>
		///     Parses a section of the stream that is known to be file data.
		/// </summary>
		/// <param name="parameters">
		///     The header parameters of this file, expects "name" and "filename" to be valid keys
		/// </param>
		/// <param name="reader">
		///     The StreamReader to read the data from
		/// </param>
		/// <returns>
		///     The <see cref="FilePart" /> con[]              taining the parsed data (name, filename, stream containing file).
		/// </returns>
		private void ParseFilePart(Dictionary<string, string> parameters, RebufferableBinaryReader reader)
		{
			string name = parameters["name"];
			string filename = parameters["filename"];
			string contentType = parameters.ContainsKey("content-type") ? parameters["content-type"] : "text/plain";
			string contentDisposition = parameters.ContainsKey("content-disposition") ? parameters["content-disposition"] : "form-data";
			var curBuffer = new byte[BinaryBufferSize];
			var prevBuffer = new byte[BinaryBufferSize];
			var fullBuffer = new byte[BinaryBufferSize * 2];
			int curLength = 0;
			int prevLength = 0;
			int fullLength = 0;
			prevLength = reader.Read(prevBuffer, 0, prevBuffer.Length);
			do
			{
				curLength = reader.Read(curBuffer, 0, curBuffer.Length);
				Buffer.BlockCopy(prevBuffer, 0, fullBuffer, 0, prevLength);
				Buffer.BlockCopy(curBuffer, 0, fullBuffer, prevLength, curLength);
				fullLength = prevLength + curLength;
				int endBoundaryPos = SubsequenceFinder.Search(fullBuffer, endBoundaryBinary, fullLength);
				int endBoundaryLength = endBoundaryBinary.Length;
				int boundaryPos = SubsequenceFinder.Search(fullBuffer, boundaryBinary, fullLength);
				int boundaryLength = boundaryBinary.Length;
				if(boundaryPos + boundaryLength == fullLength)
				{
					boundaryPos = -1;
				}
				int endPos = -1;
				int endPosLength = 0;
				if(endBoundaryPos >= 0 && boundaryPos >= 0)
				{
					if(boundaryPos < endBoundaryPos)
					{
						endPos = boundaryPos;
						endPosLength = boundaryLength;
					}
					else
					{
						endPos = endBoundaryPos;
						endPosLength = endBoundaryLength;
						readEndBoundary = true;
					}
				}
				else if(boundaryPos >= 0 && endBoundaryPos < 0)
				{
					endPos = boundaryPos;
					endPosLength = boundaryLength;
				}
				else if(boundaryPos < 0 && endBoundaryPos >= 0)
				{
					endPos = endBoundaryPos;
					endPosLength = endBoundaryLength;
					readEndBoundary = true;
				}
				if(endPos != -1)
				{
					int boundaryNewlineOffset = CalculateNewlineLength(ref fullBuffer, Math.Min(fullLength - 1, endPos + endPosLength));
					int maxNewlineBytes = Encoding.GetMaxByteCount(2);
					int bufferNewlineOffset = FindNextNewline(ref fullBuffer, Math.Max(0, endPos - maxNewlineBytes), maxNewlineBytes);
					int bufferNewlineLength = CalculateNewlineLength(ref fullBuffer, bufferNewlineOffset);
					FileHandler(name, filename, contentType, contentDisposition, fullBuffer, endPos - bufferNewlineLength);
					int writeBackOffset = endPos + endPosLength + boundaryNewlineOffset;
					int writeBackAmount = (prevLength + curLength) - writeBackOffset;
					var writeBackBuffer = new byte[writeBackAmount];
					Buffer.BlockCopy(fullBuffer, writeBackOffset, writeBackBuffer, 0, writeBackAmount);
					reader.Buffer(writeBackBuffer);
					break;
				}
				FileHandler(name, filename, contentType, contentDisposition, prevBuffer, prevLength);
				byte[] tempBuffer = curBuffer;
				curBuffer = prevBuffer;
				prevBuffer = tempBuffer;
				prevLength = curLength;
			}
			while (prevLength != 0);
		}

		/// <summary>
		///     Parses a section of the stream that is known to be parameter data.
		/// </summary>
		/// <param name="parameters">
		///     The header parameters of this section. "name" must be a valid key.
		/// </param>
		/// <param name="reader">
		///     The StreamReader to read the data from
		/// </param>
		/// <returns>
		///     The <see cref="ParameterPart" /> containing the parsed data (name, value).
		/// </returns>
		/// <exception cref="MultipartParseException">
		///     thrown if unexpected data is found such as running out of stream before hitting the boundary.
		/// </exception>
		private void ParseParameterPart(Dictionary<string, string> parameters, RebufferableBinaryReader reader)
		{
			var data = new StringBuilder();
			bool firstTime = true;
			string line = reader.ReadLine();
			while (line != boundary && line != endBoundary)
			{
				if(line == null)
				{
					throw new MultipartParseException("Unexpected end of stream. Is there an end boundary?");
				}
				if(firstTime)
				{
					data.Append(line);
					firstTime = false;
				}
				else
				{
					data.Append(Environment.NewLine);
					data.Append(line);
				}
				line = reader.ReadLine();
			}
			if(line == endBoundary)
			{
				readEndBoundary = true;
			}
			var part = new ParameterPart(parameters["name"], data.ToString());
			ParameterHandler(part);
		}

		/// <summary>
		///     Parses the header of the next section of the multipart stream and
		///     determines if it contains file data or parameter data.
		/// </summary>
		/// <param name="reader">
		///     The StreamReader to read data from.
		/// </param>
		/// <exception cref="MultipartParseException">
		///     thrown if unexpected data is hit such as end of stream.
		/// </exception>
		private void ParseSection(RebufferableBinaryReader reader)
		{
			var parameters = new Dictionary<string, string>();
			string line = reader.ReadLine();
			while (line != string.Empty)
			{
				if(line == null)
				{
					throw new MultipartParseException("Unexpected end of stream");
				}
				if(line == boundary || line == endBoundary)
				{
					throw new MultipartParseException("Unexpected end of section");
				}
				Dictionary<string, string> values = SplitBySemicolonIgnoringSemicolonsInQuotes(line).Select(x => x.Split(new[] {
					':',
					'='
				}, 2)).Where(x => x.Length == 2).ToDictionary(x => x[0].Trim().Replace("\"", string.Empty).ToLower(), x => x[1].Trim().Replace("\"", string.Empty));
				try
				{
					foreach (var pair in values)
					{
						parameters.Add(pair.Key, pair.Value);
					}
				}
				catch(ArgumentException)
				{
					throw new MultipartParseException("Duplicate field in section");
				}
				line = reader.ReadLine();
			}
			if(parameters.ContainsKey("filename"))
			{
				ParseFilePart(parameters, reader);
			}
			else
			{
				ParseParameterPart(parameters, reader);
			}
		}

		/// <summary>
		///     Splits a line by semicolons but ignores semicolons in quotes.
		/// </summary>
		/// <param name="line">The line to split</param>
		/// <returns>The split strings</returns>
		private IEnumerable<string> SplitBySemicolonIgnoringSemicolonsInQuotes(string line)
		{
			bool inQuotes = false;
			string workingString = "";
			foreach (char c in line)
			{
				if(c == '"')
				{
					inQuotes = !inQuotes;
				}
				if(c == ';' && !inQuotes)
				{
					yield return workingString;
					workingString = "";
				}
				else
				{
					workingString += c;
				}
			}
			yield return workingString;
		}
	}
}
