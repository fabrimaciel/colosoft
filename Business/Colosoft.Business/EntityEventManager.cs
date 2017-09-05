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
	/// Armanze as informações do evento da entidade.
	/// </summary>
	public class EntityEventInfo
	{
		/// <summary>
		/// Tipo de evento.
		/// </summary>
		public EntityEventType EventType
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador da exportão da classe do evento.
		/// </summary>
		public int ExportId
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Implementação padrão do gerenciador de eventos das entidades.
	/// </summary>
	public class EntityEventManager : IEntityEventManager
	{
		private static object _staticObjLock = new object();

		/// <summary>
		/// Identifica se houve algum erro ao recuperar a instancia pelo ServiceLocator.
		/// </summary>
		private static bool _tryGetInstanceByServiceLocatorError = false;

		/// <summary>
		/// Armazena a relação dos evento escritos.
		/// </summary>
		private Dictionary<EntityEventType, List<Domain.SubscriptionToken>> _eventSubscribers = new Dictionary<EntityEventType, List<Domain.SubscriptionToken>>();

		private static IEntityEventManager _instance = null;

		/// <summary>
		/// Relação mdos tipos registrados.
		/// </summary>
		private List<Reflection.TypeName> _registeredTypes = new List<Reflection.TypeName>();

		/// <summary>
		/// Evento acionado quando ocorre um erro ao registra as informações do 
		/// evento de uma entidade.
		/// </summary>
		public event RegisterEntityEventInfoErrorHandler RegisterEventInfoError;

		/// <summary>
		/// Recupera a instancia do gerenciador e evento das entidades
		/// </summary>
		public static IEntityEventManager Instance
		{
			get
			{
				if(_instance == null)
					lock (_staticObjLock)
						if(_instance == null && !_tryGetInstanceByServiceLocatorError)
							try
							{
								_instance = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEntityEventManager>();
							}
							catch(Exception ex)
							{
								_tryGetInstanceByServiceLocatorError = true;
								System.Diagnostics.Trace.WriteLine(ex);
							}
				return _instance;
			}
			set
			{
				lock (_staticObjLock)
					_instance = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EntityEventManager()
		{
			_eventSubscribers.Add(EntityEventType.Initialized, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.PropertyChanging, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.PropertyChanged, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.Validating, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.Validated, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.Saving, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.Saved, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.Deleting, new List<Domain.SubscriptionToken>());
			_eventSubscribers.Add(EntityEventType.Deleted, new List<Domain.SubscriptionToken>());
		}

		/// <summary>
		/// Método acionado quando ocorre um erro no registro do EntityEventInfo.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="eventInfo"></param>
		/// <param name="error"></param>
		private void OnRegisterEventInfoError(Reflection.TypeName typeName, EntityEventInfo eventInfo, Exception error)
		{
			if(RegisterEventInfoError != null)
				RegisterEventInfoError(this, new RegisterEntityEventInfoErrorArgs(typeName, eventInfo, error));
		}

		/// <summary>
		/// Registra o evento para a entidade.
		/// </summary>
		/// <param name="entityType">Nome da entidade.</param>
		/// <param name="info">Informações do evento.</param>
		private void Register(Reflection.TypeName entityType, EntityEventInfo info)
		{
			Domain.SubscriptionToken token = null;
			switch(info.EventType)
			{
			case EntityEventType.Initialized:
				token = RegisterEvent<EntityInitializedEvent, IEntityInitializedEventSubscription, EntityInitializedEventArgs>(entityType, info);
				break;
			case EntityEventType.PropertyChanging:
				token = RegisterEvent<EntityPropertyChangingEvent, IEntityPropertyChangingEventSubscription, EntityPropertyChangingEventArgs>(entityType, info);
				break;
			case EntityEventType.PropertyChanged:
				token = RegisterEvent<EntityPropertyChangedEvent, IEntityPropertyChangedEventSubscription, EntityPropertyChangedEventArgs>(entityType, info);
				break;
			case EntityEventType.Validating:
				token = RegisterEvent<EntityValidatingEvent, IEntityValidatingEventSubscription, EntityValidatingEventArgs>(entityType, info);
				break;
			case EntityEventType.Validated:
				token = RegisterEvent<EntityValidatedEvent, IEntityValidatedEventSubscription, EntityValidatedEventArgs>(entityType, info);
				break;
			case EntityEventType.Saving:
				token = RegisterEvent<EntitySavingEvent, IEntitySavingEventSubscription, EntitySavingEventArgs>(entityType, info);
				break;
			case EntityEventType.Saved:
				token = RegisterEvent<EntitySavedEvent, IEntitySavedEventSubscription, EntitySavedEventArgs>(entityType, info);
				break;
			case EntityEventType.Deleting:
				token = RegisterEvent<EntityDeletingEvent, IEntityDeletingEventSubscription, EntityDeletingEventArgs>(entityType, info);
				break;
			case EntityEventType.Deleted:
				token = RegisterEvent<EntityDeletedEvent, IEntityDeletedEventSubscription, EntityDeletedEventArgs>(entityType, info);
				break;
			}
			if(token != null)
			{
				List<Domain.SubscriptionToken> tokens = _eventSubscribers[info.EventType];
				lock (tokens)
					tokens.Add(token);
			}
		}

		/// <summary>
		/// Registra o evento para a entidade.
		/// </summary>
		/// <typeparam name="TEvent">Tipo da classe do evento.</typeparam>
		/// <typeparam name="TExecuteSubscription">Tipo da inscrição de execução.</typeparam>
		/// <typeparam name="TArgs">Tipo dos argumentos do evento.</typeparam>
		/// <param name="entityType">Nome da entidade associada.</param>
		/// <param name="info"></param>
		private Domain.SubscriptionToken RegisterEvent<TEvent, TExecuteSubscription, TArgs>(Reflection.TypeName entityType, EntityEventInfo info) where TEvent : Domain.CompositeDomainEvent<TArgs>, new() where TExecuteSubscription : Domain.IExecuteSubscription<TArgs> where TArgs : IEntityEventArgs
		{
			TExecuteSubscription subscription = default(TExecuteSubscription);
			try
			{
				subscription = Extensions.ExtensionServiceLocator.Current.GetInstance<TExecuteSubscription>(info.ExportId);
			}
			catch(Exception ex)
			{
				ex = new RegisterEntityEventException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityEventManager_GetEventTypeImplementationError, info.EventType.ToString(), entityType, info.ExportId), ex);
				OnRegisterEventInfoError(entityType, info, ex);
				return null;
			}
			var subscriptionInfo = new EventSubscriptionInfo<TArgs>(info, entityType, subscription);
			return Domain.DomainEvents.Instance.GetEvent<TEvent>().Subscribe(subscriptionInfo.Execute, Domain.DomainEventThreadOption.PublisherThread, true, subscriptionInfo.CanExecute, false, subscriptionInfo.ToString());
		}

		/// <summary>
		/// Recupera as informações do evento com base no nome do tipo
		/// da entidade de negócio.
		/// </summary>
		/// <param name="entityType">Nome do tipo da entidade de negócio.</param>
		/// <returns></returns>
		protected virtual IEnumerable<EntityEventInfo> GetEventInfo(Reflection.TypeName entityType)
		{
			return null;
		}

		/// <summary>
		/// Verifica se a o tipo da entidade já foi registrado.
		/// </summary>
		/// <param name="entityType">Nome do tipo da entidade de negócio.</param>
		/// <returns></returns>
		protected virtual bool IsRegistered(Reflection.TypeName entityType)
		{
			lock (_registeredTypes)
				return _registeredTypes.BinarySearch(entityType, Reflection.TypeName.TypeNameFullNameComparer.Instance) >= 0;
		}

		/// <summary>
		/// Define que a o tipo da entidade de negócio foi registrado.
		/// </summary>
		/// <param name="entityType">Nome do tipo da entidade de negócio.</param>
		protected virtual void Registered(Reflection.TypeName entityType)
		{
			lock (_registeredTypes)
			{
				var index = _registeredTypes.BinarySearch(entityType, Reflection.TypeName.TypeNameFullNameComparer.Instance);
				if(index < 0)
					_registeredTypes.Insert(~index, entityType);
			}
		}

		/// <summary>
		/// Registra o tipo da entidade que será geranciada.
		/// </summary>
		/// <param name="entityType"></param>
		public void Register(Type entityType)
		{
			entityType.Require("entityType").NotNull();
			IEnumerable<EntityEventInfo> infos = null;
			var typeName = new Reflection.TypeName(entityType.AssemblyQualifiedName);
			if(IsRegistered(typeName))
				return;
			try
			{
				infos = GetEventInfo(typeName);
			}
			catch(Exception ex)
			{
				throw new RegisterEntityEventException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityEventManager_GetEntityEventInfosError, typeName.FullName), ex);
			}
			Registered(typeName);
			if(infos != null)
			{
				foreach (var i in infos)
					Register(typeName, i);
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			foreach (var pair in _eventSubscribers)
			{
				lock (pair.Value)
				{
					foreach (var i in pair.Value)
					{
						switch(pair.Key)
						{
						case EntityEventType.Initialized:
							Domain.DomainEvents.Instance.GetEvent<EntityInitializedEvent>().Unsubscribe(i);
							break;
						case EntityEventType.PropertyChanging:
							Domain.DomainEvents.Instance.GetEvent<EntityPropertyChangingEvent>().Unsubscribe(i);
							break;
						case EntityEventType.PropertyChanged:
							Domain.DomainEvents.Instance.GetEvent<EntityPropertyChangedEvent>().Unsubscribe(i);
							break;
						case EntityEventType.Validating:
							Domain.DomainEvents.Instance.GetEvent<EntityValidatingEvent>().Unsubscribe(i);
							break;
						case EntityEventType.Validated:
							Domain.DomainEvents.Instance.GetEvent<EntityValidatedEvent>().Unsubscribe(i);
							break;
						case EntityEventType.Saving:
							Domain.DomainEvents.Instance.GetEvent<EntitySavingEvent>().Unsubscribe(i);
							break;
						case EntityEventType.Saved:
							Domain.DomainEvents.Instance.GetEvent<EntitySavedEvent>().Unsubscribe(i);
							break;
						case EntityEventType.Deleting:
							Domain.DomainEvents.Instance.GetEvent<EntityDeletingEvent>().Unsubscribe(i);
							break;
						case EntityEventType.Deleted:
							Domain.DomainEvents.Instance.GetEvent<EntityDeletedEvent>().Unsubscribe(i);
							break;
						}
					}
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

		class EventSubscriptionInfo<TPayLoad> where TPayLoad : IEntityEventArgs
		{
			/// <summary>
			/// Inscrição.
			/// </summary>
			public Domain.IExecuteSubscription<TPayLoad> Subscription
			{
				get;
				set;
			}

			/// <summary>
			/// Informações da entidade.
			/// </summary>
			public EntityEventInfo Info
			{
				get;
				set;
			}

			/// <summary>
			/// Nome completo do tipo da entidade do evento.
			/// </summary>
			public Reflection.TypeName EntityType
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="info">Informações do evento da entidade.</param>
			/// <param name="entityType">Nome completo do tipo da entidade.</param>
			/// <param name="subscription"></param>
			public EventSubscriptionInfo(EntityEventInfo info, Reflection.TypeName entityType, Domain.IExecuteSubscription<TPayLoad> subscription)
			{
				subscription.Require("subscription").NotNull();
				this.Info = info;
				this.EntityType = entityType;
				this.Subscription = subscription;
			}

			/// <summary>
			/// Executa a inscrição do evento.
			/// </summary>
			/// <param name="args"></param>
			public void Execute(TPayLoad args)
			{
				this.Subscription.Execute(args);
			}

			/// <summary>
			/// Verifica se pode executar a inscrição.
			/// </summary>
			/// <param name="args"></param>
			/// <returns></returns>
			public bool CanExecute(TPayLoad args)
			{
				if(args.Entity != null)
					return EntityType.FullName == args.Entity.GetType().FullName;
				return false;
			}

			/// <summary>
			/// Recupera o texto que representqa a instancia
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("[EntityType : {0} -- Subscription : {1}]", EntityType.FullName, this.Subscription);
			}
		}
	}
}
