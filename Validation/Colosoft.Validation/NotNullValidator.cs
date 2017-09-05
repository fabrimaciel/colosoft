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
	/// Implementação do validador de valor não nulo..
	/// </summary>
	public class NotNullValidator : IValidator, IEquatable<IValidator>
	{
		/// <summary>
		/// Nome do validador.
		/// </summary>
		public string FullName
		{
			get
			{
				return "NotNullValue";
			}
		}

		/// <summary>
		/// Mensagem padrão.
		/// </summary>
		public IMessageFormattable DefaultMessageTemplate
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Parametros de retorno.
		/// </summary>
		public string ReturnedParameters
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsExclusiveInList
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Aplica a validação.
		/// </summary>
		/// <param name="currentTarget"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyLabel"></param>
		/// <param name="objectToValidate"></param>
		/// <param name="validationResults"></param>
		/// <param name="messageProvider"></param>
		public void DoValidate(object currentTarget, string propertyName, IPropertyLabel propertyLabel, object objectToValidate, ValidationResult validationResults, IValidationMessageProvider messageProvider)
		{
			if(objectToValidate == null)
				validationResults.Invalid(ValidationResultType.Error, ResourceMessageFormatter.Create(() => Properties.Resources.Validators_NotNullValidator_MessageTemplate, ((propertyLabel != null) && (propertyLabel.Title != null) && (!String.IsNullOrWhiteSpace(propertyLabel.Title.Format()))) ? propertyLabel.Title : propertyName.GetFormatter(), ValidatorsHelper.GetCurrentTargetName(currentTarget)));
		}

		/// <summary>
		/// Compara com a outra instância.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IValidator other)
		{
			var asSame = other as NotNullValidator;
			return (asSame != null);
		}
	}
}
