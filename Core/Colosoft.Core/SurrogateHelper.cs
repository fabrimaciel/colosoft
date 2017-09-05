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
using System.Reflection.Emit;
using System.Reflection;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Assinatura dos métodos para chamada do construtor padrão de uma instancia.
	/// </summary>
	/// <returns></returns>
	public delegate object DefaultConstructorDelegate ();
	/// <summary>
	/// Classe com métodos que auxiliam os subtitutos de serialização.
	/// </summary>
	public class SurrogateHelper
	{
		/// <summary>
		/// Recupera o delegate para a criação padrão para o tipo.
		/// </summary>
		/// <param name="type">Tipo da instancia que será criada.</param>
		/// <returns></returns>
		public static DefaultConstructorDelegate CreateDefaultConstructorDelegate(Type type)
		{
			if(type == null)
			{
				throw new ArgumentNullException("genericType");
			}
			Type[] emptyTypes = Type.EmptyTypes;
			var method = new DynamicMethod(string.Empty, MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(object), emptyTypes, type, true);
			ILGenerator iLGenerator = method.GetILGenerator();
			EmitDefaultCreatorMethod(type, iLGenerator);
			return (DefaultConstructorDelegate)method.CreateDelegate(typeof(DefaultConstructorDelegate));
		}

		/// <summary>
		/// Cria uma instancia para o tipo generico informado.
		/// </summary>
		/// <param name="name">Nome completo do tipo que será criado.</param>
		/// <param name="types">Tipos genericos associados.</param>
		/// <returns></returns>
		public static object CreateGenericType(string name, params Type[] types)
		{
			return Activator.CreateInstance(Type.GetType(name + "`" + types.Length).MakeGenericType(types));
		}

		/// <summary>
		/// Cria uma instancia para o tipo generico informado.
		/// </summary>
		/// <param name="genericType">Instancia do tipo generico que será criado.</param>
		/// <param name="typeParams">Tipos genericos associados.</param>
		/// <returns></returns>
		public static object CreateGenericTypeInstance(Type genericType, params Type[] typeParams)
		{
			genericType.Require("genericType").NotNull();
			try
			{
				genericType = genericType.MakeGenericType(typeParams);
			}
			catch(InvalidOperationException)
			{
				throw;
			}
			catch(ArgumentNullException)
			{
				throw;
			}
			catch(ArgumentException)
			{
				throw;
			}
			catch(NotSupportedException)
			{
				throw;
			}
			return Activator.CreateInstance(genericType);
		}

		/// <summary>
		/// Emite o método para criador padrão.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="il"></param>
		internal static void EmitDefaultCreatorMethod(Type type, ILGenerator il)
		{
			il.DeclareLocal(typeof(object));
			if(!type.IsValueType)
			{
				ConstructorInfo con = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
				if(con == null)
				{
					throw new ArgumentException("constructor not found", type.Name);
				}
				il.Emit(OpCodes.Newobj, con);
			}
			else
			{
				LocalBuilder local = il.DeclareLocal(type);
				il.Emit(OpCodes.Ldloca_S, local);
				il.Emit(OpCodes.Initobj, type);
				il.Emit(OpCodes.Ldloc_1);
				il.Emit(OpCodes.Box, type);
			}
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);
		}
	}
}
