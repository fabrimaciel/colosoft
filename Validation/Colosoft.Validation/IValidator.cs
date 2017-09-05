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

namespace Colosoft.Validation
{
	/// <summary>
	/// Contrato que representa um objeto de validação
	/// </summary>
	public interface IValidator
	{
		/// <summary>
		/// Nome do validador.
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Mensagem padrão a ser apresentada ao cliente
		/// </summary>
		IMessageFormattable DefaultMessageTemplate
		{
			get;
		}

		/// <summary>
		/// Descrição dos parâmetros que são retornados em caso de erro caso existam
		/// </summary>
		string ReturnedParameters
		{
			get;
		}

		/// <summary>
		/// Indica que a validação é exclusiva na lista
		/// </summary>
		bool IsExclusiveInList
		{
			get;
		}

		/// <summary>
		/// Realiza a validação do objeto
		/// </summary>
		/// <param name="currentTarget">Objeto Alvo da validação</param>
		/// <param name="propertyName">Nome da propriedade que está sendo validada.</param>
		/// <param name="propertyLabel">Label da propriedade está sendo validada.</param>
		/// <param name="objectToValidate">Objeto a ser validado</param>
		/// <param name="validationResults">Objeto com os registros de resultado da validação</param>
		/// <param name="messageProvider">Fluxo para a obtenção de mensagem de erro</param>
		void DoValidate(object currentTarget, string propertyName, IPropertyLabel propertyLabel, object objectToValidate, ValidationResult validationResults, IValidationMessageProvider messageProvider);
	}
}
