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
using System.Diagnostics;

namespace Colosoft.Mef
{
	/// <summary>
	/// Implementação da classe para compor as definições que irão ser usadas
	/// para criar novos objetos.
	/// </summary>
	class ProviderComposablePartDefinition : ComposablePartDefinition
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IEnumerable<ExportDefinition> _exports;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Lazy<IEnumerable<ImportDefinition>> _imports;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IEnumerable<ProviderParameterImportDefinition> _importsParameters;

		private ProviderParameterImportDefinitionContainer _parameterImportDefinitions;

		private ProviderComposablePart _composablePart;

		private ComposablePartCatalog _catalog;

		private Reflection.TypeName _typeName;

		private System.ComponentModel.Composition.CreationPolicy _partCreationPolicy;

		private bool _useDispatcher;

		/// <summary>
		/// Catálogo associado.
		/// </summary>
		public ComposablePartCatalog Catalog
		{
			get
			{
				return _catalog;
			}
		}

		/// <summary>
		/// Criação.
		/// </summary>
		public System.ComponentModel.Composition.CreationPolicy PartCreationPolicy
		{
			get
			{
				return _partCreationPolicy;
			}
		}

		/// <summary>
		/// Recupera as definições de exportação.
		/// </summary>
		public override IEnumerable<ExportDefinition> ExportDefinitions
		{
			[DebuggerStepThrough]
			get
			{
				return _exports;
			}
		}

		/// <summary>
		/// Recupera as definições de importação.
		/// </summary>
		/// <value>A collection of <see cref="ImportDefinition"/> objects.</value>
		public override IEnumerable<ImportDefinition> ImportDefinitions
		{
			[DebuggerStepThrough]
			get
			{
				return _imports.Value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<ProviderParameterImportDefinition> ImportsParametersDefinitions
		{
			get
			{
				if(_importsParameters == null)
					_importsParameters = _parameterImportDefinitions.GetImportDefinitions();
				return _importsParameters;
			}
		}

		/// <summary>
		/// Identifica se é para usar o dispatcher para criar a instancia.
		/// </summary>
		public bool UseDispatcher
		{
			get
			{
				return _useDispatcher;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="catalog"></param>
		/// <param name="typeName"></param>
		/// <param name="exportDefinitions"></param>
		/// <param name="importDefinitions"></param>
		/// <param name="parameterImportDefinitions"></param>
		/// <param name="partCreationPolicy"></param>
		/// <param name="useDispatcher"></param>
		public ProviderComposablePartDefinition(ComposablePartCatalog catalog, Colosoft.Reflection.TypeName typeName, IList<ProviderExportDefinition> exportDefinitions, Lazy<IEnumerable<ImportDefinition>> importDefinitions, ProviderParameterImportDefinitionContainer parameterImportDefinitions, System.ComponentModel.Composition.CreationPolicy partCreationPolicy, bool useDispatcher)
		{
			_exports = exportDefinitions.Cast<ExportDefinition>().ToArray();
			_imports = importDefinitions;
			_parameterImportDefinitions = parameterImportDefinitions;
			_catalog = catalog;
			_typeName = typeName;
			_partCreationPolicy = partCreationPolicy;
			_useDispatcher = useDispatcher;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override ComposablePart CreatePart()
		{
			if(_composablePart == null)
				_composablePart = new ProviderComposablePart(this);
			return _composablePart;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}, {1}", _typeName.FullName, _typeName.AssemblyName.Name);
		}
	}
}
