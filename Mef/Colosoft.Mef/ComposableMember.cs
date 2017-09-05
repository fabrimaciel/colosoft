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

namespace Colosoft.Mef
{
	/// <summary>
	/// Representa um membro de composição.
	/// </summary>
	abstract class ComposableMember
	{
		private static MethodInfo _ToLazyTM;

		private static MethodInfo _ToLazyT;

		/// <summary>
		/// Recupera o tipo deo elemento generico.
		/// </summary>
		private Type _elementType;

		/// <summary>
		/// tipo do objeto que o membro pertence a exportável.
		/// </summary>
		public virtual Type DeclaringType
		{
			get
			{
				return this.Member.DeclaringType;
			}
		}

		/// <summary>
		/// Tipo do elemento.
		/// </summary>
		public Type ElementType
		{
			get
			{
				if(_elementType == null)
					_elementType = this.GetElementType();
				return _elementType;
			}
		}

		/// <summary>
		/// Identifica se o tipo possui cargar tardia.
		/// </summary>
		public abstract bool IsLazyType
		{
			get;
		}

		/// <summary>
		/// Identifica o valor pode ser recuperado do membro.
		/// </summary>
		public abstract bool IsReadable
		{
			get;
		}

		/// <summary>
		/// Identifica se é necessário uma instancia para o valor ser recuperado ou definido.
		/// </summary>
		public abstract bool IsInstanceNeeded
		{
			get;
		}

		/// <summary>
		/// Identifica se o valor pode ser definido para o membro.
		/// </summary>
		public abstract bool IsWritable
		{
			get;
		}

		/// <summary>
		/// Instancia do membro associado.
		/// </summary>
		protected MemberInfo Member
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do retorno do valor.
		/// </summary>
		public abstract Type ReturnType
		{
			get;
		}

		/// <summary>
		/// Função usada para recuperar o valor do membro.
		/// </summary>
		protected Func<object, object> ValueGetter
		{
			get;
			set;
		}

		/// <summary>
		/// Função usada para definir o valor do membro.
		/// </summary>
		protected Action<object, object> ValueSetter
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static ComposableMember()
		{
			_ToLazyTM = typeof(ComposableMember).GetMethod("ToLazyTM", BindingFlags.NonPublic | BindingFlags.Static);
			_ToLazyT = typeof(ComposableMember).GetMethod("ToLazyT", BindingFlags.NonPublic | BindingFlags.Static);
		}

		/// <summary>
		/// Cria a instancia a partir das informações do membro.
		/// </summary>
		/// <param name="member">Membro que será exportado.</param>
		protected ComposableMember(MemberInfo member)
		{
			Member = member;
		}

		/// <summary>
		/// Cria um novo <see cref="ComposableMember"/> para o especifico <paramref name="member"/>
		/// </summary>
		/// <param name="member">Membro que será usado na criação.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static ComposableMember Create(MemberInfo member)
		{
			member.Require("member").NotNull();
			switch(member.MemberType)
			{
			case MemberTypes.Field:
				return new ComposableField(member);
			case MemberTypes.Method:
				return new ComposableMethod(member);
			case MemberTypes.Property:
				return new ComposableProperty(member);
			case MemberTypes.Constructor:
				return new ComposableConstructor(member);
			case MemberTypes.NestedType:
			case MemberTypes.TypeInfo:
				return new ComposableType(member);
			}
			throw new InvalidOperationException("Unsupported member type.");
		}

		/// <summary>
		/// Recupera o valor importado com base no exports.
		/// </summary>
		/// <param name="exports"></param>
		/// <returns></returns>
		public object GetImportValueFromExports(IEnumerable<System.ComponentModel.Composition.Primitives.Export> exports)
		{
			return this.ReturnType.IsEnumerable() ? this.GetImportValueCollection(exports) : this.GetImportValueSingle(exports);
		}

		/// <summary>
		/// Recupera o valor do membro.
		/// </summary>
		/// <param name="instance">Instancia de onde o valor será recuperado.</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public virtual object GetValue(object instance)
		{
			if(this.ValueGetter == null)
				throw new InvalidOperationException("No function for retrieving the value has been set.");
			return this.ValueGetter.Invoke(instance);
		}

