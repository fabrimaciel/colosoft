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
	/// Classe responsável por fazer o bind dos dados do registro para uma instancia
	/// do descritor da entidade.
	/// </summary>
	class EntityDescriptorQueryResultBindStrategy : Colosoft.Query.IQueryResultBindStrategy
	{
		private IEntityLoader _entityLoader;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityLoader">Instancia do loader da entidade.</param>
		public EntityDescriptorQueryResultBindStrategy(IEntityLoader entityLoader)
		{
			entityLoader.Require("entityLoader").NotNull();
			_entityLoader = entityLoader;
		}

		/// <summary>
		/// Recupera as informações dos valores de uma lista de propriedades.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="entityDescriptor"></param>
		/// <param name="record"></param>
		/// <returns></returns>
		private IEnumerable<object> Merge(string[] properties, IEntityDescriptor entityDescriptor, Query.IRecord record)
		{
			var entityDescriptorStateble = entityDescriptor as IEntityDescriptorStateble;
			foreach (var p in properties)
			{
				var index = record.Descriptor.GetFieldPosition(p);
				if(index >= 0)
					yield return record.GetValue(record.Descriptor.GetFieldPosition(p));
				else
				{
					if(entityDescriptorStateble != null && entityDescriptorStateble.Contains(p))
					{
						yield return entityDescriptorStateble[p];
					}
					else
						yield return null;
				}
			}
		}

		/// <summary>
		/// Cria uma instancia do descritor.
		/// </summary>
		/// <returns></returns>
		private IEntityDescriptor CreateEntityDescriptor()
		{
			var creator = _entityLoader.GetEntityDescriptorCreator();
			if(creator != null)
				return creator.CreateEntityDescriptor();
			return null;
		}

		/// <summary>
		/// Realiza os bind dos dados do registro informado.
		/// </summary>
		/// <param name="record">Registro contendo os dados.</param>
		/// <param name="mode"></param>
		/// <param name="recordKeyFactory">Factory responsável pela geração da chave do registro.</param>
		/// <param name="instance"></param>
		/// <returns></returns>
		private IEnumerable<string> Bind(Query.IRecord record, Query.BindStrategyMode mode, Query.IRecordKeyFactory recordKeyFactory, ref object instance)
		{
			record.Require("record").NotNull();
			var descriptor = instance as IEntityDescriptor;
			if(descriptor == null)
				return new string[0];
			var idFieldIndex = record.Descriptor.GetFieldPosition(_entityLoader.UidPropertyName);
			var descriptionFieldIndex = record.Descriptor.GetFieldPosition(_entityLoader.DescriptionPropertyName);
			string nameFieldValue = string.Empty;
			if(_entityLoader.FindNameProperties.Count() > 1)
			{
				var info = Merge(_entityLoader.FindNameProperties, descriptor, record).ToArray();
				if(info.Length == 0 && mode == Query.BindStrategyMode.Differences)
					nameFieldValue = descriptor.Name;
				else
					nameFieldValue = _entityLoader.FindNameConverter.Convert(info);
			}
			else
			{
				var nameFieldIndex = record.Descriptor.GetFieldPosition(_entityLoader.FindNameProperties.FirstOrDefault());
				if(nameFieldIndex >= 0)
					nameFieldValue = record[nameFieldIndex];
				else if(mode == Query.BindStrategyMode.Differences)
					nameFieldValue = descriptor.Name;
			}
			descriptor.Id = idFieldIndex >= 0 ? record[idFieldIndex] : mode == Query.BindStrategyMode.Differences ? descriptor.Id : 0;
			descriptor.Name = nameFieldValue;
			descriptor.Description = descriptionFieldIndex >= 0 ? record[descriptionFieldIndex] : mode == Query.BindStrategyMode.Differences ? descriptor.Description : string.Empty;
			if(record.Descriptor.Contains("ActivatedDate") && record.Descriptor.Contains("ExpiredDate"))
			{
				DateTimeOffset activatedDate = record["ActivatedDate"];
				DateTimeOffset? expiredDate = record["ExpiredDate"];
				var isActive = activatedDate < ServerData.GetDateTimeOffSet() && !(expiredDate.HasValue && (ServerData.GetDateTimeOffSet() > expiredDate));
				var isExpired = expiredDate.HasValue && (ServerData.GetDateTimeOffSet() > expiredDate);
				descriptor.ConfigureStatusControl(isActive, isExpired);
			}
			if(descriptor is BusinessEntityDescriptor)
			{
				var businessDescritor = (BusinessEntityDescriptor)descriptor;
				businessDescritor.RecordKey = recordKeyFactory.Create(_entityLoader.DataModelTypeName, record);
			}
			var changedProperties = new List<string> {
				"Id",
				"Name",
				"Description",
				"IsActive",
				"IsExpired"
			};
			if(descriptor is IBindableEntityDescriptor)
				changedProperties = changedProperties.Union(((IBindableEntityDescriptor)descriptor).Bind(record, mode)).ToList();
			return changedProperties.ToArray();
		}

		/// <summary>
		/// Cria uma sessão de estratégia de vinculação para o descritor informado.
		/// </summary>
		/// <param name="recordDescriptor">Descritor dos registros para fazer a vinculação.</param>
		/// <returns></returns>
		public Colosoft.Query.IQueryResultBindStrategySession CreateSession(Colosoft.Query.Record.RecordDescriptor recordDescriptor)
		{
			return new EntityDescriptorQueryResultBindStrategySession(this);
		}

		/// <summary>
		/// Realiza o bind dos dados dos registro e retorna as instancia criadas.
		/// </summary>
		/// <param name="records"></param>
		/// <param name="mode"></param>
		/// <param name="creator"></param>
		/// <returns></returns>
		public System.Collections.IEnumerable Bind(IEnumerable<Query.IRecord> records, Query.BindStrategyMode mode, Query.IQueryResultObjectCreator creator)
		{
			var recordKeyFactory = _entityLoader.GetRecordKeyFactory();
			foreach (var record in records)
			{
				object instance = CreateEntityDescriptor();
				Bind(record, mode, recordKeyFactory, ref instance);
				yield return instance;
			}
		}

		/// <summary>
		/// Realiza o bind dos dados dos registro e retorna as instancia criadas.
		/// </summary>
		/// <param name="records"></param>
		/// <param name="mode"></param>
		/// <param name="creator"></param>
		/// <returns></returns>
		public IEnumerable<T> Bind<T>(IEnumerable<Query.IRecord> records, Query.BindStrategyMode mode, Query.IQueryResultObjectCreator creator)
		{
			var recordKeyFactory = _entityLoader.GetRecordKeyFactory();
			foreach (var record in records)
			{
				object instance = CreateEntityDescriptor();
				Bind(record, mode, recordKeyFactory, ref instance);
				yield return (T)instance;
			}
		}

		/// <summary>
		/// Realiza o bind dos dados do registro para a instancia informada.
		/// </summary>
		/// <param name="record">Registro com os dados que serão processados.</param>
		/// <param name="mode">Mode de vinculação.</param>
		/// <param name="instance">Instancia que será preenchida.</param>
		/// <returns></returns>
		public IEnumerable<string> Bind(Query.IRecord record, Query.BindStrategyMode mode, ref object instance)
		{
			var recordKeyFactory = _entityLoader.GetRecordKeyFactory();
			return Bind(record, mode, recordKeyFactory, ref instance);
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado na operação.</typeparam>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		public IEnumerable<string> Bind<T>(Query.IRecord record, Query.BindStrategyMode mode, ref T instance)
		{
			record.Require("record").NotNull();
			object instance2 = instance;
			var recordKeyFactory = _entityLoader.GetRecordKeyFactory();
			var result = Bind(record, mode, recordKeyFactory, ref instance2);
			instance = (T)instance2;
			return result;
		}
	}
}
