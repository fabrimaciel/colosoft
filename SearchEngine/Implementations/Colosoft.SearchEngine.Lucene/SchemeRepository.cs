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
using System.ComponentModel.Composition;

namespace Colosoft.SearchEngine.Lucene
{
	/// <summary>
	/// Implementação do repositório de esquema.
	/// </summary>
	[Export(typeof(ISchemeRepository))]
	public class SchemeRepository : ISchemeRepository
	{
		private bool _initialized;

		private List<SchemeField> _fieldSchemes;

		private List<SchemeIndex> _fieldSchemeIndex;

		private IDictionary<byte, Channel> _channels;

		private readonly IRepositoryLoader _loader;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="loader">Loader dos dados.</param>
		[ImportingConstructor]
		public SchemeRepository(IRepositoryLoader loader)
		{
			if(loader == null)
				throw new ArgumentNullException("loader");
			_loader = loader;
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		public void Initialize()
		{
			if(!_initialized)
			{
				_initialized = true;
				_fieldSchemes = new List<SchemeField>(_loader.GetSchemeFields());
				_fieldSchemeIndex = new List<SchemeIndex>(_loader.GetSchemeIndex());
				_fieldSchemes.Sort(SchemeField.NameComparerSchemeField);
				_channels = new Dictionary<byte, Channel>();
				using (var channelsEnumerator = _loader.GetChannels().GetEnumerator())
				{
					while (channelsEnumerator.MoveNext())
					{
						var channel = channelsEnumerator.Current;
						_channels.Add(channel.ChannelId, channel);
					}
				}
			}
		}

		/// <summary>
		/// Retorna a lista com os schemas dos campos
		/// </summary>
		/// <returns>Lista de schemas</returns>
		public IList<SchemeField> GetFieldSchemas()
		{
			return _fieldSchemes;
		}

		/// <summary>
		/// Recupera os dados do canal pelo identificador informado.
		/// </summary>
		/// <param name="channelId"></param>
		/// <returns></returns>
		public Channel GetChannel(byte channelId)
		{
			Channel result = null;
			if(_channels.TryGetValue(channelId, out result))
				return result;
			return null;
		}

		/// <summary>
		/// Carrega os canais cadastrados
		/// </summary>
		/// <returns>Dicionário de canais e seus Ids</returns>
		public ICollection<Channel> GetChannels()
		{
			return _channels.Values;
		}

		/// <summary>
		/// Recupera o campo do esquema.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SchemeField GetSchemaField(string name)
		{
			_fieldSchemes = new List<SchemeField>(_loader.GetSchemeFields());
			return _fieldSchemes.Where(f => f.Name == name).FirstOrDefault();
		}

		/// <summary>
		/// Recupera o indice do esquema.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SchemeIndex GetSchemaIndex(string name)
		{
			_fieldSchemeIndex = new List<SchemeIndex>(_loader.GetSchemeIndex());
			return _fieldSchemeIndex.Where(f => f.Name == name).FirstOrDefault();
		}
	}
}
