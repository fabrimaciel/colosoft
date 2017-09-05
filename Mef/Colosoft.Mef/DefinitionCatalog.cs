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
	/// Implementação do catalogo de definições.
	/// </summary>
	class DefinitionCatalog : ComposablePartCatalog
	{
		/// <summary>
		/// Fila da exceptions ocorridas.
		/// </summary>
		private Queue<Exception> _processingExceptions = new Queue<Exception>();

		private IQueryable<ComposablePartDefinition> _parts;

		private IEnumerable<PartDescription> _partDescriptions;

		private IAssemblyContainer _assemblies;

		/// <summary>
		/// Partes do catálogo.
		/// </summary>
		public override IQueryable<ComposablePartDefinition> Parts
		{
			[DebuggerStepThrough]
			get
			{
				if(_parts == null)
					_parts = GetComposablePartDefinitions().Cast<ComposablePartDefinition>().AsQueryable();
				return _parts;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <param name="partDescriptions"></param>
		public DefinitionCatalog(IAssemblyContainer assemblies, IEnumerable<PartDescription> partDescriptions)
		{
			assemblies.Require("assemblies").NotNull();
			partDescriptions.Require("partDescriptions").NotNull();
			_assemblies = assemblies;
			_partDescriptions = partDescriptions;
		}

		/// <summary>
		/// Gets a list of all composable part definitions which was discovered by the catalog.
		/// </summary>
		/// <returns>A <see cref="List{T}"/> object.</returns>
		private List<ProviderComposablePartDefinition> GetComposablePartDefinitions()
		{
			var definitions = new List<ProviderComposablePartDefinition>();
			foreach (PartDescription part in _partDescriptions)
			{
				var container = new ProviderParameterImportDefinitionContainer(part, _assemblies);
				definitions.Add(new ProviderComposablePartDefinition(this, part.TypeName, GetExportDefinitions(part), GetImportDefinitions(part, container), container, part.PartCreationPolicy, part.UseDispatcher));
			}
			return definitions;
		}

		/// <summary>
		/// Recupera a lista de todas as definições de export para a parte.
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		private IList<ProviderExportDefinition> GetExportDefinitions(PartDescription part)
		{
			var exports = new List<ProviderExportDefinition>();
			foreach (var i in part.Exports)
			{
				try
				{
					ExportDescription export = i;
					var getter = new ProviderExportDefinitionMemberGetter(this, part, export);
					exports.Add(new ProviderExportDefinition(getter.GetMember, export, part.PartCreationPolicy));
				}
				catch(Exception)
				{
					throw;
				}
			}
			return exports;
		}

		/// <summary>
		/// Recupera as definição para importar os parametros
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		private IList<ProviderParameterImportDefinition> GetParametersImportDefinitions(PartDescription part)
		{
			var imports = new List<ProviderParameterImportDefinition>();
			if(part.ImportingConstructor != null)
			{
				foreach (var i in part.ImportingConstructor.GetParameterImportDefinitions(_assemblies))
					imports.Add(new ProviderParameterImportDefinition(i.Item2, i.Item1, System.ComponentModel.Composition.CreationPolicy.Any));
			}
			return imports;
		}

		/// <summary>
		/// Recupera uma lista de todas as definições de import da parte.
		/// </summary>
		/// <param name="part"></param>
		/// <param name="providerParameterImportContainer"></param>
		/// <returns></returns>
		private Lazy<IEnumerable<ImportDefinition>> GetImportDefinitions(PartDescription part, ProviderParameterImportDefinitionContainer providerParameterImportContainer)
		{
			return new Lazy<IEnumerable<ImportDefinition>>(() =>  {
				var imports = new List<ImportDefinition>();
				foreach (var i in part.Imports)
				{
					ImportDescription import = i;
					imports.Add(new ProviderImportDefinition(new Lazy<System.Reflection.MemberInfo>(() => GetMemberInfo(part.TypeName, import.MemberName), false), import, part.PartCreationPolicy));
				}
				foreach (var i in providerParameterImportContainer.GetImportDefinitions())
					imports.Add(i);
				return imports;
			});
		}

		/// <summary>
		/// Recupera o membro específico.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="memberName"></param>
		/// <returns>A <see cref="System.Reflection.MemberInfo"/> object.</returns>
		public System.Reflection.MemberInfo GetMemberInfo(Colosoft.Reflection.TypeName typeName, string memberName)
		{
			typeName.Require("typeName").NotNull();
			System.Reflection.Assembly assembly = null;
			if(!_assemblies.TryGet(typeName.AssemblyName.Name, out assembly))
				throw new InvalidOperationException(string.Format("Not found assembly '{0}'", typeName.AssemblyName.Name));
			Type inspectedType = assembly.GetType(typeName.FullName, true, true);
			return (string.IsNullOrEmpty(memberName)) ? inspectedType : inspectedType.GetMember(memberName).FirstOrDefault();
		}

		/// <summary>
		/// Classe em encapsula a recupera do membro.
		/// </summary>
		class ProviderExportDefinitionMemberGetter
		{
			private DefinitionCatalog _catalog;

			private PartDescription _part;

			private ExportDescription _export;

			private System.Reflection.MemberInfo _member;

			private object _lock = new object();

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="catalog"></param>
			/// <param name="part"></param>
			/// <param name="export"></param>
			public ProviderExportDefinitionMemberGetter(DefinitionCatalog catalog, PartDescription part, ExportDescription export)
			{
				_catalog = catalog;
				_part = part;
				_export = export;
			}

			/// <summary>
			/// Instancia do membro associado.
			/// </summary>
			public System.Reflection.MemberInfo GetMember()
			{
				if(_member == null)
					if(_member == null)
					{
						if(_part != null && _part.ImportingConstructor != null)
							_member = (System.Reflection.MemberInfo)_part.ImportingConstructor.GetConstructor(_catalog._assemblies);
						else if(_part != null)
							_member = _catalog.GetMemberInfo(_part.TypeName, _export.MemberName);
						if(_member != null)
						{
							_catalog = null;
							_part = null;
							_export = null;
						}
					}
				return _member;
			}
		}
	}
}
