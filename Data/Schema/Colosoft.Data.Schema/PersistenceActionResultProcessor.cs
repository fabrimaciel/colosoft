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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Processador do resultado das ações de persistencia.
	/// </summary>
	public class PersistenceActionResultProcessor
	{
		/// <summary>
		/// Nome do parametro que armazena o resultado de uma ação de exclusão.
		/// </summary>
		public const string DeleteActionResultParameterName = "DeleteActionResult";

		private ITypeSchema _typeSchema;

		private Query.IRecordKeyFactory _recordKeyFactory;

		/// <summary>
		/// Instancia da factory das chaves de registros.
		/// </summary>
		public Query.IRecordKeyFactory RecordKeyFactory
		{
			get
			{
				return _recordKeyFactory;
			}
		}

		/// <summary>
		/// Instancia do esquema de tipos do sistema.
		/// </summary>
		public ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema">Esquema dos tipos.</param>
		/// <param name="recordKeyFactory">Fabrica das chaves de registro.</param>
		public PersistenceActionResultProcessor(ITypeSchema typeSchema, Query.IRecordKeyFactory recordKeyFactory)
		{
			typeSchema.Require("typeSchema").NotNull();
			recordKeyFactory.Require("recordKeyFactory").NotNull();
			_typeSchema = typeSchema;
			_recordKeyFactory = recordKeyFactory;
		}

		/// <summary>
		/// Recupera os campos do registro.
		/// </summary>
		/// <param name="entityFullName">Nome completo da entidade.</param>
		/// <param name="typeMetadata">Metadados do tipo.</param>
		/// <returns></returns>
		private static List<Query.Record.Field> GetRecordFields(string entityFullName, Colosoft.Data.Schema.ITypeMetadata typeMetadata)
		{
			var recordFields = new List<Colosoft.Query.Record.Field>();
			foreach (var propertyMetadata in typeMetadata)
			{
				Type propertyType = null;
				try
				{
					propertyType = Type.GetType(propertyMetadata.PropertyType, true);
				}
				catch(Exception ex)
				{
					throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.PersistenceActionResultProcessor_GetPropertyTypeFromPropertyMetadataError, propertyMetadata.PropertyType, propertyMetadata.Name, entityFullName).Format(), ex);
				}
				recordFields.Add(new Colosoft.Query.Record.Field(propertyMetadata.Name, propertyType));
			}
			if(typeMetadata.IsVersioned && !recordFields.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, "RowVersion")))
				recordFields.Add(new Query.Record.Field("RowVersion", typeof(long)));
			return recordFields;
		}

		/// <summary>
		/// Recupera o registrao associado com a ação.
		/// </summary>
		/// <param name="action">Instancia da ação de persistencia realizada.</param>
		/// <param name="record">Registro gerado.</param>
		/// <param name="recordKey">Chave do registro.</param>
		/// <returns>Nome do tipo associado.</returns>
		private Colosoft.Reflection.TypeName GetActionRecord(PersistenceAction action, out Query.Record record, out Query.RecordKey recordKey)
		{
			var typeName = new Reflection.TypeName(action.EntityFullName);
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			var typeFields = GetRecordFields(action.EntityFullName, typeMetadata);
			var fields = new List<Query.Record.Field>();
			var recordValues = new List<object>();
			foreach (var actionParameter in action.Parameters)
			{
				var indexOf = 0;
				for(; indexOf < typeFields.Count; indexOf++)
					if(typeFields[indexOf].Name == actionParameter.Name)
						break;
				if(indexOf < typeFields.Count)
				{
					fields.Add(new Query.Record.Field(typeFields[indexOf].Name, typeFields[indexOf].Type));
					recordValues.Add(actionParameter.Value);
				}
			}
			if(typeMetadata.IsVersioned && !fields.Any(f => f.Name == "RowVersion"))
			{
				fields.Add(new Query.Record.Field("RowVersion", typeof(long)));
				recordValues.Add(action.RowVersion);
			}
			var recordDescriptor = new Query.Record.RecordDescriptor("default", fields);
			record = recordDescriptor.CreateRecord(recordValues.ToArray());
			recordKey = RecordKeyFactory.Create(typeName, record);
			return typeName;
		}

		/// <summary>
		/// Navega pelas ações informada.
		/// </summary>
		/// <param name="actions"></param>
		private void NavigateActions(IEnumerable<PersistenceAction> actions)
		{
			foreach (var action in actions)
				ProcessAction(action);
		}

		/// <summary>
		/// Processa a ações informada.
		/// </summary>
		/// <param name="action"></param>
		private void ProcessAction(PersistenceAction action)
		{
			if(action.BeforeActions.Count > 0)
			{
				NavigateActions(action.BeforeActions);
			}
			switch(action.Type)
			{
			case PersistenceActionType.Insert:
				ProcessInsertAction(action);
				break;
			case PersistenceActionType.Update:
				ProcessUpdateAction(action);
				break;
			case PersistenceActionType.Delete:
				ProcessDeleteAction(action);
				break;
			}
			if(action.AfterActions.Count > 0)
			{
				NavigateActions(action.AfterActions);
			}
		}

		/// <summary>
		/// Processa a ação de inserção.
		/// </summary>
		/// <param name="action"></param>
		private void ProcessInsertAction(PersistenceAction action)
		{
			var observerManager = Colosoft.Query.RecordObserverManager.Instance;
			if(observerManager == null)
				return;
			Colosoft.Reflection.TypeName typeName = null;
			Query.Record record;
			Query.RecordKey recordKey;
			typeName = GetActionRecord(action, out record, out recordKey);
			observerManager.NotifyRecordsInserted(typeName, new Colosoft.Query.Record[] {
				record
			});
		}

		/// <summary>
		/// Processa a ação de atualização.
		/// </summary>
		/// <param name="action"></param>
		private void ProcessUpdateAction(PersistenceAction action)
		{
			var observerManager = Colosoft.Query.RecordObserverManager.Instance;
			if(observerManager == null)
				return;
			Colosoft.Reflection.TypeName typeName = null;
			Query.Record record;
			Query.RecordKey recordKey;
			typeName = GetActionRecord(action, out record, out recordKey);
			var notifier = observerManager.GetRecordChangedNotifier(typeName, recordKey);
			if(notifier.IsValid)
				notifier.Notify(record);
		}

		/// <summary>
		/// Processa a ação de exclusão.
		/// </summary>
		/// <param name="action"></param>
		private void ProcessDeleteAction(PersistenceAction action)
		{
			var observerManager = Colosoft.Query.RecordObserverManager.Instance;
			if(observerManager == null)
				return;
			Colosoft.Reflection.TypeName typeName = null;
			Query.Record record;
			Query.RecordKey recordKey;
			var deleteActionResult = action.Parameters.Where(f => f.Name == DeleteActionResultParameterName).Select(f => (IDeleteActionResult)f.Value).FirstOrDefault();
			IEnumerable<Colosoft.Query.RecordKey> recordKeys = null;
			if(deleteActionResult == null)
			{
				typeName = GetActionRecord(action, out record, out recordKey);
				recordKeys = new Colosoft.Query.RecordKey[] {
					recordKey
				};
			}
			else
			{
				typeName = new Reflection.TypeName(action.EntityFullName);
				recordKeys = deleteActionResult.RecordKeys;
			}
			observerManager.NotifyRecordDeleted(typeName, recordKeys);
		}

		/// <summary>
		/// Faz uma fixação do dados do resultado com as ações informadas.
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static IEnumerable<PersistenceAction> FixActionsResults(IEnumerable<PersistenceAction> actions, PersistenceActionResult[] result)
		{
			var actionsResult = new List<PersistenceAction>();
			using (var actionEnumerator = actions.GetEnumerator())
			{
				if(actionEnumerator == null)
					yield break;
				for(var i = 0; actionEnumerator.MoveNext() && i < result.Length; i++)
				{
					var resultItem = result[i];
					var action = actionEnumerator.Current;
					if(resultItem != null && action != null && resultItem.AlternativeActions != null && resultItem.AlternativeActions.Length > 0)
					{
						using (var beforeActionsEnumerator = FixActionsResults(action.BeforeActions, resultItem.BeforeActions).GetEnumerator())
							while (beforeActionsEnumerator.MoveNext())
								;
						foreach (var alternativeAction in FixActionsResults(action.AlternativeActions, resultItem.AlternativeActions))
							yield return alternativeAction;
						using (var afterActionsEnumerator = FixActionsResults(action.AfterActions, resultItem.AfterActions).GetEnumerator())
							while (afterActionsEnumerator.MoveNext())
								;
						yield break;
					}
					if(resultItem != null && action != null && resultItem.Success)
					{
						using (var beforeActionsEnumerator = FixActionsResults(action.BeforeActions, resultItem.BeforeActions).GetEnumerator())
							while (beforeActionsEnumerator.MoveNext())
								;
						var parameters = new List<PersistenceParameter>(action.Parameters);
						if(resultItem.Parameters != null)
						{
							foreach (var parameter in resultItem.Parameters)
							{
								var indexOf = parameters.FindIndex(f => f.Name == parameter.Name);
								if(indexOf >= 0)
									parameters[indexOf] = parameter;
								else
									parameters.Add(parameter);
							}
						}
						if(resultItem.Result is IDeleteActionResult)
							parameters.Add(new PersistenceParameter(DeleteActionResultParameterName, resultItem.Result));
						if(resultItem.RowVersion != 0 && !parameters.Exists(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, "RowVersion")))
							parameters.Add(new PersistenceParameter("RowVersion", resultItem.RowVersion));
						action.Parameters.Clear();
						action.Parameters.AddRange(parameters.ToArray());
						using (var afterActionsEnumerator = FixActionsResults(action.AfterActions, resultItem.AfterActions).GetEnumerator())
							while (afterActionsEnumerator.MoveNext())
								;
					}
					yield return action;
				}
			}
		}

		/// <summary>
		/// Processa as ações informadas.
		/// </summary>
		/// <param name="actions">Ações que serão processadas.</param>
		public void Process(IEnumerable<PersistenceAction> actions)
		{
			NavigateActions(actions);
		}
	}
}
