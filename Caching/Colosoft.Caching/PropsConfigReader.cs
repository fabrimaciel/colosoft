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
using Colosoft.Caching.Exceptions;
using System.Collections;

namespace Colosoft.Caching.Configuration
{
	/// <summary>
	/// Implementação de um leitor de configuraçãos de propriedades.
	/// </summary>
	public class PropsConfigReader : ConfigReader
	{
		private string _propString;

		/// <summary>
		/// Hashtable as propriedades.
		/// </summary>
		public override Hashtable Properties
		{
			get
			{
				return this.GetProperties(_propString);
			}
		}

		/// <summary>
		/// String com as propriedades.
		/// </summary>
		public string PropertyString
		{
			get
			{
				return _propString;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="propString"></param>
		public PropsConfigReader(string propString)
		{
			_propString = propString;
		}

		/// <summary>
		/// Recupera as propriedades da string informada.
		/// </summary>
		/// <param name="propString"></param>
		/// <returns></returns>
		private Hashtable GetProperties(string propString)
		{
			bool flag = false;
			Hashtable hashtable = new Hashtable();
			Tokenizer tokenizer = new Tokenizer(propString);
			string tokenValue = "";
			int num = 0;
			State keyNeeded = State.keyNeeded;
			Stack stack = new Stack();
			while (true)
			{
				switch(tokenizer.GetNextToken())
				{
				case Tokenizer.UNNEST:
					if(keyNeeded != State.keyNeeded)
						throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_PropsConfigReader_CloseParanthesisMisplaced).Format());
					if(num < 1)
						throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_PropsConfigReader_CloseParenthesisUnexpected).Format());
					if(flag)
						flag = false;
					hashtable = stack.Pop() as Hashtable;
					num--;
					continue;
				case Tokenizer.ID:
					int num2;
					switch(keyNeeded)
					{
					case State.keyNeeded:
						if(tokenValue == "parameters")
							flag = true;
						tokenValue = tokenizer.TokenValue;
						num2 = tokenizer.GetNextToken();
						if(((num2 == Tokenizer.CONTINUE) || (num2 == Tokenizer.UNNEST)) || ((num2 == Tokenizer.ID) || (num2 == Tokenizer.EOF)))
							throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_PropsConfigReader_KeyFollowingBadToken).Format());
						switch(num2)
						{
						case Tokenizer.ASSIGN:
							keyNeeded = State.valNeeded;
							continue;
						case Tokenizer.NEST:
							stack.Push(hashtable);
							hashtable[tokenValue.ToLower()] = new Hashtable();
							hashtable = hashtable[tokenValue.ToLower()] as Hashtable;
							keyNeeded = State.keyNeeded;
							num++;
							break;
						}
						continue;
					case State.valNeeded:
					{
						string str2 = tokenizer.TokenValue;
						num2 = tokenizer.GetNextToken();
						keyNeeded = State.keyNeeded;
						switch(num2)
						{
						case Tokenizer.ASSIGN:
						case Tokenizer.ID:
						case Tokenizer.EOF:
							throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_PropsConfigReader_ValueFollowingBadToken).Format());
						}
						if(flag)
							hashtable[tokenValue] = str2;
						else
							hashtable[tokenValue.ToLower()] = str2;
						switch(num2)
						{
						case Tokenizer.NEST:
							stack.Push(hashtable);
							hashtable[tokenValue.ToLower()] = new Hashtable();
							hashtable = hashtable[tokenValue.ToLower()] as Hashtable;
							hashtable.Add("id", tokenValue);
							hashtable.Add("type", str2);
							keyNeeded = State.keyNeeded;
							num++;
							break;
						case Tokenizer.UNNEST:
							if(num < 1)
								throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_PropsConfigReader_CloseParenthesisUnexpected).Format());
							if(flag)
								flag = false;
							hashtable = stack.Pop() as Hashtable;
							num--;
							keyNeeded = State.keyNeeded;
							break;
						}
						continue;
					}
					}
					continue;
				case Tokenizer.EOF:
					if(keyNeeded != State.keyNeeded)
						throw new ConfigurationException(ResourceMessageFormatter.Create(() => Caching.Properties.Resources.ConfigurationException_PropsConfigReader_InvalidEOF).Format());
					if(num > 0)
						throw new ConfigurationException("Invalid property string, un-matched paranthesis");
					return hashtable;
				}
				throw new ConfigurationException("Invalid property string");
			}
		}

		/// <summary>
		/// Converte as propriedades para xml.
		/// </summary>
		/// <returns></returns>
		public string ToPropertiesXml()
		{
			return ConfigReader.ToPropertiesXml(this.Properties);
		}

		/// <summary>
		/// Possíveis estados do Tokenizer.
		/// </summary>
		private enum State
		{
			keyNeeded,
			valNeeded
		}

		/// <summary>
		/// Tokenizer das propriedades.
		/// </summary>
		private class Tokenizer
		{
			/// <summary>
			/// Identifica um sinal
			/// </summary>
			public const int ASSIGN = 0;

			/// <summary>
			/// Identifica um continue.
			/// </summary>
			public const int CONTINUE = 3;

			/// <summary>
			/// Identifica fim de arquivo.
			/// </summary>
			public const int EOF = -1;

			/// <summary>
			/// Identifica que é os dados de um identificador.
			/// </summary>
			public const int ID = 4;

			/// <summary>
			/// Aninhado.
			/// </summary>
			public const int NEST = 1;

			/// <summary>
			/// Desaninhado.
			/// </summary>
			public const int UNNEST = 2;

			private int _index;

			private string _text;

			private string _token;

			/// <summary>
			/// Valor do token.
			/// </summary>
			public string TokenValue
			{
				get
				{
					return _token;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="text">Texto que será processado.</param>
			public Tokenizer(string text)
			{
				_text = text;
			}

			/// <summary>
			/// Recupera o identificador.
			/// </summary>
			/// <returns></returns>
			private string GetIdentifier()
			{
				StringBuilder builder = new StringBuilder();
				if(_text[_index] != '\'')
				{
					while (_index < _text.Length)
					{
						if(((_text[_index] == '\r') || (_text[_index] == '\n')) || (_text[_index] == '\t'))
							_index++;
						else
						{
							if("=();".IndexOf(_text[_index]) != -1)
								return builder.ToString();
							builder.Append(_text[_index++]);
						}
					}
					return null;
				}
				_index++;
				while (_index < _text.Length)
				{
					if(_text[_index] == '\'')
					{
						_index++;
						return builder.ToString();
					}
					if(((_text[_index] == '\r') || (_text[_index] == '\n')) || (_text[_index] == '\t'))
						_index++;
					else
						builder.Append(_text[_index++]);
				}
				return null;
			}

			/// <summary>
			/// Recupera o próximo token.
			/// </summary>
			/// <returns></returns>
			public int GetNextToken()
			{
				string str = "=();";
				while (_index < _text.Length)
				{
					if(str.IndexOf(_text[_index]) != -1)
					{
						_token = _text[_index].ToString();
						return str.IndexOf(_text[_index++]);
					}
					if(((_text[_index] == '\r') || (_text[_index] == '\n')) || ((_text[_index] == '\t') || (_text[_index] == ' ')))
						_index++;
					else
					{
						_token = this.GetIdentifier();
						if(_token != null)
							return ID;
					}
				}
				return EOF;
			}
		}
	}
}
