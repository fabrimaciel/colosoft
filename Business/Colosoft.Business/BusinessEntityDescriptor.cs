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

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação do descritor de uma entidade.
	/// </summary>
	public class BusinessEntityDescriptor : NotificationObject, IBindableEntityDescriptor, IEntityRecordObserver, Colosoft.Query.IRecordKeyEquatable, IDisposable, IEquatable<IEntityDescriptor>, IEntityDescriptorStateble
	{
		private int _id;

		private string _name;

		private string _description;

		private bool _isActive;

		private bool _isExpired;

		private System.Collections.Hashtable _fields = new System.Collections.Hashtable();

		private Query.RecordKey _recordKey;

		/// <summary>
		/// Instancia do observer do registro associado com a instancia.
		/// </summary>
		private Colosoft.Query.IRecordObserver _recordObserver;

		/// <summary>
		/// Loader da entidade associada.
		/// </summary>
		private IEntityLoader _entityLoader;

		/// <summary>
		/// Identificador da entidade.
		/// </summary>
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				if(_id != value)
				{
					_id = value;
					RaisePropertyChanged("Id");
				}
			}
		}

		/// <summary>
		/// Nome da entidade.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(_name != value)
				{
					_name = value;
					RaisePropertyChanged("Name");
				}
			}
		}

		/// <summary>
		/// Descrição da entidade.
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if(_description != value)
				{
					_description = value;
					RaisePropertyChanged("Description");
				}
			}
		}

		/// <summary>
		/// Identifica se a instancia está ativa.
		/// </summary>
		public virtual bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				if(_isActive != value)
				{
					_isActive = value;
					RaisePropertyChanged("IsActive");
				}
			}
		}

		/// <summary>
		/// Identifica se a instancia está expirada.
		/// </summary>
		public virtual bool IsExpired
		{
			get
			{
				return _isExpired;
			}
			set
			{
				if(_isExpired != value)
				{
					_isExpired = value;
					RaisePropertyChanged("IsExpired");
				}
			}
		}

		/// <summary>
		/// Representa o chave da entidade associada.
		/// </summary>
		public Query.RecordKey RecordKey
		{
			get
			{
				return _recordKey;
			}
			set
			{
				if(_recordKey != value)
				{
					_recordKey = value;
					RaisePropertyChanged("EntityRecordKey");
				}
			}
		}

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		public virtual Type ModelType
		{
			get
			{
				return _entityLoader.DataModelType;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="args"></param>
		public BusinessEntityDescriptor(CreateEntityDescriptorArgs args)
		{
			args.Require("args").NotNull();
			_entityLoader = args.Loader;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~BusinessEntityDescriptor()
		{
			Dispose(false);
		}

		/// <summary>
		/// Remove o registro do observer do registro associado com a entidade.
		/// </summary>
		private void UnregisterRecordObserver()
		{
			var modelType = ModelType;
			var manager = Colosoft.Query.RecordObserverManager.Instance;
			if(modelType != null && _recordObserver != null && manager != null && manager.IsEnabled)
			{
				manager.Unregister(Colosoft.Reflection.TypeName.Get(modelType), _recordObserver);
				if(_recordObserver is IDisposable)
					((IDisposable)_recordObserver).Dispose();
				_recordObserver = null;
			}
		}

		/// <summary>
		/// Método acionado quando o valor do campo for atualizado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		protected virtual void OnUpdateField(string fieldName, object value)
		{
		}

		/// <summary>
		/// Método usado para criar o observer do registro.
		/// </summary>
		/// <returns></returns>
		protected virtual Colosoft.Query.IRecordObserver CreateRecordObserver()
		{
			return new EntityDescriptorRecordObserver(this);
		}

		/// <summary>
		/// Adiciona um campo para a instancia.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		protected void Update(string fieldName, object value)
		{
			fieldName.Require("fieldName").NotNull().NotEmpty();
			_fields[fieldName] = value;
			OnUpdateField(fieldName, value);
		}

		/// <summary>
		/// Configura o controle de status associado.
		/// </summary>
		/// <param name="isActive">Identifica se os dados estão ativos.</param>
		/// <param name="isExpired">Identifica se os dados foram expirados.</param>
		public void ConfigureStatusControl(bool isActive, bool isExpired)
		{
			_isActive = isActive;
			_isExpired = isExpired;
			RaisePropertyChanged("IsActive", "IsExpired");
		}

		/// <summary>
		/// Compara a instancia com a chave do registro informada.
		/// </summary>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		public bool Equals(Colosoft.Query.RecordKey recordKey)
		{
			return Equals(recordKey, Query.RecordKeyComparisonType.Key);
		}

		/// <summary>
		/// Verifica se a instancia possui dados iguais a chave
		/// de registro informada.
		/// </summary>
		/// <param name="recordKey">Instancia da chave que será comparada.</param>
		/// <param name="comparisonType">Tipo de comparação que será realizada.</param>
		/// <returns></returns>
		public virtual bool Equals(Colosoft.Query.RecordKey recordKey, Colosoft.Query.RecordKeyComparisonType comparisonType)
		{
			recordKey.Require("recordKey").NotNull();
			bool result = false;
			if((comparisonType & Query.RecordKeyComparisonType.Key) == Query.RecordKeyComparisonType.Key)
				result = recordKey.Key == _recordKey.Key;
			if((comparisonType & Query.RecordKeyComparisonType.RowVersion) == Query.RecordKeyComparisonType.RowVersion)
				result = result || _recordKey.RowVersion == recordKey.RowVersion;
			return result;
		}

		/// <summary>
		/// Registra a instancia para ser observada.
		/// </summary>
		/// <param name="recordKey">Chave do registro que representa a instancia.</param>
		void IEntityRecordObserver.RegisterObserver(Query.RecordKey recordKey)
		{
			recordKey.Require("record").NotNull();
			var manager = Colosoft.Query.RecordObserverManager.Instance;
			var modelType = ModelType;
			if(manager != null && manager.IsEnabled && ModelType != null)
			{
				if(_recordObserver != null)
					UnregisterRecordObserver();
				var observer = CreateRecordObserver();
				manager.Register(Colosoft.Reflection.TypeName.Get(modelType), recordKey, observer);
				_recordObserver = observer;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			UnregisterRecordObserver();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Vincula os dados adicionais.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public virtual IEnumerable<string> Bind(Query.IRecord record, Query.BindStrategyMode mode)
		{
			IEnumerable<string> fields = new string[0];
			if(_entityLoader != null)
			{
				if(_entityLoader.FindNameProperties != null)
					fields = fields.Concat(_entityLoader.FindNameProperties);
				if(!string.IsNullOrEmpty(_entityLoader.DescriptionPropertyName))
					fields = fields.Concat(new string[] {
						_entityLoader.DescriptionPropertyName
					});
				if(!string.IsNullOrEmpty(_entityLoader.UidPropertyName))
					fields = fields.Concat(new string[] {
						_entityLoader.UidPropertyName
					});
				if(typeof(Data.BaseTraceableModel).IsAssignableFrom(_entityLoader.DataModelType))
					fields = fields.Concat(new string[] {
						"ActivatedDate",
						"ExpiredDate"
					});
			}
			foreach (var field in fields)
			{
				if(record.Descriptor.Contains(field))
				{
					var index = record.Descriptor.GetFieldPosition(field);
					if(index >= 0)
						Update(field, record.GetValue(index));
				}
			}
			return new string[0];
		}

		/// <summary>
		/// Verifica se as instâncias são iguais.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj as IEntityDescriptor);
		}

		/// <summary>
		/// Recupera o código Hash da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return EntityDescriptorIdComparer.Instance.GetHashCode(this);
		}

		/// <summary>
		/// Verifica se as instâncias são iguais.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IEntityDescriptor other)
		{
			return EntityDescriptorIdComparer.Instance.Equals(this, other);
		}

		/// <summary>
		/// Quantidades de campos associados.
		/// </summary>
		int IEntityDescriptorStateble.FieldsCount
		{
			get
			{
				return _fields.Count;
			}
		}

		/// <summary>
		/// Recupera o valor do campos associado com o nome informado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		object IEntityDescriptorStateble.this[string fieldName]
		{
			get
			{
				if(!_fields.ContainsKey(fieldName))
					throw new KeyNotFoundException(string.Format("Field is name {0} not found.", fieldName));
				return _fields[fieldName];
			}
		}

		/// <summary>
		/// Verifica se na instancia existe o campo com o nome informado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		bool IEntityDescriptorStateble.Contains(string fieldName)
		{
			return _fields.ContainsKey(fieldName);
		}

		/// <summary>
		/// Recupera a relação dos nomes dos campos associados.
		/// </summary>
		/// <returns></returns>
		string[] IEntityDescriptorStateble.GetFields()
		{
			var index = 0;
			var result = new string[_fields.Count];
			foreach (var key in _fields.Keys)
				result[index++] = key.ToString();
			return result;
		}

		/// <summary>
		/// Adiciona um campo para a instancia.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		void IEntityDescriptorStateble.Update(string fieldName, object value)
		{
			Update(fieldName, value);
		}

		/// <summary>
		/// Remove o campo da instancia.
		/// </summary>
		/// <param name="fieldName"></param>
		void IEntityDescriptorStateble.Remove(string fieldName)
		{
			_fields.Remove(fieldName);
		}

		/// <summary>
		/// Implementação o observer do registro associado com o descritor da entidade.
		/// </summary>
		class EntityDescriptorRecordObserver : Colosoft.Query.IRecordObserver
		{
			private BusinessEntityDescriptor _entityDescriptor;

			private ulong _uid;

			/// <summary>
			/// Identificador unico do observer.
			/// </summary>
			public ulong Uid
			{
				get
				{
					return _uid;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="entityDescriptor"></param>
			public EntityDescriptorRecordObserver(BusinessEntityDescriptor entityDescriptor)
			{
				entityDescriptor.Require("entityDescriptor").NotNull();
				_uid = Query.RecordObserverUidGenerator.CreateUid();
				_entityDescriptor = entityDescriptor;
			}

			/// <summary>
			/// Método usado para notificar que o registro foi alterado.
			/// </summary>
			/// <param name="record"></param>
			public void OnChanged(Query.IRecord record)
			{
				var bindingStrategy = _entityDescriptor._entityLoader.GetEntityDescriptorBindStrategy();
				object instance = _entityDescriptor;
				var propertiesChanged = bindingStrategy.Bind(record, Query.BindStrategyMode.Differences, ref instance).ToArray();
				_entityDescriptor.RaisePropertyChanged(propertiesChanged);
			}
		}
	}
}
