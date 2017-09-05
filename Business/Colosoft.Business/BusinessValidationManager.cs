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
	/// Implementação básica do gerenciador de validação.
	/// </summary>
	public class BusinessValidationManager : Validation.ValidationManager
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger"></param>
		public BusinessValidationManager(Colosoft.Logging.ILogger logger) : base(logger)
		{
		}

		/// <summary>
		/// Cria o estado da propriedade.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="propertyName"></param>
		/// <param name="information"></param>
		/// <param name="uiContext"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public override Validation.IStatebleItem CreatePropertyState(Validation.IStateble owner, string propertyName, Validation.IPropertySettingsInfo information, string uiContext, System.Globalization.CultureInfo culture)
		{
			return new BusinessEntityPropertyState(owner, propertyName);
		}
	}
}
