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
	/// Armazena as informa
	/// </summary>
	/// <typeparam name="TParentEntity"></typeparam>
	/// <typeparam name="TParentModel"></typeparam>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TModel"></typeparam>
	class EntityLoaderChildInfo<TParentEntity, TParentModel, TEntity, TModel> : EntityLoaderChildInfo where TParentModel : class, Colosoft.Data.IModel where TParentEntity : IEntity<TParentModel> where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
	{
		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		public override Type DataModelType
		{
			get
			{
				return typeof(TModel);
			}
		}

		/// <summary>
		/// Tipo da entidade.
		/// </summary>
		public override Type EntityType
		{
			get
			{
				return typeof(TEntity);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="parentProperty">Propriedade que será definida para o pai.</param>
		/// <param name="foreignProperty">Propriedade que será usada como chave estrangeira.</param>
		/// <param name="conditional">Condicional que será usado na carga.</param>
		/// <param name="parentLoader">Instancia do loader da entidade.</param>
		/// <param name="isSingle">Identifica se é um filho unico.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority">Prioridade para salvar o filho</param>
		public EntityLoaderChildInfo(string name, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, ConditionalLoader conditional, IEntityLoader parentLoader, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority) : base(name, parentProperty.GetMember().Name, GetParentUidSetter(foreignProperty), null, f => parentProperty.Compile()((TParentEntity)f), GetParentValueSetter(parentProperty), foreignProperty.GetMember() as System.Reflection.PropertyInfo, conditional, parentLoader, isSingle, options, savePriority)
		{
		}

		/// <summary>
		/// Construtor padrão com suporte para foreign key Nullable.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="parentProperty">Propriedade que será definida para o pai.</param>
		/// <param name="foreignProperty">Propriedade que será usada como chave estrangeira.</param>
		/// <param name="conditional">Condicional que será usado na carga.</param>
		/// <param name="parentLoader">Instancia do loader associado.</param>
		/// <param name="isSingle">Identifica se é um filho unico.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority">Prioridade para salvar o filho</param>
		public EntityLoaderChildInfo(string name, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, ConditionalLoader conditional, IEntityLoader parentLoader, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority) : base(name, parentProperty.GetMember().Name, GetParentUidSetterNullable(foreignProperty), null, f => parentProperty.Compile()((TParentEntity)f), GetParentValueSetter(parentProperty), foreignProperty.GetMember() as System.Reflection.PropertyInfo, conditional, parentLoader, isSingle, options, savePriority)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="parentProperty">Propriedade que será definida para o pai.</param>
		/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
		/// <param name="foreignProperty">Propriedade que será usada como chave estrangeira.</param>
		/// <param name="conditional">Condicional que será usado na carga.</param>
		/// <param name="parentLoader">Instancia do loader associado.</param>
		/// <param name="isSingle">Identifica se é um filho unico.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority">Prioridade para salvar o filho</param>
		public EntityLoaderChildInfo(string name, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TParentModel, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, ConditionalLoader conditional, IEntityLoader parentLoader, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority) : base(name, parentProperty.GetMember().Name, GetParentUidSetter2(parentPropertyUid, foreignProperty), GetParentUidGetter(parentPropertyUid), f => parentProperty.Compile()((TParentEntity)f), GetParentValueSetter(parentProperty), foreignProperty.GetMember() as System.Reflection.PropertyInfo, conditional, parentLoader, isSingle, options, savePriority)
		{
		}

		/// <summary>
		/// Construtor padrão com suporte para foreign key Nullable.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="parentProperty">Propriedade que será definida para o pai.</param>
		/// <param name="parentPropertyUid">Propriedade que será usada para recuepra o identificador unico do pai.</param>
		/// <param name="foreignProperty">Propriedade que será usada como chave estrangeira.</param>
		/// <param name="conditional">Condicional que será usado na carga.</param>
		/// <param name="parentLoader">Instancia do loader associado.</param>
		/// <param name="isSingle">Identifica se é um filho unico.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority">Prioridade para salvar o filho</param>
		public EntityLoaderChildInfo(string name, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TParentModel, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty, ConditionalLoader conditional, IEntityLoader parentLoader, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority) : base(name, parentProperty.GetMember().Name, GetParentUidSetter2Nullable(parentPropertyUid, foreignProperty), GetParentUidGetter(parentPropertyUid), f => parentProperty.Compile()((TParentEntity)f), GetParentValueSetter(parentProperty), foreignProperty.GetMember() as System.Reflection.PropertyInfo, conditional, parentLoader, isSingle, options, savePriority)
		{
		}

		/// <summary>
		/// Recupera o delegate usado para recupera o identificador unico do pai.
		/// </summary>
		/// <param name="parentPropertyUid"></param>
		/// <returns></returns>
		private static Func<Colosoft.Data.IModel, int> GetParentUidGetter(System.Linq.Expressions.Expression<Func<TParentModel, int>> parentPropertyUid)
		{
			var getter = parentPropertyUid.Compile();
			return parent =>  {
				return getter((TParentModel)parent);
			};
		}

		/// <summary>
		/// Recupera o método que define a UID do pai para o filho
		/// </summary>
		/// <param name="foreignProperty"></param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetter(System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty)
		{
			var property = foreignProperty.GetMember() as System.Reflection.PropertyInfo;
			return (parent, child) =>  {
				var dataModel = ((TEntity)child).DataModel;
				var oldValue = (int)property.GetValue(dataModel, null);
				if(oldValue != parent.Uid)
				{
					property.SetValue(dataModel, parent.Uid, null);
					var registerPropertyChanged = child as IRegisterPropertyChanged;
					if(registerPropertyChanged != null)
						registerPropertyChanged.RegistrerPropertyChanged(property.Name);
				}
			};
		}

		/// <summary>
		/// Recupera o método que define a UID do pai para o filho
		/// </summary>
		/// <param name="foreignProperty"></param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetterNullable(System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty)
		{
			var property = foreignProperty.GetMember() as System.Reflection.PropertyInfo;
			return (parent, child) =>  {
				var dataModel = ((TEntity)child).DataModel;
				var oldValue = (int?)property.GetValue(dataModel, null);
				if(oldValue != parent.Uid)
				{
					property.SetValue(dataModel, (int?)parent.Uid, null);
					var registerPropertyChanged = child as IRegisterPropertyChanged;
					if(registerPropertyChanged != null)
						registerPropertyChanged.RegistrerPropertyChanged(property.Name);
				}
			};
		}

		/// <summary>
		/// Recupera o método que define o UID do para para o filho.
		/// </summary>
		/// <param name="parentPropertyUid">Expressão que será usada pare recupera o UID do pai.</param>
		/// <param name="foreignProperty">Expressão da propriedade que recupera o valor da chave estrangeira.</param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetter2(System.Linq.Expressions.Expression<Func<TParentModel, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty)
		{
			var pProperty = parentPropertyUid.Compile();
			var fkProperty = foreignProperty.GetMember() as System.Reflection.PropertyInfo;
			return (parent, child) =>  {
				var parentValue = pProperty(((TParentEntity)parent).DataModel);
				var dataModel = ((TEntity)child).DataModel;
				var oldValue = (int)fkProperty.GetValue(dataModel, null);
				if(oldValue != parentValue)
				{
					fkProperty.SetValue(dataModel, parentValue, null);
					var registerPropertyChanged = child as IRegisterPropertyChanged;
					if(registerPropertyChanged != null)
						registerPropertyChanged.RegistrerPropertyChanged(fkProperty.Name);
				}
			};
		}

		/// <summary>
		/// Recupera o método que define o UID do para para o filho. (Suporte a ForeignKey Nullable)
		/// </summary>
		/// <param name="parentPropertyUid">Expressão que será usada pare recupera o UID do pai.</param>
		/// <param name="foreignProperty">Expressão da propriedade que recupera o valor da chave estrangeira.</param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetter2Nullable(System.Linq.Expressions.Expression<Func<TParentModel, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int?>> foreignProperty)
		{
			var pProperty = parentPropertyUid.Compile();
			var fkProperty = foreignProperty.GetMember() as System.Reflection.PropertyInfo;
			return (parent, child) =>  {
				var parentValue = pProperty(((TParentEntity)parent).DataModel);
				var dataModel = ((TEntity)child).DataModel;
				var oldValue = (int?)fkProperty.GetValue(dataModel, null);
				if(!oldValue.HasValue || oldValue.GetValueOrDefault() != parentValue)
				{
					fkProperty.SetValue(dataModel, (int?)parentValue, null);
					var registerPropertyChanged = child as IRegisterPropertyChanged;
					if(registerPropertyChanged != null)
						registerPropertyChanged.RegistrerPropertyChanged(fkProperty.Name);
				}
			};
		}

		/// <summary>
		/// Recupera o método usado para define o valor do filho para o pai.
		/// </summary>
		/// <param name="parentProperty"></param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentValueSetter(System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty)
		{
			var property = parentProperty.GetMember() as System.Reflection.PropertyInfo;
			if(property.CanWrite)
			{
				return (parent, child) =>  {
					if(parent == null)
						throw new ArgumentNullException("parent");
					property.SetValue(parent, child, null);
				};
			}
			else
				return null;
		}

		/// <summary>
		/// Avalia se o registro contém dados associados
		/// com a lista dos filhos..
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parent">Instancia do pai.</param>
		/// <returns></returns>
		public override bool Evaluate(Query.IRecord record, IEntity parent)
		{
			if(record.Descriptor.Contains(this.ForeignPropertyName) && this.ParentUidGetter != null)
			{
				var parentUid = this.ParentUidGetter(((TParentEntity)parent).DataModel);
				var recordValue = record.GetInt32(this.ForeignPropertyName);
				return parentUid == recordValue;
			}
			return false;
		}
	}
}
