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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Locking
{
	/// <summary>
	/// Possíveis modos de locks.
	/// </summary>
	public enum LockMode
	{
		/// <summary>
		/// Modo de leitura.
		/// </summary>
		Reader,
		/// <summary>
		/// Modo de escrita.
		/// </summary>
		Write,
		/// <summary>
		/// None.
		/// </summary>
		None
	}
	/// <summary>
	/// Classe responsável pelo gerenciamento de locks.
	/// </summary>
	[Serializable]
	public class LockManager : ICompactSerializable
	{
		private LockMode _lockMode = LockMode.None;

		private List<LockHandle> _readerLocks = new List<LockHandle>();

		private LockHandle _writerLock;

		/// <summary>
		/// Modo de lock da instancia.
		/// </summary>
		public LockMode Mode
		{
			get
			{
				return _lockMode;
			}
		}

		/// <summary>
		/// Adquire o lock de leitura.
		/// </summary>
		/// <param name="lockHandle"></param>
		/// <returns></returns>
		public bool AcquireReaderLock(string lockHandle)
		{
			lock (this)
			{
				if(_lockMode == LockMode.None || _lockMode == LockMode.Reader)
				{
					_readerLocks.Add(new LockHandle(lockHandle));
					_lockMode = LockMode.Reader;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Adquire o lock de escrita.
		/// </summary>
		/// <param name="lockHandle"></param>
		/// <returns></returns>
		public bool AcquireWriterLock(string lockHandle)
		{
			lock (this)
			{
				if(_lockMode == LockMode.None)
				{
					_writerLock = new LockHandle(lockHandle);
					_lockMode = LockMode.Write;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Faz a liberação do lock de leitura.
		/// </summary>
		/// <param name="lockId"></param>
		public void ReleaseReaderLock(string lockId)
		{
			if(lockId != null)
			{
				lock (this)
				{
					if((_lockMode == LockMode.Reader) && _readerLocks.Contains(new LockHandle(lockId)))
					{
						_readerLocks.Remove(new LockHandle(lockId));
						if(_readerLocks.Count == 0)
						{
							_lockMode = LockMode.None;
						}
					}
				}
			}
		}

		/// <summary>
		/// Faz a liberação do lock de escrita.
		/// </summary>
		/// <param name="lockId"></param>
		public void ReleaseWriterLock(string lockId)
		{
			lock (this)
			{
				if(_lockMode == LockMode.Write && _writerLock.Equals(lockId))
				{
					_writerLock = null;
					_lockMode = LockMode.None;
				}
			}
		}

		/// <summary>
		/// Valida o lock.
		/// </summary>
		/// <param name="lockId">Identificador do lock.</param>
		/// <returns></returns>
		public bool ValidateLock(string lockId)
		{
			return ValidateLock(_lockMode, lockId);
		}

		/// <summary>
		/// Valida o lock.
		/// </summary>
		/// <param name="mode">Modo que será validado.</param>
		/// <param name="lockId">Identificador do lock.</param>
		/// <returns></returns>
		public bool ValidateLock(LockMode mode, string lockId)
		{
			if((lockId != null) || (mode == LockMode.None))
			{
				lock (this)
				{
					switch(mode)
					{
					case LockMode.Reader:
						return _readerLocks.Contains(new LockHandle(lockId));
					case LockMode.Write:
						return _writerLock.Equals(lockId);
					case LockMode.None:
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.Write((byte)_lockMode);
			if(_writerLock != null)
				writer.WriteObject(_writerLock.LockId);
			else
				writer.WriteObject(null);
			writer.Write(_readerLocks.Count);
			foreach (LockHandle handle in _readerLocks)
				writer.WriteObject(handle.LockId);
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_lockMode = (LockMode)reader.ReadByte();
			string str = reader.ReadObject() as string;
			if(!string.IsNullOrEmpty(str))
				_writerLock = new LockHandle(str);
			int num = reader.ReadInt32();
			_readerLocks = new List<LockHandle>();
			for(int i = 0; i < num; i++)
				_readerLocks.Add(new LockHandle(reader.ReadObject() as string));
		}

		/// <summary>
		/// Representa o manipulador do lock.
		/// </summary>
		[Serializable]
		private class LockHandle : ICompactSerializable
		{
			private string _lockId;

			private DateTime _lockTime;

			/// <summary>
			/// Identificador do lock.
			/// </summary>
			public string LockId
			{
				get
				{
					return _lockId;
				}
			}

			/// <summary>
			/// Horário do lock.
			/// </summary>
			public DateTime LockTime
			{
				get
				{
					return _lockTime;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="lockId">Identificador do lock.</param>
			public LockHandle(string lockId)
			{
				_lockId = lockId;
				_lockTime = DateTime.Now;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public override bool Equals(object obj)
			{
				LockManager.LockHandle handle = obj as LockManager.LockHandle;
				return (((handle != null) && (handle._lockId == _lockId)) || ((obj is string) && (_lockId == ((string)obj))));
			}

			/// <summary>
			/// Recupera o hashcode da instancia.
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return (_lockId ?? "").GetHashCode() ^ _lockTime.GetHashCode();
			}

			/// <summary>
			/// Serializa os dados da instancia.
			/// </summary>
			/// <param name="writer"></param>
			public void Serialize(CompactWriter writer)
			{
				writer.WriteObject(_lockId);
			}

			/// <summary>
			/// Deserializa os dados na instancia.
			/// </summary>
			/// <param name="reader"></param>
			public void Deserialize(CompactReader reader)
			{
				_lockId = reader.ReadObject() as string;
			}
		}
	}
}
