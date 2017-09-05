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
using System.Collections;

namespace Colosoft.Security.Util
{
	[Serializable]
	internal class StringExpressionSet
	{
		protected static readonly char _alternateDirectorySeparator;

		protected static readonly char _directorySeparator;

		protected string _expressions;

		protected string[] _expressionsArray;

		protected bool _ignoreCase;

		protected List<string> _list;

		protected static readonly char[] _separators;

		protected bool _throwOnRelative;

		protected static readonly char[] _trimChars;

		/// <summary>
		/// Construtor usado para instancia as váriaveis da classe.
		/// </summary>
		static StringExpressionSet()
		{
			_separators = new char[] {
				';'
			};
			_trimChars = new char[] {
				' '
			};
			_directorySeparator = '\\';
			_alternateDirectorySeparator = '/';
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public StringExpressionSet() : this(true, null, false)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreCase">Identifica se é para ignorar a comparação das strings.</param>
		/// <param name="str">String do conjunto.</param>
		/// <param name="throwOnRelative">Identifica se é para disparar um excessão caso a expressão seja relativa.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214")]
		public StringExpressionSet(bool ignoreCase, string str, bool throwOnRelative)
		{
			_list = null;
			_ignoreCase = ignoreCase;
			_throwOnRelative = throwOnRelative;
			if(str == null)
				_expressions = null;
			else
				AddExpressions(str);
		}

		/// <summary>
		/// Constrói um novo cojunto de expressões.
		/// </summary>
		/// <param name="str">Texto da expressões para inicializar a instancia.</param>
		public StringExpressionSet(string str) : this(true, str, false)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreCase"></param>
		/// <param name="throwOnRelative"></param>
		public StringExpressionSet(bool ignoreCase, bool throwOnRelative) : this(ignoreCase, null, throwOnRelative)
		{
		}

		/// <summary>
		/// Processa a string inteira.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static string StaticProcessWholeString(string str)
		{
			return str.Replace(_alternateDirectorySeparator, _directorySeparator);
		}

		/// <summary>
		/// Processa uma string simples.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static string StaticProcessSingleString(string str)
		{
			return str.Trim(_trimChars);
		}

		/// <summary>
		/// Recupera o caminho correto.
		/// </summary>
		/// <param name="path">Caminho que será processado.</param>
		/// <param name="needFullPath">Identifica se é para recupera o caminho completo.</param>
		/// <returns></returns>
		internal static string CanonicalizePath(string path, bool needFullPath)
		{
			if(path.IndexOf(':', 2) != -1)
				throw new NotSupportedException(Properties.Resources.Argument_PathFormatNotSupported);
			return path;
		}

		/// <summary>
		/// Recupera o caminho correto.
		/// </summary>
		/// <param name="path">Caminho que será processado.</param>
		/// <returns></returns>
		internal static string CanonicalizePath(string path)
		{
			return CanonicalizePath(path, true);
		}

		/// <summary>
		/// Cria uma lista de items das expressões informadas
		/// </summary>
		/// <param name="str"></param>
		/// <param name="needFullPath"></param>
		/// <returns></returns>
		internal static List<string> CreateListFromExpressions(string[] str, bool needFullPath)
		{
			if(str == null)
				throw new ArgumentNullException("str");
			List<string> list = new List<string>();
			for(int i = 0; i < str.Length; i++)
			{
				if(str[i] == null)
					throw new ArgumentNullException("str");
				string str2 = StaticProcessWholeString(str[i]);
				if((str2 != null) && (str2.Length != 0))
				{
					string path = StaticProcessSingleString(str2);
					int index = path.IndexOf('\0');
					if(index != -1)
						path = path.Substring(0, index);
					if((path != null) && (path.Length != 0))
					{
						path = CanonicalizePath(path, needFullPath);
						list.Add(path);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Cria a lista do conjunto.
		/// </summary>
		protected void CreateList()
		{
			string[] strArray = Split(_expressions);
			_list = new List<string>();
			for(int i = 0; i < strArray.Length; i++)
			{
				if((strArray[i] != null) && !strArray[i].Equals(""))
				{
					string path = this.ProcessSingleString(strArray[i]);
					int index = path.IndexOf('\0');
					if(index != -1)
						path = path.Substring(0, index);
					if((path != null) && !path.Equals(""))
					{
						if(_throwOnRelative)
						{
							path = CanonicalizePath(path);
						}
						_list.Add(path);
					}
				}
			}
		}

		/// <summary>
		/// Verifica a lista do conjunto.
		/// </summary>
		protected void CheckList()
		{
			if((_list == null) && (_expressions != null))
				CreateList();
		}

		/// <summary>
		/// Processa a string inteira.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		protected virtual string ProcessWholeString(string str)
		{
			return StaticProcessWholeString(str);
		}

		/// <summary>
		/// Processa uma string simples.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		protected virtual string ProcessSingleString(string str)
		{
			return StaticProcessSingleString(str);
		}

		/// <summary>
		/// Cria um conjunto vazio.
		/// </summary>
		/// <returns></returns>
		protected virtual StringExpressionSet CreateNewEmpty()
		{
			return new StringExpressionSet();
		}

		/// <summary>
		/// Reduz o conjunto.
		/// </summary>
		protected void Reduce()
		{
			this.CheckList();
			if(_list != null)
			{
				for(int i = 0; i < (_list.Count - 1); i++)
				{
					int index = i + 1;
					while (index < this._list.Count)
					{
						if(this.StringSubsetString(this._list[index], this._list[i], this._ignoreCase))
							this._list.RemoveAt(index);
						else
						{
							if(this.StringSubsetString(this._list[i], this._list[index], this._ignoreCase))
							{
								_list[i] = this._list[index];
								_list.RemoveAt(index);
								index = i + 1;
								continue;
							}
							index++;
						}
					}
				}
			}
		}

		/// <summary>
		/// Quebra as expressões em partes.
		/// </summary>
		/// <param name="expressions"></param>
		/// <returns></returns>
		protected string[] Split(string expressions)
		{
			if(!_throwOnRelative)
				return expressions.Split(_separators);
			var list = new List<string>();
			string[] strArray = expressions.Split(new char[] {
				'"'
			});
			for(int i = 0; i < strArray.Length; i++)
			{
				if((i % 2) == 0)
				{
					string[] strArray2 = strArray[i].Split(new char[] {
						';'
					});
					for(int j = 0; j < strArray2.Length; j++)
					{
						if((strArray2[j] != null) && !strArray2[j].Equals(""))
						{
							list.Add(strArray2[j]);
						}
					}
				}
				else
				{
					list.Add(strArray[i]);
				}
			}
			string[] strArray3 = new string[list.Count];
			IEnumerator enumerator = list.GetEnumerator();
			int num3 = 0;
			while (enumerator.MoveNext())
			{
				strArray3[num3++] = (string)enumerator.Current;
			}
			return strArray3;
		}

		/// <summary>
		/// Verifica se a string é um subconjunto de expressões informado.
		/// </summary>
		/// <param name="left">String que será usada na verificação.</param>
		/// <param name="right">Conjunto de expressões onde será feita a verificação.</param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		protected bool StringSubsetStringExpression(string left, StringExpressionSet right, bool ignoreCase)
		{
			for(int i = 0; i < right._list.Count; i++)
			{
				if(this.StringSubsetString(left, (string)right._list[i], ignoreCase))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Verifica se a string é um subconjunto de uma string.
		/// </summary>
		/// <param name="left">String que será usada na verificação.</param>
		/// <param name="right">String onde será feita a verificação.</param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		protected virtual bool StringSubsetString(string left, string right, bool ignoreCase)
		{
			StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			if(((right == null) || (left == null)) || (((right.Length == 0) || (left.Length == 0)) || (right.Length > left.Length)))
				return false;
			if(right.Length == left.Length)
				return (string.Compare(right, left, comparisonType) == 0);
			if(((left.Length - right.Length) == 1) && (left[left.Length - 1] == _directorySeparator))
				return (string.Compare(left, 0, right, 0, right.Length, comparisonType) == 0);
			if(right[right.Length - 1] == _directorySeparator)
				return (string.Compare(right, 0, left, 0, right.Length, comparisonType) == 0);
			return ((left[right.Length] == _directorySeparator) && (string.Compare(right, 0, left, 0, right.Length, comparisonType) == 0));
		}

		/// <summary>
		/// Verifica se a string é um subconjunto de um conjunto de expressões de um caminho descoberto.
		/// </summary>
		/// <param name="left">String que será usada na verificação.</param>
		/// <param name="right">Conjunto de expressões onde será feita a verificação.</param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		protected static bool StringSubsetStringExpressionPathDiscovery(string left, StringExpressionSet right, bool ignoreCase)
		{
			for(int i = 0; i < right._list.Count; i++)
			{
				if(StringSubsetStringPathDiscovery(left, (string)right._list[i], ignoreCase))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Verifica se a string é um subconjunto de uma string de um caminho descoberto.
		/// </summary>
		/// <param name="left">String que será usada na verificação.</param>
		/// <param name="right">String do caminho.</param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		protected static bool StringSubsetStringPathDiscovery(string left, string right, bool ignoreCase)
		{
			string str;
			string str2;
			StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			if(((right == null) || (left == null)) || ((right.Length == 0) || (left.Length == 0)))
				return false;
			if(right.Length == left.Length)
				return (string.Compare(right, left, comparisonType) == 0);
			if(right.Length < left.Length)
			{
				str = right;
				str2 = left;
			}
			else
			{
				str = left;
				str2 = right;
			}
			if(string.Compare(str, 0, str2, 0, str.Length, comparisonType) != 0)
				return false;
			if(((str.Length != 3) || !str.EndsWith(@":\", StringComparison.Ordinal)) || (((str[0] < 'A') || (str[0] > 'Z')) && ((str[0] < 'a') || (str[0] > 'z'))))
				return (str2[str.Length] == _directorySeparator);
			return true;
		}

		/// <summary>
		/// Insere um expressão simples sem duplicatas.
		/// </summary>
		/// <param name="expression">Expressão que será adicionada.</param>
		protected void AddSingleExpressionNoDuplicates(string expression)
		{
			int index = 0;
			_expressionsArray = null;
			_expressions = null;
			if(_list == null)
				_list = new List<string>();
			while (index < this._list.Count)
			{
				if(this.StringSubsetString(_list[index], expression, this._ignoreCase))
					this._list.RemoveAt(index);
				else
				{
					if(this.StringSubsetString(expression, _list[index], this._ignoreCase))
						return;
					index++;
				}
			}
			_list.Add(expression);
		}

		/// <summary>
		/// Gera a String do conjunto.
		/// </summary>
		protected void GenerateString()
		{
			if(_list != null)
			{
				StringBuilder builder = new StringBuilder();
				IEnumerator<string> enumerator = _list.GetEnumerator();
				bool flag = true;
				while (enumerator.MoveNext())
				{
					if(!flag)
						builder.Append(_separators[0]);
					else
						flag = false;
					string current = enumerator.Current;
					if(current != null)
					{
						int index = current.IndexOf(_separators[0]);
						if(index != -1)
							builder.Append('"');
						builder.Append(current);
						if(index != -1)
							builder.Append('"');
					}
				}
				_expressions = builder.ToString();
			}
			else
				_expressions = null;
		}

		/// <summary>
		/// Cria um cópia do conjunto.
		/// </summary>
		/// <returns></returns>
		public virtual StringExpressionSet Copy()
		{
			StringExpressionSet set = this.CreateNewEmpty();
			if(_list != null)
			{
				set._list = new List<string>(_list);
			}
			set._expressions = _expressions;
			set._ignoreCase = _ignoreCase;
			set._throwOnRelative = _throwOnRelative;
			return set;
		}

		/// <summary>
		/// Adiciona novas expressões.
		/// </summary>
		/// <param name="str"></param>
		public void AddExpressions(string str)
		{
			if(str == null)
				throw new ArgumentNullException("str");
			if(str.Length != 0)
			{
				str = ProcessWholeString(str);
				if(_expressions == null)
					_expressions = str;
				else
					_expressions = this._expressions + _separators[0] + str;
				_expressionsArray = null;
				string[] strArray = this.Split(str);
				if(_list == null)
					_list = new List<string>();
				for(int i = 0; i < strArray.Length; i++)
				{
					if((strArray[i] != null) && !strArray[i].Equals(""))
					{
						string path = this.ProcessSingleString(strArray[i]);
						int index = path.IndexOf('\0');
						if(index != -1)
						{
							path = path.Substring(0, index);
						}
						if((path != null) && !path.Equals(""))
						{
							if(_throwOnRelative)
							{
								path = CanonicalizePath(path);
							}
							_list.Add(path);
						}
					}
				}
				Reduce();
			}
		}

		/// <summary>
		/// Adiciona várias expressões para o conjunto.
		/// </summary>
		/// <param name="expressions"></param>
		/// <param name="checkForDuplicates">True para verifica duplicatas.</param>
		public void AddExpressions(List<string> expressions, bool checkForDuplicates)
		{
			_expressionsArray = null;
			_expressions = null;
			if(_list != null)
				_list.AddRange(expressions);
			else
				_list = new List<string>(expressions);
			if(checkForDuplicates)
				this.Reduce();
		}

		/// <summary>
		/// Adiciona várias expressões para o conjunto. 
		/// </summary>
		/// <param name="str">Expressões que serão adicionadas.</param>
		/// <param name="checkForDuplicates">True para verifica duplicatas.</param>
		/// <param name="needFullPath">True é for necessário o caminho completo.</param>
		public void AddExpressions(string[] str, bool checkForDuplicates, bool needFullPath)
		{
			this.AddExpressions(CreateListFromExpressions(str, needFullPath), checkForDuplicates);
		}

		/// <summary>
		/// Verifica se o conjunto está vazio.
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			if(_list == null)
				return (_expressions == null);
			return (_list.Count == 0);
		}

		/// <summary>
		/// Recupera a intercessão com o conjunto informado.
		/// </summary>
		/// <param name="ses"></param>
		/// <returns></returns>
		public StringExpressionSet Intersect(StringExpressionSet ses)
		{
			if((this.IsEmpty() || (ses == null)) || ses.IsEmpty())
				return this.CreateNewEmpty();
			this.CheckList();
			ses.CheckList();
			StringExpressionSet set = this.CreateNewEmpty();
			for(int i = 0; i < _list.Count; i++)
			{
				for(int j = 0; j < ses._list.Count; j++)
				{
					if(this.StringSubsetString(_list[i], ses._list[j], _ignoreCase))
					{
						if(set._list == null)
							set._list = new List<string>();
						set.AddSingleExpressionNoDuplicates(_list[i]);
					}
					else if(this.StringSubsetString(ses._list[j], _list[i], _ignoreCase))
					{
						if(set._list == null)
							set._list = new List<string>();
						set.AddSingleExpressionNoDuplicates((string)ses._list[j]);
					}
				}
			}
			set.GenerateString();
			return set;
		}

		/// <summary>
		/// Realiza a união com o conjunto informado.
		/// </summary>
		/// <param name="ses"></param>
		/// <returns></returns>
		public StringExpressionSet Union(StringExpressionSet ses)
		{
			if((ses == null) || ses.IsEmpty())
				return this.Copy();
			if(this.IsEmpty())
				return ses.Copy();
			this.CheckList();
			ses.CheckList();
			StringExpressionSet set = (ses._list.Count > _list.Count) ? ses : this;
			StringExpressionSet set2 = (ses._list.Count <= _list.Count) ? ses : this;
			StringExpressionSet set3 = set.Copy();
			set3.Reduce();
			for(int i = 0; i < set2._list.Count; i++)
				set3.AddSingleExpressionNoDuplicates(set2._list[i]);
			set3.GenerateString();
			return set3;
		}

		/// <summary>
		/// Verifica se o conjunto informado é subconjunto do atual.
		/// </summary>
		/// <param name="ses">Conjunto que será verificado.</param>
		/// <returns></returns>
		public bool IsSubsetOf(StringExpressionSet ses)
		{
			if(!IsEmpty())
			{
				if((ses == null) || ses.IsEmpty())
					return false;
				this.CheckList();
				ses.CheckList();
				for(int i = 0; i < _list.Count; i++)
				{
					if(!this.StringSubsetStringExpression(this._list[i], ses, this._ignoreCase))
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Verifica se o conjunto informado é subconjunto do caminho descoberto.
		/// </summary>
		/// <param name="ses"></param>
		/// <returns></returns>
		public bool IsSubsetOfPathDiscovery(StringExpressionSet ses)
		{
			if(!this.IsEmpty())
			{
				if((ses == null) || ses.IsEmpty())
					return false;
				this.CheckList();
				ses.CheckList();
				for(int i = 0; i < _list.Count; i++)
				{
					if(!StringSubsetStringExpressionPathDiscovery(_list[i], ses, _ignoreCase))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Define se é para dispara a excessão caso o caminho seja relativo.
		/// </summary>
		/// <param name="throwOnRelative"></param>
		public void SetThrowOnRelative(bool throwOnRelative)
		{
			_throwOnRelative = throwOnRelative;
		}

		public override string ToString()
		{
			this.CheckList();
			this.Reduce();
			this.GenerateString();
			return _expressions;
		}

		public string[] ToStringArray()
		{
			if((_expressionsArray == null) && (_list != null))
				_expressionsArray = _list.ToArray();
			return _expressionsArray;
		}
	}
}
