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

namespace Colosoft.Data.Schema.Server
{
	/// <summary>
	/// Classe que fornecem os esquema para os tipos do sistema.
	/// </summary>
	public class TypeSchema : ITypeSchema, IDisposable
	{
		private Dictionary<string, TypeMetadata> _typeMetadataDictionaryKeyFullName = new Dictionary<string, TypeMetadata>();

		private Dictionary<string, List<TypeMetadata>> _typeMetadataDictionaryKeyAssembly = new Dictionary<string, List<TypeMetadata>>();

		private Dictionary<int, TypeMetadata> _typeMetadataDictionaryKeyTypeCode = new Dictionary<int, TypeMetadata>();

		private Dictionary<int, PropertyMetadata> _propertyMetadataDictionary = new Dictionary<int, PropertyMetadata>();

		private System.Threading.ManualResetEvent _allDone;

		private Colosoft.Logging.ILogger _logger;

		private TypeMetadata _typeMetadata = new TypeMetadata();

		private Exception _loadException;

		private object _checkLoadObjLock = new object();

		/// <summary>
		/// <see cref="Colosoft.Logging.ILogger"/> que será usado na instancia.
		/// </summary>
		public Colosoft.Logging.ILogger Logger
		{
			get
			{
				return _logger ?? Colosoft.Log.Logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="logger">Recebe o seviço de login</param>
		public TypeSchema(Colosoft.Logging.ILogger logger)
		{
			_logger = logger;
			_allDone = new System.Threading.ManualResetEvent(false);
			BeginLoadTableAndPropertyData();
		}

		/// <summary>
		/// Recupera os metadados do tipo com o nome informado.
		/// </summary>
		/// <param name="fullName">Nome completo do tipo.</param>
		/// <returns>Instancia dos metadados do tipo.</returns>
		public ITypeMetadata GetTypeMetadata(string fullName)
		{
			CheckLoad();
			try
			{
				return _typeMetadataDictionaryKeyFullName[fullName];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.TypeNotMapped, fullName).Format(), ex);
			}
		}

