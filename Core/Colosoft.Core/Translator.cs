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

namespace Colosoft
{
	/// <summary>
	/// Representa a descrição de quando o valor for vazio.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class EmptyDescriptionAttribute : Attribute
	{
		private string _description;

		/// <summary>
		/// Descrição associada.
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public EmptyDescriptionAttribute()
		{
		}

		/// <summary>
		/// Cria a instancia com a descrição informada.
		/// </summary>
		/// <param name="description"></param>
		public EmptyDescriptionAttribute(string description)
		{
			_description = description;
		}
	}
	/// <summary>
	/// Classe que prove método de extensão para traução.
	/// </summary>
	public static class Translator
	{
		/// <summary>
		/// Recupera as traduções do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<Globalization.TranslateInfo> GetTranslates<T>()
		{
			return GetTranslates(typeof(T));
		}

		/// <summary>
		/// Carrega as traduções do tipo de enum informado.
		/// </summary>
		/// <param name="type">Tipo do enum que será traduzido.</param>
		/// <returns></returns>
		public static IEnumerable<Globalization.TranslateInfo> GetTranslates(Type type)
		{
			return GetTranslates(type, null);
		}

		/// <summary>
		/// Carrega as traduções do tipo de enum informado.
		/// </summary>
		/// <param name="type">Tipo do enum que será traduzido.</param>
		/// <param name="groupKey">Chave do grupo de traduções.</param>
		/// <returns></returns>
		public static IEnumerable<Globalization.TranslateInfo> GetTranslates(Type type, object groupKey)
		{
			if(type == null)
				yield break;
			var translateAttribute = (TranslateAttribute)type.GetCustomAttributes(typeof(TranslateAttribute), false).FirstOrDefault();
			if(type.IsEnum)
			{
				object provider = null;
				if(translateAttribute != null && translateAttribute.ProviderType != null)
					try
					{
						provider = type.GetCustomAttributes(typeof(TranslateAttribute), false).Select(f => Activator.CreateInstance(((TranslateAttribute)f).ProviderType)).FirstOrDefault();
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("Não foi possível carregar o provedor de tradução para o tipo '{0}'", type), ex);
					}
				if(provider is Globalization.ITranslateProvider)
				{
					IEnumerable<Globalization.TranslateInfo> translates = null;
					if(provider is Globalization.IMultipleTranslateProvider)
						translates = ((Globalization.IMultipleTranslateProvider)provider).GetTranslates(groupKey);
					else
						translates = ((Globalization.ITranslateProvider)provider).GetTranslates();
					foreach (var i in translates)
						yield return i;
					yield break;
				}
				foreach (var field in type.GetFields().Where(f => f.FieldType.IsEnum))
				{
					yield return new Globalization.TranslateInfo(field.GetValue(null), (field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).Select(f => ((System.ComponentModel.DescriptionAttribute)f).Description).FirstOrDefault() ?? field.Name).GetFormatter());
				}
			}
		}

		/// <summary>
		/// Carrega as traduções do tipo de enum informado.
		/// </summary>
		/// <param name="typeName">Tipo do enum que será traduzido.</param>
		/// <returns></returns>
		public static IEnumerable<Globalization.TranslateInfo> GetTranslatesFromTypeName(string typeName)
		{
			return GetTranslates(Type.GetType(typeName, true));
		}

		/// <summary>
		/// Carrega as traduções do tipo de enum informado.
		/// </summary>
		/// <param name="typeName">Tipo do enum que será traduzido.</param>
		/// <param name="groupKey">Chave do grupo de traduções.</param>
		/// <returns></returns>
		public static IEnumerable<Globalization.TranslateInfo> GetTranslatesFromTypeName(string typeName, object groupKey)
		{
			return GetTranslates(Type.GetType(typeName, true), groupKey);
		}

		/// <summary>
		/// Traduz o valor do enumerador.
		/// </summary>
		/// <param name="value">Valor que será traduzido.</param>
		/// <returns></returns>
		public static IMessageFormattable Translate(this object value)
		{
			return Translate(value, null);
		}

		/// <summary>
		/// Traduz o valor do enumerador.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="groupKey">Chave do grupo de traduções.</param>
		/// <returns></returns>
		public static IMessageFormattable Translate(this object value, object groupKey)
		{
			if(value == null)
				return MessageFormattable.Empty;
			var result = new List<IMessageFormattable>();
			var valueType = value.GetType();
			if(valueType.IsEnum)
			{
				var translateAttribute = (TranslateAttribute)valueType.GetCustomAttributes(typeof(TranslateAttribute), false).FirstOrDefault();
				object provider = null;
				if(translateAttribute != null && translateAttribute.ProviderType != null)
					try
					{
						provider = valueType.GetCustomAttributes(typeof(TranslateAttribute), false).Select(f => Activator.CreateInstance(((TranslateAttribute)f).ProviderType)).FirstOrDefault();
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("Não foi possível carregar o provedor de tradução para o tipo '{0}'", valueType), ex);
					}
				IEnumerable<Globalization.TranslateInfo> translates = null;
				var baseType = Enum.GetUnderlyingType(valueType).Name;
				if(value != null)
				{
					IEnumerable<System.Reflection.FieldInfo> fields = null;
					var isFlags = valueType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;
					long[] flags = null;
					if(isFlags)
						flags = ((Enum)value).GetIndividualFlags().ToArray();
					fields = valueType.GetFields().Where(f =>  {
						if(f.FieldType.IsEnum)
						{
							var fieldValue = f.GetValue(null);
							if(isFlags)
							{
								return flags.Contains(Convert.ToInt64((Enum)fieldValue));
							}
							return fieldValue.ToString() == value.ToString();
						}
						return false;
					});
					if(provider is Globalization.ITranslateProvider)
					{
						if(translates == null)
						{
							if(provider is Globalization.IMultipleTranslateProvider)
								translates = ((Globalization.IMultipleTranslateProvider)provider).GetTranslates(groupKey);
							else
								translates = ((Globalization.ITranslateProvider)provider).GetTranslates();
						}
						var texts = fields.Select(field => translates.Where(f => field.Name.Equals((f.Key ?? "").ToString())).Select(f => f.Text).FirstOrDefault()).Where(f => f != null);
						return texts.Join(", ");
					}
					else
					{
						var texts = fields.Select(field => (field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false).Select(f => ((System.ComponentModel.DescriptionAttribute)f).Description).FirstOrDefault() ?? field.Name).GetFormatter());
						return texts.Join(", ");
					}
				}
				var emptyDescription = valueType.GetCustomAttributes(typeof(EmptyDescriptionAttribute), false) as EmptyDescriptionAttribute[];
				if(emptyDescription != null && emptyDescription.Length > 0)
					return emptyDescription[0].Description.GetFormatter();
			}
			return MessageFormattable.Empty;
		}

		/// <summary>
		/// Enumerates through the individual set bits in a flag enum.
		/// </summary>
		/// <param name="flags">The flags enum value.</param>
		/// <returns>An enumeration of just the <i>set</i> bits in the flags enum.</returns>
		static IEnumerable<long> GetIndividualFlags(Enum flags)
		{
			long flagsLong = Convert.ToInt64(flags);
			for(int i = 0; i < sizeof(long) * 8; i++)
			{
				long individualFlagPosition = (long)Math.Pow(2, i);
				long individualFlag = flagsLong & individualFlagPosition;
				if(individualFlag == individualFlagPosition)
				{
					yield return individualFlag;
				}
			}
		}
	}
}
