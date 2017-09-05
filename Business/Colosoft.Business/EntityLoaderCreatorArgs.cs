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
	/// Armazena os argumentos usado pelo criador da entidade.
	/// </summary>
	public class EntityLoaderCreatorArgs : IDisposable
	{
		private Data.IModel _dataModel;

		private Colosoft.Query.RecordKey _recordKey;

		private EntityLoaderChildContainer _children;

		private IEntityLoaderLinksContainer _links;

		private EntityLoaderReferenceContainer _references;

		private string _uiContext;

		private IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Instancia do modelo de dados.
		/// </summary>
		public Data.IModel DataModel
		{
			get
			{
				return _dataModel;
			}
		}

		/// <summary>
		/// Chave que representa o registro associado com os dados.
		/// </summary>
		public Query.RecordKey RecordKey
		{
			get
			{
				return _recordKey;
			}
		}

		/// <summary>
		/// Container dos filhos.
		/// </summary>
		public EntityLoaderChildContainer Children
		{
			get
			{
				return _children;
			}
		}

		/// <summary>
		/// Container dos links.
		/// </summary>
		public IEntityLoaderLinksContainer Links
		{
			get
			{
				return _links;
			}
		}

		/// <summary>
		/// Container das referências.
		/// </summary>
		public EntityLoaderReferenceContainer References
		{
			get
			{
				return _references;
			}
		}

		/// <summary>
		/// Contexto visual.
		/// </summary>
		public string UIContext
		{
			get
			{
				return _uiContext;
			}
		}

		/// <summary>
		/// Gerenciador de tipos.
		/// </summary>
		public IEntityTypeManager TypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="dataModel"></param>
		/// <param name="recordKey"></param>
		/// <param name="children"></param>
		/// <param name="linkContainer"></param>
		/// <param name="references">Container das referências.</param>
		/// <param name="uiContext"></param>
		/// <param name="typeManager"></param>
		public EntityLoaderCreatorArgs(Data.IModel dataModel, Query.RecordKey recordKey, EntityLoaderChildContainer children, IEntityLoaderLinksContainer linkContainer, EntityLoaderReferenceContainer references, string uiContext, IEntityTypeManager typeManager)
		{
			_dataModel = dataModel;
			_recordKey = recordKey;
			_children = children;
			_links = linkContainer;
			_references = references;
			_uiContext = uiContext;
			_entityTypeManager = typeManager;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityLoaderCreatorArgs()
		{
			Dispose(false);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_children.Dispose();
			_links.Dispose();
			_references.Dispose();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
