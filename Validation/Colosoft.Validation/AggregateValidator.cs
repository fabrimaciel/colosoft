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
	/// Implementação de uma agregador de validadores.
	/// </summary>
	public class AggregateValidator : IValidator, IEquatable<IValidator>
	{
		private List<IValidator> _validators = new List<IValidator>();

		/// <summary>
		/// Nome do validador.
		/// </summary>
		public string FullName
		{
			get
			{
				return "AggregateValidator";
			}
		}

		/// <summary>
		/// Modelo de mensagem padrão.
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
		/// Quantidade de validadores agregados.
		/// </summary>
		public int Count
		{
			get
			{
				return _validators.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AggregateValidator()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="validators">Relação dos validadores que serão agregados.</param>
		public AggregateValidator(IEnumerable<IValidator> validators)
		{
			if(validators != null)
				_validators.AddRange(validators);
		}

		/// <summary>
		/// Adiciona um novo validador para a agregação.
		/// </summary>
		/// <param name="validator"></param>
		public void Add(IValidator validator)
		{
			validator.Require("validator").NotNull();
			_validators.Add(validator);
		}

		/// <summary>
		/// Realiza a validação.
		/// </summary>
		/// <param name="currentTarget"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyLabel"></param>
		/// <param name="objectToValidate"></param>
		/// <param name="validationResults"></param>
		/// <param name="messageProvider"></param>
		public void DoValidate(object currentTarget, string propertyName, IPropertyLabel propertyLabel, object objectToValidate, ValidationResult validationResults, IValidationMessageProvider messageProvider)
		{
			foreach (var validator in _validators)
				validator.DoValidate(currentTarget, propertyName, propertyLabel, objectToValidate, validationResults, messageProvider);
		}

		/// <summary>
		/// Compara com a outra instância.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IValidator other)
		{
			var asSame = other as AggregateValidator;
			return (asSame != null) && Enumerable.SequenceEqual(_validators, asSame._validators);
		}
	}
}
