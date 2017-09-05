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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Resultado do download de um pacote de assembly.
	/// </summary>
	public class AssemblyPackageDownloaderResult : IEnumerable<AssemblyPackageDownloaderResult.Item>, IDisposable
	{
		private List<Item> _items = new List<Item>();

		private System.IO.Stream _stream;

		private IO.Compression.ZipArchive _zipArchive;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="inputStream">Stream de entrada.</param>
		public AssemblyPackageDownloaderResult(System.IO.Stream inputStream)
		{
			inputStream.Require("inputStream").NotNull();
			_stream = inputStream;
		}

		/// <summary>
		/// Constrói o resultado na stream informada.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="outStream"></param>
		public static void Build(IEnumerable<Item> items, System.IO.Stream outStream)
		{
			items.Require("items").NotNull();
			using (var zipArchive = new Colosoft.IO.Compression.ZipArchive(outStream, System.IO.FileAccess.Write))
			{
				foreach (var i in items)
				{
					var stream = i.Stream;
					if(stream != null)
						zipArchive.CopyFromStream(i.Stream, i.LastWriteTime, string.Format("{0}.xap", i.Uid.ToString()));
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
			if(_zipArchive != null)
			{
				_zipArchive.Dispose();
				_zipArchive = null;
			}
			foreach (var i in _items)
				i.Dispose();
			_items.Clear();
		}

		/// <summary>
		/// Representa um item do resultado.
		/// </summary>
		public class Item : IDisposable
		{
			private Guid _uid;

			private System.IO.Stream _stream;

			private DateTime _lastWriteTime;

			/// <summary>
			/// Identificador unico do item.
			/// </summary>
			public Guid Uid
			{
				get
				{
					return _uid;
				}
			}

			/// <summary>
			/// Stream do item.
			/// </summary>
			public System.IO.Stream Stream
			{
				get
				{
					return _stream;
				}
			}

			/// <summary>
			/// Data da ultima escrita no item.
			/// </summary>
			public DateTime LastWriteTime
			{
				get
				{
					return _lastWriteTime;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="uid">Identificador unico do item.</param>
			/// <param name="lastWriteTime">Data da última escrita do item.</param>
			/// <param name="stream"></param>
			public Item(Guid uid, DateTime lastWriteTime, System.IO.Stream stream)
			{
				_uid = uid;
				_lastWriteTime = lastWriteTime;
				_stream = stream;
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
				if(_stream != null)
				{
					_stream.Dispose();
					_stream = null;
				}
			}
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		private IEnumerator<AssemblyPackageDownloaderResult.Item> GetEnumeratorInternal()
		{
			if(_zipArchive == null)
				_zipArchive = new IO.Compression.ZipArchive(_stream, System.IO.FileAccess.Read);
			foreach (var file in _zipArchive.Files)
			{
				Guid uid = Guid.Empty;
				try
				{
					var name = System.IO.Path.GetFileNameWithoutExtension(file.Name);
					uid = Guid.Parse(name);
				}
				catch
				{
					continue;
				}
				var item = _items.Find(f => f.Uid == uid);
				if(item == null)
				{
					item = new Item(uid, file.LastWriteTime, file.OpenRead());
					_items.Add(item);
				}
				yield return item;
			}
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyPackageDownloaderResult.Item> GetEnumerator()
		{
			return GetEnumeratorInternal();
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumeratorInternal();
		}
	}
}
