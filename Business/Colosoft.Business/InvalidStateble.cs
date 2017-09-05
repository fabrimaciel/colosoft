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
	/// Representa um Stateble inválido;
	/// </summary>
	sealed class InvalidStateble : Validation.IStateble
	{
		private Colosoft.Validation.StatebleParameterCollection _parameters;

		/// <summary>
		/// Instancia única da classe.
		/// </summary>
		public static InvalidStateble Instance = new InvalidStateble();

		/// <summary>
		/// Evento acionado quando o estado for alterado.
		/// </summary>
		public event Validation.StateChangedEventHandler StateChanged {
			add
			{
			}
			remove {
			}
		}

		/// <summary>
		/// Método acionado quando uma propriedade for alterada.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged {
			add
			{
			}
			remove {
			}
		}

		/// <summary>
		/// Tipo do item.
		/// </summary>
		public Reflection.TypeName Type
		{
			get
			{
				return Reflection.TypeName.Get<InvalidStateble>();
			}
		}

		/// <summary>
		/// Identifica se a instancia é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Parametros.
		/// </summary>
		public Colosoft.Validation.StatebleParameterCollection Parameters
		{
			get
			{
				if(_parameters == null)
					_parameters = new Validation.StatebleParameterCollection();
				return _parameters;
			}
		}

		/// <summary>
		/// Construtor privado.
		/// </summary>
		private InvalidStateble()
		{
		}

		/// <summary>
		/// Limpa o estado do cache.
		/// </summary>
		public void ClearStateCache()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Notifica a alteração de uma propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		public void NotifyStatePropertyChanged(string propertyName)
		{
		}

		/// <summary>
		/// Recarrega as configurações.
		/// </summary>
		public void ReloadTypeSettings()
		{
		}

		/// <summary>
		/// Recupera o item associado com a nome da propriedade.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public Validation.IStatebleItem this[string propertyName]
		{
			get
			{
				return InvalidStatebleItem.Instance;
			}
		}

		/// <summary>
		/// Recuppera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Validation.IStatebleItem> GetEnumerator()
		{
			yield break;
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			yield break;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Implementação para um item inválido.
		/// </summary>
		class InvalidStatebleItem : Colosoft.Validation.IStatebleItem
		{
			public static InvalidStatebleItem Instance = new InvalidStatebleItem();

			public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged {
				add
				{
				}
				remove {
				}
			}

			public bool CopyValue
			{
				get
				{
					return false;
				}
			}

			public string Identifier
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public bool IsConfigured
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public bool IsEnabled
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public bool IsHidden
			{
				get
				{
					return true;
				}
				set
				{
				}
			}

			public bool IsNecessary
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
				set
				{
				}
			}

			public bool IsRequired
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public bool IsSorted
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public Validation.IPropertyLabel Label
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.InputRulesOptions Options
			{
				get
				{
					return Colosoft.Validation.InputRulesOptions.Disable | Colosoft.Validation.InputRulesOptions.Hidden | Colosoft.Validation.InputRulesOptions.ReadOnly;
				}
			}

			public Validation.IStateble Owner
			{
				get
				{
					return null;
				}
			}

			public Validation.IParse Parser
			{
				get
				{
					return null;
				}
			}

			public string PropertyName
			{
				get
				{
					return null;
				}
			}

			public bool ReloadSettings
			{
				get
				{
					return false;
				}
			}

			public Validation.IValidator Validation
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.StatebleItemVisibility Visibility
			{
				get
				{
					return Colosoft.Validation.StatebleItemVisibility.Hidden;
				}
				set
				{
				}
			}

			public Validation.CharacterCase CharCase
			{
				get
				{
					return Colosoft.Validation.CharacterCase.Undefined;
				}
				set
				{
				}
			}

			public Validation.ICheckDigits CheckDigits
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.IInputValidateCustomization Customization
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.IDefaultValue DefaultValue
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public IEnumerable<Validation.IIndexedValue> IndexedValues
			{
				get
				{
					return new Validation.IIndexedValue[0];
				}
				set
				{
				}
			}

			public Validation.ILength Length
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.IMask Mask
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.IRange Range
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public Validation.IValidChars ValidChars
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			public IEnumerable<Validation.IPropertyValue> Values
			{
				get
				{
					return new Validation.IPropertyValue[0];
				}
				set
				{
				}
			}

			public Validation.IStatebleItem CopyFrom(Validation.IStatebleItem instance)
			{
				return instance;
			}

			public bool IsEmpty()
			{
				return true;
			}

			public object Clone()
			{
				return this;
			}
		}
	}
}
