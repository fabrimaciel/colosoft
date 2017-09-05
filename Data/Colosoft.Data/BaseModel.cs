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

namespace Colosoft.Data
{
	/// <summary>
	/// Implementação base para a interface <see cref="IModel"/>.
	/// </summary>
	[Serializable]
	public abstract class BaseModel : IModel, IStorageControl
	{
		private bool _existsInStorage;

		/// <summary>
		/// Verifica se a instnacia já existe na fonte de armazenamento.
		/// </summary>
		public bool ExistsInStorage
		{
			get
			{
				return _existsInStorage;
			}
			set
			{
				_existsInStorage = value;
			}
		}

		/// <summary>
		/// Identifica se é para ignorar a propriedade na criação do registro.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected virtual bool CreateRecordIgnoreProperty(string propertyName)
		{
			return propertyName == "ExistsInStorage";
		}

		/// <summary>
		/// Cria um registro a partir dos dados da instancia.
		/// </summary>
		/// <returns></returns>
		public virtual Query.Record CreateRecord()
		{
			var instanceType = GetType();
			var properties = instanceType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).Where(f => f.CanRead);
			var fields = new List<Query.Record.Field>();
			var values = new List<object>();
			foreach (var property in properties)
			{
				if(CreateRecordIgnoreProperty(property.Name))
					continue;
				fields.Add(new Query.Record.Field(property.Name, property.PropertyType));
				values.Add(property.GetValue(this, null));
			}
			var descriptor = new Query.Record.RecordDescriptor(instanceType.FullName, fields);
			return descriptor.CreateRecord(values.ToArray());
		}

		/// <summary>
		/// Cria a chave de registro da instancia.
		/// </summary>
		/// <param name="recordKeyFactory"></param>
		/// <returns></returns>
		public Query.RecordKey CreateRecordKey(Query.IRecordKeyFactory recordKeyFactory)
		{
			recordKeyFactory.Require("recordKeyFactory").NotNull();
			var typeName = Colosoft.Reflection.TypeName.Get(GetType());
			var keyFields = recordKeyFactory.GetKeyFields(typeName).ToArray();
			if(keyFields.Length == 0)
				return new Query.RecordKey(string.Empty, 0);
			var instanceType = GetType();
			var properties = instanceType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).Where(f => f.CanRead && keyFields.Contains(f.Name));
			var fields = new List<Query.Record.Field>();
			var values = new List<object>();
			foreach (var property in properties)
			{
				if(CreateRecordIgnoreProperty(property.Name))
					continue;
				fields.Add(new Query.Record.Field(property.Name, property.PropertyType));
				values.Add(property.GetValue(this, null));
			}
			var descriptor = new Query.Record.RecordDescriptor(instanceType.FullName, fields);
			return recordKeyFactory.Create(typeName, descriptor.CreateRecord(values.ToArray()));
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			var instanceType = GetType();
			var baseType = GetType().BaseType;
			var fields = instanceType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			var baseFields = baseType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			var newInstance = Activator.CreateInstance(instanceType) as BaseModel;
			newInstance.ExistsInStorage = ExistsInStorage;
			foreach (var i in fields)
			{
				var value = i.GetValue(this);
				if(value is ICloneable)
					value = ((ICloneable)value).Clone();
				i.SetValue(newInstance, value);
			}
			foreach (var i in baseFields)
			{
				var value = i.GetValue(this);
				if(value is ICloneable)
					value = ((ICloneable)value).Clone();
				i.SetValue(newInstance, value);
			}
			return newInstance;
		}
	}
}
