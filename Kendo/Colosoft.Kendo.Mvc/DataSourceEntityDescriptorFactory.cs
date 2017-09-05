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

using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Kendo.Mvc.UI.Fluent
{
	/// <summary>
	/// Classe usada para configuração do descritor da entidade,
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class DataSourceEntityDescriptorFactory<TEntity> where TEntity : class
	{
		/// <summary>
		/// Model associada.
		/// </summary>
		protected readonly ModelDescriptor _model;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="model"></param>
		public DataSourceEntityDescriptorFactory(ModelDescriptor model)
		{
			_model = model;
			var loader = Colosoft.Business.EntityTypeManager.Instance.GetLoader(typeof(TEntity));
			if(loader.HasUid)
				Id(loader.UidPropertyName);
		}

		/// <summary>
		/// Specify the member used to identify an unique Model instance.
		/// </summary>
		/// <param name="fieldName">The member name.</param>
		protected void Id(string fieldName)
		{
			IGridDataKey<TEntity> dataKey;
			if(typeof(TEntity) == typeof(System.Data.DataRowView))
			{
				dataKey = (IGridDataKey<TEntity>)new GridRowViewDataKey(fieldName);
			}
			else
			{
				dataKey = GetDataKeyForField(fieldName);
			}
			dataKey.RouteKey = dataKey.Name;
			_model.Id = dataKey;
		}

		/// <summary>
		/// Recupera os dados da chave para o campo.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		protected IGridDataKey<TEntity> GetDataKeyForField(string fieldName)
		{
			var lambdaExpression = ExpressionBuilder.Lambda<TEntity>(fieldName);
			var columnType = typeof(GridDataKey<, >).MakeGenericType(new[] {
				typeof(TEntity),
				lambdaExpression.Body.Type
			});
			var constructor = columnType.GetConstructor(new[] {
				lambdaExpression.GetType()
			});
			return (IGridDataKey<TEntity>)constructor.Invoke(new object[] {
				lambdaExpression
			});
		}

		/// <summary>
		/// Adiciona o descritor do campo.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="memberName"></param>
		/// <param name="memberType"></param>
		/// <returns></returns>
		protected DataSourceModelFieldDescriptorBuilder<TValue> AddFieldDescriptor<TValue>(string memberName, Type memberType)
		{
			var descriptor = _model.AddDescriptor(memberName);
			descriptor.MemberType = memberType;
			return new DataSourceModelFieldDescriptorBuilder<TValue>(descriptor);
		}

		/// <summary>
		/// Describes a Model field
		/// </summary>
		/// <typeparam name="TValue">Field type</typeparam>
		/// <param name="expression">Member access expression which describes the field</param>        
		public virtual DataSourceModelFieldDescriptorBuilder<TValue> Field<TValue>(Expression<Func<TEntity, TValue>> expression)
		{
			return AddFieldDescriptor<TValue>(expression.MemberWithoutInstance(), typeof(TValue));
		}

		/// <summary>
		/// Describes a Model field
		/// </summary>
		/// <param name="memberName">Field name</param>
		/// <param name="memberType">Field type</param>        
		public virtual DataSourceModelFieldDescriptorBuilder<object> Field(string memberName, Type memberType)
		{
			return AddFieldDescriptor<object>(memberName, memberType);
		}

		/// <summary>
		/// Describes a Model field
		/// </summary>
		/// <typeparam name="TValue">Field type</typeparam>
		/// <param name="memberName">Member name</param>        
		public virtual DataSourceModelFieldDescriptorBuilder<TValue> Field<TValue>(string memberName)
		{
			return AddFieldDescriptor<TValue>(memberName, typeof(TValue));
		}
	}
}
