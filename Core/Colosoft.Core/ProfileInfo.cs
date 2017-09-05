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

namespace Colosoft.Security.Profile
{
	/// <summary>
	/// Armazena as informações do perfil.
	/// </summary>
	[Serializable]
	public class ProfileInfo : IEquatable<ProfileInfo>, System.Runtime.Serialization.ISerializable
	{
		private int _profileId;

		private bool _isAnonymous;

		private DateTimeOffset _lastActivityDate;

		private DateTimeOffset _lastUpdatedDate;

		private string _userName;

		private string _fullName;

		private ProfileSearchMode _searchMode;

		[NonSerialized]
		private IAuthenticationSource _source;

		private int? _markGroupId;

		private int? _sellerTreeId;

		private int? _intermediateId;

		/// <summary>
		/// Identificador do perfil.
		/// </summary>
		public int ProfileId
		{
			get
			{
				return _profileId;
			}
			set
			{
				_profileId = value;
			}
		}

		/// <summary>
		/// Nome do perfil.
		/// </summary>
		public string FullName
		{
			get
			{
				return _fullName;
			}
			set
			{
				_fullName = value;
			}
		}

		/// <summary>
		/// Nome do usuário do perfil
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
			}
		}

		/// <summary>
		/// Modo de pesquisa associado com o perfil.
		/// </summary>
		public ProfileSearchMode SearchMode
		{
			get
			{
				return _searchMode;
			}
			set
			{
				_searchMode = value;
			}
		}

		/// <summary>
		/// Origem da autenticação.
		/// </summary>
		public IAuthenticationSource Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}

		/// <summary>
		/// Data da ultima atividade com o perfil.
		/// </summary>
		public DateTimeOffset LastActivityDate
		{
			get
			{
				return _lastActivityDate;
			}
			set
			{
				_lastActivityDate = value;
			}
		}

		/// <summary>
		/// Data da ultima vez que o perfil foi atualizado.
		/// </summary>
		public DateTimeOffset LastUpdatedDate
		{
			get
			{
				return _lastUpdatedDate;
			}
			set
			{
				_lastUpdatedDate = value;
			}
		}

		/// <summary>
		/// Identifica se o usuário do perfil é anonimo.
		/// </summary>
		public bool IsAnonymous
		{
			get
			{
				return _isAnonymous;
			}
			set
			{
				_isAnonymous = value;
			}
		}

		/// <summary>
		/// Grupo de marcadores associado ao perfil.
		/// </summary>
		public int? MarkGroupId
		{
			get
			{
				return _markGroupId;
			}
			set
			{
				_markGroupId = value;
			}
		}

		/// <summary>
		/// Identificador da árvore de vendedores associada ao perfil.
		/// </summary>
		public int? SellerTreeId
		{
			get
			{
				return _sellerTreeId;
			}
			set
			{
				_sellerTreeId = value;
			}
		}

		/// <summary>
		/// Identificador do intermediador associado ao perfil.
		/// </summary>
		public int? IntermediateId
		{
			get
			{
				return _intermediateId;
			}
			set
			{
				_intermediateId = value;
			}
		}

		/// <summary>
		/// Construtor usado na serialização.
		/// </summary>
		public ProfileInfo()
		{
			_searchMode = ProfileSearchMode.All;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="profileId"></param>
		/// <param name="username"></param>
		/// <param name="fullName"></param>
		/// <param name="searchMode">Modo de pesquisa associado.</param>
		/// <param name="source">Source.</param>
		/// <param name="isAnonymous"></param>
		/// <param name="lastActivityDate"></param>
		/// <param name="lastUpdatedDate"></param>
		/// <param name="markGroupId"></param>
		/// <param name="sellerTreeId"></param>
		/// <param name="intermediateId"></param>
		public ProfileInfo(int profileId, string username, string fullName, bool isAnonymous, ProfileSearchMode searchMode, IAuthenticationSource source, DateTimeOffset lastActivityDate, DateTimeOffset lastUpdatedDate, int? markGroupId, int? sellerTreeId, int? intermediateId)
		{
			_profileId = profileId;
			_fullName = fullName;
			if(username != null)
				username = username.Trim();
			_userName = username;
			_source = source;
			_lastActivityDate = lastActivityDate;
			_lastUpdatedDate = lastUpdatedDate;
			_isAnonymous = isAnonymous;
			_markGroupId = markGroupId;
			_sellerTreeId = sellerTreeId;
			_intermediateId = intermediateId;
			_searchMode = searchMode;
		}

		/// <summary>
		/// Cria uma instancia com base nos dados do perfil.
		/// </summary>
		/// <param name="profile"></param>
		public ProfileInfo(IProfile profile)
		{
			profile.Require("profile").NotNull();
			_profileId = profile.ProfileId;
			_fullName = profile.FullName;
			_userName = profile.UserName;
			_source = profile.Source;
			_lastActivityDate = profile.LastActivityDate;
			_lastUpdatedDate = profile.LastUpdatedDate;
			_isAnonymous = profile.IsAnonymous;
			_markGroupId = profile.MarkGroupId;
			_searchMode = profile.SearchMode;
			_sellerTreeId = profile.SellerTreeId;
			_intermediateId = profile.IntermediateId;
		}

		/// <summary>
		/// Construtor usado na serialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ProfileInfo(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_profileId = info.GetInt32("ProfileId");
			_isAnonymous = info.GetBoolean("IsAnonymous");
			_lastActivityDate = (DateTimeOffset)info.GetValue("LastActivityDate", typeof(DateTimeOffset));
			_lastUpdatedDate = (DateTimeOffset)info.GetValue("LastUpdatedDate", typeof(DateTimeOffset));
			_userName = info.GetString("UserName");
			_fullName = info.GetString("FullName");
			_searchMode = (ProfileSearchMode)info.GetInt32("SearchMode");
			_source = (IAuthenticationSource)info.GetValue("Source", typeof(AuthenticationSource));
			_markGroupId = (int?)info.GetValue("MarkGroupId", typeof(int?));
			_sellerTreeId = (int?)info.GetValue("SellerTreeId", typeof(int?));
			_intermediateId = (int?)info.GetValue("IntermediateId", typeof(int?));
		}

		/// <summary>
		/// Verifica os dados das instancia são equivalentes.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ProfileInfo other)
		{
			if(other == null)
				return false;
			return (FullName == other.FullName && UserName == other.UserName && Source != null && other.Source != null && Source.FullName == other.Source.FullName && Nullable<int>.Equals(MarkGroupId, other.MarkGroupId) && Nullable<int>.Equals(SellerTreeId, other.SellerTreeId) && Nullable<int>.Equals(IntermediateId, other.IntermediateId));
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase"), System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("ProfileId", ProfileId);
			info.AddValue("IsAnonymous", IsAnonymous);
			info.AddValue("LastActivityDate", LastActivityDate, typeof(DateTimeOffset));
			info.AddValue("LastUpdatedDate", LastUpdatedDate, typeof(DateTimeOffset));
			info.AddValue("UserName", UserName);
			info.AddValue("FullName", FullName);
			info.AddValue("SearchMode", (int)SearchMode);
			info.AddValue("MarkGroupId", MarkGroupId, typeof(int?));
			info.AddValue("SellerTreeId", SellerTreeId, typeof(int?));
			info.AddValue("IntermediateId", IntermediateId, typeof(int?));
		}
	}
}
