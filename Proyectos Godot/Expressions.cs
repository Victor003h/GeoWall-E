using System;
using System.Collections.Generic;
using System.IO;
namespace Godot;

// a=2   x,c =  a      s=3  d,as= c x s
	public abstract class Expressions
	{
		public virtual TokenType Type { get => type; set => type = value; }
		public TokenType type;

	}
	public class UnaryExpression : Expressions

	{
		public override TokenType Type { get => TokenType.BinaryExpression; }
		private Expressions right;
		private Token nodo;

		public UnaryExpression(Token nodo, Expressions right)
		{
			this.right = right;
			this.nodo = nodo;
		}
		public Expressions Right { get => right; set => right = value; }
		public Token Nodo { get => nodo; set => nodo = value; }

	}
	public class ParenthesesExpression : Expressions
	{
		public override TokenType Type { get => TokenType.ParenthesesExpression; }
		private Expressions corpus;
		public ParenthesesExpression(Token nodo, Expressions corpus)
		{
			this.corpus = corpus;
		}
		public Expressions Corpus { get => corpus; set => corpus = value; }
	}
	public class BinaryExpression : Expressions
	{
		private Expressions left;
		private Expressions right;
		private Token nodo;

		public Expressions Left { get => left; set => left = value; }
		public Expressions Right { get => right; set => right = value; }
		public Token Nodo { get => nodo; set => nodo = value; }
		public override TokenType Type { get => TokenType.BinaryExpression; }

		public BinaryExpression(Expressions left, Token nodo, Expressions right)
		{
			this.left = left;
			this.right = right;
			this.nodo = nodo;

		}
	}

	public class ArrobaExpression : Expressions
	{

		private Expressions left;
		private Expressions right;
		private Token nodo;

		public Expressions Left { get => left; set => left = value; }
		public Expressions Right { get => right; set => right = value; }
		public Token Nodo { get => nodo; set => nodo = value; }
		public override TokenType Type { get => TokenType.BinaryExpression; }

		public ArrobaExpression(Expressions left, Token nodo, Expressions right)
		{
			this.left = left;
			this.right = right;
			this.nodo = nodo;

		}

	}
	public class NumberExpression : Expressions
	{
		public override TokenType Type { get => TokenType.NumberExpression; }
		public Token Number { get => number; set => number = value; }

		private Token number;

		public NumberExpression(Token number)
		{
			this.number = number;
		}
	}

	public class BoolExpression : Expressions
	{
	
		private Token nodo;
		public Token Nodo { get => nodo; set => nodo = value; }

		public override TokenType Type { get => TokenType.BoolExpression; }
		public BoolExpression(Token nodo)
		{    
			this.nodo = nodo;
		}
	}

	public class StringExpression : Expressions
	{
		private Token nodo;
		public StringExpression(Token nodo)
		{
			this.nodo = nodo;
		}

		public Token Nodo { get => nodo; set => nodo = value; }

		public override TokenType Type { get => TokenType.StringExpression; }

	}

	public abstract class Identifiers : Expressions
	{


	}

	public class IdentifierExpression : Identifiers
	{
		private Token identifier;

		public IdentifierExpression(Token identifier)
		{
			this.identifier = identifier;
		}

		public new TokenType type = TokenType.Identifier;
		public override TokenType Type { get => this.type; set => this.type = value; }
		public Token Identifier { get => identifier; set => identifier = value; }
		

		public override bool Equals(object obj)
		{
			if(obj is IdentifierExpression id)
				return id.Identifier.dato==identifier.dato;
			
			return false;
		}

		public override int GetHashCode()
		{
			return identifier.dato.GetHashCode();
		}
	}
	public class MultipleIdentifiers : Identifiers
	{
		public Expressions sequence;
		public List<IdentifierExpression> identifiers;
		public bool underscore;

		public MultipleIdentifiers(List<IdentifierExpression> identifiers)
		{
			this.identifiers = identifiers;
		}
	
		
	}

	public class FunctionExpression : Expressions
	{
		private Token function;

		private List<Expressions> child;

		public FunctionExpression(Token function, List<Expressions> child)
		{
			this.function = function;
			this.child = child;

		}
		public Token Function { get => function; set => function = value; }
		public List<Expressions> Child { get => child; set => child = value; }

		public override TokenType Type { get => TokenType.FunctionExpression; set => type = value; }

	}
	public class LetExpression : Expressions
	{
		private Expressions scope;
		public Context context;
		public List<Expressions> listReserverdFunction= new List<Expressions>();
	 

		public LetExpression(Expressions scope,Context context,List<Expressions> listReserverdFunction)
		{
			this.listReserverdFunction=listReserverdFunction;
			this.scope = scope;
			this.context=context;
		}

		public override TokenType Type { get => TokenType.FunctionExpression; set => type = value; }
		public Expressions Scope { get => scope; set => scope = value; }
	}

	public class Function : Expressions
	{
		public new TokenType type = TokenType.Function;
		public override TokenType Type { get => this.type; set => this.type = value; }

		private Expressions corpus;

		public List<IdentifierExpression> args;
		public Expressions Corpus { get => corpus; set => corpus = value; }

		private Token name;
		public Token NameToken { get => name; set => name = value; }


		public Context Context;
		


		public Function(Token name, Context Context,List<IdentifierExpression>  args, Expressions corpus)
		{
			this.name = name;
			this.Context = Context;
			this.corpus = corpus;
			this.args = args;
		}
	}
	public class FunctionCall : Expressions
	{
		public  Token function;
		public  List<Expressions> arg;

		public FunctionCall(Token function, List<Expressions> arg)
		{
			this.function = function;
			this.arg = arg;
		}

	}

	public class DrawFunction: Expressions
	{
		private List<Expressions> figures= new List<Expressions>();
		private Token function;
		public string text = "";

		public List<Expressions> Figures { get => figures; set => figures = value; }
		public Token Function { get => function; set => function = value; }
		
		public DrawFunction(Token function , List<Expressions> figures)
		{
			this.function= function;
			this.figures= figures;
		}
	}
	public class ErrorExpression : Expressions
	{
		public int position { get; set; }
		public Token token { get; set; }

		public ErrorExpression(int position, Token token)
		{
			this.token = token;
			this.position = position;
		}
	}

	public class Sequence:Expressions
	{
		public List<Expressions> sequence;
		public virtual  Expressions Current{get{
			return sequence[position];
		}}
		public int position=-1;
		public Sequence(List<Expressions> sequence)
		{
			this.sequence = sequence;
		}

		public virtual bool MoveNext()
		{
			if(position+1<sequence.Count)
			{
				position++;
				return true;
			}
			return false;
		}
		public virtual Sequence Take(int index)
		{
			List<Expressions> new_secuence= new List<Expressions>();
			new_secuence.Add(Current);
			while(MoveNext())
			{
				new_secuence.Add(Current);
			}
			return new Sequence(new_secuence);
		}
	}
	public class InfineSequence : Sequence
	{
		public bool hasEnd;
		internal int stard;
		internal int end;
		public override Expressions Current {get{
			return  new NumberExpression(new Token((stard+position).ToString(),0,0,TokenType.NumberExpression));
		}}

		public InfineSequence(List<Expressions> range,bool hasEnd) : base(range)
		{
		   this.hasEnd=hasEnd;
		}
		public int? GetLimit(bool first)
		{
			int index;
			if(first)   index=0;
			else    index=1;
			var x=Execute.Evaluator(sequence[index]);
			if(x.Resultado is NumberExpression n)
			{
				bool isInt=int.TryParse(n.Number.dato,out int f);
				if(isInt && f>=0)
				{
					if(first)   stard=f;
					else    end=f;
					return  f;
				}    
			}
			Errors Error = new Errors($"! SEMANTIC ERROR: Sequence range must be between positive integers.", 0, 0);
			Lexer.error_list.Add(Error);
			return null;
		}

		public bool CheckDefined()
		{
			return stard<=end;
		}	


		public override Sequence Take(int index)  
		{
			List<Expressions> range= new List<Expressions>();
			range.Add(Current);
			if(!hasEnd)				
				return new InfineSequence(range,false);
			
			while(MoveNext())
			{
				range.Add(Current);
			}
			return new Sequence(range);
		}

		public override bool MoveNext()               
		{
			if(!hasEnd)
			{
				position++;
				return  true;
			}
			if(position+1<end-stard+1)
			{
				position++;
				return true;
			}
			return false;
		}

	}
	public class ColorExpression : Expressions
	{
		public System.Drawing.Color color;

		public ColorExpression(System.Drawing.Color color)
		{
			this.color = color;
		}
	}
	public class RestoreExpression : Expressions
	{
	}
	public class MeasureExpression : Expressions
	{
		public Expressions point1 { get; set; }
		public Expressions point2 { get; set; }

		public MeasureExpression(Expressions point1, Expressions point2)
		{
			this.point1 = point1;
			this.point2 = point2;

		}
	}

	public class Undefined :Expressions
	{
	}

	public class ImportExpression:Expressions
	{
		public List<Expressions> expressions { get; set; }

		public ImportExpression(List<Expressions> expressions)
		{
			this.expressions = expressions;
		}
	}
	public class Context
	{
		public Dictionary<Identifiers, Expressions> Variables;
		public List<MultipleIdentifiers> multipleIdentifiersList;
		public List<Function> functionsList;
		public Context Father;
		public Context(Dictionary<Identifiers, Expressions> variables, Context  father,List<Function> functionsList,List<MultipleIdentifiers> multipleIdentifiersList)
		{
			this.Variables = variables;
			this.Father = father;
			this.multipleIdentifiersList=multipleIdentifiersList;
			this.functionsList=functionsList;
		}
	   

		public Expressions  GetValue(IdentifierExpression identifier)
		{	
			if(Variables.ContainsKey(identifier))   return Variables[identifier];

			if (this.Father == null)
			{
				Errors Error = new Errors($"! SEMANTIC ERROR: The name '{identifier.Identifier.dato}' does not exist in the current context.",identifier.Identifier.line,identifier.Identifier.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			return this.Father.GetValue(identifier);
		}
		
		public void AddIdentifiers(MultipleIdentifiers mult,Expressions seq)
		{
			System.Console.WriteLine("entro");		
			var sequence  = Execute.Evaluator(seq);
			
			if(sequence.Resultado is InfineSequence infineSequence )
			{
				bool defined=true;
				var stard= infineSequence.GetLimit(true);
				if(stard==null) return;
				if(infineSequence.hasEnd)
				{
					var end=infineSequence.GetLimit(false);
					defined= infineSequence.stard<=infineSequence.end;
					if(end==null)   return;	
				}	
				
				for (int j = 0; j < mult.identifiers.Count; j++)
				{
					Expressions current= new Undefined();
					if(infineSequence.MoveNext()&& defined) current=infineSequence.Current;
					if(j==mult.identifiers.Count-1)
					{
						if (mult.identifiers[j].Identifier.dato=="_")continue;   
						if(current is Undefined)
						{
							if(defined)
								Variables.Add(mult.identifiers[j],new Sequence(new List<Expressions>()));
							else Variables.Add(mult.identifiers[j],current);
						}
						else
						{
							var s=infineSequence.Take(j);							
							Variables.Add(mult.identifiers[j],s);
						}
						continue;
					}
					if (mult.identifiers[j].Identifier.dato=="_")continue;
					Variables.Add(mult.identifiers[j],current);
				} 
				return;
			}
			
			if(Lexer.error_list.Count==0 && sequence.Resultado is Sequence sec)
			{  
				for (int j = 0; j < mult.identifiers.Count; j++)
				{
					Expressions c= new Undefined();
					if(sec.MoveNext()) c=sec.Current;
					if(j==mult.identifiers.Count-1)
					{
						if(mult.identifiers[j].Identifier.dato=="_")	continue;
						if(c is Undefined)
							Variables.Add(mult.identifiers[j],new Sequence(new List<Expressions>()));   
						else
						{
							Variables.Add(mult.identifiers[j],sec.Take(j)); 
						}
						continue;
					}
					if(mult.identifiers[j].Identifier.dato=="_")  continue;
					Variables.Add(mult.identifiers[j],c);
				
				}
				return;
			}
			
			if(Lexer.error_list.Count==0 &&	sequence.Resultado is Undefined)
			{
				for (int j = 0; j < mult.identifiers.Count; j++)
				{
					Variables.Add(mult.identifiers[j],new Undefined());
				}
				return;
			}
			Errors Error = new Errors($"! SEMANTIC ERROR: Unexepected {sequence.Type}	Expected Sequence .",0,0);
			Lexer.error_list.Add(Error);
			return;
		}

		internal Function Getfunction(Token dato)
		{
			foreach (var funcion in functionsList)
			{
				if(funcion.NameToken.dato==dato.dato)    return funcion;
			}
			if(Father==null)
			{
				Errors Error = new Errors($"! SEMANTIC ERROR: The name '{dato.dato}' does not exist in the current context.",dato.line,dato.col);
				Lexer.error_list.Add(Error);
				return null;
			}
			return Father.Getfunction(dato);
		}
	}

