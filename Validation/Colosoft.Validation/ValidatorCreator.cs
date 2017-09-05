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
	/// Criador da validador.
	/// </summary>
	public class ValidatorCreator : IValidatorCreator
	{
		private Type _validatorType;

		private System.Reflection.ConstructorInfo _constructor;

		private System.Reflection.ParameterInfo[] _parameters;

		private string[] _parameterNames;

		private System.Globalization.CultureInfo _cultureInfo;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="validatorType">Tipo do validador associado.</param>
		/// <param name="cultureInfo"></param>
		public ValidatorCreator(Type validatorType, System.Globalization.CultureInfo cultureInfo)
		{
			validatorType.Require("validatorType").NotNull();
			_validatorType = validatorType;
			_cultureInfo = cultureInfo;
			_constructor = validatorType.GetConstructors().FirstOrDefault();
			_parameters = _constructor.GetParameters();
			_parameterNames = _parameters.Select(f => char.ToUpper(f.Name[0]) + f.Name.Substring(1)).ToArray();
		}

		/// <summary>
		/// Cria o objeto Data de um IValidator
		/// </summary>
		/// <param name="parameters">Lista de parâmetros para a criação</param>
		/// <param name="cultureInfo">Cultura que será usada na criação do validador.</param>
		/// <param name="resultType">Tipo de resultado que será retornado </param>
		/// <returns>Objeto Data</returns> 
		public IValidator CreateValidator(IEnumerable<ParameterValue> parameters, System.Globalization.CultureInfo cultureInfo, ValidationResultType resultType = ValidationResultType.Error)
		{
			object[] values = _parameters.Select(f => f.IsOptional ? f.DefaultValue : null).ToArray();
			foreach (var i in parameters)
			{
				var index = Array.FindIndex(_parameterNames, f => f == i.Name);
				if(index >= 0)
				{
					var parameter = _parameters[index];
					var converter = System.ComponentModel.TypeDescriptor.GetConverter(parameter.ParameterType);
					object value = null;
					if(converter != null && converter.CanConvertFrom(typeof(string)))
					{
						try
						{
							value = converter.ConvertFrom(null, cultureInfo ?? _cultureInfo, i.Value);
						}
						catch(Exception ex)
						{
							throw new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidatorCreator_FailOnConvertParameterValue, i.Value, i.Name).Format(), ex);
						}
					}
					else
						throw new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidatorCreator_FailOnConvertParameterValue, i.Value, i.Name).Format());
					values[index] = value;
				}
			}
			IValidator result = null;
			try
			{
				result = (IValidator)Activator.CreateInstance(_validatorType, values);
			}
			catch(Exception ex)
			{
				throw new NotSupportedException(ResourceMessageFormatter.Create(() => Properties.Resources.ValidatorCreator_FailOnCreateValidator, _validatorType.FullName).Format(), ex);
			}
			return result;
		}

		/// <summary>
		/// Cria um criador de <see cref="IValidator"/>.
		/// </summary>
		/// <param name="validatorType">Tipo do validador.</param>
		/// <param name="cultureInfo"></param>
		/// <returns></returns>
		public static Func<IValidatorCreator> CreateLazyCreator(Type validatorType, System.Globalization.CultureInfo cultureInfo)
		{
			return () => CreateCreator(validatorType, cultureInfo);
		}

		/// <summary>
		///  Cria um criador de <see cref="IValidator"/>.
		/// </summary>
		/// <param name="validatiorType">Tipo do validador.</param>
		/// <param name="cultureInfo"></param>
		/// <returns></returns>
		public static IValidatorCreator CreateCreator(Type validatiorType, System.Globalization.CultureInfo cultureInfo)
		{
			validatiorType.Require("validatiorType").NotNull();
			return new ValidatorCreator(validatiorType, cultureInfo);
		}
	}
}
