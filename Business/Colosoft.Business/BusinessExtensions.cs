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

namespace Colosoft
{
	/// <summary>
	/// Classe com os métodos de extenção de negócio.
	/// </summary>
	public static class BusinessExtentions
	{
		/// <summary>
		/// Cria uma entidade do tipo TEntity e a conecta ao contexto da origem dos dados.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="sourceContext"></param>
		/// <returns></returns>
		public static TEntity Create<TEntity>(this Colosoft.Query.ISourceContext sourceContext) where TEntity : Colosoft.Business.IEntity, new()
		{
			try
			{
				var entity = new TEntity();
				if(entity is Colosoft.Business.IConnectedEntity)
					((Colosoft.Business.IConnectedEntity)entity).Connect(sourceContext);
				return entity;
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// Recupera o pai do tipo informado.
		/// </summary>
		/// <typeparam name="TOwner"></typeparam>
		/// <returns></returns>
		public static TOwner GetOwner<TOwner>(this Colosoft.Business.IEntity entity) where TOwner : Colosoft.Business.IEntity
		{
			if(entity == null)
				return default(TOwner);
			var myList = entity.MyList;
			if(myList is Colosoft.Business.IEntity)
				return ((Colosoft.Business.IEntity)myList).GetOwner<TOwner>();
			if(entity.Owner == null)
				return default(TOwner);
			if(entity.Owner is TOwner)
				return (TOwner)entity.Owner;
			return GetOwner<TOwner>(entity.Owner);
		}

		/// <summary>
		/// Recupera um resultado de exclusão para o resultado das ações.
		/// </summary>
		/// <param name="actionsResult"></param>
		/// <returns></returns>
		public static Colosoft.Business.DeleteResult ToDeleteResult(this Colosoft.Data.ExecuteActionsResult actionsResult)
		{
			return new Colosoft.Business.DeleteResult(actionsResult.Status == Colosoft.Data.ExecuteActionsResultStatus.Success, actionsResult.FailureMessage.GetFormatter());
		}

		/// <summary>
		/// Recupera um resultado de save para o resultado das ações.
		/// </summary>
		/// <param name="actionsResult"></param>
		/// <returns></returns>
		public static Colosoft.Business.SaveResult ToSaveResult(this Colosoft.Data.ExecuteActionsResult actionsResult)
		{
			return new Colosoft.Business.SaveResult(actionsResult.Status == Colosoft.Data.ExecuteActionsResultStatus.Success, actionsResult.FailureMessage.GetFormatter());
		}

		/// <summary>
		/// Executa a sessão de persistencia verificando se o resultado for válido.
		/// </summary>
		/// <param name="saveResult"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public static Colosoft.Business.SaveResult Execute(this Colosoft.Business.SaveResult saveResult, Colosoft.Data.IPersistenceSession session)
		{
			saveResult.Require("saveResult").NotNull();
			session.Require("session").NotNull();
			if(!saveResult)
				return saveResult;
			return session.Execute(false).ToSaveResult();
		}

		/// <summary>
		/// Executa a sessão de persistencia verificando se o resultado for válido.
		/// </summary>
		/// <param name="deleteResult"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public static Colosoft.Business.DeleteResult Execute(this Colosoft.Business.DeleteResult deleteResult, Colosoft.Data.IPersistenceSession session)
		{
			deleteResult.Require("deleteResult").NotNull();
			session.Require("session").NotNull();
			if(!deleteResult)
				return deleteResult;
			return session.Execute(false).ToDeleteResult();
		}

		/// <summary>
		/// Recupera o nome do tipo do import associado com o tipo de evento.
		/// </summary>
		/// <param name="eventType">Tipo do evento.</param>
		/// <returns></returns>
		public static Colosoft.Reflection.TypeName GetImportTypeName(this Colosoft.Business.EntityEventType eventType)
		{
			switch(eventType)
			{
			case Colosoft.Business.EntityEventType.PropertyChanging:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityPropertyChangingEventSubscription>();
			case Colosoft.Business.EntityEventType.PropertyChanged:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityPropertyChangedEventSubscription>();
			case Colosoft.Business.EntityEventType.Initialized:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityInitializedEventSubscription>();
			case Colosoft.Business.EntityEventType.Validating:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityValidatingEventSubscription>();
			case Colosoft.Business.EntityEventType.Validated:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityValidatedEventSubscription>();
			case Colosoft.Business.EntityEventType.Saving:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntitySavingEventSubscription>();
			case Colosoft.Business.EntityEventType.Saved:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntitySavedEventSubscription>();
			case Colosoft.Business.EntityEventType.Deleting:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityDeletingEventSubscription>();
			case Colosoft.Business.EntityEventType.Deleted:
				return Colosoft.Reflection.TypeName.Get<Colosoft.Business.IEntityDeletedEventSubscription>();
			default:
				throw new InvalidCastException();
			}
		}

		/// <summary>
		/// Despacha as notificações associadas com a instancia.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="level">Nível no qual as mensagens serão despachadas.</param>
		public static void DispatchNotifications(this Colosoft.Business.INotificationContainer container, Colosoft.Business.DispatchNotificationLevel level)
		{
			if(container != null)
				container.DispatchNotifications(Colosoft.Notifications.Notification.Dispatcher, level);
		}

		/// <summary>
		/// Lança uma exception caso o resultado não seja satisfatório.
		/// </summary>
		/// <param name="result"></param>
		public static void ThrowInvalid(this Business.SaveResult result)
		{
			if(result != null && !result)
				throw new Business.SaveResultException(result);
		}

		/// <summary>
		/// Lança uma exception caso o resultado não seja satisfatório.
		/// </summary>
		/// <param name="result"></param>
		public static void ThrowInvalid(this Business.DeleteResult result)
		{
			if(result != null && !result)
				throw new Business.DeleteResultException(result);
		}

		/// <summary>
		/// Lança uma exception caso o resultado não seja satisfatório.
		/// </summary>
		/// <param name="result"></param>
		public static void ThrowInvalid(this Business.OperationResult result)
		{
			if(result != null && !result)
				throw new Business.OperationResultException(result);
		}

		/// <summary>
		/// Salva os dados da entidade na linha.
		/// </summary>
		/// <remarks>Essa operação abre uma sessão de persistencia e executa o save da entidade.</remarks>
		/// <typeparam name="T">Tipo da entidade que será salva.</typeparam>
		/// <param name="context"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static Business.SaveResult ExecuteSave<T>(this Data.IPersistenceContext context, T entity) where T : Business.IEntity
		{
			using (var session = context.CreateSession())
			{
				var resultado = entity.Save(session);
				if(!resultado)
					return resultado;
				return session.Execute(false).ToSaveResult();
			}
		}

		/// <summary>
		/// Executa a sessão na linha.
		/// </summary>
		/// <remarks>Essa operação abre uma sessão de persistencia e executa o save da entidade.</remarks>
		/// <param name="context"></param>
		/// <param name="items">Referências dos métodos save que deverão ser acionados.</param>
		/// <returns></returns>
		public static Business.SaveResult ExecuteSave(this Data.IPersistenceContext context, params Func<Data.IPersistenceSession, Business.SaveResult>[] items)
		{
			using (var session = context.CreateSession())
			{
				var resultado = session.Save(items);
				if(!resultado)
					return resultado;
				return session.Execute(false).ToSaveResult();
			}
		}

		/// <summary>
		/// Apaga os dados da entidade na linha.
		/// </summary>
		/// <remarks>Essa operação abre uma sessão de persistencia e executa o delete da entidade.</remarks>
		/// <typeparam name="T">Tipo da entidade que será salva.</typeparam>
		/// <param name="context"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static Business.DeleteResult ExecuteDelete<T>(this Data.IPersistenceContext context, T entity) where T : Business.IEntity
		{
			using (var session = context.CreateSession())
			{
				var resultado = entity.Delete(session);
				if(!resultado)
					return resultado;
				return session.Execute(false).ToDeleteResult();
			}
		}

		/// <summary>
		/// Executa a exclusão na linha
		/// </summary>
		/// <remarks>Essa operação abre uma sessão de persistencia e executa o save da entidade.</remarks>
		/// <param name="context"></param>
		/// <param name="items">Referências dos métodos save que deverão ser acionados.</param>
		/// <returns></returns>
		public static Business.DeleteResult ExecuteDelete(this Data.IPersistenceContext context, params Func<Data.IPersistenceSession, Business.DeleteResult>[] items)
		{
			using (var session = context.CreateSession())
			{
				var resultado = session.DeleteItems(items);
				if(!resultado)
					return resultado;
				return session.Execute(false).ToDeleteResult();
			}
		}

		/// <summary>
		/// Recupera o método para salvar a entidade. Caso a entidade seja nula retornar um método válido.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static Func<Data.IPersistenceSession, Business.SaveResult> GetSaveMethod(this Business.IEntity entity)
		{
			if(entity != null)
				return entity.Save;
			return session => new Colosoft.Business.SaveResult(true, null);
		}

		/// <summary>
		/// Recupera o método para apagar a entidade. Caso a entidade seja nula retornar um método válido.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static Func<Data.IPersistenceSession, Business.DeleteResult> GetDeleteMethod(this Business.IEntity entity)
		{
			if(entity != null)
				return entity.Delete;
			return session => new Colosoft.Business.DeleteResult(true, null);
		}

		/// <summary>
		/// Executa o save dos itens informados.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="items">Referências dos métodos save que deverão ser acionados.</param>
		/// <returns></returns>
		public static Business.SaveResult Save(this Data.IPersistenceSession session, params Func<Data.IPersistenceSession, Business.SaveResult>[] items)
		{
			items.Require("items").NotNull();
			Business.SaveResult result = null;
			foreach (var i in items)
				if(i != null && !(result = i(session)))
					return result;
			return new Business.SaveResult(true, null);
		}

		/// <summary>
		/// Executa o delete dos itens informados.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="items">Referências dos métodos save que deverão ser acionados.</param>
		/// <returns></returns>
		public static Business.DeleteResult DeleteItems(this Data.IPersistenceSession session, params Func<Data.IPersistenceSession, Business.DeleteResult>[] items)
		{
			items.Require("items").NotNull();
			Business.DeleteResult result = null;
			foreach (var i in items)
				if(i != null && !(result = i(session)))
					return result;
			return new Business.DeleteResult(true, null);
		}
	}
}
