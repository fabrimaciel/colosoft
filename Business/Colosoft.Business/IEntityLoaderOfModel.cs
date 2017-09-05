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
	/// Representa o associação entre um modelo de dados e a chave do registro.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public class DataModelRecordKey<Model> where Model : class, Data.IModel
	{
		private Model _dataModel;

		private Query.RecordKey _key;

		/// <summary>
		/// Instancia do modelo de dados.
		/// </summary>
		public Model DataModel
		{
			get
			{
				return _dataModel;
			}
		}

		/// <summary>
		/// Chave que representa o registro.
		/// </summary>
		public Query.RecordKey Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataModel">Instancia do modelo de dados.</param>
		/// <param name="key">Chave do registro.</param>
		public DataModelRecordKey(Model dataModel, Query.RecordKey key)
		{
			_dataModel = dataModel;
			_key = key;
		}
	}
	/// <summary>
	/// Assinatura de uma interface responsável por carregar os dados de uma entidade.
	/// </summary>
	public interface IEntityLoader<Model> : IEntityLoader where Model : class, Data.IModel
	{
		/// <summary>
		/// Realiza a carga completa da entidade.
		/// </summary>
		/// <param name="dataModelRecordKey">Instancia com o modelo de dados da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		IEntity<Model> FullLoad(DataModelRecordKey<Model> dataModelRecordKey, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Realiza a carga completa das entidades contidas nos dados informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		IEnumerable<IEntity<Model>> GetFullEntities(IEnumerable<DataModelRecordKey<Model>> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Realiza a carga completa em modo lazy das entidades contidas nos dados informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		IEnumerable<IEntity<Model>> GetLazyFullEntities(IEnumerable<DataModelRecordKey<Model>> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager);

		/// <summary>
		/// Realiza a carga da entidade com carga tardia dos dados filhos.
		/// </summary>
		/// <param name="dataModelRecordKey">Instancia do modelo de dados da entidade.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Contexto visual.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <returns></returns>
		IEntity<Model> LazyLoad(DataModelRecordKey<Model> dataModelRecordKey, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null);

		/// <summary>
		/// Realiza a carga tardia dos dados filhos da entidades contidas nos dados informados.
		/// </summary>
		/// <param name="result">Registros com os dados da entidades que serão carregadas.</param>
		/// <param name="sourceContext">Contexto de origem dos dados.</param>
		/// <param name="uiContext">Converto visual.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador de tipos de entidades.</param>
		/// <param name="args">Argumentos que serão usados</param>
		/// <returns></returns>
		IEnumerable<IEntity<Model>> GetLazyEntities(IEnumerable<DataModelRecordKey<Model>> result, Colosoft.Query.ISourceContext sourceContext, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs args = null);
	}
}
