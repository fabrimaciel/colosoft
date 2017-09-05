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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Classe responável pela criação de outras classes.
	/// </summary>
	class ClassFactory
	{
		private int _classCount;

		private Dictionary<Signature, Type> _classes;

		public static readonly ClassFactory Instance = new ClassFactory();

		private ModuleBuilder _module;

		private ReaderWriterLock _rwLock;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		private ClassFactory()
		{
			var name = new System.Reflection.AssemblyName("DynamicClasses");
			_module = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run).DefineDynamicModule("Module");
			_classes = new Dictionary<Signature, Type>();
			_rwLock = new ReaderWriterLock();
		}

		/// <summary>
		/// Cria o tipo da classe dinâmmica com as propriedades informadas.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private Type CreateDynamicClass(DynamicProperty[] properties)
		{
			Type type2;
			LockCookie lockCookie = this._rwLock.UpgradeToWriterLock(-1);
			try
			{
				string name = "DynamicClass" + (this._classCount + 1);
				TypeBuilder tb = this._module.DefineType(name, System.Reflection.TypeAttributes.Public, typeof(DynamicClass));
				var fields = this.GenerateProperties(tb, properties);
				GenerateEquals(tb, fields);
				GenerateGetHashCode(tb, fields);
				Type type = tb.CreateType();
				_classCount++;
				type2 = type;
			}
			finally
			{
				_rwLock.DowngradeFromWriterLock(ref lockCookie);
			}
			return type2;
		}

		/// <summary>
		/// Gera um estrutura de comparação para os campos informados.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="fields"></param>
		private void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
		{
			ILGenerator iLGenerator = tb.DefineMethod("Equals", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, typeof(bool), new Type[] {
				typeof(object)
			}).GetILGenerator();
			LocalBuilder local = iLGenerator.DeclareLocal(tb);
			Label label = iLGenerator.DefineLabel();
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Isinst, tb);
			iLGenerator.Emit(OpCodes.Stloc, local);
			iLGenerator.Emit(OpCodes.Ldloc, local);
			iLGenerator.Emit(OpCodes.Brtrue_S, label);
			iLGenerator.Emit(OpCodes.Ldc_I4_0);
			iLGenerator.Emit(OpCodes.Ret);
			iLGenerator.MarkLabel(label);
			foreach (FieldInfo info in fields)
			{
				Type fieldType = info.FieldType;
				Type type2 = typeof(EqualityComparer<>).MakeGenericType(new Type[] {
					fieldType
				});
				label = iLGenerator.DefineLabel();
				iLGenerator.EmitCall(OpCodes.Call, type2.GetMethod("get_Default"), null);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldfld, info);
				iLGenerator.Emit(OpCodes.Ldloc, local);
				iLGenerator.Emit(OpCodes.Ldfld, info);
				iLGenerator.EmitCall(OpCodes.Callvirt, type2.GetMethod("Equals", new Type[] {
					fieldType,
					fieldType
				}), null);
				iLGenerator.Emit(OpCodes.Brtrue_S, label);
				iLGenerator.Emit(OpCodes.Ldc_I4_0);
				iLGenerator.Emit(OpCodes.Ret);
				iLGenerator.MarkLabel(label);
			}
			iLGenerator.Emit(OpCodes.Ldc_I4_1);
			iLGenerator.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Gera um método para calcula o hashcode dos campos informados.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="fields"></param>
		private void GenerateGetHashCode(TypeBuilder tb, FieldInfo[] fields)
		{
			ILGenerator iLGenerator = tb.DefineMethod("GetHashCode", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, typeof(int), Type.EmptyTypes).GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldc_I4_0);
			foreach (FieldInfo info in fields)
			{
				Type fieldType = info.FieldType;
				Type type2 = typeof(EqualityComparer<>).MakeGenericType(new Type[] {
					fieldType
				});
				iLGenerator.EmitCall(OpCodes.Call, type2.GetMethod("get_Default"), null);
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldfld, info);
				iLGenerator.EmitCall(OpCodes.Callvirt, type2.GetMethod("GetHashCode", new Type[] {
					fieldType
				}), null);
				iLGenerator.Emit(OpCodes.Xor);
			}
			iLGenerator.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Gera os campos das propriedades informadas.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		private FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
		{
			FieldInfo[] infoArray = new FieldBuilder[properties.Length];
			for(int i = 0; i < properties.Length; i++)
			{
				DynamicProperty property = properties[i];
				FieldBuilder field = tb.DefineField("_" + property.Name, property.Type, FieldAttributes.Private);
				PropertyBuilder builder2 = tb.DefineProperty(property.Name, PropertyAttributes.HasDefault, property.Type, null);
				MethodBuilder mdBuilder = tb.DefineMethod("get_" + property.Name, MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, property.Type, Type.EmptyTypes);
				ILGenerator iLGenerator = mdBuilder.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				iLGenerator.Emit(OpCodes.Ldfld, field);
				iLGenerator.Emit(OpCodes.Ret);
				MethodBuilder builder4 = tb.DefineMethod("set_" + property.Name, MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, null, new Type[] {
					property.Type
				});
				ILGenerator generator2 = builder4.GetILGenerator();
				generator2.Emit(OpCodes.Ldarg_0);
				generator2.Emit(OpCodes.Ldarg_1);
				generator2.Emit(OpCodes.Stfld, field);
				generator2.Emit(OpCodes.Ret);
				builder2.SetGetMethod(mdBuilder);
				builder2.SetSetMethod(builder4);
				infoArray[i] = field;
			}
			return infoArray;
		}

		/// <summary>
		/// Recupera a classe dinâmica para as propriedades informadas.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
		{
			Type type2;
			_rwLock.AcquireReaderLock(-1);
			try
			{
				Type type;
				Signature key = new Signature(properties);
				if(!_classes.TryGetValue(key, out type))
				{
					type = CreateDynamicClass(key._properties);
					_classes.Add(key, type);
				}
				type2 = type;
			}
			finally
			{
				_rwLock.ReleaseReaderLock();
			}
			return type2;
		}
	}
}
