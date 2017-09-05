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
	/// Assinatura do método usado para cria um entidade com base no modelo de dados.
	/// </summary>
	/// <param name="uiContext">Contexto de interface com o usuário.</param>
	/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
	/// <param name="dataModel">Modelo de dados que será usado com base.</param>
	/// <param name="sourceContext">Contexto da origem dos dados.</param>
	/// <returns></returns>
	public delegate IEntity EntityFromModelCreatorHandler (string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Data.IModel dataModel, Colosoft.Query.ISourceContext sourceContext);
	/// <summary>
	/// Estado dos dados de carga tardia.
	/// </summary>
	public class LazyDataState
	{
		/// <summary>
		/// Instancia da entidade associada.
		/// </summary>
		public IEntity Entity
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Assinatura de uma interface responsável por carregar os dados de uma entidade.
	/// </summary>
	public interface IEntityLoader
	{
		/// <summary>
		/// Nome da propriedade do identificador unico da entidade.
		/// </summary>
		string UidPropertyName
		{
			get;
		}

		/// <summary>
		/// Propriedades que compôem o nome único.
		/// </summary>
		string[] FindNameProperties
		{
			get;
		}

		/// <summary>
		/// Conversor que gera o nome unico da instância.
		/// </summary>
		IFindNameConverter FindNameConverter
		{
			get;
		}

		/// <summary>
		/// Nome da propriedade da descrição da entidade. 
		/// </summary>
		string DescriptionPropertyName
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade possui identificador unico.
		/// </summary>
		bool HasUid
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade possui nome unico.
		/// </summary>
		bool HasFindName
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade possui descrição.
		/// </summary>
		bool HasDescription
		{
			get;
		}

		/// <summary>
		/// Tipo do modelo de dados associado com a entidade.
		/// </summary>
		Type DataModelType
		{
			get;
		}

		/// <summary>
		/// Nome do tipo do modelo de dados associado com a entidade.
		/// </summary>
		Colosoft.Reflection.TypeName DataModelTypeName
		{
			get;
		}

		/// <summary>
		/// Nome s das propriedades chave.
		/// </summary>
		IEnumerable<string> KeysPropertyNames
		{
			get;
		}

		/// <summary>
		/// Descritor da chave de registro da entidade.
		/// </summary>
		Query.Record.RecordDescriptor KeyRecordDescriptor
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade associada possui chaves.
		/// </summary>
		bool HasKeys
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade associada possui 
		/// suporte para instancia interna.
		/// </summary>
		bool InnerInstanceSupport
		{
			get;
		}

		/// <summary>
		/// Identifica se a entidade possui filhos.
		/// </summary>
		bool HasChildren
		{
			get;
		}

		/// <summary>
		/// Parametros customizados associados ao Loader.
		/// </summary>
		System.Collections.Hashtable CustomParameters
		{
			get;
		}

		/// <summary>
		/// Tenta recupera a instancia do single child com base no nome da propriedade.
		/// </summary>
		/// <param name="entity">Instancia onde o filho está.</param>
		/// <param name="propertyName"></param>
		/// <param name="childName">Nome do filho.</param>
		/// <param name="child">Instancia do filho.</param>
		/// <returns></returns>
		bool TryGetSingleChildFromProperty(IEntity entity, string propertyName, out string childName, out IEntity child);

		/// <summary>
		/// Cria uma descritor de entidade.
		/// </summary>
		/// <returns>Nova instancia do descritor de entidade.</returns>
		IEntityDescriptor CreateEntityDescriptor();

		/// <summary>
		/// Cria um descritor com base na entidade informada.
		/// </summary>
		/// <param name="entity">Instancia da entidade com os valores que serão usados na criação do descritor.</param>
		/// <returns></returns>
		IEntityDescriptor CreateEntityDescriptor(IEntity entity);

		/// <summary>
		/// Recupera a instancia da estratégia de vinculação.
		/// </summary>
		/// <returns></returns>
		Colosoft.Query.IQueryResultBindStrategy GetBindStrategy();

		/// <summary>
		/// Recupera a instancia do responsável por criar os objetos da entidade.
		/// </summary>
		/// <returns></returns>
		Colosoft.Query.IQueryResultObjectCreator GetObjectCreator();

		/// <summary>
		/// Recupera a estratégia de vinculação para o EntityDescriptor.
		/// </summary>
		/// <returns></returns>
		Colosoft.Query.IQueryResultBindStrategy GetEntityDescriptorBindStrategy();

		/// <summary>
		/// Recupera a instancia do resposável por criar os descritores de entidade.
		/// </summary>
		/// <returns></returns>
		IEntityDescriptorCreator GetEntityDescriptorCreator();

		/// <summary>
		/// Recupera a instancia da factory da chave dos registros.
		/// </summary>
		/// <returns></returns>
		Query.IRecordKeyFactory GetRecordKeyFactory();

		/// <summary>
		/// Recupera o identificador unico associadom com a instancia do modelo informado.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		int GetInstanceUid(Colosoft.Data.IModel model);

		/// <summary>
		/// Define o identificador único para o modelo informado.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="uid"></param>
		void SetInstanceUid(Data.IModel model, int uid);

		/// <summary>
		/// Recupera o identificador unico da instancia da entidade informada.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int GetInstanceUid(IEntity entity);

		/// <summary>
		/// Define o identificador unico da instanciada entidade.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="uid"></param>
		void SetInstanceUid(IEntity entity, int uid);

		/// <summary>
		/// Recupera o nome unico da instancia da entidade informada.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		string GetInstanceFindName(IEntity entity);

		/// <summary>
		/// Recupera o nome unico da instancia da entidade a partir dos valores que compôem o nome.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		string GetInstanceFindName(object[] values);

		/// <summary>
		/// Notifica que o Uid a entidade foi alterado.
		/// </summary>
		/// <param name="entity"></param>
		void NotifyUidChanged(IEntity entity);

		/// <summary>
		/// Reseta todos os identificadores associados com a entidade e seus filhos.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="typeManager"></param>
		void ResetAllUids(IEntity entity, IEntityTypeManager typeManager);

		/// <summary>
		/// Recupera os valores das chaves da instancia.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		IEnumerable<Tuple<string, object>> GetInstanceKeysValues(IEntity entity);

		/// <summary>
		/// Realiza a carga completa da entidade.
		/// </summary>
		/// <param name="query">Instancia da consulta para recuperar a entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Gerenciador de tipos.</param>
		/// <returns></returns>
		IEntity FullLoad(Colosoft.Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Realiza a carga completa das entidades contidas nos registros informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		IEnumerable<IEntity> GetFullEntities(IEnumerable<Colosoft.Query.IRecord> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Recupera a entidade completas com base na consulta informada.
		/// </summary>
		/// <param name="queryable">Consulta que será realizado.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador dos tipos de entidades.</param>
		/// <returns></returns>
		IEnumerable<IEntity> GetFullEntities(Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Recupera as entidades completas com bas na preparação da consulta.
		/// </summary>
		/// <param name="prepareResult">Resultado da preparação da consulta.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		IEnumerable<IEntity> GetFullEntities(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext);

		/// <summary>
		/// Recupera as entidades completas com bas na preparação da consulta.
		/// </summary>
		/// <param name="prepareResult">Resultado da preparação da consulta.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="queryResult">Resultado da consulta.</param>
		/// <returns></returns>
		IEnumerable<IEntity> GetFullEntities(PrepareNestedQueriesResult prepareResult, Colosoft.Query.ISourceContext sourceContext, Query.IQueryResult queryResult);

		/// <summary>
		/// Recupera os descritores das entidades associadas com o consulta informada.
		/// </summary>
		/// <param name="queryable">Consulta que será realizada.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns></returns>
		IEnumerable<T> GetEntityDescriptors<T>(Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, string uiContext) where T : IEntityDescriptor;

		/// <summary>
		/// Recupera os descritores das entidades associados com os registros informados.
		/// </summary>
		/// <param name="records">Relação dos registros com os dados que serão processados.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns></returns>
		IEnumerable<IEntityDescriptor> GetEntityDescriptors(IEnumerable<Colosoft.Query.IRecord> records, Colosoft.Query.ISourceContext sourceContext, string uiContext);

		/// <summary>
		/// Cria as consultas aninhadas para recupera as referencia da entidade pai.
		/// </summary>
		/// <param name="queryable">Consulta usada para recuperar a entidade.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="parentResult">Resultados dos itens pai.</param>
		/// <param name="exceptions">Fila dos erros ocorridos.</param>
		void CreateNestedQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager, IList<EntityLoaderCreatorArgs> parentResult, Queue<Exception> exceptions);

		/// <summary>
		/// Prepara as consultas aninhadas para a consulta informada.
		/// </summary>
		/// <param name="queryable">Consulta que será usada para recuperar a entidade.</param>
		/// <param name="uiContext">Contexto visual;</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns>Dados da preparação.</returns>
		PrepareNestedQueriesResult PrepareNestedQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Recupera os dados de carga tardia .
		/// </summary>
		/// <param name="creatorArgs">Argumentos para a criação da entidade.</param>
		/// <param name="state">Estado dos dados de carga tardia.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador dos tipos de entidades.</param>
		/// <param name="exceptions">Fila dos erros ocorridos.</param>
		void GetLazyData(EntityLoaderCreatorArgs creatorArgs, LazyDataState state, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions);

		/// <summary>
		/// Realiza a carga completa da entidade.
		/// </summary>
		/// <param name="record">Registro dos dados da entidade.</param>
		/// <param name="recordKey">Chave que representa o registro.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Gerenciador de tipos.</param>
		/// <returns></returns>
		IEntity FullLoad(Colosoft.Query.IRecord record, Colosoft.Query.RecordKey recordKey, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Realiza a carga completa da entidade.
		/// </summary>
		/// <param name="record">Registro dos dados da entidade.</param>
		/// <param name="recordKey">Chave que representa o registro.</param>
		/// <param name="bindStrategySession">Estratégia de vinculação dos dados do resultado.</param>
		/// <param name="objectCreator">Instancia responsável pela criação de objetos.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Gerenciador de tipos.</param>
		/// <returns></returns>
		IEntity FullLoad(Colosoft.Query.IRecord record, Colosoft.Query.RecordKey recordKey, Colosoft.Query.IQueryResultBindStrategySession bindStrategySession, Colosoft.Query.IQueryResultObjectCreator objectCreator, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Realiza a carga da entidade com carga tardia dos dados filhos.
		/// </summary>
		/// <param name="query">Instancia da consulta para recuperar a entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		IEntity LazyLoad(Colosoft.Query.Queryable query, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null);

		/// <summary>
		/// Realiza a carga da entidade com carga tardia dos dados filhos.
		/// </summary>
		/// <param name="record">Registro dos dados da entidade.</param>
		/// <param name="recordKey">Chave do registro.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <returns></returns>
		IEntity LazyLoad(Colosoft.Query.IRecord record, Query.RecordKey recordKey, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null);

		/// <summary>
		/// Realiza a carga da entidade com carga tardia dos dados filhos.
		/// </summary>
		/// <param name="record">Registro dos dados da entidade.</param>
		/// <param name="recordKey">Chave do registro.</param>
		/// <param name="bindStrategy">Estratégia de vinculação dos dados do resultado.</param>
		/// <param name="objectCreator">Instancia responsável pela criação de objetos.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <returns></returns>
		IEntity LazyLoad(Colosoft.Query.IRecord record, Query.RecordKey recordKey, Colosoft.Query.IQueryResultBindStrategy bindStrategy, Colosoft.Query.IQueryResultObjectCreator objectCreator, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null);

		/// <summary>
		/// Realiza a carga completa das entidades contidas nos registros informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args"></param>
		/// <returns></returns>
		IEnumerable<IEntity> GetLazyEntities(IEnumerable<Colosoft.Query.IRecord> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null);

		/// <summary>
		/// Clona os dados da entidade informada.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		IEntity Clone(IEntity entity);

		/// <summary>
		/// Copia os dados da entidade de origem para entidade de destino.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		void Copy(IEntity source, IEntity destination);

		/// <summary>
		/// Recupera o registro que representa a chave da entidade.
		/// </summary>
		/// <param name="propertyGetter">Getter para recuperar o valor da propriedade.</param>
		/// <returns></returns>
		Colosoft.Query.IRecord GetRecordOfKey(Func<string, object> propertyGetter);

		/// <summary>
		/// Recupera a chave de registro associada com a entidade.
		/// </summary>
		/// <param name="propertyGetter">Getter para recuperar o valor da propriedade</param>
		/// <returns></returns>
		Colosoft.Query.RecordKey GetRecordKey(Func<string, object> propertyGetter);

		/// <summary>
		/// Recupera a chave de registro associada com a entidade.
		/// </summary>
		/// <param name="entity">Instancia da entidade de onde os dados serão recuperados.</param>
		/// <returns></returns>
		Colosoft.Query.RecordKey GetRecordKey(IEntity entity);

		/// <summary>
		/// Recupera os getters dos filhos da entidade.
		/// </summary>
		/// <param name="savePriority">Prioridade dos filhos que serão recuperados</param>
		/// <returns></returns>
		IEnumerable<IEntityAccessor> GetChildrenAccessors(EntityChildSavePriority? savePriority = null);

		/// <summary>
		/// Recupera os getters dos links da entidade.
		/// </summary>
		/// <param name="savePriority">Prioridade dos filhos que serão recuperados</param>
		/// <returns></returns>
		IEnumerable<IEntityAccessor> GetLinksAccessors(EntityChildSavePriority? savePriority = null);

		/// <summary>
		/// Cria uma instancia de uma filho
		/// </summary>
		/// <typeparam name="TChild">Tipo da entidade filho.</typeparam>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="name">Nome do filho.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos das entidades.</param>
		/// <param name="sourceContext">Contexto da origem dos dados.</param>
		/// <returns></returns>
		TChild CreateChild<TChild>(IEntity parent, string name, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext) where TChild : IEntity;

		/// <summary>
		/// Registra o filho da entidade.
		/// </summary>
		/// <param name="parent">Instancia do pai.</param>
		/// <param name="child">Instancia do filho.</param>
		/// <param name="childName">Nome do filho.</param>
		void RegisterChild(IEntity parent, IEntity child, string childName);

		/// <summary>
		/// Cria uma instancia de um link.
		/// </summary>
		/// <typeparam name="TLink">Tipo da entidade do link.</typeparam>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="name">Nome do link.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos das entidades.</param>
		/// <param name="sourceContext">Contexto da origem dos dados.</param>
		/// <returns></returns>
		TLink CreateLink<TLink>(IEntity parent, string name, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext) where TLink : IEntity;

		/// <summary>
		/// Cria uma nova instancia da entidade associada.
		/// </summary>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		IEntity Create(string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext);

		/// <summary>
		/// Cria uma nova instancia da entidade associada.
		/// </summary>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
		/// <param name="dataModel">Modelo de dados que será usado com base.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		IEntity Create(string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Data.IModel dataModel, Colosoft.Query.ISourceContext sourceContext);

		/// <summary>
		/// Cria uma nova instancia da entidade associada.
		/// </summary>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Gerenciador do tipo da entidade.</param>
		/// <param name="creatorArgs">Argumentos que serão usadaos na crição da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <returns></returns>
		IEntity Create(string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderCreatorArgs creatorArgs, Colosoft.Query.ISourceContext sourceContext);

		/// <summary>
		/// Cria uma instancia de uma referencia.
		/// </summary>
		/// <typeparam name="TReference">Tipo da referencia.</typeparam>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="name">Nome da referencia.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos das entidades.</param>
		/// <param name="sourceContext">Origem do contexto dos dados.</param>
		/// <returns></returns>
		TReference CreateReference<TReference>(IEntity parent, string name, string uiContext, IEntityTypeManager entityTypeManager, Query.ISourceContext sourceContext) where TReference : IEntity;

		/// <summary>
		/// Recupera a referencia da entidade.
		/// </summary>
		/// <param name="parent">Instancia da entidade pai.</param>
		/// <param name="referenceName">Nome da referencia.</param>
		/// <param name="uiContext">Contexto da interface com o usuário.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador dos tipos das entidade.s</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="isLazy">Identifica se é para fazer a carga em modo Lazy.</param>
		/// <returns></returns>
		IEntity GetEntityReference(IEntity parent, string referenceName, string uiContext, IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext, bool isLazy);

		/// <summary>
		/// Recupera as referencias da entidade.
		/// </summary>
		/// <returns></returns>
		IEnumerable<EntityLoaderReference> GetReferences();

		/// <summary>
		/// Tenta recupera a referencia pelo nome.
		/// </summary>
		/// <param name="referenceName"></param>
		/// <param name="reference"></param>
		/// <returns></returns>
		bool TryGetReference(string referenceName, out EntityLoaderReference reference);

		/// <summary>
		/// Tenta recupera as referencias pelo nome da propriedade monitorada.
		/// </summary>
		/// <param name="propertyNames">Nome da propriedade monitorada.</param>
		/// <returns></returns>
		IEnumerable<EntityLoaderReference> GetReferenceByWatchedProperties(string[] propertyNames);
	}
}
