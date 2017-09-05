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

namespace Colosoft.Data.Schema.GDA
{
	class Program
	{
		private static string ConvertType(string type)
		{
			switch(type)
			{
			case "System.Int32":
			case "System.UInt32":
				return "int";
			case "System.Int16":
			case "System.UInt16":
				return "short";
			case "System.Int64":
				return "long";
			case "System.String":
				return "string";
			case "System.Decimal":
				return "decimal";
			case "System.Double":
				return "double";
			case "System.Single":
				return "float";
			case "System.Boolean":
				return "bool";
			case "System.DateTime":
				return "DateTime";
			}
			return type;
		}

		static void Main(string[] args)
		{
			var assembly = typeof(Glass.Data.CFeUtils.ConfigCFe).Assembly;
			var typeSchema = new Local.TypeSchema();
			var typeCode = 1;
			var propertyCode = 1;
			var types = assembly.GetTypes();
			var outputDirectory = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location) + "\\output";
			if(!System.IO.Directory.Exists(outputDirectory))
				System.IO.Directory.CreateDirectory(outputDirectory);
			if(!System.IO.Directory.Exists(System.IO.Path.Combine(outputDirectory, "Models")))
				System.IO.Directory.CreateDirectory(System.IO.Path.Combine(outputDirectory, "Models"));
			if(!System.IO.Directory.Exists(System.IO.Path.Combine(outputDirectory, "Entities")))
				System.IO.Directory.CreateDirectory(System.IO.Path.Combine(outputDirectory, "Entities"));
			foreach (var type in types)
			{
				var customAttributes = type.GetCustomAttributes(typeof(global::GDA.PersistenceClassAttribute), false);
				if(customAttributes.Length > 0)
				{
					var persistenceClass = (global::GDA.PersistenceClassAttribute)customAttributes[0];
					var propertiesMetadata = new List<Local.PropertyMetadata>();
					foreach (var property in type.GetProperties())
					{
						var persistenceProperty = (global::GDA.PersistencePropertyAttribute)property.GetCustomAttributes(typeof(global::GDA.PersistencePropertyAttribute), false).FirstOrDefault();
						var propertyType = property.PropertyType;
						if(propertyType.IsNullable())
							propertyType = propertyType.GetRootType();
						if(propertyType == typeof(uint))
							propertyType = typeof(int);
						if(persistenceProperty != null)
						{
							var parameterType = PersistenceParameterType.Field;
							switch(persistenceProperty.ParameterType)
							{
							case global::GDA.PersistenceParameterType.Field:
								parameterType = PersistenceParameterType.Field;
								break;
							case global::GDA.PersistenceParameterType.IdentityKey:
								parameterType = PersistenceParameterType.IdentityKey;
								break;
							case global::GDA.PersistenceParameterType.Key:
								parameterType = PersistenceParameterType.Key;
								break;
							}
							var directionParameter = (DirectionParameter)Enum.Parse(typeof(DirectionParameter), persistenceProperty.Direction.ToString());
							if(directionParameter == DirectionParameter.InputOptional || directionParameter == DirectionParameter.InputOptionalOutput || directionParameter == DirectionParameter.InputOptionalOutputOnlyInsert)
								continue;
							var pMetadata = new Local.PropertyMetadata(propertyCode++, property.Name, persistenceProperty.Name, propertyType.FullName, null, false, DirectionParameter.InputOutput, false, parameterType, false);
							propertiesMetadata.Add(pMetadata);
						}
					}
					var typeMetadata = new Colosoft.Data.Schema.Local.TypeMetadata(typeCode++, type.Name, type.Namespace, assembly.GetName().Name, false, false, new TableName("WebGlass", "dbo", persistenceClass.Name), propertiesMetadata.Select(f => (IPropertyMetadata)f).ToArray());
					typeSchema.AddTypeMetadata((ITypeMetadata)typeMetadata.Clone());
					CreateCSharpFile(type, typeMetadata, outputDirectory);
				}
			}
			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Local.TypeSchema));
			var xml = new StringBuilder();
			xmlSerializer.Serialize(new System.IO.StringWriter(xml), typeSchema);
			var xmlText = xml.ToString();
			var typeSchema2 = xmlSerializer.Deserialize(new System.IO.StringReader(xmlText));
			var schemaFileName = System.IO.Path.Combine(outputDirectory, "TypeSchema.xml");
			if(System.IO.File.Exists(schemaFileName))
				System.IO.File.Delete(schemaFileName);
			System.IO.File.WriteAllText(schemaFileName, xmlText);
		}

		private static void CreateCSharpFile(Type type, Local.TypeMetadata typeMetadata, string outputDirectory)
		{
			var dataClassNamespace = "Glass.Data.Models";
			var dataClassText = new StringBuilder();
			dataClassText.AppendLine("using System;").AppendLine("using System.Collections.Generic;").AppendLine("using System.Text;").AppendLine();
			dataClassText.AppendLine("namespace " + dataClassNamespace).AppendLine("{");
			var nestedTypes = type.GetNestedTypes();
			foreach (var nType in nestedTypes)
			{
				if(nType.IsEnum)
				{
					var name = nType.Name;
					if(nType.Name.EndsWith(type.Name))
					{
						var matchName = nType.Name.Substring(0, nType.Name.Length - type.Name.Length);
						var matchProperty = typeMetadata.Where(f => f.Name == matchName).FirstOrDefault();
						if(matchProperty != null)
						{
							((Local.PropertyMetadata)matchProperty).PropertyType = "Data.Models." + name;
						}
					}
					else if(nType.Name.EndsWith("Enum"))
					{
						var matchProperty = typeMetadata.Where(f => f.Name == nType.Name.Replace("Enum", "")).FirstOrDefault();
						name = name.Replace("Enum", type.Name);
						if(matchProperty != null)
							((Local.PropertyMetadata)matchProperty).PropertyType = "Data.Models." + name;
					}
					dataClassText.AppendLine("\t/// <summary>").AppendLine("\t/// " + name).AppendLine("\t/// </summary>").AppendLine("\tpublic enum " + name).AppendLine("\t{");
					dataClassText.AppendLine(string.Join(",\r\n", Enum.GetNames(nType).Select(f => new StringBuilder().AppendLine("\t\t/// <summary>").AppendLine("\t\t/// " + f).AppendLine("\t\t/// </summary>").Append("\t\t" + f).ToString()).ToArray()));
					dataClassText.AppendLine("\t}").AppendLine();
				}
			}
			dataClassText.AppendLine("\t/// <summary>").AppendLine("\t/// " + type.Name).AppendLine("\t/// </summary>").AppendLine("\tpublic class " + type.Name + " : Colosoft.Data.BaseModel").AppendLine("\t{").AppendLine("\t\t#region Propriedades").AppendLine();
			foreach (var property in typeMetadata)
			{
				dataClassText.AppendLine("\t\t/// <summary>").AppendLine("\t\t/// " + property.Name).AppendLine("\t\t/// </summary>").AppendLine("\t\tpublic " + ConvertType(property.PropertyType) + " " + property.Name + " { get; set; }").AppendLine();
			}
			dataClassText.AppendLine("\t\t#endregion").AppendLine("\t}").AppendLine("}");
			var text = dataClassText.ToString();
			var dataClassFile = System.IO.Path.Combine(outputDirectory, "Models", type.Name + ".cs");
			if(System.IO.File.Exists(dataClassFile))
				System.IO.File.Delete(dataClassFile);
			System.IO.File.WriteAllText(dataClassFile, text);
			var entityClassText = new StringBuilder();
			var entityClassNamespace = "Glass.Business.Entities";
			entityClassText.AppendLine("using System;").AppendLine("using System.Collections.Generic;").AppendLine("using System.Text;").AppendLine();
			entityClassText.AppendLine("namespace " + entityClassNamespace).AppendLine("{");
			entityClassText.AppendLine("\t/// <summary>").AppendLine("\t/// " + type.Name).AppendLine("\t/// </summary>").AppendLine("\t[Colosoft.Business.EntityLoader(typeof(" + type.Name + "Loader))]").Append("\tpublic class " + type.Name).Append(" : Colosoft.Business.Entity<").Append(dataClassNamespace).Append(".").Append(type.Name).AppendLine(">").AppendLine("\t{").AppendLine("\t\t#region Tipos Aninhados").AppendLine();
			entityClassText.AppendLine("\t\t/// <summary>").AppendLine("\t\t/// " + type.Name).AppendLine("\t\t/// </summary>").Append("\t\tpublic class " + type.Name + "Loader").Append(" : Colosoft.Business.EntityLoader<").Append(type.Name).Append(", ").Append(dataClassNamespace).Append(".").Append(type.Name).AppendLine(">").AppendLine("\t\t{").AppendLine("\t\t\t#region Construtores").AppendLine().Append("\t\t\tpublic ").Append(type.Name).AppendLine("Loader()").AppendLine("\t\t\t{").AppendLine("\t\t\t\tConfigure()");
			var keys = typeMetadata.Where(f => f.ParameterType == PersistenceParameterType.IdentityKey || f.ParameterType == PersistenceParameterType.Key).ToArray();
			if(keys.Length == 1)
			{
				entityClassText.Append("\t\t\t\t\t.Uid(f => f.").Append(keys[0].Name).AppendLine(")");
			}
			else if(keys.Length > 1)
			{
				entityClassText.Append("\t\t\t\t\t.Keys(").Append(string.Join(", ", keys.Select(f => "f => f." + f.Name).ToArray())).AppendLine(")");
			}
			entityClassText.AppendLine("\t\t\t\t\t.Creator(f => new " + type.Name + "(f));");
			entityClassText.AppendLine("\t\t\t}").AppendLine().AppendLine("\t\t\t#endregion").AppendLine("\t\t}").AppendLine();
			entityClassText.AppendLine("\t\t#endregion").AppendLine().AppendLine("\t\t#region Propriedades").AppendLine();
			foreach (var property in typeMetadata)
			{
				entityClassText.AppendLine("\t\t/// <summary>").AppendLine("\t\t/// " + property.Name).AppendLine("\t\t/// </summary>").AppendLine("\t\tpublic " + ConvertType(property.PropertyType) + " " + property.Name).AppendLine("\t\t{").AppendLine("\t\t\tget { return DataModel." + property.Name + "; }").AppendLine("\t\t\tset").AppendLine("\t\t\t{").AppendLine("\t\t\t\tif (DataModel." + property.Name + " != value)").AppendLine("\t\t\t\t{").AppendLine("\t\t\t\t\tDataModel." + property.Name + " = value;").Append("\t\t\t\t\tRaisePropertyChanged(\"").Append(property.Name).AppendLine("\");").AppendLine("\t\t\t\t}").AppendLine("\t\t\t}").AppendLine("\t\t}").AppendLine();
			}
			entityClassText.AppendLine("\t\t#endregion").AppendLine().AppendLine("\t\t#region Construtores").AppendLine();
			entityClassText.AppendLine("\t\t/// <summary>").AppendLine("\t\t/// Construtor padrão.").AppendLine("\t\t/// </summary>").AppendLine("\t\tpublic " + type.Name + "()").AppendLine("\t\t\t: this(null, null, null)").AppendLine("\t\t{").AppendLine("\t\t}").AppendLine().AppendLine("\t\t/// <summary>").AppendLine("\t\t/// Construtor padrão.").AppendLine("\t\t/// </summary>").Append("\t\tpublic " + type.Name).Append("(Colosoft.Business.EntityLoaderCreatorArgs<").Append(dataClassNamespace).Append(".").Append(type.Name).Append("> args)").AppendLine().AppendLine("\t\t\t: base(args.DataModel, args.UIContext, args.TypeManager)").AppendLine("\t\t{").AppendLine("\t\t}").AppendLine().AppendLine("\t\t/// <summary>").AppendLine("\t\t/// Construtor padrão.").AppendLine("\t\t/// </summary>").Append("\t\tpublic " + type.Name).Append("(").Append(dataClassNamespace).Append(".").Append(type.Name).Append(" dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)").AppendLine().AppendLine("\t\t\t: base(dataModel, uiContext, entityTypeManager)").AppendLine("\t\t{").AppendLine("\t\t}").AppendLine();
			entityClassText.AppendLine("\t\t#endregion").AppendLine("\t}").AppendLine("}");
			var text2 = entityClassText.ToString();
			var entityClassFile = System.IO.Path.Combine(outputDirectory, "Entities", type.Name + ".cs");
			if(System.IO.File.Exists(entityClassFile))
				System.IO.File.Delete(entityClassFile);
			System.IO.File.WriteAllText(entityClassFile, text2);
		}
	}
}
