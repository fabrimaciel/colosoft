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
	/// Representa um exception de uma operação de negócio.
	/// </summary>
	[Serializable]
	public class BusinessException : Colosoft.DetailsException
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Instancia da mensagem formatável.</param>
		public BusinessException(IMessageFormattable message) : base(message)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message">Instancia da mensagem formatável.</param>
		/// <param name="innerException"></param>
		public BusinessException(IMessageFormattable message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor de deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected BusinessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
	/// <summary>
	/// Representa uma exception da operação de save.
	/// </summary>
	[Serializable]
	public class SaveResultException : BusinessException
	{
		private SaveResult _saveResult;

		/// <summary>
		/// Resultado.
		/// </summary>
		public SaveResult Result
		{
			get
			{
				return _saveResult;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result"></param>
		public SaveResultException(SaveResult result) : base(result.Message)
		{
		}

		/// <summary>
		/// Construtor de deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SaveResultException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			_saveResult = new SaveResult(false, MessageFormattable ?? Colosoft.MessageFormattable.Empty);
		}
	}
	/// <summary>
	/// Representa uma exception da operação de delete.
	/// </summary>
	[Serializable]
	public class DeleteResultException : BusinessException
	{
		private DeleteResult _deleteResult;

		/// <summary>
		/// Resultado.
		/// </summary>
		public DeleteResult Result
		{
			get
			{
				return _deleteResult;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result"></param>
		public DeleteResultException(DeleteResult result) : base(result.Message)
		{
		}

		/// <summary>
		/// Construtor de deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DeleteResultException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			_deleteResult = new DeleteResult(false, MessageFormattable ?? Colosoft.MessageFormattable.Empty);
		}
	}
	/// <summary>
	/// Repreesnta uma exceção do resultado de uma operação.
	/// </summary>
	[Serializable]
	public class OperationResultException : BusinessException
	{
		private OperationResult _operationResult;

		/// <summary>
		/// Resultado.
		/// </summary>
		public OperationResult Result
		{
			get
			{
				return _operationResult;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="result"></param>
		public OperationResultException(OperationResult result) : base(result.Message)
		{
		}

		/// <summary>
		/// Construtor de deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected OperationResultException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			_operationResult = new OperationResult(false, MessageFormattable ?? Colosoft.MessageFormattable.Empty);
		}
	}
}
