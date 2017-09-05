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

namespace Colosoft.Query
{
	/// <summary>
	/// Implementação da sessão da estratégia de vinculação do tipo.
	/// </summary>
	class TypeBindStrategySession : IQueryResultBindStrategySession
	{
		private TypeBindRecordDescriptorSchema _schema;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="schema"></param>
		public TypeBindStrategySession(TypeBindRecordDescriptorSchema schema)
		{
			_schema = schema;
		}

		/// <summary>
		/// Preenche os dados do objeto com o conteudo do registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="mode"></param>
		/// <param name="instance"></param>
		/// <returns>Nomes das propriedade alteradas.</returns>
		public IEnumerable<string> Bind(IRecord record, BindStrategyMode mode, ref object instance)
		{
			var result = new List<string>();
			if(instance is Colosoft.Data.IStorageControl)
				((Colosoft.Data.IStorageControl)instance).ExistsInStorage = true;
			foreach (var property in _schema)
			{
				var value = property.GetRecordValue(record);
				if(mode == BindStrategyMode.Differences)
				{
					object oldValue = null;
					try
					{
						oldValue = property.GetValue(instance);
					}
					catch(Exception ex)
					{
						throw new TypeBindStrategyException(ResourceMessageFormatter.Create(() => Properties.Resources.TypeBindStrategy_GetPropertyValueError, property.FieldName, instance.GetType().FullName).Format(), ex);
					}
					if((typeof(IComparable).IsAssignableFrom(property.PropertyType) && System.Collections.Comparer.Default.Compare(value, oldValue) == 0) || object.ReferenceEquals(value, oldValue))
					{
						continue;
					}
				}
				try
				{
					property.SetValue(instance, value);
				}
				catch(ArgumentException ex)
				{
					throw new TypeBindStrategyException(ResourceMessageFormatter.Create(() => Properties.Resources.TypeBindStrategy_ConvertValueError, property.FieldName, instance.GetType().FullName, (value != null ? value.GetType().FullName : "System.Object"), property.PropertyType.FullName).Format(), ex);
				}
				result.Add(property.Name);
			}
			if(instance is IExtensiveData)
				((IExtensiveData)instance).Process(record);
			return result;
		}
	}
}
