using System;
using System.Collections.Generic;
using System.IO;
namespace Godot;
public class Parser
	{

		public bool import = true;
		public List<Line> lines = new List<Line>();
		private Line currentLine;
		public static Stack<System.Drawing.Color> colorsList = new Stack<System.Drawing.Color>();
		private Token currentToken;
		private int position;
		public int lineNumber { get; set; }
		public static Context GlobalContext=new Context( new Dictionary<Identifiers,Expressions>(),null,new List<Function>(), new List<MultipleIdentifiers>());
		
		public static string contextOfFunction;
		
		public static bool correct = true;
		public static  bool comingFromFunctionParam=false;
		public static bool comingFromLet=false;
		private static IdentifierExpression comingFromIdentifier;
		public static List<string> importNames = new List<string>();

		public Parser(string code)
		{
			Lexer lexer= new Lexer(code);
			var l=lexer.Scanner();
			lineNumber = 0;
			position=0;
			if(l==null) 
			{
				lines=new List<Line>();
				currentLine = new Line(new Token[1],0);
				currentToken = new Token("",0,0,TokenType.BadToken);
				return;
			}
			if(l.Count==0)  return;            
			lines=l;
			currentLine = lines[0];
			currentToken = currentLine.tokens[0];            
			
			
		}
		private bool IsDeclaration(int position)
		{
			for (int i = position+1; i < currentLine.tokens.Length-1; i++)  
			{
				if(currentLine.tokens[i].dato==")")
				{
					if(currentLine.tokens[i+1].dato=="=")   return true;
				}
			}
				
			return false;
		}
		private Token NextToken()
		{
			if (currentLine.tokens[position+1].type==TokenType.EndLine)
			{
				NextLine();   
				return currentToken; 
			
			}
			if (currentLine.tokens[position+1].type==TokenType.EndFile)
			{
				position++;

				currentToken=currentLine.tokens[position];
				return currentToken;
			}
			currentToken = currentLine.tokens[position + 1];
			position++;
			return currentToken;
		}
		public void NextLine()  
		{

			if (lineNumber == lines.Count - 1)
			{
				lineNumber++;
				position = 0;
				return;
			}
			lineNumber++;
			currentLine = lines[lineNumber];
			currentToken = currentLine.tokens[0];
			position = 0;

		}

		private int GetPrecendece(TokenType type)
		{
			switch (type)
			{
				case TokenType.OrOperatorToken:
					return 1;
				case TokenType.AndOperatorToken:
					return 2;
				case TokenType.EqualEqualToken:
				case TokenType.NotEqualToken:
					return 3;
				case TokenType.ComparisonToken:
					return 4;
				case TokenType.PlusToken:
				case TokenType.MinusToken:
					return 5;
				case TokenType.MultiplicationToken:
				case TokenType.DivisionToken:
				case TokenType.RestoToken:
					return 6;
				case TokenType.ExponentToken:
					return 7;

				default: return 0;

			}



		}

		public Expressions Parse()
		{
			if (currentToken.dato != "import" )
			{
				import = false;
			}
			Expressions expr = _Parse();
			if (currentToken.type != TokenType.SemiColonToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token {currentToken.dato}, Expected ';'. ", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);

			}
			return expr;
		}
	   
		private Expressions _Parse(int precedence = 0)
		{
			var left = Factor();
			if (left is ErrorExpression) return left;

			while (true)
			{
				int _precedence = GetPrecendece(currentToken.type);

				if (_precedence == 0 || _precedence <= precedence)
					break;

				Token nodo = currentToken; NextToken();
				var right = _Parse(_precedence);
				if (right is ErrorExpression e)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{e.token.dato}',expected expression after '{nodo.dato}'.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
				}
				left = new BinaryExpression(left, nodo, right);
			}

			return left;
		}

		private Expressions Factor()
		{
			Expressions factor = new NumberExpression(currentToken);
			System.Drawing.Color color= System.Drawing.Color.Black;
			if(colorsList.Count!=0) color= colorsList.Peek();

			if (currentToken.type == TokenType.NumberToken)
			{
				NextToken();
				return factor;
			}

			if (currentToken.type == TokenType.StringToken)
			{
				var x=new StringExpression(currentToken);
				NextToken();
				return x;
			}

			if (currentToken.type == TokenType.IdentifierToken)
			{
				IdentifierExpression iden = new IdentifierExpression(currentToken);
				if (currentLine.tokens[position + 1].type == TokenType.OpenParenthesesToken)
				{
					if(DeclaredFunction(iden,GlobalContext,true) || contextOfFunction==iden.Identifier.dato)   currentToken.type = TokenType.FunctionCall;
					else
					{
						if(IsDeclaration(position))
						{
							var x=FunctionDeclaration();    if(x==null) return factor;
							return x;
						}
						Errors Error = new Errors($"! SINTAX ERROR: The function '{currentToken.dato}' is not defined.", currentToken.line, currentToken.col);
						Lexer.error_list.Add(Error);
						return factor;
					}             
				} 
			  
				else       
				{    
					if(NextToken().type==TokenType.ColonToken && !comingFromFunctionParam)
					{
						if(DeclaredVariable(iden,GlobalContext,false))  return factor;
						List<IdentifierExpression> identifierList = new List<IdentifierExpression>();
						identifierList.Add(iden);
						NextToken();

						var mI= MultiIdentifierDeclaration(identifierList);
						if(mI==null)    return factor;
						if (currentToken.type != TokenType.AssignmentToken)
						{
							Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected '=' .", currentToken.line, currentToken.col);
							Lexer.error_list.Add(Error);
							return null;
						}
						NextToken();
						var sequence =_Parse();
						mI.sequence=sequence;
						GlobalContext.Variables.Add(mI,sequence);
						return mI;
					}
					
					if(currentToken.type==TokenType.AssignmentToken)
					{
						if(contextOfFunction!=null && !comingFromLet)
						{
							Errors Err = new Errors($"! SINTAX ERRORR: Cannot declare constants in a function .", iden.Identifier.line, iden.Identifier.col);
							Lexer.error_list.Add(Err);
							return factor;
						}
						if(DeclaredVariable(iden,GlobalContext,false)) return factor;
						NextToken();
						comingFromIdentifier=iden;
						Expressions value= _Parse();
						AddToContext(iden,value);
						return iden;
					} 
					
					if(!DeclaredVariable(iden,GlobalContext,true))
					{
						Errors Err = new Errors($"! SINTAX ERRORR: The name '{iden.Identifier.dato}' does not exist in the current context.", iden.Identifier.line, iden.Identifier.col);
						Lexer.error_list.Add(Err);
						return factor;

					}
					return iden;
				}
			}
		   
			if (currentToken.type == TokenType.MinusToken || currentToken.type == TokenType.PlusToken || currentToken.type == TokenType.NotOperatorToken)
			{
				Token nodo = currentToken;
				NextToken();
				var right = Factor();
				return new UnaryExpression(nodo, right);
			}

			if (currentToken.type == TokenType.OpenParenthesesToken)
			{
				NextToken();
				factor = _Parse();
				if (currentToken.type != TokenType.CloseParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected ')'.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				NextToken();
				return factor;
			}

			if (currentToken.type == TokenType.OpenKeyToken)
			{
				List<Expressions> elements = new List<Expressions>();
				NextToken();
				if(currentToken.type==TokenType.CloseKeyToken)
				{
					NextToken();
					return new Sequence(elements);
				}
				
				comingFromFunctionParam=true;
				Expressions expr = _Parse();
				elements.Add(expr);  
				if(currentToken.type==TokenType.PuntoSequenceToken)
				{
					if(NextToken().type==TokenType.CloseKeyToken)
					{
						NextToken();
						comingFromFunctionParam=false;
						return   new InfineSequence(elements,false);
					}
					Expressions exp2= _Parse();
					elements.Add(exp2);
					if (currentToken.type != TokenType.CloseKeyToken)
					{
						Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'CloseKey Token'.", currentToken.line, currentToken.col);
						Lexer.error_list.Add(Error);
						return factor;
					}
					NextToken();
					comingFromFunctionParam=false;
					return new InfineSequence(elements,true);
				}     
				if(currentToken.type==TokenType.CloseKeyToken)
				{
					NextToken();
					comingFromFunctionParam=false;
					return new Sequence(elements);
				}
				NextToken();
				while (true)
				{
					Expressions exp=_Parse();
					elements.Add(exp);   
					if (currentToken.type != TokenType.ColonToken)
						break;
					NextToken();
				}
				if (currentToken.type != TokenType.CloseKeyToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'CloseKey Token'.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				NextToken();
				comingFromFunctionParam=false;
				return new Sequence(elements);

			}

			if (currentToken.type == TokenType.FunctionCall)
			{
				comingFromFunctionParam=true;
				List<Expressions> argsList = new List<Expressions>();
				Token FName = currentToken;
				if (NextToken().type != TokenType.OpenParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected '('.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					correct = false;
					return factor;
				}
				NextToken();
				while (true)                                                        // busco los argumento
				{
					if (currentToken.type == TokenType.CloseParenthesesToken) break;
					Expressions arg = _Parse();
					argsList.Add(arg);
					//NextToken();
					if (currentToken.type != TokenType.ColonToken) break;
					NextToken();
				}

				if (currentToken.type != TokenType.CloseParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected ')'.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					correct = false;
					return factor;
				}

				NextToken(); 
				factor = new FunctionCall(FName, argsList);
				comingFromFunctionParam=false;
				return factor;
			}

			if (currentToken.type == TokenType.ReservedWordToken)
			{
				factor = ReserverWord(currentToken);
				return factor;
			}

			if (currentToken.type == TokenType.ReservedFunctionToken)
			{
				factor = ReserverdFunction(currentToken);
				return factor;
			}

			if (currentToken.type == TokenType.AssignmentToken)
			{
				Errors E = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}' .", currentToken.line, currentToken.col);
				Lexer.error_list.Add(E);
			}

			if (currentToken.type == TokenType.Point)        
			{
				Token token=currentToken;
				NextToken();
				if (currentToken.type != TokenType.IdentifierToken)
				{
					comingFromFunctionParam=true;
					if(currentToken.type==TokenType.OpenParenthesesToken)
					{
						if(comingFromIdentifier==null&& !comingFromFunctionParam)
						{
							Errors Err = new Errors($"! SINTAX ERROR: Unexpected Tokensss '{currentToken.dato}'.", currentToken.line, currentToken.col);
							Lexer.error_list.Add(Err);
							return factor;

						}
						NextToken();         
						Expressions x=_Parse();
						if(currentToken.type!=TokenType.ColonToken)
						{
							Errors Err = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
							Lexer.error_list.Add(Err);
							return factor;
						   
						}
						NextToken();
						Expressions y=_Parse();
						if (currentToken.type != TokenType.CloseParenthesesToken)
						{
							Errors Err = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Close Parenthesis token expected.", currentToken.line, currentToken.col);
							Lexer.error_list.Add(Err);
							return factor;
						}

						PointExpression point=new PointExpression(x,y,comingFromIdentifier,color);
						if(colorsList.Count!=0)  point.color=colorsList.Peek();
						comingFromFunctionParam=false;
						comingFromIdentifier=null;
						NextToken();
						return point;

					}
					
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				
				IdentifierExpression id = new IdentifierExpression(currentToken);
				if(DeclaredVariable(id,GlobalContext,false)) return factor;

				PointExpression p = GeneratePoint();
				p.Id=id;
				 if(colorsList.Count!=0)  p.color=colorsList.Peek();
				AddToContext(id,p);

				NextToken();
				return p;
			}

		   if (currentToken.type == TokenType.Line || currentToken.type == TokenType.Segment || currentToken.type == TokenType.Ray)    
			{
				var auxToken = currentToken;


				NextToken();

				if (currentToken.type == TokenType.IdentifierToken)
				{
					IdentifierExpression id = new IdentifierExpression(currentToken);
					if (DeclaredVariable(id,GlobalContext,false)) return factor;

					PointExpression point1 = GeneratePoint();
					PointExpression point2 = GeneratePoint();

					if (auxToken.type == TokenType.Line)
					{
						LineExpression lineExp = new LineExpression(point1, point2,color);
						lineExp.Id=id;
						if(colorsList.Count!=0)  lineExp.color=colorsList.Peek();
						AddToContext(id,lineExp);
						NextToken();
						return lineExp;
					}
					else if (auxToken.type == TokenType.Segment)
					{
						SegmentExpression segmentExp = new SegmentExpression(point1, point2,color);
						segmentExp.Id=id;
						if(colorsList.Count!=0)  segmentExp.color=colorsList.Peek();
						AddToContext(id,segmentExp);
						NextToken();
						return segmentExp;
					}
					else
					{
						RayExpression rayExp = new RayExpression(point1, point2,color);
						rayExp.Id=id;
						if(colorsList.Count!=0)  rayExp.color=colorsList.Peek();
						AddToContext(id,rayExp);
						NextToken();
						return rayExp;
					}
				}
				comingFromFunctionParam=true;
				if (currentToken.type != TokenType.OpenParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Open Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				NextToken();
				Expressions p1 = _Parse();

				if (currentToken.type != TokenType.ColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				NextToken();
				Expressions p2 = _Parse();

				if (currentToken.type != TokenType.CloseParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Close Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				comingFromFunctionParam = false;

				if (auxToken.type == TokenType.Line)
				{
					LineExpression lineExp = new LineExpression(p1, p2,color);

					NextToken();
					return lineExp;
				}
				else if(auxToken.type == TokenType.Segment)
				{
					SegmentExpression segmentExp = new SegmentExpression(p1, p2,color);

					NextToken();
					return segmentExp;
				}
				else
				{
					RayExpression rayExp = new RayExpression(p1, p2,color);

					NextToken();
					return rayExp;
				}
			}

			if (currentToken.type == TokenType.Circunference)
			{
				NextToken();

				if (currentToken.type == TokenType.IdentifierToken)
				{
					IdentifierExpression id = new IdentifierExpression(currentToken);
					if (DeclaredVariable(id,GlobalContext,false)) return factor;

					PointExpression point = GeneratePoint();
					Random rand = new Random(Guid.NewGuid().GetHashCode());

					double ratio = rand.NextDouble()*200;
					CircunferenceExpression auxCircle = new CircunferenceExpression(point, new NumberExpression(new Token(ratio.ToString(), 0, 0, TokenType.NumberToken)),color);  
					auxCircle.Id=id;
					if(DeclaredVariable(id,GlobalContext,false)) return factor;
					if(colorsList.Count!=0)  auxCircle.color=colorsList.Peek();
					AddToContext(id,auxCircle);

					NextToken();
					return auxCircle;
				}

				comingFromFunctionParam = true;
				if (currentToken.type != TokenType.OpenParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Open Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				NextToken();
				Expressions c = _Parse();

				if (currentToken.type != TokenType.ColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				NextToken();
				Expressions r = _Parse();

				if (currentToken.type != TokenType.CloseParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Close Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				CircunferenceExpression circle = new CircunferenceExpression(c, r,color);
				NextToken();
				comingFromFunctionParam = false;
				return circle;

			}
			
			if(currentToken.type == TokenType.Measure)
			{
				NextToken();

				comingFromFunctionParam = true;

				if (currentToken.type != TokenType.OpenParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Open Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				NextToken();
				Expressions point1 = _Parse();

				if (currentToken.type != TokenType.ColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				NextToken();
				Expressions point2 = _Parse();

				if (currentToken.type != TokenType.CloseParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Close Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				MeasureExpression measure = new MeasureExpression(point1, point2);
				NextToken();
				comingFromFunctionParam = false;
				return measure;

			}

			if (currentToken.type == TokenType.Arc)
			{
				NextToken();

				if (currentToken.type == TokenType.IdentifierToken)
				{
					IdentifierExpression id = new IdentifierExpression(currentToken);
					if (DeclaredVariable(id,GlobalContext,false)) return factor;

					PointExpression auxPoint1 = GeneratePoint();
					PointExpression auxPoint2 = GeneratePoint();
					PointExpression auxPoint3 = GeneratePoint();

					Random rand = new Random(Guid.NewGuid().GetHashCode());

					double ratio = rand.NextDouble()*200;

					ArcExpression auxArc = new ArcExpression(auxPoint1, auxPoint2, auxPoint3, new NumberExpression(new Token(ratio.ToString(), 0, 0, TokenType.NumberToken)),color);
					auxArc.Id=id;
					if(!comingFromLet)  AddToContext(id, auxArc);//////////////

					NextToken();
					return auxArc;
				}

				comingFromFunctionParam = true;

				if (currentToken.type != TokenType.OpenParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Open Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				NextToken();
				Expressions center = _Parse();

				if (currentToken.type != TokenType.ColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				NextToken();
				Expressions point1 = _Parse();

				if (currentToken.type != TokenType.ColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				NextToken();
				Expressions point2 = _Parse();

				if (currentToken.type != TokenType.ColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Colon token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				NextToken();
				Expressions measure = _Parse();

				if (currentToken.type != TokenType.CloseParenthesesToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', Close Parenthesis token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}

				ArcExpression arc = new ArcExpression(center, point1, point2, measure,color);

				NextToken();
				comingFromFunctionParam = false;
				return arc;

			}

			if (currentToken.type == TokenType.Color)
			{
				NextToken();
				if (currentToken.type != TokenType.IdentifierToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', color identifier token expected.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return factor;
				}
				System.Drawing.Color Color1 = new System.Drawing.Color();
				switch (currentToken.dato)
				{
					case "blue":
						Color1 = System.Drawing.Color.Blue;
						break;
					case "red":
						Color1 = System.Drawing.Color.Red;
						break;
					case "yellow":
						Color1 = System.Drawing.Color.Yellow;
						break;

					case "green":
						Color1 = System.Drawing.Color.Green;
						break;

					case "cyan":
						Color1 = System.Drawing.Color.Cyan;
						break;

					case "magenta":
						Color1 = System.Drawing.Color.Magenta;
						break;

					case "white":
						Color1 = System.Drawing.Color.White;
						break;

					case "gray":
						Color1 = System.Drawing.Color.Gray;
						break;

					case "black":
						Color1 = System.Drawing.Color.Black;
						break;

					default:;
						Errors Error = new Errors($"! SINTAX ERROR: Unexpected Color identifier '{currentToken.dato}'.", currentToken.line, currentToken.col);
						Lexer.error_list.Add(Error);
						return factor;
				}
				NextToken();
				var c =new ColorExpression(Color1);
				colorsList.Push(c.color);
				return c;
			}
		   
			if (currentToken.type == TokenType.Restore)
			{
				NextToken();
				return new RestoreExpression();
			}

			if (currentToken.type == TokenType.AndOperatorToken || currentToken.type == TokenType.OrOperatorToken || currentToken.type == TokenType.ComparisonToken || currentToken.type == TokenType.EqualEqualToken || currentToken.type == TokenType.NotEqualToken
			   || currentToken.type == TokenType.DivisionToken || currentToken.type == TokenType.ExponentToken || currentToken.type == TokenType.MultiplicationToken || currentToken.type == TokenType.RestoToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Expected expression before '{currentToken.dato}'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				factor = new ErrorExpression(currentToken.line, currentToken);

			}
			else
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Tokennn '{currentToken.dato}'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				factor = new ErrorExpression(currentToken.line, currentToken);
			}

			return factor;
		}

		private MultipleIdentifiers MultiIdentifierDeclaration(List<IdentifierExpression> identifierList) 
		{
			
			while (currentToken.type == TokenType.IdentifierToken)
			{
				IdentifierExpression idn = new IdentifierExpression(currentToken);
				if (DeclaredVariable(idn,GlobalContext,false))    return null;
				identifierList.Add(idn);
				if (NextToken().type != TokenType.ColonToken)
				{
					break;
				}
				NextToken();
			}
			MultipleIdentifiers mI = new MultipleIdentifiers(identifierList);
			return mI;
		}

		private Expressions ReserverWord(Token Token)
		{
			switch (Token.dato)
			{
				case "PI":
					{
						NextToken();
						Token.dato = Math.PI.ToString();
						Token.type = TokenType.NumberToken;
						return new NumberExpression(Token);
					}
				case "E":
					{
						NextToken();
						Token.dato = Math.E.ToString();
						Token.type = TokenType.NumberToken;
						return new NumberExpression(Token);
					}

				case "if":
					{
						return IfFunction(Token);
					}
				case "let":
					{
						var r= LetFunction();
						if(r==null) return new ErrorExpression(Token.line, Token);
						return r;
					}
				case "import":
					{
						if (import)
						{
							return Import();
						}
						Errors Err = new Errors($"! SINTAX ERROR: Unexpected Token '{Token.dato}', to import files is only allowed at the beginning of the current file.", currentToken.line, currentToken.col);
						Lexer.error_list.Add(Err);
						return new ErrorExpression(Token.line, Token);
					}

				default:
					NextToken();
					Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{Token.dato}'.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return new ErrorExpression(Token.line, Token);

			}
		}
		private Expressions Import()
		{
			NextToken();

			if (currentToken.type != TokenType.StringToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected string token.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return new ErrorExpression(currentToken.line, currentToken);
			}


			string name = currentToken.dato;

			if(importNames.Contains(name))
			{
				Errors Error = new Errors($"! SINTAX ERROR: {name} file is already imported'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return new ErrorExpression(currentToken.line, currentToken);
			}

			if (!File.Exists("../ImportLibrary/" + name + ".txt"))
			{
				Errors Error = new Errors($"! SINTAX ERROR: {name} file was not found'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return new ErrorExpression(currentToken.line, currentToken);
			}
			
			importNames.Add(name);

			StreamReader reader = new StreamReader("../ImportLibrary/"+name+".txt");
			string text = reader.ReadToEnd();
			Parser parser = new Parser(text);
			List<Expressions> list = new List<Expressions>();

			while (parser.lineNumber < parser.lines.Count)
			{
				var tree = parser.Parse();
				if (tree is Function || tree is Sequence || tree is IdentifierExpression)
				{
					list.Add(tree);
				}
				parser.NextLine();
			}
			NextToken();
			return new ImportExpression(list);
		}

		private Expressions IfFunction(Token aux)
		{
			List<Expressions> childs = new List<Expressions>();
			Expressions result = new NumberExpression(aux);
			NextToken();

			if (currentToken.dato =="then")
			{
				Errors Error = new Errors($"! SINTAX ERROR: The expression if-else must receive an argument before the expression then'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return result;
			}
			Expressions boolCondition = _Parse();
			childs.Add(boolCondition);
			if (currentToken.dato != "then")
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'then'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return result;
			}
			NextToken();
			Expressions child_1 = _Parse();

			childs.Add(child_1);

			if (currentToken.dato != "else")
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'else'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return result;
			}
			NextToken();
			Expressions child_2 = _Parse();
			childs.Add(child_2);
			result = new FunctionExpression(aux, childs);
			return result;
		}

		private Expressions ReserverdFunction(Token function)
		{
			if (function.dato == "draw")
			{
				return DrawFuntion();
			}
			comingFromFunctionParam=true;
			List<Expressions> childs = new List<Expressions>();
			NextToken();
			if (currentToken.type != TokenType.OpenParenthesesToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected '('.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
			}
			NextToken();
			if (currentToken.type == TokenType.CloseParenthesesToken)
			{
				NextToken();
				comingFromFunctionParam=false;
				return new FunctionExpression(function, childs);
			}
			while (true)                                                        // busco los argumentos
			{
				Expressions arg = _Parse();
				childs.Add(arg);
				if (currentToken.type != TokenType.ColonToken) break;
				NextToken();
			}
			if (currentToken.type != TokenType.CloseParenthesesToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
			}
			NextToken();
			comingFromFunctionParam=false;
			return new FunctionExpression(function, childs);

		}
		private Expressions DrawFuntion()
		{
			comingFromFunctionParam=true;
			Token function = currentToken;
			List<Expressions> figures = new List<Expressions>();
			NextToken();
			string text = "";

			if (currentToken.type != TokenType.OpenKeyToken)
			{
				Expressions exp = _Parse();
				if(currentToken.type == TokenType.StringToken)
				{
					text = currentToken.dato;
					NextToken();
				}

				comingFromFunctionParam = false;
				DrawFunction draw = new DrawFunction(function, figures);
				draw.text = text;
				figures.Add(exp);
				return draw;
			}
			
			Expressions expression = Factor();
			if (expression is Sequence s)
			{
				figures = s.sequence;
			}
			comingFromFunctionParam=false;
			return new DrawFunction(function, figures);
		}
		private Expressions FunctionDeclaration()  
		{        
			Token NameFunction = currentToken;
			Dictionary<Identifiers, Expressions> listVariable = new Dictionary<Identifiers, Expressions>();
			List< MultipleIdentifiers > multipleIdList = new();
			List<Function> functionsList= new List<Function>();
			Context context= new Context(listVariable,null,functionsList,multipleIdList);
			GlobalContext=new Context(listVariable,GlobalContext,functionsList,multipleIdList);
			bool ExpectedIdentifier = true;
			
			List<IdentifierExpression> arguments = new List<IdentifierExpression>();

			if (NextToken().type != TokenType.OpenParenthesesToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected '('.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			
			NextToken();
			
			if (currentToken.type == TokenType.CloseParenthesesToken)
			{
				ExpectedIdentifier = false;
			}

			else if (currentToken.type != TokenType.IdentifierToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			
			while (currentToken.type == TokenType.IdentifierToken)                // Busco los argumentos que recibe la funcion.
			{
				IdentifierExpression id = new IdentifierExpression(currentToken);
				arguments.Add(id);
				listVariable.Add(id,null);
				ExpectedIdentifier = false;
				if (NextToken().type == TokenType.ColonToken)
				{
					NextToken();
					ExpectedIdentifier = true;
				}
			}

			if (ExpectedIdentifier)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',Indentifier expected.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return null;
			}
		   
			if (currentToken.type != TokenType.CloseParenthesesToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected ')'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			
			if (NextToken().type != TokenType.AssignmentToken)
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}', expected '='.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			NextToken();
			contextOfFunction = NameFunction.dato;
			Expressions corpus = _Parse();                                   // el cuerpo de la funcion 
			Function function = new Function(NameFunction, context,arguments ,corpus);
			if (correct)
			{
				Lexer.function.Add(NameFunction.dato);
			}
			GlobalContext=GlobalContext.Father;
			GlobalContext.functionsList.Add(function);
			contextOfFunction=null;
			return function;
		}    
		
		static public PointExpression GeneratePoint()
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			double x = random.NextDouble()*780 - 390;
			System.Console.WriteLine($"x es {x}");

			Random random2 = new Random(Guid.NewGuid().GetHashCode());
			double y = random2.NextDouble()*480 - 240;
			System.Console.WriteLine($"y es {y}");

			NumberExpression X = new NumberExpression(new Token(x.ToString(), 0, 0, TokenType.NumberToken));
			NumberExpression Y = new NumberExpression(new Token(y.ToString(), 0, 0, TokenType.NumberToken));
			PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)),System.Drawing.Color.Black);
			return point;
		}

		private bool DeclaredVariable(IdentifierExpression v,Context context,bool notError)
		{

			foreach (var id in context.Variables)
			{
				if(id.Key is IdentifierExpression i)
				{
					if(i.Equals(v))
					{
						if(notError)    return true;
						Errors Error = new Errors($"! SINTAX ERRORr: The name '{v.Identifier.dato}' is already declared in this context.", v.Identifier.line, v.Identifier.col);
						Lexer.error_list.Add(Error);
						return true;
					}
				}
				if(id.Key is MultipleIdentifiers mult )
				{
					foreach (var item in mult.identifiers)
					{
						if(item.Identifier.dato=="_") continue;	
						if (item.Equals(v)) 
						{
							if(notError)    return true;
							Errors Error = new Errors($"! SINTAX ERRORr: The name '{v.Identifier.dato}' is already declared in this context.", v.Identifier.line, v.Identifier.col);
							Lexer.error_list.Add(Error);
							return true;
						}

					} 
				}
			}
			if(context.Father==null)    return false;
			return DeclaredVariable(v,context.Father,notError);
		}
		private bool DeclaredFunction(IdentifierExpression v,Context context,bool notError)
		{
			foreach (var item in context.functionsList)
			{
				if (item.NameToken.dato == v.Identifier.dato) 
				{
					if(notError)    return true;
					Errors Error = new Errors($"! SINTAX ERROR: The function '{v.Identifier.dato}' is already declared in this context.", v.Identifier.line, v.Identifier.col);
					Lexer.error_list.Add(Error);
					return true;
				}
			}
			if(context.Father==null)    return false;
			return DeclaredFunction(v,context.Father,notError);
		}
		
		private void AddToContext(IdentifierExpression id, Expressions value)
		{
			GlobalContext.Variables.Add(id,value);
		}    
		private Expressions LetFunction()
		{
			Dictionary<Identifiers, Expressions> listVariable = new Dictionary<Identifiers, Expressions>();
			List< MultipleIdentifiers > multipleIdList = new List< MultipleIdentifiers >();
			List<Function> functionsList= new List<Function>();
			List<Expressions> listReserverdFunction= new List<Expressions>();
			Context context= new Context(listVariable,null,functionsList,multipleIdList);
			GlobalContext=new Context(listVariable,GlobalContext,functionsList,multipleIdList);
			NextToken();
			comingFromLet= true;
			while(currentToken.dato!="in" && currentToken.type!=TokenType.EndFile)
			{
			   if(currentToken.type==TokenType.IdentifierToken ||currentToken.type==TokenType.Point|| currentToken.type==TokenType.Line||currentToken.type==TokenType.Segment
				||currentToken.type==TokenType.Ray || currentToken.type==TokenType.Circunference ||currentToken.type==TokenType.Arc)
				{			
					Factor();
				}
				else if(currentToken.type==TokenType.ReservedFunctionToken)
				{
					var function=ReserverdFunction(currentToken);
					listReserverdFunction.Add(function);
				}				
				if (currentToken.type != TokenType.SemiColonToken)
				{
					Errors Error = new Errors($"! SINTAX ERROR: Unexpecteddd Token '{currentToken.dato}',expected ';'.", currentToken.line, currentToken.col);
					Lexer.error_list.Add(Error);
					return null;
		
				}  
				NextToken();
			}
			if(currentToken.dato!="in")
			{
				Errors Error = new Errors($"! SINTAX ERROR: Unexpected Token '{currentToken.dato}',expected 'in'.", currentToken.line, currentToken.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			NextToken();
			comingFromLet=true;
			Expressions corpus= _Parse();
			comingFromLet=false;
			GlobalContext=GlobalContext.Father;
			return new LetExpression(corpus, context,listReserverdFunction);
		}
	
	}
