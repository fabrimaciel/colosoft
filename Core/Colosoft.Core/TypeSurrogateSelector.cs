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
	/// Classe responsável pela seleção dos tipos substitutos.
	/// </summary>
	public sealed class TypeSurrogateSelector
	{
		/// <summary>
		/// Faixa do identificador do tipos customizados.
		/// </summary>
		private static short CUSTOM_TYPE_RANGE = 0x3e8;

		/// <summary>
		/// Instancia padrão do substituto de serialização de vetores.
		/// </summary>
		private static ISerializationSurrogate defaultArraySurrogate = new ObjectArraySerializationSurrogate();

		/// <summary>
		/// Instancia padrão do substituto de serialização de objetos.
		/// </summary>
		private static ISerializationSurrogate defaultSurrogate = new ObjectSerializationSurrogate(typeof(object));

		private static IDictionary handleSurrogateMap = Hashtable.Synchronized(new Hashtable());

		private static ISerializationSurrogate nullSurrogate = new NullSerializationSurrogate();

		private static short typeHandle = -32768;

		/// <summary>
		/// Mapa dos tipos substitutos.
		/// </summary>
		private static IDictionary typeSurrogateMap = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Mapa dos manipuladores dos tipos do usuário.
		/// </summary>
		private static IDictionary userTypeHandleSurrogateMap = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Mapa dos substitutos dos tipos do usuário.
		/// </summary>
		private static IDictionary userTypeSurrogateMap = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// 
		/// </summary>
		static TypeSurrogateSelector()
		{
			RegisterTypeSurrogate(nullSurrogate);
			RegisterTypeSurrogate(defaultSurrogate);
			RegisterTypeSurrogate(defaultArraySurrogate);
			RegisterBuiltinSurrogates();
		}

		/// <summary>
		/// Recupera o substituto para o objeto informado.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal static ISerializationSurrogate GetSurrogateForObject(object graph, string cacheContext)
		{
			if(graph == null)
				return nullSurrogate;
			Type actualType = null;
			if(graph is ArrayList)
				actualType = typeof(ArrayList);
			else if(graph is Hashtable)
				actualType = typeof(Hashtable);
			else if(graph is SortedList)
				actualType = typeof(SortedList);
			else if(((graph is IList) && graph.GetType().FullName.Contains("System.Collections.Generic")) && graph.GetType().IsGenericType)
				actualType = typeof(IList<>);
			else if(((graph is IDictionary) && graph.GetType().IsGenericType) && graph.GetType().FullName.Contains("System.Collections.Generic"))
				actualType = typeof(IDictionary<, >);
			else if(graph is ICompactSerializable)
				actualType = typeof(ICompactSerializable);
			else if(graph.GetType().IsArray && UserTypeSurrogateExists(graph.GetType().GetElementType(), cacheContext))
				actualType = new ObjectArraySerializationSurrogate().ActualType;
			else
				actualType = graph.GetType();
			return GetSurrogateForType(actualType, cacheContext);
		}

		/// <summary>
		/// Recupera o substituto pelo ID do subtipo.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="subHandle"></param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal static ISerializationSurrogate GetSurrogateForSubTypeHandle(short handle, short subHandle, string cacheContext)
		{
			ISerializationSurrogate defaultSurrogate = null;
			Hashtable hashtable = (Hashtable)userTypeHandleSurrogateMap[cacheContext];
			if((hashtable != null) && (hashtable[handle] != null))
			{
				defaultSurrogate = (ISerializationSurrogate)((Hashtable)hashtable[handle])[subHandle];
				if((defaultSurrogate == null) && (((Hashtable)hashtable[handle]).Count > 0))
				{
					IDictionaryEnumerator enumerator = (IDictionaryEnumerator)((Hashtable)hashtable[handle]).Values.GetEnumerator();
					enumerator.MoveNext();
					defaultSurrogate = (ISerializationSurrogate)enumerator.Value;
				}
			}
			if(defaultSurrogate == null)
				defaultSurrogate = TypeSurrogateSelector.defaultSurrogate;
			return defaultSurrogate;
		}

		/// <summary>
		/// Recupera o substituto para o tipo informado.
		/// </summary>
		/// <param name="type">Tipo associado com o substituto.</param>
		/// <param name="cacheContext">Contexto que será usado na recuperação.</param>
		/// <returns></returns>
		public static ISerializationSurrogate GetSurrogateForType(Type type, string cacheContext)
		{
			ISerializationSurrogate defaultSurrogate = (ISerializationSurrogate)typeSurrogateMap[type];
			if((defaultSurrogate == null) && (cacheContext != null))
			{
				Hashtable hashtable = (Hashtable)userTypeSurrogateMap[cacheContext];
				if(hashtable != null)
					defaultSurrogate = (ISerializationSurrogate)hashtable[type];
			}
			if(defaultSurrogate == null)
			{
				defaultSurrogate = TypeSurrogateSelector.defaultSurrogate;
			}
			return defaultSurrogate;
		}

		/// <summary>
		/// Recupera o substituto para tratar o tipo.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		internal static ISerializationSurrogate GetSurrogateForTypeHandle(short handle, string cacheContext)
		{
			ISerializationSurrogate defaultSurrogate = null;
			if(handle < CUSTOM_TYPE_RANGE)
				defaultSurrogate = (ISerializationSurrogate)handleSurrogateMap[handle];
			else
			{
				Hashtable hashtable = (Hashtable)userTypeHandleSurrogateMap[cacheContext];
				if((hashtable != null) && hashtable.Contains(handle))
				{
					if(!(hashtable[handle] is ISerializationSurrogate))
					{
						return null;
					}
					defaultSurrogate = (ISerializationSurrogate)hashtable[handle];
				}
			}
			if(defaultSurrogate == null)
				defaultSurrogate = TypeSurrogateSelector.defaultSurrogate;
			return defaultSurrogate;
		}

		internal static ISerializationSurrogate GetSurrogateForTypeStrict(Type type, string cacheContext)
		{
			ISerializationSurrogate surrogate = null;
			surrogate = (ISerializationSurrogate)typeSurrogateMap[type];
			if((surrogate == null) && (cacheContext != null))
			{
				Hashtable hashtable = (Hashtable)userTypeSurrogateMap[cacheContext];
				if(hashtable != null)
				{
					surrogate = (ISerializationSurrogate)hashtable[type];
				}
			}
			return surrogate;
		}

		/// <summary>
		/// Registra os substitutos nativos.
		/// </summary>
		public static void RegisterBuiltinSurrogates()
		{
			RegisterTypeSurrogate(new BooleanSerializationSurrogate());
			RegisterTypeSurrogate(new ByteSerializationSurrogate());
			RegisterTypeSurrogate(new CharSerializationSurrogate());
			RegisterTypeSurrogate(new SingleSerializationSurrogate());
			RegisterTypeSurrogate(new DoubleSerializationSurrogate());
			RegisterTypeSurrogate(new DecimalSerializationSurrogate());
			RegisterTypeSurrogate(new Int16SerializationSurrogate());
			RegisterTypeSurrogate(new Int32SerializationSurrogate());
			RegisterTypeSurrogate(new Int64SerializationSurrogate());
			RegisterTypeSurrogate(new StringSerializationSurrogate());
			RegisterTypeSurrogate(new DateTimeSerializationSurrogate());
			RegisterTypeSurrogate(new DateTimeOffsetSerializationSurrogate());
			RegisterTypeSurrogate(new TimeSpanSerializationSurrogate());
			RegisterTypeSurrogate(new NullSerializationSurrogate());
			RegisterTypeSurrogate(new BooleanArraySerializationSurrogate());
			RegisterTypeSurrogate(new ByteArraySerializationSurrogate());
			RegisterTypeSurrogate(new CharArraySerializationSurrogate());
			RegisterTypeSurrogate(new SingleArraySerializationSurrogate());
			RegisterTypeSurrogate(new DoubleArraySerializationSurrogate());
			RegisterTypeSurrogate(new Int16ArraySerializationSurrogate());
			RegisterTypeSurrogate(new Int32ArraySerializationSurrogate());
			RegisterTypeSurrogate(new Int64ArraySerializationSurrogate());
			RegisterTypeSurrogate(new StringArraySerializationSurrogate());
			RegisterTypeSurrogate(new EOFSerializationSurrogate());
			RegisterTypeSurrogate(new SkipSerializationSurrogate());
			RegisterTypeSurrogate(new DecimalArraySerializationSurrogate());
			RegisterTypeSurrogate(new DateTimeArraySerializationSurrogate());
			RegisterTypeSurrogate(new GuidArraySerializationSurrogate());
			RegisterTypeSurrogate(new SByteArraySerializationSurrogate());
			RegisterTypeSurrogate(new UInt16ArraySerializationSurrogate());
			RegisterTypeSurrogate(new UInt32ArraySerializationSurrogate());
			RegisterTypeSurrogate(new UInt64ArraySerializationSurrogate());
			RegisterTypeSurrogate(new GuidSerializationSurrogate());
			RegisterTypeSurrogate(new SByteSerializationSurrogate());
			RegisterTypeSurrogate(new UInt16SerializationSurrogate());
			RegisterTypeSurrogate(new UInt32SerializationSurrogate());
			RegisterTypeSurrogate(new UInt64SerializationSurrogate());
			RegisterTypeSurrogate(new ArraySerializationSurrogate(typeof(Array)));
			RegisterTypeSurrogate(new IListSerializationSurrogate(typeof(ArrayList)));
			RegisterTypeSurrogate(new NullSerializationSurrogate());
			RegisterTypeSurrogate(new IDictionarySerializationSurrogate(typeof(Hashtable)));
			RegisterTypeSurrogate(new IDictionarySerializationSurrogate(typeof(SortedList)));
			RegisterTypeSurrogate(new NullSerializationSurrogate());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<bool>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<byte>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<char>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<float>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<double>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<decimal>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<short>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<int>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<long>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<DateTime>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<DateTimeOffset>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<TimeSpan>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<Guid>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<sbyte>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<ushort>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<uint>());
			RegisterTypeSurrogate(new NullableArraySerializationSurrogate<ulong>());
		}

		/// <summary>
		/// Registra o substituto no sistema.
		/// </summary>
		/// <param name="surrogate">Instancia do substituto que será registrado.</param>
		/// <returns>True ser o registro foi feito com sucesso.</returns>
		public static bool RegisterTypeSurrogate(ISerializationSurrogate surrogate)
		{
			bool flag;
			while (true)
			{
				try
				{
					flag = RegisterTypeSurrogate(surrogate, typeHandle = (short)(typeHandle + 1));
				}
				catch(ArgumentException)
				{
					continue;
				}
				catch(Exception)
				{
					throw;
				}
				return flag;
			}
		}

		/// <summary>
		/// Registra o substituto no sistema.
		/// </summary>
		/// <param name="surrogate">Instancia do substituto que será registrado.</param>
		/// <param name="typehandle">Id do tipo.</param>
		/// <returns>rue ser o registro foi feito com sucesso.</returns>
		public static bool RegisterTypeSurrogate(ISerializationSurrogate surrogate, short typehandle)
		{
			surrogate.Require("surrogate").NotNull();
			lock (typeSurrogateMap.SyncRoot)
			{
				if(handleSurrogateMap.Contains(typehandle))
					throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_TypeAlreadyRegistered).Format());
				if(!typeSurrogateMap.Contains(surrogate.ActualType))
				{
					surrogate.TypeHandle = typehandle;
					typeSurrogateMap.Add(surrogate.ActualType, surrogate);
					handleSurrogateMap.Add(surrogate.TypeHandle, surrogate);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Registra um tipo substituto.
		/// </summary>
		/// <param name="surrogate">Implementação do substituto.</param>
		/// <param name="typehandle">Id do tipo.</param>
		/// <param name="cacheContext">Contexto do cache.</param>
		/// <param name="subTypeHandle">Id do subtipo.</param>
		/// <param name="portable">Identifica se é portável.</param>
		/// <returns></returns>
		public static bool RegisterTypeSurrogate(ISerializationSurrogate surrogate, short typehandle, string cacheContext, short subTypeHandle, bool portable)
		{
			surrogate.Require("surrogate").NotNull();
			lock (typeSurrogateMap.SyncRoot)
			{
				if(cacheContext != null)
				{
					Hashtable hashtable = (Hashtable)userTypeHandleSurrogateMap[cacheContext];
					if(hashtable == null)
					{
						hashtable = new Hashtable();
						userTypeHandleSurrogateMap.Add(cacheContext, hashtable);
					}
					if(portable)
					{
						if(hashtable.Contains(typehandle) && ((Hashtable)hashtable[typehandle]).Contains(subTypeHandle))
							throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_SubtypeAlreadyRegistered).Format());
					}
					else if(hashtable.Contains(typehandle))
						throw new ArgumentException(ResourceMessageFormatter.Create(() => Properties.Resources.Argument_TypeAlreadyRegistered).Format());
					Hashtable hashtable2 = (Hashtable)userTypeSurrogateMap[cacheContext];
					if(hashtable2 == null)
					{
						hashtable2 = new Hashtable();
						userTypeSurrogateMap.Add(cacheContext, hashtable2);
					}
					if(!hashtable2.Contains(surrogate.ActualType))
					{
						if(portable)
						{
							surrogate.TypeHandle = typehandle;
							surrogate.SubTypeHandle = subTypeHandle;
							if(!hashtable.Contains(typehandle) || (hashtable[typehandle] == null))
								hashtable[typehandle] = new Hashtable();
							((Hashtable)hashtable[typehandle]).Add(subTypeHandle, surrogate);
							hashtable2.Add(surrogate.ActualType, surrogate);
							return true;
						}
						surrogate.TypeHandle = typehandle;
						hashtable2.Add(surrogate.ActualType, surrogate);
						hashtable.Add(surrogate.TypeHandle, surrogate);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Desregistra todos os substitutos.
		/// </summary>
		public static void UnregisterAllSurrogates()
		{
			lock (typeSurrogateMap.SyncRoot)
			{
				typeSurrogateMap.Clear();
				handleSurrogateMap.Clear();
				userTypeHandleSurrogateMap.Clear();
				userTypeSurrogateMap.Clear();
				typeHandle = -32768;
				RegisterTypeSurrogate(nullSurrogate);
				RegisterTypeSurrogate(defaultSurrogate);
			}
		}

		/// <summary>
		/// Remove todos os substitutos no contexto informado.
		/// </summary>
		/// <param name="cacheContext"></param>
		public static void UnregisterAllSurrogates(string cacheContext)
		{
			lock (typeSurrogateMap.SyncRoot)
			{
				if(cacheContext != null)
				{
					if(userTypeHandleSurrogateMap.Contains(cacheContext))
						userTypeHandleSurrogateMap.Remove(cacheContext);
					if(userTypeSurrogateMap.Contains(cacheContext))
						userTypeSurrogateMap.Remove(cacheContext);
				}
			}
		}

		/// <summary>
		/// Remove o registro do tipo de substituto.
		/// </summary>
		/// <param name="surrogate"></param>
		public static void UnregisterTypeSurrogate(ISerializationSurrogate surrogate)
		{
		}

		/// <summary>
		/// Remove o registro do tipo de substituto.
		/// </summary>
		/// <param name="surrogate"></param>
		/// <param name="cacheContext"></param>
		public static void UnregisterTypeSurrogate(ISerializationSurrogate surrogate, string cacheContext)
		{
		}

		/// <summary>
		/// Verifica se existe algum substituto para o tipo do usuário no contexto.
		/// </summary>
		/// <param name="type">Tipo que será verificado.</param>
		/// <param name="cacheContext"></param>
		/// <returns></returns>
		private static bool UserTypeSurrogateExists(Type type, string cacheContext)
		{
			bool flag = false;
			if(cacheContext != null)
			{
				Hashtable hashtable = (Hashtable)userTypeSurrogateMap[cacheContext];
				if(hashtable != null)
					flag = hashtable.Contains(type);
			}
			return flag;
		}
	}
}
