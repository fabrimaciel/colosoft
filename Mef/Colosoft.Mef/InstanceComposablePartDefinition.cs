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
	/// Armazena a definição da parte de composição.
	/// </summary>
	class InstanceComposablePartDefinition : ComposablePartDefinition
	{
		private TypeInstance _typeInstance;

		private Dictionary<string, object> _metadata;

		/// <summary>
		/// Recupera  definição de export.
		/// </summary>
		public override IEnumerable<ExportDefinition> ExportDefinitions
		{
			get
			{
				var metadata = new Dictionary<string, object>();
				metadata.Add("ExportTypeIdentity", _typeInstance.TypeFullName);
				yield return new ExportDefinition(_typeInstance.ContractName, metadata);
			}
		}

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
		/// Metadados.
		/// </summary>
		public override IDictionary<string, object> Metadata
		{
			get
			{
				if(_metadata == null)
				{
					_metadata = new Dictionary<string, object>();
					_metadata.Add("ExportTypeIdentity", _typeInstance.TypeFullName);
				}
				return _metadata;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="instance"></param>
		public InstanceComposablePartDefinition(TypeInstance instance)
		{
			_typeInstance = instance;
		}

		/// <summary>
		/// Cria um parte de composição.
		/// </summary>
		/// <returns></returns>
		public override ComposablePart CreatePart()
		{
			return new InstanceComposablePart(_typeInstance, Metadata);
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
