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
using System.IO;

namespace Colosoft.Text.Parser
{
	/// <summary>
	/// Possíveis identificadores de registros.
	/// </summary>
	internal enum RecordId
	{
		/// <summary>
		/// CharSets
		/// </summary>
		CharSets = 0x43,
		/// <summary>
		/// Comentários
		/// </summary>
		Comment = 0x21,
		/// <summary>
		/// 
		/// </summary>
		DFAStates = 0x44,
		/// <summary>
		/// 
		/// </summary>
		Initial = 0x49,
		/// <summary>
		/// 
		/// </summary>
		LRTables = 0x4c,
		/// <summary>
		/// 
		/// </summary>
		Parameters = 80,
		/// <summary>
		/// 
		/// </summary>
		Rules = 0x52,
		/// <summary>
		/// 
		/// </summary>
		Symbols = 0x53,
		/// <summary>
		/// 
		/// </summary>
		TableCounts = 0x54
	}
	internal enum ParseResult
	{
		Accept = 0x12d,
		InternalError = 0x196,
		ReduceEliminated = 0x130,
		ReduceNormal = 0x12f,
		Shift = 0x12e,
		SyntaxError = 0x131
	}
	/// <summary>
	/// Possíveis mensagens do parser.
	/// </summary>
	public enum ParseMessage
	{
		/// <summary>
		/// Token lido.
		/// </summary>
		TokenRead,
		/// <summary>
		/// Redução.
		/// </summary>
		Reduction,
		/// <summary>
		/// Aceito.
		/// </summary>
		Accept,
		/// <summary>
		/// Erro lexo.
		/// </summary>
		LexicalError,
		/// <summary>
		/// Erro sitaxo.
		/// </summary>
		SyntaxError,
		/// <summary>
		/// Erro de comentário.
		/// </summary>
		CommentError,
		/// <summary>
		/// Erro interno.
		/// </summary>
		InternalError
	}
	/// <summary>
	/// Classe que representa o parser da gramática.
	/// </summary>
	public class GoldParser : IDisposable
	{
		private bool _caseSensitive;

		private string[] _charsets;

		private int _commentLevel;

		private FAState[] _DfaStates;

		private Symbol _endSymbol;

		private Symbol _errorSymbol;

		private bool _haveReduction;

		private int _initDfaState;

		private bool _initialized;

		private int _initLalrState;

		private TokenStack _inputTokens;

		private int _LalrState;

		private LRActionTable[] _LalrTables;

		private int _lineNumber;

		private TokenStack _outputTokens;

		private Hashtable _parameters;

		private Rule[] _rules;

		private LookAheadReader _source;

		private int _startSymbol;

		private Symbol[] _symbols;

		private TokenStack _tempStack;

		private bool _trimReductions;

		/// <summary>
		/// Estado inicial do LALR.
		/// </summary>
		public int InitialLalrState
		{
			get
			{
				return _initLalrState;
			}
		}

		/// <summary>
		/// Número da atual linha em foco.
		/// </summary>
		public int CurrentLineNumber
		{
			get
			{
				return _lineNumber;
			}
		}

		/// <summary>
		/// Instancia da atual redução.
		/// </summary>
		public Reduction CurrentReduction
		{
			get
			{
				if(_haveReduction)
					return _tempStack.PeekToken().Data as Reduction;
				return null;
			}
			set
			{
				if(_haveReduction)
					_tempStack.PeekToken().Data = value;
			}
		}

		/// <summary>
		/// Instanc do atual token.
		/// </summary>
		public Token CurrentToken
		{
			get
			{
				return _inputTokens.PeekToken();
			}
		}

		/// <summary>
		/// Recupera o ultimo token.
		/// </summary>
		public Token LastToken
		{
			get
			{
				return _tempStack.PeekToken();
			}
		}

