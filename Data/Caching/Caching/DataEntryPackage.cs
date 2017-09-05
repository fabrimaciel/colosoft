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
using System.Text;

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Representa um pacote de entradas do cache.
	/// </summary>
	public class DataEntryPackage : IDisposable
	{
		private ICSharpCode.SharpZipLib.Zip.ZipFile _zipFile;

		private System.IO.Stream _inStream;

		private List<System.IO.Stream> _openedStreams = new List<System.IO.Stream>();

		/// <summary>
		/// Cria a instanci com os dados contidos na stream informada.
		/// </summary>
		/// <param name="inStream">Stream contendo os dados do pacote.</param>
		public DataEntryPackage(System.IO.Stream inStream)
		{
			_inStream = inStream;
			if(inStream != null)
				_zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(inStream);
		}

		/// <summary>
		/// Recupera as entradas de dados.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public IEnumerable<Tuple<DataEntryVersion, System.IO.Stream>> GetDataEntries()
		{
			if(_inStream == null)
				return new Tuple<DataEntryVersion, System.IO.Stream>[0];
			return new GetDataEntriesEnumerable(this);
		}

		/// <summary>
		/// Constrói um pacote com as dependencias informadas.
		/// </summary>
		/// <param name="outStream">Stream de saída.</param>
		/// <param name="entries"></param>
		/// <returns></returns>
		public static void BuildPackage(System.IO.Stream outStream, IEnumerator<Tuple<DataEntryVersion, System.IO.Stream>> entries)
		{
			outStream.Require("outStream").NotNull();
			outStream.Require("entries").NotNull();
			var buffer = new byte[1024];
			int read = 0;
			var zipOut = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(outStream);
			while (entries.MoveNext())
			{
				var item = entries.Current;
				if(item == null)
					continue;
				var entryName = item.Item1.TypeName.FullName;
				var entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(entryName);
				entry.Size = item.Item2.Length;
				zipOut.PutNextEntry(entry);
				while ((read = item.Item2.Read(buffer, 0, buffer.Length)) > 0)
					zipOut.Write(buffer, 0, read);
				zipOut.CloseEntry();
			}
			zipOut.Finish();
			zipOut.Flush();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_zipFile.Close();
		}

		sealed class GetDataEntriesEnumerable : IEnumerable<Tuple<DataEntryVersion, System.IO.Stream>>, IDisposable
		{
			private GetDataEntriesEnumerator _enumerator;

			private DataEntryPackage _package;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="package"></param>
			public GetDataEntriesEnumerable(DataEntryPackage package)
			{
				_package = package;
			}

			public IEnumerator<Tuple<DataEntryVersion, System.IO.Stream>> GetEnumerator()
			{
				if(_enumerator == null)
					_enumerator = new GetDataEntriesEnumerator(_package);
				return _enumerator;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				if(_enumerator == null)
					_enumerator = new GetDataEntriesEnumerator(_package);
				return _enumerator;
			}

			class GetDataEntriesEnumerator : IEnumerator<Tuple<DataEntryVersion, System.IO.Stream>>
			{
				private int _currentPosition = -1;

				private Tuple<DataEntryVersion, System.IO.Stream> _current;

				private System.IO.Stream _currrentStream;

				private DataEntryPackage _package;

				private byte[] _buffer = new byte[1024];

				/// <summary>
				/// Construtor padrão.
				/// </summary>
				/// <param name="package"></param>
				public GetDataEntriesEnumerator(DataEntryPackage package)
				{
					_package = package;
				}

				public Tuple<DataEntryVersion, System.IO.Stream> Current
				{
					get
					{
						return _current;
					}
				}

				public void Dispose()
				{
					if(_currrentStream != null)
					{
						_currrentStream.Dispose();
						_currrentStream = null;
					}
					_currentPosition = -1;
					_current = null;
				}

				object System.Collections.IEnumerator.Current
				{
					get
					{
						return _current;
					}
				}

				[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
				public bool MoveNext()
				{
					_currentPosition++;
					if(_currentPosition >= _package._zipFile.Count)
						return false;
					var zipEntry = _package._zipFile[_currentPosition];
					using (var stream = _package._zipFile.GetInputStream(zipEntry))
					{
						var outStream = new System.IO.MemoryStream((int)zipEntry.Size);
						var read = 0;
						while ((read = stream.Read(_buffer, 0, _buffer.Length)) > 0)
							outStream.Write(_buffer, 0, read);
						outStream.Position = 0;
						try
						{
							var reader = new Colosoft.Serialization.IO.CompactBinaryReader(outStream);
							var isNullTypeName = reader.ReadByte() == 0;
							Colosoft.Reflection.TypeName typeName = null;
							if(!isNullTypeName)
							{
								typeName = new Reflection.TypeName();
								typeName.Deserialize(reader);
							}
							var version = reader.ReadDateTime();
							var hasRecords = reader.ReadByte() == 1;
							outStream.Position = 0;
							_currrentStream = outStream;
							_current = new Tuple<DataEntryVersion, System.IO.Stream>(new DataEntryVersion {
								TypeName = typeName,
								Version = version
							}, outStream);
						}
						catch
						{
							outStream.Dispose();
							throw;
						}
					}
					return true;
				}

				public void Reset()
				{
					if(_currrentStream != null)
					{
						_currrentStream.Dispose();
						_currrentStream = null;
					}
					_currentPosition = -1;
					_current = null;
				}
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_enumerator != null)
				{
					_enumerator.Dispose();
					_enumerator = null;
				}
			}
		}
	}
}
