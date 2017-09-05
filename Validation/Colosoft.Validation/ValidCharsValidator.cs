﻿/* 
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

namespace Colosoft.Validation.Validators
{
	/// <summary>
	/// Validação de caracteres válidos.
	/// </summary>
	public class ValidCharsValidator : Colosoft.Validation.IValidator, IEquatable<IValidator>
	{
		private Colosoft.Validation.IValidChars _test;

		/// <summary>
		/// Dados de verificação.
		/// </summary>
		public Colosoft.Validation.IValidChars Test
		{
			get
			{
				return _test;
			}
		}

		/// <summary>
		/// Construtor parametrizado.
		/// </summary>
		/// <param name="charsValue">Dados de verificação.</param>
		public ValidCharsValidator(Colosoft.Validation.IValidChars charsValue)
		{
			_test = charsValue;
		}

		/// <summary>
		/// Mensagem padrão a ser apresentada ao cliente.
		/// </summary>
		public IMessageFormattable DefaultMessageTemplate
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Nome do validador.
		/// </summary>
		public string FullName
		{
			get
			{
				return "ValidChars";
			}
		}

		/// <summary>
		/// Indica que a validação é exclusiva na lista.
		/// </summary>
		public bool IsExclusiveInList
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Descrição dos parâmetros que são retornados em caso de erro caso existam.
		/// </summary>
		public string ReturnedParameters
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Realiza a validação do objeto.
		/// </summary>
		/// <param name="currentTarget">Objeto Alvo da validação.</param>
		/// <param name="propertyName">Nome da propriedade que está sendo validada.</param>
		/// <param name="propertyLabel">Label da propriedade que está sendo validada.</param>
		/// <param name="objectToValidate">Valor a ser validado.</param>
		/// <param name="validationResults">Objeto com os resultados das validações.</param>
		/// <param name="messageProvider">Fluxo para a obtenção de mensagem de erro.</param>
		public void DoValidate(object currentTarget, string propertyName, Colosoft.Validation.IPropertyLabel propertyLabel, object objectToValidate, Colosoft.Validation.ValidationResult validationResults, Colosoft.Validation.IValidationMessageProvider messageProvider)
		{
			var input = objectToValidate as string;
			if((_test != null) && (!String.IsNullOrEmpty(input)) && (!input.All(c => _test.ValidChars.Contains(c))))
			{
				validationResults.Invalid(Colosoft.Validation.ValidationResultType.Error, ResourceMessageFormatter.Create(() => Properties.Resources.Validators_ValidCharsValidator_MessageTemplate, ((propertyLabel != null) && (propertyLabel.Title != null) && (!String.IsNullOrWhiteSpace(propertyLabel.Title.Format()))) ? propertyLabel.Title : propertyName.GetFormatter(), ValidatorsHelper.GetCurrentTargetName(currentTarget)));
			}
		}

		/// <summary>
		/// Compara com a outra instância.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IValidator other)
		{
			var asSame = other as ValidCharsValidator;
			return (asSame != null) && _test.GetType().Equals(asSame._test.GetType()) && String.Equals(_test.ValidChars, asSame._test.ValidChars);
		}
	}
}
