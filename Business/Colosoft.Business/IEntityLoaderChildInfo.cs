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

namespace Colosoft.Business
{
	/// <summary>
	/// Armazena os argumentos do evento acionado quando a consulta de um filho for executada.
	/// </summary>
	class EntityLoaderExecuteQueryEventArgs : EventArgs
	{
		/// <summary>
		/// Tipo da entidade associada com o resultado.
		/// </summary>
		public Type EntityType
		{
			get;
			private set;
		}

		/// <summary>
		/// Tipo do modelo de dados associado com o resultado.
		/// </summary>
		public Type DataModelType
		{
			get;
			private set;
		}

		/// <summary>
		/// Valores dos parametros de referencia.
		/// </summary>
		public Query.ReferenceParameterValueCollection ReferenceParameterValues
		{
			get;
			private set;
		}

		/// <summary>
		/// Resultado
		/// </summary>
		public IEnumerable<IEntity> Result
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataModelType">Tipo do modelo de dados.</param>
		/// <param name="entityType">Tipo da entidade do resultado.</param>
		/// <param name="referenceParameterValues">Valores dos parametros de referencia.</param>
		/// <param name="result"></param>
		public EntityLoaderExecuteQueryEventArgs(Type dataModelType, Type entityType, Query.ReferenceParameterValueCollection referenceParameterValues, IEnumerable<IEntity> result)
		{
			this.DataModelType = dataModelType;
			this.EntityType = entityType;
			this.ReferenceParameterValues = referenceParameterValues;
			this.Result = result;
		}
	}
	/// <summary>
	/// Representa o método acionado quando a consulta do filho for executada.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	delegate void EntityLoaderExecuteQueryHandler (object sender, EntityLoaderExecuteQueryEventArgs e);
	/// <summary>
	/// Assinatura das informações de um filho.
	/// </summary>
	interface IEntityLoaderChildInfo : IEntityAccessor
	{
		/// <summary>
		/// Nome da propriedade estrangueira da entidade na qual o filho está associado.
		/// </summary>
		string ForeignPropertyName
		{
			get;
		}

		/// <summary>
		/// Nome da propriedade onde a instancia do filho está armazenada.
		/// </summary>
		string PropertyName
		{
			get;
		}

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		Type DataModelType
		{
			get;
		}

		/// <summary>
		/// Tipo da entidade.
		/// </summary>
		Type EntityType
		{
			get;
		}

		/// <summary>
		/// Identifica se é apena um filho.
		/// </summary>
		bool IsSingle
		{
			get;
		}

		/// <summary>
		/// Opções de carga.
		/// </summary>
		LoadOptions Options
		{
			get;
		}

		/// <summary>
		/// Prioridade para salvar o filho.
		/// </summary>
		EntityChildSavePriority SavePriority
		{
			get;
		}

		/// <summary>
		/// Instancia responsável por define para o filho o identificador do pai.
		/// </summary>
		Action<IEntity, IEntity> ParentUidSetter
		{
			get;
		}

		/// <summary>
		/// Instancia responsável por recupera o valor pai.
		/// </summary>
		Func<IEntity, IEntity> ParentValueGetter
		{
			get;
		}

		/// <summary>
		/// Delegate usado para recuperar o valor da instancia do pai.
		/// </summary>
		Action<IEntity, IEntity> ParentValueSetter
		{
			get;
		}

		/// <summary>
		/// Delegate usado para recuperar a chave unica do pai.
		/// </summary>
		Func<Colosoft.Data.IModel, int> ParentUidGetter
		{
			get;
		}

		/// <summary>
		/// Func usado para carregar a condicional de carga.
		/// </summary>
		ConditionalLoader Conditional
		{
			get;
		}

		/// <summary>
		/// Cria as consultas para recuperar os itens filhos.
		/// </summary>
		/// <param name="parentUid">Identificador da entidade pai.</param>
		/// <param name="parentDataModel">Instancia com os dados do pai.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <returns></returns>
		EntityInfoQuery[] CreateQueries(int parentUid, Colosoft.Data.IModel parentDataModel, Colosoft.Query.ISourceContext sourceContext);

		/// <summary>
		/// Cria as consulta para recupera os itens filhos.
		/// </summary>
		/// <param name="queryable">Consulta do item pai.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="exceptions">Fila do erros ocorridos.</param>
		/// <param name="entityTypeManager">Instancia do gereciador de tipos da entidade de negócio.</param>
		/// <param name="callBack"></param>
		/// <param name="failedCallBack"></param>
		/// <returns></returns>
		void CreateQueries(Colosoft.Query.Queryable queryable, string uiContext, IEntityTypeManager entityTypeManager, Queue<Exception> exceptions, EntityLoaderExecuteQueryHandler callBack, Colosoft.Query.SubQueryFailedCallBack failedCallBack);

		/// <summary>
		/// Avalia se o registro contém dados associados
		/// com a lista dos filhos..
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parent">Instancia do pai.</param>
		/// <returns></returns>
		bool Evaluate(Query.IRecord record, IEntity parent);
	}
}
