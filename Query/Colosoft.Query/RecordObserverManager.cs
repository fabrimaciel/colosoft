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

namespace Colosoft.Query
{
	/// <summary>
	/// Classe geral usada para armazena a instancia do observer
	/// utilizado pelo sistema.
	/// </summary>
	public class RecordObserverManager : IRecordObserverManager
	{
		private static IRecordObserverManager _instance;

		private Dictionary<string, TypeNameEntry> _typeNameEntries = new Dictionary<string, TypeNameEntry>();

		private DateTime _nextValidatingEntries = DateTime.Now.AddSeconds(15);

		private bool _validatingEntries = false;

		private bool _isEnabled;

		/// <summary>
		/// Relação dos tipos que devem ser ignorados.
		/// </summary>
		private List<Colosoft.Reflection.TypeName> _ignoreTypes = new List<Colosoft.Reflection.TypeName>();

		/// <summary>
		/// Evento acionado quando o registro de um terminado tipo sofrer alguma alteração.
		/// </summary>
		public event EventHandler<TypeRecordChangedEventArgs> TypeRecordChanged;

		/// <summary>
		/// Instancia do manager utilizado pelo sistma.
		/// </summary>
		public static IRecordObserverManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = new RecordObserverManager();
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Quantidade de observers de registros registrados.
		/// </summary>
		public int RecordObserversCount
		{
			get
			{
				return _typeNameEntries.Values.Select(f => f.RecordObserverEntriesCount).Sum();
			}
		}

		/// <summary>
		/// Quantidade de observers de resultado de consultas registradas.
		/// </summary>
		public int QueryResultObserverCount
		{
			get
			{
				return _typeNameEntries.Values.Select(f => f.QueryResultObserverEntriesCount).Sum();
			}
		}

