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
	partial class EntityLoader<TEntity1, Model>
	{
		/// <summary>
		/// Classe que auxilia na configuração.
		/// </summary>
		public sealed class FluentEntityLoader
		{
			private EntityLoader<TEntity1, Model> _entityLoader;

			/// <summary>
			/// Loader associado.
			/// </summary>
			public EntityLoader<TEntity1, Model> Loader
			{
				get
				{
					return _entityLoader;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="entityLoader"></param>
			internal FluentEntityLoader(EntityLoader<TEntity1, Model> entityLoader)
			{
				_entityLoader = entityLoader;
			}

			/// <summary>
			/// Adiciona uma referencia para o loader.
			/// </summary>
			/// <param name="reference"></param>
			private void AddReference<TEntity, TModel>(EntityLoaderReference reference) where TEntity : IEntity<TModel> where TModel : class, Data.IModel
			{
				_entityLoader._references.Add(reference.Name, reference);
				foreach (var property in reference.WatchedPropertyNames)
				{
					List<EntityLoaderReference> references = null;
					if(!_entityLoader._referencesWatchedProperties.TryGetValue(property, out references))
					{
						references = new List<EntityLoaderReference> {
							reference
						};
						_entityLoader._referencesWatchedProperties.Add(property, references);
					}
					else
						references.Add(reference);
				}
			}

			/// <summary>
			/// Define a propriedade que define o identificador unico da entidade.
			/// </summary>
			/// <param name="uidProperty"></param>
			/// <returns></returns>
			public FluentEntityLoader Uid(System.Linq.Expressions.Expression<Func<Model, int>> uidProperty)
			{
				uidProperty.Require("uidProperty").NotNull();
				var property = uidProperty.GetMember() as System.Reflection.PropertyInfo;
				_entityLoader._uidPropertyName = property.Name;
				_entityLoader._uidGetter = uidProperty.Compile();
				_entityLoader._uidSetter = (dataModel, uid) =>  {
					try
					{
						property.SetValue(dataModel, uid, null);
					}
					catch(System.Reflection.TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				};
				return this;
			}

			/// <summary>
			/// Registra a propriedade que define a descrição da entidade.
			/// </summary>
			/// <param name="description"></param>
			/// <returns></returns>
			public FluentEntityLoader Description(System.Linq.Expressions.Expression<Func<Model, string>> description)
			{
				description.Require("description").NotNull();
				if(description.Name == null)
					_entityLoader._descriptionPropertyName = description.ToString().Split('.').Last();
				else
					_entityLoader._descriptionPropertyName = description.Name;
				_entityLoader._descriptionGetter = description.Compile();
				return this;
			}

			/// <summary>
			/// Registra a forma de criação o descritor para a entidade.
			/// </summary>
			/// <param name="creator"></param>
			/// <returns></returns>
			public FluentEntityLoader Descriptor(Func<CreateEntityDescriptorArgs, IEntityDescriptor> creator)
			{
				creator.Require("creator").NotNull();
				_entityLoader._entityDescriptorGetter = creator;
				return this;
			}

			/// <summary>
			/// Registra as propriedades que compõem o nome único da instância.
			/// </summary>
			/// <param name="converter">conversor</param>
			/// <param name="findNameProperties">nome das propriedades</param>
			/// <returns></returns>
			public FluentEntityLoader FindName(IFindNameConverter converter, params System.Linq.Expressions.Expression<Func<Model, object>>[] findNameProperties)
			{
				findNameProperties.Require("findNameProperties").NotNull();
				var properties = new string[findNameProperties.Length];
				var getters = new Func<Model, object>[findNameProperties.Length];
				for(int index = 0; index < properties.Length; index++)
				{
					properties[index] = findNameProperties[index].GetMember().Name;
					getters[index] = findNameProperties[index].Compile();
				}
				_entityLoader._findNameProperties = properties;
				_entityLoader._findNameGetters = getters;
				_entityLoader._findNameConverter = converter;
				_entityLoader._hasFindName = true;
				return this;
			}

			/// <summary>
			/// Define a propriedade que define o nome unico da entidade.
			/// </summary>
			/// <param name="findNameProperty"></param>
			/// <returns></returns>
			public FluentEntityLoader FindName(System.Linq.Expressions.Expression<Func<Model, object>> findNameProperty)
			{
				return FindName(null, findNameProperty);
			}

			/// <summary>
			/// Define as propriedades que representa a chave da entidade.
			/// </summary>
			/// <param name="properties">Propriedades que representa a chave.</param>
			/// <returns></returns>
			public FluentEntityLoader Keys(params System.Linq.Expressions.Expression<Func<Model, object>>[] properties)
			{
				properties.Require("properties").NotNull().NotEmptyCollection();
				var hash = new Dictionary<string, Tuple<Func<Model, object>, Action<Model, object>>>();
				foreach (var property in properties)
				{
					var propInfo = property.GetMember() as System.Reflection.PropertyInfo;
					if(hash.ContainsKey(propInfo.Name))
						continue;
					var getter = property.Compile();
					var setter = new Action<Model, object>((dataModel, value) =>  {
						try
						{
							propInfo.SetValue(dataModel, value, null);
						}
						catch(System.Reflection.TargetInvocationException ex)
						{
							throw ex.InnerException;
						}
					});
					hash.Add(propInfo.Name, new Tuple<Func<Model, object>, Action<Model, object>>(getter, setter));
				}
				_entityLoader._keysGettersAndSetters = hash;
				return this;
			}

			/// <summary>
			/// Registra um referencia da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade da referencia.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade de referencia.</typeparam>
			/// <param name="name">Nome da referencia.</param>
			/// <param name="parentProperty">Propriedade da referencia no pai.</param>
			/// <param name="foreignProperty">Propridade na qual a referencia está ligada.</param>
			/// <returns></returns>
			public FluentEntityLoader Reference<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, object>> foreignProperty) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				return Reference<TEntity, TModel>(name, parentProperty, foreignProperty, true, LoadOptions.None);
			}

			/// <summary>
			/// Registra um referencia da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade da referencia.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade de referencia.</typeparam>
			/// <param name="name">Nome da referencia.</param>
			/// <param name="parentProperty">Propriedade da referencia no pai.</param>
			/// <param name="foreignProperty">Propridade na qual a referencia está ligada.</param>
			/// <param name="isLoadLazy">Identifica se a referência terá carga tardia.</param>
			/// <param name="options">Opções de carga da referência.</param>
			/// <param name="savePriority"></param>
			/// <returns></returns>
			public FluentEntityLoader Reference<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, object>> foreignProperty, bool isLoadLazy, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				string foreignPropertyName = foreignProperty.GetMember().Name;
				Func<Model, object> foreignPropertyGetter = foreignProperty.Compile();
				var conditionalLoader = new ConditionalLoader(() =>  {
					return new EntityLoaderConditional("TableId == ?foreignId").Add("?foreignId", new Colosoft.Query.ReferenceParameter(foreignPropertyName));
				});
				var reference = new EntityLoaderReference<TEntity1, Model, TEntity, TModel>(name, parentProperty, conditionalLoader, isLoadLazy, options, savePriority, new System.Linq.Expressions.Expression<Func<Model, object>>[] {
					foreignProperty
				});
				AddReference<TEntity, TModel>(reference);
				return this;
			}

			/// <summary>
			/// Registra um referencia da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade da referencia.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade de referencia.</typeparam>
			/// <param name="name">Nome da referencia.</param>
			/// <param name="parentProperty">Propriedade da referencia no pai.</param>
			/// <param name="conditional">Condicional para carga da referencia.</param>
			/// <param name="watchedProperties">Propriedade que serão monitoradas para realiza a recarga da referência.</param>
			/// <returns></returns>
			public FluentEntityLoader Reference<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, ConditionalLoader conditional, params System.Linq.Expressions.Expression<Func<Model, object>>[] watchedProperties) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				return Reference<TEntity, TModel>(name, parentProperty, conditional, true, LoadOptions.None, watchedProperties);
			}

			/// <summary>
			/// Registra um referencia da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade da referencia.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade de referencia.</typeparam>
			/// <param name="name">Nome da referencia.</param>
			/// <param name="parentProperty">Propriedade da referencia no pai.</param>
			/// <param name="conditional">Condicional para carga da referencia.</param>
			/// <param name="isLoadLazy">Identifica se a referência terá carga tardia.</param>
			/// <param name="options">Opções da carga da referência.</param>
			/// <param name="watchedProperties">Propriedade que serão monitoradas para realiza a recarga da referência.</param>
			/// <returns></returns>
			public FluentEntityLoader Reference<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, ConditionalLoader conditional, bool isLoadLazy, LoadOptions options, params System.Linq.Expressions.Expression<Func<Model, object>>[] watchedProperties) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				return Reference<TEntity, TModel>(name, parentProperty, conditional, true, LoadOptions.None, EntityChildSavePriority.AfterEntity, watchedProperties);
			}

			/// <summary>
			/// Registra um referencia da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade da referencia.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade de referencia.</typeparam>
			/// <param name="name">Nome da referencia.</param>
			/// <param name="parentProperty">Propriedade da referencia no pai.</param>
			/// <param name="conditional">Condicional para carga da referencia.</param>
			/// <param name="isLoadLazy">Identifica se a referência terá carga tardia.</param>
			/// <param name="options">Opções da carga da referência.</param>
			/// <param name="savePriority"></param>
			/// <param name="watchedProperties">Propriedade que serão monitoradas para realiza a recarga da referência.</param>
			/// <returns></returns>
			public FluentEntityLoader Reference<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, ConditionalLoader conditional, bool isLoadLazy, LoadOptions options, EntityChildSavePriority savePriority, params System.Linq.Expressions.Expression<Func<Model, object>>[] watchedProperties) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				var reference = new EntityLoaderReference<TEntity1, Model, TEntity, TModel>(name, parentProperty, conditional, isLoadLazy, options, savePriority, watchedProperties);
				AddReference<TEntity, TModel>(reference);
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, null, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usada na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, conditional, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options"></param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, null, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, conditional, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, null, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, conditional, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, null, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho simples da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleChild<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, conditional, _entityLoader, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">>Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, null, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">>Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, conditional, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, null, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, parentPropertyUid, foreignProperty, conditional, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, null, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, conditional, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, null, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho da entidade. Com suporte para foreign key Nullable.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filha.</typeparam>
			/// <typeparam name="TModel">Tipo do modelo de dados da entidade filha.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade para recupera o pai.</param>
			/// <param name="foreignProperty">Expressão da chave estrangeira do o filho com a entidade.</param>
			/// <param name="conditional">Condicional que será usado na carga.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o filho.</param>
			/// <returns></returns>
			public FluentEntityLoader Child<TEntity, TModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, ConditionalLoader conditional, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
			{
				_entityLoader._children.Add(new EntityLoaderChildInfo<TEntity1, Model, TEntity, TModel>(name, parentProperty, foreignProperty, conditional, _entityLoader, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um filho dinamico.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filho dinâmico.</typeparam>
			/// <typeparam name="TDynamic">Tipo com a configuração de carga do filho dinamico.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade do pai no qual o filho é vinculado.</param>
			/// <param name="options">Opções de carga.</param>
			/// <returns></returns>
			public FluentEntityLoader DynamicChild<TEntity, TDynamic>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, LoadOptions options = LoadOptions.None) where TEntity : IEntity where TDynamic : EntityDynamicChild, new()
			{
				TDynamic dynamicChild = default(TDynamic);
				try
				{
					dynamicChild = new TDynamic();
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				_entityLoader._children.Add(new EntityLoaderDynamicChildInfo<TEntity, TEntity1>(name, dynamicChild, parentProperty, null, false, _entityLoader, options));
				return this;
			}

			/// <summary>
			/// Registra um filho dinamico simples.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filho dinâmico.</typeparam>
			/// <typeparam name="TDynamic">Tipo com a configuração de carga do filho dinamico.</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade do pai no qual o filho é vinculado.</param>
			/// <param name="options">Opções de carga.</param>
			/// <returns></returns>
			public FluentEntityLoader DynamicSingleChild<TEntity, TDynamic>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, LoadOptions options = LoadOptions.None) where TEntity : IEntity where TDynamic : EntityDynamicChild, new()
			{
				TDynamic dynamicChild = default(TDynamic);
				try
				{
					dynamicChild = new TDynamic();
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				_entityLoader._children.Add(new EntityLoaderDynamicChildInfo<TEntity, TEntity1>(name, dynamicChild, parentProperty, null, true, _entityLoader, options));
				return this;
			}

			/// <summary>
			/// Registra um filho dinâmico.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filho dinâmico.</typeparam>
			/// <typeparam name="TTypesProviderModel">Tipo do provedore de tipos para os filhos</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade do pai no qual o filho é vinculado.</param>
			/// <param name="foreignProperty">Propriedade que será usada com relacionamento.</param>
			/// <param name="options">Opções de carga.</param>
			/// <returns></returns>
			public FluentEntityLoader DynamicChild<TEntity, TTypesProviderModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> foreignProperty, LoadOptions options = LoadOptions.None) where TEntity : IEntity where TTypesProviderModel : IEntityDynamicChildTypesModel, new()
			{
				var typesProvider = new EntityDynamicChildTypesProvider<TTypesProviderModel>();
				var dynamic = new EntityDynamicChild<TEntity1, Model>();
				var foreignKeyPropertyName = foreignProperty.GetMember().Name;
				foreach (var i in typesProvider.GetAllTypes())
				{
					Type entityType = null;
					entityType = typesProvider.GetType(i);
					if(entityType == null)
						throw new InvalidOperationException(string.Format("Type '{0}' not found", i.FullName));
					dynamic.Register(entityType, foreignKeyPropertyName, foreignKeyPropertyName, null, null);
				}
				_entityLoader._children.Add(new EntityLoaderDynamicChildInfo<TEntity, TEntity1>(name, dynamic, parentProperty, null, false, _entityLoader, options));
				return this;
			}

			/// <summary>
			/// Registra um filho dinâmico simples.
			/// </summary>
			/// <typeparam name="TEntity">Tipo da entidade do filho dinâmico.</typeparam>
			/// <typeparam name="TTypesProviderModel">Tipo do provedore de tipos para os filhos</typeparam>
			/// <param name="name">Nome do filho.</param>
			/// <param name="parentProperty">Propriedade do pai no qual o filho é vinculado.</param>
			/// <param name="foreignProperty">Propriedade que será usada com relacionamento.</param>
			/// <param name="options">Opções de carga.</param>
			/// <returns></returns>
			public FluentEntityLoader DynamicSingleChild<TEntity, TTypesProviderModel>(string name, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<Model, int>> foreignProperty, LoadOptions options = LoadOptions.None) where TEntity : IEntity where TTypesProviderModel : IEntityDynamicChildTypesModel, new()
			{
				var typesProvider = new EntityDynamicChildTypesProvider<TTypesProviderModel>();
				var dynamic = new EntityDynamicChild<TEntity1, Model>();
				var foreignKeyPropertyName = foreignProperty.GetMember().Name;
				foreach (var i in typesProvider.GetAllTypes())
				{
					var entityType = typesProvider.GetType(i);
					if(entityType == null)
						throw new InvalidOperationException(string.Format("Type '{0}' not found", i.FullName));
					dynamic.Register(entityType, foreignKeyPropertyName, foreignKeyPropertyName, null, null);
				}
				_entityLoader._children.Add(new EntityLoaderDynamicChildInfo<TEntity, TEntity1>(name, dynamic, parentProperty, null, true, _entityLoader, options));
				return this;
			}

			/// <summary>
			/// Registra um link da entidade.
			/// </summary>
			/// <typeparam name="TLink">Tipo da entidade do link.</typeparam>
			/// <typeparam name="TLinkModel">Tipo do modelo de dados do entidade link.</typeparam>
			/// <typeparam name="TParentModel">Tipo do modelo de dados do filho para onde será feito o link.</typeparam>
			/// <param name="name">Nome do link.</param>
			/// <param name="childName">Nome do filho no qual o link está associado.</param>
			/// <param name="parentProperty">Propriedade pai da entidade para onde o link será atribuído.</param>
			/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link foi feito.</param>
			/// <param name="childProperty">Propriedade do filho no qual se associa com o link.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o link.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleLink<TLink, TLinkModel, TParentModel>(string name, string childName, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TParentModel, int>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkModel, int>> childProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TLink : class, IEntity<TLinkModel> where TLinkModel : class, Colosoft.Data.IModel where TParentModel : class, Colosoft.Data.IModel
			{
				_entityLoader._links.Add(new EntityLoaderLinkInfo<TEntity1, Model, TLink, TLinkModel, TParentModel>(name, childName, parentProperty, childForeignProperty, childProperty, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um link da entidade.
			/// </summary>
			/// <typeparam name="TLink">Tipo da entidade do link.</typeparam>
			/// <typeparam name="TLinkModel">Tipo do modelo de dados do entidade link.</typeparam>
			/// <typeparam name="TParentModel">Tipo do modelo de dados do filho para onde será feito o link.</typeparam>
			/// <param name="name">Nome do link.</param>
			/// <param name="childName">Nome do filho no qual o link está associado.</param>
			/// <param name="parentProperty">Propriedade pai da entidade para onde o link será atribuído.</param>
			/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link foi feito.</param>
			/// <param name="childProperty">Propriedade do filho no qual se associa com o link.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o link.</param>
			/// <returns></returns>
			public FluentEntityLoader SingleLink<TLink, TLinkModel, TParentModel>(string name, string childName, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TParentModel, int?>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkModel, int?>> childProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TLink : class, IEntity<TLinkModel> where TLinkModel : class, Colosoft.Data.IModel where TParentModel : class, Colosoft.Data.IModel
			{
				_entityLoader._links.Add(new EntityLoaderLinkInfo<TEntity1, Model, TLink, TLinkModel, TParentModel>(name, childName, parentProperty, childForeignProperty, childProperty, true, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um link da entidade.
			/// </summary>
			/// <typeparam name="TLink">Tipo da entidade do link.</typeparam>
			/// <typeparam name="TLinkModel">Tipo do modelo de dados do entidade link.</typeparam>
			/// <typeparam name="TParentModel">Tipo do modelo de dados do filho para onde será feito o link.</typeparam>
			/// <param name="name">Nome do link.</param>
			/// <param name="childName">Nome do filho no qual o link está associado.</param>
			/// <param name="parentProperty">Propriedade pai da entidade para onde o link será atribuído.</param>
			/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link foi feito.</param>
			/// <param name="childProperty">Propriedade do filho no qual se associa com o link.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o link.</param>
			/// <returns></returns>
			public FluentEntityLoader Link<TLink, TLinkModel, TParentModel>(string name, string childName, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TParentModel, int>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkModel, int>> childProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TLink : class, IEntity<TLinkModel> where TLinkModel : class, Colosoft.Data.IModel where TParentModel : class, Colosoft.Data.IModel
			{
				_entityLoader._links.Add(new EntityLoaderLinkInfo<TEntity1, Model, TLink, TLinkModel, TParentModel>(name, childName, parentProperty, childForeignProperty, childProperty, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Registra um link da entidade.
			/// </summary>
			/// <typeparam name="TLink">Tipo da entidade do link.</typeparam>
			/// <typeparam name="TLinkModel">Tipo do modelo de dados do entidade link.</typeparam>
			/// <typeparam name="TParentModel">Tipo do modelo de dados do filho para onde será feito o link.</typeparam>
			/// <param name="name">Nome do link.</param>
			/// <param name="childName">Nome do filho no qual o link está associado.</param>
			/// <param name="parentProperty">Propriedade pai da entidade para onde o link será atribuído.</param>
			/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link foi feito.</param>
			/// <param name="childProperty">Propriedade do filho no qual se associa com o link.</param>
			/// <param name="options">Opções de carga.</param>
			/// <param name="savePriority">Prioridade para salvar o link.</param>
			/// <returns></returns>
			public FluentEntityLoader Link<TLink, TLinkModel, TParentModel>(string name, string childName, System.Linq.Expressions.Expression<Func<TEntity1, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TParentModel, int?>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkModel, int?>> childProperty, LoadOptions options = LoadOptions.None, EntityChildSavePriority savePriority = EntityChildSavePriority.AfterEntity) where TLink : class, IEntity<TLinkModel> where TLinkModel : class, Colosoft.Data.IModel where TParentModel : class, Colosoft.Data.IModel
			{
				_entityLoader._links.Add(new EntityLoaderLinkInfo<TEntity1, Model, TLink, TLinkModel, TParentModel>(name, childName, parentProperty, childForeignProperty, childProperty, false, options, savePriority));
				return this;
			}

			/// <summary>
			/// Desabilita o suporte para instancia interna.
			/// </summary>
			/// <returns></returns>
			public FluentEntityLoader DisableInnerInstance()
			{
				_entityLoader._innerInstanceSupport = false;
				return this;
			}

			/// <summary>
			/// Define o criador da instancia.
			/// </summary>
			/// <param name="func"></param>
			/// <returns></returns>
			public FluentEntityLoader Creator(Func<EntityLoaderCreatorArgs<Model>, IEntity<Model>> func)
			{
				_entityLoader._entityCreator = func;
				return this;
			}
		}
	}
}
