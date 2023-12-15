namespace Godot;

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


