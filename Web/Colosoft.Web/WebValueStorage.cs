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

namespace Colosoft.Web
{
	/// <summary>
	/// Implementação do Storage para web.
	/// </summary>
	public class WebValueStorage : Colosoft.Runtime.IRuntimeValueStorage
	{
		/// <summary>
		/// Identifica se está em uma requisição Http.
		/// </summary>
		public static bool IsInHttpContext
		{
			get
			{
				return (object)System.Web.HttpContext.Current != null;
			}
		}

		/// <summary>
		/// Recupera o valor pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetValue(string name)
		{
			return Colosoft.Runtime.RuntimeValueStorage.Default.GetValue(name);
		}

		/// <summary>
		/// Define o valor associado com o nome.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(string name, object value)
		{
			Colosoft.Runtime.RuntimeValueStorage.Default.SetValue(name, value);
		}

		/// <summary>
		/// Remove o valor pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		public void RemoveValue(string name)
		{
			var value = Colosoft.Runtime.RuntimeValueStorage.Default.GetValue(name);
			if(value is IDisposable)
				((IDisposable)value).Dispose();
			Colosoft.Runtime.RuntimeValueStorage.Default.RemoveValue(name);
		}

		/// <summary>
		/// Extensão do container.
		/// </summary>
		class ContainerExtension : System.ServiceModel.IExtension<System.ServiceModel.OperationContext>
		{
			private System.Collections.Hashtable _items = new System.Collections.Hashtable();

			/// <summary>
			/// Relação dos itens.
			/// </summary>
			public System.Collections.Hashtable Items
			{
				get
				{
					return _items;
				}
			}

			/// <summary>
			/// Anexa um proprietário.
			/// </summary>
			/// <param name="owner"></param>
			public void Attach(System.ServiceModel.OperationContext owner)
			{
			}

			/// <summary>
			/// Remove o anexo do proprietário.
			/// </summary>
			/// <param name="owner"></param>
			public void Detach(System.ServiceModel.OperationContext owner)
			{
			}
		}
	}
}
