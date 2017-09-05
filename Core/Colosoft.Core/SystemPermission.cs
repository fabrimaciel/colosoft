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
using System.Security;
using System.Security.Permissions;
using System.Security.AccessControl;
using Colosoft.Security.Util;

namespace Colosoft.Security.Permissions
{
	/// <summary>
	/// Tipos de permissões de acesso do sistema.
	/// </summary>
	[Serializable]
	[Flags]
	public enum SystemPermissionAccess
	{
		/// <summary>
		/// Identifica permissão para acesso total.
		/// </summary>
		AllAccess = 7,
		/// <summary>
		/// Negação de acesso.
		/// </summary>
		NoAccess = 0,
		/// <summary>
		/// Permissão para leitura.
		/// </summary>
		Read = 1,
		/// <summary>
		/// Permissão para escrita.
		/// </summary>
		Write = 2,
		/// <summary>
		/// Permissão para execução.
		/// </summary>
		Execute = 4
	}
	/// <summary>
	/// Representa permissões do sistema.
	/// </summary>
	[Serializable, System.Runtime.InteropServices.ComVisible(true)]
	public class SystemPermission : IPermission, IUnrestrictedPermission
	{
		private bool _unrestricted;

		/// <summary>
		/// Armazena os caracteres inválidos.
		/// </summary>
		private static readonly char[] _illegalCharacters;

		private SystemAccess _execute;

		[System.Runtime.Serialization.OptionalField(VersionAdded = 2)]
		private SystemAccess _changeAcl;

		private SystemAccess _pathDiscovery;

		private SystemAccess _read;

		[System.Runtime.Serialization.OptionalField(VersionAdded = 2)]
		private SystemAccess _viewAcl;

		private SystemAccess _write;

