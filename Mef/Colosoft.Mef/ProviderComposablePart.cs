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
using System.ComponentModel.Composition.Primitives;

namespace Colosoft.Mef
{
	/// <summary>
	/// 
	/// </summary>
	class ProviderComposablePart : ComposablePart
	{
		/// <summary>
		/// Cria uma nova instancia usadando a definição informada.
		/// </summary>
		/// <param name="definition"></param>
		public ProviderComposablePart(ProviderComposablePartDefinition definition) : this(null, definition)
		{
		}

		/// <summary>
		/// Cria uma nova instancia com base na instancia informada.
		/// </summary>
		/// <param name="instance"></param>
		public ProviderComposablePart(object instance) : this(instance, null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="definition"></param>
		private ProviderComposablePart(object instance, ProviderComposablePartDefinition definition)
		{
			this.Definition = definition;
			this.ImportedValues = new Dictionary<System.ComponentModel.Composition.Primitives.ImportDefinition, ImportableInfo>();
			this.Instance = instance;
		}

		/// <summary>
		/// Instancia da definição da parte associada.
		/// </summary>
		/// <value>A <see cref="ProviderComposablePartDefinition"/> object.</value>
		public ProviderComposablePartDefinition Definition
		{
			get;
			private set;
		}

		/// <summary>
		/// Lista de todas as instancias <see cref="ExportDefinition"/> conhecidas pela parte.
		/// </summary>
		public override IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> ExportDefinitions
		{
			get
			{
				return this.Definition.ExportDefinitions;
			}
		}

		/// <summary>
		/// Lista de todas as instancias <see cref="ImportDefinition"/> conhecidas pela parte.
		/// </summary>
		public override IEnumerable<System.ComponentModel.Composition.Primitives.ImportDefinition> ImportDefinitions
		{
			get
			{
				var imports = this.Definition.ImportDefinitions.Concat(this.Definition.ImportsParametersDefinitions).ToArray();
				return imports;
			}
		}

		/// <summary>
		/// Recupera e define o dicionário de todos os valores dos imports.
		/// </summary>
		private Dictionary<System.ComponentModel.Composition.Primitives.ImportDefinition, ImportableInfo> ImportedValues
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia associada.
		/// </summary>
		private object Instance
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a parte já foi composta.
		/// </summary>
		private bool IsComposed
		{
			get;
			set;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ComposableMember"/>, specified by the parameter, if it's needed.
		/// </summary>
		/// <param name="exportable">The exportable member to create an instance of, if it is required.</param>
		/// <returns>An instance of the object specified by the provided <see cref="ComposableMember"/>.</returns>
		private object GetActivatedInstance(ComposableMember exportable)
		{
			if(Definition.PartCreationPolicy == System.ComponentModel.Composition.CreationPolicy.Shared && this.Instance != null)
				return this.Instance;
			var constructor = exportable as ComposableConstructor;
			if(constructor == null)
				try
				{
					if(this.Definition.UseDispatcher && Colosoft.Threading.DispatcherManager.Dispatcher != null && Colosoft.Threading.DispatcherManager.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
					{
						this.Instance = Colosoft.Threading.DispatcherManager.Dispatcher.Invoke(new Func<Type, object>(Activator.CreateInstance), new object[] {
							exportable.DeclaringType
						});
					}
					else
						this.Instance = Activator.CreateInstance(exportable.DeclaringType);
				}
				catch(Exception ex)
				{
					if(ex is System.Reflection.TargetInvocationException && ex.InnerException != null)
						ex = ex.InnerException;
					throw new ActivatedInstanceException(string.Format("An error ocurred when create instance for type '{0}'. \r\nMessage: {1}", exportable.DeclaringType, ex.Message), ex);
				}
			else
			{
				var ctorParameters = constructor.Constructor.GetParameters();
				var parameters = new object[ctorParameters.Length];
				var i = 0;
				foreach (var j in Definition.ImportsParametersDefinitions)
				{
					var value = this.ImportedValues[j];
					value.ClearExportValue();
					try
					{
						parameters[i] = value.GetValue();
					}
					catch(Exception ex)
					{
						throw new ComposableMemberParameterException(exportable, j, ex);
					}
					i++;
				}
				try
				{
					if(this.Definition.UseDispatcher && Colosoft.Threading.DispatcherManager.Dispatcher != null && Colosoft.Threading.DispatcherManager.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
					{
						var declaringType = exportable.DeclaringType;
						this.Instance = Colosoft.Threading.DispatcherManager.Dispatcher.Invoke(new Func<Type, object[], object>(Activator.CreateInstance), new object[] {
							declaringType,
							parameters
						});
					}
					else
						this.Instance = constructor.Constructor.Invoke(parameters);
				}
				catch(Exception ex)
				{
					if(ex is System.Reflection.TargetInvocationException && ex.InnerException != null)
						ex = ex.InnerException;
					throw new ActivatedInstanceException(string.Format("An error ocurred when create instance for type '{0}'. \r\nMessage: {1}", exportable.DeclaringType, ex.Message), ex);
				}
			}
			if(this.IsComposed)
				this.SatisfyPostCompositionImports();
			return this.Instance;
		}

		/// <summary>
		/// Recupera o valor exportado.
		/// </summary>
		/// <param name="definition"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public override object GetExportedValue(System.ComponentModel.Composition.Primitives.ExportDefinition definition)
		{
			try
			{
				ProviderExportDefinition export = definition as ProviderExportDefinition;
				if(export == null)
					throw new InvalidOperationException("The supplied export definition was of an unknown type.");
				if(export.Member == null)
					throw new InvalidOperationException(string.Format("Not found member to export definition. {0}", export.ContractName));
				ComposableMember exportable = export.Member.ToComposableMember();
				object instance = null;
				if(exportable.IsInstanceNeeded)
					instance = this.GetActivatedInstance(exportable);
				object value = exportable.GetValue(instance);
				return value;
			}
			catch(Exception ex)
			{
				ComposablePartErrorHandler.NotifyGetExportedValueError(definition, ex);
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Activate()
		{
			OnComposed();
		}

		/// <summary>
		/// 
		/// </summary>
		public void OnComposed()
		{
			if(this.Instance != null)
			{
				if(!IsComposed)
				{
					SatisfyPostCompositionImports();
				}
			}
			IsComposed = true;
		}

		/// <summary>
		/// 
		/// </summary>
		private void SatisfyPostCompositionImports()
		{
			IEnumerable<System.ComponentModel.Composition.Primitives.ImportDefinition> members = this.ImportDefinitions.Where(import => !import.IsPrerequisite);
			foreach (var i in members)
			{
				var definition = i as ProviderImportDefinition;
				if(definition == null)
					continue;
				ImportableInfo value;
				if(this.ImportedValues.TryGetValue(definition, out value))
				{
					ComposableMember importable = definition.Member.ToComposableMember();
					importable.SetValue(this.Instance, value.GetValue());
				}
			}
		}

		/// <summary>
		/// Define o import.
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="exports"></param>
		/// <exception cref="InvalidOperationException">The supplied import definition was of an unknown type.</exception>
		public override void SetImport(System.ComponentModel.Composition.Primitives.ImportDefinition definition, IEnumerable<Export> exports)
		{
			ProviderImportDefinition import = definition as ProviderImportDefinition;
			this.ImportedValues[definition] = new ImportableInfo(definition, import, exports, this);
		}

		class ImportableInfo
		{
			private System.ComponentModel.Composition.Primitives.ImportDefinition _definition;

			private ProviderImportDefinition _import;

			private IEnumerable<Export> _exports;

			private ProviderComposablePart _composablePart;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="definition"></param>
			/// <param name="import"></param>
			/// <param name="exports"></param>
			/// <param name="composablePart"></param>
			public ImportableInfo(System.ComponentModel.Composition.Primitives.ImportDefinition definition, ProviderImportDefinition import, IEnumerable<Export> exports, ProviderComposablePart composablePart)
			{
				_definition = definition;
				_import = import;
				_exports = exports;
				_composablePart = composablePart;
			}

			/// <summary>
			/// Limpa os valores dos exports associados.
			/// </summary>
			public void ClearExportValue()
			{
				var field = typeof(Export).GetField("_exportedValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var emptyValue = typeof(Export).GetField("_EmptyValue", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
				foreach (var export in _exports)
					field.SetValue(export, emptyValue);
			}

			/// <summary>
			/// Recupera o valor.
			/// </summary>
			/// <returns></returns>
			public object GetValue()
			{
				ComposableMember importable = null;
				if(_import == null)
				{
					if(_definition is ProviderParameterImportDefinition)
						importable = ((ProviderParameterImportDefinition)_definition).Parameter.ToComposableMember();
					else
						throw new InvalidOperationException("The supplied import definition was of an unknown type.");
				}
				else
					importable = _import.Member.ToComposableMember();
				object value = importable.GetImportValueFromExports(_exports);
				return value;
			}
		}
	}
	/// <summary>
	/// Classe com os evento que auxiliam a tratar o errors ocorridos na composição de partes.
	/// </summary>
	public static class ComposablePartErrorHandler
	{
		/// <summary>
		/// Evento acionado para tratar o erro ao recupera o valor exportado.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public static event Action<ExportDefinition, Exception> GetExportedValueError;

		/// <summary>
		/// Notifica o erro ocorrido.
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="e"></param>
		internal static void NotifyGetExportedValueError(ExportDefinition definition, Exception e)
		{
			if(GetExportedValueError != null)
				GetExportedValueError(definition, e);
		}
	}
}