		/// <summary>
		/// Identifica se suporta trim reductions.
		/// </summary>
		public bool TrimReductions
		{
			get
			{
				return _trimReductions;
			}
			set
			{
				_trimReductions = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GoldParser()
		{
		}

		/// <summary>
		/// Cria uma nova instancia com a stream da gramática.
		/// </summary>
		/// <param name="stream"></param>
		public GoldParser(Stream stream)
		{
			LoadGrammar(stream);
		}

		/// <summary>
		/// Cria uma nova instancia com o arquivo da gramática.
		/// </summary>
		/// <param name="filename"></param>
		public GoldParser(string filename)
		{
			LoadGrammar(filename);
		}

		/// <summary>
		/// Reduz a regra.
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		private ParseResult Reduce(Rule rule)
		{
			ParseResult reduceEliminated;
			Token token;
			if(this._trimReductions && rule.ContainsOneNonTerminal)
			{
				token = _tempStack.PopToken();
				token.SetParent(rule.RuleNonTerminal);
				reduceEliminated = ParseResult.ReduceEliminated;
			}
			else
			{
				Reduction reduction = new Reduction();
				reduction.ParentRule = rule;
				_tempStack.PopTokensInto(reduction, rule.SymbolCount);
				token = new Token();
				token.Data = reduction;
				token.SetParent(rule.RuleNonTerminal);
				this._haveReduction = true;
				reduceEliminated = ParseResult.ReduceNormal;
			}
			int state = _tempStack.PeekToken().State;
			LRAction actionForSymbol = _LalrTables[state].GetActionForSymbol(rule.RuleNonTerminal.TableIndex);
			if(actionForSymbol == null)
			{
				throw new ParserException("Action for LALR state is null");
			}
			token.State = _LalrState = actionForSymbol.Value;
			this._tempStack.PushToken(token);
			return reduceEliminated;
		}

		/// <summary>
		/// Reseta os dados da instancia.
		/// </summary>
		private void Reset()
		{
			foreach (Symbol symbol in _symbols)
			{
				if(symbol.Kind == SymbolType.Error)
					_errorSymbol = symbol;
				else if(symbol.Kind == SymbolType.End)
					_endSymbol = symbol;
			}
			_haveReduction = false;
			_LalrState = this._initLalrState;
			_lineNumber = 1;
			_commentLevel = 0;
			_inputTokens.Clear();
			_outputTokens.Clear();
			_tempStack.Clear();
		}

		/// <summary>
		/// Recupera um token.
		/// </summary>
		/// <returns></returns>
		private Token RetrieveToken()
		{
			Token result;
			int peekChar = -1;
			int currentPos = 0;
			int lastAcceptState = -1;
			int lastAcceptPos = -1;
			FAState currentState = _DfaStates[_initDfaState];
			while (true)
			{
				int target = -1;
				peekChar = _source.LookAhead(currentPos);
				if(peekChar == -1)
				{
					break;
				}
				char ch = FixCase((char)peekChar);
				foreach (FAEdge edge in currentState.Edges)
				{
					String chars = edge.Characters;
					if(chars.IndexOf(ch) != -1)
					{
						target = edge.TargetIndex;
						break;
					}
				}
				if(target != -1)
				{
					if(_DfaStates[target].AcceptSymbol != -1)
					{
						lastAcceptState = target;
						lastAcceptPos = currentPos;
					}
					currentState = _DfaStates[target];
					currentPos++;
				}
				else
				{
					break;
				}
			}
			if(lastAcceptState != -1)
			{
				Symbol symbol = _symbols[_DfaStates[lastAcceptState].AcceptSymbol];
				result = new Token(symbol);
				result.Data = _source.Read(lastAcceptPos + 1);
			}
			else
			{
				if(peekChar == -1)
				{
					result = new Token(_endSymbol);
					result.Data = "";
				}
				else
				{
					result = new Token(_errorSymbol);
					result.Data = _source.Read(1);
				}
			}
			UpdateLineNumber((String)result.Data);
			return result;
		}

		/// <summary>
		/// Atualiza a linha.
		/// </summary>
		/// <param name="text"></param>
		private void UpdateLineNumber(string text)
		{
			int num;
			int startIndex = 0;
			while ((num = text.IndexOf('\n', startIndex)) != -1)
			{
				startIndex = num + 1;
				this._lineNumber++;
			}
		}

		/// <summary>
		/// Adiciona um charset para o parser.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="charset"></param>
		private void AddCharset(int index, string charset)
		{
			if(!_initialized)
				throw new ParserException("Table sizes not initialized");
			_charsets[index] = FixCase(charset);
		}

		/// <summary>
		/// Adiciona um estado.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="fastate"></param>
		private void AddDfaState(int index, FAState fastate)
		{
			if(!_initialized)
				throw new ParserException("Table sizes not initialized");
			_DfaStates[index] = fastate;
		}

		/// <summary>
		/// Adiciona uma tabela de ação.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="table"></param>
		private void AddLalrTable(int index, LRActionTable table)
		{
			if(!_initialized)
				throw new ParserException("Table counts not initialized");
			_LalrTables[index] = table;
		}

		/// <summary>
		/// Adiciona uma nova regra.
		/// </summary>
		/// <param name="rule">Instancia da regra que será adicionada.</param>
		private void AddRule(Rule rule)
		{
			if(!_initialized)
				throw new ParserException("Table sizes not initialized");
			int tableIndex = rule.TableIndex;
			_rules[tableIndex] = rule;
		}

		/// <summary>
		/// Adiciona um novo símbolo.
		/// </summary>
		/// <param name="symbol"></param>
		private void AddSymbol(Symbol symbol)
		{
			if(!_initialized)
				throw new ParserException("Table sizes not initialized");
			int tableIndex = symbol.TableIndex;
			_symbols[tableIndex] = symbol;
		}

		/// <summary>
		/// Verifica se o token informado é um token de comentário.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private bool CommentToken(Token token)
		{
			if(token.Kind != SymbolType.CommentLine)
				return (token.Kind == SymbolType.CommentStart);
			return true;
		}

		/// <summary>
		/// Discarta uma linha.
		/// </summary>
		private void DiscardLine()
		{
			_source.DiscardLine();
			_lineNumber++;
		}

		/// <summary>
		/// Fixa a letra para a forma de comparação.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		private char FixCase(char character)
		{
			if(_caseSensitive)
				return character;
			return char.ToLower(character);
		}

		/// <summary>
		/// Fixa o texto para a forma de comparação.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private string FixCase(string text)
		{
			if(_caseSensitive)
				return text;
			return text.ToLower();
		}

		/// <summary>
		/// Fecha o arquivo do origem.
		/// </summary>
		public void CloseFile()
		{
			if(_source != null)
				_source.Close();
			_source = null;
		}

		/// <summary>
		/// Recupera o valor do parametro pelo nome informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns></returns>
		public string GetParameter(string name)
		{
			string str = (string)_parameters[name];
			if(str == null)
				return "";
			return str;
		}

		/// <summary>
		/// Recupera a pilha dos tokens de saída.
		/// </summary>
		/// <returns></returns>
		public TokenStack GetTokens()
		{
			return _outputTokens;
		}

		/// <summary>
		/// Carrega a gramática que será utilizada pela instancia.
		/// </summary>
		/// <param name="stream">Stream com os dados da gramática</param>
		public void LoadGrammar(Stream stream)
		{
			_parameters = new Hashtable();
			_inputTokens = new TokenStack();
			_outputTokens = new TokenStack();
			_tempStack = new TokenStack();
			_initialized = false;
			_trimReductions = false;
			LoadTables(new GrammarReader(stream));
		}

		/// <summary>
		/// Carrega a gramática que será utilizada pela instancia.
		/// </summary>
		/// <param name="filename">Nome do arquivo onde estão salvos os dados da gramática.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void LoadGrammar(string filename)
		{
			_parameters = new Hashtable();
			_inputTokens = new TokenStack();
			_outputTokens = new TokenStack();
			_tempStack = new TokenStack();
			_initialized = false;
			_trimReductions = false;
			LoadTables(new GrammarReader(filename));
		}

		/// <summary>
		/// Carrega as tabelas da gramática.
		/// </summary>
		/// <param name="reader"></param>
		private void LoadTables(GrammarReader reader)
		{
			object obj;
			int index;
			while (reader.MoveNext())
			{
				byte id = (byte)reader.RetrieveNext();
				switch((RecordId)id)
				{
				case RecordId.Parameters:
					_parameters["Name"] = (string)reader.RetrieveNext();
					_parameters["Version"] = (string)reader.RetrieveNext();
					_parameters["Author"] = (string)reader.RetrieveNext();
					_parameters["About"] = (string)reader.RetrieveNext();
					_caseSensitive = (bool)reader.RetrieveNext();
					_startSymbol = (int)reader.RetrieveNext();
					break;
				case RecordId.TableCounts:
					_symbols = new Symbol[(int)reader.RetrieveNext()];
					_charsets = new String[(int)reader.RetrieveNext()];
					_rules = new Rule[(int)reader.RetrieveNext()];
					_DfaStates = new FAState[(int)reader.RetrieveNext()];
					_LalrTables = new LRActionTable[(int)reader.RetrieveNext()];
					_initialized = true;
					break;
				case RecordId.Initial:
					_initDfaState = (int)reader.RetrieveNext();
					_initLalrState = (int)reader.RetrieveNext();
					break;
				case RecordId.Symbols:
					index = (int)reader.RetrieveNext();
					var name = (string)reader.RetrieveNext();
					var kind = (SymbolType)(int)reader.RetrieveNext();
					var symbol = new Symbol(index, name, kind);
					AddSymbol(symbol);
					break;
				case RecordId.CharSets:
					index = (int)reader.RetrieveNext();
					var charset = (string)reader.RetrieveNext();
					AddCharset(index, charset);
					break;
				case RecordId.Rules:
					index = (int)reader.RetrieveNext();
					var head = _symbols[(int)reader.RetrieveNext()];
					var rule = new Rule(index, head);
					reader.RetrieveNext();
					while ((obj = reader.RetrieveNext()) != null)
						rule.AddItem(_symbols[(int)obj]);
					AddRule(rule);
					break;
				case RecordId.DFAStates:
					FAState fastate = new FAState();
					index = (int)reader.RetrieveNext();
					if((bool)reader.RetrieveNext())
						fastate.AcceptSymbol = (int)reader.RetrieveNext();
					else
						reader.RetrieveNext();
					reader.RetrieveNext();
					while (!reader.RetrieveDone())
					{
						var ci = (int)reader.RetrieveNext();
						var ti = (int)reader.RetrieveNext();
						reader.RetrieveNext();
						fastate.AddEdge(_charsets[ci], ti);
					}
					AddDfaState(index, fastate);
					break;
				case RecordId.LRTables:
					LRActionTable table = new LRActionTable();
					index = (int)reader.RetrieveNext();
					reader.RetrieveNext();
					while (!reader.RetrieveDone())
					{
						var sid = (int)reader.RetrieveNext();
						var action = (int)reader.RetrieveNext();
						var tid = (int)reader.RetrieveNext();
						reader.RetrieveNext();
						table.AddItem(_symbols[sid], (Action)action, tid);
					}
					AddLalrTable(index, table);
					break;
				case RecordId.Comment:
					Console.WriteLine("Comment record encountered");
					break;
				default:
					throw new ParserException("Wrong id for record");
				}
			}
		}

		/// <summary>
		/// Abre o arquivo onde será executa o parser.
		/// </summary>
		/// <param name="filename"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public void OpenFile(string filename)
		{
			Reset();
			_source = new LookAheadReader(new StreamReader(new FileStream(filename, FileMode.Open)));
			PrepareToParse();
		}

		/// <summary>
		/// Abre o leitor onde será executado o parser.
		/// </summary>
		/// <param name="stream"></param>
		public void OpenStream(TextReader stream)
		{
			Reset();
			_source = new LookAheadReader(stream);
			PrepareToParse();
		}

		/// <summary>
		/// Executa o parser.
		/// </summary>
		/// <returns></returns>
		public ParseMessage Parse()
		{
			while (true)
			{
				if(_inputTokens.Count == 0)
				{
					Token token = RetrieveToken();
					if(token == null)
						throw new ParserException("RetrieveToken returned null");
					if(token.Kind != SymbolType.Whitespace)
					{
						_inputTokens.PushToken(token);
						if(_commentLevel == 0 && !CommentToken(token))
							return ParseMessage.TokenRead;
					}
				}
				else if(_commentLevel > 0)
				{
					Token token = _inputTokens.PopToken();
					switch(token.Kind)
					{
					case SymbolType.CommentStart:
						_commentLevel++;
						break;
					case SymbolType.CommentEnd:
						_commentLevel--;
						break;
					case SymbolType.End:
						return ParseMessage.CommentError;
					}
				}
				else
				{
					Token token = _inputTokens.PeekToken();
					switch(token.Kind)
					{
					case SymbolType.CommentStart:
						_inputTokens.PopToken();
						_commentLevel++;
						break;
					case SymbolType.CommentLine:
						_inputTokens.PopToken();
						DiscardLine();
						break;
					default:
						ParseResult result = ParseToken(token);
						switch(result)
						{
						case ParseResult.Accept:
							return ParseMessage.Accept;
						case ParseResult.InternalError:
							return ParseMessage.InternalError;
						case ParseResult.ReduceNormal:
							return ParseMessage.Reduction;
						case ParseResult.Shift:
							_inputTokens.PopToken();
							break;
						case ParseResult.SyntaxError:
							return ParseMessage.SyntaxError;
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Executa parser para o token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private ParseResult ParseToken(Token token)
		{
			ParseResult result = ParseResult.InternalError;
			LRActionTable table = _LalrTables[_LalrState];
			LRAction action = table.GetActionForSymbol(token.TableIndex);
			if(action != null)
			{
				_haveReduction = false;
				_outputTokens.Clear();
				switch(action.Action)
				{
				case Action.Accept:
					_haveReduction = true;
					result = ParseResult.Accept;
					break;
				case Action.Shift:
					token.State = _LalrState = action.Value;
					_tempStack.PushToken(token);
					result = ParseResult.Shift;
					break;
				case Action.Reduce:
					result = Reduce(_rules[action.Value]);
					break;
				}
			}
			else
			{
				_outputTokens.Clear();
				foreach (LRAction a in table.Members)
				{
					SymbolType kind = a.Symbol.Kind;
					if(kind == SymbolType.Terminal || kind == SymbolType.End)
						_outputTokens.PushToken(new Token(a.Symbol));
				}
				result = ParseResult.SyntaxError;
			}
			return result;
		}

		/// <summary>
		/// Recupera o token do tipo do pilha.
		/// </summary>
		/// <returns></returns>
		public Token PopInputToken()
		{
			return _inputTokens.PopToken();
		}

		/// <summary>
		/// Prepara a instancia para a execução.
		/// </summary>
		private void PrepareToParse()
		{
			Token token = new Token();
			token.State = _initLalrState;
			token.SetParent(_symbols[_startSymbol]);
			_tempStack.PushToken(token);
		}

		/// <summary>
		/// Adiciona um novo token para o topo da pilha.
		/// </summary>
		/// <param name="token"></param>
		public void PushInputToken(Token token)
		{
			_inputTokens.PushToken(token);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_source != null)
			{
				_source.Dispose();
				_source = null;
			}
		}
	}
}
