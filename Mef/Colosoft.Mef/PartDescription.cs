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
using System.Collections.ObjectModel;

namespace Colosoft.Mef
{
	/// <summary>
	/// Define uma descrição abstrata de uma parte.
	/// </summary>
	class PartDescription
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PartDescription() : this(null, null, null, false, null)
		{
		}

		/// <summary>
		/// Construtor de inicialização.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="exports"></param>
		/// <param name="imports"></param>
		/// <param name="useDispatcher"></param>
		/// <param name="importingConstruct"></param>
		/// <param name="partCreationPolicy"></param>
		public PartDescription(Colosoft.Reflection.TypeName typeName, IEnumerable<ExportDescription> exports, IEnumerable<ImportDescription> imports, bool useDispatcher, ImportingConstructorDescription importingConstruct = null, System.ComponentModel.Composition.CreationPolicy partCreationPolicy = System.ComponentModel.Composition.CreationPolicy.Shared)
		{
			this.UseDispatcher = useDispatcher;
			this.Exports = exports;
			this.Imports = imports;
			this.ImportingConstructor = importingConstruct;
			this.TypeName = typeName;
			this.PartCreationPolicy = partCreationPolicy;
		}

		/// <summary>
		/// Imports associados com a parte.
		/// </summary>
		public IEnumerable<ExportDescription> Exports
		{
			get;
			set;
		}

		/// <summary>
		/// Exports associados com a parte.
		/// </summary>
		public IEnumerable<ImportDescription> Imports
		{
			get;
			set;
		}

		/// <summary>
		/// Descrição da importação do construtor.
		/// </summary>
		public ImportingConstructorDescription ImportingConstructor
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo associado.
		/// </summary>
		public Colosoft.Reflection.TypeName TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Política de criação.
		/// </summary>
		public System.ComponentModel.Composition.CreationPolicy PartCreationPolicy
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é para usar o Dispatcher.
		/// </summary>
		public bool UseDispatcher
		{
			get;
			set;
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(TypeName != null)
				return string.Format("{0}, {1}", TypeName.FullName, TypeName.AssemblyName.Name);
			return base.ToString();
		}
	}
}
