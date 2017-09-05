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
using Colosoft.Serialization.IO;
using System.Collections;
using Colosoft.Serialization;
using System.Reflection;
using Colosoft.IO.Compression;
using Colosoft.Caching.Util;
using Colosoft.Caching.Exceptions;
using Colosoft.Logging;

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa o gerenciador do provedor de escrita.
	/// </summary>
	internal class WriteThruProviderManager : IDisposable
	{
		private bool _asyncWrites;

		private string _cacheName;

		private CacheRuntimeContext _context;

		private IWriteThruProvider _dsWriter;

		private string _myProvider;

		public bool AsyncWriteEnabled
		{
			get
			{
				return _asyncWrites;
			}
		}

		public string MyProviderName
		{
			get
			{
				return _myProvider;
			}
		}

		private ILogger Logger
		{
			get
			{
				return _context.Logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public WriteThruProviderManager()
		{
		}

		public WriteThruProviderManager(string cacheName, IDictionary properties, CacheRuntimeContext context, int timeout, string providerName)
		{
			_context = context;
			_cacheName = cacheName;
			_myProvider = providerName;
			this.Initialize(properties);
		}

		public void DequeueWriteBehindTask(string taskId)
		{
			if((_asyncWrites && (_context.DatasourceMgr != null)) && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
				_context.DatasourceMgr._writeBehindAsyncProcess.Dequeue(taskId);
		}

		private string GetWriteThruAssemblyPath(string asm)
		{
			string str = @"\";
			string[] strArray = asm.Split(new char[] {
				',',
				'='
			});
			return (str + strArray[0]);
		}

		private void Initialize(IDictionary properties)
		{
			Assembly assembly = null;
			if(properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			try
			{
				if(!properties.Contains("assembly-name"))
				{
					throw new Colosoft.Caching.Exceptions.ConfigurationException("Missing assembly name for write-thru option");
				}
				if(!properties.Contains("class-name"))
				{
					throw new Colosoft.Caching.Exceptions.ConfigurationException("Missing class name for write-thru option");
				}
				string asm = Convert.ToString(properties["assembly-name"]);
				string typeName = Convert.ToString(properties["class-name"]);
				IDictionary parameters = properties["parameters"] as IDictionary;
				string extension = ".dll";
				if(properties.Contains("full-name"))
				{
					extension = System.IO.Path.GetExtension(Convert.ToString(properties["full-name"]));
				}
				if(parameters == null)
				{
					parameters = new Hashtable();
				}
				if(properties.Contains("async-mode"))
				{
					_asyncWrites = Convert.ToBoolean(properties["async-mode"]);
				}
				try
				{
					if(extension.Equals(".dll") || extension.Equals(".exe"))
					{
						string assemblyFile = CachingUtils.DeployedAssemblyDir + _cacheName + this.GetWriteThruAssemblyPath(asm) + extension;
						try
						{
							assembly = Assembly.LoadFrom(assemblyFile);
						}
						catch(Exception exception)
						{
							throw new Exception(string.Format("Could not load assembly " + assemblyFile + ". Error {0}", exception.Message));
						}
						if(assembly != null)
							_dsWriter = (IWriteThruProvider)assembly.CreateInstance(typeName);
						if(_dsWriter == null)
							throw new Exception("Unable to instantiate " + typeName);
						_dsWriter.Start(parameters);
					}
				}
				catch(InvalidCastException)
				{
					throw new ConfigurationException("The class specified in write-thru does not implement IDatasourceWriter");
				}
				catch(Exception exception2)
				{
					throw new ConfigurationException(exception2.Message, exception2);
				}
			}
			catch(ConfigurationException)
			{
				throw;
			}
			catch(Exception exception3)
			{
				throw new ConfigurationException("Configuration Error: " + exception3.ToString(), exception3);
			}
		}

		public void SetState(string taskId, WriteBehindAsyncProcessor.TaskState state)
		{
			if((_asyncWrites && (_context.DatasourceMgr != null)) && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
			{
				_context.DatasourceMgr._writeBehindAsyncProcess.SetState(taskId, state);
			}
		}

		public void SetState(string taskId, WriteBehindAsyncProcessor.TaskState state, Hashtable newTable)
		{
			if((_asyncWrites && (_context.DatasourceMgr != null)) && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
			{
				_context.DatasourceMgr._writeBehindAsyncProcess.SetState(taskId, state, newTable);
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		void IDisposable.Dispose()
		{
			if(_dsWriter != null)
			{
				lock (_dsWriter)
				{
					try
					{
						_dsWriter.Stop();
					}
					catch(Exception exception)
					{
						this.Logger.Error("WriteThruProviderMgr".GetFormatter(), ("User code threw " + exception.GetType().Name).GetFormatter());
					}
					_dsWriter = null;
				}
			}
		}

		public void WriteBehind(CacheBase internalCache, object key, CacheEntry entry, string source, string taskId, OpCode operationCode)
		{
			if(_asyncWrites && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
				_context.DatasourceMgr._writeBehindAsyncProcess.Enqueue(new WriteBehindTask(internalCache, this, key, entry, source, taskId, _myProvider, operationCode));
		}

		public void WriteBehind(CacheBase internalCache, object[] keys, object[] values, CacheEntry[] entries, string source, string taskId, OpCode operationCode)
		{
			if(_asyncWrites && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
				_context.DatasourceMgr._writeBehindAsyncProcess.Enqueue(new BulkWriteBehindTask(internalCache, this, keys, values, entries, source, taskId, _myProvider, operationCode));
		}

		public void WriteBehind(CacheBase internalCache, object key, CacheEntry entry, string source, string taskId, OpCode operationCode, WriteBehindAsyncProcessor.TaskState state)
		{
			if(_asyncWrites && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
				_context.DatasourceMgr._writeBehindAsyncProcess.Enqueue(new WriteBehindTask(internalCache, this, key, entry, source, taskId, _myProvider, operationCode, state));
		}

		public void WriteBehind(CacheBase internalCache, object[] keys, object[] values, CacheEntry[] entries, string source, string taskId, OpCode operationCode, WriteBehindAsyncProcessor.TaskState state)
		{
			if(_asyncWrites && (_context.DatasourceMgr._writeBehindAsyncProcess != null))
				_context.DatasourceMgr._writeBehindAsyncProcess.Enqueue(new BulkWriteBehindTask(internalCache, this, keys, values, entries, source, taskId, _myProvider, operationCode, state));
		}

		private Hashtable WriteThru(string[] keys, object[] values, OpCode opCode)
		{
			if(_dsWriter != null)
			{
				switch(opCode)
				{
				case OpCode.Add:
					return _dsWriter.AddToSource(keys, values);
				case OpCode.Update:
					return _dsWriter.UpdateSource(keys, values);
				case OpCode.Remove:
					return _dsWriter.RemoveFromSource(keys);
				}
			}
			return null;
		}

		private int WriteThru(string key, object val, OpCode opCode)
		{
			if(_dsWriter != null)
			{
				switch(opCode)
				{
				case OpCode.Add:
					if(_dsWriter.AddToSource(key, val))
						return 1;
					return 0;
				case OpCode.Update:
					if(_dsWriter.UpdateSource(key, val))
						return 1;
					return 0;
				case OpCode.Remove:
					if(_dsWriter.RemoveFromSource(key))
						return 1;
					return 0;
				case OpCode.Clear:
					if(_dsWriter.Clear())
						return 1;
					return 0;
				}
			}
			return -1;
		}

		public bool WriteThru(CacheBase cacheImpl, string key, CacheEntry cacheEntry, OpCode operationCode, OperationContext operationContext)
		{
			if((_context.DatasourceMgr == null) || ((_context != null) && !_context.DatasourceMgr.IsWriteThruEnabled))
				throw new OperationFailedException("Backing source not available. Verify backing source settings");
			Exception inner = null;
			int num = 0;
			try
			{
				object serializedObject = null;
				if(((cacheEntry != null) && (operationCode != OpCode.Remove)) && (operationCode != OpCode.Clear))
				{
					if(cacheEntry.Value is CallbackEntry)
						serializedObject = ((CallbackEntry)cacheEntry.Value).Value;
					else
						serializedObject = cacheEntry.Value;
					if(serializedObject != null)
					{
						serializedObject = CompressionUtil.Decompress(((UserBinaryObject)serializedObject).GetFullObject() as byte[], cacheEntry.Flag);
						serializedObject = SerializationUtil.SafeDeserialize(serializedObject, _context.SerializationContext, cacheEntry.Flag);
					}
				}
				num = this.WriteThru(key, serializedObject, operationCode);
			}
			catch(Exception exception2)
			{
				inner = exception2;
			}
			finally
			{
				if(num == 0)
				{
					try
					{
						switch(operationCode)
						{
						case OpCode.Add:
						case OpCode.Update:
							cacheImpl.Remove(key, ItemRemoveReason.Removed, false, null, 0, LockAccessType.IGNORE_LOCK, operationContext);
							break;
						}
					}
					catch(Exception exception3)
					{
						throw new OperationFailedException("error while trying to synchronize the cache with data source. Error: " + exception3.Message, exception3);
					}
					if(inner != null)
						throw new OperationFailedException("IWriteThruProvider failed. Error: " + inner.Message, inner);
				}
			}
			if(num != 1)
				return false;
			return true;
		}

		public Hashtable WriteThru(CacheBase cacheImpl, string[] keys, object[] values, CacheEntry[] entries, Hashtable returnSet, OpCode operationCode, OperationContext operationContext)
		{
			Exception inner = null;
			Hashtable hashtable = null;
			try
			{
				int num;
				if((values == null) || (values.Length <= 0))
					hashtable = this.WriteThru(keys, values, operationCode);
				num = 0;
				while (num < values.Length)
				{
					values[num] = SerializationUtil.SafeDeserialize(values[num], _context.SerializationContext, null);
					num++;
				}
			}
			catch(Exception exception2)
			{
				inner = exception2;
			}
			finally
			{
				if((hashtable == null) || (hashtable.Count <= 0))
					if(inner != null)
						throw new OperationFailedException("IWriteThruProvider.AddToSource failed. Error: " + inner.Message, inner);
				string[] strArray = null;
				int num3 = 0;
				switch(operationCode)
				{
				case OpCode.Add:
				case OpCode.Update:
					if(inner == null)
					{
						strArray = new string[hashtable.Count];
						foreach (DictionaryEntry entry in hashtable)
							strArray[num3++] = entry.Key as string;
						break;
					}
					strArray = keys;
					if((hashtable != null) && (hashtable.Count > 0))
					{
						IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
						while (enumerator.MoveNext())
							returnSet[enumerator.Key] = enumerator.Value;
					}
					break;
				default:
					break;
				}
				try
				{
					switch(operationCode)
					{
					case OpCode.Add:
					case OpCode.Update:
						if(strArray.Length > 0)
							cacheImpl.Remove(strArray, ItemRemoveReason.Removed, false, operationContext);
						if(inner != null)
							throw new OperationFailedException("IWriteThruProvider.AddToSource failed. Error: " + inner.Message, inner);
						break;
					}
				}
				catch(Exception exception3)
				{
					throw new OperationFailedException("error while trying to synchronize the cache with data source. Error: " + exception3.Message, exception3);
				}
				if(inner != null)
					throw new OperationFailedException("IWriteThruProvider.AddToSource failed. Error: " + inner.Message, inner);
			}
			return hashtable;
		}

		internal class BulkWriteBehindTask : WriteBehindAsyncProcessor.IWriteBehindTask, ICompactSerializable
		{
			private CacheEntry[] _entries;

			private CacheBase _internalCache;

			private object[] _keys;

			private OpCode _opCode;

			private WriteThruProviderManager _parent;

			private string _providerName;

			private string _source;

			private WriteBehindAsyncProcessor.TaskState _state;

			private string _taskId;

			private object[] _values;

			public CacheBase CacheImpl
			{
				get
				{
					return _internalCache;
				}
				set
				{
					_internalCache = value;
				}
			}

			public WriteThruProviderManager Manager
			{
				get
				{
					return _parent;
				}
				set
				{
					_parent = value;
				}
			}

			public OpCode OperationCode
			{
				get
				{
					return _opCode;
				}
			}

			public string ProviderName
			{
				get
				{
					return _providerName;
				}
			}

			public long Size
			{
				get
				{
					long num = 0;
					if(_entries != null)
					{
						for(int i = 0; i < _entries.Length; i++)
						{
							num += _entries[i].Size;
						}
					}
					return num;
				}
			}

			public string Source
			{
				get
				{
					return _source;
				}
			}

			public WriteBehindAsyncProcessor.TaskState State
			{
				get
				{
					return _state;
				}
				set
				{
					_state = value;
				}
			}

			public string TaskId
			{
				get
				{
					return _taskId;
				}
			}

			public BulkWriteBehindTask(CacheBase internalCache, WriteThruProviderManager parent, object[] keys, object[] values, CacheEntry[] entries, string source, string taskId, string providerName, OpCode opCode) : this(internalCache, parent, keys, values, entries, source, taskId, providerName, opCode, WriteBehindAsyncProcessor.TaskState.Wait)
			{
			}

			public BulkWriteBehindTask(CacheBase internalCache, WriteThruProviderManager parent, object[] keys, object[] values, CacheEntry[] entries, string source, string taskId, string providerName, OpCode opCode, WriteBehindAsyncProcessor.TaskState state)
			{
				_internalCache = internalCache;
				_parent = parent;
				_keys = keys;
				_values = values;
				_entries = entries;
				_opCode = opCode;
				_taskId = taskId;
				_providerName = providerName;
				_source = source;
				_state = state;
			}

			public void Deserialize(CompactReader reader)
			{
				_keys = reader.ReadObject() as object[];
				_entries = reader.ReadObject() as CacheEntry[];
				_values = reader.ReadObject() as object[];
				_opCode = (OpCode)reader.ReadInt32();
				_source = reader.ReadString();
				_taskId = reader.ReadString();
				_state = (WriteBehindAsyncProcessor.TaskState)reader.ReadByte();
				_providerName = reader.ReadString();
			}

			public void Process()
			{
				Hashtable result = null;
				CallbackEntry cbEntry = null;
				if(((_entries != null) && (_entries[0] != null)) && (_entries[0].Value is CallbackEntry))
				{
					cbEntry = _entries[0].Value as CallbackEntry;
				}
				try
				{
					result = _parent.WriteThru(_internalCache, _keys as string[], _values, _entries, new Hashtable(), _opCode, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					if(result == null)
					{
						result = new Hashtable(_keys.Length);
					}
					if(cbEntry != null)
					{
						for(int i = 0; i < _keys.Length; i++)
						{
							if(!result.ContainsKey(_keys[i]))
							{
								result.Add(_keys[i], "1");
							}
						}
					}
				}
				catch(Exception exception)
				{
					_internalCache.Context.Logger.Error("WriteBehindTask.Process".GetFormatter(), exception.GetFormatter());
					if(cbEntry != null)
					{
						for(int j = 0; j < _keys.Length; j++)
						{
							result.Add(_keys[j], exception);
						}
					}
				}
				if(cbEntry != null)
				{
					try
					{
						_internalCache.NotifyWriteBehindTaskStatus(_opCode, result, cbEntry, _taskId, _providerName, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					}
					catch
					{
					}
				}
			}

			public void Serialize(CompactWriter writer)
			{
				writer.WriteObject(_keys);
				writer.WriteObject(_entries);
				writer.WriteObject(_values);
				writer.Write((int)_opCode);
				writer.Write(_source);
				writer.Write(_taskId);
				writer.Write((byte)_state);
				writer.Write(_providerName);
			}

			public void Update(Hashtable updates)
			{
				if(updates.Count != _keys.Length)
				{
					object[] objArray = new object[updates.Count];
					CacheEntry[] entryArray = null;
					object[] objArray2 = null;
					if(_opCode == OpCode.Remove)
					{
						entryArray = new CacheEntry[updates.Count];
					}
					else
					{
						objArray2 = new object[updates.Count];
					}
					int index = 0;
					int num2 = 0;
					while (index < _keys.Length)
					{
						if(updates.ContainsKey(_keys[index]))
						{
							objArray[num2] = _keys[index];
							if(_opCode == OpCode.Remove)
							{
								entryArray[num2++] = _entries[index];
							}
							else
							{
								objArray2[num2++] = _values[index];
							}
						}
						index++;
					}
				}
			}
		}

		internal class WriteBehindTask : WriteBehindAsyncProcessor.IWriteBehindTask, ICompactSerializable
		{
			private CacheEntry _entry;

			private CacheBase _internalCache;

			private object _key;

			private OpCode _opCode;

			private WriteThruProviderManager _parent;

			private string _providerName;

			private string _source;

			private WriteBehindAsyncProcessor.TaskState _state;

			private string _taskId;

			public CacheBase CacheImpl
			{
				get
				{
					return _internalCache;
				}
				set
				{
					_internalCache = value;
				}
			}

			public CacheEntry Entry
			{
				get
				{
					return _entry;
				}
			}

			public WriteThruProviderManager Manager
			{
				get
				{
					return _parent;
				}
				set
				{
					_parent = value;
				}
			}

			public OpCode OperationCode
			{
				get
				{
					return _opCode;
				}
			}

			public string ProviderName
			{
				get
				{
					return _providerName;
				}
			}

			public long Size
			{
				get
				{
					if(_entry != null)
						return (long)_entry.Size;
					return 0;
				}
			}

			public string Source
			{
				get
				{
					return _source;
				}
			}

			public WriteBehindAsyncProcessor.TaskState State
			{
				get
				{
					return _state;
				}
				set
				{
					_state = value;
				}
			}

			public string TaskId
			{
				get
				{
					return _taskId;
				}
			}

			public WriteBehindTask(CacheBase internalCache, WriteThruProviderManager parent, object key, CacheEntry entry, string source, string taskId, string providerName, OpCode opCode) : this(internalCache, parent, key, entry, source, taskId, providerName, opCode, WriteBehindAsyncProcessor.TaskState.Wait)
			{
			}

			public WriteBehindTask(CacheBase internalCache, WriteThruProviderManager parent, object key, CacheEntry entry, string source, string taskId, string providerName, OpCode opCode, WriteBehindAsyncProcessor.TaskState state)
			{
				_internalCache = internalCache;
				_parent = parent;
				_key = key;
				_entry = entry;
				_opCode = opCode;
				_source = source;
				_taskId = taskId;
				_providerName = providerName;
				_state = state;
			}

			public void Deserialize(CompactReader reader)
			{
				_key = reader.ReadObject();
				_entry = reader.ReadObject() as CacheEntry;
				_opCode = (OpCode)reader.ReadInt32();
				_source = reader.ReadString();
				_taskId = reader.ReadString();
				_state = (WriteBehindAsyncProcessor.TaskState)reader.ReadByte();
				_providerName = reader.ReadString();
			}

			public void Process()
			{
				Hashtable result = new Hashtable(1);
				string key = _key as string;
				CallbackEntry cbEntry = null;
				if(_opCode == OpCode.Clear)
				{
					key = string.Empty;
				}
				try
				{
					bool flag = _parent.WriteThru(_internalCache, key, _entry, _opCode, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					_internalCache.DoWrite("IWriteBehindTask.Process", "taskId=" + _taskId + "operation result=" + (flag ? "true" : "false"), new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
					result.Add(key, flag ? "1" : "0");
				}
				catch(Exception exception)
				{
					if(_internalCache.Context.Logger.IsErrorEnabled)
					{
						_internalCache.Context.Logger.Error(("WriteBehindTask.Process: " + exception.Message).GetFormatter());
					}
					result.Add(key, exception);
				}
				if((_entry != null) && (_entry.Value is CallbackEntry))
				{
					cbEntry = _entry.Value as CallbackEntry;
				}
				try
				{
					_internalCache.NotifyWriteBehindTaskStatus(_opCode, result, cbEntry, _taskId, _providerName, new OperationContext(OperationContextFieldName.OperationType, OperationContextOperationType.CacheOperation));
				}
				catch
				{
				}
			}

			public void Serialize(CompactWriter writer)
			{
				writer.WriteObject(_key);
				writer.WriteObject(_entry);
				writer.Write((int)_opCode);
				writer.Write(_source);
				writer.Write(_taskId);
				writer.Write((byte)_state);
				writer.Write(_providerName);
			}

			public void Update(Hashtable updates)
			{
			}
		}
	}
}
