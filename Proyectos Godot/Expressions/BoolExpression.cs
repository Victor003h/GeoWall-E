namespace Godot;

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


