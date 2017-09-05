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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação do validador das seções de persistencia usando como base o esquema de tipos.
	/// </summary>
	public class SchemaPersistenceSessionValidator : Data.IPersistenceSessionValidator
	{
		private ITypeSchema _typeSchema;

		/// <summary>
		/// Armazena a relação da propriedade identidade com o tipo informado.
		/// </summary>
		private Dictionary<string, IPropertyMetadata> _typesIdentityProperty = new Dictionary<string, IPropertyMetadata>();

		/// <summary>
		/// Tipo de esquema associado.
		/// </summary>
		protected ITypeSchema TypeSchema
		{
			get
			{
				return _typeSchema;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema"></param>
		public SchemaPersistenceSessionValidator(ITypeSchema typeSchema)
		{
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
		}

		/// <summary>
		/// Recupera a coluna identidade associada com entidade da ação de persistencia.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		protected IPropertyMetadata GetIdentityProperty(PersistenceAction action)
		{
			IPropertyMetadata property = null;
			lock (_typesIdentityProperty)
				if(_typesIdentityProperty.TryGetValue(action.EntityFullName, out property))
					return property;
			var typeMetadata = TypeSchema.GetTypeMetadata(action.EntityFullName);
			if(typeMetadata != null)
				property = typeMetadata.Where(f => f.ParameterType == PersistenceParameterType.IdentityKey).FirstOrDefault();
			lock (_typesIdentityProperty)
				if(!_typesIdentityProperty.ContainsKey(action.EntityFullName))
					_typesIdentityProperty.Add(action.EntityFullName, property);
			return property;
		}

		/// <summary>
		/// Navega pelas as ações.
		/// </summary>
		/// <param name="action">Instancia da ação.</param>
		/// <param name="state">Estado que está sendo usado na validação.</param>
		/// <param name="result">Resultado da validação.</param>
		private void NavigatePersistenceAction(PersistenceAction action, ValidationState state, PersistenceSessionValidateResult result)
		{
			if(action.Type == PersistenceActionType.Insert)
			{
				int virtualId = 0;
				PersistenceAction duplicateAction = null;
				IMessageFormattable message = null;
				if(!state.ValidateInsertionAction(action, out virtualId, out duplicateAction, out message))
				{
					result.AddError(action, message);
				}
			}
			foreach (var i in action.BeforeActions)
				NavigatePersistenceAction(i, state, result);
			foreach (var i in action.AfterActions)
				NavigatePersistenceAction(i, state, result);
		}

		/// <summary>
		/// Realiza a validaçao da sessão informada.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public PersistenceSessionValidateResult Validate(PersistenceSession session)
		{
			var state = new ValidationState(this);
			var result = new PersistenceSessionValidateResult();
			foreach (var action in session)
				NavigatePersistenceAction(action, state, result);
			return result;
		}

		/// <summary>
		/// Classe que armanzena os dados validação.
		/// </summary>
		class ValidationState
		{
			private SchemaPersistenceSessionValidator _validator;

			/// <summary>
			/// Armazena a relação das ações e os Uids da instancia que estão sendo inseridas.
			/// </summary>
			private Dictionary<int, Data.PersistenceAction> _insertActions = new Dictionary<int, PersistenceAction>();

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="validator"></param>
			public ValidationState(SchemaPersistenceSessionValidator validator)
			{
				_validator = validator;
			}

			/// <summary>
			/// Valida a ações de inserção.
			/// </summary>
			/// <param name="action"></param>
			/// <param name="virtualId"></param>
			/// <param name="duplicateAction">Instancia da ação duplicada.</param>
			/// <param name="message">Mensagem de retorno.</param>
			/// <returns></returns>
			public bool ValidateInsertionAction(Data.PersistenceAction action, out int virtualId, out PersistenceAction duplicateAction, out IMessageFormattable message)
			{
				var identityProperty = _validator.GetIdentityProperty(action);
				if(identityProperty == null)
				{
					duplicateAction = null;
					virtualId = 0;
					message = null;
					return true;
				}
				var identityParameter = action.Parameters.Where(f => f.Name == identityProperty.Name).FirstOrDefault();
				if(identityParameter != null)
				{
					object value = identityParameter.Value;
					if(value is int)
						virtualId = (int)value;
					else if(value is uint)
						virtualId = (int)(uint)value;
					else
					{
						var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(int));
						if(converter == null && !converter.CanConvertFrom(value.GetType()))
						{
							duplicateAction = null;
							virtualId = 0;
							message = null;
							return true;
						}
						try
						{
							virtualId = (int)converter.ConvertFrom(value);
						}
						catch(NotSupportedException)
						{
							duplicateAction = null;
							virtualId = 0;
							message = null;
							return true;
						}
					}
					if(virtualId > 0)
					{
						message = ResourceMessageFormatter.Create(() => Properties.Resources.SchemaPersistenceSessionValidator_NonNegativeIdentityProperty, virtualId, identityProperty.Name, action.EntityFullName);
						duplicateAction = null;
						return false;
					}
					if(_insertActions.TryGetValue(virtualId, out duplicateAction))
					{
						message = ResourceMessageFormatter.Create(() => Properties.Resources.SchemaPersistenceSessionValidator_DuplicateInsertionAction, action.EntityFullName, virtualId);
						return false;
					}
					_insertActions.Add(virtualId, action);
				}
				else
					virtualId = 0;
				message = null;
				duplicateAction = null;
				return true;
			}
		}
	}
}
