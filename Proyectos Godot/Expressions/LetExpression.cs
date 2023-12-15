using System.Collections.Generic;
namespace Godot;

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


