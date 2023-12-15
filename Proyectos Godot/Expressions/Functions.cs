using System.Collections.Generic;
namespace Godot;

public class ReservedFunction : Expressions
{
	private Token function;
	private List<Expressions> child;
	public ReservedFunction(Token function, List<Expressions> child)
	{
		this.function = function;
		this.child = child;

	}
	public Token Function { get => function; set => function = value; }
	public List<Expressions> Child { get => child; set => child = value; }

	public override TokenType Type { get => TokenType.FunctionExpression; set => type = value; }

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