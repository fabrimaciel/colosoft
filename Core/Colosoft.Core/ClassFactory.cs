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

namespace Colosoft.Reflection.Dynamic
{
	/// <summary>
	/// Factory usada na criação das classe dinamicas.
	/// </summary>
	class ClassFactory
	{
		public static readonly ClassFactory Instance = new ClassFactory();

		System.Reflection.Emit.ModuleBuilder module;

		private Dictionary<Signature, Type> classes;

		private int classCount;

		private System.Threading.ReaderWriterLock rwLock;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		private ClassFactory()
		{
			var name = new System.Reflection.AssemblyName("DynamicClasses");
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, System.Reflection.Emit.AssemblyBuilderAccess.Run);
			#if ENABLE_LINQ_PARTIAL_TRUST
			            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
			try
			{
				module = assembly.DefineDynamicModule("Module");
			}
			finally
			{
				#if ENABLE_LINQ_PARTIAL_TRUST
				                PermissionSet.RevertAssert();
#endif
			}
			classes = new Dictionary<Signature, Type>();
			rwLock = new System.Threading.ReaderWriterLock();
		}

		/// <summary>
		/// Cria o tipo da classe dinâmica.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private Type CreateDynamicClass(DynamicProperty[] properties)
		{
			var cookie = rwLock.UpgradeToWriterLock(System.Threading.Timeout.Infinite);
			try
			{
				string typeName = "DynamicClass" + (classCount + 1);
				#if ENABLE_LINQ_PARTIAL_TRUST
				                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
				try
				{
					var tb = this.module.DefineType(typeName, System.Reflection.TypeAttributes.Class | System.Reflection.TypeAttributes.Public, typeof(DynamicClass));
					var fields = GenerateProperties(tb, properties);
					GenerateEquals(tb, fields);
					GenerateGetHashCode(tb, fields);
					Type result = tb.CreateType();
					classCount++;
					return result;
				}
				finally
				{
					#if ENABLE_LINQ_PARTIAL_TRUST
					                    PermissionSet.RevertAssert();
#endif
				}
			}
			finally
			{
				rwLock.DowngradeFromWriterLock(ref cookie);
			}
		}

		/// <summary>
		/// Gera as propriedades para o tipo que será criado.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		private System.Reflection.FieldInfo[] GenerateProperties(System.Reflection.Emit.TypeBuilder tb, DynamicProperty[] properties)
		{
			var fields = new System.Reflection.Emit.FieldBuilder[properties.Length];
			for(int i = 0; i < properties.Length; i++)
			{
				DynamicProperty dp = properties[i];
				var fb = tb.DefineField("_" + dp.Name, dp.Type, System.Reflection.FieldAttributes.Private);
				var pb = tb.DefineProperty(dp.Name, System.Reflection.PropertyAttributes.HasDefault, dp.Type, null);
				var mbGet = tb.DefineMethod("get_" + dp.Name, System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.SpecialName | System.Reflection.MethodAttributes.HideBySig, dp.Type, Type.EmptyTypes);
				var genGet = mbGet.GetILGenerator();
				genGet.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
				genGet.Emit(System.Reflection.Emit.OpCodes.Ldfld, fb);
				genGet.Emit(System.Reflection.Emit.OpCodes.Ret);
				var mbSet = tb.DefineMethod("set_" + dp.Name, System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.SpecialName | System.Reflection.MethodAttributes.HideBySig, null, new Type[] {
					dp.Type
				});
				var genSet = mbSet.GetILGenerator();
				genSet.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
				genSet.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
				genSet.Emit(System.Reflection.Emit.OpCodes.Stfld, fb);
				genSet.Emit(System.Reflection.Emit.OpCodes.Ret);
				pb.SetGetMethod(mbGet);
				pb.SetSetMethod(mbSet);
				fields[i] = fb;
			}
			return fields;
		}

		/// <summary>
		/// Cria o método de comparação Equals.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="fields"></param>
		public void GenerateEquals(System.Reflection.Emit.TypeBuilder tb, System.Reflection.FieldInfo[] fields)
		{
			var mb = tb.DefineMethod("Equals", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot | System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig, typeof(bool), new Type[] {
				typeof(object)
			});
			var gen = mb.GetILGenerator();
			var other = gen.DeclareLocal(tb);
			var next = gen.DefineLabel();
			gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
			gen.Emit(System.Reflection.Emit.OpCodes.Isinst, tb);
			gen.Emit(System.Reflection.Emit.OpCodes.Stloc, other);
			gen.Emit(System.Reflection.Emit.OpCodes.Ldloc, other);
			gen.Emit(System.Reflection.Emit.OpCodes.Brtrue_S, next);
			gen.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
			gen.Emit(System.Reflection.Emit.OpCodes.Ret);
			gen.MarkLabel(next);
			foreach (var field in fields)
			{
				Type ft = field.FieldType;
				Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
				next = gen.DefineLabel();
				gen.EmitCall(System.Reflection.Emit.OpCodes.Call, ct.GetMethod("get_Default"), null);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldloc, other);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
				gen.EmitCall(System.Reflection.Emit.OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] {
					ft,
					ft
				}), null);
				gen.Emit(System.Reflection.Emit.OpCodes.Brtrue_S, next);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
				gen.Emit(System.Reflection.Emit.OpCodes.Ret);
				gen.MarkLabel(next);
			}
			gen.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
			gen.Emit(System.Reflection.Emit.OpCodes.Ret);
		}

		/// <summary>
		/// Cria o método para calcular o hashcode.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="fields"></param>
		private void GenerateGetHashCode(System.Reflection.Emit.TypeBuilder tb, System.Reflection.FieldInfo[] fields)
		{
			var mb = tb.DefineMethod("GetHashCode", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot | System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig, typeof(int), Type.EmptyTypes);
			var gen = mb.GetILGenerator();
			gen.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
			foreach (var field in fields)
			{
				Type ft = field.FieldType;
				Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
				gen.EmitCall(System.Reflection.Emit.OpCodes.Call, ct.GetMethod("get_Default"), null);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
				gen.Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
				gen.EmitCall(System.Reflection.Emit.OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] {
					ft
				}), null);
				gen.Emit(System.Reflection.Emit.OpCodes.Xor);
			}
			gen.Emit(System.Reflection.Emit.OpCodes.Ret);
		}

		/// <summary>
		/// Recupera a classe dinâmica associada com as propriedades informada.
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
		{
			rwLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			try
			{
				Signature signature = new Signature(properties);
				Type type;
				if(!classes.TryGetValue(signature, out type))
				{
					type = CreateDynamicClass(signature.Properties);
					classes.Add(signature, type);
				}
				return type;
			}
			finally
			{
				rwLock.ReleaseReaderLock();
			}
		}
	}
}
