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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Controla atualizações de TypeMetadata e PropertyMetadata
	/// </summary>
	public static class TypeSchemaUpdate
	{
		private static bool _isRunning = false;

		private static DateTime _lastCheckTime = DateTime.Now;

		private static DateTime _lastUpdateTime = DateTime.MinValue;

		private static bool _isLoaded = false;

		private static object _lockIsLoadedListObject = new object();

		private static object _lockListObject = new object();

		private static object _lockLastCheckTimeObject = new object();

		private static object _lockLastUpdateTimeObject = new object();

		private static object _lockIsRunningObject = new object();

		private static ITypeSchema _typeSchema;

		private static object _lockTypeSchema = new object();

		/// <summary>
		/// Marca que a carga dos exports está feita.
		/// </summary>
		public static bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
			set
			{
				lock (_lockIsLoadedListObject)
				{
					_isLoaded = value;
				}
			}
		}

		/// <summary>
		/// Indica que a Thread já está rodando.
		/// </summary>
		public static bool IsRunning
		{
			get
			{
				return _isRunning;
			}
		}

		/// <summary>
		/// Recarrega as configurações.
		/// </summary>
		private static bool ReloadSettings()
		{
			lock (_lockTypeSchema)
			{
				ServiceLocatorValidator.Validate();
				if(_typeSchema == null)
				{
					try
					{
						_typeSchema = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITypeSchema>();
					}
					catch
					{
						return false;
					}
					return true;
				}
				else
				{
					_typeSchema.Reload();
					return true;
				}
			}
		}

		/// <summary>
		/// Indica que o cache deve ser obrigatóriamente recarregado.
		/// </summary>
		public static void VerifyCacheUpdate()
		{
			try
			{
				while (!_isLoaded)
					System.Threading.Thread.Sleep(500);
				ITypeSchemaUpdateInfoGetter infoGetter = null;
				try
				{
					infoGetter = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITypeSchemaUpdateInfoGetter>();
				}
				catch
				{
					return;
				}
				TypeSchemaUpdateInfo updateInfo = null;
				try
				{
					updateInfo = infoGetter.GetLatUpdateTypeSchemaInfo();
				}
				catch
				{
				}
				if(updateInfo != null)
					_lastUpdateTime = updateInfo.LastSchemaChangedDate;
				while (true)
				{
					try
					{
						try
						{
							updateInfo = infoGetter.GetLatUpdateTypeSchemaInfo();
						}
						catch
						{
							updateInfo = null;
							System.Threading.Thread.Sleep(3000);
							continue;
						}
						if(updateInfo != null && (updateInfo.LastSchemaChangedDate > new DateTime(2010, 1, 1)) && (updateInfo.LastSchemaChangedDate != _lastUpdateTime))
						{
							if(updateInfo.LastSchemaChangedDate != _lastUpdateTime)
							{
								lock (_lockLastUpdateTimeObject)
								{
									_lastUpdateTime = updateInfo.LastSchemaChangedDate;
								}
								if(!ReloadSettings())
								{
									System.Threading.Thread.Sleep(15000);
									continue;
								}
							}
						}
						lock (_lockLastCheckTimeObject)
							_lastCheckTime = DateTime.Now;
						System.Threading.Thread.Sleep(15000);
					}
					catch(System.Threading.ThreadAbortException)
					{
						return;
					}
					catch(Exception ex)
					{
						Colosoft.Log.Logger.Error(ex.Message.GetFormatter(), ex);
					}
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				Colosoft.Log.Logger.Error(ex.Message.GetFormatter(), ex);
			}
		}

		/// <summary>
		/// Instania do esquema dos tipos.
		/// </summary>
		public static ITypeSchema TypeSchema
		{
			get
			{
				while (_typeSchema == null)
				{
					ReloadSettings();
					System.Threading.Thread.Sleep(5000);
				}
				return _typeSchema;
			}
		}

		/// <summary>
		/// Para a execução.
		/// </summary>
		public static void StopRun()
		{
			_isRunning = false;
		}
	}
}
