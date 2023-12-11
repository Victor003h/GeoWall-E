using System;
using System.Collections.Generic;
using System.IO;
namespace Godot;



public class Lexer
    {
        public static List<string> function = new List<string>();
        public static List<Errors> error_list = new List<Errors>();
        private static int col = 0;
        private  int line = 1;
        private int position=0;
        private string code;
        private List<Token> tokens=new List<Token>();
        


        public Lexer(string code)
        {
            this.code=code;
        }

        public  List<Line> Scanner()
        {
            List<Token> token_list = new List<Token>();
            List<Line> lines = new List<Line>();
            string aux = string.Empty;          
            while (position < code.Length)
            {
                var token=Tokenizer();
                if(token.type==TokenType.WhiteSpaceToken)   continue;
                if(token.type==TokenType.BadToken)
                {
                    error_list.Add(new Errors($"! LEXICAL ERROR: Invalid Token {token.dato} ", token.line, token.col));
                    return null;
                }
                if(token.type==TokenType.Error) return null;

                if(token.type==TokenType.NextLine2Token || token.type==TokenType.NextLineToken ) 
                {
                    if(token_list.Count!=0)
                    {   
                        if(position==code.Length-1) token_list.Add(new Token("EndFile",line,col,TokenType.EndFile));
                        else token_list.Add(new Token("EndLine",line,col,TokenType.EndLine));
                        lines.Add(new Line(token_list.ToArray(),line-1));
                        token_list.Clear();
                    } 
                    line++;
                    col=0;  
                    position+=token.dato.Length;
                    if(token.type==TokenType.NextLine2Token && Peek(1,'\n'))
                    {
                        position++;

                    }
                    continue;   
                } 
                else token_list.Add(token);
                if(position==code.Length-1 && token_list.Count!=0)
                {
                    token_list.Add(new Token("EndFile",line,col,TokenType.EndFile));
                    lines.Add(new Line(token_list.ToArray(),line));
                    line++;
                    col=0;
                }
                col+=token.dato.Length;
                position+=token.dato.Length;

            }
            return lines;
        }
        private Token Tokenizer()
        {
        
            if (code[position] == ' ')
            {
                col++;position++;  return new Token("",line,col,TokenType.WhiteSpaceToken);
            }

            if (code[position] == '\n')
            {
                return new Token("\n",line,col,TokenType.NextLineToken);
            }
            if (code[position] == '\r')
            {
                return new Token("\r",line,col,TokenType.NextLine2Token);
            }

            
            if (code[position] == '\t')
            {
                col+=4;position+=1; return new Token("",line,col,TokenType.WhiteSpaceToken);
            }
           
            if (code[position] == '"')
            {
                string dato = IsLiteral();
                if (dato != string.Empty)
                {
                    return new Token(dato, line, col, TokenType.StringToken);
                }
                return new Token("",0,0,TokenType.Error);
            }
            if (char.IsDigit(code[position]) )
            {
                double number = IsNumber(code);
                if(number==double.NaN)  return new Token(number.ToString(), line, col, TokenType.Error);
                    
                return new Token(number.ToString(), line, col, TokenType.NumberToken);
            }
            
            if (char.IsLetterOrDigit(code[position]) || code[position] == '_')
                return Word();

            if (code[position] == '+')
                return  new Token(code[position].ToString(), line, col, TokenType.PlusToken);
            if (code[position] == '^')
                return  new Token(code[position].ToString(), line, col, TokenType.ExponentToken);
            if (code[position] == '-')
                return new Token(code[position].ToString(), line, col, TokenType.MinusToken);
            if (code[position] == '*')
                return  new Token(code[position].ToString(), line, col, TokenType.MultiplicationToken);            
            if (code[position] == '/')
                return new Token(code[position].ToString(), line, col, TokenType.DivisionToken);
            if (code[position] == '%')
                return new Token(code[position].ToString(), line, col, TokenType.RestoToken);
            if (code[position] == '=')
            {
                if (Peek(1,'='))
                    return new Token("==", line, col, TokenType.EqualEqualToken);
                if (Peek(1,'>'))              
                    return new Token("=>", line, col, TokenType.AssignmentFunctionToken);
        
                return new Token(code[position].ToString(), line, col, TokenType.AssignmentToken);
            }
            if (code[position] == '(')
                return new Token(code[position].ToString(), line, col, TokenType.OpenParenthesesToken);
            if (code[position] == ')')
                return new Token(code[position].ToString(), line, col, TokenType.CloseParenthesesToken);
            if (code[position] == '{')
                return new Token(code[position].ToString(), line, col, TokenType.OpenKeyToken);
            if (code[position] == '}')
                return  new Token(code[position].ToString(), line, col, TokenType.CloseKeyToken);
            if (code[position] == '@')
                return new Token(code[position].ToString(), line, col, TokenType.ArrobaToken);
            if (code[position] == '>')
            {
                if (Peek(1,'='))
                    return new Token(">=", line, col, TokenType.ComparisonToken);
                
                return new Token(code[position].ToString(), line, col, TokenType.ComparisonToken);
            }
            if (code[position] == '<')
            {
                if (Peek(1,'='))
                    return new Token("<=", line, col, TokenType.ComparisonToken);
                
                return new Token(code[position].ToString(), line, col, TokenType.ComparisonToken);
            }
            if (code[position] == '&')
                return new Token(code[position].ToString(), line, col, TokenType.AndOperatorToken);
            if (code[position] == '|')
                return new Token(code[position].ToString(), line, col, TokenType.OrOperatorToken);
            if (code[position] == '!')
            {
                if (Peek(1,'='))
                    return new Token("!=", line, col, TokenType.NotEqualToken);
                
                return new Token(code[position].ToString(), line, col, TokenType.NotOperatorToken);
            }
            if (code[position] == ',')
                return new Token(code[position].ToString(), line, col, TokenType.ColonToken);
            if (code[position] == '.')
            {
                if(Peek(1,'.') && Peek(2,'.'))
                    return new Token("...", line, col, TokenType.PuntoSequenceToken);

                return new Token("",0,0,TokenType.Error);  
            }
            if (code[position] == ';')
                return  new Token(code[position].ToString(), line, col, TokenType.SemiColonToken);
            
            return new Token(code[position].ToString(),line,col,TokenType.BadToken);
        }
        private Token Word()
        {
            string aux=string.Empty;
            for (int i =position; i < code.Length; i++)
            {
                if(char.IsLetterOrDigit(code[i]) || code[i]=='_')   aux+=code[i];
                else    break;
            }
            return CheckWord(aux);
        }

        private bool Peek(int n,char c)
        {
            if(position+n>=code.Length) return false;
            if(code[position+n]==c) return true;
            return false;
        }
        
        private  Token CheckWord(string str)
        {
            switch (str)
            {
                case "let":
                case "in":
                case "if":
                case "then":
                case "else":
                case "function":
                case "PI":
                case "E":
                case "import":
                    return new Token(str, line, col, TokenType.ReservedWordToken);
                case "print":
                case "sin":
                case "cos":
                case "log":
                case "rand":
                case "pow":
                case "count":
                case "samples":
                case "points":
                case "intersect":
                case "draw":
                    return new Token(str, line, col, TokenType.ReservedFunctionToken);
                case "point":
                    return new Token(str, line, col, TokenType.Point);
                case "line":
                    return new Token(str, line, col, TokenType.Line);
                case "segment":
                    return new Token(str, line, col, TokenType.Segment);
                case "ray":
                    return new Token(str, line, col, TokenType.Ray);
                case "circle":
                    return new Token(str, line, col, TokenType.Circunference);
                case "color":
                    return new Token(str, line, col, TokenType.Color);
                case "restore":
                    return new Token(str, line, col, TokenType.Restore);
                case "arc":
                    return new Token(str, line, col, TokenType.Arc);
                case "measure":
                    return new Token(str, line, col, TokenType.Measure);
                case "undefined":
                    return new Token(str, line, col, TokenType.UndefinedToken);
                default:
                    return new Token(str, line, col, TokenType.IdentifierToken);
            }
        }

        private  double IsNumber(string code)
        {
            int cont = 0;
            double x;
            int i=position;
            string num="";
            while(char.IsDigit(code[i])|| code[i]=='.')
            {
                if (char.IsLetter(code[i]) || code[i]=='_')
                {
                    for (int j = i; j < code.Length; j++)
                    {
                        if (char.IsLetterOrDigit(code[j]) || code[j] == '.'||code[j] == '_')
                            num += code[j];
                        else break;
                    }
                    Errors err = new Errors($"! LEXICAL ERROR: {num} is not  valided token.", line, col);
                    error_list.Add(err);
                    return double.NaN;
                }
                if (char.IsDigit(code[i]) || code[i] == '.')
                {
                    if (code[i] == '.')
                    {
                        cont++;
                    }
                    num += code[i];
                }
                i++;
            }
            if (cont <= 1)
            {
                x = double.Parse(num);
                return x;
            }
        
            Errors error = new Errors("! LEXICAL ERROR:" + " '" + num + "' is not  valided token.", 0, 0);
            Lexer.error_list.Add(error);
            return double.NaN;
            
            
        }
        private  string IsLiteral()
        {
            string result = "";
            for (int i = position + 1; i < code.Length; i++)
            {
                if (code[i] == '"')
                {
                    position+=2;col+=2;
                    return result;
                
                }
                result += code[i];
            }
            Errors error = new Errors($"! LEXICAL ERROR: Expected end of the string.", line, col);
            error_list.Add(error);
            return string.Empty;

        }
    }

    public class Token
    {
        public string dato;
        public int line;
        public int col;

        public TokenType type;

        public Token(string dato, int line, int col, TokenType type)
        {
            this.dato = dato;
            this.line = line;
            this.col = col;
            this.type = type;
        }
    }
    public class Line
    {
        public Token[] tokens;
        public int line;

        public Line(Token[] tokens, int line)
        {
            this.line = line;
            this.tokens = tokens;

        }



    }
    public class Errors
    {
        public string error_type { get; }
        public int col { get; }
        public int line { get; }
        public Errors(string error_type, int line, int col)
        {
            this.error_type = error_type;
            this.col = col;
            this.line = line;
        }
        
    }
    