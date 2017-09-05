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
using Colosoft.Serialization;
using System.Collections;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching
{
	/// <summary>
	/// Código da operação.
	/// </summary>
	public enum OpCode
	{
		/// <summary>
		/// Operação de adição.
		/// </summary>
		Add,
		/// <summary>
		/// Operação de atualização.
		/// </summary>
		Update,
		/// <summary>
		/// Operação de remoção.
		/// </summary>
		Remove,
		/// <summary>
		/// Operação de limpeza.
		/// </summary>
		Clear
	}
	/// <summary>
	/// Nomes dos campos de contexto de operação.
	/// </summary>
	public enum OperationContextFieldName
	{
		/// <summary>
		/// Tipo de operação.
		/// </summary>
		OperationType,
		/// <summary>
		/// Dispara um notificação de continuos query.
		/// </summary>
		RaiseCQNotification,
		/// <summary>
		/// 
		/// </summary>
		ReadThru,
		/// <summary>
		/// 
		/// </summary>
		ReadThruProviderName,
		/// <summary>
		/// 
		/// </summary>
		ClientLastViewId,
		/// <summary>
		/// Recipiente identado.
		/// </summary>
		IntendedRecipient
	}
	/// <summary>
	/// Tipos de operação do contexto.
	/// </summary>
	public enum OperationContextOperationType
	{
		/// <summary>
		/// Operação de cache.
		/// </summary>
		CacheOperation = 1
	}
	/// <summary>
	/// Representa um contexto de operação.
	/// </summary>
	[Serializable]
	public class OperationContext : ICompactSerializable, ICloneable
	{
		private Hashtable _fieldValueTable;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public OperationContext()
		{
		}

		/// <summary>
		/// Cria uma nova instancia já defininod o valor para um campo.
		/// </summary>
		/// <param name="fieldName">Nome do campo.</param>
		/// <param name="fieldValue">Valor do campo.</param>
		public OperationContext(OperationContextFieldName fieldName, object fieldValue)
		{
			this.Add(fieldName, fieldValue);
		}

		/// <summary>
		/// Adiciona um novo campo para o contexto.
		/// </summary>
		/// <param name="fieldName">Nome do campo.</param>
		/// <param name="fieldValue">Valor do campo.</param>
		public void Add(OperationContextFieldName fieldName, object fieldValue)
		{
			lock (this)
			{
				if(_fieldValueTable == null)
					_fieldValueTable = new Hashtable();
				_fieldValueTable[fieldName] = fieldValue;
			}
		}

		/// <summary>
		/// Cria um clone da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			OperationContext context = new OperationContext();
			lock (this)
				context._fieldValueTable = _fieldValueTable;
			return context;
		}

		/// <summary>
		/// Verifica se no contexto existe algum valor para o nome de campo informado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public bool Contains(OperationContextFieldName fieldName)
		{
			bool flag = false;
			if(_fieldValueTable != null)
				flag = _fieldValueTable.Contains(fieldName);
			return flag;
		}

		/// <summary>
		/// Recupera o valor pelo nome do campo informado.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public object GetValueByField(OperationContextFieldName fieldName)
		{
			object obj2 = null;
			if(_fieldValueTable != null)
				obj2 = _fieldValueTable[fieldName];
			return obj2;
		}

		/// <summary>
		/// Verifica se o contexto é um operação do tipo informado.
		/// </summary>
		/// <param name="operationType"></param>
		/// <returns></returns>
		public bool IsOperation(OperationContextOperationType operationType)
		{
			return (((OperationContextOperationType)this.GetValueByField(OperationContextFieldName.OperationType)) == operationType);
		}

		/// <summary>
		/// Remove o valor do campo com o nome informado.
		/// </summary>
		/// <param name="fieldName">Nome do campo que será removido.</param>
		public void RemoveValueByField(OperationContextFieldName fieldName)
		{
			if(_fieldValueTable != null)
				_fieldValueTable.Remove(fieldName);
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_fieldValueTable);
		}

		/// <summary>
		/// Deserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_fieldValueTable = (Hashtable)reader.ReadObject();
		}
	}
}
