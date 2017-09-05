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
	/// ComposablePart para instancias.
	/// </summary>
	class InstanceComposablePart : ComposablePart
	{
		private IDictionary<string, object> _metadata;

		private TypeInstance _typeInstance;

		/// <summary>
		/// Definições de importação.
		/// </summary>
		public override IEnumerable<ImportDefinition> ImportDefinitions
		{
			get
			{
				return new ImportDefinition[0];
			}
		}

		/// <summary>
		/// Definições de exportação.
		/// </summary>
		public override IEnumerable<ExportDefinition> ExportDefinitions
		{
			get
			{
				yield return new ExportDefinition(_typeInstance.ContractName, Metadata);
			}
		}

		/// <summary>
		/// Metadados.
		/// </summary>
		public override IDictionary<string, object> Metadata
		{
			get
			{
				return _metadata;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeInstance"></param>
		/// <param name="metadata"></param>
		public InstanceComposablePart(TypeInstance typeInstance, IDictionary<string, object> metadata)
		{
			_metadata = metadata;
			_typeInstance = typeInstance;
		}

		/// <summary>
		/// Recupera o valor exportado.
		/// </summary>
		/// <param name="definition"></param>
		/// <returns></returns>
		public override object GetExportedValue(ExportDefinition definition)
		{
			return _typeInstance.Value;
		}

		/// <summary>
		/// Define o import.
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="exports"></param>
		public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
		{
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _typeInstance.TypeFullName;
		}
	}
}
