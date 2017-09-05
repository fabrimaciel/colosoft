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
using System.Reflection;
using Colosoft.Serialization.IO;
using System.Collections;
using System.Reflection.Emit;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Classe responsável pela construção de substitutos dinamicos.
	/// </summary>
	public class DynamicSurrogateBuilder
	{
		private static Hashtable _attributeOrder;

		private static MethodInfo _compactBinaryReaderIfSkip = typeof(CompactBinaryReader).GetMethod("IfSkip", new Type[] {
			typeof(object),
			typeof(object)
		});

		private static MethodInfo _compactBinaryReaderReadObject = typeof(CompactBinaryReader).GetMethod("ReadObject", Type.EmptyTypes);

		private static MethodInfo _compactBinaryReaderReadObjectAs = typeof(CompactBinaryReader).GetMethod("ReadObjectAs", new Type[] {
			typeof(Type)
		});

		private static MethodInfo _compactBinaryReaderSkipObject = typeof(CompactBinaryReader).GetMethod("SkipObject");

		private static MethodInfo _compactBinaryWriterWriteObject = typeof(CompactBinaryWriter).GetMethod("WriteObject", new Type[] {
			typeof(object)
		});

		private static MethodInfo _compactBinaryWriterWriteObjectAs = typeof(CompactBinaryWriter).GetMethod("WriteObjectAs", new Type[] {
			typeof(object),
			typeof(Type)
		});

		private static short _subTypeHandle;

		private static MethodInfo _type_GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

		private static Label EOFNet;

		private static bool _portable;

		/// <summary>
		/// True identifica se será usado de forma portavel.
		/// </summary>
		public static bool Portable
		{
			get
			{
				return _portable;
			}
			set
			{
				_portable = value;
			}
		}

		/// <summary>
		/// Numero do manipulador de subtipo.
		/// </summary>
		public static short SubTypeHandle
		{
			get
			{
				return _subTypeHandle;
			}
			set
			{
				_subTypeHandle = value;
			}
		}

		/// <summary>
		/// Recupera o delegate do método responsável por lê a instancia do tipo passado.
		/// </summary>
		/// <param name="type">Tipo que será lido.</param>
		/// <returns></returns>
		internal static ReadObjectDelegate CreateReaderDelegate(Type type)
		{
			Type[] parameterTypes = new Type[] {
				typeof(CompactBinaryReader),
				typeof(object)
			};
			DynamicMethod method = new DynamicMethod(string.Empty, MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(object), parameterTypes, type, true);
			ILGenerator iLGenerator = method.GetILGenerator();
			iLGenerator.DeclareLocal(type);
			if(!_portable)
				EmitReaderMethod(type, iLGenerator);
			else
				EmitPortableReaderMethod(type, iLGenerator);
			return (ReadObjectDelegate)method.CreateDelegate(typeof(ReadObjectDelegate));
		}

		/// <summary>
		/// Cria um tipo substituto para o tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será criado para substituir.</param>
		/// <param name="attributeOrder">Hash com os atributos ordenados.</param>
		/// <returns></returns>
		public static SerializationSurrogate CreateTypeSurrogate(Type type, Hashtable attributeOrder)
		{
			Type type2;
			if(attributeOrder != null)
				_attributeOrder = new Hashtable();
			if((_attributeOrder != null) && (attributeOrder != null))
				_attributeOrder.Add(type, attributeOrder[type.FullName]);
			if(type.IsValueType)
				type2 = typeof(DynamicValueTypeSurrogate<>);
			else
				type2 = typeof(DynamicRefTypeSurrogate<>);
			return (SerializationSurrogate)SurrogateHelper.CreateGenericTypeInstance(type2, new Type[] {
				type
			});
		}

		internal static WriteObjectDelegate CreateWriterDelegate(Type type)
		{
			Type[] parameterTypes = new Type[] {
				typeof(CompactBinaryWriter),
				typeof(object)
			};
			DynamicMethod method = new DynamicMethod(string.Empty, MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, null, parameterTypes, type, true);
			ILGenerator iLGenerator = method.GetILGenerator();
			if(!_portable)
			{
				EmitWriterMethod(type, iLGenerator);
			}
			else
			{
				EmitPortableWriterMethod(type, iLGenerator);
			}
			return (WriteObjectDelegate)method.CreateDelegate(typeof(WriteObjectDelegate));
		}

		internal static void EmitWriterMethod(Type type, ILGenerator il)
		{
			List<FieldInfo> allFields = new List<FieldInfo>();
			allFields = GetAllFields(type, allFields);
			LocalBuilder local = il.DeclareLocal(type);
			il.Emit(OpCodes.Ldarg_1);
			if(type.IsValueType)
			{
				il.Emit(OpCodes.Unbox_Any, type);
			}
			else
			{
				il.Emit(OpCodes.Castclass, type);
			}
			il.Emit(OpCodes.Stloc_0);
			foreach (FieldInfo info in allFields)
			{
				il.Emit(OpCodes.Ldarg_0);
				if(type.IsValueType)
				{
					il.Emit(OpCodes.Ldloca_S, local);
				}
				else
				{
					il.Emit(OpCodes.Ldloc_0);
				}
				EmitWriteInstruction(info, il);
			}
			il.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Recupera todos os campos do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static List<FieldInfo> GetAllFields(Type type, List<FieldInfo> list)
		{
			if(type == typeof(object))
				return new List<FieldInfo>();
			string[][] strArray = null;
			if(_attributeOrder != null)
				strArray = (string[][])_attributeOrder[type];
			BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			List<FieldInfo> list2 = new List<FieldInfo>();
			list2.AddRange(type.GetFields(bindingAttr));
			Type baseType = type;
			while (true)
			{
				baseType = baseType.BaseType;
				if(baseType == typeof(object))
					break;
				list2.AddRange(baseType.GetFields(bindingAttr));
			}
			FieldInfo[] infoArray = list2.ToArray();
			FieldInfo[] collection = null;
			if(((strArray == null) || !Portable) || (strArray.Length == 0))
				collection = infoArray;
			else
			{
				FieldInfo[] infoArray3 = new FieldInfo[strArray[0].Length + 1];
				bool flag = true;
				for(int i = 0; i < (strArray[0].Length + 1); i++)
				{
					if((i == strArray[0].Length) && flag)
						break;
					int index = i;
					if(!flag)
						index = i - 1;
					if(strArray[0][index] != "skip.attribute")
					{
						if((strArray[1][index] != "-1") || !flag)
						{
							for(int j = 0; j < infoArray.Length; j++)
							{
								if(strArray[0][index] == infoArray[j].Name)
									infoArray3[i] = infoArray[j];
							}
							if(infoArray3[i] == null)
								throw new Exception("Unable to intialize Compact Serialization: Assembly mismatch, The Assembly provided to NCManager is different to the one used locally: Unable to find Field " + strArray[0][index] + " in " + type.FullName);
						}
						else
						{
							infoArray3[i] = null;
							flag = false;
						}
					}
					else
					{
						infoArray3[i] = null;
					}
				}
				collection = infoArray3;
			}
			list.AddRange(collection);
			return list;
		}

		/// <summary>
		/// Recupera o método de leitura dos dados para o tipo.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="il"></param>
		internal static void EmitReaderMethod(Type type, ILGenerator il)
		{
			List<FieldInfo> allFields = new List<FieldInfo>();
			allFields = GetAllFields(type, allFields);
			LocalBuilder local = il.DeclareLocal(type);
			il.DeclareLocal(typeof(object));
			EOFNet = il.DefineLabel();
			il.Emit(OpCodes.Ldarg_1);
			if(type.IsValueType)
				il.Emit(OpCodes.Unbox_Any, type);
			else
				il.Emit(OpCodes.Castclass, type);
			il.Emit(OpCodes.Stloc_0);
			foreach (FieldInfo info in allFields)
			{
				if(type.IsValueType)
					il.Emit(OpCodes.Ldloca_S, local);
				else
					il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ldarg_0);
				EmitReadInstruction(info, il);
			}
			il.MarkLabel(EOFNet);
			il.Emit(OpCodes.Ldloc_0);
			if(type.IsValueType)
				il.Emit(OpCodes.Box, type);
			il.Emit(OpCodes.Stloc_1);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Recupera o método de leitura de forma portavel.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="il"></param>
		private static void EmitPortableReaderMethod(Type type, ILGenerator il)
		{
			List<FieldInfo> allFields = new List<FieldInfo>();
			allFields = GetAllFields(type, allFields);
			string[][] strArray = null;
			if(_attributeOrder != null)
				strArray = (string[][])_attributeOrder[type];
			LocalBuilder local = il.DeclareLocal(type);
			il.DeclareLocal(typeof(object));
			EOFNet = il.DefineLabel();
			il.Emit(OpCodes.Ldarg_1);
			if(type.IsValueType)
				il.Emit(OpCodes.Unbox_Any, type);
			else
				il.Emit(OpCodes.Castclass, type);
			il.Emit(OpCodes.Stloc_0);
			for(int i = 0; i < allFields.Count; i++)
			{
				FieldInfo field = allFields[i];
				bool flag = false;
				if((i < strArray[1].Length) && (strArray[1][i] == "0"))
					flag = true;
				if((field != null) || !flag)
				{
					if(type.IsValueType)
						il.Emit(OpCodes.Ldloca_S, local);
					else
						il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Ldarg_0);
					EmitPortableReadInstruction(field, il, local);
				}
				else
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Callvirt, _compactBinaryReaderSkipObject);
				}
			}
			il.MarkLabel(EOFNet);
			il.Emit(OpCodes.Ldloc_0);
			if(type.IsValueType)
				il.Emit(OpCodes.Box, type);
			il.Emit(OpCodes.Ret);
		}

		private static void EmitPortableReadInstruction(FieldInfo field, ILGenerator il, LocalBuilder objLocal)
		{
			MethodInfo method = null;
			if(field != null)
			{
				Type fieldType = field.FieldType;
				if(fieldType.IsPrimitive)
				{
					string name = "Read" + fieldType.Name;
					method = typeof(CompactBinaryReader).GetMethod(name);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObject);
					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Ldfld, field);
					il.Emit(OpCodes.Box, fieldType);
					il.Emit(OpCodes.Callvirt, _compactBinaryReaderIfSkip);
					il.Emit(OpCodes.Unbox_Any, fieldType);
					il.Emit(OpCodes.Stfld, field);
				}
				if(method == null)
				{
					if(fieldType.IsInterface || !fieldType.IsPrimitive)
					{
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObject);
						il.Emit(OpCodes.Ldloc_0);
						il.Emit(OpCodes.Ldfld, field);
						il.Emit(OpCodes.Box, fieldType);
						il.Emit(OpCodes.Callvirt, _compactBinaryReaderIfSkip);
					}
					else
					{
						il.Emit(OpCodes.Ldtoken, fieldType);
						il.Emit(OpCodes.Call, _type_GetTypeFromHandle);
						il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObjectAs);
					}
					if(fieldType.IsValueType)
					{
						il.Emit(OpCodes.Unbox_Any, fieldType);
					}
					else
					{
						il.Emit(OpCodes.Castclass, fieldType);
					}
					il.Emit(OpCodes.Stfld, field);
				}
			}
			else
			{
				il.Emit(OpCodes.Pop);
				il.Emit(OpCodes.Pop);
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObject);
				il.Emit(OpCodes.Unbox_Any, typeof(short));
				il.Emit(OpCodes.Ldc_I4, (int)SubTypeHandle);
				il.Emit(OpCodes.Ceq);
				il.Emit(OpCodes.Brfalse_S, EOFNet);
			}
		}

		private static void EmitPortableWriteInstruction(FieldInfo field, ILGenerator il)
		{
			MethodInfo meth = null;
			if(field != null)
			{
				Type fieldType = field.FieldType;
				il.Emit(OpCodes.Ldfld, field);
				if(fieldType.IsPrimitive)
				{
					ISerializationSurrogate surrogateForType = TypeSurrogateSelector.GetSurrogateForType(fieldType, null);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldc_I4, (int)surrogateForType.TypeHandle);
					il.Emit(OpCodes.Conv_I2);
					MethodInfo method = typeof(CompactBinaryWriter).GetMethod("Write", new Type[] {
						typeof(short)
					});
					if(method != null)
					{
						il.Emit(OpCodes.Callvirt, method);
					}
					meth = typeof(CompactBinaryWriter).GetMethod("Write", new Type[] {
						fieldType
					});
					if(meth != null)
					{
						il.Emit(OpCodes.Callvirt, meth);
					}
				}
				if(meth == null)
				{
					if(fieldType.IsValueType)
						il.Emit(OpCodes.Box, fieldType);
					if(fieldType.IsInterface || !fieldType.IsPrimitive)
						il.Emit(OpCodes.Callvirt, _compactBinaryWriterWriteObject);
					else
					{
						il.Emit(OpCodes.Ldtoken, fieldType);
						il.Emit(OpCodes.Call, _type_GetTypeFromHandle);
						il.Emit(OpCodes.Callvirt, _compactBinaryWriterWriteObjectAs);
					}
				}
			}
			else
				throw new NotSupportedException();
		}

		private static void EmitPortableWriterMethod(Type type, ILGenerator il)
		{
			List<FieldInfo> allFields = new List<FieldInfo>();
			allFields = GetAllFields(type, allFields);
			string[][] strArray = null;
			if(_attributeOrder != null)
				strArray = (string[][])_attributeOrder[type];
			LocalBuilder local = il.DeclareLocal(type);
			il.Emit(OpCodes.Ldarg_1);
			if(type.IsValueType)
				il.Emit(OpCodes.Unbox_Any, type);
			else
				il.Emit(OpCodes.Castclass, type);
			il.Emit(OpCodes.Stloc_0);
			for(int i = 0; i < allFields.Count; i++)
			{
				FieldInfo field = allFields[i];
				bool flag = false;
				if((i < strArray[1].Length) && (strArray[1][i] == "0"))
					flag = true;
				if((field != null) || !flag)
				{
					il.Emit(OpCodes.Ldarg_0);
					if(type.IsValueType)
						il.Emit(OpCodes.Ldloca_S, local);
					else
						il.Emit(OpCodes.Ldloc_0);
					EmitPortableWriteInstruction(field, il);
				}
				else
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Newobj, typeof(SkipSerializationSurrogate).GetConstructor(Type.EmptyTypes));
					il.Emit(OpCodes.Callvirt, _compactBinaryWriterWriteObject);
				}
			}
			il.Emit(OpCodes.Ret);
		}

		private static void EmitReadInstruction(FieldInfo field, ILGenerator il)
		{
			MethodInfo meth = null;
			if(field != null)
			{
				Type fieldType = field.FieldType;
				if(fieldType.IsPrimitive)
				{
					string name = "Read" + fieldType.Name;
					meth = typeof(CompactBinaryReader).GetMethod(name);
					if(meth != null)
						il.Emit(OpCodes.Callvirt, meth);
				}
				if(meth == null)
				{
					if(fieldType.IsInterface || !fieldType.IsPrimitive)
						il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObject);
					else
					{
						il.Emit(OpCodes.Ldtoken, fieldType);
						il.Emit(OpCodes.Call, _type_GetTypeFromHandle);
						il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObjectAs);
					}
					if(fieldType.IsValueType)
						il.Emit(OpCodes.Unbox_Any, fieldType);
					else
						il.Emit(OpCodes.Castclass, fieldType);
				}
				il.Emit(OpCodes.Stfld, field);
			}
			else
			{
				il.Emit(OpCodes.Callvirt, _compactBinaryReaderReadObject);
				il.Emit(OpCodes.Unbox_Any, typeof(bool));
				il.Emit(OpCodes.Pop);
				il.Emit(OpCodes.Brtrue, EOFNet);
			}
		}

		private static void EmitWriteInstruction(FieldInfo field, ILGenerator il)
		{
			MethodInfo meth = null;
			if(field != null)
			{
				Type fieldType = field.FieldType;
				il.Emit(OpCodes.Ldfld, field);
				if(fieldType.IsPrimitive)
				{
					meth = typeof(CompactBinaryWriter).GetMethod("Write", new Type[] {
						fieldType
					});
					if(meth != null)
					{
						il.Emit(OpCodes.Callvirt, meth);
					}
				}
				if(meth == null)
				{
					if(fieldType.IsValueType)
					{
						il.Emit(OpCodes.Box, fieldType);
					}
					if(fieldType.IsInterface || !fieldType.IsPrimitive)
					{
						il.Emit(OpCodes.Callvirt, _compactBinaryWriterWriteObject);
					}
					else
					{
						il.Emit(OpCodes.Ldtoken, fieldType);
						il.Emit(OpCodes.Call, _type_GetTypeFromHandle);
						il.Emit(OpCodes.Callvirt, _compactBinaryWriterWriteObjectAs);
					}
				}
			}
			else
				throw new NotSupportedException();
		}
	}
}