		/// <summary>
		/// Recupera os metadados do tipo com base no código informado.
		/// </summary>
		/// <param name="typeCode">Código do tipo.</param>
		/// <returns></returns>
		public ITypeMetadata GetTypeMetadata(int typeCode)
		{
			CheckLoad();
			try
			{
				return _typeMetadataDictionaryKeyTypeCode[typeCode];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.TypeCodeNotMapped, typeCode.ToString()).Format(), ex);
			}
		}

		/// <summary>
		/// Recupera os metadados de uma propriedade pelo código informado.
		/// </summary>
		/// <param name="propertyCode">Código da propriedade.</param>
		/// <returns>Instancia dos metadados da propriedade.</returns>
		public IPropertyMetadata GetPropertyMetadata(int propertyCode)
		{
			CheckLoad();
			try
			{
				return _propertyMetadataDictionary[propertyCode];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.PropertyCodeNotMapped, propertyCode.ToString()).Format(), ex);
			}
		}

		/// <summary>
		/// Recupera os metadados de todos o tipos registrados
		/// </summary>
		/// <returns>Intâncias dos metadados dos tipos</returns>
		public IEnumerable<ITypeMetadata> GetTypeMetadatas()
		{
			CheckLoad();
			foreach (var metadataList in _typeMetadataDictionaryKeyAssembly)
			{
				foreach (var metadata in metadataList.Value)
					yield return metadata;
			}
		}

		/// <summary>
		/// Recupera os metadados dos tipos associados com o assembly informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly onde os tipo estão inseridos.</param>
		/// <returns></returns>
		public IEnumerable<ITypeMetadata> GetTypeMetadatas(string assemblyName)
		{
			CheckLoad();
			try
			{
				return _typeMetadataDictionaryKeyAssembly[assemblyName];
			}
			catch(KeyNotFoundException ex)
			{
				throw new Exception(ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyNotMapped, assemblyName).Format(), ex);
			}
		}

		public void Reload()
		{
		}

		/// <summary>
		/// Verifica a carga.
		/// </summary>
		private void CheckLoad()
		{
			_allDone.WaitOne();
			if(_loadException != null)
				lock (_checkLoadObjLock)
				{
					if(_loadException != null)
					{
						var ex = _loadException;
						_loadException = null;
						BeginLoadTableAndPropertyData();
						throw ex;
					}
				}
		}

		/// <summary>
		/// Inicia a carga dos dados
		/// </summary>
		private void BeginLoadTableAndPropertyData()
		{
			new Action(LoadTableAndPropertyData).BeginInvoke(ar =>  {
				Action action = (Action)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
				try
				{
					action.EndInvoke(ar);
				}
				catch(Exception ex)
				{
					_loadException = ex;
					Logger.Error(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_FailOnLoadTableAndPropertyData), ex);
				}
				_allDone.Set();
			}, null);
		}

		/// <summary>
		/// Carrega os dados.
		/// </summary>
		private void LoadTableAndPropertyData()
		{
			IList<TypeMetadata> typeMetadatas = null;
			IList<PropertyMetadata> propertyMetadatas = null;
			try
			{
				string message;
				GDA.GDAOperations.DebugTrace += (s, m) =>  {
					message = m;
				};
				typeMetadatas = new GDA.Sql.Query().ToList<TypeMetadata>();
			}
			catch(Exception ex)
			{
				_loadException = ex;
				Logger.Write(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_FaultOnLoadTypesMetadata), ex, Logging.Priority.High);
				return;
			}
			try
			{
				propertyMetadatas = new GDA.Sql.Query().ToList<PropertyMetadata>();
			}
			catch(Exception ex)
			{
				_loadException = ex;
				Logger.Write(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_FaultOnLoadPropertiesMetadata), ex, Logging.Priority.High);
				return;
			}
			if(typeMetadatas != null)
			{
				foreach (var typeMetadata in typeMetadatas)
				{
					if(_typeMetadataDictionaryKeyFullName.ContainsKey(typeMetadata.FullName))
						continue;
					_typeMetadataDictionaryKeyFullName.Add(typeMetadata.FullName, typeMetadata);
					_typeMetadataDictionaryKeyTypeCode.Add(typeMetadata.TypeCode, typeMetadata);
					List<TypeMetadata> auxList;
					if(_typeMetadataDictionaryKeyAssembly.TryGetValue(typeMetadata.Assembly, out auxList))
						auxList.Add(typeMetadata);
					else
					{
						auxList = new List<TypeMetadata>();
						auxList.Add(typeMetadata);
						_typeMetadataDictionaryKeyAssembly.Add(typeMetadata.Assembly, auxList);
					}
				}
			}
			if(propertyMetadatas != null)
			{
				foreach (var propertyMetadata in propertyMetadatas)
				{
					TypeMetadata typeMetadata = null;
					if(_typeMetadataDictionaryKeyTypeCode.TryGetValue(propertyMetadata.TypeCode, out typeMetadata))
					{
						_propertyMetadataDictionary.Add(propertyMetadata.PropertyCode, propertyMetadata);
						typeMetadata.AddPropertyMetadata(propertyMetadata, Logger);
					}
				}
			}
			foreach (var typeMetadata in _typeMetadataDictionaryKeyTypeCode.Values)
				if(typeMetadata.IsVersioned)
				{
					var isDeletedPropertyMetadata = new PropertyMetadata {
						PropertyCode = -16,
						TypeCode = typeMetadata.TypeCode,
						Name = "IsDeleted",
						ColumnName = "IsDeleted",
						Direction = DirectionParameter.InputOutput,
						PropertyType = typeof(bool).FullName,
						ParameterType = PersistenceParameterType.Field,
					};
					typeMetadata.AddPropertyMetadata(isDeletedPropertyMetadata, Logger);
				}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_allDone.Dispose();
		}
	}
}
