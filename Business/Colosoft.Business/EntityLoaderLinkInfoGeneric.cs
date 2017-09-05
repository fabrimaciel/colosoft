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
	/// Implementação genérica para as informações do link da entidade.
	/// </summary>
	/// <typeparam name="TParentEntity">Tipo da entidade pai.</typeparam>
	/// <typeparam name="TParentDataModel">Tipo dos dados da entidade pai.</typeparam>
	/// <typeparam name="TLinkDataModel">Tipo dos dados do link.</typeparam>
	/// <typeparam name="TLinkEntity">Tipo da entidade do link.</typeparam>
	/// <typeparam name="TLinkParentDataModel">Tipo dos dados do pai do link,</typeparam>
	class EntityLoaderLinkInfo<TParentEntity, TParentDataModel, TLinkEntity, TLinkDataModel, TLinkParentDataModel> : EntityLoaderLinkInfo, IEntityPropertyAccessor where TParentDataModel : class, Colosoft.Data.IModel where TParentEntity : IEntity<TParentDataModel> where TLinkEntity : class, IEntity<TLinkDataModel> where TLinkDataModel : class, Colosoft.Data.IModel where TLinkParentDataModel : class, Colosoft.Data.IModel
	{
		private System.Linq.Expressions.Expression<Func<TLinkParentDataModel, int>> _foreignProperty;

		private System.Linq.Expressions.Expression<Func<TLinkParentDataModel, int?>> _foreignPropertyNullable;

		private System.Reflection.PropertyInfo _foreignPropertyInfo;

		private Func<TLinkParentDataModel, int> _foreignPropertyCompiled;

		private Func<TLinkParentDataModel, int?> _foreignPropertyNullableCompiled;

		private System.Linq.Expressions.Expression<Func<TLinkDataModel, int>> _childProperty;

		private System.Linq.Expressions.Expression<Func<TLinkDataModel, int?>> _childPropertyNullable;

		private System.Reflection.PropertyInfo _childPropertyInfo;

		private Func<TLinkDataModel, int> _childPropertyCompiled;

		private Func<TLinkDataModel, int?> _childPropertyNullableCompiled;

		private System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> _parentProperty;

		private Func<TParentEntity, IEntity> _parentPropertyCompiled;

		/// <summary>
		/// Tipo do modelo de dados do link.
		/// </summary>
		public override Type LinkDataModelType
		{
			get
			{
				return typeof(TLinkDataModel);
			}
		}

		/// <summary>
		/// Tipo da entidade do link.
		/// </summary>
		public override Type LinkEntityType
		{
			get
			{
				return typeof(TLinkEntity);
			}
		}

		/// <summary>
		/// Tipo do modelo da dados do filho associado.
		/// </summary>
		public override Type ChildDataModelType
		{
			get
			{
				return typeof(TLinkParentDataModel);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do link.</param>
		/// <param name="childName">Nome do pai associado.</param>
		/// <param name="parentProperty">Propriedade da entidade pai na qual o link será definido.</param>
		/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link está associado.</param>
		/// <param name="childProperty">Propriedade do link que faz associação com o filho.</param>
		/// <param name="isSingle">Identifica se é um filho simples.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority"></param>
		public EntityLoaderLinkInfo(string name, string childName, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TLinkParentDataModel, int>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkDataModel, int>> childProperty, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority)
		{
			_foreignProperty = childForeignProperty;
			_parentProperty = parentProperty;
			_childProperty = childProperty;
			Initialize(name, childName, SetLinkParentForeignKey, GetLinkParentForeignKey, GetChildKey, GetParentValue, GetParentValueSetter(parentProperty), GetForeignPropertyInfo(), GetChildPropertyInfo(), CreateLink, CreateEntityFromLink, isSingle, options, savePriority);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do link.</param>
		/// <param name="childName">Nome do pai associado.</param>
		/// <param name="parentProperty">Propriedade da entidade pai na qual o link será definido.</param>
		/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link está associado.</param>
		/// <param name="childProperty">Propriedade do link que faz associação com o filho.</param>
		/// <param name="isSingle">Identifica se é um filho simples.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority"></param>
		public EntityLoaderLinkInfo(string name, string childName, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TLinkParentDataModel, int>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkDataModel, int?>> childProperty, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority)
		{
			_foreignProperty = childForeignProperty;
			_parentProperty = parentProperty;
			_childPropertyNullable = childProperty;
			Initialize(name, childName, SetLinkParentForeignKey, GetLinkParentForeignKey, GetChildKeyNullable, GetParentValue, GetParentValueSetter(parentProperty), GetForeignPropertyInfo(), GetChildPropertyInfo(), CreateLink, CreateEntityFromLink, isSingle, options, savePriority);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do link.</param>
		/// <param name="childName">Nome do pai associado.</param>
		/// <param name="parentProperty">Propriedade da entidade pai na qual o link será definido.</param>
		/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link está associado.</param>
		/// <param name="childProperty">Propriedade do link que faz associação com o filho.</param>
		/// <param name="isSingle">Identifica se é um filho simples.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority"></param>
		public EntityLoaderLinkInfo(string name, string childName, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TLinkParentDataModel, int?>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkDataModel, int?>> childProperty, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority)
		{
			_foreignPropertyNullable = childForeignProperty;
			_childPropertyNullable = childProperty;
			_parentProperty = parentProperty;
			Initialize(name, childName, SetLinkParentForeignKeyNullable, GetLinkParentForeignKeyNullable, GetChildKeyNullable, GetParentValue, GetParentValueSetter(parentProperty), GetForeignPropertyInfo(), GetChildPropertyInfo(), CreateLink, CreateEntityFromLink, isSingle, options, savePriority);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do link.</param>
		/// <param name="childName">Nome do pai associado.</param>
		/// <param name="parentProperty">Propriedade da entidade pai na qual o link será definido.</param>
		/// <param name="childForeignProperty">Propriedade do filho da entidade na qual o link está associado.</param>
		/// <param name="childProperty">Propriedade do link que faz associação com o filho.</param>
		/// <param name="isSingle">Identifica se é um filho simples.</param>
		/// <param name="options">Opções de carga.</param>
		/// <param name="savePriority"></param>
		public EntityLoaderLinkInfo(string name, string childName, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, System.Linq.Expressions.Expression<Func<TLinkParentDataModel, int?>> childForeignProperty, System.Linq.Expressions.Expression<Func<TLinkDataModel, int>> childProperty, bool isSingle, LoadOptions options, EntityChildSavePriority savePriority)
		{
			_foreignPropertyNullable = childForeignProperty;
			_childProperty = childProperty;
			_parentProperty = parentProperty;
			Initialize(name, childName, SetLinkParentForeignKeyNullable, GetLinkParentForeignKeyNullable, GetChildKey, GetParentValue, GetParentValueSetter(parentProperty), GetForeignPropertyInfo(), GetChildPropertyInfo(), CreateLink, CreateEntityFromLink, isSingle, options, savePriority);
		}

		/// <summary>
		/// Recupera o método usado para define o valor do filho para o pai.
		/// </summary>
		/// <param name="parentProperty"></param>
		/// <returns></returns>
		private Action<IEntity, IEntity> GetParentValueSetter(System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty)
		{
			var parentPropertyInfo = parentProperty.GetMember() as System.Reflection.PropertyInfo;
			return parentPropertyInfo.CanWrite ? SetParentValue : (Action<IEntity, IEntity>)null;
		}

		/// <summary>
		/// Recupera a propriedade do filho compilada.
		/// </summary>
		/// <returns></returns>
		private Func<TLinkDataModel, int> GetChildPropertyCompiled()
		{
			if(_childProperty != null && _childPropertyCompiled == null)
				_childPropertyCompiled = _childProperty.Compile();
			return _childPropertyCompiled;
		}

		/// <summary>
		/// Recupera a propriedade do filho compilada.
		/// </summary>
		/// <returns></returns>
		private Func<TLinkDataModel, int?> GetChildPropertyNullableCompiled()
		{
			if(_childPropertyNullable != null && _childPropertyNullableCompiled == null)
				_childPropertyNullableCompiled = _childPropertyNullable.Compile();
			return _childPropertyNullableCompiled;
		}

		/// <summary>
		/// Recupera as informações da propriedade do filho.
		/// </summary>
		/// <returns></returns>
		private System.Reflection.PropertyInfo GetChildPropertyInfo()
		{
			if(_childPropertyInfo == null)
			{
				if(_childProperty != null)
					_childPropertyInfo = _childProperty.GetMember() as System.Reflection.PropertyInfo;
				else
					_childPropertyInfo = _childPropertyNullable.GetMember() as System.Reflection.PropertyInfo;
			}
			return _childPropertyInfo;
		}

		/// <summary>
		/// Recupera a propriedade estrangeira compilada.
		/// </summary>
		/// <returns></returns>
		private Func<TLinkParentDataModel, int> GetForeignPropertyCompiled()
		{
			if(_foreignProperty != null && _foreignPropertyCompiled == null)
				_foreignPropertyCompiled = _foreignProperty.Compile();
			return _foreignPropertyCompiled;
		}

		/// <summary>
		/// Recupera a propriedade estrangeira compilada.
		/// </summary>
		/// <returns></returns>
		private Func<TLinkParentDataModel, int?> GetForeignPropertyNullableCompiled()
		{
			if(_foreignPropertyNullable != null && _foreignPropertyNullableCompiled == null)
				_foreignPropertyNullableCompiled = _foreignPropertyNullable.Compile();
			return _foreignPropertyNullableCompiled;
		}

		/// <summary>
		/// Recupera as informações da propriedade estrangeira.
		/// </summary>
		/// <returns></returns>
		private System.Reflection.PropertyInfo GetForeignPropertyInfo()
		{
			if(_foreignPropertyInfo == null)
			{
				if(_foreignProperty != null)
					_foreignPropertyInfo = _foreignProperty.GetMember() as System.Reflection.PropertyInfo;
				else
					_foreignPropertyInfo = _foreignPropertyNullable.GetMember() as System.Reflection.PropertyInfo;
			}
			return _foreignPropertyInfo;
		}

		/// <summary>
		/// Recupera o método compilado para recuperar o pai do link.
		/// </summary>
		/// <returns></returns>
		private Func<TParentEntity, IEntity> GetParentPropertyCompiled()
		{
			if(_parentProperty == null)
				throw new InvalidOperationException(string.Format("Not found parent property for link '{0}' from child '{1}'", Name, ChildName));
			if(_parentPropertyCompiled == null)
				_parentPropertyCompiled = _parentProperty.Compile();
			return _parentPropertyCompiled;
		}

		/// <summary>
		/// Cria um entidade do pai com base na entidade do link.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		private IEntity CreateEntityFromLink(EntityFromLinkCreatorArgs e)
		{
			var dataModel = Activator.CreateInstance<TLinkParentDataModel>();
			System.Reflection.PropertyInfo propertyInfo = null;
			try
			{
				propertyInfo = GetForeignPropertyInfo();
				if(_foreignProperty != null)
					propertyInfo.SetValue(dataModel, e.LinkEntity.Uid, null);
				else
					propertyInfo.SetValue(dataModel, (int?)e.LinkEntity.Uid, null);
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			var entity = e.EntityCreator(e.UIContext, e.EntityTypeManager, dataModel, e.SourceContext);
			var registerPropertyChanged = entity as IRegisterPropertyChanged;
			if(registerPropertyChanged != null)
				registerPropertyChanged.RegistrerPropertyChanged(propertyInfo.Name);
			return entity;
		}

		/// <summary>
		/// Cria uma instancia da entidade do link.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		private IEntity CreateLink(LinkCreatorArgs e)
		{
			int? foreignValue = 0;
			if(_foreignProperty != null)
			{
				var method = GetForeignPropertyCompiled();
				if(method == null)
					throw new InvalidOperationException("Foreign property getter not found");
				foreignValue = method((TLinkParentDataModel)e.ChildDataModel);
			}
			else
			{
				var method = GetForeignPropertyNullableCompiled();
				if(method == null)
					throw new InvalidOperationException("Foreign property (nullable) getter not found");
				foreignValue = method((TLinkParentDataModel)e.ChildDataModel);
			}
			var sourceContext = e.SourceContext;
			if(sourceContext != null)
			{
				var query = sourceContext.CreateQuery().From(new Query.EntityInfo(typeof(TLinkDataModel).FullName)).Where(string.Format("{0} == ?entityuid", ChildPropertyName)).Add("?entityuid", foreignValue);
				if(e.IsLazy)
					return query.ProcessLazyResult<TLinkEntity>(e.UIContext).FirstOrDefault();
				else
					return query.ProcessResult<TLinkEntity>(e.UIContext).FirstOrDefault();
			}
			else
				return default(TLinkEntity);
		}

		/// <summary>
		/// Recupera o método que define a UID do pai para o link
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		private void SetLinkParentForeignKey(IEntity parent, IEntity child)
		{
			GetForeignPropertyInfo().SetValue(((IEntity<TParentDataModel>)child).DataModel, parent.Uid, null);
		}

		/// <summary>
		/// Recupera o método que define a UID do pai para o filho
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		private void SetLinkParentForeignKeyNullable(IEntity parent, IEntity child)
		{
			GetForeignPropertyInfo().SetValue(((TLinkEntity)child).DataModel, (int?)parent.Uid, null);
		}

		/// <summary>
		/// Recupera o método que recupera o identificador da chave estrangeira do modelo de dados informado.
		/// </summary>
		/// <returns></returns>
		public int GetLinkParentForeignKey(Colosoft.Data.IModel model)
		{
			return GetForeignPropertyCompiled()((TLinkParentDataModel)model);
		}

		/// <summary>
		/// Recupera o método que recupera o identificador da chave estrangeira do modelo de dados informado.
		/// </summary>
		/// <returns></returns>
		public int GetLinkParentForeignKeyNullable(Colosoft.Data.IModel model)
		{
			return GetForeignPropertyNullableCompiled()((TLinkParentDataModel)model).GetValueOrDefault();
		}

		/// <summary>
		/// Recupera o identificador do filho que está no modelo de dados do link.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public int GetChildKey(Colosoft.Data.IModel model)
		{
			return GetChildPropertyCompiled()((TLinkDataModel)model);
		}

		/// <summary>
		/// Recupera o identificador do filho que está no modelo de dados do link.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public int GetChildKeyNullable(Colosoft.Data.IModel model)
		{
			return GetChildPropertyNullableCompiled()((TLinkDataModel)model).GetValueOrDefault();
		}

		/// <summary>
		/// Recupera o instancia do link a partir da entidade pai.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private IEntity GetParentValue(IEntity parent)
		{
			return GetParentPropertyCompiled()((TParentEntity)parent);
		}

		/// <summary>
		/// Associa a instancia do filho para o pai.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		private void SetParentValue(IEntity parent, IEntity child)
		{
			var property = _parentProperty.GetMember() as System.Reflection.PropertyInfo;
			if(property.CanWrite)
			{
				if(parent == null)
					throw new ArgumentNullException("parent");
				property.SetValue(parent, child, null);
			}
		}

		/// <summary>
		/// Propriedade que representa o valor o link.
		/// </summary>
		string IEntityPropertyAccessor.PropertyName
		{
			get
			{
				return _parentProperty.GetMember().Name;
			}
		}
	}
}
