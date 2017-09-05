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
using Colosoft.Lock;
using System.Collections;

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura das classe que fazema limpeza das alterações.
	/// </summary>
	public interface IClearableChangedTracking : System.ComponentModel.IChangeTracking
	{
		/// <summary>
		/// Limpa as alteração.es
		/// </summary>
		void ClearChanges();

		/// <summary>
		/// Ignora as alterações das propriedades informadas.
		/// </summary>
		/// <param name="propertyNames"></param>
		void IgnoreChanges(params string[] propertyNames);
	}
	/// <summary>
	/// Contrato para os objetos do sistema
	/// </summary>
	public interface IEntity : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged, Colosoft.ILoadable, Validation.IStateControl, ICloneable, IEquatable<IEntity>, System.ComponentModel.IChangeTracking, System.ComponentModel.IRevertibleChangeTracking, IClearableChangedTracking, System.ComponentModel.IEditableObject, System.ComponentModel.ISupportInitialize, System.ComponentModel.IRaiseItemChangedEvents, IDisposable, Colosoft.Lock.ILockable, INamedType, Colosoft.Query.IRecordKeyEquatable
	{
		/// <summary>
		/// Evento chamado ao terminar atualizações.
		/// </summary>
		event EventHandler AcceptedChanges;

		/// <summary>
		/// Evento acionado quando a entidade sofrer alguma alteração.
		/// </summary>
		event EventHandler Changed;

		/// <summary>
		/// Dono da lista.
		/// </summary>
		IEntity Owner
		{
			get;
			set;
		}

		/// <summary>
		/// Gerenciador dos tipos associado com a instncia.
		/// </summary>
		IEntityTypeManager TypeManager
		{
			get;
		}

		/// <summary>
		/// Loader da instancia.
		/// </summary>
		IEntityLoader Loader
		{
			get;
		}

		/// <summary>
		/// Contexto de interface com o usuário
		/// </summary>
		string UIContext
		{
			get;
		}

		/// <summary>
		/// Objeto que controla os objetos que observam as alterações
		/// </summary>
		ObserverControl Observer
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador unico da entidade.
		/// </summary>
		new int Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Nome que identifica a instância
		/// </summary>
		string FindName
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade está sendo editada.
		/// </summary>
		bool IsEditing
		{
			get;
		}

		/// <summary>
		/// Identifica se a instancia é para somente leitura.
		/// </summary>
		bool IsReadOnly
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a entidade está sendo inicializada.
		/// </summary>
		bool IsInitializing
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade possui um identificador unico.
		/// </summary>
		bool HasUid
		{
			get;
		}

		/// <summary>
		/// Identifica se a instância possui um nome unico.
		/// </summary>
		bool HasFindName
		{
			get;
		}

		/// <summary>
		/// Instancia da lista associada.
		/// </summary>
		Colosoft.Collections.IObservableCollection MyList
		{
			get;
		}

		/// <summary>
		/// Identifica se a instancia já existe na fonte de armazenamento.
		/// </summary>
		bool ExistsInStorage
		{
			get;
		}

		/// <summary>
		/// Determina se a instancia da entidade pode ser editada.
		/// </summary>
		bool CanEdit
		{
			get;
		}

		/// <summary>
		/// Instancia original do objeto.
		/// </summary>
		IEntity Instance
		{
			get;
		}

		/// <summary>
		/// Identificador se a instancia original do objeto foi inicializada.
		/// </summary>
		bool IsInstanceInicialized
		{
			get;
		}

		/// <summary>
		/// Indica se a instância está locada para a pessoa.
		/// </summary>
		bool IsLockedToMe
		{
			get;
		}

		/// <summary>
		/// Veifica se a instância está ou não locada.
		/// </summary>
		bool IsLocked
		{
			get;
		}

		/// <summary>
		/// Instância a partir da qual q instância atual foi clonada.
		/// </summary>
		IEntity CloneFrom
		{
			get;
		}

		/// <summary>
		/// Recupera o nome do tipo da entidade.
		/// </summary>
		IMessageFormattable EntityTypeName
		{
			get;
		}

		/// <summary>
		/// Cria um <see cref="IEntityDescriptor"/> da entidade.
		/// </summary>
		/// <returns></returns>
		IEntityDescriptor CreateDescriptor();

		/// <summary>
		/// Cria uma instância com a cópia da instância original para edição. 
		/// </summary>
		/// <returns></returns>
		IEntity CloneToEdit();

		/// <summary>
		/// Registra na entidade que ela foi clona para edição a partir da instancia informada.
		/// </summary>
		/// <param name="cloneFrom">Instancia de origem da entidade.</param>
		void RegisterCloneToEdit(IEntity cloneFrom);

		/// <summary>
		/// Notifica todos os observadores das mudanças ocorridas.
		/// </summary>
		void Flush();

		/// <summary>
		/// Recupera a instancia com os dados originais da entidade.
		/// </summary>
		/// <returns></returns>
		IEntity GetOriginal();

		/// <summary>
		/// Copia os dados da entidade informada para a instancia,
		/// inclusives os dados de alteração da nova instancia..
		/// </summary>
		/// <param name="fromEntity">Instancia com os dados que serão copiados.</param>
		void CopyFrom(IEntity fromEntity);

		/// <summary>
		/// Salva os dados da entidade.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		/// <returns>Retorna true se a operação foi executada com sucesso.</returns>
		SaveResult Save(Data.IPersistenceSession session);

		/// <summary>
		/// Salva os dados da entidade.
		/// </summary>
		SaveResult Save();

		/// <summary>
		/// Registra a entidade para ser apagada.
		/// </summary>
		/// <param name="session">Sessão onde a operação será realizada.</param>
		/// <returns></returns>
		DeleteResult Delete(Data.IPersistenceSession session);

		/// <summary>
		/// Valida os dados da instancia.
		/// </summary>
		/// <returns></returns>
		Validation.ValidationResult Validate();

		/// <summary>
		/// Valida os dados da intancia.
		/// </summary>
		/// <param name="validationResult">Resultado da validação.</param>
		void Validate(ref Validation.ValidationResult validationResult);

		/// <summary>
		/// Valida as propriedades informadas.
		/// </summary>
		/// <param name="propertyNames">Nomes das propriedades que serão validadas.</param>
		/// <returns></returns>
		Validation.ValidationResult Validate(params string[] propertyNames);

		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <param name="token">token que irá bloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		LockProcessResult Lock(string token, string hostName, LockType lockType, bool mainInLock);

		/// <summary>
		/// Bloqueia a inatância.
		/// </summary>
		/// <returns></returns>
		LockProcessResult Lock();

		/// <summary>
		/// Realiza o lock da instancia.
		/// </summary>
		/// <param name="session">Instancia da sessão que deverá ser utilizada para o lock.</param>
		/// <returns></returns>
		LockProcessResult Lock(LockSession session);

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="groupSession">Sessão do grupo.</param>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		LockProcessResult UnLock(string groupSession, string token, string hostName, LockType lockType, bool mainInLock);

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="token">token que irá desbloquear</param>
		/// <param name="hostName">Nome do host</param>
		/// <param name="lockType">tipo do bloqueio</param>
		/// <param name="mainInLock">Indica que é a instância principal do lock por exemplo se for um bloqueio de venda não faz sentido eu bloquear para uso o contato do cliente, mas quando o bloqueio é sobre o cliente isso já faz sentido</param>
		/// <returns></returns>
		LockProcessResult UnLock(string token, string hostName, LockType lockType, bool mainInLock);

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <param name="groupSession">Sessão do grupo.</param>
		/// <returns></returns>
		LockProcessResult UnLock(string groupSession);

		/// <summary>
		/// Desbloqueia a inatância.
		/// </summary>
		/// <returns></returns>
		LockProcessResult UnLock();

		/// <summary>
		/// Inicializa a lista.
		/// </summary>
		/// <param name="ownerList">Lista mãe da entidade</param>
		void InitList(Colosoft.Collections.IObservableCollection ownerList);

		/// <summary>
		/// Remove o controle de armazenamento da entidade.
		/// </summary>
		void RemoveStorageControl();

		/// <summary>
		/// Remove o control de armazenamento da entidade e de seus filhos.
		/// </summary>
		void RemoveAllStorageControl();

		/// <summary>
		/// Reseta todos os Uids da instancia e das instancias filhas.
		/// </summary>
		void ResetAllUids();

		/// <summary>
		/// Recupera o manipulador que cancela a velidação de somente leitura
		/// na alteração de propriedades.
		/// </summary>
		/// <returns></returns>
		IDisposable CreateReadOnlyPropertyChangingCancelHandler();
	}
	/// <summary>
	/// Assinatura da entidade de um modelo de dados.
	/// </summary>
	public interface IEntityOfModel : IEntity
	{
		/// <summary>
		/// Instancia do modelo de dados associado.
		/// </summary>
		Colosoft.Data.IModel DataModel
		{
			get;
		}
	}
	/// <summary>
	/// Assinatura das classes de entidade que estão associadas diretamente com um modelo de dados.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public interface IEntity<Model> : IEntityOfModel, IEquatable<IEntity<Model>> where Model : class, Colosoft.Data.IModel
	{
		/// <summary>
		/// Instancia do modelo de dados associado.
		/// </summary>
		new Model DataModel
		{
			get;
		}

		/// <summary>
		/// Altera o valor da instancia do modelo dados.
		/// </summary>
		/// <param name="dataModel"></param>
		void CopyFromDataModel(Model dataModel);
	}
}
