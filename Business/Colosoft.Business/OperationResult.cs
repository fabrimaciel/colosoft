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
using System.Xml.Serialization;

namespace Colosoft.Business
{
	/// <summary>
	/// Armazena o resultado de qualquer operação que necessite de retorno.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class OperationResult : IXmlSerializable, IOperationResult
	{
		/// <summary>
		/// Identifica se a operação foi executada com sucesso.
		/// </summary>
		public virtual bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Mensagem do resultado.
		/// </summary>
		public virtual IMessageFormattable Message
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public OperationResult()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success">Identifica se a operação foi realizada com sucesso.</param>
		/// <param name="message">Mensagem do resultado.</param>
		public OperationResult(bool success, IMessageFormattable message)
		{
			this.Success = success;
			this.Message = message;
		}

		/// <summary>
		/// Cria o resultado de operação para o valor informado, marcando com se a
		/// operação foi executada com sucesso.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static OperationResult<T> Create<T>(T result)
		{
			return new OperationResult<T>(result);
		}

		/// <summary>
		/// Converte implicitamente para um Boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static implicit operator bool(OperationResult value)
		{
			if(value == null)
				return false;
			return value.Success;
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			BusinessNamespace.ResolveRolePermissionSchema(xs);
			return new System.Xml.XmlQualifiedName("OperationResult", BusinessNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Lê o xml.
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Success"))
			{
				bool success;
				if(bool.TryParse(reader.ReadContentAsString(), out success))
					Success = success;
			}
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				var text = reader.ReadElementString("Message");
				if(!text.Equals(string.Empty))
					Message = text.GetFormatter();
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Escreve o xml.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Success", Success.ToString());
			writer.WriteStartElement("Message");
			writer.WriteValue(Message.FormatOrNull());
			writer.WriteEndElement();
		}
	}
	/// <summary>
	/// Representa o resultado 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class OperationResult<T> : OperationResult, IOperationResultExtended
	{
		/// <summary>
		/// Resultado.
		/// </summary>
		public T Result
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="message"></param>
		/// <param name="result"></param>
		public OperationResult(bool success, IMessageFormattable message, T result) : base(success, message)
		{
			this.Result = result;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="message"></param>
		public OperationResult(bool success, IMessageFormattable message) : base(success, message)
		{
		}

		/// <summary>
		/// Cria a instancia com o resultado da operação.
		/// </summary>
		/// <param name="result"></param>
		public OperationResult(T result) : base(true, null)
		{
			this.Result = result;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public OperationResult()
		{
		}

		/// <summary>
		/// Resultado.
		/// </summary>
		object IOperationResultExtended.Result
		{
			get
			{
				return Result;
			}
			set
			{
				Result = (T)value;
			}
		}
	}
	/// <summary>
	/// Representa o resultado de uma operação que possui um método de execução.
	/// </summary>
	/// <typeparam name="TArg"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	public class OperationResult<TArg, TResult> : OperationResult
	{
		private Func<TArg, TResult> _execute;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success">Identifica se a operação foi executa com sucesso.</param>
		/// <param name="message">Mensagem da operação.</param>
		/// <param name="execute">Comando que será usado para execução.</param>
		public OperationResult(bool success, IMessageFormattable message, Func<TArg, TResult> execute) : base(success, message)
		{
			_execute = execute;
		}

		/// <summary>
		/// Executa a operação encapsulada.
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		public TResult Execute(TArg arg)
		{
			return _execute(arg);
		}
	}
}