		/// <summary>
		/// Recupera e define acesso a todos os itens.
		/// </summary>
		public SystemPermissionAccess AllItems
		{
			get
			{
				if(this._unrestricted)
					return SystemPermissionAccess.AllAccess;
				SystemPermissionAccess noAccess = SystemPermissionAccess.NoAccess;
				if((_read != null) && _read.AllItems)
					noAccess |= SystemPermissionAccess.Read;
				if((_write != null) && _write.AllItems)
					noAccess |= SystemPermissionAccess.Write;
				if((_execute != null) && _execute.AllItems)
					noAccess |= SystemPermissionAccess.Execute;
				return noAccess;
			}
			set
			{
				if(value == SystemPermissionAccess.AllAccess)
				{
					this._unrestricted = true;
				}
				else
				{
					if((value & SystemPermissionAccess.Read) != SystemPermissionAccess.NoAccess)
					{
						if(_read == null)
							_read = new SystemAccess();
						_read.AllItems = true;
					}
					else if(_read != null)
						_read.AllItems = false;
					if((value & SystemPermissionAccess.Write) != SystemPermissionAccess.NoAccess)
					{
						if(_write == null)
							_write = new SystemAccess();
						_write.AllItems = true;
					}
					else if(_write != null)
						_write.AllItems = false;
					if((value & SystemPermissionAccess.Execute) != SystemPermissionAccess.NoAccess)
					{
						if(_execute == null)
							_execute = new SystemAccess();
						_execute.AllItems = true;
					}
					else if(_execute != null)
						_execute.AllItems = false;
				}
			}
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static SystemPermission()
		{
			_illegalCharacters = new char[] {
				'?',
				'*'
			};
		}

		/// <summary>
		/// Cria uma permissão informado o estado de permissão.
		/// </summary>
		/// <param name="state"></param>
		public SystemPermission(PermissionState state)
		{
			if(state == PermissionState.Unrestricted)
				_unrestricted = true;
			else
			{
				if(state != PermissionState.None)
					throw new ArgumentException(Properties.Resources.Argument_InvalidPermissionState);
				_unrestricted = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="access">Tipo de acesso.</param>
		/// <param name="control">Ações de controle de acesso.</param>
		/// <param name="path">Caminho do sistema onde a permissão será aplicada.</param>
		public SystemPermission(SystemPermissionAccess access, AccessControlActions control, string path)
		{
			this.VerifyAccess(access);
			string[] pathListOrig = new string[] {
				path
			};
			this.AddPathList(access, control, pathListOrig, false, true, false);
		}

		/// <summary>
		/// Cria a permissão com o caminho informado.
		/// </summary>
		/// <param name="access">Tipo de acesso.</param>
		/// <param name="path">Caminho do sistema onde a permissão será aplicada.</param>
		public SystemPermission(SystemPermissionAccess access, string path) : this(access, AccessControlActions.None, path)
		{
		}

		/// <summary>
		/// Dispara a exception de segurança.
		/// </summary>
		/// <param name="message">Mensagem de exception.</param>
		/// <param name="parameters"></param>
		private void ThrowSecurityException(string message, params object[] parameters)
		{
			System.Reflection.AssemblyName assemblyName = null;
			System.Security.Policy.Evidence evidence = null;
			try
			{
				System.Reflection.Assembly callingAssembly = System.Reflection.Assembly.GetCallingAssembly();
				assemblyName = callingAssembly.GetName();
				if(callingAssembly != System.Reflection.Assembly.GetExecutingAssembly())
					evidence = callingAssembly.Evidence;
			}
			catch
			{
			}
			throw new SecurityException(string.Format(message, parameters), assemblyName, null, null, null, SecurityAction.Demand, this, this, evidence);
		}

		/// <summary>
		/// Verifica se os texto informados possui caracteres inválidos.
		/// </summary>
		/// <param name="str"></param>
		private static void HasIllegalCharacters(string[] str)
		{
			for(int i = 0; i < str.Length; i++)
			{
				if(str[i] == null)
					throw new ArgumentNullException("str");
				Colosoft.Util.PathUtil.CheckInvalidPathChars(str[i]);
				if(str[i].IndexOfAny(_illegalCharacters) != -1)
					throw new ArgumentException(Properties.Resources.Argument_InvalidPathChars);
			}
		}

		/// <summary>
		/// Adiciona uma lista de caminhos e acesso para a permissão.
		/// </summary>
		/// <param name="access"></param>
		/// <param name="control"></param>
		/// <param name="pathListOrig"></param>
		/// <param name="checkForDuplicates"></param>
		/// <param name="needFullPath"></param>
		/// <param name="copyPathList"></param>
		internal void AddPathList(SystemPermissionAccess access, AccessControlActions control, string[] pathListOrig, bool checkForDuplicates, bool needFullPath, bool copyPathList)
		{
			this.VerifyAccess(access);
			if(pathListOrig == null)
				throw new ArgumentNullException("pathList");
			if(pathListOrig.Length == 0)
				throw new ArgumentException(Properties.Resources.Argument_EmptyPath);
			if(!_unrestricted)
			{
				string[] destinationArray = pathListOrig;
				if(copyPathList)
				{
					destinationArray = new string[pathListOrig.Length];
					Array.Copy(pathListOrig, destinationArray, pathListOrig.Length);
				}
				HasIllegalCharacters(destinationArray);
				var values = StringExpressionSet.CreateListFromExpressions(destinationArray, needFullPath);
				if((access & SystemPermissionAccess.Read) != SystemPermissionAccess.NoAccess)
				{
					if(_read == null)
						this._read = new SystemAccess();
					this._read.AddExpressions(values, checkForDuplicates);
				}
				if((access & SystemPermissionAccess.Write) != SystemPermissionAccess.NoAccess)
				{
					if(this._write == null)
						this._write = new SystemAccess();
					this._write.AddExpressions(values, checkForDuplicates);
				}
				if((access & SystemPermissionAccess.Execute) != SystemPermissionAccess.NoAccess)
				{
					if(this._execute == null)
						this._execute = new SystemAccess();
					this._execute.AddExpressions(values, checkForDuplicates);
				}
				if((control & AccessControlActions.View) != AccessControlActions.None)
				{
					if(this._viewAcl == null)
						this._viewAcl = new SystemAccess();
					this._viewAcl.AddExpressions(values, checkForDuplicates);
				}
				if((control & AccessControlActions.Change) != AccessControlActions.None)
				{
					if(this._changeAcl == null)
						this._changeAcl = new SystemAccess();
					this._changeAcl.AddExpressions(values, checkForDuplicates);
				}
			}
		}

		/// <summary>
		/// Adiciona uma lista de caminhos e acesso para a permissão.
		/// </summary>
		/// <param name="access"></param>
		/// <param name="pathListOrig"></param>
		/// <param name="checkForDuplicates"></param>
		/// <param name="needFullPath"></param>
		/// <param name="copyPathList"></param>
		internal void AddPathList(SystemPermissionAccess access, string[] pathListOrig, bool checkForDuplicates, bool needFullPath, bool copyPathList)
		{
			this.AddPathList(access, AccessControlActions.None, pathListOrig, checkForDuplicates, needFullPath, copyPathList);
		}

		/// <summary>
		/// Verifica a permissão de acesso.
		/// </summary>
		/// <param name="access"></param>
		/// <param name="question"></param>
		/// <returns></returns>
		private bool AccessIsSet(SystemPermissionAccess access, SystemPermissionAccess question)
		{
			return ((access & question) != SystemPermissionAccess.NoAccess);
		}

		/// <summary>
		/// Verifica a permissão de acesso.
		/// </summary>
		/// <param name="access"></param>
		private void VerifyAccess(SystemPermissionAccess access)
		{
			if((access & ~SystemPermissionAccess.AllAccess) != SystemPermissionAccess.NoAccess)
				throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.Arg_EnumIllegalVal, access));
		}

		/// <summary>
		/// Verifica um acesso exclusivo.
		/// </summary>
		/// <param name="access"></param>
		private void ExclusiveAccess(SystemPermissionAccess access)
		{
			if(access == SystemPermissionAccess.NoAccess)
				throw new ArgumentException(Properties.Resources.Arg_EnumNotSingleFlag);
			if((access & (access - 1)) != SystemPermissionAccess.NoAccess)
				throw new ArgumentException(Properties.Resources.Arg_EnumNotSingleFlag);
		}

		/// <summary>
		/// Verifica se a instancia é vazia.
		/// </summary>
		/// <returns></returns>
		private bool IsEmpty()
		{
			if((((!this._unrestricted && ((this._read == null) || this._read.IsEmpty())) && ((this._write == null) || this._write.IsEmpty())) && (((this._execute == null) || this._execute.IsEmpty()) && ((this._pathDiscovery == null) || this._pathDiscovery.IsEmpty()))) && ((this._viewAcl == null) || this._viewAcl.IsEmpty()))
			{
				if(this._changeAcl != null)
					return this._changeAcl.IsEmpty();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Cria um elemento de permissão.
		/// </summary>
		/// <param name="perm"></param>
		/// <param name="permname"></param>
		/// <returns></returns>
		internal static SecurityElement CreatePermissionElement(IPermission perm, string permname)
		{
			SecurityElement element = new SecurityElement("IPermission");
			XMLUtil.AddClassAttribute(element, perm.GetType(), permname);
			element.AddAttribute("version", "1");
			return element;
		}

		/// <summary>
		/// Valida o elemento de permissão.
		/// </summary>
		/// <param name="elem"></param>
		/// <param name="perm"></param>
		internal static void ValidateElement(SecurityElement elem, IPermission perm)
		{
			if(elem == null)
				throw new ArgumentNullException("elem");
			if(!XMLUtil.IsPermissionElement(perm, elem))
				throw new ArgumentException(Properties.Resources.Argument_NotAPermissionElement);
			string str = elem.Attribute("version");
			if((str != null) && !str.Equals("1"))
				throw new ArgumentException(Properties.Resources.Argument_InvalidXMLBadVersion);
		}

		/// <summary>
		/// Verifica a permissão.
		/// </summary>
		public void Demand()
		{
			var profile = Security.Profile.ProfileManager.CurrentProfile;
			if(profile == null)
				ThrowSecurityException(Properties.Resources.InvalidOperation_NotFoundProfileForCurrentUser);
			if(!profile.RoleSet.IsAllowed(this))
				ThrowSecurityException(Properties.Resources.Security_SystemPermission);
		}

		/// <summary>
		/// Verifica se a permissão informada é permitida no conjunto.
		/// </summary>
		/// <returns></returns>
		public bool IsAllowed()
		{
			var profile = Security.Profile.ProfileManager.CurrentProfile;
			if(profile == null)
				return false;
			return profile.RoleSet.IsAllowed(this);
		}

		/// <summary>
		/// Adiciona uma lista de caminhos e o acesso para a permissão.
		/// </summary>
		/// <param name="access"></param>
		/// <param name="pathList"></param>
		public void AddPathList(SystemPermissionAccess access, string[] pathList)
		{
			AddPathList(access, pathList, true, true, true);
		}

		/// <summary>
		/// Adiciona um caminho e o acesso para a permissão.
		/// </summary>
		/// <param name="access"></param>
		/// <param name="path"></param>
		public void AddPathList(SystemPermissionAccess access, string path)
		{
			string[] strArray;
			if(path == null)
				strArray = new string[0];
			else
				strArray = new string[] {
					path
				};
			AddPathList(access, strArray, false, true, false);
		}

		/// <summary>
		/// Compara um objeto com a instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			SystemPermission permission = obj as SystemPermission;
			if(permission == null)
				return false;
			if(!this._unrestricted || !permission._unrestricted)
			{
				if(this._unrestricted != permission._unrestricted)
					return false;
				if(this._read == null)
				{
					if((permission._read != null) && !permission._read.IsEmpty())
						return false;
				}
				else if(!this._read.Equals(permission._read))
					return false;
				if(this._write == null)
				{
					if((permission._write != null) && !permission._write.IsEmpty())
						return false;
				}
				else if(!this._write.Equals(permission._write))
					return false;
				if(this._execute == null)
				{
					if((permission._execute != null) && !permission._execute.IsEmpty())
						return false;
				}
				else if(!this._execute.Equals(permission._execute))
					return false;
				if(this._pathDiscovery == null)
				{
					if((permission._pathDiscovery != null) && !permission._pathDiscovery.IsEmpty())
						return false;
				}
				else if(!this._pathDiscovery.Equals(permission._pathDiscovery))
					return false;
				if(this._viewAcl == null)
				{
					if((permission._viewAcl != null) && !permission._viewAcl.IsEmpty())
						return false;
				}
				else if(!this._viewAcl.Equals(permission._viewAcl))
					return false;
				if(this._changeAcl == null)
				{
					if((permission._changeAcl != null) && !permission._changeAcl.IsEmpty())
						return false;
				}
				else if(!this._changeAcl.Equals(permission._changeAcl))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Hash code da instancia.
		/// </summary>
		/// <returns></returns>
		[System.Runtime.InteropServices.ComVisible(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Une a instancia com a permissão informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public IPermission Union(IPermission other)
		{
			if(other == null)
				return this.Copy();
			var permission = other as SystemPermission;
			if(permission == null)
				throw new ArgumentException(string.Format(Properties.Resources.Argument_WrongType, base.GetType().FullName));
			if(this.IsUnrestricted() || permission.IsUnrestricted())
				return new SystemPermission(PermissionState.Unrestricted);
			SystemAccess access = (this._read == null) ? permission._read : this._read.Union(permission._read);
			SystemAccess access2 = (this._write == null) ? permission._write : this._write.Union(permission._write);
			SystemAccess access3 = (this._execute == null) ? permission._execute : this._execute.Union(permission._execute);
			SystemAccess access4 = (this._pathDiscovery == null) ? permission._pathDiscovery : this._pathDiscovery.Union(permission._pathDiscovery);
			SystemAccess access5 = (this._viewAcl == null) ? permission._viewAcl : this._viewAcl.Union(permission._viewAcl);
			SystemAccess access6 = (this._changeAcl == null) ? permission._changeAcl : this._changeAcl.Union(permission._changeAcl);
			if(((((access == null) || access.IsEmpty()) && ((access2 == null) || access2.IsEmpty())) && (((access3 == null) || access3.IsEmpty()) && ((access4 == null) || access4.IsEmpty()))) && (((access5 == null) || access5.IsEmpty()) && ((access6 == null) || access6.IsEmpty())))
				return null;
			SystemPermission permission2 = new SystemPermission(PermissionState.None);
			permission2._unrestricted = false;
			permission2._read = access;
			permission2._write = access2;
			permission2._execute = access3;
			permission2._pathDiscovery = access4;
			permission2._viewAcl = access5;
			permission2._changeAcl = access6;
			return permission2;
		}

		/// <summary>
		/// Recupera a intercessão da instancia com a permissão informada.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public IPermission Intersect(IPermission target)
		{
			if(target == null)
				return null;
			SystemPermission permission = target as SystemPermission;
			if(permission == null)
				throw new ArgumentException(string.Format(Properties.Resources.Argument_WrongType, base.GetType().FullName));
			if(this.IsUnrestricted())
				return target.Copy();
			if(permission.IsUnrestricted())
				return this.Copy();
			SystemAccess access = (this._read == null) ? null : this._read.Intersect(permission._read);
			SystemAccess access2 = (this._write == null) ? null : this._write.Intersect(permission._write);
			SystemAccess access3 = (this._execute == null) ? null : this._execute.Intersect(permission._execute);
			SystemAccess access4 = (this._pathDiscovery == null) ? null : this._pathDiscovery.Intersect(permission._pathDiscovery);
			SystemAccess access5 = (this._viewAcl == null) ? null : this._viewAcl.Intersect(permission._viewAcl);
			SystemAccess access6 = (this._changeAcl == null) ? null : this._changeAcl.Intersect(permission._changeAcl);
			if(((((access == null) || access.IsEmpty()) && ((access2 == null) || access2.IsEmpty())) && (((access3 == null) || access3.IsEmpty()) && ((access4 == null) || access4.IsEmpty()))) && (((access5 == null) || access5.IsEmpty()) && ((access6 == null) || access6.IsEmpty())))
				return null;
			SystemPermission permission2 = new SystemPermission(PermissionState.None);
			permission2._unrestricted = false;
			permission2._read = access;
			permission2._write = access2;
			permission2._execute = access3;
			permission2._pathDiscovery = access4;
			permission2._viewAcl = access5;
			permission2._changeAcl = access6;
			return permission2;
		}

		/// <summary>
		/// Verifica se a permissão informada é um subconjunto da instancia.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool IsSubsetOf(IPermission target)
		{
			if(target == null)
				return this.IsEmpty();
			SystemPermission permission = target as SystemPermission;
			if(permission == null)
				throw new ArgumentException(string.Format(Properties.Resources.Argument_WrongType, base.GetType().FullName));
			if(permission.IsUnrestricted())
				return true;
			if(!this.IsUnrestricted() && (((((this._read == null) || this._read.IsSubsetOf(permission._read)) && ((this._write == null) || this._write.IsSubsetOf(permission._write))) && (((this._execute == null) || this._execute.IsSubsetOf(permission._execute)) && ((this._pathDiscovery == null) || this._pathDiscovery.IsSubsetOf(permission._pathDiscovery)))) && ((this._viewAcl == null) || this._viewAcl.IsSubsetOf(permission._viewAcl))))
			{
				if(this._changeAcl != null)
					return this._changeAcl.IsSubsetOf(permission._changeAcl);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Verifica se a instancia está sem restrição.
		/// </summary>
		/// <returns></returns>
		public bool IsUnrestricted()
		{
			return _unrestricted;
		}

		/// <summary>
		/// Cria uma cópia da permissão.
		/// </summary>
		/// <returns></returns>
		public IPermission Copy()
		{
			SystemPermission permission = new SystemPermission(PermissionState.None);
			if(this._unrestricted)
			{
				permission._unrestricted = true;
				return permission;
			}
			permission._unrestricted = false;
			if(this._read != null)
				permission._read = this._read.Copy();
			if(this._write != null)
				permission._write = this._write.Copy();
			if(this._execute != null)
				permission._execute = this._execute.Copy();
			if(this._pathDiscovery != null)
				permission._pathDiscovery = this._pathDiscovery.Copy();
			if(this._viewAcl != null)
				permission._viewAcl = this._viewAcl.Copy();
			if(this._changeAcl != null)
				permission._changeAcl = this._changeAcl.Copy();
			return permission;
		}

		/// <summary>
		/// Recupera a lista de caminhos com o acesso informado.
		/// </summary>
		/// <param name="access"></param>
		/// <returns></returns>
		public string[] GetPathList(SystemPermissionAccess access)
		{
			this.VerifyAccess(access);
			this.ExclusiveAccess(access);
			if(this.AccessIsSet(access, SystemPermissionAccess.Read))
			{
				if(this._read == null)
					return null;
				return this._read.ToStringArray();
			}
			if(this.AccessIsSet(access, SystemPermissionAccess.Write))
			{
				if(this._write == null)
					return null;
				return this._write.ToStringArray();
			}
			if(this.AccessIsSet(access, SystemPermissionAccess.Execute))
			{
				if(this._execute == null)
					return null;
				return this._execute.ToStringArray();
			}
			return new string[0];
		}

		/// <summary>
		/// Recupera os dados da permissão do elemento de segurança informado.
		/// </summary>
		/// <param name="esd"></param>
		public void FromXml(SecurityElement esd)
		{
			ValidateElement(esd, this);
			if(XMLUtil.IsUnrestricted(esd))
				_unrestricted = true;
			else
			{
				_unrestricted = false;
				string str = esd.Attribute("Read");
				if(str != null)
					_read = new SystemAccess(str.TrimStart().TrimEnd());
				else
					_read = null;
				str = esd.Attribute("Write");
				if(str != null)
					_write = new SystemAccess(str.TrimStart().TrimEnd());
				else
					_write = null;
				str = esd.Attribute("Execute");
				if(str != null)
					_execute = new SystemAccess(str.TrimStart().TrimEnd());
				else
					_execute = null;
				str = esd.Attribute("PathDiscovery");
				if(str != null)
				{
					_pathDiscovery = new SystemAccess(str.TrimStart().TrimEnd());
					_pathDiscovery.PathDiscovery = true;
				}
				else
					_pathDiscovery = null;
				str = esd.Attribute("ViewAcl");
				if(str != null)
					_viewAcl = new SystemAccess(str.TrimStart().TrimEnd());
				else
					_viewAcl = null;
				str = esd.Attribute("ChangeAcl");
				if(str != null)
					_changeAcl = new SystemAccess(str.TrimStart().TrimEnd());
				else
					_changeAcl = null;
			}
		}

		/// <summary>
		/// Recupera o elemento XML de segurança da instancia.
		/// </summary>
		/// <returns></returns>
		public SecurityElement ToXml()
		{
			SecurityElement element = CreatePermissionElement(this, "Colosoft.Security.Permissions.SystemPermission");
			if(!this.IsUnrestricted())
			{
				if((this._read != null) && !this._read.IsEmpty())
					element.AddAttribute("Read", SecurityElement.Escape(_read.ToString()));
				if((this._write != null) && !this._write.IsEmpty())
					element.AddAttribute("Write", SecurityElement.Escape(_write.ToString()));
				if((this._execute != null) && !this._execute.IsEmpty())
					element.AddAttribute("Execute", SecurityElement.Escape(_execute.ToString()));
				if((this._pathDiscovery != null) && !this._pathDiscovery.IsEmpty())
					element.AddAttribute("PathDiscovery", SecurityElement.Escape(_pathDiscovery.ToString()));
				if((this._viewAcl != null) && !this._viewAcl.IsEmpty())
					element.AddAttribute("ViewAcl", SecurityElement.Escape(_viewAcl.ToString()));
				if((this._changeAcl != null) && !this._changeAcl.IsEmpty())
					element.AddAttribute("ChangeAcl", SecurityElement.Escape(_changeAcl.ToString()));
				return element;
			}
			element.AddAttribute("Unrestricted", "true");
			return element;
		}
	}
}
