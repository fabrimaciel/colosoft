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
using System.Threading.Tasks;

namespace Colosoft
{
	/// <summary>
	/// Formatador de mensagens de recursos.
	/// </summary>
	public class ResourceMessageFormatter : IMessageFormattable, ICloneable
	{
		/// <summary>
		/// Nome básico do recurso.
		/// </summary>
		public string BaseName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do recurso.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo da classe do recurso.
		/// </summary>
		public Type ResourceType
		{
			get;
			set;
		}

		/// <summary>
		/// Parametros do formatador.
		/// </summary>
		public object[] Parameters
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public ResourceMessageFormatter()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="baseName">Nome base do recurso.</param>
		/// <param name="name">Nome do recurso.</param>
		/// <param name="resourceType">Tipo que trata o recurso.</param>
		/// <param name="parameters"></param>
		public ResourceMessageFormatter(string baseName, string name, Type resourceType, params object[] parameters)
		{
			if(string.IsNullOrEmpty(baseName))
				throw new ArgumentNullException("baseName");
			else if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			else if(resourceType == null)
				throw new ArgumentNullException("resourceType");
			BaseName = baseName;
			Name = name;
			ResourceType = resourceType;
			Parameters = parameters;
		}

		/// <summary>
		/// Cria uma nova messagem formatada a partir da propriedade de um Recurso.
		/// </summary>
		/// <param name="propertySelector"></param>
		/// <param name="parameters">Parametros que será usados para formatar a mensagem.</param>
		/// <returns></returns>
		public static ResourceMessageFormatter Create(System.Linq.Expressions.Expression<Func<string>> propertySelector, params object[] parameters)
		{
			var propertyExpression = (System.Linq.Expressions.MemberExpression)propertySelector.Body;
			var resourceType = propertyExpression.Member.DeclaringType;
			var resourceManagerProperty = resourceType.GetProperty("ResourceManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var resourceManager = (System.Resources.ResourceManager)resourceManagerProperty.GetValue(null, null);
			return new ResourceMessageFormatter(resourceManager.BaseName, propertySelector.GetMember().Name, resourceType, parameters);
		}

		/// <summary>
		/// Formata a mensagem na cultura informada.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		string IMessageFormattable.Format(System.Globalization.CultureInfo culture)
		{
			return Format(culture, Parameters);
		}

		/// <summary>
		/// Junta a mensagem com outra.
		/// </summary>
		/// <param name="separator">Separador que será usado.</param>
		/// <param name="message">Mensagem que será anexada.</param>
		/// <returns></returns>
		public IMessageFormattable Join(string separator, IMessageFormattable message)
		{
			return new Colosoft.Text.JoinMessageFormattable(this, separator, message);
		}

		/// <summary>
		/// Formata a mensagem na cultura informada usando os parametros.
		/// </summary>
		/// <param name="culture">Cultura que será usada na formatação.</param>
		/// <param name="parameters">Parametros que serão usados na formatação.</param>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format(System.Globalization.CultureInfo culture, params object[] parameters)
		{
			var resourceManagerProperty = ResourceType.GetProperty("ResourceManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var resourceManager = (System.Resources.ResourceManager)resourceManagerProperty.GetValue(null, null);
			var text = resourceManager.GetString(Name, culture);
			if(string.IsNullOrEmpty(text))
				return null;
			object[] values = null;
			if(parameters != null)
			{
				values = new object[parameters.Length];
				for(int i = 0; i < parameters.Length; i++)
				{
					var obj = parameters[i];
					if(obj is IMessageFormattable)
						values[i] = ((IMessageFormattable)obj).Format(culture);
					else
						values[i] = obj;
				}
			}
			if(values != null && values.Length > 0)
				return string.Format(culture, text, values);
			return text;
		}

		/// <summary>
		/// Formata a mensagem na cultura padrão da thread.
		/// </summary>
		/// <returns>Texto da mensagem formatada.</returns>
		public string Format()
		{
			return Format(System.Threading.Thread.CurrentThread.CurrentCulture, Parameters);
		}

		/// <summary>
		/// Retorna um valor indicando se a linguagem da descrição 
		/// da suporte a cultura informada.
		/// </summary>
		/// <param name="culture">Instancia da cultura que será comparado com a linguagem da mensagem.</param>
		/// <returns></returns>
		public bool Matches(System.Globalization.CultureInfo culture)
		{
			return true;
		}

		/// <summary>
		/// Compara as instancia da mensagem informado.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IMessageFormattable other)
		{
			var other2 = other as ResourceMessageFormatter;
			if(other2 != null)
			{
				if(other2.BaseName == BaseName && other2.Name == Name && other2.ResourceType == ResourceType)
				{
					if(other2.Parameters == null && Parameters == null)
						return true;
					if(other2.Parameters.Length != Parameters.Length)
						return false;
					for(var i = 0; i < Parameters.Length; i++)
						if(Parameters[i] != other2.Parameters[i])
							return false;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Recupera o texto da mensagem.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Format();
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new ResourceMessageFormatter(BaseName, Name, ResourceType, Parameters);
		}
	}
}
