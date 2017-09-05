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
	/// Implementação padrão da interface <see cref="IValidationTypeFactory"/>.
	/// </summary>
	public class ValidationTypeFactory : IValidationTypeFactory, IEnumerable<KeyValuePair<ITypeDefinition, Func<IValidatorCreator>>>
	{
		private Dictionary<ITypeDefinition, Func<IValidatorCreator>> _validators;

		/// <summary>
		/// Recupera a quantidade de validações agrupadas.
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
		public ValidationTypeFactory()
		{
			_validators = new Dictionary<ITypeDefinition, Func<IValidatorCreator>>(new TypeDefinitionComparer());
			Add(typeof(Validators.StringLengthValidation), System.Globalization.CultureInfo.InvariantCulture);
			Add(typeof(Validators.NotNullValidator), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Cria o tipo da validação pela definição do tipo informada.
		/// </summary>
		/// <param name="typeDefinition"></param>
		/// <returns></returns>
		public IValidatorCreator GetCreator(ITypeDefinition typeDefinition)
		{
			Func<IValidatorCreator> validator = null;
			if(_validators.TryGetValue(typeDefinition, out validator))
				return validator();
			return null;
		}

		/// <summary>
		/// Adiciona uma informação de um tipo de validação.
		/// </summary>
		/// <param name="typeDefinition"></param>
		/// <param name="validator"></param>
		public void Add(ITypeDefinition typeDefinition, Func<IValidatorCreator> validator)
		{
			_validators.Add(typeDefinition, validator);
		}

		/// <summary>
		/// Adiciona um novo tipo de validador para a factory.
		/// </summary>
		/// <param name="validatorType"></param>
		/// <param name="cultureInfo"></param>
		public void Add(Type validatorType, System.Globalization.CultureInfo cultureInfo)
		{
			validatorType.Require("validatorType").NotNull();
			Add(TypeDefinition.Get(validatorType), ValidatorCreator.CreateLazyCreator(validatorType, cultureInfo));
		}

		/// <summary>
		/// Recupera o enumerator dos tipos.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<ITypeDefinition, Func<IValidatorCreator>>> GetEnumerator()
		{
			return _validators.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos tipos de validação.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _validators.GetEnumerator();
		}
	}
}
