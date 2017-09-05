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
using Colosoft.Serialization.Surrogates;
using System.Collections;

namespace Colosoft.Serialization
{
	/// <summary>
	/// Classe que auxilia com método de formatação de serviços.
	/// </summary>
	public sealed class CompactFormatterServices
	{
		/// <summary>
		/// Registra um tipo compacto.
		/// </summary>
		/// <param name="type"></param>
		public static void RegisterCompactType(Type type)
		{
			type.Require("type").NotNull();
			if(TypeSurrogateSelector.GetSurrogateForTypeStrict(type, null) != null)
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_TypeAlreadyRegistered2, type.FullName).Format());
			ISerializationSurrogate surrogate = null;
			if(typeof(IDictionary).IsAssignableFrom(type))
				surrogate = new IDictionarySerializationSurrogate(type);
			else if(type.IsArray)
				surrogate = new ArraySerializationSurrogate(type);
			else if(typeof(IList).IsAssignableFrom(type))
				surrogate = new IListSerializationSurrogate(type);
			else if(typeof(ICompactSerializable).IsAssignableFrom(type))
				surrogate = new ICompactSerializableSerializationSurrogate(type);
			else if(typeof(Enum).IsAssignableFrom(type))
				surrogate = new EnumSerializationSurrogate(type);
			if(surrogate == null)
				throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_NoAppropriateSurrogateFound, type.FullName).Format());
			TypeSurrogateSelector.RegisterTypeSurrogate(surrogate);
		}

		/// <summary>
		/// Registra o tipo compacto.
		/// </summary>
		/// <param name="type">Tipo que será registrado.</param>
		/// <param name="typeHandle">Número do manipulador do tipo.</param>
		public static void RegisterCompactType(Type type, short typeHandle)
		{
			type.Require("type").NotNull();
			ISerializationSurrogate surrogateForTypeStrict = null;
			surrogateForTypeStrict = TypeSurrogateSelector.GetSurrogateForTypeStrict(type, null);
			if(surrogateForTypeStrict != null)
			{
				if(surrogateForTypeStrict.TypeHandle != typeHandle)
					throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_TypeAlreadyRegisteredWithDifferentHandle, type.FullName).Format());
			}
			else
			{
				if(typeof(IDictionary).IsAssignableFrom(type))
				{
					if(type.IsGenericType)
						surrogateForTypeStrict = new GenericIDictionarySerializationSurrogate(typeof(IDictionary<, >));
					else
						surrogateForTypeStrict = new IDictionarySerializationSurrogate(type);
				}
				else if(type.IsArray)
				{
					surrogateForTypeStrict = new ArraySerializationSurrogate(type);
				}
				else if(typeof(IList).IsAssignableFrom(type))
				{
					if(type.IsGenericType)
						surrogateForTypeStrict = new GenericIListSerializationSurrogate(typeof(IList<>));
					else
						surrogateForTypeStrict = new IListSerializationSurrogate(type);
				}
				else if(typeof(ICompactSerializable).IsAssignableFrom(type))
					surrogateForTypeStrict = new ICompactSerializableSerializationSurrogate(type);
				else if(typeof(Enum).IsAssignableFrom(type))
					surrogateForTypeStrict = new EnumSerializationSurrogate(type);
				if(surrogateForTypeStrict == null)
					throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_NoAppropriateSurrogateFoundForType, type.FullName).Format());
				TypeSurrogateSelector.RegisterTypeSurrogate(surrogateForTypeStrict, typeHandle);
			}
		}

		/// <summary>
		/// Registra um tipo customizado compacto.
		/// </summary>
		/// <param name="type">Tipo que será registrado.</param>
		/// <param name="typeHandle"></param>
		/// <param name="cacheContext"></param>
		/// <param name="subTypeHandle"></param>
		/// <param name="attributeOrder"></param>
		/// <param name="portable"></param>
		public static void RegisterCustomCompactType(Type type, short typeHandle, string cacheContext, short subTypeHandle, Hashtable attributeOrder, bool portable)
		{
			type.Require("type").NotNull();
			cacheContext.Require("cacheContext").NotNull();
			var surrogateForTypeStrict = TypeSurrogateSelector.GetSurrogateForTypeStrict(type, cacheContext);
			if(surrogateForTypeStrict != null)
			{
				if((surrogateForTypeStrict.TypeHandle != typeHandle) || ((surrogateForTypeStrict.SubTypeHandle != subTypeHandle) && (surrogateForTypeStrict.SubTypeHandle == 0)))
					throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_TypeAlreadyRegisteredWithDifferentHandle, type.FullName).Format());
			}
			else
			{
				if(typeof(IDictionary).IsAssignableFrom(type))
				{
					if(type.IsGenericType)
						surrogateForTypeStrict = new GenericIDictionarySerializationSurrogate(typeof(IDictionary<, >));
					else
						surrogateForTypeStrict = new IDictionarySerializationSurrogate(type);
				}
				else if(type.IsArray)
				{
					surrogateForTypeStrict = new ArraySerializationSurrogate(type);
				}
				else if(typeof(IList).IsAssignableFrom(type))
				{
					if(type.IsGenericType)
						surrogateForTypeStrict = new GenericIListSerializationSurrogate(typeof(IList<>));
					else
						surrogateForTypeStrict = new IListSerializationSurrogate(type);
				}
				else if(typeof(ICompactSerializable).IsAssignableFrom(type))
				{
					surrogateForTypeStrict = new ICompactSerializableSerializationSurrogate(type);
				}
				else if(typeof(Enum).IsAssignableFrom(type))
				{
					surrogateForTypeStrict = new EnumSerializationSurrogate(type);
				}
				else
				{
					DynamicSurrogateBuilder.Portable = portable;
					if(portable)
						DynamicSurrogateBuilder.SubTypeHandle = subTypeHandle;
					surrogateForTypeStrict = DynamicSurrogateBuilder.CreateTypeSurrogate(type, attributeOrder);
				}
				if(surrogateForTypeStrict == null)
				{
					throw new ArgumentException("No appropriate surrogate found for type " + type.FullName);
				}
				TypeSurrogateSelector.RegisterTypeSurrogate(surrogateForTypeStrict, typeHandle, cacheContext, subTypeHandle, portable);
			}
		}

		/// <summary>
		/// Remove o registro de todos os tipos compactor customizados.
		/// </summary>
		/// <param name="cacheContext"></param>
		public static void UnregisterAllCustomCompactTypes(string cacheContext)
		{
			cacheContext.Require("cacheContext").NotNull();
			TypeSurrogateSelector.UnregisterAllSurrogates(cacheContext);
		}

		/// <summary>
		/// Remove o registro do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		public static void UnregisterCompactType(Type type)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Remove o registro do tipo customizado informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="cacheContext"></param>
		public static void UnregisterCustomCompactType(Type type, string cacheContext)
		{
			throw new NotImplementedException();
		}
	}
}
