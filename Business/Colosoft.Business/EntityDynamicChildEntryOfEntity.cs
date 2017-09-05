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
	/// Armazena os dados da entrada do filho dinamico.
	/// </summary>
	/// <typeparam name="TParentEntity"></typeparam>
	/// <typeparam name="TParentModel"></typeparam>
	/// <typeparam name="TChild"></typeparam>
	/// <typeparam name="TModel"></typeparam>
	public class EntityDynamicChildEntry<TParentEntity, TParentModel, TChild, TModel> : EntityDynamicChildEntry where TParentModel : class, Colosoft.Data.IModel where TParentEntity : IEntity<TParentModel> where TModel : class, Colosoft.Data.IModel where TChild : IEntity<TModel>
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
				return typeof(TChild);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parentPropertyUid"></param>
		/// <param name="foreignProperty"></param>
		/// <param name="conditional"></param>
		/// <param name="executePredicate">Predicado para executar a consulta do filho.</param>
		public EntityDynamicChildEntry(System.Linq.Expressions.Expression<Func<TParentModel, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty, EntityLoaderConditional conditional, Colosoft.Query.QueryExecutePredicate executePredicate) : base(GetParentUidSetter(foreignProperty), GetParentUidGetter(parentPropertyUid), foreignProperty.GetMember() as System.Reflection.PropertyInfo, conditional, executePredicate)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parentPropertyUidName"></param>
		/// <param name="foreignPropertyName"></param>
		/// <param name="conditional"></param>
		/// <param name="executePredicate">Predicado para executar a consulta do filho.</param>
		public EntityDynamicChildEntry(string parentPropertyUidName, string foreignPropertyName, EntityLoaderConditional conditional, Colosoft.Query.QueryExecutePredicate executePredicate) : base(GetParentUidSetter(parentPropertyUidName), GetParentUidGetter(foreignPropertyName), GetProperty<TModel>(foreignPropertyName), conditional, executePredicate)
		{
		}

		/// <summary>
		/// Recupera a propriedade do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private static System.Reflection.PropertyInfo GetProperty<T>(string propertyName)
		{
			propertyName.Require("propertyName").NotNull();
			var property = typeof(TModel).GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(property == null)
				throw new InvalidOperationException(string.Format("Property '{0}' not found in type '{1}'", propertyName, typeof(T).FullName));
			return property;
		}

		/// <summary>
		/// Recupera o método que define o UID do pai para o filho.
		/// </summary>
		/// <param name="foreignPropertyName">Nome da propriedade que representa a chave estrangeira.</param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetter(string foreignPropertyName)
		{
			foreignPropertyName.Require("foreignPropertyName").NotNull();
			var property = GetProperty<TModel>(foreignPropertyName);
			if(property == null)
				throw new InvalidOperationException(string.Format("Property '{0}' not found in type '{1}'", foreignPropertyName, typeof(TModel).FullName));
			return GetParentUidSetter(property);
		}

		/// <summary>
		/// Recupera o método que define a UID do pai para o filho
		/// </summary>
		/// <param name="foreignProperty"></param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetter(System.Linq.Expressions.Expression<Func<TModel, int>> foreignProperty)
		{
			return GetParentUidSetter(foreignProperty.GetMember() as System.Reflection.PropertyInfo);
		}

		/// <summary>
		/// Recupera o método que define o UID do pai para o filho.
		/// </summary>
		/// <param name="foreignProperty">Instancia da propriedade que representa a chave estrangeira.</param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentUidSetter(System.Reflection.PropertyInfo foreignProperty)
		{
			return (parent, child) =>  {
				var dataModel = ((TChild)child).DataModel;
				var oldValue = foreignProperty.GetValue(dataModel, null);
				var changed = false;
				if(parent.HasUid && (int)oldValue != parent.Uid)
				{
					foreignProperty.SetValue(dataModel, parent.Uid, null);
					changed = true;
				}
				else if(parent.Loader.HasKeys)
				{
					var keysPropertyNames = parent.Loader.KeysPropertyNames.ToArray();
					if(keysPropertyNames.Length > 0)
					{
						var keys = new object[keysPropertyNames.Length];
						var parentDataModel = ((TParentEntity)parent).DataModel;
						var dataModelType = parentDataModel.GetType();
						for(var i = 0; i < keysPropertyNames.Length; i++)
						{
							var propertyInfo = dataModelType.GetProperty(keysPropertyNames[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
							if(propertyInfo != null)
								keys[i] = propertyInfo.GetValue(parentDataModel, null);
						}
						if((oldValue == null && keys[0] != null) || (oldValue != null && keys[0] == null) || (oldValue != null && !oldValue.Equals(keys[0])))
						{
							foreignProperty.SetValue(dataModel, keys[0], null);
							changed = true;
						}
					}
				}
				if(changed)
				{
					var registerPropertyChanged = child as IRegisterPropertyChanged;
					if(registerPropertyChanged != null)
						registerPropertyChanged.RegistrerPropertyChanged(foreignProperty.Name);
				}
			};
		}

		/// <summary>
		/// Recupera o delegate usado para recupera o identificador unico do pai.
		/// </summary>
		/// <param name="parentPropertyUidName"></param>
		/// <returns></returns>
		private static Func<Colosoft.Data.IModel, int> GetParentUidGetter(string parentPropertyUidName)
		{
			var property = GetProperty<TParentModel>(parentPropertyUidName);
			if(property == null)
				throw new InvalidOperationException(string.Format("Property '{0}' not found in type '{1}'", parentPropertyUidName, typeof(TParentModel).FullName));
			return parent =>  {
				return (int)property.GetValue((TParentModel)parent, null);
			};
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
	}
}
