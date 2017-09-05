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
using System.IO;

namespace Colosoft.Text.Parser
{
	/// <summary>
	/// Possíveis tipos de conteúdo de entrada.
	/// </summary>
	internal enum EntryContent
	{
		/// <summary>
		/// Identifica que o conteúdo é um booleano.
		/// </summary>
		Boolean = 0x42,
		/// <summary>
		/// Conteúdo é um byte;
		/// </summary>
		Byte = 0x62,
		/// <summary>
		/// Conteúdo vazio.
		/// </summary>
		Empty = 0x45,
		/// <summary>
		/// Inteiro.
		/// </summary>
		Integer = 0x49,
		/// <summary>
		/// Multitype.
		/// </summary>
		Multi = 0x4d,
		/// <summary>
		/// Texto
		/// </summary>
		String = 0x53
	}
	/// <summary>
	/// Classe responsável pela leitura da gramática.
	/// </summary>
	internal class GrammarReader : IDisposable
	{
		private System.Text.Encoding _encoding;

		private System.Collections.Queue _entryQueue;

		private BinaryReader _reader;

		/// <summary>
		/// Cria uma nova instancia lendo os dados da stream informada.
		/// </summary>
		/// <param name="stream"></param>
		public GrammarReader(Stream stream)
		{
			try
			{
				_encoding = new UnicodeEncoding(false, true);
				_reader = new BinaryReader(stream);
				_entryQueue = new System.Collections.Queue();
			}
			catch(Exception exception)
			{
				throw new ParserException("Error constructing GrammarReader", exception);
			}
			if(!this.HasValidHeader())
			{
				throw new ParserException("Incorrect file header");
			}
		}

		/// <summary>
		/// Cria uma nova instancia lendo os dados do arquivo informado.
		/// </summary>
		/// <param name="filename"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public GrammarReader(string filename)
		{
			try
			{
				_encoding = new UnicodeEncoding(false, true);
				_reader = new BinaryReader(new FileStream(filename, FileMode.Open));
				_entryQueue = new System.Collections.Queue();
			}
			catch(Exception exception)
			{
				throw new ParserException("Error constructing GrammarReader", exception);
			}
			if(!this.HasValidHeader())
				throw new ParserException("Incorrect file header");
		}

		/// <summary>
		/// Verifica se o cabeçalho é válido.
		/// </summary>
		/// <returns></returns>
		private bool HasValidHeader()
		{
			return this.ReadString().Equals("GOLD Parser Tables/v1.0");
		}

		/// <summary>
		/// Move para o prómixo registro
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			try
			{
				if(_reader.BaseStream.Position == _reader.BaseStream.Length)
					return false;
				var content = (EntryContent)_reader.ReadByte();
				if(content == EntryContent.Multi)
				{
					_entryQueue.Clear();
					int count = _reader.ReadInt16();
					for(int n = 0; n < count; n++)
					{
						ReadEntry();
					}
					return true;
				}
				else
					return false;
			}
			catch(IOException)
			{
				return false;
			}
		}

		/// <summary>
		/// Verifica se a recuperação já foi feita.
		/// </summary>
		/// <returns></returns>
		public bool RetrieveDone()
		{
			return _entryQueue.Count == 0;
		}

		/// <summary>
		/// Recupera o próximo ite
		/// </summary>
		/// <returns></returns>
		public object RetrieveNext()
		{
			if(_entryQueue.Count == 0)
				return null;
			return _entryQueue.Dequeue();
		}

		/// <summary>
		/// Lê a atual entrada.
		/// </summary>
		private void ReadEntry()
		{
			var entryContent = (EntryContent)_reader.ReadByte();
			switch(entryContent)
			{
			case EntryContent.Integer:
				_entryQueue.Enqueue((int)_reader.ReadUInt16());
				return;
			case EntryContent.String:
				_entryQueue.Enqueue(ReadString());
				return;
			case EntryContent.Byte:
				_entryQueue.Enqueue(_reader.ReadByte());
				return;
			case EntryContent.Boolean:
				_entryQueue.Enqueue(_reader.ReadByte() == 1);
				return;
			case EntryContent.Empty:
				_entryQueue.Enqueue(new object());
				return;
			}
			throw new ParserException("Error reading CGT: unknown entry-content type");
		}

		/// <summary>
		/// Recupera uam string do leitor.
		/// </summary>
		/// <returns></returns>
		private string ReadString()
		{
			StringBuilder result = new StringBuilder();
			char unicodeChar = (char)(int)_reader.ReadUInt16();
			while (unicodeChar != (char)0)
			{
				result.Append(unicodeChar);
				unicodeChar = (char)(int)_reader.ReadUInt16();
			}
			return result.ToString();
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
			if(_reader != null)
			{
				_reader.Dispose();
				_reader = null;
			}
		}
	}
}