		/// <summary>
		/// Identifica que o gerenciador de observers de registros está habilitado.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				if(!value)
					lock (_typeNameEntries)
						_typeNameEntries.Clear();
				_isEnabled = value;
			}
		}

		/// <summary>
		/// Método que processa a validação da entradas.
		/// </summary>
		/// <param name="state"></param>
		private void DoValidateEntries(object state)
		{
			TypeNameEntry[] entries = null;
			lock (_typeNameEntries)
				entries = _typeNameEntries.Values.ToArray();
			var validateDate = DateTime.Now.AddSeconds(-15);
			foreach (var i in entries)
				if(validateDate > i.LastRefresh)
					try
					{
						i.Refresh();
					}
					catch
					{
					}
			_nextValidatingEntries = DateTime.Now.AddSeconds(15);
			_validatingEntries = false;
		}

		/// <summary>
		/// Valida as entradas do gerenciador.
		/// </summary>
		private void ValidateEntries()
		{
			if(_validatingEntries || _nextValidatingEntries > DateTime.Now)
				return;
			_validatingEntries = true;
			if(!System.Threading.ThreadPool.QueueUserWorkItem(DoValidateEntries, null))
				DoValidateEntries(null);
		}

		/// <summary>
		/// Recupera uma entrada do nome do tipo.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private TypeNameEntry GetTypeNameEntry(Colosoft.Reflection.TypeName typeName)
		{
			ValidateEntries();
			lock (_typeNameEntries)
			{
				TypeNameEntry entry = null;
				if(!_typeNameEntries.TryGetValue(typeName.FullName, out entry))
				{
					entry = new TypeNameEntry(typeName, this);
					_typeNameEntries.Add(typeName.FullName, entry);
				}
				else
					entry.Refresh();
				return entry;
			}
		}

		/// <summary>
		/// Método aciona quando o registro sofreu uma alteração.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="record"></param>
		protected virtual void OnTypeRecordChanged(Colosoft.Reflection.TypeName typeName, IRecord record)
		{
			if(TypeRecordChanged != null)
				TypeRecordChanged(this, new TypeRecordChangedEventArgs(typeName, TypeRecordChangeType.Update, record, null));
		}

		/// <summary>
		/// Método acionado quando um registro for inserido.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="record"></param>
		protected virtual void OnTypeRecordInserted(Colosoft.Reflection.TypeName typeName, IRecord record)
		{
			if(TypeRecordChanged != null)
				TypeRecordChanged(this, new TypeRecordChangedEventArgs(typeName, TypeRecordChangeType.Insert, record, null));
		}

		/// <summary>
		/// Método acionado quando um registro for apagado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="recordKey"></param>
		protected virtual void OnTypeRecordDeleted(Colosoft.Reflection.TypeName typeName, RecordKey recordKey)
		{
			if(TypeRecordChanged != null)
				TypeRecordChanged(this, new TypeRecordChangedEventArgs(typeName, TypeRecordChangeType.Delete, null, recordKey));
		}

		/// <summary>
		/// Verifica se o tipo é valido para o gerenciado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		protected bool IsValidType(Colosoft.Reflection.TypeName typeName)
		{
			return _ignoreTypes.BinarySearch(typeName, Colosoft.Reflection.TypeName.TypeNameFullNameComparer.Instance) < 0;
		}

		/// <summary>
		/// Adicionar o tipo que deve ser ignorado pelo gerenciador.
		/// </summary>
		/// <param name="typeNames"></param>
		public void IgnoreTypes(params Colosoft.Reflection.TypeName[] typeNames)
		{
			if(typeNames == null)
				return;
			foreach (var i in typeNames)
				IgnoreType(i);
		}

		/// <summary>
		/// Adicionar o tipo que deve ser ignorado pelo gerenciador.
		/// </summary>
		/// <param name="typeName"></param>
		public void IgnoreType(Colosoft.Reflection.TypeName typeName)
		{
			typeName.Require("typeName").NotNull();
			int index = _ignoreTypes.BinarySearch(typeName, Colosoft.Reflection.TypeName.TypeNameFullNameComparer.Instance);
			if(index < 0)
				_ignoreTypes.Insert(~index, typeName);
		}

		/// <summary>
		/// Recupera as descrição das entradas dos tipos registrados.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetTypeEntryDescriptions()
		{
			lock (_typeNameEntries)
				return _typeNameEntries.OrderBy(f => f.Value.QueryResultObserverEntriesCount).Select(f => f.Value.ToString()).ToArray();
		}

		/// <summary>
		/// Registro o registro para ser observado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="recordKey"></param>
		/// <param name="observer"></param>
		public void Register(Reflection.TypeName typeName, Query.RecordKey recordKey, Colosoft.Query.IRecordObserver observer)
		{
			if(IsEnabled && IsValidType(typeName))
			{
				var typeNameEntry = GetTypeNameEntry(typeName);
				typeNameEntry.AddRecordKey(recordKey, observer);
			}
		}

		/// <summary>
		/// Registra o observer para o resultado da consulta. Toda vez 
		/// que ocorrer alguma alteração que implique sobre os dados
		/// do resultado o observer será notificado.
		/// </summary>
		/// <param name="typeName">Nome do tipo no qual o resultado se basea.</param>
		/// <param name="observer">Instancia do observer.</param>
		public void Register(Colosoft.Reflection.TypeName typeName, IQueryResultChangedObserver observer)
		{
			if(IsEnabled && IsValidType(typeName))
			{
				var typeNameEntry = GetTypeNameEntry(typeName);
				typeNameEntry.AddObserver(observer);
			}
		}

		/// <summary>
		/// Remove o observador do sistema.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="observer"></param>
		public void Unregister(Reflection.TypeName typeName, Colosoft.Query.IRecordObserver observer)
		{
			if(IsEnabled && IsValidType(typeName))
			{
				var typeNameEntry = GetTypeNameEntry(typeName);
				typeNameEntry.RemoveObserver(observer);
			}
		}

		/// <summary>
		/// Remove o registro do observer.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="observer"></param>
		public void Unregister(Colosoft.Reflection.TypeName typeName, IQueryResultChangedObserver observer)
		{
			if(IsEnabled)
			{
				var typeNameEntry = GetTypeNameEntry(typeName);
				typeNameEntry.RemoveObserver(observer);
			}
		}

		/// <summary>
		/// Notifica que um registro foi apagado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="recordKeys"></param>
		public void NotifyRecordDeleted(Colosoft.Reflection.TypeName typeName, IEnumerable<RecordKey> recordKeys)
		{
			if(!IsEnabled || !IsValidType(typeName))
				return;
			var typeNameEntry = GetTypeNameEntry(typeName);
			var observers = typeNameEntry.GetChangeObserverEntries();
			if(observers == null)
				return;
			foreach (var key in recordKeys)
			{
				for(var i = 0; i < observers.Count; i++)
				{
					var entry = observers[i];
					if(!entry.IsAlive)
					{
						typeNameEntry.RemoveObserver(entry);
						observers.RemoveAt(i--);
					}
					else
					{
						var observer = entry.Observer;
						if(observer.IsAlive && observer.Evaluate(key))
							observer.OnRecordDeleted(key);
					}
				}
				OnTypeRecordDeleted(typeName, key);
			}
			observers.Clear();
		}

		/// <summary>
		/// Notific que o registro foram inseridos.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="records"></param>
		public void NotifyRecordsInserted(Colosoft.Reflection.TypeName typeName, IEnumerable<IRecord> records)
		{
			if(!IsEnabled || !IsValidType(typeName))
				return;
			var typeNameEntry = GetTypeNameEntry(typeName);
			var observers = typeNameEntry.GetChangeObserverEntries();
			if(observers == null)
				return;
			foreach (var record in records)
			{
				for(var i = 0; i < observers.Count; i++)
				{
					var entry = observers[i];
					if(!entry.IsAlive)
					{
						typeNameEntry.RemoveObserver(entry);
						observers.RemoveAt(i--);
					}
					else
					{
						var observer = entry.Observer;
						if(observer.Evaluate(record))
							observer.OnRecordInserted(record);
					}
				}
				OnTypeRecordInserted(typeName, record);
			}
			observers.Clear();
		}

		/// <summary>
		/// Recupera o notificador de alterações no registro.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		public IRecordObserverNotifier GetRecordChangedNotifier(Colosoft.Reflection.TypeName typeName, RecordKey recordKey)
		{
			if(IsEnabled)
			{
				var typeNameEntry = GetTypeNameEntry(typeName);
				return typeNameEntry.GetNotifier(recordKey);
			}
			return null;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("TypeEntries: {0}; RecordObservers: {1}; QueryResultObserver: {2}", _typeNameEntries.Count, RecordObserversCount, QueryResultObserverCount);
		}

		/// <summary>
		/// Implementação do notificador para alterações no registro.
		/// </summary>
		class ChangeRecordObserverNotifier : IRecordObserverNotifier
		{
			private RecordKeyEntry _keyEntry;

			private TypeNameEntry _typeNameEntry;

			/// <summary>
			/// Identifica se o notificador é valido.
			/// </summary>
			public bool IsValid
			{
				get
				{
					return _keyEntry != null;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="keyEntry"></param>
			/// <param name="typeNameEntry"></param>
			public ChangeRecordObserverNotifier(RecordKeyEntry keyEntry, TypeNameEntry typeNameEntry)
			{
				_keyEntry = keyEntry;
				_typeNameEntry = typeNameEntry;
			}

			/// <summary>
			/// Notifica a alteração que contém os dados no registro informado.
			/// </summary>
			/// <param name="record"></param>
			public void Notify(IRecord record)
			{
				foreach (var observer in _keyEntry.GetObservers())
				{
					if(observer != null)
						observer.OnChanged(record);
				}
				_typeNameEntry.ObserverManager.OnTypeRecordChanged(_typeNameEntry.TypeName, record);
			}
		}

		/// <summary>
		/// Implementação da entrada do observer de alterações do resultado de consulta.
		/// </summary>
		class QueryResultChangeObserverEntry
		{
			private WeakReference _reference;

			private ulong _referenceUid;

			/// <summary>
			/// Hash code da referencia associada.
			/// </summary>
			public ulong ReferenceUid
			{
				get
				{
					return _referenceUid;
				}
			}

			/// <summary>
			/// Identifica se a entrada está viva.
			/// </summary>
			public bool IsAlive
			{
				get
				{
					if(_reference != null && _reference.IsAlive)
					{
						var target = _reference.Target;
						if(target is IDisposableState && ((IDisposableState)target).IsDisposed)
						{
							_reference = null;
							return false;
						}
						if(_reference.IsAlive)
						{
							var observer = _reference.Target as IQueryResultChangedObserver;
							if(observer != null)
								return observer.IsAlive;
						}
					}
					return false;
				}
			}

			/// <summary>
			/// Instancia do observer associado.
			/// </summary>
			public IQueryResultChangedObserver Observer
			{
				get
				{
					return _reference.Target as IQueryResultChangedObserver;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="observer">Instancia do observer a entrada.</param>
			public QueryResultChangeObserverEntry(IQueryResultChangedObserver observer)
			{
				_reference = new WeakReference(observer);
				_referenceUid = observer.Uid;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("QueryResultChangeObserverEntry (IsAlive={0})", IsAlive);
			}
		}

		/// <summary>
		/// Armazena a entrada do observer do registro.
		/// </summary>
		class RecordObserverEntry
		{
			private WeakReference _reference;

			/// <summary>
			/// Identifica se a entrada está viva.
			/// </summary>
			public bool IsAlive
			{
				get
				{
					if(_reference != null && _reference.IsAlive)
					{
						if(_reference.Target is IDisposableState && ((IDisposableState)_reference.Target).IsDisposed)
						{
							_reference = null;
							return false;
						}
						return true;
					}
					return false;
				}
			}

			/// <summary>
			/// Instancia do observer associado.
			/// </summary>
			public IRecordObserver Observer
			{
				get
				{
					return _reference.Target as IRecordObserver;
				}
			}

			/// <summary>
			/// Evento acionado quando a entrada for apagada.
			/// </summary>
			public event EventHandler Removed;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="observer">Instancia do observer que a entra encapsula.</param>
			public RecordObserverEntry(IRecordObserver observer)
			{
				_reference = new WeakReference(observer);
			}

			/// <summary>
			/// Remove a entrada.
			/// </summary>
			public void Remove()
			{
				if(Removed != null)
					Removed(this, EventArgs.Empty);
				_reference = null;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("RecordObserverEntry (IsAlive={0})", IsAlive);
			}
		}

		/// <summary>
		/// Armazena os dado da entrada de chave de um registro.
		/// </summary>
		class RecordKeyEntry
		{
			private string _key;

			private List<RecordObserverEntry> _observers;

			/// <summary>
			/// Chave associada.
			/// </summary>
			public string Key
			{
				get
				{
					return _key;
				}
			}

			/// <summary>
			/// Evento acionado quando a entrada foi removida.
			/// </summary>
			public event EventHandler Removed;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="key">Chave que a instancia representa.</param>
			public RecordKeyEntry(string key)
			{
				_key = key;
				_observers = new List<RecordObserverEntry>();
			}

			/// <summary>
			/// Adiciona um observer para a entrada.
			/// </summary>
			/// <param name="observer"></param>
			/// <returns>Entrada gerada para o observer.</returns>
			public RecordObserverEntry AddObserver(IRecordObserver observer)
			{
				var entry = new RecordObserverEntry(observer);
				entry.Removed += ObserverEntryRemoved;
				lock (_observers)
					_observers.Add(entry);
				return entry;
			}

			/// <summary>
			/// Recupera a instancia dos observer para serem processados.
			/// </summary>
			/// <returns></returns>
			public IEnumerable<IRecordObserver> GetObservers()
			{
				RecordObserverEntry[] entries = null;
				lock (_observers)
					entries = _observers.ToArray();
				for(var i = 0; i < entries.Length; i++)
					if(!entries[i].IsAlive)
						entries[i].Remove();
					else
						yield return entries[i].Observer;
			}

			/// <summary>
			/// Método acionado quando uma entrada do observer for removida.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void ObserverEntryRemoved(object sender, EventArgs e)
			{
				var observersCount = 0;
				lock (_observers)
				{
					var entry = (RecordObserverEntry)sender;
					entry.Removed -= ObserverEntryRemoved;
					for(var i = 0; i < _observers.Count; i++)
						if(!_observers[i].IsAlive)
							_observers.RemoveAt(i--);
					observersCount = _observers.Count;
				}
				if(observersCount == 0)
					if(Removed != null)
						Removed(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Armazena os dados da entrada do nome de um tipo.
		/// </summary>
		class TypeNameEntry
		{
			private Colosoft.Reflection.TypeName _typeName;

			private Dictionary<string, RecordKeyEntry> _keys;

			private RecordObserverManager _observerManager;

			private DateTime _lastRefresh = DateTime.Now;

			private readonly object _validaObjLock = new object();

			/// <summary>
			/// Dicionário com a relação das entradas de observers registrados.
			/// </summary>
			private Dictionary<ulong, RecordObserverEntry> _recordObserverEntries = new Dictionary<ulong, RecordObserverEntry>();

			/// <summary>
			/// Dicionário com a relação das entradas de observers registrados.
			/// </summary>
			private Dictionary<ulong, QueryResultChangeObserverEntry> _queryResultObserverEntries = new Dictionary<ulong, QueryResultChangeObserverEntry>();

			/// <summary>
			/// Horário do ultimo refresh do tipo.
			/// </summary>
			public DateTime LastRefresh
			{
				get
				{
					return _lastRefresh;
				}
			}

			/// <summary>
			/// Nome do tipo associado.
			/// </summary>
			public Colosoft.Reflection.TypeName TypeName
			{
				get
				{
					return _typeName;
				}
			}

			/// <summary>
			/// Instancia do gerenciador do observer.
			/// </summary>
			internal RecordObserverManager ObserverManager
			{
				get
				{
					return _observerManager;
				}
			}

			/// <summary>
			/// Quantidade de entradas de observers de registro.
			/// </summary>
			public int RecordObserverEntriesCount
			{
				get
				{
					return _recordObserverEntries.Count;
				}
			}

			/// <summary>
			/// Quantidade de entradas de observers de resultado de consultas.
			/// </summary>
			public int QueryResultObserverEntriesCount
			{
				get
				{
					return _queryResultObserverEntries.Count;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="typeName">Nome do tipo da entrada.</param>
			/// <param name="observerManager"></param>
			public TypeNameEntry(Colosoft.Reflection.TypeName typeName, RecordObserverManager observerManager)
			{
				_typeName = typeName;
				_observerManager = observerManager;
				_keys = new Dictionary<string, RecordKeyEntry>();
			}

			/// <summary>
			/// Valida os observers da instancia.
			/// </summary>
			public void Refresh()
			{
				_lastRefresh = DateTime.Now;
				lock (_validaObjLock)
				{
					var recordEntries = new List<RecordObserverEntry>();
					lock (_recordObserverEntries)
					{
						var entries = _recordObserverEntries.Where(f => !f.Value.IsAlive).ToArray();
						foreach (var i in entries)
						{
							_recordObserverEntries.Remove(i.Key);
							i.Value.Remove();
						}
					}
					lock (_queryResultObserverEntries)
					{
						var queryResultEntries = _queryResultObserverEntries.Where(f => !f.Value.IsAlive).Select(f => f.Key).ToArray();
						foreach (var i in queryResultEntries)
							_queryResultObserverEntries.Remove(i);
					}
				}
			}

			/// <summary>
			/// Adiciona o observer para a entrada do tipo.
			/// </summary>
			/// <param name="observer"></param>
			public void AddObserver(IQueryResultChangedObserver observer)
			{
				lock (_queryResultObserverEntries)
				{
					if(_queryResultObserverEntries.ContainsKey(observer.Uid))
						return;
					var entry = new QueryResultChangeObserverEntry(observer);
					_queryResultObserverEntries.Add(observer.Uid, entry);
				}
			}

			/// <summary>
			/// Adiciona a chave de um registro.
			/// </summary>
			/// <param name="recordKey"></param>
			/// <param name="recordObserver">Observer do registro.</param>
			/// <returns></returns>
			public void AddRecordKey(RecordKey recordKey, IRecordObserver recordObserver)
			{
				lock (_recordObserverEntries)
				{
					if(_recordObserverEntries.ContainsKey(recordObserver.Uid))
						return;
					RecordKeyEntry entry = null;
					var key = recordKey.Key;
					lock (_keys)
						if(!_keys.TryGetValue(key, out entry))
						{
							entry = new RecordKeyEntry(key);
							entry.Removed += RecordKeyEntryRemoved;
							_keys.Add(key, entry);
						}
					var observerEntry = entry.AddObserver(recordObserver);
					_recordObserverEntries.Add(recordObserver.Uid, observerEntry);
				}
			}

			/// <summary>
			/// Remove o observer associado.
			/// </summary>
			/// <param name="recordObserver">Instancia do observer que será removido.</param>
			/// <returns>True se for removido com sucesso.</returns>
			public bool RemoveObserver(IRecordObserver recordObserver)
			{
				if(recordObserver == null)
					return false;
				RecordObserverEntry entry = null;
				lock (_recordObserverEntries)
					if(_recordObserverEntries.TryGetValue(recordObserver.Uid, out entry))
						return false;
				if(entry != null)
				{
					_recordObserverEntries.Remove(recordObserver.Uid);
					entry.Remove();
					return true;
				}
				return false;
			}

			/// <summary>
			/// Remove o observer associado.
			/// </summary>
			/// <param name="queryResultObserver">Instancia do observer que será removido.</param>
			/// <returns>True se for removido com sucesso.</returns>
			public bool RemoveObserver(IQueryResultChangedObserver queryResultObserver)
			{
				lock (_queryResultObserverEntries)
					return _queryResultObserverEntries.Remove(queryResultObserver.Uid);
			}

			/// <summary>
			/// Remove a entrada do observer.
			/// </summary>
			/// <param name="entry"></param>
			/// <returns></returns>
			public bool RemoveObserver(QueryResultChangeObserverEntry entry)
			{
				lock (_queryResultObserverEntries)
					return _queryResultObserverEntries.Remove(entry.ReferenceUid);
			}

			/// <summary>
			/// Recupera o notificador associado com a chave do registor.
			/// </summary>
			/// <param name="recordKey"></param>
			/// <returns></returns>
			public IRecordObserverNotifier GetNotifier(RecordKey recordKey)
			{
				RecordKeyEntry entry = null;
				lock (_keys)
					_keys.TryGetValue(recordKey.Key, out entry);
				return new ChangeRecordObserverNotifier(entry, this);
			}

			/// <summary>
			/// Recupera os observers de alteração.
			/// </summary>
			/// <returns></returns>
			public List<QueryResultChangeObserverEntry> GetChangeObserverEntries()
			{
				lock (_queryResultObserverEntries)
				{
					if(_queryResultObserverEntries.Count == 0)
						return null;
					return _queryResultObserverEntries.Values.ToList();
				}
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("{0}; RecordObserverEntries: {1} -- QueryResultObserverEntries: {2}", _typeName, _recordObserverEntries.Count, _queryResultObserverEntries.Count);
			}

			/// <summary>
			/// Método acionado quando uma entrada da chave do registro for removida
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void RecordKeyEntryRemoved(object sender, EventArgs e)
			{
				var entry = (RecordKeyEntry)sender;
				entry.Removed -= RecordKeyEntryRemoved;
				lock (_keys)
					_keys.Remove(entry.Key);
			}
		}
	}
}