		/// <summary>
		/// Define o valor para o membro.
		/// </summary>
		/// <param name="instance">Instancia onde o membro está inserido.</param>
		/// <param name="value">Valor que será atribuído.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public virtual void SetValue(object instance, object value)
		{
			if(!this.IsWritable || this.ValueSetter == null)
			{
				throw new InvalidOperationException("Either the member is not writeable or oo function for assigning the value has been set.");
			}
			this.ValueSetter.Invoke(instance, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Type ToLazyType()
		{
			return this.ElementType != null ? this.ElementType.ToLazyCollectionType() : this.ReturnType.ToLazyType();
		}

		/// <summary>
		/// Recupera o tipo do elemento.
		/// </summary>
		/// <returns></returns>
		private Type GetElementType()
		{
			if(this.ReturnType.IsEnumerable())
			{
				Type[] collectionInterfaces = new[] {
					typeof(ICollection<>),
					typeof(IEnumerable<>),
					typeof(IList<>)
				};
				if(ReturnType.IsGenericType)
				{
					var genericTypeDefinition = ReturnType.GetGenericTypeDefinition();
					if(collectionInterfaces.Contains(genericTypeDefinition))
						return ReturnType.GetGenericArguments()[0];
				}
				foreach (Type targetInterface in ReturnType.GetInterfaces().Where(i => i.IsGenericType))
				{
					var genericTypeDefinition = targetInterface.GetGenericTypeDefinition();
					if(collectionInterfaces.Contains(genericTypeDefinition))
					{
						return targetInterface.GetGenericArguments()[0];
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Recupera o valor importado.
		/// </summary>
		/// <param name="exports"></param>
		/// <returns></returns>
		private object GetImportValueCollection(IEnumerable<System.ComponentModel.Composition.Primitives.Export> exports)
		{
			if(exports.Count() == 0)
				return null;
			bool isLazy = this.ElementType.IsLazyType();
			if(!isLazy)
			{
				var exportEnumerator = exports.GetEnumerator();
				System.Collections.IList collection = null;
				if(exportEnumerator.MoveNext())
				{
					Type[] genericTypeInformation = this.ReturnType.GetGenericArguments();
					if(genericTypeInformation.Length == 0)
					{
						return exportEnumerator.Current.Value;
					}
					else
					{
						Type collectionType = typeof(List<>).MakeGenericType(genericTypeInformation[0]);
						collection = (System.Collections.IList)Activator.CreateInstance(collectionType);
					}
					collection.Add(exportEnumerator.Current.Value);
					while (exportEnumerator.MoveNext())
						collection.Add(exportEnumerator.Current.Value);
				}
				else
					collection = new System.Collections.ArrayList();
				return collection;
			}
			var exportCollection = (System.Collections.IList)Activator.CreateInstance(this.ToLazyType(), null);
			MethodInfo lazyFactory;
			if(isLazy)
			{
				var genericArgs = ElementType.GetGenericArguments();
				if(genericArgs.Length == 0)
				{
					throw new InvalidOperationException(string.Format("Not found generic arguments for type '{0}' in lazy.", ElementType.FullName));
				}
				Type contractType = genericArgs[0];
				if(genericArgs.Length == 2)
				{
					Type metadataViewType = genericArgs.Length == 2 ? genericArgs[1] : typeof(IDictionary<string, object>);
					lazyFactory = _ToLazyTM.MakeGenericMethod(contractType, metadataViewType);
				}
				else
					lazyFactory = _ToLazyT.MakeGenericMethod(contractType);
			}
			else
				lazyFactory = _ToLazyT.MakeGenericMethod(ElementType);
			foreach (var item in exports)
			{
				var typed = lazyFactory.Invoke(null, new[] {
					item
				});
				exportCollection.Add(typed);
			}
			return exportCollection;
		}

		private static Lazy<T, TMetadata> ToLazyTM<T, TMetadata>(System.ComponentModel.Composition.Primitives.Export export)
		{
			Func<T> valueFactory = () => (T)export.Value;
			TMetadata metadata = System.ComponentModel.Composition.AttributedModelServices.GetMetadataView<TMetadata>(export.Metadata);
			return new Lazy<T, TMetadata>(valueFactory, metadata);
		}

		/// <summary>
		/// Cria um Lazy para exportar o valor do membro.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="export"></param>
		/// <returns></returns>
		private static Lazy<T> ToLazyT<T>(System.ComponentModel.Composition.Primitives.Export export)
		{
			Func<T> valueFactory = () => (T)export.Value;
			return new Lazy<T>(valueFactory);
		}

		/// <summary>
		/// Recupera o valor simple com base nos exports.
		/// </summary>
		/// <param name="exports"></param>
		/// <returns></returns>
		private object GetImportValueSingle(IEnumerable<System.ComponentModel.Composition.Primitives.Export> exports)
		{
			var single = exports.FirstOrDefault();
			if(single == null)
				return null;
			MethodInfo lazyFactory;
			if(!this.ReturnType.IsLazyType())
				return single.Value;
			if(!this.ReturnType.IsGenericType)
				return single;
			var genericArgs = ReturnType.GetGenericArguments();
			Type contractType = genericArgs[0];
			if(genericArgs.Length == 2)
			{
				Type metadataViewType = genericArgs.Length == 2 ? genericArgs[1] : typeof(IDictionary<string, object>);
				lazyFactory = _ToLazyTM.MakeGenericMethod(contractType, metadataViewType);
			}
			else
				lazyFactory = _ToLazyT.MakeGenericMethod(contractType);
			return lazyFactory.Invoke(null, new[] {
				single
			});
		}
	}
}
