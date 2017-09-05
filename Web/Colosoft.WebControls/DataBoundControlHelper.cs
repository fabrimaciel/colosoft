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
using System.Text;
using System.Web.UI;
using System.Web;
using System.Collections.Specialized;

namespace Colosoft.WebControls
{
	public static class DataBoundControlHelper
	{
		/// <summary>
		/// Método usado para comparar dois vetores de string.
		/// </summary>
		/// <param name="stringA"></param>
		/// <param name="stringB"></param>
		/// <returns></returns>
		public static bool CompareStringArrays(string[] stringA, string[] stringB)
		{
			if((stringA != null) || (stringB != null))
			{
				if((stringA == null) || (stringB == null))
				{
					return false;
				}
				if(stringA.Length != stringB.Length)
				{
					return false;
				}
				for(int i = 0; i < stringA.Length; i++)
				{
					if(!string.Equals(stringA[i], stringB[i], StringComparison.Ordinal))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static void ExtractValuesFromBindableControls(IOrderedDictionary dictionary, Control container)
		{
			IBindableControl control = container as IBindableControl;
			if(control != null)
			{
				control.ExtractValues(dictionary);
			}
			foreach (Control control2 in container.Controls)
			{
				ExtractValuesFromBindableControls(dictionary, control2);
			}
		}

		/// <summary>
		/// Localiza um controle do tipo com o ID informado
		/// </summary>
		/// <param name="control"></param>
		/// <param name="controlID"></param>
		/// <returns></returns>
		public static Control FindControl(Control control, string controlID)
		{
			Control namingContainer = control;
			Control control3 = null;
			if(control != control.Page)
			{
				while ((control3 == null) && (namingContainer != control.Page))
				{
					namingContainer = namingContainer.NamingContainer;
					if(namingContainer == null)
					{
						throw new HttpException(string.Format("No naming container. {0}, {1}", control.GetType().Name, control.ID));
					}
					control3 = namingContainer.FindControl(controlID);
				}
				return control3;
			}
			return control.FindControl(controlID);
		}

		/// <summary>
		/// Verifica se o tipo suporte um link bind.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsBindableType(Type type)
		{
			if(type == null)
			{
				return false;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null)
			{
				type = underlyingType;
			}
			if(((!type.IsPrimitive && (type != typeof(string))) && ((type != typeof(DateTime)) && (type != typeof(decimal)))) && ((type != typeof(Guid)) && (type != typeof(DateTimeOffset))))
			{
				return (type == typeof(TimeSpan));
			}
			return true;
		}
	}
}
