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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Armazena os dados do assembly.
	/// </summary>
	[Serializable]
	public class AsmData
	{
		/// <summary>
		/// Possíveis validades do assembly.
		/// </summary>
		public enum AsmValidity
		{
			/// <summary>
			/// Identifica que é valido.
			/// </summary>
			Valid,
			/// <summary>
			/// Somente as referencias.
			/// </summary>
			ReferencesOnly,
			/// <summary>
			/// Inválido.
			/// </summary>
			Invalid,
			/// <summary>
			/// Dependencia circular
			/// </summary>
			CircularDependency,
			/// <summary>
			/// Redirecionado.
			/// </summary>
			Redirected
		}

		private string _assemblyFullName;

		private LibraryImport[] _imports;

		private string _name;

		private string _path;

		private List<AsmData> _references;

		private AsmValidity _validity;

		/// <summary>
		/// Informações adicionais.
		/// </summary>
		public string AdditionalInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Arquitetura.
		/// </summary>
		public System.Reflection.ProcessorArchitecture Architecture
		{
			get;
			set;
		}

		/// <summary>
		/// Nome completo do assembly.
		/// </summary>
		public string AssemblyFullName
		{
			get
			{
				return _assemblyFullName;
			}
			set
			{
				_assemblyFullName = value;
			}
		}

		/// <summary>
		/// Nome do produto.
		/// </summary>
		public string AssemblyProductName
		{
			get;
			set;
		}

		/// <summary>
		/// Biblioteca importadas.
		/// </summary>
		public LibraryImport[] Imports
		{
			get
			{
				return _imports;
			}
			set
			{
				_imports = value;
			}
		}

		/// <summary>
		/// Detalhes do assembly inválido.
		/// </summary>
		public string InvalidAssemblyDetails
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do assembly.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Versão original.
		/// </summary>
		public string OriginalVersion
		{
			get;
			set;
		}

		/// <summary>
		/// Caminho do assembly.
		/// </summary>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		/// <summary>
		/// Referencias.
		/// </summary>
		internal List<AsmData> References
		{
			get
			{
				return _references;
			}
		}

		/// <summary>
		/// Validade do assembly.
		/// </summary>
		public AsmValidity Validity
		{
			get
			{
				return _validity;
			}
			set
			{
				_validity = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do assembly.</param>
		/// <param name="path">Caminho do assembly.</param>
		public AsmData(string name, string path)
		{
			_name = name;
			_path = path;
			_imports = new LibraryImport[0];
			_references = new List<AsmData>();
			_validity = AsmValidity.Invalid;
			Architecture = System.Reflection.ProcessorArchitecture.None;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder stringValue = new StringBuilder();
			stringValue.AppendLine(this.AssemblyFullName);
			stringValue.AppendLine(_path);
			if(!string.IsNullOrEmpty(this.OriginalVersion))
			{
				stringValue.Append("Original referenced assembly version: ");
				stringValue.AppendLine(this.OriginalVersion);
			}
			if(_imports.Length > 0)
			{
				stringValue.AppendLine();
				stringValue.AppendLine("Imports: ");
				foreach (LibraryImport imp in _imports)
					stringValue.AppendLine(imp.GetLongDescription());
			}
			if(!string.IsNullOrEmpty(this.InvalidAssemblyDetails) && (this.Validity != AsmValidity.Valid))
				stringValue.AppendLine("\r\n" + this.InvalidAssemblyDetails);
			if(!string.IsNullOrEmpty(this.AdditionalInfo))
				stringValue.AppendLine("\r\n" + this.AdditionalInfo);
			stringValue.AppendLine("Architecture: " + this.Architecture);
			return stringValue.ToString();
		}
	}
}
