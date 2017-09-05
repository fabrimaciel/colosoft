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

namespace Colosoft.Validation.Validators
{
	/// <summary>
	/// Validador do tamanho de uma string.
	/// </summary>
	public class StringLengthValidation : IValidator, IEquatable<IValidator>
	{
		private int _length;

		private bool _trimString;

		/// <summary>
		/// Nome do validador.
		/// </summary>
		public string FullName
		{
			get
			{
				return "StringLengthValidation";
			}
		}

		/// <summary>
		/// Modelo de mensagem padrão.
		/// </summary>
		public IMessageFormattable DefaultMessageTemplate
		{
			get
			{
				return "".GetFormatter();
			}
		}

		/// <summary>
		/// Descrição dos parâmetros que são retornados em caso de erro caso existam
		/// </summary>
		public string ReturnedParameters
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Indica que a validação é exclusiva na lista
		/// </summary>
		public bool IsExclusiveInList
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="trimString"></param>
		public StringLengthValidation(int length, bool trimString)
		{
			_length = length;
			_trimString = trimString;
		}

		/// <summary>
		/// Valida.
		/// </summary>
		/// <param name="currentTarget"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyLabel"></param>
		/// <param name="objectToValidate"></param>
		/// <param name="validationResults"></param>
		/// <param name="messageProvider"></param>
		public void DoValidate(object currentTarget, string propertyName, IPropertyLabel propertyLabel, object objectToValidate, ValidationResult validationResults, IValidationMessageProvider messageProvider)
		{
			var text = (objectToValidate ?? "").ToString();
			if(_trimString)
				text = text.Trim();
			if(text.Length > _length)
				validationResults.Invalid(ValidationResultType.Error, ResourceMessageFormatter.Create(() => Properties.Resources.Validators_StringLengthValidation_MessageTemplate, ((propertyLabel != null) && (propertyLabel.Title != null) && (!String.IsNullOrWhiteSpace(propertyLabel.Title.Format()))) ? propertyLabel.Title : propertyName.GetFormatter(), ValidatorsHelper.GetCurrentTargetName(currentTarget)));
		}

		/// <summary>
		/// Compara com a outra instância.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IValidator other)
		{
			var asSame = other as StringLengthValidation;
			return (asSame != null) && (_length == asSame._length) && (_trimString == asSame._trimString);
		}
	}
}
