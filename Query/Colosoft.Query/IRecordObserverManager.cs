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
	/// Assinatura da classe responsável por notificar 
	/// os observer de uma alteração no registro.
	/// </summary>
	public interface IRecordObserverNotifier
	{
		/// <summary>
		/// Identifica se o notificador é valido.
		/// </summary>
		bool IsValid
		{
			get;
		}

		/// <summary>
		/// Notifica a alteração que contém os dados no registro informado.
		/// </summary>
		/// <param name="record"></param>
		void Notify(IRecord record);
	}
	/// <summary>
	/// Possíveis alterações para o registro de um tipo.
	/// </summary>
	public enum TypeRecordChangeType
	{
		/// <summary>
		/// Identifica que um registro foi inserido.
		/// </summary>
		Insert,
		/// <summary>
		/// Identifica que um registro foi apagado.
		/// </summary>
		Delete,
		/// <summary>
		/// Identifica que um registro foi atualizado.
		/// </summary>
		Update
	}
	/// <summary>
	/// Armazena os argumentos do evento acionado quando o registro 
	/// de um determinado tipo for alterado.
	/// </summary>
	public class TypeRecordChangedEventArgs : EventArgs
	{
		private Colosoft.Reflection.TypeName _typeName;

		private TypeRecordChangeType _changeType;

		private IRecord _record;

		private RecordKey _recordKey;

		/// <summary>
		/// Nome do tipo no qual os dados sofreram alteração.
		/// </summary>
		public Colosoft.Reflection.TypeName TypeName
		{
			get
			{
				return _typeName;
			}
		}

		/// <summary>
		/// Tipo de alteração.
		/// </summary>
		public TypeRecordChangeType ChangeType
		{
			get
			{
				return _changeType;
			}
		}

		/// <summary>
		/// Instancia do registro que está sofrendo a alteração.
		/// </summary>
		public IRecord Record
		{
			get
			{
				return _record;
			}
		}

		/// <summary>
		/// Chave do registro que está sofrendo alteração.
		/// </summary>
		public RecordKey RecordKey
		{
			get
			{
				return _recordKey;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="changeType"></param>
		/// <param name="record"></param>
		/// <param name="recordKey"></param>
		public TypeRecordChangedEventArgs(Colosoft.Reflection.TypeName typeName, TypeRecordChangeType changeType, IRecord record, RecordKey recordKey)
		{
			_typeName = typeName;
			_changeType = changeType;
			_record = record;
			_recordKey = recordKey;
		}
	}
	/// <summary>
	/// Assinatura do gerenciador dos observers dos registros de dados.
	/// </summary>
	public interface IRecordObserverManager
	{
		/// <summary>
		/// Evento acionado quando o registro de um terminado tipo sofrer alguma alteração.
		/// </summary>
		event EventHandler<TypeRecordChangedEventArgs> TypeRecordChanged;

		/// <summary>
		/// Quantidade de observers de registros registrados.
		/// </summary>
		int RecordObserversCount
		{
			get;
		}

		/// <summary>
		/// Quantidade de observers de resultado de consultas registradas.
		/// </summary>
		int QueryResultObserverCount
		{
			get;
		}

		/// <summary>
		/// Identifica que o gerenciador de observers de registros está habilitado.
		/// </summary>
		bool IsEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// Adicionar o tipo que deve ser ignorado pelo gerenciador.
		/// </summary>
		/// <param name="typeNames"></param>
		void IgnoreTypes(params Colosoft.Reflection.TypeName[] typeNames);

		/// <summary>
		/// Recupera as descrição das entradas dos tipos registrados.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetTypeEntryDescriptions();

		/// <summary>
		/// Registra o observer para o registro informado. Toda
		/// vez que ocorrer alguma alteração que implique sobre os
		/// dados do registro o observer será notificado.
		/// </summary>
		/// <param name="typeName">Nome do tipo no qual o registro representa.</param>
		/// <param name="recordKey">Chave que representa o registro.</param>
		/// <param name="observer">Instancia do observer que reberá as notificações.</param>
		void Register(Colosoft.Reflection.TypeName typeName, RecordKey recordKey, IRecordObserver observer);

		/// <summary>
		/// Registra o observer para o resultado da consulta. Toda vez 
		/// que ocorrer alguma alteração que implique sobre os dados
		/// do resultado o observer será notificado.
		/// </summary>
		/// <param name="typeName">Nome do tipo no qual o resultado se basea.</param>
		/// <param name="observer">Instancia do observer.</param>
		void Register(Colosoft.Reflection.TypeName typeName, IQueryResultChangedObserver observer);

		/// <summary>
		/// Remove o registro do observer.
		/// </summary>
		/// <param name="typeName">Nome do tipo no qual o observer está associado.</param>
		/// <param name="observer"></param>
		void Unregister(Colosoft.Reflection.TypeName typeName, IRecordObserver observer);

		/// <summary>
		/// Remove o registro do observer.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="observer"></param>
		void Unregister(Colosoft.Reflection.TypeName typeName, IQueryResultChangedObserver observer);

		/// <summary>
		/// Notifica que os registros foram apagados.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="recordKeys"></param>
		void NotifyRecordDeleted(Colosoft.Reflection.TypeName typeName, IEnumerable<RecordKey> recordKeys);

		/// <summary>
		/// Notific que o registro foram inseridos.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="records"></param>
		void NotifyRecordsInserted(Colosoft.Reflection.TypeName typeName, IEnumerable<IRecord> records);

		/// <summary>
		/// Recupera o notificador de alterações no registro.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		IRecordObserverNotifier GetRecordChangedNotifier(Colosoft.Reflection.TypeName typeName, RecordKey recordKey);
	}
}
