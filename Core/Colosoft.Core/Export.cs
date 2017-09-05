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

namespace Colosoft.Reflection.Composition
{
	/// <summary>
	/// Implementação que armazena os dados de exportção.
	/// </summary>
	[Serializable]
	public class Export : IExport2, Serialization.ICompactSerializable, System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// Tipo exportado.
		/// </summary>
		public TypeName Type
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do contrato.
		/// </summary>
		public string ContractName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo do contrato.
		/// </summary>
		public TypeName ContractType
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é para importa o construtor.
		/// </summary>
		public bool ImportingConstructor
		{
			get;
			set;
		}

		/// <summary>
		/// Política de criação.
		/// </summary>
		public CreationPolicy CreationPolicy
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é para usar o dispatcher
		/// padrão do sistema para criar a instancia.
		/// </summary>
		public bool UseDispatcher
		{
			get;
			set;
		}

		/// <summary>
		/// Metadados da instancia do export.
		/// </summary>
		public IDictionary<string, object> Metadata
		{
			get;
			set;
		}

		/// <summary>
		/// Contexto de interface com o usuário.
		/// </summary>
		public string UIContext
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Export()
		{
		}

		/// <summary>
		/// Cria a instancia com base na export informada.
		/// </summary>
		/// <param name="export"></param>
		public Export(IExport export)
		{
			this.Type = new TypeName(export.Type.AssemblyQualifiedName);
			this.ContractName = export.ContractName;
			this.ContractType = new TypeName(export.ContractType.AssemblyQualifiedName);
			this.ImportingConstructor = export.ImportingConstructor;
			this.UseDispatcher = export.UseDispatcher;
			this.CreationPolicy = export.CreationPolicy;
			if(export is IExport2)
				this.UIContext = ((IExport2)export).UIContext;
			if(export.Metadata != null)
			{
				Metadata = new Dictionary<string, object>();
				foreach (var i in export.Metadata)
					Metadata.Add(i);
			}
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected Export(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.Type = (TypeName)info.GetValue("Type", typeof(TypeName));
			this.ContractName = info.GetString("ContractName");
			this.ContractType = (TypeName)info.GetValue("ContractType", typeof(TypeName));
			this.ImportingConstructor = info.GetBoolean("ImportingConstructor");
			this.CreationPolicy = (CreationPolicy)info.GetInt32("CreationPolicy");
			this.UseDispatcher = info.GetBoolean("UseDispatcher");
			this.UIContext = info.GetString("UIContext");
			this.Metadata = (Dictionary<string, object>)info.GetValue("Metadata", typeof(Dictionary<string, object>));
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="reader"></param>
		void Serialization.ICompactSerializable.Deserialize(Serialization.IO.CompactReader reader)
		{
			if(reader.ReadBoolean())
			{
				this.Type = new TypeName();
				this.Type.Deserialize(reader);
			}
			ContractName = reader.ReadString();
			if(reader.ReadBoolean())
			{
				ContractType = new TypeName();
				ContractType.Deserialize(reader);
			}
			ImportingConstructor = reader.ReadBoolean();
			this.CreationPolicy = (CreationPolicy)reader.ReadInt32();
			UseDispatcher = reader.ReadBoolean();
			UIContext = reader.ReadString();
			var count = reader.ReadInt32();
			Metadata = new Dictionary<string, object>(count);
			for(var i = 0; i < count; i++)
			{
				var key = reader.ReadString();
				var value = reader.ReadObject();
				Metadata.Add(key, value);
			}
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void Serialization.ICompactSerializable.Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(this.Type != null);
			if(this.Type != null)
				((Serialization.ICompactSerializable)this.Type).Serialize(writer);
			writer.Write(ContractName);
			writer.Write(this.ContractType != null);
			if(this.Type != null)
				((Serialization.ICompactSerializable)this.ContractType).Serialize(writer);
			writer.Write(ImportingConstructor);
			writer.Write((int)CreationPolicy);
			writer.Write(UseDispatcher);
			writer.Write(UIContext);
			writer.Write(this.Metadata != null ? this.Metadata.Count : 0);
			if(this.Metadata != null)
			{
				foreach (var item in Metadata)
				{
					writer.Write(item.Key);
					writer.WriteObject(item.Value);
				}
			}
		}

		/// <summary>
		/// Recupera os dados que serão usados pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Type", this.Type, typeof(TypeName));
			info.AddValue("ContractName", ContractName);
			info.AddValue("ContractType", ContractType, typeof(TypeName));
			info.AddValue("ImportingConstructor", ImportingConstructor);
			info.AddValue("CreationPolicy", (int)CreationPolicy);
			info.AddValue("UIContext", UIContext);
			info.AddValue("Metadata", Metadata is Dictionary<string, object> ? Metadata : new Dictionary<string, object>(Metadata), typeof(Dictionary<string, object>));
		}
	}
}
